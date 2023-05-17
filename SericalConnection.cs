using System.IO.Ports;

namespace AutoTricklerGui
{
    class SerialPortWrapper
    {
        private SerialPort _serialPort;

        public SerialPort SerialConnection {
            get { return _serialPort; }
            private set { }
        }

        public SerialPortWrapper(string COMPort, int baud)
        {
            _serialPort = new SerialPort(COMPort, baud, Parity.None, 8, StopBits.One);
            _serialPort.Handshake = Handshake.None;
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            _serialPort.Open();
        }
    }
}
