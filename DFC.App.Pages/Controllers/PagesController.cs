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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.Pages.Controllers
{
    public class PagesController : Controller
    {
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
            logger.LogInformation($"{nameof(Index)} has been called");

            var viewModel = new IndexViewModel()
            {
                LocalPath = LocalPath,
                Documents = new List<IndexDocumentViewModel>
                {
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
            logger.LogInformation($"{nameof(Document)} has been called");


            var (location, article) = PagesControlerHelpers.ExtractPageLocation(pageRequestModel);
            var contentPageModel = await pagesControlerHelpers.GetContentPageAsync(location, article).ConfigureAwait(false);

            if (contentPageModel != null)
            {
                var viewModel = mapper.Map<DocumentViewModel>(contentPageModel);

                if (contentPageModel.ShowBreadcrumb)
                {
                    viewModel.Breadcrumb = mapper.Map<BreadcrumbViewModel>(contentPageModel);

                    if (viewModel.Breadcrumb?.Breadcrumbs != null && viewModel.Breadcrumb.Breadcrumbs.Any())
                    {
                        foreach (var breadcrumb in viewModel.Breadcrumb.Breadcrumbs)
                        {
                            var route = breadcrumb.Route == "/" ? string.Empty : breadcrumb.Route;
                            breadcrumb.Route = $"/pages{route}/document";
                        }
                    }
                }
                else
                {
                    viewModel.Breadcrumb = new BreadcrumbViewModel
                    {
                        Breadcrumbs = new List<BreadcrumbItemViewModel>(),
                    };
                }

                viewModel.Breadcrumb.Breadcrumbs.Insert(0, new BreadcrumbItemViewModel { Route = "/", Title = "[ Index ]" });

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
        [Route("pages/head")]
        [Route("pages/{location1}/head")]
        [Route("pages/{location1}/{location2}/head")]
        [Route("pages/{location1}/{location2}/{location3}/head")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/head")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/{location5}/head")]
        public async Task<IActionResult> Head(PageRequestModel pageRequestModel)
        {
            logger.LogInformation($"{nameof(Head)} has been called");


            var (location, article) = PagesControlerHelpers.ExtractPageLocation(pageRequestModel);
            var viewModel = new HeadViewModel();
            var contentPageModel = await pagesControlerHelpers.GetContentPageAsync(location, article).ConfigureAwait(false);

            if (contentPageModel != null)
            {
                mapper.Map(contentPageModel, viewModel);
                viewModel.CanonicalUrl = BuildCanonicalUrl(contentPageModel);
                logger.LogInformation($"{nameof(Head)} has succeeded for: /{location}/{article}");
            }
            else
            {
                logger.LogInformation($"{nameof(Head)} has returned no content for: /{location}/{article}");
            }

            return this.NegotiateContentResult(viewModel);
        }

        private Uri BuildCanonicalUrl(ContentPageModel contentPageModel)
        {
            var pathDirectory1 = contentPageModel.PageLocation?[1..] ?? string.Empty;
            var pathDirectory2 = contentPageModel.CanonicalName ?? string.Empty;

            var uriString = Path.Combine(
                Request.GetBaseAddress()?.ToString() ?? string.Empty,
                pathDirectory1,
                pathDirectory2 == pathDirectory1 ? string.Empty : pathDirectory2);

            return new Uri(uriString, UriKind.RelativeOrAbsolute);
        }

        [Route("pages/breadcrumb")]
        [Route("pages/{location1}/breadcrumb")]
        [Route("pages/{location1}/{location2}/breadcrumb")]
        [Route("pages/{location1}/{location2}/{location3}/breadcrumb")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/breadcrumb")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/{location5}/breadcrumb")]
        public async Task<IActionResult> Breadcrumb(PageRequestModel pageRequestModel)
        {
            logger.LogInformation($"{nameof(Breadcrumb)} has been called");

            var (location, article) = PagesControlerHelpers.ExtractPageLocation(pageRequestModel);
            var contentPageModel = await pagesControlerHelpers.GetContentPageAsync(location, article).ConfigureAwait(false);

            if (contentPageModel == null || !contentPageModel.ShowBreadcrumb)
            {
                logger.LogInformation($"{nameof(Breadcrumb)} Breadcrumb disabled or no content found for: /{location}/{article}");

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
            logger.LogWarning($"{nameof(BodyTop)} has returned no content");

            return NoContent();
        }

        [HttpGet]
        [Route("pages/herobanner")]
        [Route("pages/{location1}/herobanner")]
        [Route("pages/{location1}/{location2}/herobanner")]
        [Route("pages/{location1}/{location2}/{location3}/herobanner")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/herobanner")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/{location5}/herobanner")]
        public async Task<IActionResult> HeroBanner(PageRequestModel pageRequestModel)
        {
            logger.LogInformation($"{nameof(HeroBanner)} has been called");

            var (location, article) = PagesControlerHelpers.ExtractPageLocation(pageRequestModel);
            var contentPageModel = await pagesControlerHelpers.GetContentPageAsync(location, article).ConfigureAwait(false);

            if (contentPageModel == null)
            {
                logger.LogInformation($"{nameof(HeroBanner)} found no content for: /{location}/{article}");

                return NoContent();
            }

            var viewModel = mapper.Map<HeroBannerViewModel>(contentPageModel);

            logger.LogInformation($"{nameof(HeroBanner)} has returned content for: /{location}/{article}");

            return this.NegotiateContentResult(viewModel);
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
            logger.LogInformation($"{nameof(Body)} has been called");


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
                var pageLocation = $"{Request.GetBaseAddress()}".TrimEnd('/') + redirectedContentPageModel.PageLocation.TrimEnd('/');
                var redirectedUrl = $"{pageLocation}/{redirectedContentPageModel.CanonicalName}";
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
        [Route("pages/{location1}/{location2}/{location3}/sidebarright")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/sidebarright")]
        [Route("pages/{location1}/{location2}/{location3}/{location4}/{location5}/sidebarright")]
        public IActionResult SidebarRight(PageRequestModel pageRequestModel)
        {
            logger.LogWarning($"{nameof(SidebarRight)} has returned no content");

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
            logger.LogWarning($"{nameof(SidebarLeft)} has returned no content");

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
            logger.LogWarning($"{nameof(BodyFooter)} has returned no content");

            return NoContent();
        }
    }
}