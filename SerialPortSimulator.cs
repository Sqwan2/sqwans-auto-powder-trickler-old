using System;
using System.IO.Ports;
using System.Runtime.CompilerServices;

namespace SerialPortSimulator
{
    class SerialPortSimulator
    {
        public static void Main()
        {
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();
            decimal scaleValue = 0.0M;

            Console.WriteLine("The following serial ports were found:");

            // Display each port name to the console.
            foreach (string port in ports)
            {
                Console.WriteLine(port);
            }
            
            SerialPort _serialPort = new SerialPort("COM5", 9600, Parity.None, 8, StopBits.One);
            _serialPort.Handshake = Handshake.None;
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            _serialPort.Open();
            
            while (true)
            {
                try
                {
                    _serialPort.WriteLine(scaleValue.ToString());
                    string message = _serialPort.ReadLine();
                    if(message.Equals("Add",StringComparison.OrdinalIgnoreCase))
                    {
                           Random rnd = new Random();
                        int a = rnd.Next(4);
                        string b = "0," + a;
                        decimal c = Convert.ToDecimal(b);
                        
                        //scaleValue = scaleValue + 0.1M;
                        scaleValue = scaleValue + c;
                        Console.WriteLine(scaleValue);
                    }
                    else if (message.Equals("TARA", StringComparison.OrdinalIgnoreCase))
                    {
                        scaleValue = 0.0M;
                        _serialPort.WriteLine(scaleValue.ToString());
                    }
                    
                    Console.WriteLine(message);
                }
                catch (TimeoutException e)
                {
                    //Console.WriteLine(e.ToString());
                }
            }

        }
    }
}
