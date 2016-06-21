using ScheduleMaster.DataAccess;
using System.Web.Mvc;

namespace ScheduleMaster.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly ScheduleMasterContext _database;

        protected internal virtual ScheduleMasterContext Database { get { return _database; } }

        protected BaseController()
        {
            _database = new ScheduleMasterContext();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && _database != null)
                _database.Dispose();
        }
    }
}