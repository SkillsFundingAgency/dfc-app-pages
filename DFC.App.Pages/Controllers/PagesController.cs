using DFC.App.Pages.Cms.Data.Content;
using DFC.App.Pages.Data.Contracts;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Extensions;
using DFC.App.Pages.Helpers;
using DFC.App.Pages.Models;
using DFC.App.Pages.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.PageBreadcrumb;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.Pages.Controllers
{
    public class PagesController : Controller
    {
        private const string LocalPath = "Pages";

        private readonly ILogger<PagesController> logger;
        private readonly IContentPageService<ContentPageModel> contentPageService;
        private readonly AutoMapper.IMapper mapper;
        private readonly IPagesControlerHelpers pagesControlerHelpers;
        private ISharedContentRedisInterface sharedContentRedisInterface;
        private IOptionsMonitor<contentModeOptions> _options;
        private string status;

        public PagesController(ILogger<PagesController> logger,
                               IContentPageService<ContentPageModel> contentPageService,
                               AutoMapper.IMapper mapper,
                               IPagesControlerHelpers pagesControlerHelpers,
                               ISharedContentRedisInterface sharedContentRedisInterface,
                               IOptionsMonitor<contentModeOptions> options)
        {
            this.logger = logger;
            this.contentPageService = contentPageService;
            this.mapper = mapper;
            this.pagesControlerHelpers = pagesControlerHelpers;
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            _options = options;

        }

        [HttpGet]
        [Route("/")]
        [Route("Pages")]
        public async Task<IActionResult> Index()
        {
            if (_options.CurrentValue.contentMode != null)
            {
                status = _options.CurrentValue.contentMode;
            }
            else
            {
                status = "PUBLISHED";
            }

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
            var pageUrlResponse = await this.sharedContentRedisInterface.GetDataAsync<PageUrlReponse>("pagesurl" + "/" + status);
            if (pageUrlResponse.Page == null)
            {
                return NoContent();
            }

            viewModel.Documents.AddRange(pageUrlResponse.Page.OrderBy(o => o.PageLocation.UrlName).Select(a => mapper.Map<IndexDocumentViewModel>(a)));
            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("Pages/document")]
        [Route("Pages/{location1}/document")]
        [Route("Pages/{location1}/{location2}/document")]
        [Route("Pages/{location1}/{location2}/{location3}/document")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/document")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/{location5}/document")]
        public async Task<IActionResult> Document(PageRequestModel pageRequestModel)
        {
            logger.LogInformation($"{nameof(Document)} has been called");
            if (_options.CurrentValue.contentMode != null)
            {
                status = _options.CurrentValue.contentMode;
            }
            else
            {
                status = "PUBLISHED";
            }

            var (location, article) = PagesControlerHelpers.ExtractPageLocation(pageRequestModel);
            string pageUrl = GetPageUrl(location, article);
            var pageResponse = await this.sharedContentRedisInterface.GetDataAsync<Page>("Page" + pageUrl + "/" + status);
            if (pageResponse != null)
            {
                var viewModel = mapper.Map<DocumentViewModel>(pageResponse);
                if (pageResponse.ShowBreadcrumb)
                {
                    viewModel.Breadcrumb = await GetBreadcrumb(location, article);

                    if (viewModel.Breadcrumb?.Breadcrumbs != null && viewModel.Breadcrumb.Breadcrumbs.Any())
                    {
                        foreach (var breadcrumb in viewModel.Breadcrumb.Breadcrumbs)
                        {
                            var route = breadcrumb.Route == "/" ? string.Empty : breadcrumb.Route;
                            breadcrumb.Route = $"/Pages{route}/document";
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
        [Route("Pages/head")]
        [Route("Pages/{location1}/head")]
        [Route("Pages/{location1}/{location2}/head")]
        [Route("Pages/{location1}/{location2}/{location3}/head")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/head")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/{location5}/head")]
        public async Task<IActionResult> Head(PageRequestModel pageRequestModel)
        {
            logger.LogInformation($"{nameof(Head)} has been called");
            if (_options.CurrentValue.contentMode != null)
            {
                status = _options.CurrentValue.contentMode;
            }
            else
            {
                status = "PUBLISHED";
            }

            var (location, article) = PagesControlerHelpers.ExtractPageLocation(pageRequestModel);
            string pageUrl = GetPageUrl(location, article);
            var pageResponse = await this.sharedContentRedisInterface.GetDataAsync<Page>("Page" + pageUrl + "/" + status);
            var viewModel = new HeadViewModel();
            if (pageResponse != null && pageResponse.PageLocation != null)
            {
                mapper.Map(pageResponse, viewModel);
                viewModel.CanonicalUrl = BuildCanonicalUrl(pageResponse);
            }

            return this.NegotiateContentResult(viewModel);
        }

        [Route("Pages/breadcrumb")]
        [Route("Pages/{location1}/breadcrumb")]
        [Route("Pages/{location1}/{location2}/breadcrumb")]
        [Route("Pages/{location1}/{location2}/{location3}/breadcrumb")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/breadcrumb")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/{location5}/breadcrumb")]
        public async Task<IActionResult> Breadcrumb(PageRequestModel pageRequestModel)
        {
            logger.LogInformation($"{nameof(Breadcrumb)} has been called");

            var (location, article) = PagesControlerHelpers.ExtractPageLocation(pageRequestModel);
            var breadcrumbResponse = await GetBreadcrumb(location, article);

            if (breadcrumbResponse == null)
            {
                return NoContent();
            }

            return this.NegotiateContentResult(breadcrumbResponse);
        }

        [HttpGet]
        [Route("Pages/bodytop")]
        [Route("Pages/{location1}/bodytop")]
        [Route("Pages/{location1}/{location2}/bodytop")]
        [Route("Pages/{location1}/{location2}/{location3}/bodytop")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/bodytop")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/{location5}/bodytop")]
        public IActionResult BodyTop(PageRequestModel pageRequestModel)
        {
            logger.LogWarning($"{nameof(BodyTop)} has returned no content");

            return NoContent();
        }

        [HttpGet]
        [Route("Pages/herobanner")]
        [Route("Pages/{location1}/herobanner")]
        [Route("Pages/{location1}/{location2}/herobanner")]
        [Route("Pages/{location1}/{location2}/{location3}/herobanner")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/herobanner")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/{location5}/herobanner")]
        public async Task<IActionResult> HeroBanner(PageRequestModel pageRequestModel)
        {
            logger.LogInformation($"{nameof(HeroBanner)} has been called");
            if (_options.CurrentValue.contentMode != null)
            {
                status = _options.CurrentValue.contentMode;
            }
            else
            {
                status = "PUBLISHED";
            }

            var (location, article) = PagesControlerHelpers.ExtractPageLocation(pageRequestModel);
            string pageUrl = GetPageUrl(location, article);
            var pageResponse = await this.sharedContentRedisInterface.GetDataAsync<Page>("Page" + pageUrl + "/" + status);
            if (pageResponse == null)
            {
                return NoContent();
            }

            var viewModel = mapper.Map<HeroBannerViewModel>(pageResponse);

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("Pages/body")]
        [Route("Pages/{location1}/body")]
        [Route("Pages/{location1}/{location2}/body")]
        [Route("Pages/{location1}/{location2}/{location3}/body")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/body")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/{location5}/body")]
        public async Task<IActionResult> Body(PageRequestModel pageRequestModel)
        {
            logger.LogInformation($"{nameof(Body)} has been called");
            if (_options.CurrentValue.contentMode != null)
            {
                status = _options.CurrentValue.contentMode;
            }
            else
            {
                status = "PUBLISHED";
            }

            var (location, article) = PagesControlerHelpers.ExtractPageLocation(pageRequestModel);
            string pageUrl = GetPageUrl(location, article);
            var pageResponse = await this.sharedContentRedisInterface.GetDataAsync<Page>("Page" + pageUrl + "/" + status);
            var viewModel = new BodyViewModel();
            if (pageResponse != null)
            {
                mapper.Map(pageResponse, viewModel);
                return this.NegotiateContentResult(viewModel);
            }

            var redirectedContentPageModel = await this.sharedContentRedisInterface.GetDataAsync<IList<PageUrl>>("Page/GetPageUrls");
            var filterList = redirectedContentPageModel.Where(ctr => (ctr.PageLocation.RedirectLocations ?? "").Split("\r\n").Contains(pageUrl)).ToList();
            if (filterList.Count > 0)
            {
                var pageLocation = $"{Request.GetBaseAddress()}".TrimEnd('/');
                var redirectedUrl = $"{pageLocation}{filterList.FirstOrDefault().PageLocation.FullUrl}";
                logger.LogWarning($"{nameof(Document)} has been redirected for: /{location}/{article} to {redirectedUrl}");
                return RedirectPermanent(redirectedUrl);
            }

            logger.LogWarning($"{nameof(Body)} has not returned any content for: /{location}/{article}");
            return NotFound();
        }

        [HttpGet]
        [Route("Pages/sidebarright")]
        [Route("Pages/{location1}/sidebarright")]
        [Route("Pages/{location1}/{location2}/sidebarright")]
        [Route("Pages/{location1}/{location2}/{location3}/sidebarright")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/sidebarright")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/{location5}/sidebarright")]
        public IActionResult SidebarRight(PageRequestModel pageRequestModel)
        {
            logger.LogWarning($"{nameof(SidebarRight)} has returned no content");

            return NoContent();
        }

        [HttpGet]
        [Route("Pages/sidebarleft")]
        [Route("Pages/{location1}/sidebarleft")]
        [Route("Pages/{location1}/{location2}/sidebarleft")]
        [Route("Pages/{location1}/{location2}/{location3}/sidebarleft")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/sidebarleft")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/{location5}/sidebarleft")]
        public IActionResult SidebarLeft(PageRequestModel pageRequestModel)
        {
            logger.LogWarning($"{nameof(SidebarLeft)} has returned no content");

            return NoContent();
        }

        [HttpGet]
        [Route("Pages/bodyfooter")]
        [Route("Pages/{location1}/bodyfooter")]
        [Route("Pages/{location1}/{location2}/bodyfooter")]
        [Route("Pages/{location1}/{location2}/{location3}/bodyfooter")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/bodyfooter")]
        [Route("Pages/{location1}/{location2}/{location3}/{location4}/{location5}/bodyfooter")]
        public IActionResult BodyFooter(PageRequestModel pageRequestModel)
        {
            logger.LogWarning($"{nameof(BodyFooter)} has returned no content");

            return NoContent();
        }

        #region private functions
        private string GetPageUrl(string location, string article)
        {
            string pageUrl = string.Empty;
            if (string.IsNullOrWhiteSpace(location) && string.IsNullOrWhiteSpace(article))
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
            if (_options.CurrentValue.contentMode != null)
            {
                status = _options.CurrentValue.contentMode;
            }
            else
            {
                status = "PUBLISHED";
            }

            var breadcrumbResponse = await this.sharedContentRedisInterface.GetDataAsync<PageBreadcrumb>("PageLocation/" + status);
            string pageUrl = GetPageUrl(location, article);
            var pageResponse = await this.sharedContentRedisInterface.GetDataAsync<Page>("Page" + pageUrl + "/" + status);

            if (pageResponse == null || !pageResponse.ShowBreadcrumb)
            {
                return null;
            }

            string breadCrumbDisplayText = pageResponse.Breadcrumb.TermContentItems.FirstOrDefault().DisplayText;
            var jdoc = JObject.Parse(breadcrumbResponse.Content);
            var root = jdoc.SelectToken("$.TaxonomyPart.Terms");
            var token = root.SelectTokens($"$..Terms[?(@.DisplayText == '{breadCrumbDisplayText}')]");
            var path = token.FirstOrDefault().Path;
            var result = BuildBreadCrumb(path, jdoc);

            if (!pageResponse.PageLocation.DefaultPageForLocation && !string.IsNullOrWhiteSpace(pageResponse.DisplayText))
            {
                var articlePathViewModel = new BreadcrumbItemViewModel
                {
                    Route = $"{pageResponse.Breadcrumb.TermContentItems.FirstOrDefault().DisplayText}",
                    Title = pageResponse.DisplayText,
                    AddHyperlink = false,
                };

                result.Breadcrumbs.Add(articlePathViewModel);
            }

            return result;
        }

        private BreadcrumbViewModel BuildBreadCrumb(string path, JObject doc)
        {
            StringBuilder breadCrumbText = new StringBuilder();
            BreadcrumbViewModel breadCrumbs = new BreadcrumbViewModel()
            {
                Breadcrumbs = new List<BreadcrumbItemViewModel>(),
            };

            int index = 0;
            do
            {
                var breadCrumbPath = $"{path}.PageLocation.BreadcrumbText.Text";
                var displayTextPath = $"{path}.DisplayText";
                var breadCrumbToken = doc.SelectToken(breadCrumbPath);
                var displayTextToken = doc.SelectToken(displayTextPath);
                breadCrumbs.Breadcrumbs.Add(new BreadcrumbItemViewModel()
                {
                    Route = displayTextToken.ToString(),
                    Title = breadCrumbToken.ToString(),
                });
                index = path.LastIndexOf(".");
                path = path.Remove(index);
            }
            while (path.LastIndexOf(".") > 0);

            breadCrumbs.Breadcrumbs.Reverse();
            return breadCrumbs;
        }

        private Uri BuildCanonicalUrl(Page pageModel)
        {
            var pathDirectory1 = pageModel.PageLocation.UrlName ?? string.Empty;

            var uriString = Path.Combine(
                Request.GetBaseAddress()?.ToString() ?? string.Empty,
                pathDirectory1);

            return new Uri(uriString, UriKind.RelativeOrAbsolute);
        }

        #endregion
    }
}