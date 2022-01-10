using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using FordAPI.Models;
using System.Text;
using System.Net.Http.Headers;
using System.Web;
using System.Net.Http;
using System.Net.Http.Formatting;
using Nest;
using Microsoft.Owin.Security.Infrastructure;
using System.Web.Http.Controllers;
using Newtonsoft.Json;
using System.Data.SqlClient;
using FordAPI.Helper;
using System.Data;
using System.Configuration;

namespace FordAPI.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

       

        //private readonly IUserService _userService;
        public static string UserConnection
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["UserConnectionString"].ToString();  
            }
        }
        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            string usernameVal = context.UserName;
            string passwordVal = context.Password;

            var user = Validate(usernameVal, passwordVal).ToList();
            int uId = user.FirstOrDefault().UserId;
            string uName = user.FirstOrDefault().UserName;
            
            // Verification.  
            if (user == null || user.Count() <= 0 || (uId == 0 && uName == "invalid_grant"))
            {
                // Settings.  
                context.SetError(user[0].UserName, user[0].Password);

                // Retuen info.  
                return;
            }

            // Initialization.  
            var claims = new List<Claim>();
            var userInfo = user.FirstOrDefault();

            // Setting  
            claims.Add(new Claim(ClaimTypes.Name, userInfo.UserName + ":" + userInfo.UserId));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, Convert.ToString(userInfo.UserId)));

            // Setting Claim Identities for OAUTH 2 protocol.  
            ClaimsIdentity oAuthClaimIdentity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesClaimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType);

            // Setting user authentication.  
            AuthenticationProperties properties = CreateProperties(userInfo.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthClaimIdentity, properties);

            // Grant access to authorize user.  
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesClaimIdentity);
            
        }

        private static List<UsersList> Validate(string usernameVal, string passwordVal)
        {
            List<UsersList> uList = new List<UsersList>(); 

            SqlParameter[] sqlParams = {
                   new SqlParameter("@username", usernameVal.Trim()),
                     new SqlParameter("@password", passwordVal.Trim()),
            };

            var dbResult = SqlHelper.ExecuteDataset(UserConnection, CommandType.StoredProcedure, "SP_CheckUserCredentails", sqlParams);
            if (dbResult != null && dbResult.Tables[0].Rows.Count > 0)
            {
                for (var i = 0; i < dbResult.Tables[0].Rows.Count; i++)
                {
                    UsersList objList = new UsersList();
                    objList.UserId = Convert.ToInt32(dbResult.Tables[0].Rows[i]["UserId"]);
                    objList.UserName = Convert.ToString(dbResult.Tables[0].Rows[i]["Email"]);
                    objList.Password = Convert.ToString(passwordVal);
                    uList.Add(objList);
                }
            }
            else
            {
                UsersList objList = new UsersList();
                objList.UserId = Convert.ToInt32(0);
                objList.UserName = Convert.ToString("invalid_grant");
                objList.Password = Convert.ToString("The user name or password is incorrect.");
                uList.Add(objList);
            }
            return uList;
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {

            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        //private static async Task<Models.Token> GetElibilityToken(OAuthTokenEndpointContext client)
        //{
        //    //string baseAddress = @"https://blah.blah.blah.com/oauth2/token";
        //    //HttpResponseMessage tokenResponse = await client.PostAsync(baseAddress, new FormUrlEncodedContent(client.Properties.Dictionary));
        //    //var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
        //    //Models.Token tok = JsonConvert.DeserializeObject<Models.Token>(jsonContent);
        //    //return tok;
        //}

        //public static async Task<string> GetAuthorizeToken(OAuthTokenEndpointContext context)
        //{
        //    string responseObj = string.Empty;
        //    using (var client = new HttpClient())
        //    {
        //        HttpContent requestParams = new FormUrlEncodedContent(context.Properties.Dictionary);

        //        HttpResponseMessage response = new HttpResponseMessage();
        //        // HTTP POST  
        //        response = await client.PostAsync("Token", requestParams).ConfigureAwait(false);

        //        // Verification  
        //        if (response.IsSuccessStatusCode)
        //        {  
        //             // Reading Response.  
        //        }
        //    }
        //    return responseObj;
        //}

        //public static async Task<string> GetAuthorizeToken()
        //{
        //    //// Acquire the access token.
        //    //string[] scopes = new string[] { "user.read" };
        //    //string accessToken = await GetAccessTokenForUserAsync(scopes);
        //    //// Use the access token to call a protected web API.
        //    //HttpClient client = new HttpClient();
        //    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        //    string baseAddress = "http://localhost:47970/";
        //    //Initialization.
        //    string responseObj = string.Empty;
        //    //oauthv2accesstoken.GenerateAccessToken.access_token
        //    // Posting.  
        //    using (var client = new HttpClient())
        //    {
        //        // Setting Base address.  
        //        //http://localhost:47970/Token
        //        //client.BaseAddress = new Uri("/api/Account/ExternalLogin");
        //        //client.BaseAddress = new Uri("http://localhost:47970/Token");
        //        // Setting content type.  
        //        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes("xyz:secretKey"));
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", authorizationHeader);
        //        var form = new Dictionary<string, string>
        //       {
        //           {"grant_type", "password"},
        //           {"username", "jeevan.pinnireddy@Ford.com"},
        //           {"password", "jkp@gd123"},
        //       };
        //        var tokenResponse = client.PostAsync(baseAddress + "Token", new FormUrlEncodedContent(form)).Result;
        //        var token = tokenResponse.Content.ReadAsAsync<Models.Token>(new[] { new JsonMediaTypeFormatter() }).Result;
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        //        var authorizedResponse = client.GetAsync(baseAddress + "test").Result;
        //        Console.WriteLine(authorizedResponse);
        //        Console.WriteLine(authorizedResponse.Content.ReadAsStringAsync().Result);
        //        //string baseAddress = "http://localhost:47970/";
        //        // using (var client = new HttpClient())
        //        // {
        //        //     var data = new Dictionary<string, string>
        //        //{
        //        //    {"grant_type", "password"},
        //        //    {"username", "jeevan.pinnireddy@Ford.com"},
        //        //    {"password", "jkp@gd123"},
        //        //};
        //        //     var tokenResponse = client.PostAsync(baseAddress + "/Token", new FormUrlEncodedContent(data)).Result;
        //        //     //var token = tokenResponse.Content.ReadAsStringAsync().Result;  
        //        //     var token = tokenResponse.Content.ReadAsAsync<Models.Token>(new[] { new JsonMediaTypeFormatter() }).Result;
        //        //     if (string.IsNullOrEmpty(token.AccessToken))
        //        //     {
        //        //         Console.WriteLine("Token issued is: {0}", token.AccessToken);
        //        //     }
        //        //     else
        //        //     {
        //        //         Console.WriteLine("Error : {0}", token);
        //        //     }
        //        // }
        //        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        //        //5HbRp9vVusBxS9XGq-XlerF6zt260JwZsRLrsR_tiGLvs4qmoeEkt_E6UHbXquqOz8OJQ4GoOu7pt_Ax_IDItxbuOjGKsO4ydOltw0nsBvAxZgPPWD9Pt4gs6Mf0l8KvRb0q8Q3Wz9BoxD7FYbIc7l9TqT9a5a84KVmNS6iQIo_Fqst5o8JC6fUga-mjBZcnl34qjTu-VG-4ajZG-Zlz2B0Nl_01s2sgNPsicoL6wuzMBe08sezFNc4fa5x70QC3kaoWRcxnqjEKc8vLN6CMUQ
        //        // Initialization.  
        //        HttpResponseMessage response = new HttpResponseMessage();
        //        List<KeyValuePair<string, string>> allIputParams = new List<KeyValuePair<string, string>>();
        //        // Convert Request Params to Key Value Pair.  
        //        // URL Request parameters.  
        //        HttpContent requestParams = new FormUrlEncodedContent(allIputParams);
        //        // HTTP POST  
        //        response = await client.PostAsync("Token", requestParams).ConfigureAwait(false);
        //        // Verification  
        //        if (response.IsSuccessStatusCode)
        //        {
        //            // Reading Response.   
        //        }
        //    }
        //    return responseObj;
        //}

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
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

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