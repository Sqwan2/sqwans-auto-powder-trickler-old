using System;
using System.Windows;
using System.Windows.Input;
using System.IO.Ports;
using System.Threading;
using System.Windows.Controls;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Windows.Media.Media3D;

namespace AutoTricklerGui
{
    public partial class MainWindow : Window
    {
        private int count = 0;
        SerialPortWrapper serialPort = new SerialPortWrapper("COM4", 9600);
        private delegate void SetTextDeleg(string text);
        public bool isEnabled = false;

        string scaleValueStr = "";

        bool semaphore = true;

        private ScaleData _scaleData = new ScaleData();

        public MainWindow()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comPorts.Items.Add(port);
                comPortsTrickler.Items.Add(port);
            }

            serialPort.SerialConnection.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);

            this.DataContext = _scaleData;

            new Thread(readScaleValue).Start();
        }

        private void readScaleValue()
        {
            while (true)
            {
                if (semaphore)
                {
                    try
                    {
                        semaphore = false;
                        byte[] bytesToSend = { 0x1B, 0x70 };
                        serialPort.SerialConnection.Write(bytesToSend, 0, bytesToSend.Length);
                    }catch { }
                }
                Thread.Sleep(5);
            }
        }

        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[1024];
            int bytesRead = serialPort.SerialConnection.Read(data, 0, data.Length);
            string scaleValuePart = Encoding.ASCII.GetString(data, 0, bytesRead);

            scaleValueStr += scaleValuePart;
            if (scaleValueStr.Contains("\n"))
            {

                string valueNumberPart = scaleValueStr.Substring(3, 7).Replace('.', ',');
                _scaleData.CurrentScaleValue = Convert.ToDecimal(valueNumberPart);
                if (scaleValueStr.StartsWith('-'))
                {
                    _scaleData.CurrentScaleValue = _scaleData.CurrentScaleValue * -1;
                }

                //Dispatcher.BeginInvoke(new SetTextDeleg(si_DataReceived), new object[] { scaleValueStr });
                scaleValueStr = "";
                semaphore = true;
            }
        }
        private void si_DataReceived(string data) 
        {
            string valueNumberPart = data.Substring(3,7).Replace('.',',');
            _scaleData.CurrentScaleValue = Convert.ToDecimal(valueNumberPart);
            if(data.StartsWith('-'))
            {
                _scaleData.CurrentScaleValue = _scaleData.CurrentScaleValue * -1;
            }
        }

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            _scaleData.addScaleValue(_scaleData.CurrentScaleValue);
            /*
            decimal powderQtyDouble = 0.0M;
            
            try
            {
                powderQtyDouble = Convert.ToDecimal(powderQty.Text);
                _serialPort.WriteLine("TARA");
                
                new Thread(() => {
                    Thread.Sleep(3000);
                    while (Decimal.Compare(scaleValue, powderQtyDouble) < 0)
                    {
                        _serialPort.WriteLine("Add");
                        Thread.Sleep(50);
                    }
                }).Start();

                this.count++;
                Stk.Content = count + " Stk";
            }
            catch {
                MessageBox.Show("Ihre Eingabe ist keine Zahl", "Fehler im Zahlenformat");
                return;
            }
            */
        }

        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            _scaleData.ResetScaleValueList();
            //this.count = 0;
            //Stk.Content = count + " Stk";
        }

        private void powderQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            if (e.Text.Equals(",")) {
                if(powderQty.Text.Contains(','))
                {
                    e.Handled = true;
                    return;
                }
            }

            foreach (var ch in e.Text)
            {
                if (!(Char.IsDigit(ch) || ch.Equals(',')))
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void taraBtn_Click(object sender, RoutedEventArgs e)
        {
            byte[] bytesToSend = { 0x1B, 0x74 };
            serialPort.SerialConnection.Write(bytesToSend, 0, bytesToSend.Length);
        }
    }
}
