using GCode.Helpers;
using SerialConnection.Interface;
using SerialConnection.SerialProviders;
using System.Configuration;
using System.Windows;

namespace GCode
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region properties
        public MarlinSettings MarlinSettings { get; set; } = new MarlinSettings();

        public ISerialConn SerialPortStream { get; set; } = null;

        string PortName { get; set; } = "COM12"; // default to COM2

        int BaudRate { get; set; } = 250000;
        #endregion

        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.PortName = ConfigurationManager.AppSettings["PortName"];
            this.BaudRate = int.Parse(ConfigurationManager.AppSettings["BaudRate"]);

            this.SerialPortStream = new RJCPSerialPortStream()
            {
                ComPortName = this.PortName,
                BaudRate = this.BaudRate
            };
            this.SerialPortStream.Init();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            this.SerialPortStream.Close();
        }
    }
}
