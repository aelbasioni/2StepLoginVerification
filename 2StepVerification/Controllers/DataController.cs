using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace _2StepVerification.Controllers
{
    public class DataController : ApiController
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("api/FetchGendreLookUp")]
        public IHttpActionResult GetPublicData()
        {
            return Ok(new string[] { "Male", "Female" });
        }


        [Authorize(Roles = "verifieduser")]
        [HttpGet]
        [Route("api/FetchEmployees")]
        public IHttpActionResult GetSecuredData()
        {
            DataTable dt = new DataTable();
            string queryString = @"SELECT * from [Employees]";

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                SqlDataAdapter sda = new SqlDataAdapter(command);
                sda.Fill(dt);
            }
            return Ok(dt);
        }
    }
}
