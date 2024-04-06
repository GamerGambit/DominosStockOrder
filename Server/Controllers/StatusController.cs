using DominosStockOrder.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DominosStockOrder.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly Status _statusService;

        public StatusController(Status statusService)
        {
            _statusService = statusService;
        }

        [HttpGet]
        public ActionResult<Status> Get()
        {
            return _statusService;
        }
    }
}
