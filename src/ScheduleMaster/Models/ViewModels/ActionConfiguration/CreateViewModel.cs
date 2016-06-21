using ScheduleMaster.Models.Entities;

namespace ScheduleMaster.Models.ViewModels.ActionConfiguration
{
    public class CreateViewModel<T>:ICreateViewModel
        where T: Entities.ActionConfiguration, new()
    {
        public ActionType ActionType { get; set; }
        public T ActionConfiguration { get; set; }
        
    }
}