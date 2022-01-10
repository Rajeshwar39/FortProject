using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FordAPI.Models;
using System.Web.Http.Description;
using System.Web;
using System.Text;
using FordAPI.Repository;
using System.Configuration;
namespace FordAPI.Controllers
{
    public class TokenController : ApiController
    {
        static TokenRepository repository = new TokenRepository();
        private string SiteId = ConfigurationManager.AppSettings["SiteId"];
        private string SiteName = ConfigurationManager.AppSettings["SiteName"];
        
    }
}
