using DFC.App.Pages.Cms.Data.Content;
using DFC.App.Pages.Extensions;
using DFC.App.Pages.Models;
using DFC.App.Pages.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.PageBreadcrumb;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
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
using Constants = DFC.Common.SharedContent.Pkg.Netcore.Constant.ApplicationKeys;

namespace DFC.App.Pages.Controllers
{
    public class PagesController : Controller
    {
        private const string LocalPath = "pages";

        private readonly ILogger<PagesController> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IOptionsMonitor<ContentModeOptions> options;
        private string status = string.Empty;

        public PagesController(
            ILogger<PagesController> logger,
            AutoMapper.IMapper mapper,
            ISharedContentRedisInterface sharedContentRedisInterface,
            IOptionsMonitor<ContentModeOptions> options)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            this.options = options;
        }

        [HttpGet]
        [Route("/")]
        [Route("pages")]
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
            var pageUrlResponse = await this.sharedContentRedisInterface.GetDataAsync<PageUrlResponse>(Constants.PagesUrlSuffix, status);
            if (pageUrlResponse.Page == null)
            {
                return NoContent();
            }

            viewModel.Documents.AddRange(pageUrlResponse.Page.OrderBy(o => o.PageLocation.UrlName).Select(a => mapper.Map<IndexDocumentViewModel>(a)));
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
            if (options.CurrentValue.contentMode != null)
            {
                status = options.CurrentValue.contentMode;
            }
            else
            {
                status = "PUBLISHED";
            }

            var (location, article) = ExtractPageLocation(pageRequestModel);
            string pageUrl = GetPageUrl(location, article);
            var pageResponse = await this.sharedContentRedisInterface.GetDataAsync<Page>(Constants.PageSuffix + pageUrl, status);
            if (pageResponse == null)
            {
                pageUrl = pageUrl.Remove(pageUrl.LastIndexOf('/'));
                pageResponse = await this.sharedContentRedisInterface.GetDataAsync<Page>(Constants.PageSuffix + pageUrl, status);
            }

            if (pageResponse != null)
            {
                var viewModel = mapper.Map<DocumentViewModel>(pageResponse);
                if (pageResponse.ShowBreadcrumb.GetValueOrDefault(false))
                {
                    viewModel.Breadcrumb = await GetBreadcrumb(location, article, pageUrl);

                    if (viewModel.Breadcrumb?.Breadcrumbs != null && viewModel.Breadcrumb.Breadcrumbs.Any())
                    {
                        foreach (var breadcrumb in viewModel.Breadcrumb.Breadcrumbs)
                        {
                            var route = breadcrumb.Route == "/" ? string.Empty : breadcrumb.Route;
                            breadcrumb.Route = $"Page/{route}/document";
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
            if (options.CurrentValue.contentMode != null)
            {
                status = options.CurrentValue.contentMode;
            }
            else
            {
                status = "PUBLISHED";
            }

            var (location, article) = ExtractPageLocation(pageRequestModel);
            string pageUrl = GetPageUrl(location, article);
            var viewModel = GetResponse<HeadViewModel>(pageUrl).Result;

            if (viewModel != null)
            {
                return this.NegotiateContentResult(viewModel);

            }

            var redirectedContentPageModel = await this.sharedContentRedisInterface.GetDataAsync<PageUrlResponse>(Constants.PagesUrlSuffix, status);
            var filterList = redirectedContentPageModel.Page.Where(ctr => (ctr.PageLocation.RedirectLocations ?? string.Empty).Split("\r\n").Contains(pageUrl)).ToList();

            if (filterList.Count > 0)
            {
                var pageLocation = $"{Request.GetBaseAddress()}".TrimEnd('/');
                var redirectedUrl = $"{pageLocation}{filterList.FirstOrDefault()?.PageLocation?.FullUrl}";

                logger.LogWarning($"{nameof(Document)} has been redirected for: /{location}/{article} to {redirectedUrl}");
                return RedirectPermanent(redirectedUrl);
            }
            else
            {
                var redirectViewModel = GetResponse<HeadViewModel>(pageUrl, redirectedContentPageModel).Result;
                if (redirectViewModel != null)
                {
                    return this.NegotiateContentResult(redirectViewModel);
                }
            }

            return NoContent();
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
            string pageUrl = string.Empty;
            var (location, article) = ExtractPageLocation(pageRequestModel);
            var breadcrumbResponse = await GetBreadcrumb(location, article, pageUrl);

            if (breadcrumbResponse == null)
            {
                return NoContent();
            }

            return this.NegotiateContentResult(breadcrumbResponse);
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
            if (options.CurrentValue.contentMode != null)
            {
                status = options.CurrentValue.contentMode;
            }
            else
            {
                status = "PUBLISHED";
            }

            var (location, article) = ExtractPageLocation(pageRequestModel);
            string pageUrl = GetPageUrl(location, article);
            var viewModel = GetResponse<HeroBannerViewModel>(pageUrl).Result;

            if (viewModel != null)
            {
                return this.NegotiateContentResult(viewModel);
            }

            var redirectedContentPageModel = await this.sharedContentRedisInterface.GetDataAsync<PageUrlResponse>(Constants.PagesUrlSuffix, status);
            var filterList = redirectedContentPageModel.Page.Where(ctr => (ctr.PageLocation.RedirectLocations ?? "").Split("\r\n").Contains(pageUrl)).ToList();

            if (filterList.Count > 0)
            {
                var pageLocation = $"{Request.GetBaseAddress()}".TrimEnd('/');
                var redirectedUrl = $"{pageLocation}{filterList.FirstOrDefault()?.PageLocation?.FullUrl}";

                logger.LogWarning($"{nameof(Document)} has been redirected for: /{location}/{article} to {redirectedUrl}");
                return RedirectPermanent(redirectedUrl);
            }
            else
            {
                var redirectViewModel = GetResponse<HeroBannerViewModel>(pageUrl, redirectedContentPageModel).Result;
                if (redirectViewModel != null)
                {
                    return this.NegotiateContentResult(redirectViewModel);
                }
            }

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
            logger.LogInformation($"{nameof(Body)} has been called");
            if (options.CurrentValue.contentMode != null)
            {
                status = options.CurrentValue.contentMode;
            }
            else
            {
                status = "PUBLISHED";
            }

            var (location, article) = ExtractPageLocation(pageRequestModel);
            string pageUrl = GetPageUrl(location, article);
            var viewModel = GetResponse<BodyViewModel>(pageUrl).Result;

            if (viewModel != null)
            {
                return this.NegotiateContentResult(viewModel);
            }

            var redirectedContentPageModel = await this.sharedContentRedisInterface.GetDataAsync<PageUrlResponse>(Constants.PagesUrlSuffix, status);
            var filterList = redirectedContentPageModel.Page.Where(ctr => (ctr.PageLocation.RedirectLocations ?? "").Split("\r\n").Contains(pageUrl)).ToList();

            if (filterList.Count > 0)
            {
                var pageLocation = $"{Request.GetBaseAddress()}".TrimEnd('/');
                var redirectedUrl = $"{pageLocation}{filterList.FirstOrDefault()?.PageLocation?.FullUrl}";

                logger.LogWarning($"{nameof(Document)} has been redirected for: /{location}/{article} to {redirectedUrl}");
                return RedirectPermanent(redirectedUrl);
            }
            else
            {
                var redirectViewModel = GetResponse<BodyViewModel>(pageUrl, redirectedContentPageModel).Result;
                if (redirectViewModel != null)
                {
                    return this.NegotiateContentResult(redirectViewModel);
                }
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

        #region private functions
        private static (string location, string? article) ExtractPageLocation(PageRequestModel pageRequestModel)
        {
            _ = pageRequestModel ?? throw new ArgumentNullException(nameof(pageRequestModel));

            var pageLocation = string.Join("/", new[] { pageRequestModel.Location1, pageRequestModel.Location2, pageRequestModel.Location3, pageRequestModel.Location4, pageRequestModel.Location5 });
            var pageLocations = pageLocation.Split("/", StringSplitOptions.RemoveEmptyEntries);
            var location = string.Empty;
            var article = string.Empty;

            if (pageLocations.Length == 1)
            {
                location = pageLocations.First();
            }
            else if (pageLocations.Length > 1)
            {
                location = string.Join("/", pageLocations, 0, pageLocations.Length - 1);
                article = pageLocations.Last();
            }

            return (location, article);
        }

        private async Task<T?> GetResponse<T>(string pageUrl)
            where T : new()
        {
            var pageResponse = await this.sharedContentRedisInterface.GetDataAsync<Page>(Constants.PageSuffix + pageUrl, status);
            var viewModel = new T();

            if (pageResponse == null)
            {
                pageUrl = pageUrl.Remove(pageUrl.LastIndexOf('/'));
                pageResponse = await this.sharedContentRedisInterface.GetDataAsync<Page>(Constants.PageSuffix + pageUrl, status);
            }

            if (pageResponse != null)
            {
                mapper.Map(pageResponse, viewModel);
                return viewModel;
            }

            return default(T);
        }

        private async Task<T?> GetResponse<T>(string pageUrl, PageUrlResponse redirectedContentPageModel)
            where T : new()
        {
            foreach (var page in redirectedContentPageModel.Page.Where(ctr => (ctr.PageLocation.DefaultPageForLocation == true)))
            {
                var fullUrl = page.PageLocation.FullUrl;

                var pageLocationUrl = $"{fullUrl}".Substring(0, fullUrl.LastIndexOf('/'));

                if (pageUrl == pageLocationUrl)
                {
                    var pageResponse = await this.sharedContentRedisInterface.GetDataAsync<Page>(Constants.PageSuffix + fullUrl, status);
                    var viewModel = new T();
                    mapper.Map(pageResponse, viewModel);
                    return viewModel;
                }
            }

            return default(T);
        }

        private async Task<BreadcrumbViewModel> GetBreadcrumb(string location, string article, string pageUrl)
        {
            if (options.CurrentValue.contentMode != null)
            {
                status = options.CurrentValue.contentMode;
            }
            else
            {
                status = "PUBLISHED";
            }

            var breadcrumbResponse = await this.sharedContentRedisInterface.GetDataAsync<PageBreadcrumb>(Constants.PageLocationSuffix, status);
            if (pageUrl == string.Empty)
            {
                pageUrl = GetPageUrl(location, article);
            }
            var pageResponse = await this.sharedContentRedisInterface.GetDataAsync<Page>(Constants.PageSuffix + pageUrl, status);

            if (pageResponse == null || !pageResponse.ShowBreadcrumb.GetValueOrDefault(false))
            {
                return null;
            }

            string breadCrumbDisplayText = pageResponse?.Breadcrumb?.TermContentItems?.FirstOrDefault()?.DisplayText ?? string.Empty;
            var jdoc = JObject.Parse(breadcrumbResponse.Content);
            var root = jdoc.SelectToken("$.TaxonomyPart.Terms");
            var token = root.SelectTokens($"$..Terms[?(@.DisplayText == '{breadCrumbDisplayText}')]");
            var path = token.FirstOrDefault()?.Path ?? string.Empty;
            var result = BuildBreadCrumb(path, jdoc);

            if (!(bool)pageResponse.PageLocation.DefaultPageForLocation && !string.IsNullOrWhiteSpace(pageResponse.DisplayText))
            {
                var articlePathViewModel = new BreadcrumbItemViewModel
                {
                    Route = $"{pageResponse?.Breadcrumb?.TermContentItems?.FirstOrDefault()?.DisplayText ?? string.Empty}",
                    Title = pageResponse?.DisplayText ?? string.Empty,
                    AddHyperlink = false,
                };
                result.Breadcrumbs.Add(articlePathViewModel);
                string segmentRoute = string.Empty;

                foreach (var segment in result.Breadcrumbs)
                {
                    if (segment.Route != "/")
                    {
                        segmentRoute += "/" + segment.Route;
                        segment.Route = segmentRoute;
                    }
                }
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

        private string GetPageUrl(string location, string article)
        {
            string pageUrl = string.Empty;
            if (string.IsNullOrWhiteSpace(location) && string.IsNullOrWhiteSpace(article))
            {
                pageUrl = "/home";
            }
            else
            {
                pageUrl = $"/{location}/{(string.IsNullOrWhiteSpace(article) ? location : article)}";
            }

            return pageUrl;
        }

        #endregion
    }
}