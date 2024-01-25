using AutoMapper;
using DFC.App.Pages.Data.Models;
using DFC.App.Pages.Models.Api;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.Pages.Controllers
{
    public class ApiController : Controller
    {
        private readonly ILogger<ApiController> logger;
        private readonly IContentPageService<ContentPageModel> contentPageService;
        private readonly AutoMapper.IMapper mapper;

        public ApiController(ILogger<ApiController> logger, IContentPageService<ContentPageModel> contentPageService, IMapper mapper)
        {
            this.logger = logger;
            this.contentPageService = contentPageService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("api/Pages")]
        public async Task<IActionResult> Index()
        {
            logger.LogInformation($"{nameof(Index)} has been called");

            var pages = new Dictionary<Guid, GetIndexModel>();

            var contentPageModels = await contentPageService.GetAllAsync().ConfigureAwait(false);

            if (contentPageModels != null && contentPageModels.Any())
            {
                pages = (from a in contentPageModels.OrderBy(o => o.PageLocation).ThenBy(o => o.CanonicalName)
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
        [Route("api/Pages/{id}")]
        public async Task<IActionResult> Document(Guid id)
        {
            logger.LogInformation($"{nameof(Document)} has been called");

            var contentPageModel = await contentPageService.GetByIdAsync(id).ConfigureAwait(false);

            if (contentPageModel != null)
            {
                var getIndexModel = mapper.Map<GetIndexModel>(contentPageModel);
                logger.LogInformation($"{nameof(Document)} has succeeded");
                return Ok(getIndexModel);
            }

            logger.LogWarning($"{nameof(Document)} has returned with no content");

            return NoContent();
        }
    }
}
