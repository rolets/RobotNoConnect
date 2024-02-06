using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Vec3 = MathNet.Spatial.Euclidean.Vector3D;
namespace RoboKinematics
{
    public class ArmPosition
    {
        public double[] X { get; }
        public double[] Y { get; }
        public double[] Z { get; }
        public double[] MotorAngles { get; }
        public double Roll { get; }
        public double Pitch { get; }
        public double Yaw { get; }
        public int JointsCount { get; }

        public double EndEffectorX => X[X.Length - 1];
        public double EndEffectorY => Y[Y.Length - 1];
        public double EndEffectorZ => Z[Z.Length - 1];
        internal Vec3[] Points => X.ZipWith3(Y, Z, (x, y, z) => new Vec3(x, y, z)).ToArray();

        public ArmPosition(Matrix<double>[] matrices, double[] q)
        {
            MotorAngles = q;
            var length = matrices.Length;
            JointsCount = length;

            if (length < 1)
            {
                throw new ArgumentException("Массив матриц не может быть пустым");
            }

            X = new double[length];
            Y = new double[length];
            Z = new double[length];
            

            for (int i = 0; i < length; ++i)
            {
                var mat = matrices[i];
                X[i] = mat.At(0, 3);
                Y[i] = mat.At(1, 3);
                Z[i] = mat.At(2, 3);
            }

            var rpy = matrices.Last().SubMatrix(0, 3, 0, 3).ToEulerAngles();
            Roll = rpy[0];
            Pitch = rpy[1];
            Yaw = rpy[2];

        }

        public ArmPosition(Matrix<double> transform, double[] q) : this(new[] { transform }, q)
        {
        }
    }
}