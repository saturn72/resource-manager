using System;
using Lvp.Module.Squish;

namespace Lvp.Module
{
    internal class SquishFacade
    {
        internal static Automation SquishAgent { get; private set; }

        public static void EvalAndUnref(string squishCommand, Action<ObjectRef> action)
        {
            throw new NotImplementedException();
        }

        public static void Init(Automation squishAgent)
        {
            SquishAgent = squishAgent;
        }
    }
}