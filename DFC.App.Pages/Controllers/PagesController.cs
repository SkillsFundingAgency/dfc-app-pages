using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Extensions;
using DFC.App.Pages.Models;
using DFC.App.Pages.ViewModels;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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
            };
            var contentPageModels = await contentPageService.GetAllAsync().ConfigureAwait(false);

            if (contentPageModels != null)
            {
                viewModel.Documents = (from a in contentPageModels.OrderBy(o => o.CanonicalName)
                                       select mapper.Map<IndexDocumentViewModel>(a)).ToList();

                viewModel.Documents.Add(new IndexDocumentViewModel { CanonicalName = HealthController.HealthViewCanonicalName });
                viewModel.Documents.Add(new IndexDocumentViewModel { CanonicalName = SitemapController.SitemapViewCanonicalName });
                viewModel.Documents.Add(new IndexDocumentViewModel { CanonicalName = HomeController.ThisViewCanonicalName });

                Logger.LogInformation($"{nameof(Index)} has succeeded");
            }
            else
            {
                Logger.LogWarning($"{nameof(Index)} has returned with no results");
            }

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("pages/{article}")]
        public async Task<IActionResult> Document(string article)
        {
            var contentPageModel = await GetContentPageAsync(article).ConfigureAwait(false);

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
                var alternateContentPageModel = await GetAlternativeContentPageAsync(article).ConfigureAwait(false);

                if (alternateContentPageModel != null)
                {
                    var alternateUrl = $"{Request.GetBaseAddress()}{LocalPath}/{alternateContentPageModel.CanonicalName}";
                    Logger.LogWarning($"{nameof(Document)} has been redirected for: {article} to {alternateUrl}");

                    return RedirectPermanentPreserveMethod(alternateUrl);
                }
            }

            Logger.LogWarning($"{nameof(Document)} has returned no content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("pages/{article}/htmlhead")]
        public async Task<IActionResult> HtmlHead(string? article)
        {
            var viewModel = new HtmlHeadViewModel();
            var contentPageModel = await GetContentPageAsync(article).ConfigureAwait(false);

            if (contentPageModel != null)
            {
                mapper.Map(contentPageModel, viewModel);

                viewModel.CanonicalUrl = new Uri($"{Request.GetBaseAddress()}{RegistrationPath}/{contentPageModel.CanonicalName}", UriKind.RelativeOrAbsolute);
            }

            Logger.LogInformation($"{nameof(HtmlHead)} has returned content for: {article}");

            return this.NegotiateContentResult(viewModel);
        }

        [Route("pages/{article}/breadcrumb")]
        public async Task<IActionResult> Breadcrumb(string? article)
        {
            var contentPageModel = await GetContentPageAsync(article).ConfigureAwait(false);
            var breadcrumbItemModel = mapper.Map<BreadcrumbItemModel>(contentPageModel);
            var viewModel = BuildBreadcrumb(RegistrationPath, breadcrumbItemModel);

            Logger.LogInformation($"{nameof(Breadcrumb)} has returned content for: {article}");

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("pages/{article}/bodytop")]
        public IActionResult BodyTop(string? article)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/{article}/herobanner")]
        public IActionResult HeroBanner(string? article)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/{article}/body")]
        public async Task<IActionResult> Body(string? article)
        {
            var viewModel = new BodyViewModel();
            var contentPageModel = await GetContentPageAsync(article).ConfigureAwait(false);

            if (contentPageModel != null)
            {
                mapper.Map(contentPageModel, viewModel);
                Logger.LogInformation($"{nameof(Body)} has returned content for: {article}");

                return this.NegotiateContentResult(viewModel, contentPageModel);
            }

            if (!string.IsNullOrWhiteSpace(article))
            {
                var alternateContentPageModel = await GetAlternativeContentPageAsync(article).ConfigureAwait(false);

                if (alternateContentPageModel != null)
                {
                    var alternateUrl = $"{Request.GetBaseAddress()}{RegistrationPath}/{alternateContentPageModel.CanonicalName}";
                    Logger.LogWarning($"{nameof(Body)} has been redirected for: {article} to {alternateUrl}");

                    return RedirectPermanentPreserveMethod(alternateUrl);
                }
            }

            Logger.LogWarning($"{nameof(Body)} has not returned any content for: {article}");
            return NotFound();
        }

        [HttpGet]
        [Route("pages/{article}/sidebarright")]
        public IActionResult SidebarRight(string? article)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/{article}/sidebarleft")]
        public IActionResult SidebarLeft(string? article)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/{article}/bodyfooter")]
        public IActionResult BodyFooter(string? article)
        {
            return NoContent();
        }

        #region Define helper methods

        private async Task<ContentPageModel?> GetContentPageAsync(string? article)
        {
            const string defaultArticleName = HomeController.ThisViewCanonicalName;
            var articleName = string.IsNullOrWhiteSpace(article) ? defaultArticleName : article;

            var contentPageModel = await contentPageService.GetByNameAsync(articleName).ConfigureAwait(false);

            return contentPageModel;
        }

        private async Task<ContentPageModel?> GetAlternativeContentPageAsync(string article)
        {
            var contentPageModel = await contentPageService.GetByAlternativeNameAsync(article).ConfigureAwait(false);

            return contentPageModel;
        }

        #endregion Define helper methods
    }
}