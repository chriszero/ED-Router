using System;

namespace ED_Router.Events
{
    public class SetVariableEventArgs : EventArgs
    {
        public Type ValueType { get; set; }
        public object Value { get; set; }
        public string VariableName { get; set; }
    }
}