using System.Threading.Tasks;
using LabManager.Common.Domain.Command;
using Saturn72.Core.Services;
using Saturn72.Extensions;

namespace LabManager.Services.Command
{
    public class CommandResourceService : ICommandResourceService
    {
        public Task<ServiceResponse<CommandModel>> SendCommand(CommandModel command)
        {
            var serviceResponse = new ServiceResponse<CommandModel>(ServiceRequestType.Command)
            {Model = command};
            ValidateModelBeforeCommand(serviceResponse);
            if (serviceResponse.HasErrors())
                return Task.FromResult(serviceResponse);

            throw new System.NotImplementedException();
        }

        private static void ValidateModelBeforeCommand(ServiceResponse<CommandModel> serviceResponse)
        {
            var model = serviceResponse.Model;
            if (model.IsNull() || 
                !model.SessionId.HasValue() ||
                !model.ResourceId.HasValue())
            {
                serviceResponse.ErrorMessage = "Missing model data or null model.";
                serviceResponse.Result = ServiceResponseResult.Fail;
            }

#warning Validate session + user 
        }
    }
}