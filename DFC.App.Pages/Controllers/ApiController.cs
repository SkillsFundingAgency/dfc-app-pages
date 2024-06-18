using AutoMapper;
using DFC.App.Pages.Cms.Data.Content;
using DFC.App.Pages.Models.Api;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.Pages.Controllers
{
    public class ApiController : Controller
    {
        private const string expiryAppSettings = "Cms:Expiry";
        private readonly IConfiguration configuration;
        private readonly ILogger<ApiController> logger;
        private readonly IMapper mapper;
        private ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IOptionsMonitor<ContentModeOptions> options;
        private string status = string.Empty;
        private double expiryInHours = 4;

        public ApiController(IConfiguration configuration, ILogger<ApiController> logger, IMapper mapper, ISharedContentRedisInterface sharedContentRedisInterface, IOptionsMonitor<ContentModeOptions> options)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.mapper = mapper;
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            this.options = options;

            if (this.configuration != null)
            {
                string expiryAppString = this.configuration.GetSection(expiryAppSettings).Get<string>();
                if (double.TryParse(expiryAppString, out var expiryAppStringParseResult))
                {
                    expiryInHours = expiryAppStringParseResult;
                }
            }
        }

        [HttpGet]
        [Route("api/pages")]
        public async Task<IActionResult> Index()
        {
            if (options.CurrentValue.contentMode != null)
            {
                status = options.CurrentValue.contentMode;
            }
            else
            {
                status = "PUBLISHED";
            }

            logger.LogInformation($"{nameof(Index)} has been called");

            var pages = new Dictionary<Guid, GetIndexModel>();

            var contentPageModels = await sharedContentRedisInterface.GetDataAsyncWithExpiry<PageApiResponse>("PagesApi/All", status, expiryInHours);

            var contentPageModelsList = contentPageModels?.Page.ToList();

            if (contentPageModelsList != null && contentPageModelsList.Any())
            {
                pages = (from a in contentPageModelsList.OrderBy(o => o.PageLocation?.FullUrl).ThenBy(o => o.DisplayText)
                         select mapper.Map<GetIndexModel>(a)).ToDictionary(x => x.Id);
                logger.LogInformation($"{nameof(Index)} has succeeded");
            }
            else
            {
                logger.LogWarning($"{nameof(Index)} has returned with no results");
            }

            return Ok(pages);
        }

        [HttpGet]
        [Route("api/pages/{id}")]
        public async Task<IActionResult> Document(Guid id)
        {
            if (options.CurrentValue.contentMode != null)
            {
                status = options.CurrentValue.contentMode;
            }
            else
            {
                status = "PUBLISHED";
            }

            logger.LogInformation($"{nameof(Document)} has been called");

            var contentPageModel = await sharedContentRedisInterface.GetDataAsyncWithExpiry<GetByPageApiResponse>("PageApi" + "/" + id, status, expiryInHours);

            if (contentPageModel != null)
            {
                var contentPage = contentPageModel?.Page;

                PageApi page = new PageApi()
                {
                    DisplayText = contentPage?.FirstOrDefault()?.DisplayText,
                    GraphSync = contentPage?.FirstOrDefault()?.GraphSync,
                    PageLocation = contentPage?.FirstOrDefault()?.PageLocation,
                };

                var getIndexModel = mapper.Map<GetIndexModel>(page);
                logger.LogInformation($"{nameof(Document)} has succeeded");
                return Ok(getIndexModel);
            }
            else
            {
                logger.LogWarning($"{nameof(Document)} has returned with no content");
                return NoContent();
            }
        }
    }
}
