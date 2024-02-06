using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RoboKinematics
{
    class RobotConnection
    {
        private Socket _socket;
        private NetworkStream _stream;
        private StreamReader _reader;
        private readonly string _host;
        private readonly int _port;
        private bool _connected;

        public string Host { get { return _host; } }
        public int Port { get { return _port; } }
        public bool Connected { get { return _connected; } }

        public RobotConnection(string host, int port)
        {
            _host = host;
            _port = port;
            _connected = false;
        }

        public void Connect()
        {
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true,
                ReceiveTimeout = 10000
            };
            _socket.Connect(_host, _port);
            _stream = new NetworkStream(_socket);
            _reader = new StreamReader(_stream);
            _connected = true;
            Console.WriteLine("Сработал Connect()");
        }

        public void Disconnect()
        {
            _connected = false;
            _socket.Disconnect(true);
            _socket.Dispose();
            _socket = null;
            _stream.Dispose();
            _stream = null;
            _reader.Dispose();
            _reader = null;
        }

        public void Execute(string command)
        {
            var statusCode = Send(command);
            if (statusCode.AnyOf(0xFF, 0xFE, 0xFD, 0xFC, 0xF1, 0xF2, 0xF3, 0xF4))
            {
               // throw new RobotConnectionException(Utils.StatusCodeToString(statusCode));
            }

        }

        public string Execute2(string command)
        {
            var statusCode = Send(command);
            
            //Console.WriteLine("statusCode "+ asString);
           if (statusCode.AnyOf(0xFF, 0xFE, 0xFD, 0xFC, 0xF1, 0xF2, 0xF3, 0xF4))
            //if (statusCode == 1)
                {
                // throw new RobotConnectionException(Utils.StatusCodeToString(statusCode));
               // Console.WriteLine("Сработало");
            }
            var resultString = _reader.ReadLine();
        
            //Console.WriteLine(resultString);
            return resultString;
        }


        // получить результат состояния робота
        public double[] ExecuteWithResult(string command)
        {
            var statusCode = Send(command);
        
             if (statusCode.AnyOf(0xFF, 0xFE, 0xFD, 0xFC, 0xF0, 0xF2, 0xF3, 0xF4))
           
            {
                //throw new RobotConnectionException(Utils.StatusCodeToString(statusCode));
                
            }

            var resultString = _reader.ReadLine().Trim();
            if (resultString[0] < 33 || resultString[0] > 57)
            {
                resultString = resultString.Substring(1);
            }
            if (string.IsNullOrEmpty(resultString))
            {
                throw new RobotConnectionException("Сервер вернул статус 'F1' но результат пуст");
            }

            var split = resultString.Split(';').Where(s => !string.IsNullOrEmpty(s)).ToArray();
             return split.Select(s => double.Parse(s, CultureInfo.InvariantCulture)).Select(d => Math.Round(d, 2)).ToArray();
            //return resultString;
        }
        // отправить команду на робота
        private int Send(string command)
        {
           

            try
            {
                if (!_connected) throw new RobotConnectionException("Невозможно выполнить отправку. Соединение не установлено");
                var bytes = Encoding.ASCII.GetBytes(command + Environment.NewLine);
                _stream.Write(bytes, 0, bytes.Length);
                 return _stream.ReadByte();
                
            }
            catch
            {
                Connect();
                if (!_connected) throw new RobotConnectionException("Невозможно выполнить отправку. Соединение не установлено");
                var bytes = Encoding.ASCII.GetBytes(command + Environment.NewLine);
                _stream.Write(bytes, 0, bytes.Length);
                 return _stream.ReadByte();
                
            }


        }
    }


    public class RobotConnectionException : Exception
    {
        public RobotConnectionException(string message) : base(message)
        {
        }
    }
}
