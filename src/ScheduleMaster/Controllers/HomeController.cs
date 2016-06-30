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

        [HttpGet]
        public ViewResult Status()
        {
            return View();
        }
    }
}