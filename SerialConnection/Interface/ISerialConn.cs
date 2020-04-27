using System;

namespace SerialConnection.Interface
{
    public interface ISerialConn
    {
        event EventHandler<EventHandlerParam> DataReceived;

        string ComPortName { get; set; }
        int BaudRate { get; set; }
        bool IsOpen { get; }
        int BytesToRead { get; }

        void Init();
        void Open();
        void Close();
        int Read(byte[] buffer, int offset, int count);
        void Write(byte[] buffer, int offset, int count);
        void Flush();
    }
}
