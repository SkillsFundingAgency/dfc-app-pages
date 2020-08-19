using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Extensions;
using DFC.App.Pages.Helpers;
using DFC.App.Pages.Models;
using DFC.App.Pages.ViewModels;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.Pages.Controllers
{
    public class PagesController : Controller
    {
        private const string RegistrationPath = "pages";
        private const string LocalPath = "pages";

        private readonly ILogger<PagesController> logger;
        private readonly IContentPageService<ContentPageModel> contentPageService;
        private readonly AutoMapper.IMapper mapper;
        private readonly IPagesControlerHelpers pagesControlerHelpers;

        public PagesController(ILogger<PagesController> logger, IContentPageService<ContentPageModel> contentPageService, AutoMapper.IMapper mapper, IPagesControlerHelpers pagesControlerHelpers)
        {
            this.logger = logger;
            this.contentPageService = contentPageService;
            this.mapper = mapper;
            this.pagesControlerHelpers = pagesControlerHelpers;
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

                logger.LogInformation($"{nameof(Index)} has succeeded");
            }
            else
            {
                logger.LogWarning($"{nameof(Index)} has returned with no results");
            }

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("pages/document")]
        [Route("pages/{location1}/document")]
        [Route("pages/{location1}/{location2}/document")]
        [Route("pages/{location1}/{location2}/{location3}/document")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/document")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/{location5}/document")]
        public async Task<IActionResult> Document(PageRequestModel pageRequestModel)
        {
            var (location, article) = PagesControlerHelpers.ExtractPageLocation(pageRequestModel);
            var contentPageModel = await pagesControlerHelpers.GetContentPageAsync(location, article).ConfigureAwait(false);

            if (contentPageModel != null)
            {
                var viewModel = mapper.Map<DocumentViewModel>(contentPageModel);
                viewModel.Breadcrumb = mapper.Map<BreadcrumbViewModel>(contentPageModel);

                if (viewModel.Breadcrumb?.Breadcrumbs != null && viewModel.Breadcrumb.Breadcrumbs.Any())
                {
                    foreach (var breadcrumb in viewModel.Breadcrumb.Breadcrumbs)
                    {
                        var route = breadcrumb.Route == "/" ? string.Empty : breadcrumb.Route;
                        breadcrumb.Route = $"/pages{route}/document";
                    }

                    viewModel.Breadcrumb.Breadcrumbs.Insert(0, new BreadcrumbItemViewModel { Route = "/", Title = "[ Index ]" });
                }

                logger.LogInformation($"{nameof(Document)} has succeeded for: /{location}/{article}");

                return this.NegotiateContentResult(viewModel);
            }

            var redirectedContentPageModel = await pagesControlerHelpers.GetRedirectedContentPageAsync(location, article).ConfigureAwait(false);

            if (redirectedContentPageModel != null)
            {
                var redirectedUrl = $"{Request.GetBaseAddress()}{LocalPath}{redirectedContentPageModel.PageLocation}";
                if (redirectedContentPageModel.PageLocation != "/")
                {
                    redirectedUrl += "/";
                }

                redirectedUrl += $"{redirectedContentPageModel.CanonicalName}/document";

                logger.LogWarning($"{nameof(Document)} has been redirected for: /{location}/{article} to {redirectedUrl}");

                return RedirectPermanent(redirectedUrl);
            }

            logger.LogWarning($"{nameof(Document)} has returned no content for: /{location}/{article}");

            return NoContent();
        }

        [HttpGet]
        [Route("pages/htmlhead")]
        [Route("pages/{location1}/htmlhead")]
        [Route("pages/{location1}/{location2}/htmlhead")]
        [Route("pages/{location1}/{location2}/{location3}/htmlhead")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/htmlhead")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/{location5}/htmlhead")]
        public async Task<IActionResult> HtmlHead(PageRequestModel pageRequestModel)
        {
            var (location, article) = PagesControlerHelpers.ExtractPageLocation(pageRequestModel);
            var viewModel = new HtmlHeadViewModel();
            var contentPageModel = await pagesControlerHelpers.GetContentPageAsync(location, article).ConfigureAwait(false);

            if (contentPageModel != null)
            {
                mapper.Map(contentPageModel, viewModel);

                viewModel.CanonicalUrl = new Uri($"{Request.GetBaseAddress()}{RegistrationPath}/{contentPageModel.CanonicalName}", UriKind.RelativeOrAbsolute);
            }

            logger.LogInformation($"{nameof(HtmlHead)} has returned content for: /{location}/{article}");

            return this.NegotiateContentResult(viewModel);
        }

        [Route("pages/breadcrumb")]
        [Route("pages/{location1}/breadcrumb")]
        [Route("pages/{location1}/{location2}/breadcrumb")]
        [Route("pages/{location1}/{location2}/{location3}/breadcrumb")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/breadcrumb")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/{location5}/breadcrumb")]
        public async Task<IActionResult> Breadcrumb(PageRequestModel pageRequestModel)
        {
            var (location, article) = PagesControlerHelpers.ExtractPageLocation(pageRequestModel);
            var contentPageModel = await pagesControlerHelpers.GetContentPageAsync(location, article).ConfigureAwait(false);

            if (contentPageModel == null)
            {
                return NoContent();
            }

            var viewModel = mapper.Map<BreadcrumbViewModel>(contentPageModel);

            logger.LogInformation($"{nameof(Breadcrumb)} has returned content for: /{location}/{article}");

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("pages/bodytop")]
        [Route("pages/{location1}/bodytop")]
        [Route("pages/{location1}/{location2}/bodytop")]
        [Route("pages/{location1}/{location2}/{location3}/bodytop")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/bodytop")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/{location5}/bodytop")]
        public IActionResult BodyTop(PageRequestModel pageRequestModel)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/herobanner")]
        [Route("pages/{location1}/herobanner")]
        [Route("pages/{location1}/{location2}/herobanner")]
        [Route("pages/{location1}/{location2}/{location3}/herobanner")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/herobanner")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/{location5}/herobanner")]
        public IActionResult HeroBanner(PageRequestModel pageRequestModel)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/body")]
        [Route("pages/{location1}/body")]
        [Route("pages/{location1}/{location2}/body")]
        [Route("pages/{location1}/{location2}/{location3}/body")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/body")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/{location5}/body")]
        public async Task<IActionResult> Body(PageRequestModel pageRequestModel)
        {
            var (location, article) = PagesControlerHelpers.ExtractPageLocation(pageRequestModel);
            var viewModel = new BodyViewModel();
            var contentPageModel = await pagesControlerHelpers.GetContentPageAsync(location, article).ConfigureAwait(false);

            if (contentPageModel != null)
            {
                mapper.Map(contentPageModel, viewModel);
                logger.LogInformation($"{nameof(Body)} has returned content for: /{location}/{article}");

                return this.NegotiateContentResult(viewModel, contentPageModel);
            }

            var redirectedContentPageModel = await pagesControlerHelpers.GetRedirectedContentPageAsync(location, article).ConfigureAwait(false);

            if (redirectedContentPageModel != null)
            {
                var pageLocation = redirectedContentPageModel.PageLocation;
                if (pageLocation != null && pageLocation.StartsWith("/", StringComparison.Ordinal))
                {
                    pageLocation = pageLocation.Substring(1);
                }

                var redirectedUrl = $"{Request.GetBaseAddress()}{pageLocation}/{redirectedContentPageModel.CanonicalName}";
                logger.LogWarning($"{nameof(Document)} has been redirected for: /{location}/{article} to {redirectedUrl}");

                return RedirectPermanent(redirectedUrl);
            }

            logger.LogWarning($"{nameof(Body)} has not returned any content for: /{location}/{article}");
            return NotFound();
        }

        [HttpGet]
        [Route("pages/sidebarright")]
        [Route("pages/{location1}/sidebarright")]
        [Route("pages/{location1}/{location2}/sidebarright")]
        [Route("pages/{location1}/{location2}/{location3}/bosidebarrightdy")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/sidebarright")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/{location5}/sidebarright")]
        public IActionResult SidebarRight(PageRequestModel pageRequestModel)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/sidebarleft")]
        [Route("pages/{location1}/sidebarleft")]
        [Route("pages/{location1}/{location2}/sidebarleft")]
        [Route("pages/{location1}/{location2}/{location3}/sidebarleft")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/sidebarleft")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/{location5}/sidebarleft")]
        public IActionResult SidebarLeft(PageRequestModel pageRequestModel)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/bodyfooter")]
        [Route("pages/{location1}/bodyfooter")]
        [Route("pages/{location1}/{location2}/bodyfooter")]
        [Route("pages/{location1}/{location2}/{location3}/bodyfooter")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/bodyfooter")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/{location5}/bodyfooter")]
        public IActionResult BodyFooter(PageRequestModel pageRequestModel)
        {
            return NoContent();
        }
    }
}