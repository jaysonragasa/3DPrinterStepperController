using RJCP.IO.Ports;
using SerialConnection.Interface;
using System;

namespace SerialConnection.SerialProviders
{
    public class RJCPSerialPortStream : ISerialConn
    {
        SerialPortStream _stream = new SerialPortStream();

        public event EventHandler<EventHandlerParam> DataReceived;

        public string ComPortName { get; set; } = "COM12";

        public int BaudRate { get; set; } = 520000;

        public bool IsOpen
        {
            get { return _stream.IsOpen; }
        }

        public int BytesToRead
        {
            get
            {
                return _stream.BytesToRead;
            }
        }

        public void Init()
        {
            _stream = new SerialPortStream(this.ComPortName, this.BaudRate)
            {

            };

            _stream.DataReceived += _stream_DataReceived;
        }

        public void Close()
        {
            _stream.Close();
        }

        public void Open()
        {
            _stream.Open();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        public void Flush()
        {
            _stream.Flush();
        }

        private void _stream_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            this.DataReceived.Invoke(_stream, new EventHandlerParam()
            {
                Properties = e
            });
        }
    }
}
