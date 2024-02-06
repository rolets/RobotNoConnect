using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace RoboKinematics
{
    [XmlRoot("Robot")]
    public class Robot
    {
        private readonly RobotConnection _connection;

        [XmlAttribute(AttributeName = "ip")]
        public string IP { get; set; }
        [XmlAttribute(AttributeName = "port")]
        public int Port { get; set; }

       
        public RobotArm LeftArm { get; set; }
        public RobotArm RightArm { get; set; }

        [XmlIgnore]
        public int Version { get; private set; }

        [XmlIgnore]
        public bool Connected
        {
            get
            {
                if (!_connection.Connected) return false;
                //var version = _connection.ExecuteWithResult("VERSION:SHELL");
                return true;
            }
        }


        public Robot(SerialLink left, SerialLink right, string host, int port)
        {
            _connection = new RobotConnection(host, port);

            IP = host;
            Port = port;

            LeftArm = new RobotArm(left, "Left");
            LeftArm.RequestExecute += OnArmRequestExecute;
            LeftArm.RequestRobotData += OnArmRequestRobotData;

            RightArm = new RobotArm(right, "Right");
            RightArm.RequestExecute += OnArmRequestExecute;
           RightArm.RequestRobotData += OnArmRequestRobotData;
        }

        internal Robot() : this(null, null, "127.0.0.1", 10099)
        {

        }

        private void OnArmRequestRobotData(object sender, RequestRobotDataEventArgs e)
        {
            if (!_connection.Connected)
            {
                Connect();
                _connection.Connect();
                throw new Exception("Соединение еще не установлено");
            }

            var arm = (RobotArm) sender;
            var joints = arm.DenavitHartenberg.Joints.Select(joint => joint.Name).ToArray();

            e.MotorAngles = ReceiveFromArm(joints);
        }

        public ArmPosition[] GetRobotState()
        {
            if (!_connection.Connected)
            {
                throw new Exception("Соединение еще не установлено");
            }

            var leftArmJoint = LeftArm.DenavitHartenberg.Joints.Select(joint => joint.Name);
            var rightArmJoint = RightArm.DenavitHartenberg.Joints.Select(joint => joint.Name);

            var joints = leftArmJoint.Concat(rightArmJoint).ToArray();
            var data = ReceiveFromArm(joints);


            var leftArmPosition = LeftArm.FK(data.SubArray(0, 6));
            var rightArmPosition = RightArm.FK(data.SubArray(6, 6));

            return new[] { leftArmPosition, rightArmPosition };
        }

        private void OnArmRequestExecute(object sender, ExecuteRequestEventArgs e)
        {
            if (!_connection.Connected)
            {
                throw new Exception("Соединение еще не установлено");
            }

            var arm = (RobotArm) sender;
            var joints = arm.DenavitHartenberg.Joints.Select(joint => joint.Name).ToArray();

            SendToRobotArm(joints, e.MotorAngles, e.CommandType, e.ExecutionTime);
        }

        public void Connect()
        {
            if (_connection.Connected)
            {
                // throw new Exception("Соединение уже установлено");
            }
            else
            {
                _connection.Connect();
            }

        }


        // выполнение произвольной команды для робота
        public void ExecuteCommand(string command)
        {
            _connection.Execute(command);
        }

        public void Disconnect()
        {
            if (!_connection.Connected)
            {
                throw new Exception("Соединение еще не установлено");
            }
            _connection.Disconnect();
        }

   
        // отправка команды на робота
        public void UrologGo(string q1, string q2, string q3)
        {
            _connection.Execute("robot:GO:" + q1+";"+q2+";" + q3 + ";");
        }

        public string UrologGo2(string q1, string q2, string q3)
        {
            var resultString=_connection.Execute2("robot:GO:" + q1 + ";" + q2 + ";" + q3 + ";");
            return resultString;
        }
        private void SendToRobotArm(string[] joints, double[] data, CommandType commandType=CommandType.PositionSet, double time=0)
        {
            var commandTypeString = commandType == CommandType.PositionSet ? "posset" : "go";
            if (commandType == CommandType.Go && time < 0.1)
            {
                throw new Exception("Невозможно выполнить команду за указанное время");
            }

            var values = data.Select(Utils.RadiansToDegrees).Select(value => value.ToString("F", CultureInfo.InvariantCulture)).Aggregate((accumulator, value) => accumulator + ";" + value);
            var jointNames = string.Join(";", joints);
            var commandString = $"robot:motors:{jointNames}:{commandTypeString}:{values}:{time}";

            _connection.Execute(commandString);

            if (commandType == CommandType.Go)
            {
                var received =  ReceiveFromArm(joints);
                var error = received.Zip(data, (current, reference) => Math.Abs(current - reference)).Sum(diff => diff * diff);
                
               //if (error > 1e-2)
               //{
               //    throw new Exception("Манипулятор не достиг заданной позиции");
               //}
            }
            
        }

        private double[] ReceiveFromArm(string[] joints)
        {
            var jointNames = string.Join(";", joints);
            var response = _connection.ExecuteWithResult($"robot:motors:{jointNames}:posget");
            return response.Select(Utils.DegreesToRadians).ToArray();
        }

        public static Robot LoadFromFile(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                XmlSerializer x = new XmlSerializer(typeof(Robot));
                var robot = (Robot)x.Deserialize(new StreamReader(stream));
                return robot;
            }
        }


        public void SaveToFile(string filePath)
        {
            var serializer = new XmlSerializer(typeof(Robot));
            var settings = new XmlWriterSettings { Indent = true };
            using (var writer = XmlWriter.Create(filePath, settings))
            {
                serializer.Serialize(writer, this);
            }
        }
    }

    public enum CommandType
    {
        PositionSet,
        Go
    }

    public class RequestRobotDataEventArgs : EventArgs
    {
        public double[] MotorAngles { get; set; }
    }

    public class ExecuteRequestEventArgs : EventArgs
    {
        public double[] MotorAngles { get; }
        public double ExecutionTime { get; }
        public CommandType CommandType { get; set; }

        public ExecuteRequestEventArgs(double[] motorAngles, CommandType commandType,  double execTime)
        {
            MotorAngles = motorAngles;
            ExecutionTime = execTime;
            CommandType = commandType;
        }
    }
}
