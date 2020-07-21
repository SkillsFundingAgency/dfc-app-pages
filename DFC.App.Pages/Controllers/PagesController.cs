using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Extensions;
using DFC.App.Pages.Models;
using DFC.App.Pages.ViewModels;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DFC.App.Pages.Controllers
{
    public class PagesController : BasePagesController<PagesController>
    {
        private readonly IContentPageService<ContentPageModel> contentPageService;
        private readonly AutoMapper.IMapper mapper;

        public PagesController(ILogger<PagesController> logger, IContentPageService<ContentPageModel> contentPageService, AutoMapper.IMapper mapper) : base(logger)
        {
            this.contentPageService = contentPageService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("/")]
        [Route("pages")]
        public async Task<IActionResult> Index()
        {
            var viewModel = new IndexViewModel()
            {
                LocalPath = LocalPath,
                Documents = new List<IndexDocumentViewModel>
                {
                    new IndexDocumentViewModel { CanonicalName = HomeController.ThisViewCanonicalName, PageLocation = "/" },
                    new IndexDocumentViewModel { CanonicalName = HealthController.HealthViewCanonicalName },
                    new IndexDocumentViewModel { CanonicalName = SitemapController.SitemapViewCanonicalName },
                    new IndexDocumentViewModel { CanonicalName = RobotController.RobotsViewCanonicalName },
                },
            };
            var contentPageModels = await contentPageService.GetAllAsync().ConfigureAwait(false);

            if (contentPageModels != null)
            {
                var documents = from a in contentPageModels.OrderBy(o => o.PageLocation).ThenBy(o => o.CanonicalName)
                                select mapper.Map<IndexDocumentViewModel>(a);

                viewModel.Documents.AddRange(documents);

                Logger.LogInformation($"{nameof(Index)} has succeeded");
            }
            else
            {
                Logger.LogWarning($"{nameof(Index)} has returned with no results");
            }

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("pages/{location}/{article}")]
        [Route("pages/{location}")]
        public async Task<IActionResult> Document(string location, string? article)
        {
            var contentPageModel = await GetContentPageAsync(location, article).ConfigureAwait(false);

            if (contentPageModel != null)
            {
                var viewModel = mapper.Map<DocumentViewModel>(contentPageModel);
                var breadcrumbItemModel = mapper.Map<BreadcrumbItemModel>(contentPageModel);

                viewModel.HtmlHead = mapper.Map<HtmlHeadViewModel>(contentPageModel);
                viewModel.Breadcrumb = BuildBreadcrumb(LocalPath, breadcrumbItemModel);

                Logger.LogInformation($"{nameof(Document)} has succeeded for: {article}");

                return this.NegotiateContentResult(viewModel);
            }

            if (!string.IsNullOrWhiteSpace(article))
            {
                var redirectedContentPageModel = await GetRedirectedContentPageAsync(location, article).ConfigureAwait(false);

                if (redirectedContentPageModel != null)
                {
                    var redirectedUrlateUrl = $"{Request.GetBaseAddress()}{LocalPath}/{location}/{redirectedContentPageModel.CanonicalName}";
                    Logger.LogWarning($"{nameof(Document)} has been redirected for: /{location}/{article} to {redirectedUrlateUrl}");

                    return RedirectPermanent(redirectedUrlateUrl);
                }
            }

            Logger.LogWarning($"{nameof(Document)} has returned no content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("pages/{location}/{article}/htmlhead")]
        public async Task<IActionResult> HtmlHead(string location, string article)
        {
            var viewModel = new HtmlHeadViewModel();
            var contentPageModel = await GetContentPageAsync(location, article).ConfigureAwait(false);

            if (contentPageModel != null)
            {
                mapper.Map(contentPageModel, viewModel);

                viewModel.CanonicalUrl = new Uri($"{Request.GetBaseAddress()}{RegistrationPath}/{contentPageModel.CanonicalName}", UriKind.RelativeOrAbsolute);
            }

            Logger.LogInformation($"{nameof(HtmlHead)} has returned content for: {article}");

            return this.NegotiateContentResult(viewModel);
        }

        [Route("pages/{location}/{article}/breadcrumb")]
        public async Task<IActionResult> Breadcrumb(string location, string article)
        {
            var contentPageModel = await GetContentPageAsync(location, article).ConfigureAwait(false);
            var breadcrumbItemModel = mapper.Map<BreadcrumbItemModel>(contentPageModel);
            var viewModel = BuildBreadcrumb(RegistrationPath, breadcrumbItemModel);

            Logger.LogInformation($"{nameof(Breadcrumb)} has returned content for: {article}");

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("pages/{location}/{article}/bodytop")]
        public IActionResult BodyTop(string location, string article)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/{location}/{article}/herobanner")]
        public IActionResult HeroBanner(string location, string article)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/{location}/{article}/body")]
        public async Task<IActionResult> Body(string location, string article)
        {
            var viewModel = new BodyViewModel();
            var contentPageModel = await GetContentPageAsync(location, article).ConfigureAwait(false);

            if (contentPageModel != null)
            {
                mapper.Map(contentPageModel, viewModel);
                Logger.LogInformation($"{nameof(Body)} has returned content for: {article}");

                return this.NegotiateContentResult(viewModel, contentPageModel);
            }

            if (!string.IsNullOrWhiteSpace(article))
            {
                var redirectedContentPageModel = await GetRedirectedContentPageAsync(location, article).ConfigureAwait(false);

                if (redirectedContentPageModel != null)
                {
                    var redirectedUrl = $"{Request.GetBaseAddress()}{RegistrationPath}/{location}/{redirectedContentPageModel.CanonicalName}";
                    Logger.LogWarning($"{nameof(Document)} has been redirected for: /{location}/{article} to {redirectedUrl}");

                    return RedirectPermanent(redirectedUrl);
                }
            }

            Logger.LogWarning($"{nameof(Body)} has not returned any content for: {article}");
            return NotFound();
        }

        [HttpGet]
        [Route("pages/{location}/{article}/sidebarright")]
        public IActionResult SidebarRight(string location, string article)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/{location}/{article}/sidebarleft")]
        public IActionResult SidebarLeft(string location, string article)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/{location}/{article}/bodyfooter")]
        public IActionResult BodyFooter(string location, string article)
        {
            return NoContent();
        }

        #region Define helper methods

        private async Task<ContentPageModel?> GetContentPageAsync(string location, string? article)
        {
            Expression<Func<ContentPageModel, bool>> where;

            if (string.IsNullOrWhiteSpace(article))
            {
                where = p => p.PageLocation == "/" + location && p.IsDefaultForPageLocation;
            }
            else
            {
                where = p => p.PageLocation == "/" + location && p.CanonicalName == article.ToLowerInvariant();
            }

            var contentPageModels = await contentPageService.GetAsync(where).ConfigureAwait(false);

            if (contentPageModels != null && contentPageModels.Any())
            {
                return contentPageModels.First();
            }

            return default;
        }

        private async Task<ContentPageModel?> GetRedirectedContentPageAsync(string location, string article)
        {
            var contentPageModel = await contentPageService.GetByRedirectLocationAsync($"/{location}/{article}").ConfigureAwait(false);

            return contentPageModel;
        }

        #endregion Define helper methods
    }
}