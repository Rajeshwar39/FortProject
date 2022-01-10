using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FordAPI.Models;
using System.Data;
//using System.Web.Mvc;

using Newtonsoft.Json;
using System.Globalization;

using AutoMapper;
using System.Reflection;
using FordAPI.Helper;
using System.Configuration;


using System.Web.Http.Cors;

using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Web.Http.Description;

using System.Dynamic;
using System.Text;
using System.Web;
using Newtonsoft;
using FordAPI.DbHelper;

namespace FordAPI.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*", exposedHeaders: "X-My-Header")]
    public class ContentController : ApiController
    {
        public static string UserConnection
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["UserConnectionString"].ToString();
            }
        }

        [Route("api/v1/Content/GetUserInformation")]
        [HttpGet]
        
        [Authorize]
        public HttpResponseMessage GetUserInformation()
        {
            var obj = new AddonsClass();
            var users = obj.GetUsers();
            var res = JsonConvert.SerializeObject(users.Tables[0]);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(res, Encoding.UTF8, "application/json");
            return response;

        }
        [Route("api/v1/Content/AddUsers")] 
        [HttpGet]
       
        [Authorize]
        //public HttpResponseMessage GetCompanyListing(/*string TokenID,*/ string DisplayName, string CompanyName = null, string CompanyID = null, string CompanyTicker = null, string ISIN = null, string FromDate = null, string ToDate = null, string PageNumber = "1", string PageSize = "100")
        public HttpResponseMessage AddUsers(string username, string password, string email)
        {
            var obj = new AddonsClass();
            var users = obj.addUserinfo(username, password, email);
            var res = JsonConvert.SerializeObject(users.Tables[0]);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(res, Encoding.UTF8, "application/json");
            return response;

        }

        [Route("api/v1/Content/AddCities")]
        [HttpPost]
        
        [Authorize]
        //public HttpResponseMessage GetCompanyListing(/*string TokenID,*/ string DisplayName, string CompanyName = null, string CompanyID = null, string CompanyTicker = null, string ISIN = null, string FromDate = null, string ToDate = null, string PageNumber = "1", string PageSize = "100")
        public HttpResponseMessage AddCities(string username, string city, string country)
        {
            var obj = new AddonsClass();
            var users = obj.addcityinfo(username, city, country);
            var res = JsonConvert.SerializeObject(users.Tables[0]);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(res, Encoding.UTF8, "application/json");
            return response;

        }

        [Route("api/v1/Content/ShowCities")]
        [HttpGet]
        
        [Authorize]
        //public HttpResponseMessage GetCompanyListing(/*string TokenID,*/ string DisplayName, string CompanyName = null, string CompanyID = null, string CompanyTicker = null, string ISIN = null, string FromDate = null, string ToDate = null, string PageNumber = "1", string PageSize = "100")
        public HttpResponseMessage ShowCities()
        {
            var obj = new AddonsClass();
            var users = obj.ShowCitiesInfo();
            var res = JsonConvert.SerializeObject(users.Tables[0]);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(res, Encoding.UTF8, "application/json");
            return response;

        }


        [Route("api/v1/Content/DeleteCities")]
        [HttpPut]
       
        [Authorize]
        //public HttpResponseMessage GetCompanyListing(/*string TokenID,*/ string DisplayName, string CompanyName = null, string CompanyID = null, string CompanyTicker = null, string ISIN = null, string FromDate = null, string ToDate = null, string PageNumber = "1", string PageSize = "100")
        public HttpResponseMessage DeleteCities(string username, string city, string country)
        {
            var obj = new AddonsClass();
            var users = obj.DeleteCitiesInfo(username, city, country);
            var res = JsonConvert.SerializeObject(users.Tables[0]);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(res, Encoding.UTF8, "application/json");
            return response;

        }
       

        
       

        
    }
}

