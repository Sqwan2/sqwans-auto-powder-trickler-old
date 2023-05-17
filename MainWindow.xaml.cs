using System;
using System.Windows;
using System.Windows.Input;
using System.IO.Ports;
using System.Threading;

namespace AutoTricklerGui
{
    public partial class MainWindow : Window
    {
        private SerialPortWrapper serialPort;
        private ScaleData _scaleData;
        private ScaleController scaleController;
        private delegate void SetTextDeleg(string text);
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
            serialPort = new SerialPortWrapper("COM4", 9600);
            _scaleData = new ScaleData();
            scaleController = new ScaleController(serialPort, _scaleData);

            serialPort.SerialConnection.DataReceived += new SerialDataReceivedEventHandler(scaleController.ScaleDataReceivedHandler);

            this.DataContext = _scaleData;

            new Thread(scaleController.requestScaleValue).Start();
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
            scaleController.tara();
        }
    }
}
