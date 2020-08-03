using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Extensions;
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
                    new IndexDocumentViewModel { CanonicalName = HomeController.HomeViewCanonicalName, PageLocation = "/" },
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
        [Route("pages/{location}/{article}/document")]
        [Route("pages/{location}/document")]
        [Route("pages/document")]
        public async Task<IActionResult> Document(string? location, string? article)
        {
            var contentPageModel = await GetContentPageAsync(location, article).ConfigureAwait(false);

            if (contentPageModel != null)
            {
                var viewModel = mapper.Map<DocumentViewModel>(contentPageModel);

                Logger.LogInformation($"{nameof(Document)} has succeeded for: {article}");

                return this.NegotiateContentResult(viewModel);
            }

            var redirectedContentPageModel = await GetRedirectedContentPageAsync(location, article).ConfigureAwait(false);

            if (redirectedContentPageModel != null)
            {
                var redirectedUrlateUrl = $"{Request.GetBaseAddress()}{LocalPath}{redirectedContentPageModel.PageLocation}";
                if (redirectedContentPageModel.PageLocation != "/")
                {
                    redirectedUrlateUrl += "/";
                }

                redirectedUrlateUrl += $"{redirectedContentPageModel.CanonicalName}/document";

                Logger.LogWarning($"{nameof(Document)} has been redirected for: /{location}/{article} to {redirectedUrlateUrl}");

                return RedirectPermanent(redirectedUrlateUrl);
            }

            Logger.LogWarning($"{nameof(Document)} has returned no content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("pages/{location}/{article}/htmlhead")]
        [Route("pages/{location}/htmlhead")]
        [Route("pages/htmlhead")]
        public async Task<IActionResult> HtmlHead(string? location, string? article)
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
        [Route("pages/{location}/breadcrumb")]
        [Route("pages/breadcrumb")]
        public async Task<IActionResult> Breadcrumb(string? location, string? article)
        {
            var contentPageModel = await GetContentPageAsync(location, article).ConfigureAwait(false);
            var viewModel = mapper.Map<List<BreadcrumbItemViewModel>>(contentPageModel);

            Logger.LogInformation($"{nameof(Breadcrumb)} has returned content for: {article}");

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("pages/{location}/{article}/bodytop")]
        [Route("pages/{location}/bodytop")]
        [Route("pages/bodytop")]
        public IActionResult BodyTop(string? location, string? article)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/{location}/{article}/herobanner")]
        [Route("pages/{location}/herobanner")]
        [Route("pages/herobanner")]
        public IActionResult HeroBanner(string? location, string? article)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/{location}/{article}/body")]
        [Route("pages/{location}/body")]
        [Route("pages/body")]
        public async Task<IActionResult> Body(string? location, string? article)
        {
            var viewModel = new BodyViewModel();
            var contentPageModel = await GetContentPageAsync(location, article).ConfigureAwait(false);

            if (contentPageModel != null)
            {
                mapper.Map(contentPageModel, viewModel);
                Logger.LogInformation($"{nameof(Body)} has returned content for: {article}");

                return this.NegotiateContentResult(viewModel, contentPageModel);
            }

            var redirectedContentPageModel = await GetRedirectedContentPageAsync(location, article).ConfigureAwait(false);

            if (redirectedContentPageModel != null)
            {
                var pageLocation = redirectedContentPageModel.PageLocation;
                if (pageLocation != null && pageLocation.StartsWith("/", StringComparison.Ordinal))
                {
                    pageLocation = pageLocation.Substring(1);
                }

                var redirectedUrl = $"{Request.GetBaseAddress()}{pageLocation}/{redirectedContentPageModel.CanonicalName}";
                Logger.LogWarning($"{nameof(Document)} has been redirected for: /{location}/{article} to {redirectedUrl}");

                return RedirectPermanent(redirectedUrl);
            }

            Logger.LogWarning($"{nameof(Body)} has not returned any content for: {article}");
            return NotFound();
        }

        [HttpGet]
        [Route("pages/{location}/{article}/sidebarright")]
        [Route("pages/{location}/sidebarright")]
        [Route("pages/sidebarright")]
        public IActionResult SidebarRight(string? location, string? article)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/{location}/{article}/sidebarleft")]
        [Route("pages/{location}/sidebarleft")]
        [Route("pages/sidebarleft")]
        public IActionResult SidebarLeft(string? location, string? article)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/{location}/{article}/bodyfooter")]
        [Route("pages/{location}/bodyfooter")]
        [Route("pages/bodyfooter")]
        public IActionResult BodyFooter(string? location, string? article)
        {
            return NoContent();
        }

        #region Define helper methods

        private async Task<ContentPageModel?> GetContentPageAsync(string? location, string? article)
        {
            Expression<Func<ContentPageModel, bool>> where;

            if (string.IsNullOrWhiteSpace(location) && string.IsNullOrWhiteSpace(article))
            {
                where = p => p.PageLocation == "/" && p.IsDefaultForPageLocation;
            }
            else if (string.IsNullOrWhiteSpace(article))
            {
                where = p => (p.PageLocation == $"/{location}" && p.IsDefaultForPageLocation) || (p.PageLocation == "/" && p.CanonicalName == location);
            }
            else
            {
                where = p => p.PageLocation == $"/{location}" && p.CanonicalName == article;
            }

            var contentPageModels = await contentPageService.GetAsync(where).ConfigureAwait(false);

            if (contentPageModels != null && contentPageModels.Any())
            {
                return contentPageModels.First();
            }

            return default;
        }

        private async Task<ContentPageModel?> GetRedirectedContentPageAsync(string? location, string? article)
        {
            var redirectLocation = $"/{location}";

            if (!string.IsNullOrWhiteSpace(article))
            {
                redirectLocation += $"/{article}";
            }

            var contentPageModel = await contentPageService.GetByRedirectLocationAsync(redirectLocation).ConfigureAwait(false);

            return contentPageModel;
        }

        #endregion Define helper methods
    }
}