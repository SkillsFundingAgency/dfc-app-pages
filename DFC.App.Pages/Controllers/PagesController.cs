using DFC.App.Pages.Cms.Data.Interface;
using DFC.App.Pages.Cms.Data.Model;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Extensions;
using DFC.App.Pages.Helpers;
using DFC.App.Pages.Models;
using DFC.App.Pages.ViewModels;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentPageModel = DFC.App.Pages.Data.Models.ContentPageModel;

namespace DFC.App.Pages.Controllers
{
   
    public class PagesController : Controller
    {
        private const string LocalPath = "pages";

        private readonly ILogger<PagesController> logger;
        private readonly IContentPageService<ContentPageModel> contentPageService;
        private readonly AutoMapper.IMapper mapper;
        private readonly IPagesControlerHelpers pagesControlerHelpers;
        private readonly IPageService pageService;

        public PagesController(ILogger<PagesController> logger, IContentPageService<ContentPageModel> contentPageService, AutoMapper.IMapper mapper, IPagesControlerHelpers pagesControlerHelpers, IPageService pageService)
        {
            this.logger = logger;
            this.contentPageService = contentPageService;
            this.mapper = mapper;
            this.pagesControlerHelpers = pagesControlerHelpers;
            this.pageService = pageService;
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

            var pageUrlResponse = await this.pageService.GetPageUrls();
            viewModel.Documents.AddRange(pageUrlResponse.OrderBy(o => o.PageLocation.UrlName).Select(a => mapper.Map<IndexDocumentViewModel>(a)));
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

            string pageUrl = GetPageUrl(location, article);
            var pageResponse = await this.pageService.GetPage(pageUrl);

            if (pageResponse.FirstOrDefault() != null)
            {
                var viewModel = mapper.Map<DocumentViewModel>(pageResponse.FirstOrDefault());

                if (pageResponse.FirstOrDefault().ShowBreadcrumb)
                {
                    viewModel.Breadcrumb = await GetBreadcrumb(location, article);

                    if (viewModel.Breadcrumb?.Breadcrumbs != null && viewModel.Breadcrumb.Breadcrumbs.Any())
                    {
                        foreach (var breadcrumb in viewModel.Breadcrumb.Breadcrumbs)
                        {
                            //logger.LogInformation($"sent the ting");
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
            string pageUrl = GetPageUrl(location, article);
            var pageResponse = await this.pageService.GetPage(pageUrl);

            var viewModel = new HeadViewModel();

            if (pageResponse != null && pageResponse.Count > 0)
            {
                mapper.Map(pageResponse.First(), viewModel);
                viewModel.CanonicalUrl = BuildCanonicalUrl(pageResponse.FirstOrDefault());
                //logger.LogInformation($"{nameof(Head)} has succeeded for: /{location}/{article}");
            }
            /* else
             {
                 logger.LogInformation($"{nameof(Head)} has returned no content for: /{location}/{article}");
             }*/

            return this.NegotiateContentResult(viewModel);

        }

        private Uri BuildCanonicalUrl(Page pageModel)
        {
            var pathDirectory1 = pageModel.PageLocation.UrlName ?? string.Empty;

            var uriString = Path.Combine(
                Request.GetBaseAddress()?.ToString() ?? string.Empty,
                pathDirectory1);

            return new Uri(uriString, UriKind.RelativeOrAbsolute);
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
            var breadcrumbResponse = await GetBreadcrumb(location, article);

            if (breadcrumbResponse == null)
            {
                //logger.LogInformation($"{nameof(Breadcrumb)} Breadcrumb disabled or no content found for: /{location}/{article}");

                return NoContent();
            }

          

            //logger.LogInformation($"{nameof(Breadcrumb)} has returned content for: /{location}/{article}");



            return this.NegotiateContentResult(breadcrumbResponse);
        }

        public string GetPageUrl(string location, string article)
        {
            string pageUrl = string.Empty;

            if (string.IsNullOrEmpty(location) && string.IsNullOrWhiteSpace(article))
            {
                pageUrl = "/home";
            }
            else if (location == "home")
            {
                pageUrl = $"/{location}";
            }
            else
            {
                pageUrl = $"/{location}/{(string.IsNullOrWhiteSpace(article) ? location : article)}";
            }
            return pageUrl;
        }

        private async Task<BreadcrumbViewModel> GetBreadcrumb(string location, string article)
        {
            var breadcrumbResponse = await this.pageService.GetBreadCrumbs("PageLocation");
            string pageUrl = GetPageUrl(location, article);
            var pageResponse = await this.pageService.GetPage(pageUrl);

            if ((pageResponse == null || pageResponse.Count == 0) || !pageResponse.FirstOrDefault().ShowBreadcrumb)
            {
                return null;
            }

            string breadCrumbDisplayText = pageResponse.FirstOrDefault().Breadcrumb.TermContentItems.FirstOrDefault().DisplayText;
            var jdoc = JObject.Parse(breadcrumbResponse.Content);
            var root = jdoc.SelectToken("$.TaxonomyPart.Terms");
            var token = root.SelectTokens($"$..Terms[?(@.DisplayText == '{breadCrumbDisplayText}')]");
            var path = token.FirstOrDefault().Path;
            var result = BuildBreadCrumb(path, jdoc);

            if (!pageResponse.FirstOrDefault().PageLocation.DefaultPageForLocation && !string.IsNullOrWhiteSpace(pageResponse.FirstOrDefault().DisplayText))
            {
                var articlePathViewModel = new BreadcrumbItemViewModel
                {
                    Route = $"{pageResponse.FirstOrDefault().Breadcrumb.TermContentItems.FirstOrDefault().DisplayText}",
                    Title = pageResponse.FirstOrDefault().DisplayText,
                    AddHyperlink = false,
                };
                result.Breadcrumbs.Add(articlePathViewModel);
            }
            return result;
        }

        private BreadcrumbViewModel BuildBreadCrumb(string path, JObject jdoc)
        {
            StringBuilder breadCrumbText = new StringBuilder();
            BreadcrumbViewModel breadCrumbs = new BreadcrumbViewModel()
            {
                Breadcrumbs = new List<BreadcrumbItemViewModel>()
            };

            int index = 0;
            do
            {
                var breadCrumbPath = $"{path}.PageLocation.BreadcrumbText.Text";
                var displayTextPath = $"{path}.DisplayText";
                var breadCrumbToken = jdoc.SelectToken(breadCrumbPath);
                var displayTextToken = jdoc.SelectToken(displayTextPath);
                breadCrumbs.Breadcrumbs.Add(new BreadcrumbItemViewModel()
                {
                    Route = displayTextToken.ToString(),
                    Title = breadCrumbToken.ToString(),
                });
                index = path.LastIndexOf(".");
                path = path.Remove(index);
            } while (path.LastIndexOf(".") > 0);

            breadCrumbs.Breadcrumbs.Reverse();
            return breadCrumbs;
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
            string pageUrl = GetPageUrl(location, article);
            var pageResponse = await this.pageService.GetPage(pageUrl);


            if (pageResponse == null || pageResponse.Count == 0)
            {
                //logger.LogInformation($"{nameof(HeroBanner)} found no content for: /{location}/{article}");

                return NoContent();
            }

            var viewModel = mapper.Map<HeroBannerViewModel>(pageResponse.FirstOrDefault());

            //logger.LogInformation($"{nameof(HeroBanner)} has returned content for: /{location}/{article}");

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
            string pageUrl = GetPageUrl(location, article);
            var pageResponse = await this.pageService.GetPage(pageUrl);
            var viewModel = new BodyViewModel();

            if (pageResponse != null && pageResponse.Count == 0)
            {
                mapper.Map(pageResponse.FirstOrDefault(), viewModel);
                //logger.LogInformation($"{nameof(Body)} has returned content for: /{location}/{article}");

                return this.NegotiateContentResult(viewModel);
            }

            var redirectedContentPageModel = await this.pageService.GetPageUrls();
            var filterList = redirectedContentPageModel.Where(ctr => (ctr.PageLocation.RedirectLocations ?? "").Split("\r\n").Contains(pageUrl)).ToList();

            if (filterList.Count > 0)
            {
                var pageLocation = $"{Request.GetBaseAddress()}".TrimEnd('/');
                var redirectedUrl = $"{pageLocation}{filterList.FirstOrDefault().PageLocation.FullUrl}";
                //logger.LogWarning($"{nameof(Document)} has been redirected for: /{location}/{article} to {redirectedUrl}");

                return RedirectPermanent(redirectedUrl);
            }

            //logger.LogWarning($"{nameof(Body)} has not returned any content for: /{location}/{article}");
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