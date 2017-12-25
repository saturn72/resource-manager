namespace Lvp.Module.Squish
{
    public class ObjectRef
    {
        public Automation AutomationEnvironment { get; }

        public RefType Type { get; }

        public string Value { get; }

        public ObjectRef(Automation automation, string val, RefType type)
        {
            AutomationEnvironment = automation;
            Type = type;
            Value = val;
        }

        public override string ToString()
        {
            return Value;
        }

        public ObjectRef Call(string member)
        {
            return AutomationEnvironment.Eval(ToString() + "." + member);
        }

        public void Unref()
        {
            if (Type != RefType.Object)
                return;
            AutomationEnvironment.Eval("SquishCmd.unref('" + Value + "')");
        }
    }
}