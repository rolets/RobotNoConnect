using System;
using System.Drawing;
using System.Windows.Forms;
using RoboKinematics;
using CommandType = RoboKinematics.CommandType;
using Timer = System.Windows.Forms.Timer;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace robot_ver5
{
   

    public partial class Form1 : Form
    {
        private Class2.Vector4[] drRobotL;
        private Class2.Vector4[] drRobotR;
        private Class2.Vector4[] drRobotKoord;
        private Class2.Vector4[] drRobotFL;
        private Class2.Vector4[] drRobotKoord_1;
        private Class2.Vector4[] drRobotFL_1;
        Class2.Vector4 position = new Class2.Vector4(0, 0, 0, 0); 
        
        float size =200f;
        float yaw = 0f;
        float pitch = 0f;
        float roll = 0f;

        float heid = 2f;

        private TrackBar tbSize;
        private TrackBar tbRoll;
        private TrackBar tbPitch;
        private TrackBar tbYaw;

        private Label labelSize;
        private Label labelRoll;
        private Label labelPitch;
        private Label labelYaw;
        private Label labelVR;

        Robot robot;

        public double[] qL;// главная переменная текущего состояния левой руки
        public double[] qR;// главная переменная текущего состояния правой руки

        public double[] baseL;
        public double[] baseR;

        public int diam = 10;//диаметр элипсов на звеньях
        public int sZveno = 6;// ширина звеньев
        class Vector3Length
        {
            double[] _start;
            double[] _end;

            public Vector3Length(double[] start, double[] end)
            {
                _start = start;
                _end = end;
            }
            public double GetLength()
            {
                //return Math.Sqrt(Math.Pow(_end[0] - _start[0], 2) + Math.Pow(_end[1] - _start[1], 2) + Math.Pow(_end[2] - _start[2], 2));
                return Math.Sqrt(_start.Zip(_end, (p1, p2) => Math.Pow(p1 - p2, 2)).Sum()); //тоже самое
            }
        }
        public Form1()
        {
            InitializeComponent();
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
           // var leftArmWorldFrame = new double[,] { { 0, 0, 1, 0.4 }, { 0, 1, 0, 0 }, { -1, 0, 0, 0 }, { 0, 0, 0, 1 } };
           // var rightArmWorldFrame = new double[,] { { 0, 0, 1, -0.4 }, { 0, 1, 0, 0 }, { -1, 0, 0, 0 }, { 0, 0, 0, 1 } };

           // var leftArm = new SerialLink(leftArmWorldFrame);
           // var rightArm = new SerialLink(rightArmWorldFrame);
            //robot=Robot.LoadFromFile("settings.xml");

            //              a,     d, alpha, theta, lowerLimit ,  upperLimit , string name ,  false, "Revol"
          //  leftArm.AddJoint(0.16, 0, 1.5708, 0, -1.5740, 0, "L.ShoulderS", false, "Revol");
          //  leftArm.AddJoint(0, 0, 1.5708, -1.5708, 0, 1.5708, "L.ShoulderF", false, "Revol");
          //  leftArm.AddJoint(0, -0.422, 1.5708, 3.14, -1.5708, 1.5708, "L.ElbowR", false, "Revol");
          //  leftArm.AddJoint(0, 0, 1.5708, 3.14, -1.5708, 0, "L.Elbow", false, "Revol");
          //  leftArm.AddJoint(0, -0.457, 1.5708, 3.14, -1.5708, 1.5708, "L.WristR", false, "Revol");
         //   leftArm.AddJoint(-0.541, 0, 0, 1.5708, -1.5708, 1.5708, "L.WristF", false, "Revol");



          //  rightArm.AddJoint(0.16, -0, 1.5708, 0, -1.5740, 0, "R.ShoulderS2", false, "Revol");
          //  rightArm.AddJoint(0, 0, 1.5708, -1.5708, -1.5708, 0, "R.ShoulderF2", false, "Revol");
           // rightArm.AddJoint(0, -0.422, 1.5708, 3.14, -1.5708, 1.5708, "R.ElbowR2", false, "Revol");
           // rightArm.AddJoint(0, 0, 1.5708, 3.14, 0, 1.5708, "R.Elbow", false, "Revol");
           // rightArm.AddJoint(0, -0.457, 1.5708, 3.14, -1.5708, 1.5708, "R.WristR2", false, "Revol");
            //rightArm.AddJoint(-0.341, 0, 0, 1.5708, -1.5708, 1.5708, "R.WristF2", false, "Revol");

            string[] arStr = File.ReadAllLines("setting.txt");// читаем из файла адрес и порт
         //   robot = new Robot(leftArm, rightArm, arStr[0], Int32.Parse(arStr[1])); // Создаем объект robot с адресом и портом для полключения
            robot = Robot.LoadFromFile("settings.xml");
          // robot.SaveToFile("settings.xml");

            qL = new double[robot.LeftArm.DenavitHartenberg.Joints.Count];// главная переменная текущего состояния левой руки
            qR = new double[robot.RightArm.DenavitHartenberg.Joints.Count];// главная переменная текущего состояния правой руки
                                                                           // drRobot = new Class2.Vector4[14];
            drRobotL = new Class2.Vector4[robot.LeftArm.DenavitHartenberg.Joints.Count + 1];// переменная для хранения значений для рисования левой руки
            drRobotR = new Class2.Vector4[robot.RightArm.DenavitHartenberg.Joints.Count + 1];// переменная для хранения значений для рисования правой руки

            baseL = new[] { robot.LeftArm.DenavitHartenberg.WorldFrame[0, 3], robot.LeftArm.DenavitHartenberg.WorldFrame[1, 3], robot.LeftArm.DenavitHartenberg.WorldFrame[2, 3] };// переменная хранящая базовое смещение относительно центра левого манипулятора
            baseR = new[] { robot.RightArm.DenavitHartenberg.WorldFrame[0, 3], robot.RightArm.DenavitHartenberg.WorldFrame[1, 3], robot.RightArm.DenavitHartenberg.WorldFrame[2, 3] };// переменная хранящая базовое смещение относительно центра правого манипулятора


            drRobotKoord = new Class2.Vector4[4];// переменная для хранения значений для рисования координатных осей
            drRobotKoord_1 = new Class2.Vector4[4];// переменная для хранения значений для рисования координатных осей
            drRobotFL = new Class2.Vector4[5];// переменная для хранения значений для рисования поверхности основания
            drRobotFL_1 = new Class2.Vector4[5];// переменная для хранения значений для рисования поверхности основания

            // определяем длину робота для рисования футера
            var positionL = robot.LeftArm.FK(qL);
            var positionR = robot.RightArm.FK(qR);
            Vector3Length vL_Length = new Vector3Length(baseL , new[] { positionL.EndEffectorX, positionL.EndEffectorY, positionL.EndEffectorZ });
            Vector3Length vR_Length = new Vector3Length(baseR, new[] { positionR.EndEffectorX, positionR.EndEffectorY, positionR.EndEffectorZ });
            heid = (vL_Length.GetLength() > vR_Length.GetLength() ? (float)vL_Length.GetLength() : (float)vR_Length.GetLength());
            heid *= 1.1f;
            // double heid2 = Utils.MeasureError(baseL, new[] { positionL.EndEffectorX, positionL.EndEffectorY, positionL.EndEffectorZ });

            //рисуем координаты
            drRobotKoord_1[0] = new Class2.Vector4(0f, 0f, 0f, 1);
            drRobotKoord_1[1] = new Class2.Vector4(0.3f, 0f, 0f, 1);
            drRobotKoord_1[2] = new Class2.Vector4(0f, 0.3f, 0f, 1);
            drRobotKoord_1[3] = new Class2.Vector4(0f, 0f, 0.3f, 1);

     
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true); //включение режима двойной буферизации для отрисовки без мерцания

            // Создаем TrackBar для вращения и изменения проекции робота
            tbSize = new TrackBar { Parent = this,  Minimum = 0, Maximum = 400, Left = 900, Value = 200 };
            tbRoll = new TrackBar { Parent = this, Minimum = -180, Maximum = 180, Left = 1000, Value = 30 };
            tbPitch = new TrackBar { Parent = this, Minimum = -180, Maximum = 180, Left = 1100, Value = 20 };
            tbYaw = new TrackBar { Parent = this, Minimum = -180, Maximum = 180, Left = 1200, Value = 0 };

            labelSize = new Label { Parent = this, Left = 930, Top = 46, Text = "Масштаб" };
            labelRoll = new Label { Parent = this, Left = 1040, Top = 46, Text = "Roll" };
            labelPitch = new Label { Parent = this, Left = 1140, Top = 46, Text = "Pitch" };
            labelYaw = new Label { Parent = this, Left = 1240, Top = 46, Text = "Yaw" };

            labelVR = new Label { Parent = this, Left = 1130, Top = 70, Text = "Вращение" };

            tbSize.ValueChanged += tb_ValueChanged;
            tbRoll.ValueChanged += tb_ValueChanged;
            tbPitch.ValueChanged += tb_ValueChanged;
            tbYaw.ValueChanged += tb_ValueChanged;

            tb_ValueChanged(null, EventArgs.Empty);

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");// 

            int counter = 0;
            foreach (var joint in robot.LeftArm.DenavitHartenberg.Joints)
            {
                var min = Utils.RadiansToDegrees(joint.LowerLimit);
                var max = Utils.RadiansToDegrees(joint.UpperLimit);

                var control = new JointControl(joint.Name, min, max);
                control.Value = Utils.RadiansToDegrees(0);
                control.ValueChanged += Control_ValueChanged;
                control.Tag = counter;
                qL[counter] = 0;
                control.Dock = DockStyle.Bottom;
                flowLayoutPanel1.Controls.Add(control);

                counter++;

            }
            counter = 0;
            foreach (var jointR in robot.RightArm.DenavitHartenberg.Joints)
            {
                var min = Utils.RadiansToDegrees(jointR.LowerLimit);
                var max = Utils.RadiansToDegrees(jointR.UpperLimit);

                var control = new JointControl(jointR.Name, min, max);
                control.Value = Utils.RadiansToDegrees(0);
                control.ValueChanged += Control_ValueChangedR;
                control.Tag = counter;
                qR[counter] = 0;
                control.Dock = DockStyle.Bottom;
                flowLayoutPanel3.Controls.Add(control);

                counter++;
                
            }
        }

        private void Control_ValueChanged(object sender, EventArgs e)
        {

            var jointControl = (JointControl)sender;
            var index = (int)jointControl.Tag;
            var value = jointControl.Value;
            qL[index] = Utils.DegreesToRadians(value);
            
            var positionL = robot.LeftArm.FK(qL);           
            var positionR = robot.RightArm.FK(qR);           

            CreateCube(size, position, yaw, pitch, roll, positionL, positionR, baseL, baseR);
            pictureBox2.Invalidate();
            Invalidate();
            Collision(positionL, positionR);
            
        }

        private void Control_ValueChangedR(object sender, EventArgs e)
        {

            var jointControl = (JointControl)sender;
            var index = (int)jointControl.Tag;
            var value = jointControl.Value;
            qR[index] = Utils.DegreesToRadians(value);

            var positionL = robot.LeftArm.FK(qL);
            var positionR = robot.RightArm.FK(qR);

            CreateCube(size, position, yaw, pitch, roll, positionL, positionR, baseL, baseR);
            pictureBox2.Invalidate();
            Invalidate();
     
            Collision(positionL, positionR);
        }


        void tb_ValueChanged(object sender, EventArgs e) // функция для изменения отображения робота, срабатывает на изменения TrackBar
        {
            size = tbSize.Value;
            roll = (float)(tbRoll.Value * Math.PI / 180);
            pitch = (float)(tbPitch.Value * Math.PI / 180);            
            yaw = (float)(tbYaw.Value * Math.PI / 180);

           // Console.WriteLine(tbPitch.Value);
            var positionL = robot.LeftArm.FK(qL);
            var positionR = robot.RightArm.FK(qR);
            CreateCube(size, position, yaw, pitch, roll, positionL, positionR, baseL, baseR);
           
            pictureBox2.Invalidate();
            Invalidate();
           
        }

          void CreateCube(float scale, Class2.Vector4 position, float yaw, float pitch, float roll, ArmPosition position2, ArmPosition position3, double [] baseL, double[] baseR)
        {
            //задаем координаты точек узлов робота
            //Создаем список сочленений
           
            //черный
            drRobotL[0] = new Class2.Vector4((float)baseL[0], (float)baseL[1], (float)baseL[2], 1);
            for (int i = 0; i < drRobotL.Length - 1; i++)
            {
                drRobotL[i+1] = new Class2.Vector4((float)position2.X[i], (float)position2.Y[i], (float)position2.Z[i], 1);
            }
           
            //красный
            drRobotR[0] = new Class2.Vector4((float)baseR[0], (float)baseR[1], (float)baseR[2], 1);
            for (int i = 0; i < drRobotR.Length - 1; i++)
            {
                drRobotR[i + 1] = new Class2.Vector4((float)position3.X[i], (float)position3.Y[i], (float)position3.Z[i], 1);
            }


            //рисуем рамку
            var dlin = Utils.Vector3Length(baseR, baseL); //длина между руками
            drRobotFL_1[0] = new Class2.Vector4((float)-dlin, (float)-dlin, -heid, 1);
            drRobotFL_1[1] = new Class2.Vector4((float)dlin, (float)-dlin, -heid, 1);
            drRobotFL_1[2] = new Class2.Vector4((float)dlin, (float)dlin, -heid, 1);
            drRobotFL_1[3] = new Class2.Vector4((float)-dlin, (float)dlin, -heid, 1);
            drRobotFL_1[4] = new Class2.Vector4((float)-dlin, (float)dlin, -heid, 1);
            //матрица масштабирования
            var scaleM = Class2.Matrix4x4.CreateScale(scale);
            //матрица вращения
            var rotateM = Class2.Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll);
            //матрица переноса
            var translateM = Class2.Matrix4x4.CreateTranslation(position);
            //результирующая матрица
            var m = translateM * rotateM * scaleM;
            //умножаем векторы на матрицу
            for (int i = 0; i < drRobotL.Length; i++)
                drRobotL[i] = m * drRobotL[i];
            for (int i = 0; i < drRobotR.Length; i++)
                drRobotR[i] = m * drRobotR[i];
            for (int i = 0; i < drRobotKoord.Length; i++)
                drRobotKoord[i] = m * drRobotKoord_1[i];
            for (int i = 0; i < drRobotFL.Length; i++)
                drRobotFL[i] = m * drRobotFL_1[i];
            
        }


        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            //создаем матрицу проекции на плоскость XY
            //var paneXY = new Class2.Matrix4x4() { V00 = 2f, V11 = 2f, V33 = 1f };
            var paneXZ = new Class2.Matrix4x4() { V00 = 1f, V12 = -1f, V33 = 1f };
            //рисуем
            DrawRobot(e.Graphics, new PointF(pictureBox2.Width/2, pictureBox2.Height / 5), paneXZ);
        }

        void DrawRobot(Graphics gr, PointF startPoint, Class2.Matrix4x4 projectionMatrix)
        {
            //проекция
            var pL = new Class2.Vector4[drRobotL.Length];
            var pR = new Class2.Vector4[drRobotR.Length];
            var pKoord = new Class2.Vector4[drRobotKoord.Length];
            var pRam = new Class2.Vector4[drRobotFL.Length];
            for (int i = 0; i < drRobotL.Length; i++)
            {
                pL[i] = projectionMatrix * drRobotL[i];
            }
            for (int i = 0; i < drRobotR.Length; i++)
            {
                pR[i] = projectionMatrix * drRobotR[i];
            }
            for (int i = 0; i < drRobotKoord.Length; i++)
            {
                pKoord[i] = projectionMatrix * drRobotKoord[i];
            }
            for (int i = 0; i < drRobotFL.Length; i++)
            {
                pRam[i] = projectionMatrix * drRobotFL[i];
            }
            //сдвигаем
            gr.ResetTransform();
            gr.TranslateTransform(startPoint.X, startPoint.Y);

            // рисуем рамку
            var path44 = new GraphicsPath();
            //var ttt = new[] { pL[0], pR[0] };
            AddLine(path44, pRam[0], pRam[1], pRam[2], pRam[3],  pRam[0]);
            var myWind2 = new Pen(Color.Gray, 2);
            gr.DrawPath(myWind2, path44);
            //Закрашиваем область
            var BackC = new SolidBrush(BackColor);
            var BlackC = new SolidBrush(Color.Black);// кисть
            gr.FillPath(BackC, path44);
            // проставляем цифры по Х
            var krX=(pRam[1].X - pRam[0].X) / 6;
            var krY = (pRam[1].Y - pRam[0].Y) / 6;
            //var krXVal = (Math.Abs(baseR[0] * 2) + Math.Abs(baseL[0] * 2))/6;
            var dlin=Utils.Vector3Length(baseR, baseL); //длина между руками
            var krXVal = dlin*2 / 6;//шаг для увеличения значений
            gr.DrawString((-dlin + 0 * krXVal).ToString("f2"), new Font("Arial", 10), BlackC, pRam[0].X + 0 * krX, pRam[0].Y + 0 * krY);
            gr.DrawString((-dlin + 1 * krXVal).ToString("f2"), new Font("Arial", 10), BlackC, pRam[0].X + 1 * krX, pRam[0].Y + 1 * krY);
            gr.DrawString((-dlin + 2 * krXVal).ToString("f2"), new Font("Arial", 10), BlackC, pRam[0].X + 2 * krX, pRam[0].Y + 2 * krY);
            gr.DrawString((-dlin + 3 * krXVal).ToString("f2"), new Font("Arial", 10), BlackC, pRam[0].X + 3 * krX, pRam[0].Y + 3 * krY);
            gr.DrawString((-dlin + 4 * krXVal).ToString("f2"), new Font("Arial", 10), BlackC, pRam[0].X + 4 * krX, pRam[0].Y + 4 * krY);
            gr.DrawString((-dlin + 5 * krXVal).ToString("f2"), new Font("Arial", 10), BlackC, pRam[0].X + 5 * krX, pRam[0].Y + 5 * krY);
            gr.DrawString((-dlin + 6 * krXVal).ToString("f2"), new Font("Arial", 10), BlackC, pRam[0].X + 6 * krX, pRam[0].Y + 6 * krY);
            // проставляем цифры по Y
            krX = (pRam[3].X - pRam[0].X) / 6;
             krY = (pRam[3].Y - pRam[0].Y) / 6;
            gr.DrawString((-dlin + 0 * krXVal).ToString("f2"), new Font("Arial", 10), BlackC, pRam[0].X + 0 * krX, pRam[0].Y + 0 * krY);
            gr.DrawString((-dlin + 1 * krXVal).ToString("f2"), new Font("Arial", 10), BlackC, pRam[0].X + 1 * krX, pRam[0].Y + 1 * krY);
            gr.DrawString((-dlin + 2 * krXVal).ToString("f2"), new Font("Arial", 10), BlackC, pRam[0].X + 2 * krX, pRam[0].Y + 2 * krY);
            gr.DrawString((-dlin + 3 * krXVal).ToString("f2"), new Font("Arial", 10), BlackC, pRam[0].X + 3 * krX, pRam[0].Y + 3 * krY);
            gr.DrawString((-dlin + 4 * krXVal).ToString("f2"), new Font("Arial", 10), BlackC, pRam[0].X + 4 * krX, pRam[0].Y + 4 * krY);
            gr.DrawString((-dlin + 5 * krXVal).ToString("f2"), new Font("Arial", 10), BlackC, pRam[0].X + 5 * krX, pRam[0].Y + 5 * krY);
            gr.DrawString((-dlin + 6 * krXVal).ToString("f2"), new Font("Arial", 10), BlackC, pRam[0].X + 6 * krX, pRam[0].Y + 6 * krY);
            
            
            //рисуем левое звено
            var path = new GraphicsPath();
            AddLine(path, pL);
            Pen myWind = new Pen(Color.Blue, sZveno);
            gr.DrawPath(myWind, path);
            for (int i = 0; i < pL.Length; i++)
            {
                gr.DrawEllipse(Pens.Blue, -diam/2 + (float)pL[i].X, -diam/2 + (float)pL[i].Y, (float)diam, (float)diam);// рисуем кружочки
                gr.FillEllipse(new SolidBrush(Color.Blue), -diam / 2 + (float)pL[i].X, -diam / 2 + (float)pL[i].Y, (float)diam, (float)diam); //закрашиваем
                
            }
          
            //рисуем Правое звено
            var pathR = new GraphicsPath();
            AddLine(pathR, pR); 
            myWind = new Pen(Color.Red, sZveno);
            gr.DrawPath(myWind, pathR);

            for (int i = 0; i < pR.Length; i++)
            {
                gr.DrawEllipse(Pens.Red, -diam / 2 + (float)pR[i].X, -diam / 2 + (float)pR[i].Y, (float)diam, (float)diam);// рисуем кружочки
                gr.FillEllipse(new SolidBrush(Color.Red), -diam / 2 + (float)pR[i].X, -diam / 2 + (float)pR[i].Y, (float)diam, (float)diam); //закрашиваем
               
            }
            // выводим имена узлов
            for (int i = 0; i < pR.Length-1; i++)
            {
                if((robot.RightArm.DenavitHartenberg.Joints[i].D!=0) || (robot.RightArm.DenavitHartenberg.Joints[i].A != 0))
                gr.DrawString(robot.RightArm.DenavitHartenberg.Joints[i].Name, new Font("Arial", 10), BlackC, (float)pR[i].X+10, (float)pR[i].Y);
         
            }
            for (int i = 0; i < pL.Length - 1; i++)
            {
                if ((robot.LeftArm.DenavitHartenberg.Joints[i].D != 0) || (robot.LeftArm.DenavitHartenberg.Joints[i].A != 0))
                    gr.DrawString(robot.LeftArm.DenavitHartenberg.Joints[i].Name, new Font("Arial", 10), BlackC, (float)pL[i].X+10, (float)pL[i].Y);

            }
            //Рисуем черную линию между манипуляторами
            var path3 = new GraphicsPath();
            var ttt= new[] { pL[0], pR[0] };
            AddLine(path3, ttt);
            myWind = new Pen(Color.Black, 4);
            gr.DrawPath(myWind, path3);
            var downHeight = 520;
            var downWein = 290;

            // рисуем координаты
            pKoord[0].X = pKoord[0].X - downWein;
            pKoord[1].X = pKoord[1].X - downWein;
            pKoord[2].X = pKoord[2].X - downWein;
            pKoord[3].X = pKoord[3].X - downWein;
            pKoord[0].Y = pKoord[0].Y + downHeight;
            pKoord[1].Y = pKoord[1].Y + downHeight;
            pKoord[2].Y = pKoord[2].Y + downHeight; 
            pKoord[3].Y = pKoord[3].Y + downHeight;
            
            // рисуем ось X
            var path4= new GraphicsPath();
            //var ttt = new[] { pL[0], pR[0] };
            AddLine(path4, pKoord[0], pKoord[1]);
            myWind = new Pen(Color.Green, 2);
            gr.DrawPath(myWind, path4);
            gr.DrawString("X", new Font("Arial", 12), BlackC, pKoord[1].X, pKoord[1].Y-10);
            
            // рисуем ось Y
            path4 = new GraphicsPath();
            AddLine(path4, pKoord[0], pKoord[2]);
            myWind = new Pen(Color.Red, 2);
            gr.DrawPath(myWind, path4);
            gr.DrawString("Y", new Font("Arial", 12), BlackC, pKoord[2].X, pKoord[2].Y-10);

            // рисуем ось Z
            path4 = new GraphicsPath();
            AddLine(path4, pKoord[0], pKoord[3]);
            //рисуем
            myWind = new Pen(Color.Blue, 2);
            gr.DrawPath(myWind, path4);
            gr.DrawString("Z", new Font("Arial", 12), new SolidBrush(Color.Black), pKoord[3].X, pKoord[3].Y-10);

        }

        void AddLine(GraphicsPath path, params Class2.Vector4[] points)
        {
            foreach (var p in points)
                path.AddLines(new PointF[] { new PointF(p.X, p.Y) });
        }

        // в начальное отображение
        private void button1_Click_1(object sender, EventArgs e)
        {
            tbSize.Value = 200;
            tbRoll.Value = 30;
            tbPitch.Value = 20;
            tbYaw.Value = 0;

            size = tbSize.Value;
            pitch = (float)(tbPitch.Value * Math.PI / 180);
            roll = (float)(tbRoll.Value * Math.PI / 180);
            yaw = (float)(tbYaw.Value * Math.PI / 180);           

            var positionL = robot.LeftArm.FK(qL);
            var positionR = robot.RightArm.FK(qR);
            CreateCube(size, position, yaw, pitch, roll, positionL, positionR, baseL, baseR);

            pictureBox2.Invalidate();
            Invalidate();

        }
        // прямая задача
        private void button_FK_Click(object sender, EventArgs e)
        {
            //label_FK.Text=(string)
            var positionL = robot.LeftArm.FK(qL);
            var positionR = robot.RightArm.FK(qR);

            textBoxLeftX.Text = positionL.EndEffectorX.ToString("F3");
            textBoxLeftY.Text = positionL.EndEffectorY.ToString("F3");
            textBoxLeftZ.Text = positionL.EndEffectorZ.ToString("F3");
           
            textBoxLeftRoll.Text = Utils.RadiansToDegrees(positionL.Roll).ToString("F3");
            textBoxLeftPich.Text = Utils.RadiansToDegrees(positionL.Pitch).ToString("F3");
            textBoxLeftYaw.Text = Utils.RadiansToDegrees(positionL.Yaw).ToString("F3");

        }
 
        private void button2_LeftIk_Click(object sender, EventArgs e)
        {
            
            var s1 = Convert.ToDouble(textBoxLeftX.Text);
            var s2 = Convert.ToDouble(textBoxLeftY.Text);
            var s3 = Convert.ToDouble(textBoxLeftZ.Text);
            
            var translation = new[] { s1, s2, s3 };
            ArmPosition ress;

            if (checkBox1.Checked)
            {
                var r1 = Convert.ToDouble(textBoxLeftRoll.Text);
                var r2 = Convert.ToDouble(textBoxLeftPich.Text);
                var r3 = Convert.ToDouble(textBoxLeftYaw.Text);
                var rotation = Utils.DegreesToRadians(r1, r2, r3);
                
                 ress=robot.LeftArm.IK(translation, rotation, qL);
                
            }
            else
            {
                 ress = robot.LeftArm.IK(translation, new double[3], qL, false);
                
            }

            qL = ress.MotorAngles;
            var positionL = robot.LeftArm.FK(qL);
            var positionR = robot.RightArm.FK(qR);
            
            var controls = flowLayoutPanel1.Controls.OfType<JointControl>();
            var index = 0;
            foreach (var control in controls)
            {
                control.Value = Utils.RadiansToDegrees(qL[index]);
                index++;
            }
            var point1 = new[] { positionL.EndEffectorX, positionL.EndEffectorY, positionL.EndEffectorZ };
            var error = Utils.MeasureError(translation, point1);
            textBoxLeftErr.Text = error.ToString("F3");
            CreateCube(size, position, yaw, pitch, roll, positionL, positionR, baseL, baseR);

            pictureBox2.Invalidate();
            Invalidate();

            Collision(positionL, positionR);


        }

        private void Collision(ArmPosition positionL, ArmPosition positionR)
        {
            positionL = robot.LeftArm.FK(qL);
            positionR = robot.RightArm.FK(qR);
            var points = positionL.X.Zip(positionL.Y, (x, y) => new PointF((float)x, (float)y)).ToArray();
            if (isColidedCheckbox.InvokeRequired)
            {
                isColidedCheckbox.BeginInvoke(new Action(() =>
                isColidedCheckbox.Checked = Collide.ArmToArm(positionL, positionR, 0.20, out double[] _, out double[] _)));
            }
            else
            {
                isColidedCheckbox.Checked = Collide.ArmToArm(positionL, positionR, 0.20, out double[] _, out double[] _);
            }
            if (isColidedCheckbox.Checked)
            {
                isColidedCheckbox.Text = "Произошло столкновение";
            }
            else
            {
                isColidedCheckbox.Text = "Столкновения нет";
            }

        }

        private void button2_Leftsbros_Click(object sender, EventArgs e)
        {
            size = tbSize.Value;
            pitch = (float)(tbPitch.Value * Math.PI / 180);
            roll = (float)(tbRoll.Value * Math.PI / 180);
            yaw = (float)(tbYaw.Value * Math.PI / 180);

            for (int i = 0; i < qL.Length; i++) {
                qL[i] = 0;
            }
           
            var controls = flowLayoutPanel1.Controls.OfType<JointControl>();
            var index = 0;
            foreach (var control in controls)
            {
                control.Value = Utils.RadiansToDegrees(qL[index]);
                index++;
            }
            var positionL = robot.LeftArm.FK(qL);
            var positionR = robot.RightArm.FK(qR);
            CreateCube(size, position, yaw, pitch, roll, positionL, positionR, baseL, baseR);

            pictureBox2.Invalidate();
            Invalidate();
        }


        private void button4_RightFk_Click(object sender, EventArgs e)
        {
            var positionL = robot.LeftArm.FK(qL);
            var positionR = robot.RightArm.FK(qR);

            textBoxRightX.Text = positionR.EndEffectorX.ToString("F3");
            textBoxRightY.Text = positionR.EndEffectorY.ToString("F3");
            textBoxRightZ.Text = positionR.EndEffectorZ.ToString("F3");

            textBoxRightRoll.Text = Utils.RadiansToDegrees(positionR.Roll).ToString("F3");
            textBoxRightPich.Text = Utils.RadiansToDegrees(positionR.Pitch).ToString("F3");
            textBoxRightYaw.Text = Utils.RadiansToDegrees(positionR.Yaw).ToString("F3");
        }

        private void button3_RightIk_Click(object sender, EventArgs e)
        {
            var s1 = Convert.ToDouble(textBoxRightX.Text);
            var s2 = Convert.ToDouble(textBoxRightY.Text);
            var s3 = Convert.ToDouble(textBoxRightZ.Text);

            var translation = new[] { s1, s2, s3 };
            ArmPosition ress;

            if (checkBox2.Checked)
            {
                var r1 = Convert.ToDouble(textBoxRightRoll.Text);
                var r2 = Convert.ToDouble(textBoxRightPich.Text);
                var r3 = Convert.ToDouble(textBoxRightYaw.Text);
                var rotation = Utils.DegreesToRadians(r1, r2, r3);

                ress = robot.RightArm.IK(translation, rotation, qR);

            }
            else
            {
                ress = robot.RightArm.IK(translation, new double[3], qR, false);

            }

            qR = ress.MotorAngles; // получем текущие углы двигателя
            var positionL = robot.LeftArm.FK(qL);
            var positionR = robot.RightArm.FK(qR);

            // заполняем текущими значениями JointControl
            var controls = flowLayoutPanel3.Controls.OfType<JointControl>();// выбираем все элементы JointControl на панели flowLayoutPanel3
            var index = 0;
            foreach (var control in controls)
            {
                control.Value = Utils.RadiansToDegrees(qR[index]); // заменяем Value 
                index++;
            }
            // вычисление ошибки
            var point1 = new[] { positionR.EndEffectorX, positionR.EndEffectorY, positionR.EndEffectorZ };
            var error = Utils.MeasureError(translation, point1);
            textBoxRightErr.Text = error.ToString("F3");

            CreateCube(size, position, yaw, pitch, roll, positionL, positionR, baseL, baseR);

            pictureBox2.Invalidate();
            Invalidate();
            Collision(positionL, positionR);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            textBoxRightRoll.Enabled = checkBox2.Checked;
            textBoxRightPich.Enabled = checkBox2.Checked;
            textBoxRightYaw.Enabled = checkBox2.Checked;
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            textBoxLeftRoll.Enabled = checkBox1.Checked;
            textBoxLeftPich.Enabled = checkBox1.Checked;
            textBoxLeftYaw.Enabled = checkBox1.Checked;
       
        }

        private void button_openSave_Click(object sender, EventArgs e)
        {
            robot = Robot.LoadFromFile("settings.xml");
            zagruzra();
        }

         void zagruzra() {

            qL = new double[robot.LeftArm.DenavitHartenberg.Joints.Count];// главная переменная текущего состояния левой руки
            qR = new double[robot.RightArm.DenavitHartenberg.Joints.Count];// главная переменная текущего состояния правой руки
                                                                           // drRobot = new Class2.Vector4[14];
            drRobotL = new Class2.Vector4[robot.LeftArm.DenavitHartenberg.Joints.Count + 1];// переменная для хранения значений для рисования левой руки
            drRobotR = new Class2.Vector4[robot.RightArm.DenavitHartenberg.Joints.Count + 1];// переменная для хранения значений для рисования правой руки

            baseL = new[] { robot.LeftArm.DenavitHartenberg.WorldFrame[0, 3], robot.LeftArm.DenavitHartenberg.WorldFrame[1, 3], robot.LeftArm.DenavitHartenberg.WorldFrame[2, 3] };// переменная хранящая базовое смещение относительно центра левого манипулятора
            baseR = new[] { robot.RightArm.DenavitHartenberg.WorldFrame[0, 3], robot.RightArm.DenavitHartenberg.WorldFrame[1, 3], robot.RightArm.DenavitHartenberg.WorldFrame[2, 3] };// переменная хранящая базовое смещение относительно центра правого манипулятора


            flowLayoutPanel1.Controls.Clear();
            int counter = 0;
            foreach (var joint in robot.LeftArm.DenavitHartenberg.Joints)
            {
                var min = Utils.RadiansToDegrees(joint.LowerLimit);
                var max = Utils.RadiansToDegrees(joint.UpperLimit);

                var control = new JointControl(joint.Name, min, max);
                control.Value = Utils.RadiansToDegrees(0);
                control.Name = joint.Name;
                control.ValueChanged += Control_ValueChanged;
                control.Tag = counter;
                qL[counter] = 0;
                control.Dock = DockStyle.Bottom;
                flowLayoutPanel1.Controls.Add(control);

                counter++;

            }


            flowLayoutPanel3.Controls.Clear();

            counter = 0;
            foreach (var jointR in robot.RightArm.DenavitHartenberg.Joints)
            {
                var min = Utils.RadiansToDegrees(jointR.LowerLimit);
                var max = Utils.RadiansToDegrees(jointR.UpperLimit);
                var control = new JointControl(jointR.Name, min, max);
                control.Value = Utils.RadiansToDegrees(0);
               
                control.ValueChanged += Control_ValueChangedR;
                control.Tag = counter;
                qR[counter] = 0;
                control.Dock = DockStyle.Bottom;
                flowLayoutPanel3.Controls.Add(control);

                counter++;

            }


            var positionL = robot.LeftArm.FK(qL);
            var positionR = robot.RightArm.FK(qR);

            Vector3Length vL_Length = new Vector3Length(baseL, new[] { positionL.EndEffectorX, positionL.EndEffectorY, positionL.EndEffectorZ });
            Vector3Length vR_Length = new Vector3Length(baseR, new[] { positionR.EndEffectorX, positionR.EndEffectorY, positionR.EndEffectorZ });
            heid = (vL_Length.GetLength() > vR_Length.GetLength() ? (float)vL_Length.GetLength() : (float)vR_Length.GetLength());
            heid *= 1.1f;
            CreateCube(size, position, yaw, pitch, roll, positionL, positionR, baseL, baseR);

            pictureBox2.Invalidate();
            Invalidate();

            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            robot = Robot.LoadFromFile("settings2.xml");
            zagruzra();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var controls = flowLayoutPanel1.Controls.OfType<JointControl>();
            var controls2 = panelLeft.Controls.OfType<TextBox>();
            Console.WriteLine(controls2);
            var index = 0;
            foreach (var control in controls2)
            {
                Console.WriteLine(control.Name);
                control.Text = "0";
                index++;
            }
        }
    }
     
}

   



