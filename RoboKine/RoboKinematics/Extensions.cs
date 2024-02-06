using System;
using System.Collections.Generic;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using System.Threading.Tasks;
using Mat = MathNet.Numerics.LinearAlgebra.Matrix<double>;

namespace RoboKinematics
{
    public static class Extensions
    {
        internal static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        
        internal static async Task<int> ReadByteAsync(this StreamReader reader)
        {
            var buffer = new char[1];
            var count = await reader.ReadAsync(buffer, 0, 1);
            if (count < 1)
            {
                throw new EndOfStreamException();
            }
            return buffer[0];
        }

        internal static Vector<double> ToVector(this IEnumerable<double> obj)
        {
            return Vector<double>.Build.DenseOfEnumerable(obj);
        }

        public static bool AnyOf<T>(this T self, params T[] values)
        {
            return values.Contains(self);
        }

        public static IEnumerable<TResult> ZipWith3<T1, T2, T3, TResult>(
            this IEnumerable<T1> source,
            IEnumerable<T2> second,
            IEnumerable<T3> third,
            Func<T1, T2, T3, TResult> func)
        {
            using (var e1 = source.GetEnumerator())
            using (var e2 = second.GetEnumerator())
            using (var e3 = third.GetEnumerator())
            {
                while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
                    yield return func(e1.Current, e2.Current, e3.Current);
            }
        }

        private static bool IsRotationMatrix(Mat R)
        {
            var Rt = R.Transpose();
            var shouldBeIdentity = Rt * R;
            var I = Mat.Build.DenseIdentity(3);
            var diff = I - shouldBeIdentity;
            return diff.L1Norm() < 1e-6;
        }

        internal static int MaxIndex<T>(this IEnumerable<T> sequence)
            where T : IComparable<T>
        {
            int maxIndex = -1;
            T maxValue = default(T); // Immediately overwritten anyway

            int index = 0;
            foreach (T value in sequence)
            {
                if (value.CompareTo(maxValue) > 0 || maxIndex == -1)
                {
                    maxIndex = index;
                    maxValue = value;
                }
                index++;
            }
            return maxIndex;
        }

        internal static double[] ToEulerAngles(this Mat R)
        {

            if(!IsRotationMatrix(R))
                throw new ArgumentException("Аргумент не является матрицей вращения");

            var sy = Math.Abs(Math.Abs(R[0, 2]) - 1);

            bool singular = sy < 1e-6; // If

            double r, p, y;
            if (singular)
            {
                r = 0;
                p = Math.Asin(R[0, 2]);
                y = R[0, 2] > 0 
                    ? Math.Atan2(R[2, 1], R[1, 1])
                    : -Math.Atan2(R[1, 0], R[2, 0]);
            }
            else
            {
                r = -Math.Atan2(R[0, 1], R[0, 0]);
                y = -Math.Atan2(R[1, 2], R[2, 2]);

                var maxIndex = new[] {R[0, 0], R[0, 1], R[1, 2], R[2, 2]}.Select(Math.Abs).MaxIndex();
                switch (maxIndex)
                {
                    case 0:
                        p = Math.Atan(R[0, 2] * Math.Cos(r) / R[0, 0]);
                        break;
                    case 1:
                        p = -Math.Atan(R[0, 2] * Math.Sin(r) / R[0, 1]);
                        break;
                    case 2:
                        p = -Math.Atan(R[0, 2] * Math.Sin(y) / R[1, 2]);
                        break;
                    case 3:
                        p = Math.Atan(R[0, 2] * Math.Cos(y) / R[2, 2]);
                        break;
                    default: throw new Exception("Невозможный индекс");
                }
            }
            return new [] { r, p, y };



        }
    }
}
