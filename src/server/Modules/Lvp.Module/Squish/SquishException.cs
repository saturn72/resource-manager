using System;

namespace Lvp.Module.Squish
{
    public class SquishException : Exception
    {
        public SquishException(string message)
            : base(message)
        {
        }
    }
}