using MeetDown.Events.Core.DataAccess.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MeetDown.Events.API.Controllers
{
    [Route("/api/health")]
    public class HealthController : Controller
    {
        IMeetRepository _meetRepository;

        public HealthController(IMeetRepository meetRepository)
        {
            _meetRepository = meetRepository;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            try
            {
                // access data store to ensure system is available
                var groupExists = _meetRepository.GroupExists(1);

                return Ok();
            }
            catch
            {
                return StatusCode(Convert.ToInt32(HttpStatusCode.InternalServerError));
            }
        }
    }
}
