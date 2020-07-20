using AutoMapper;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models.Api;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.Pages.Controllers
{
    public class ApiController : BasePagesController<ApiController>
    {
        private readonly IContentPageService<ContentPageModel> contentPageService;
        private readonly AutoMapper.IMapper mapper;

        public ApiController(ILogger<ApiController> logger, IContentPageService<ContentPageModel> contentPageService, IMapper mapper) : base(logger)
        {
            this.contentPageService = contentPageService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("api/pages")]
        public async Task<IActionResult> Index()
        {
            var pages = new List<GetIndexModel>();

            var contentPageModels = await contentPageService.GetAllAsync().ConfigureAwait(false);

            if (contentPageModels != null && contentPageModels.Any())
            {
                pages = (from a in contentPageModels.OrderBy(o => o.PageLocation).ThenBy(o => o.CanonicalName)
                         select mapper.Map<GetIndexModel>(a)).ToList();
                Logger.LogInformation($"{nameof(Index)} has succeeded");
            }
            else
            {
                Logger.LogWarning($"{nameof(Index)} has returned with no results");
            }

            return Ok(pages);
        }
    }
}
