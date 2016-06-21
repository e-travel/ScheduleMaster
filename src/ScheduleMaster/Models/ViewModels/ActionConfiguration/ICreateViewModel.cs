using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleMaster.Models.ViewModels.ActionConfiguration
{
    public interface ICreateViewModel
    {
        ActionType ActionType { get; set; }
    }
}
