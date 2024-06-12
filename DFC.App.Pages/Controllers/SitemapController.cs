using DFC.App.Pages.Cms.Data.Content;
using DFC.App.Pages.Extensions;
using DFC.App.Pages.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DFC.App.Pages.Controllers
{
    public class SitemapController : Controller
    {
        public const string SitemapViewCanonicalName = "sitemap";

        private readonly ILogger<SitemapController> logger;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private IOptionsMonitor<ContentModeOptions> _options;
        private string status;

        public SitemapController(ILogger<SitemapController> logger, ISharedContentRedisInterface sharedContentRedisInterface, IOptionsMonitor<ContentModeOptions> options)
        {
            this.logger = logger;
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            _options = options;
        }

        [HttpGet]
        [Route("pages/sitemap")]
        public async Task<IActionResult> SitemapView()
        {
            var result = await Sitemap().ConfigureAwait(false);

            return result;
        }

        [HttpGet]
        [Route("/sitemap.xml")]
        public async Task<IActionResult> Sitemap()
        {
            if (_options.CurrentValue.contentMode != null)
            {
                status = _options.CurrentValue.contentMode;
            }
            else
            {
                status = "PUBLISHED";
            }

            try
            {
                logger.LogInformation("Generating Sitemap");

                var sitemapUrlPrefix = $"{Request.GetBaseAddress()}".TrimEnd('/');
                var sitemap = new Sitemap();
                var contentPageModels = await sharedContentRedisInterface.GetDataAsync<SitemapResponse>("SitemapPages/ALL", status);

                if (contentPageModels != null)
                {
                    var contentPageModelsList = contentPageModels.Page?.ToList();

                    if (contentPageModelsList.Any())
                    {
                        var sitemapContentPageModels = contentPageModelsList
                             .Where(w => w.Sitemap?.Exclude == false)
                             .OrderBy(o => o.PageLocation?.urlName);

                        foreach (var contentPageModel in sitemapContentPageModels)
                        {
                            //GraphQL stores Priority as whole number so need to place decimal in front to display proper priority number.
                                sitemap.Add(new SitemapLocation
                                {
                                    Url = $"{sitemapUrlPrefix}{contentPageModel?.PageLocation?.FullUrl}",
                                    Priority = double.Parse("0." + contentPageModel?.Sitemap?.Priority),
                                    ChangeFrequency = contentPageModel?.Sitemap?.ChangeFrequency,
                                });
                        }
                    }
                }

                if (!sitemap.Locations.Any())
                {
                    logger.LogWarning("No locations for sitemap found");
                    return NoContent();
                }

                var xmlString = sitemap.WriteSitemapToString();

                logger.LogInformation("Generated Sitemap");

                return Content(xmlString, MediaTypeNames.Application.Xml);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(Sitemap)}: {ex.Message}");
            }

            return BadRequest();
        }
    }
}