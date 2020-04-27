using System;

namespace SerialConnection
{
    public class EventHandlerParam : EventArgs
    {
        public object Properties { get; set; } = new object();
    }
}
