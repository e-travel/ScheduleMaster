using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using ScheduleMaster.Models.Entities;
using ScheduleMaster.Models.ViewModels;
using ScheduleMaster.Models.ViewModels.ActionConfiguration;
using ActionConfiguration = ScheduleMaster.Models.Entities.ActionConfiguration;
using ActionType = ScheduleMaster.Models.ViewModels.ActionConfiguration.ActionType;

namespace ScheduleMaster.Controllers
{
    public class ActionController :  BaseController
    {
        [HttpGet]
        public ActionResult Create(ActionType actionType, int jobId)
        {
            ICreateViewModel viewModel  = null;
            switch (actionType)
            {
                case ActionType.EmailActionConfiguration:

                    viewModel = new CreateEmailActionViewModel
                    {
                        ActionType = actionType,
                        ActionConfiguration = new EmailActionConfiguration
                        {
                            JobConfigurationId = jobId
                        }
                    };
                    break;
                case ActionType.HipchatActionConfiguration:
                    viewModel = new CreateHipChatActionViewModel
                    {
                        ActionType = actionType,
                        ActionConfiguration = new HipchatActionConfiguration()
                        {
                            JobConfigurationId = jobId
                        }
                    };
                    break;
                default:
                    throw new NotSupportedException();
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> CreateHipChat(CreateHipChatActionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                switch (viewModel.ActionType)
                {
                    case ActionType.HipchatActionConfiguration:
                        return await InsertOrUpdate<CreateHipChatActionViewModel, HipchatActionConfiguration>(viewModel, EntityState.Added);
                }
            }
            return View("Create",viewModel);
        }
        [HttpPost]
        public async Task<ActionResult> Create(CreateEmailActionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                switch (viewModel.ActionType)
                {
                        case ActionType.EmailActionConfiguration:
                        return await InsertOrUpdate<CreateEmailActionViewModel,EmailActionConfiguration>(viewModel, EntityState.Added);
                }

                
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? id, ActionType actionType)
        {
          
            ICreateViewModel viewModel = null;
            if (id.HasValue)
            {
                var action = await Database.ActionConfigurations.SingleOrDefaultAsync(j => j.Id == id.Value);

                if (action == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    switch (actionType)
                    {
                        case ActionType.EmailActionConfiguration:
                            viewModel = new CreateEmailActionViewModel
                            {
                                ActionType = actionType,
                                ActionConfiguration = (EmailActionConfiguration)action
                            };
                            break;
                        case ActionType.HipchatActionConfiguration:
                            viewModel = new Models.ViewModels.ActionConfiguration.CreateHipChatActionViewModel()
                            {
                                ActionType = actionType,
                                ActionConfiguration = (HipchatActionConfiguration)action
                            };
                            break;
                    }
               
                    return View(viewModel);
                }
            }
            else
            {
                return RedirectToAction("Create", new { actionType = actionType });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(CreateEmailActionViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                return await InsertOrUpdate<CreateEmailActionViewModel, EmailActionConfiguration>(viewModel, EntityState.Modified);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditHipChat(CreateHipChatActionViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                return await InsertOrUpdate<CreateHipChatActionViewModel, HipchatActionConfiguration>(viewModel, EntityState.Modified);
            }

            return View("Edit",viewModel);
        }
        

        [HttpGet]
        public async Task<ActionResult> Delete(int? id, int jobConfigurationId)
        {
            var action = await Database.ActionConfigurations.SingleOrDefaultAsync(j => j.Id == id);
            if (action != null)
            {
                Database.ActionConfigurations.Remove(action);
            }
            await Database.SaveChangesAsync();
            return RedirectToAction("Edit", "MonitorJobs",
                new { id = jobConfigurationId, searchTerm = string.Empty });
        }



        private async Task<ActionResult> InsertOrUpdate<T,TParam>(T viewModel, EntityState entityState)
            where T:CreateViewModel<TParam>
            where TParam: ActionConfiguration, new()
        {
            Database.Entry(viewModel.ActionConfiguration).State = entityState;

            await Database.SaveChangesAsync();

            ViewBag.Messages = new List<MessageViewModel>()
                {
                    new MessageViewModel {CssClass="alert alert-info", Content = "Action Saved!" }
                };

            return RedirectToAction("Edit", "MonitorJobs",
                new {id = viewModel.ActionConfiguration.JobConfigurationId, searchTerm = string.Empty});
        }
    }
}