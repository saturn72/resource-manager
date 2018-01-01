using System;
using System.Runtime.Serialization;

namespace LabManager.Common.Exceptions
{
    [Serializable]
    public class ResourceManagerException : Exception
    {
        public ResourceManagerException()
        {
        }

        public ResourceManagerException(string message)
            : base(message)
        {
        }

        public ResourceManagerException(string messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
        }

        protected ResourceManagerException(SerializationInfo
            info, StreamingContext context)
            : base(info, context)
        {
        }

        public ResourceManagerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
