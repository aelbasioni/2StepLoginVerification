using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace _2StepVerification.Controllers
{
    public class DataController : ApiController
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("api/FetchPublicData")]
        public IHttpActionResult GetPublicData()
        {
            return Ok(new string[] { "value1", "value2" });
        }


        [Authorize(Roles = "verifieduser")]
        [HttpGet]
        [Route("api/FetchSecuredData")]
        public IHttpActionResult GetSecuredData()
        {
            return Ok(new string[] { "value1", "value2" });
        }
    }
}
