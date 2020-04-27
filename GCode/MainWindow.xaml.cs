using RJCP.IO.Ports;
using SerialConnection.SerialProviders;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace GCode
{
    /// <summary>
    /// This tool is for test purposes only.lvn,
    /// Codes are simplified as well as the UI
    /// </summary>
    public partial class MainWindow : Window
    {
        #region events

        #endregion

        #region vars
        DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Normal);
        #endregion

        #region commands

        #endregion

        #region ctors
        public MainWindow()
        {
            InitializeComponent();

            ((App)App.Current).SerialPortStream.DataReceived += SerialPortStream_DataReceived;

        }
        #endregion

        #region overrides
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            btnConnect.Click += BtnConnect_Click;
            btnDisconnect.Click += BtnDisconnect_Click;
            btnSend.Click += BtnSend_Click;

            sliderSpeed.ValueChanged += SliderPositions_ValueChanged;
            sliderX.ValueChanged += SliderPositions_ValueChanged;
            sliderY.ValueChanged += SliderPositions_ValueChanged;
            sliderZ.ValueChanged += SliderPositions_ValueChanged;

            btnHome.Click += BtnHome_Click;
            btnHomeX.Click += BtnHome_Click;
            btnHomeY.Click += BtnHome_Click;
            btnHomeZ.Click += BtnHome_Click;

            cbAutoPlayZ.Checked += CbAutoPlayZ_Checked;

            this._timer.Tick += _timer_Tick;
            this._timer.Interval = TimeSpan.FromMilliseconds(50);

            btnStartHunderthExtrude.Click += BtnStartHunderthExtrude_Click;
            txLeftToMark.TextChanged += TxLeftToMark_TextChanged;
            btnSaveECalibVal.Click += BtnSaveECalibVal_Click;
            btnStartTenExtrude.Click += BtnStartTenExtrude_Click;
        }

        private void BtnSaveECalibVal_Click(object sender, RoutedEventArgs e)
        {
            SendMessage("M92 E" + txCalibValue.Text);
            SendMessage("M500");
            SendMessage("M503");
        }

        private void TxLeftToMark_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                double extrusion_length = 120 - double.Parse(txLeftToMark.Text);
                double current_e_step = ((App)App.Current).MarlinSettings.Step_E;
                double steps_taken = current_e_step * 100;
                double correct_e_step = steps_taken / extrusion_length;
                txCalibValue.Text = correct_e_step.ToString("#.##");
            }
            catch(Exception ex)
            {

            }
        }

        private void BtnStartTenExtrude_Click(object sender, RoutedEventArgs e)
        {
            SendMessage("M83");
            SendMessage("G1 E10 F100");
        }

        private void BtnStartHunderthExtrude_Click(object sender, RoutedEventArgs e)
        {
            SendMessage("M83");
            SendMessage("G1 E100 F100");
        }

        private void CbAutoPlayZ_Checked(object sender, RoutedEventArgs e)
        {
            //sliderZ.IsEnabled = !cbAutoPlayZ.IsChecked.Value;

            if (cbAutoPlayZ.IsChecked.Value) this._timer.Start();
            if (!cbAutoPlayZ.IsChecked.Value) this._timer.Stop();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            CloseConnection();
        } 
        #endregion

        #region event subscriptions
        int i = 1;
        private void _timer_Tick(object sender, EventArgs e)
        {
            if (!cbAutoPlayZ.IsChecked.Value) return;

            if (sliderZ.Value >= sliderZ.Maximum) i = -1;
            if (sliderZ.Value <= sliderZ.Minimum) i = 1;

            sliderZ.Value += i;
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            if (sender == btnHome)
            {
                SendMessage("G28");
            }
            else if (sender == btnHomeX)
            {
                SendMessage("G28 X");
            }
            else if (sender == btnHomeY)
            {
                SendMessage("G28 Y");
            }
            else if (sender == btnHomeZ)
            {
                SendMessage("G28 Z");
            }
        }

        private void SliderPositions_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string newposition = string.Empty;
            newposition = $"G1 X{sliderX.Value} Y{sliderY.Value} Z{sliderZ.Value} F{sliderSpeed.Value}";

            //if(sender == sliderSpeed)
            //{
            //    this._timer.Interval = TimeSpan.FromMilliseconds(10);
            //}

            SendMessage(newposition);
        }

        private void SerialPortStream_DataReceived(object sender, SerialConnection.EventHandlerParam e)
        {
            var p = (SerialDataReceivedEventArgs)e.Properties;

            if (!((App)App.Current).SerialPortStream.IsOpen) return;

            const int blockSize = 8192;
            byte[] readBuffer = new byte[((App)App.Current).SerialPortStream.BytesToRead];

            ((App)App.Current).SerialPortStream.Read(readBuffer, 0, readBuffer.Length);

            using (MemoryStream ms = new MemoryStream(readBuffer))
            {
                using (StreamReader reader = new StreamReader(ms))
                {
                    string log = reader.ReadToEnd();

                    Debug.WriteLine(log + "-----");

                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (log.Length > 5 && log.Substring(0, 5) == "start")
                        {
                            ((App)App.Current).MarlinSettings.ReadSettings(log);

                            lblCurrEStep.Text = ((App)App.Current).MarlinSettings.Step_E.ToString();
                        }

                        AddToLog(log);
                    }), System.Windows.Threading.DispatcherPriority.Normal, null);
                }
            }
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (((App)App.Current).SerialPortStream.IsOpen) return;

            ((App)App.Current).SerialPortStream.Open();
        }

        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (!((App)App.Current).SerialPortStream.IsOpen) return;

            CloseConnection();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage(txSend.Text);
        }
        #endregion

        #region command methods

        #endregion

        #region methods
        void SendMessage(string msg, bool append_new_line = true)
        {
            if (!((App)App.Current).SerialPortStream.IsOpen) return;

            string message = msg + (append_new_line ? "\n" : null);
            AddToLog(message);

            var bWrite = Encoding.ASCII.GetBytes(message);

            ((App)App.Current).SerialPortStream.Write(bWrite, 0, bWrite.Length);
            ((App)App.Current).SerialPortStream.Flush();
        }

        void CloseConnection()
        {
            if (((App)App.Current).SerialPortStream.IsOpen)
            {
                ((App)App.Current).SerialPortStream.Flush();
                ((App)App.Current).SerialPortStream.Close();
            }
        }

        void AddToLog(string log)
        {
            txConsoleLog.Text += log;
            txConsoleLogScroll.ScrollToBottom();
        }
        #endregion
    }
}
