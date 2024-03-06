using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DFC.App.Pages.Controllers
{
    public class ApiController : Controller
    {
        private readonly ILogger<ApiController> logger;
        private readonly IMapper mapper;

        public ApiController(ILogger<ApiController> logger, IMapper mapper)
        {
            this.logger = logger;
            this.mapper = mapper;
        }

        //TODO: Replace Cosmos call with Redis call
        /*[HttpGet]
        [Route("api/pages")]
        public async Task<IActionResult> Index()
        {
            logger.LogInformation($"{nameof(Index)} has been called");

            var pages = new Dictionary<Guid, GetIndexModel>();

            //TODO: replace with redis call
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
        }*/

        //TODO: Replace Cosmos call with Redis call
        /*[HttpGet]
        [Route("api/pages/{id}")]
        public async Task<IActionResult> Document(Guid id)
        {
            logger.LogInformation($"{nameof(Document)} has been called");

            //TODO: replace with redis call
            var contentPageModel = await contentPageService.GetByIdAsync(id).ConfigureAwait(false);

            if (contentPageModel != null)
            {
                var getIndexModel = mapper.Map<GetIndexModel>(contentPageModel);
                logger.LogInformation($"{nameof(Document)} has succeeded");
                return Ok(getIndexModel);
            }

            logger.LogWarning($"{nameof(Document)} has returned with no content");

            return NoContent();
        }*/
    }
}
