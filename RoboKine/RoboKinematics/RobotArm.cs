using System;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;

namespace RoboKinematics
{
    [XmlRoot(Namespace = "")]
    public class RobotArm
    {
        public event EventHandler<ExecuteRequestEventArgs> RequestExecute;
        public event EventHandler<RequestRobotDataEventArgs> RequestRobotData;

        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement]
        public SerialLink DenavitHartenberg { get; set; }

        public RobotArm(SerialLink model, string name)
        {
            DenavitHartenberg = model;
            Name = name;
        }

        internal RobotArm() : this(new SerialLink(new double[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } }), "n/a")
        {

        }

        public ArmPosition FK(double[] q)
        {
            var matrices = DenavitHartenberg.FK(q.ToVector());
            return new ArmPosition(matrices, q);
        }

        public ArmPosition IK(double[] translation, double[] rotation, double[] q0, bool angleLimit=true)
        {
            double x = translation[0];
            double y = translation[1];
            double z = translation[2];
            double roll = rotation[0];
            double pitch = rotation[1];
            double yaw = rotation[2];

            var transform = CoordinateSystem.Rotation(Angle.FromRadians(roll),
                                                    Angle.FromRadians(pitch),
                                                    Angle.FromRadians(yaw));
            transform[0, 3] = x;
            transform[1, 3] = y;
            transform[2, 3] = z;
            var q = DenavitHartenberg.IK(transform, q0, angleLimit);

            return new ArmPosition(transform, q);
        }

        public void SetJointAngles(double[] q, CommandType commandType=CommandType.PositionSet, double execTime=0)
        {
            var excecuteEventArgs = new ExecuteRequestEventArgs(q, commandType, execTime);
            OnRequestExecte(excecuteEventArgs);
        }

        private double[] GetJointAngles()
        {
            var requestEventArgs = new RequestRobotDataEventArgs();
            OnRequestRobotData(requestEventArgs);
            return requestEventArgs.MotorAngles;
        }

        public ArmPosition GetPosition()
        {
            var q0 = GetJointAngles();
            return FK(q0);
        }

        public ArmPosition MoveTo(double[] translation, double[] rotation, CommandType commandType = CommandType.PositionSet, double execTime=0)
        {
            var q0 = GetJointAngles();
            var q = IK(translation, rotation, q0, true);
            SetJointAngles(q.MotorAngles, commandType, execTime);
            return q;
        }

        public ArmPosition MoveTo(double[] translation, CommandType commandType = CommandType.PositionSet, double execTime=0)
        {
            var q0 = GetJointAngles();
            var q = IK(translation, new double[3], q0, false);
            SetJointAngles(q.MotorAngles, commandType, execTime);
            return q;
        }

        public double[][] JointTrajectory(double[] begin, double[] end, int count)
        {
            var result = new double[count][];
            var step = 1.0 / count;
            double t = 0;
            for (int i = 0; i < count; ++i)
            {
                t += step;
                result[i] = new double[begin.Length];
                for (int j = 0; j < begin.Length; ++j)
                {
                    result[i][j] = Utils.Lerp(begin[j], end[j], t);
                }
            }

            return result;
        }

        public void RunJointTrajectory(double[] begin, double[] end, int count)
        {
            var trajectory = JointTrajectory(begin, end, count);
            RunJointTrajectory(trajectory);
        }

        public void RunJointTrajectory(double[][] trajectory, double segmentExecTime=0.5)
        {
            foreach (var q in trajectory)
            {
                SetJointAngles(q, CommandType.Go, segmentExecTime);
            }
        }

        protected virtual void OnRequestExecte(ExecuteRequestEventArgs e)
        {
            RequestExecute?.Invoke(this, e);
        }

        protected virtual void OnRequestRobotData(RequestRobotDataEventArgs e)
        {
            RequestRobotData?.Invoke(this, e);
        }
    }
}