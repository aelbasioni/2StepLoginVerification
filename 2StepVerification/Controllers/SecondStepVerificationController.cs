using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Web.Http;
using System.Security.Claims;
using System.Data;
using Microsoft.Owin.Security;
using System.Data.SqlClient;
using System.Configuration;

namespace _2StepVerification.Controllers
{
    [Authorize]
    [RoutePrefix("api/Auth")]
    public class SecondStepVerificationController : ApiController
    {
        [Authorize(Roles = "user")]
        [Route("Validate/{code}")]
        [HttpGet]
        public IHttpActionResult ValidateCode(string code)
        {
            string accessToken = "";
            string Email = "", Name = "";
            DataTable dt = new DataTable();

            if (!string.IsNullOrWhiteSpace(code))
            {
            
                var identity = (ClaimsIdentity)User.Identity;
                IEnumerable<Claim> claims = identity.Claims;

                
                string email = claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault();

                /*
                 * the part 
                 * DATEDIFF(MINUTE, GETDATE(), VerificationCodeDate)<=10 
                 * in the queryString means that if the VerificationCodeDate passes 10 minutes it'll be invalid, 
                 * and the user neads to re-login
                 */
                string queryString  = @"SELECT FullName,Email from dbo.Users where [Email] = @UserEmail and VerificationCode = @VerificationCode and  DATEDIFF(MINUTE, GETDATE(), VerificationCodeDate)<=10; ";

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    command.Parameters.AddWithValue("@UserEmail", email);
                    command.Parameters.AddWithValue("@VerificationCode", code);

                    SqlDataAdapter sda = new SqlDataAdapter(command);
                    sda.Fill(dt);                    
                }

                if (dt.Rows.Count > 0)
                {
                    Email = Convert.ToString(dt.Rows[0]["Email"]);
                    Name = Convert.ToString(dt.Rows[0]["FullName"]);

                    identity.AddClaim(new Claim(ClaimTypes.Role, "verifieduser")); //sub role                   
                    identity.AddClaim(new Claim(ClaimTypes.Name, Name));
                    identity.AddClaim(new Claim(ClaimTypes.Email, Email));
                   
                    AuthenticationTicket ticket = new AuthenticationTicket(identity, new AuthenticationProperties());

                    DateTime currentUtc = DateTime.UtcNow;
                    ticket.Properties.IssuedUtc = currentUtc;
                    ticket.Properties.ExpiresUtc = currentUtc.Add(Startup.VerifiedAccessTokenExpireTimeSpan);

                    accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);
                   
                }
                else
                {
                    throw new Exception("Invalid Code");
                }
            }

            return Ok(new { access_token = accessToken, name = Name, email = Email});
        }

    }
}
