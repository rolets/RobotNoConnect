using System;
using System.Linq;


namespace RoboKinematics
{
    public static class Utils
    {
        internal static string StatusCodeToString(int statusCode)
        {
            switch (statusCode)
            {
                case 0xFF: return "Неизвестная команда или синтаксическая ошибка";
                case 0xFE: return "Оборудование или устройство не доступно";
                case 0xFD: return "Неизвестный идентификатор или устройство с данным идентификатором отсутствует";
                case 0xFC: return "Не корректное значение входного параметра";
                case 0xF0: return "Команда выполнена";
                case 0xF1: return "Команда выполнена с возвратом результата";
                case 0xF2: return "Команда устарела, рекомендуется использовать обновленный вариант";
                case 0xF3: return "Оборудование или устройство не поддерживает команду";
                case 0xF4: return "Текущая версия программного комплекса не поддерживает команду";
                default: throw new ArgumentException("Неизвестный код возврата", nameof(statusCode));
            }
        }

        public static double MeasureError(double[] point1, double[] point2)
        {
            if (point1.Length != point2.Length)
            {
                throw new ArgumentException("Вектора разной длинны");
            }

            var error = point1.Zip(point2, (p1, p2) => Math.Pow(p1 - p2, 2)).Sum();
         
            return Math.Sqrt(error);
        }

        public static double DegreesToRadians(double degrees)
        {
            return Math.PI / 180 * degrees;
        }

        public static double[] DegreesToRadians(params double[] degrees)
        {
            return degrees.Select(DegreesToRadians).ToArray();
        }



        public static double RadiansToDegrees(double radians)
        {
            return 180 / Math.PI * radians;
        }

        public static double[] RadiansToDegrees(params double[] radians)
        {
            return radians.Select(RadiansToDegrees).ToArray();
        }

        public static double Lerp(double begin, double end, double t)
        {
            return begin * (1 - t) + end * t;
        }
        public static double Vector3Length(double[] point1, double[] point2)
        {
       
         return Math.Sqrt(point1.Zip(point2, (p1, p2) => Math.Pow(p1 - p2, 2)).Sum()); //тоже самое
           
        }

        

    }
}
