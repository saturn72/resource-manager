using System.Threading.Tasks;
using LabManager.Common.Domain.Command;
using Saturn72.Core.Services;

namespace LabManager.Services.Command
{
    public interface ICommandResourceService
    {
        Task<ServiceResponse<CommandModel>> SendCommand(CommandModel command);
    }
}
