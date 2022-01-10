using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FordAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //return Redirect("/Technology/swagger/ui/index#/Content");
            return Redirect("/swagger/ui/index#/Content");
            //return Redirect("/ConsumerFonterraTest/swagger/ui/index#/Content");
        }
    }
}
