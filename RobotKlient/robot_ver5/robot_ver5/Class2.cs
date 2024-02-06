
namespace robot_ver5
{
    class Class2
    {
        public struct Vector4
        {
            public float X;
            public float Y;
            public float Z;
            public float W;

            public Vector4(float x, float y, float z, float w)
            {
                X = x;
                Y = y;
                Z = z;
                W = w;
            }
        }
        public struct Matrix4x4
        {
            public float V00;
            public float V01;
            public float V02;
            public float V03;
            public float V10;
            public float V11;
            public float V12;
            public float V13;
            public float V20;
            public float V21;
            public float V22;
            public float V23;
            public float V30;
            public float V31;
            public float V32;
            public float V33;

            public static Matrix4x4 Identity
            {
                get
                {
                    Matrix4x4 m = new Matrix4x4();
                    m.V00 = m.V11 = m.V22 = m.V33 = 1;
                    return m;
                }
            }

            public static Matrix4x4 CreateScale(float scale)
            {
                var m = new Matrix4x4();
                m.V00 = m.V11 = m.V22 = scale;
                m.V33 = 1f;

                return m;
            }


            public static Matrix4x4 CreateRotationY(float radians)
            {
                Matrix4x4 m = Matrix4x4.Identity;

                float cos = (float)System.Math.Cos(radians);
                float sin = (float)System.Math.Sin(radians);

                m.V00 = m.V22 = cos;
                m.V02 = sin;
                m.V20 = -sin;

                return m;
            }

            public static Matrix4x4 CreateRotationX(float radians)
            {
                Matrix4x4 m = Matrix4x4.Identity;

                float cos = (float)System.Math.Cos(radians);
                float sin = (float)System.Math.Sin(radians);

                m.V11 = m.V22 = cos;
                m.V12 = -sin;
                m.V21 = sin;

                return m;
            }

            public static Matrix4x4 CreateRotationZ(float radians)
            {
                Matrix4x4 m = Matrix4x4.Identity;

                float cos = (float)System.Math.Cos(radians);
                float sin = (float)System.Math.Sin(radians);

                m.V00 = m.V11 = cos;
                m.V01 = -sin;
                m.V10 = sin;

                return m;
            }

            public static Matrix4x4 CreateFromYawPitchRoll(float yaw, float pitch, float roll)
            {
                return (CreateRotationY(yaw) * CreateRotationX(pitch)) * CreateRotationZ(roll);
            }

            public static Matrix4x4 CreateTranslation(Vector4 position)
            {
                Matrix4x4 m = Matrix4x4.Identity;

                m.V03 = position.X;
                m.V13 = position.Y;
                m.V23 = position.Z;

                return m;
            }

            // перемножение матриц
            public static Matrix4x4 operator *(Matrix4x4 matrix1, Matrix4x4 matrix2)
            {
                Matrix4x4 m = new Matrix4x4();

                m.V00 = matrix1.V00 * matrix2.V00 + matrix1.V01 * matrix2.V10 + matrix1.V02 * matrix2.V20 + matrix1.V03 * matrix2.V30;
                m.V01 = matrix1.V00 * matrix2.V01 + matrix1.V01 * matrix2.V11 + matrix1.V02 * matrix2.V21 + matrix1.V03 * matrix2.V31;
                m.V02 = matrix1.V00 * matrix2.V02 + matrix1.V01 * matrix2.V12 + matrix1.V02 * matrix2.V22 + matrix1.V03 * matrix2.V32;
                m.V03 = matrix1.V00 * matrix2.V03 + matrix1.V01 * matrix2.V13 + matrix1.V02 * matrix2.V23 + matrix1.V03 * matrix2.V33;

                m.V10 = matrix1.V10 * matrix2.V00 + matrix1.V11 * matrix2.V10 + matrix1.V12 * matrix2.V20 + matrix1.V13 * matrix2.V30;
                m.V11 = matrix1.V10 * matrix2.V01 + matrix1.V11 * matrix2.V11 + matrix1.V12 * matrix2.V21 + matrix1.V13 * matrix2.V31;
                m.V12 = matrix1.V10 * matrix2.V02 + matrix1.V11 * matrix2.V12 + matrix1.V12 * matrix2.V22 + matrix1.V13 * matrix2.V32;
                m.V13 = matrix1.V10 * matrix2.V03 + matrix1.V11 * matrix2.V13 + matrix1.V12 * matrix2.V23 + matrix1.V13 * matrix2.V33;

                m.V20 = matrix1.V20 * matrix2.V00 + matrix1.V21 * matrix2.V10 + matrix1.V22 * matrix2.V20 + matrix1.V23 * matrix2.V30;
                m.V21 = matrix1.V20 * matrix2.V01 + matrix1.V21 * matrix2.V11 + matrix1.V22 * matrix2.V21 + matrix1.V23 * matrix2.V31;
                m.V22 = matrix1.V20 * matrix2.V02 + matrix1.V21 * matrix2.V12 + matrix1.V22 * matrix2.V22 + matrix1.V23 * matrix2.V32;
                m.V23 = matrix1.V20 * matrix2.V03 + matrix1.V21 * matrix2.V13 + matrix1.V22 * matrix2.V23 + matrix1.V23 * matrix2.V33;

                m.V30 = matrix1.V30 * matrix2.V00 + matrix1.V31 * matrix2.V10 + matrix1.V32 * matrix2.V20 + matrix1.V33 * matrix2.V30;
                m.V31 = matrix1.V30 * matrix2.V01 + matrix1.V31 * matrix2.V11 + matrix1.V32 * matrix2.V21 + matrix1.V33 * matrix2.V31;
                m.V32 = matrix1.V30 * matrix2.V02 + matrix1.V31 * matrix2.V12 + matrix1.V32 * matrix2.V22 + matrix1.V33 * matrix2.V32;
                m.V33 = matrix1.V30 * matrix2.V03 + matrix1.V31 * matrix2.V13 + matrix1.V32 * matrix2.V23 + matrix1.V33 * matrix2.V33;


                return m;
            }

            // умножение матрицы на вектор
            public static Vector4 operator *(Matrix4x4 matrix, Vector4 vector)
            {
                return new Vector4(
                    matrix.V00 * vector.X + matrix.V01 * vector.Y + matrix.V02 * vector.Z + matrix.V03 * vector.W,
                    matrix.V10 * vector.X + matrix.V11 * vector.Y + matrix.V12 * vector.Z + matrix.V13 * vector.W,
                    matrix.V20 * vector.X + matrix.V21 * vector.Y + matrix.V22 * vector.Z + matrix.V23 * vector.W,
                    matrix.V30 * vector.X + matrix.V31 * vector.Y + matrix.V32 * vector.Z + matrix.V33 * vector.W
                    );
            }

            
        }
    }
}
