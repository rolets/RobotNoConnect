using System;
using Mat = MathNet.Numerics.LinearAlgebra.Matrix<double>;
using Vec3 = MathNet.Spatial.Euclidean.Vector3D;

namespace RoboKinematics
{
    public static class Collide
    {
        /// <summary>
        /// Функция проверки столкновения манипулятора с сферчическим препятствием
        /// </summary>
        /// <param name="arm">Массив точек сочленений манипулятора</param>
        /// <param name="armRadius">Радиус звена манипулятора, считаем его цилендрическим</param>
        /// <param name="centerOfSphere">Центр препятствия</param>
        /// <param name="sphereRadius">Радиус препятствия</param>
        /// <param name="nearestPoint">Ближайшая к сфере точка лежащая на манипуляторе</param>
        /// <returns>Результат проверки. true - есть коллизия, false - нет коллизии</returns>
        public static bool ArmToSphere(ArmPosition arm, double armRadius, double[] centerOfSphere, double sphereRadius, out double[] nearestPoint)
        {
            nearestPoint = null;
            var points = arm.Points;
            var center = new Vec3(centerOfSphere[0], centerOfSphere[1], centerOfSphere[2]);
            for (int i = 1; i < arm.JointsCount; ++i)
            {
                var begin = points[i - 1];
                var end = points[i];

                var (dist, nearest) = DistanceToPoint(begin, end, center);
                nearestPoint = nearest.ToVector().ToArray();
                if (dist < (armRadius + sphereRadius))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Функция проверки пересечения манипуляторов
        /// </summary>
        /// <param name="arm1">Массив точек сочленений манипулятора 1</param>
        /// <param name="arm2">Массив точек сочленений манипулятора 2</param>
        /// <param name="armRadius">Радиус звена манипулятора, считаем его цилендрическим</param>
        /// <param name="nearest1">Ближайшая к препятсвию точка лежащая на первом манипуляторе</param>
        /// <param name="nearest2">Ближайшая к препятсвию точка лежащая на втором манипуляторе</param>
        /// <returns>Результат проверки. true - есть коллизия, false - нет коллизии</returns>
        public static bool ArmToArm(ArmPosition arm1, ArmPosition arm2, double armRadius, out double[] nearest1, out double[] nearest2)
        {
            var points1 = arm1.Points;
            var points2 = arm2.Points;
            for (int i = 1; i < arm1.JointsCount; ++i)
            {
                var begin1 = points1[i-1];
                var end1 = points1[i];
                if ((begin1 - end1).Length < 0.0001)
                {
                    continue;
                }

                for (int j = 1; j < arm2.JointsCount; ++j)
                {
                    var begin2 = points2[j-1];
                    var end2 = points2[j];
                    if ((begin2 - end2).Length < 0.0001)
                    {
                        continue;
                    }

                    var (dist, p1, p2) = DistanceToLine(begin1, end1, begin2, end2);
                    var (dist2, p12, p22) = DistanceToLine(begin2, end2, begin1, end1);
                    nearest1 = p1.ToVector().ToArray();
                    nearest2 = p2.ToVector().ToArray();
                   // Console.WriteLine(dist);
                   // Console.WriteLine("i "+i+ " j "+ j);

                    if ((dist < (2 * armRadius))|| (dist2 < (2 * armRadius)))
                    {
                        return true;
                    }

                }
            }
            nearest1 = null;
            nearest2 = null;
            return false;
        }

        private static (double dist, Vec3 nearest) DistanceToPoint(Vec3 begin, Vec3 end, Vec3 point)
        {
            var vec = end - begin;
            var normalized = vec.Normalize();
            var pointVector = point - begin;
            var normalizedPointVector = pointVector.Normalize();
            var t = normalized.DotProduct(normalizedPointVector);
            t = Trunc(t, 0, 1);

            var nearest = vec.ScaleBy(t) + begin;
            var dist = (nearest - point).Length;

            return (dist, nearest);

        }

        private static (double dist, Vec3 nearest1, Vec3 nearest) DistanceToLine(Vec3 begin1, Vec3 end1, Vec3 begin2, Vec3 end2)
        {
            var v1 = end1 - begin1;
            var v2 = end2 - begin2;
            //Console.WriteLine("v1"+ v1+" v2 " + v2);
            var v3 = v1.CrossProduct(v2);
            var p = begin2 - begin1;
            // создаем матрицу из векторов, матрица 3х3 где каждый слолбец это вектор
            var a = Mat.Build.DenseOfColumnVectors(v1.ToVector(), v2.ToVector(), v3.ToVector());
            var b = p.ToVector();

            var x = a.Solve(b);
            var t1 = Trunc(x[0], 0, 1);
            var t2 = Trunc(x[1], 0, 1);



            var nearest1 = begin1 + v1.ScaleBy(t1);
            var nearest2 = begin2 + v2.ScaleBy(t2);
            var dist = (nearest1 - nearest2).Length;

            return (dist, nearest1, nearest2);
        }

        private static double Trunc(double value, double min, double max)
        {
            return Math.Min(Math.Max(value, min), max);
        }
    }
}
