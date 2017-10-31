using QAutomation.Core.Domain;

namespace QAutomation.Core.Services
{
    public sealed class ServiceResponse<TDomainModel>
    where TDomainModel : DomainModelBase
    {
        public ServiceResponse(TDomainModel model, ServiceRequestType requestType)
        {
            Model = model;
            Result = ServiceResponseResult.Unknown;
            RequestType = requestType;
        }
        public string ErrorMessage { get; set; }

        public ServiceRequestType RequestType { get; set; }
        public ServiceResponseResult Result { get; set; }
        public TDomainModel Model { get; set; }

    }
}