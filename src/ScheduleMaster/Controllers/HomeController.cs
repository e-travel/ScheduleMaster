using ScheduleMaster.Component;
using ScheduleMaster.Models.Entities;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ScheduleMaster.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public ViewResult Index()
        {
            

            return View();
        }
        
    }
}