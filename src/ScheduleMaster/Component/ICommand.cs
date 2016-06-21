using System.Threading.Tasks;

namespace ScheduleMaster.Component
{
    public interface ICommand
    {
        Task<bool> ExecuteAsync();
    }
}