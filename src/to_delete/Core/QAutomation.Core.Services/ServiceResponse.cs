namespace QAutomation.Core.Services
{
    public sealed class ServiceResponse<TResponseObject>
    {
        public ServiceResponse(TResponseObject model, ServiceRequestType requestType)
        {
            Model = model;
            Result = ServiceResponseResult.Unknown;
            RequestType = requestType;
        }
        public string ErrorMessage { get; set; }

        public ServiceRequestType RequestType { get; set; }
        public ServiceResponseResult Result { get; set; }
        public TResponseObject Model { get; set; }
    }
}