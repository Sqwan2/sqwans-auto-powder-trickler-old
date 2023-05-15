using System;
using System.Windows;
using System.Windows.Input;
using System.IO.Ports;


namespace AutoTricklerGui
{
    public partial class MainWindow : Window
    {
        private int count = 0;

        public MainWindow()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comPorts.Items.Add(port);
                comPortsTrickler.Items.Add(port);
            }
        }

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            double powderQtyDouble;
            try
            {
                powderQtyDouble = Convert.ToDouble(powderQty.Text);
            }
            catch {
                MessageBox.Show("Ihre Eingabe ist keine Zahl", "Fehler im Zahlenformat");
            }
            this.count++;
            Stk.Content = count + " Stk";
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
    }
}
