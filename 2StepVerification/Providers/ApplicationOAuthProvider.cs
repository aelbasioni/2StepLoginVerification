using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


namespace _2StepVerification.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //Generate randum number of 6 digits to be used as verification code
            Random generator = new Random();
            String verificationCode = generator.Next(1111, 1000000).ToString("D6");


            int affectedRows = 0;
            string queryString = @"Update dbo.Users set [VerificationCode]=@VerificationCode , VerificationCodeDate = getdate() where [Email] = @UserEmail and [Password] = @UserPassword;
	                                    select @@ROWCOUNT";

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@UserEmail", context.UserName);
                command.Parameters.AddWithValue("@UserPassword", context.Password);
                command.Parameters.AddWithValue("@VerificationCode", verificationCode);

                try
                {
                    connection.Open();
                    affectedRows = Convert.ToInt16(command.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    context.SetError("sql_error", ex.Message);
                    return;
                }
                finally {
                    if (connection.State != ConnectionState.Closed)
                        connection.Close();
                }
            }
            

            if (affectedRows > 0)
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
                identity.AddClaim(new Claim(ClaimTypes.Email, context.UserName));

                /***********************************************************************
                 * In actual work we should send here, the verification code to the user BY EMAIL 
                 * but now for simplicity we'll get it from the database
                 * *********************************************************************/

                context.Validated(identity);
            }
            else
            {
                context.SetError("invalid_grant", "Provided username and password is incorrect");                
               
            }
            return;          
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            //if (context.ClientId == _publicClientId)
            //{
            Uri expectedRootUri = new Uri(context.Request.Uri, "/");

            if (expectedRootUri.AbsoluteUri == context.RedirectUri)
            {
                context.Validated();
            }
            //}

            return Task.FromResult<object>(null);

        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }

    }
}