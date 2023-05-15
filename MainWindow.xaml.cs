using System;
using System.Windows;
using System.Windows.Input;
using System.IO.Ports;
using System.Threading;
using System.Windows.Controls;

namespace AutoTricklerGui
{
    public partial class MainWindow : Window
    {
        private int count = 0;
        static SerialPort _serialPort;
        private delegate void SetTextDeleg(string text);
        decimal scaleValue = 0;
        public bool isEnabled = false;

        public MainWindow()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comPorts.Items.Add(port);
                comPortsTrickler.Items.Add(port);
            }

            _serialPort = new SerialPort("COM6", 9600, Parity.None, 8, StopBits.One);
            _serialPort.Handshake = Handshake.None;
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            _serialPort.Open();

        }

        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string scaleValue = _serialPort.ReadLine();
            Dispatcher.BeginInvoke(new SetTextDeleg(si_DataReceived), new object[] { scaleValue });
        }
        private void si_DataReceived(string data) 
        {
            scaleValue = Convert.ToDecimal(data.Trim());
            scaleValueLabel.Text = data.Trim();
        }

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
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
        }

        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            this.count = 0;
            Stk.Content = count + " Stk";
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
            _serialPort.WriteLine("TARA");
        }
    }
}
