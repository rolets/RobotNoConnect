using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using MathNet.Numerics;
using Vec = MathNet.Numerics.LinearAlgebra.Vector<double>;
using Mat = MathNet.Numerics.LinearAlgebra.Matrix<double>;
using System.IO;

namespace RoboKinematics
{
    [XmlRoot(Namespace = "")]
    public class SerialLink
    {
        private Mat _worldFrame;

        [XmlElement("Joint")]
        public List<Joint> Joints { get; set; }

        [XmlIgnore]
        public double[,] WorldFrame
        {
            get => _worldFrame.ToArray();
            set => _worldFrame = Mat.Build.DenseOfArray(value);
        }

        [XmlArray("WorldFrame")]
        [XmlArrayItem("Row", typeof(double[]))]
        public double[][] WorldFrameWrapper
        {
            get => _worldFrame.ToRowArrays();
            set => _worldFrame = Mat.Build.DenseOfRowArrays(value);
        }

        



        // создание базовой матрицы вращения
        public SerialLink(double[,] worldFrame)
        {
           Joints = new List<Joint>(6);  // можно удалить
            _worldFrame = Mat.Build.DenseOfArray(worldFrame);
        }

        public SerialLink() 
            : this(new double[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } })
        {
        }
        // создание DH модели робота 
        public void AddJoint(double a, double d, double alpha, double theta, double lowerLimit=-6.283, double upperLimit=6.283, string name="n/a", double qCurrent=0.0, string typeJoint="Revol")
        {
            Joints.Add(new Joint(a, d, alpha, theta, lowerLimit, upperLimit, name, qCurrent, typeJoint));
        }

        // Решение прямой задачи кинематики
        internal Mat[] FK(Vec q)
        {
            var jointCount = Joints.Count;
            var all = new Mat[jointCount];
            var T = _worldFrame;
            for(int i=0; i < jointCount; ++i)
            {
                var joint = Joints[i];

                var theta = joint.Theta;
                var DD = joint.D;
               if (joint.TypeJoint== "Revol")
               { 
                   theta = q[i] + joint.Theta;
               }

                if (joint.TypeJoint == "Prizm")
                {
                     DD = q[i] + joint.D;
                    //theta = q[i] + joint.Theta;
                }

                T *= TransformTo(joint.Alpha, theta, joint.A, DD);
                all[i] = T;
            }
            
            return all;
        }

        /// <summary>
        /// Функция обратной кинематики
        /// </summary>
        /// <param name="transform">Матрица 4х4 желаемого положения манипулятора</param>
        /// <param name="q0">Текущее положение двигателей манипулятора</param>
        /// <param name="angleLimit">Ограничивать вращение эффектора или нет</param>
        /// <returns>Массив углов двигателей</returns>
        internal double[] IK(Mat transform, double[] q0, bool angleLimit)
        {
            if (transform.RowCount != 4 && transform.ColumnCount != 4)
            {
                throw new ArgumentException("Матрица должна быть 4х4");
            }

            var coef = angleLimit ? 1.0 : 0.0;
            var eye4 = Mat.Build.DenseIdentity(4); //единичная матрица 4х4
            var omega = Mat.Build.Diagonal(new [] { coef, coef, coef, 3/GetLength()});
            int i = 0;
            //File.Create("D:\\new_file.txt");
            double Error(Vec q)
            {
                var Tref = transform;//матрица куда надо прийти
                var all = FK(q);
                
              
                var Tcur = all[all.Length - 1];// текущая матрица
                
                var err_mat = (Tref.Solve(Tcur) - eye4) * omega;
                // var err_mat = ((Tref.Inverse() * Tcur) - eye4) * omega;
                //Console.WriteLine((Tref.Solve(Tcur) - eye4) * omega); 
                //Console.WriteLine(((Tref.Inverse() * Tcur) - eye4) * omega);
                 
                // Console.WriteLine(Tref.Solve(Tcur) - eye4);
                //Console.WriteLine(err_mat);
                // Solve
                //Решает систему линейных уравнений AX = B где А QR-разложение
                //Tcur matrix B input
                //Tref matrix A
                // matrix X Return
                // Console.WriteLine(i);
                var err = err_mat.Enumerate().Sum(x => x * x); // сумма квадратов элементов матрицы
                                                               // Console.WriteLine(err.ToString("F9"));
                                                               // i++;
                //Console.WriteLine(err);
                //File.AppendAllText("D:\\new_file.txt", err.ToString("F15")+ ";\n");
                return err;
            }

            var lowerLimits = Joints.Select(joint => joint.LowerLimit).ToVector();
            //lowerLimits(4)
            
            var upperLimits = Joints.Select(joint => joint.UpperLimit).ToVector();
            var result = FindMinimum.OfFunctionConstrained(Error,
                lowerLimits,
                upperLimits,
                q0.ToVector()
              );
          //  Найдите вектор x, который минимизирует функцию f(x), ограниченную в границах, с помощью алгоритма Бройдена – Флетчера – Гольдфарба – Шанно с ограничениями(BFGS - B).Недостающий градиент оценивается численно(прямая разница).
            //var directionMask = Joints.Select(joint => (double)joint.RotationDirection).ToVector();
            return result.ToArray();
                //.PointwiseMultiply(directionMask).ToArray();
        }
       
        // Матриица преобразования
        private Mat TransformTo(double alpha, double theta, double a, double d)
        {
            // ReSharper disable InconsistentNaming
            var cos_theta = Math.Cos(theta);
            var sin_theta = Math.Sin(theta);
            var cos_alpha = Math.Cos(alpha);
            var sin_alpha = Math.Sin(alpha);
            // ReSharper restore InconsistentNaming

            var result = Mat.Build.DenseOfArray(new [,] 
            { { cos_theta, -sin_theta * cos_alpha, sin_theta * sin_alpha, a * cos_theta},
              { sin_theta, cos_theta * cos_alpha, -cos_theta * sin_alpha, a * sin_theta },
              { 0, sin_alpha, cos_alpha, d },
              { 0, 0, 0, 1 } });

            return result;
        }

        private double GetLength()
        {
            return Joints.Sum(joint => Math.Sqrt(Math.Pow(joint.A, 2) + Math.Pow(joint.D, 2)));
        }
    }
}
