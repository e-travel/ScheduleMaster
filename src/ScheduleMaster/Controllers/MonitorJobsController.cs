using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ScheduleMaster.Models.ViewModels.MonitorJobs;
using System.Data.Entity;
using System.Collections.Generic;
using ScheduleMaster.Models.Entities;
using ScheduleMaster.Models.ViewModels;
using ScheduleMaster.Component;
using NLog;
using System;

namespace ScheduleMaster.Controllers
{
    public class MonitorJobsController : BaseController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: MonitorJobs
        [HttpGet]
        public ViewResult Index(string searchTerm)
        {
            var viewModel = CreateIndexViewModel(searchTerm);

            return View(viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id, string searchTerm)
        {
            if (id.HasValue)
            {
                var job = await Database.JobConfigurations.SingleOrDefaultAsync(j => j.Id == id.Value);

                if (job == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    var viewModel = new JobDetailsViewModel
                    {
                        JobConfiguration = job
                    };

                    return View(viewModel);
                }
            }
            else
            {
                return RedirectToAction("Create", new { searchTerm = searchTerm });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<ActionResult> Edit(JobDetailsViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    return await InsertOrUpdate(viewModel, EntityState.Modified);
                }
                else
                {

                    viewModel.JobConfiguration.ActionConfigurations = Database.ActionConfigurations
                        .Where(a => a.JobConfigurationId == viewModel.JobConfiguration.Id)
                        .ToList();

                    return View(viewModel);
                }
            }
            catch(Exception exception)
            {
                logger.Error(exception);
                throw;
            }            
        }

        [HttpGet]
        public ViewResult Create(string searchTerm)
        {
            var viewModel = new JobDetailsViewModel
            {
                SearchTerm = searchTerm,
                JobConfiguration = new JobConfiguration()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<ActionResult> Create(JobDetailsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                return await InsertOrUpdate(viewModel, EntityState.Added);
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            var job = await Database.JobConfigurations.SingleOrDefaultAsync(j => j.Id == id);
            if (job != null)
            {
                Database.JobConfigurations.Remove(job);
            }
            await Database.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        private async Task<ActionResult> InsertOrUpdate(JobDetailsViewModel viewModel, EntityState entityState)
        {
            Database.Entry(viewModel.JobConfiguration).State = entityState;

            await Database.SaveChangesAsync();

            HangfireAdapter.Deactivate(viewModel.JobConfiguration.Id);

            if (viewModel.JobConfiguration.IsEnabled)
            {
                HangfireAdapter.Activate(viewModel.JobConfiguration.Id, viewModel.JobConfiguration.CronExpression);
            }

            ViewBag.Messages = new List<MessageViewModel>()
                {
                    new MessageViewModel {CssClass="alert alert-info", Content = "Job Saved!" }
                };

            return RedirectToAction("Index", new { searchTerm = viewModel.SearchTerm });
        }


        private IndexViewModel CreateIndexViewModel(string searchTerm)
        {
            var query = Database.JobConfigurations.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(j => j.Name.Contains(searchTerm));
            }

            var viewModel = new IndexViewModel
            {
                SearchTerm = searchTerm,
                JobConfigurations = query
            };
            return viewModel;
        }
    }
}