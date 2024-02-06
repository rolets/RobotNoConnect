using System;
using System.Drawing;
using System.Windows.Forms;
using RoboKinematics;
using CommandType = RoboKinematics.CommandType;
using Timer = System.Windows.Forms.Timer;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;      
using System.Net.Sockets;    
using System.Text;
using System.Collections.Generic;

namespace robot_ver5
{

    
    public partial class Form1 : Form
    {

        // Инициализация
        static IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        static int port = 1234;
        TcpListener server = new TcpListener(localAddr, port);


        private Class2.Vector4[] drRobotL;
        private Class2.Vector4[] drRobotR;
        private Class2.Vector4[] drRobotKoord;
        private Class2.Vector4[] drRobot_proek;
        private Class2.Vector4[] drRobotFL;
        private Class2.Vector4[] drRobotKoord_1;
        private Class2.Vector4[] drRobotFL_1;
        private Class2.Vector4[] drRobotFL_2;
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
        public string[] TypeJoinR;
        public string[] TypeJoinL;
        //private textbox labelSize
        public int mouseDown=0;
        public int firstX = 0;
        public int firstY = 0;


        public double[] qL;// главная переменная текущего состояния левой руки
        public double[] qR;// главная переменная текущего состояния правой руки

        public double[] baseL;
        public double[] baseR;
       
        public int diam = 12;//диаметр элипсов на звеньях
        public int sZveno = 9;// ширина звеньев
        float tSize2 = 100;
        //public string[] arStr; //Файл настроек

        public int koef_ = 1; //количество шагов
        public int timeKoef = 1; //пауза между шагами

        
        List<MyDataType> dataOld = new List<MyDataType>();
        public class MyDataType
        {
            public float var1;
            public float var2;
        }

        List<Class2.Vector4> pLEnd = new List<Class2.Vector4>() { };
        List<Class2.Vector4> pLEnd2 = new List<Class2.Vector4>() { };

        List<Class2.Vector4> pREnd = new List<Class2.Vector4>() { };
        List<Class2.Vector4> pREnd2 = new List<Class2.Vector4>() { };
        
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
             var leftArmWorldFrame = new double[,] { { 0, 0, 1, 0.4 }, { 0, 1, 0, 0 }, { -1, 0, 0, 0 }, { 0, 0, 0, 1 } };
             var rightArmWorldFrame = new double[,] { { 0, 0, 1, -0.4 }, { 0, 1, 0, 0 }, { -1, 0, 0, 0 }, { 0, 0, 0, 1 } };

             //var leftArm = new SerialLink(leftArmWorldFrame);
             //var rightArm = new SerialLink(rightArmWorldFrame);

            //robot=Data.robot.LoadFromFile("settings.xml");

            //              a,     d, alpha, theta, lowerLimit ,  upperLimit , string name ,  false, "Revol"
            // leftArm.AddJoint(0.16, 0, 1.5708, 0, -1.5740, 0, "L.ShoulderS", false, "Revol");
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

            Data.arStr = File.ReadAllLines("setting.txt");// читаем из файла адрес и порт
          //  robot = new Robot(leftArm, rightArm, arStr[0], Int32.Parse(arStr[1])); // Создаем объект robot с адресом и портом для полключения
            Data.robot = Robot.LoadFromFile("settings.xml");
            //Console.WriteLine("Загрузил из файла");
            // Data.robot.SaveToFile("settings.xml");
           // Data.robotForm2 = robot;
            //  qL = new double[Data.robot.LeftArm.DenavitHartenberg.Joints.Count];// главная переменная текущего состояния левой руки
            //qR = new double[Data.robot.RightArm.DenavitHartenberg.Joints.Count];// главная переменная текущего состояния правой руки
            //                                                               // drRobot = new Class2.Vector4[14];
            //drRobotL = new Class2.Vector4[Data.robot.LeftArm.DenavitHartenberg.Joints.Count + 1];// переменная для хранения значений для рисования левой руки
            //drRobotR = new Class2.Vector4[Data.robot.RightArm.DenavitHartenberg.Joints.Count + 1];// переменная для хранения значений для рисования правой руки

            //baseL = new[] { Data.robot.LeftArm.DenavitHartenberg.WorldFrame[0, 3], Data.robot.LeftArm.DenavitHartenberg.WorldFrame[1, 3], Data.robot.LeftArm.DenavitHartenberg.WorldFrame[2, 3] };// переменная хранящая базовое смещение относительно центра левого манипулятора
            //baseR = new[] { Data.robot.RightArm.DenavitHartenberg.WorldFrame[0, 3], Data.robot.RightArm.DenavitHartenberg.WorldFrame[1, 3], Data.robot.RightArm.DenavitHartenberg.WorldFrame[2, 3] };// переменная хранящая базовое смещение относительно центра правого манипулятора


            drRobotKoord = new Class2.Vector4[4];// переменная для хранения значений для рисования координатных осей
            drRobotKoord_1 = new Class2.Vector4[4];// переменная для хранения значений для рисования координатных осей
            drRobotFL = new Class2.Vector4[24];// переменная для хранения значений для рисования поверхности основания
            drRobotFL_1 = new Class2.Vector4[24];// переменная для хранения значений для рисования поверхности основания
            drRobot_proek = new Class2.Vector4[2];
            drRobotFL_2 = new Class2.Vector4[24];
            //heid = 1.1f;
            //double vL_Length = 0.0;
            //double vR_Length = 0.0;
            //var positionL = Data.robot.LeftArm.FK(qL);
            //var positionR = Data.robot.RightArm.FK(qR);
            //// определяем длину робота для рисования футера
            //for (int i = 0; i < positionL.JointsCount - 1; i++)
            //{
            //    double temp = Utils.Vector3Length(new[] { positionL.X[i], positionL.Y[i], positionL.Z[i] }, new[] { positionL.X[i + 1], positionL.Y[i + 1], positionL.Z[i + 1] });
            //    vL_Length = vL_Length + temp;

            //}
            //for (int i = 0; i < positionR.JointsCount - 1; i++)
            //{
            //    double temp = Utils.Vector3Length(new[] { positionR.X[i], positionR.Y[i], positionR.Z[i] }, new[] { positionR.X[i + 1], positionR.Y[i + 1], positionR.Z[i + 1] });
            //    vR_Length = vR_Length + temp;
            //}

            //heid = (vL_Length > vR_Length ? (float)vL_Length : (float)vR_Length);
            //heid *= 1.1f;
            // double heid2 = Utils.MeasureError(baseL, new[] { positionL.EndEffectorX, positionL.EndEffectorY, positionL.EndEffectorZ });

            //рисуем координатную ось
            drRobotKoord_1[0] = new Class2.Vector4(0f, 0f, 0f, 1);
            drRobotKoord_1[1] = new Class2.Vector4(0.3f, 0f, 0f, 1);
            drRobotKoord_1[2] = new Class2.Vector4(0f, 0.3f, 0f, 1);
            drRobotKoord_1[3] = new Class2.Vector4(0f, 0f, 0.3f, 1);
           
            
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true); //включение режима двойной буферизации для отрисовки без мерцания

            // Создаем TrackBar для вращения и изменения проекции робота
            tbSize = new TrackBar { Parent = this,  Minimum = 0, Maximum = 800, Left = 900, Top = 30, Value = (int)tSize2 };
            tbRoll = new TrackBar { Parent = this, Minimum = -280, Maximum = 280, Left = 1000, Top = 30, Value = 20 };
            tbPitch = new TrackBar { Parent = this, Minimum = -180, Maximum = 180, Left = 1100, Top = 30, Value = 20 };
            tbYaw = new TrackBar { Parent = this, Minimum = -180, Maximum = 180, Left = 1200, Top = 30, Value = 0 };

            labelSize = new Label { Parent = this, Left = 930, Top = 75, Text = "Масштаб" };
            labelRoll = new Label { Parent = this, Left = 1040, Top = 75, Text = "Roll" };
            labelPitch = new Label { Parent = this, Left = 1140, Top = 75, Text = "Pitch" };
            labelYaw = new Label { Parent = this, Left = 1240, Top = 75, Text = "Yaw" };

            labelVR = new Label { Parent = this, Left = 1130, Top = 100, Text = "Вращение" };
            
            loadRobot();
            //Data.robot.UrologGo("eee", "eee", "eee");
            tbSize.ValueChanged += tb_ValueChanged;
            tbRoll.ValueChanged += tb_ValueChanged;
            tbPitch.ValueChanged += tb_ValueChanged;
            tbYaw.ValueChanged += tb_ValueChanged;

            tb_ValueChanged(null, EventArgs.Empty);

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");// 

            

            //int counter = 0;
            //foreach (var joint in Data.robot.LeftArm.DenavitHartenberg.Joints)
            //{
            //    var min = Utils.RadiansToDegrees(joint.LowerLimit);
            //    var max = Utils.RadiansToDegrees(joint.UpperLimit);

            //    var control = new JointControl(joint.Name, min, max);
            //    control.Value = Utils.RadiansToDegrees(0);
            //    control.ValueChanged += Control_ValueChanged;
            //    control.Tag = counter;
            //    qL[counter] = 0;
            //    control.Dock = DockStyle.Bottom;
            //    flowLayoutPanel1.Controls.Add(control);

            //    counter++;

            //}
            //counter = 0;
            //foreach (var jointR in Data.robot.RightArm.DenavitHartenberg.Joints)
            //{
            //    var min = Utils.RadiansToDegrees(jointR.LowerLimit);
            //    var max = Utils.RadiansToDegrees(jointR.UpperLimit);

            //    var control = new JointControl(jointR.Name, min, max);

            //    control.Value = Utils.RadiansToDegrees(0);
            //    control.ValueChanged += Control_ValueChangedR;
            //    control.Tag = counter;
            //    qR[counter] = 0;
            //    control.Dock = DockStyle.Bottom;
            //    flowLayoutPanel3.Controls.Add(control);

            //    counter++;

            //}
        }

        private void Control_ValueChanged(object sender, EventArgs e)
        {

            var jointControl = (JointControl)sender;
            var index = (int)jointControl.Tag;
            var value = jointControl.Value;
            if (TypeJoinL[index] == "Revol")
            {
                qL[index] = Utils.DegreesToRadians(value);
               // Data.robot.LeftArm.DenavitHartenberg.Joints[index].QCurrent = Utils.DegreesToRadians(value);

 
            }
            else
            {
                qL[index] = value / 100;
               //Data.robot.LeftArm.DenavitHartenberg.Joints[index].QCurrent = value / 100;
            }
           
            
            var positionL = Data.robot.LeftArm.FK(qL);
            var positionR = Data.robot.RightArm.FK(qR);           

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
            if (TypeJoinR[index] == "Revol")
            {
                qR[index] = Utils.DegreesToRadians(value);
            }
            else
            {
                qR[index] = value/100;
            }
            var positionL = Data.robot.LeftArm.FK(qL);
            var positionR = Data.robot.RightArm.FK(qR);

                    

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

          
            var positionL = Data.robot.LeftArm.FK(qL);
            var positionR = Data.robot.RightArm.FK(qR);
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
            if (Data.arStr[6] == "1")
            {
                pLEnd.Add(new Class2.Vector4((float)position2.EndEffectorX, (float)position2.EndEffectorY, (float)position2.EndEffectorZ, 1));
                pLEnd2.Add(new Class2.Vector4((float)position2.EndEffectorX, (float)position2.EndEffectorY, (float)position2.EndEffectorZ, 1)); 
                
                pREnd.Add(new Class2.Vector4((float)position3.EndEffectorX, (float)position3.EndEffectorY, (float)position3.EndEffectorZ, 1));
                pREnd2.Add(new Class2.Vector4((float)position3.EndEffectorX, (float)position3.EndEffectorY, (float)position3.EndEffectorZ, 1));
            }

           


       


            drRobot_proek[0] = new Class2.Vector4((float)position2.EndEffectorX, (float)position2.EndEffectorY, -heid, 1);
            drRobot_proek[1] = new Class2.Vector4((float)position3.EndEffectorX, (float)position3.EndEffectorY, -heid, 1);
            //рисуем рамку


            //drRobotFL_1[0] = new Class2.Vector4((float)-dlin, (float)-dlin, -heid, 1);
            //drRobotFL_1[1] = new Class2.Vector4((float)dlin, (float)-dlin, -heid, 1);
            //drRobotFL_1[2] = new Class2.Vector4((float)dlin, (float)dlin, -heid, 1);
            //drRobotFL_1[3] = new Class2.Vector4((float)-dlin, (float)dlin, -heid, 1);
            //drRobotFL_1[4] = new Class2.Vector4((float)-dlin, (float)dlin, -heid, 1);


            drRobotFL_1[0] = new Class2.Vector4((float)-heid, (float)-heid, -heid, 1);
            drRobotFL_1[1] = new Class2.Vector4((float)heid, (float)-heid, -heid, 1);
            drRobotFL_1[2] = new Class2.Vector4((float)heid, (float)heid, -heid, 1);
            drRobotFL_1[3] = new Class2.Vector4((float)-heid, (float)heid, -heid, 1);





            var setka = -heid;
            var setka2 = heid;
            var koef = (setka2 - setka) / 6;
            // линии по вертикали
            drRobotFL_1[4] = new Class2.Vector4((float)(koef + setka),(float)setka, -heid, 1);
            drRobotFL_1[5] = new Class2.Vector4((float)(koef + setka), (float)setka2, -heid, 1);

            drRobotFL_1[6] = new Class2.Vector4((float)(koef*2 + setka), (float)setka, -heid, 1);
            drRobotFL_1[7] = new Class2.Vector4((float)(koef*2 + setka), (float)setka2, -heid, 1);

            drRobotFL_1[8] = new Class2.Vector4((float)(koef*3 + setka), (float)setka, -heid, 1);
            drRobotFL_1[9] = new Class2.Vector4((float)(koef*3 + setka), (float)setka2, -heid, 1);

            drRobotFL_1[10] = new Class2.Vector4((float)(koef*4 + setka), (float)setka, -heid, 1);
            drRobotFL_1[11] = new Class2.Vector4((float)(koef*4 + setka), (float)setka2, -heid, 1);

            drRobotFL_1[12] = new Class2.Vector4((float)(koef*5 + setka), (float)setka, -heid, 1);
            drRobotFL_1[13] = new Class2.Vector4((float)(koef*5 + setka), (float)setka2, -heid, 1);

            // линии по горизонтали
            drRobotFL_1[14] = new Class2.Vector4((float)setka,(float)(koef + setka),  -heid, 1);
            drRobotFL_1[15] = new Class2.Vector4((float)setka2,(float)(koef + setka),  -heid, 1);

            drRobotFL_1[16] = new Class2.Vector4((float)setka,(float)(koef * 2 + setka),  -heid, 1);
            drRobotFL_1[17] = new Class2.Vector4((float)setka2,(float)(koef * 2 + setka),  -heid, 1);

            drRobotFL_1[18] = new Class2.Vector4((float)setka,(float)(koef * 3 + setka),  -heid, 1);
            drRobotFL_1[19] = new Class2.Vector4((float)setka2,(float)(koef * 3 + setka),  -heid, 1);

            drRobotFL_1[20] = new Class2.Vector4((float)setka,(float)(koef * 4 + setka),  -heid, 1);
            drRobotFL_1[21] = new Class2.Vector4((float)setka2,(float)(koef * 4 + setka),  -heid, 1);

            drRobotFL_1[22] = new Class2.Vector4((float)setka,(float)(koef * 5 + setka),  -heid, 1);
            drRobotFL_1[23] = new Class2.Vector4((float)setka2,(float)(koef * 5 + setka),  -heid, 1);


            // стенка
            drRobotFL_2[0] = new Class2.Vector4((float)-heid, (float)heid, -heid, 1);
            drRobotFL_2[1] = new Class2.Vector4((float)heid, (float)heid, -heid, 1);
            drRobotFL_2[2] = new Class2.Vector4((float)heid, (float)heid, heid/100, 1);
            drRobotFL_2[3] = new Class2.Vector4((float)-heid, (float)heid, heid/100, 1);

            drRobotFL_2[4] = new Class2.Vector4((float)-heid, (float)-heid, -heid, 1);
            drRobotFL_2[5] = new Class2.Vector4((float)heid, (float)-heid, -heid, 1);
            drRobotFL_2[6] = new Class2.Vector4((float)heid, (float)-heid, heid / 100, 1);
            drRobotFL_2[7] = new Class2.Vector4((float)-heid, (float)-heid, heid / 100, 1);

            drRobotFL_2[8] = new Class2.Vector4((float)heid, (float)heid, -heid, 1);
            drRobotFL_2[9] = new Class2.Vector4((float)heid, (float)-heid, -heid, 1);
            drRobotFL_2[10] = new Class2.Vector4((float)heid, (float)-heid, heid / 100, 1);
            drRobotFL_2[11] = new Class2.Vector4((float)heid, (float)heid, heid / 100, 1);

            drRobotFL_2[12] = new Class2.Vector4((float)-heid, (float)heid, -heid, 1);
            drRobotFL_2[13] = new Class2.Vector4((float)-heid, (float)-heid, -heid, 1);
            drRobotFL_2[14] = new Class2.Vector4((float)-heid, (float)-heid, heid / 100, 1);
            drRobotFL_2[15] = new Class2.Vector4((float)-heid, (float)heid, heid / 100, 1);


            //матрица масштабирования
            var scaleM = Class2.Matrix4x4.CreateScale(scale);
            var scaleMKoord = Class2.Matrix4x4.CreateScale(150);
            //матрица вращения
            var rotateM = Class2.Matrix4x4.CreateFromYawPitchRoll(yaw, pitch, roll);
            //матрица переноса
            var translateM = Class2.Matrix4x4.CreateTranslation(position);
            //результирующая матрица
            var m = translateM * rotateM * scaleM;
            var mKoord = translateM * rotateM * scaleMKoord; 
            //умножаем векторы на матрицу
            for (int i = 0; i < drRobotL.Length; i++)
                drRobotL[i] = m * drRobotL[i];
            for (int i = 0; i < drRobotR.Length; i++)
                drRobotR[i] = m * drRobotR[i];
            for (int i = 0; i < drRobotKoord.Length; i++)
                drRobotKoord[i] = mKoord * drRobotKoord_1[i];
            for (int i = 0; i < drRobotFL.Length; i++)
                drRobotFL[i] = m * drRobotFL_1[i];
            for (int i = 0; i < drRobot_proek.Length; i++)
                drRobot_proek[i] = m * drRobot_proek[i];
            for (int i = 0; i < drRobotFL_2.Length; i++)
                drRobotFL_2[i] = m * drRobotFL_2[i];
            for (int i = 0; i < pLEnd.Count; i++)
                pLEnd2[i] = m * pLEnd[i];
            for (int i = 0; i < pREnd.Count; i++)
                pREnd2[i] = m * pREnd[i];
            // pLEnd2.Add( m * pLEnd[i]);

            

        }
       


        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            //создаем матрицу проекции на плоскость XY
            var paneXY = new Class2.Matrix4x4() { V00 = 1f, V11 = -1f, V33 = 1f };
            var paneXZ = new Class2.Matrix4x4() { V00 = 1f, V12 = -1f, V33 = 1f };
            //var paneXZ = new Class2.Matrix4x4() { V00 = 1f, V11 = 1f, V12 = -1f, V22 = 1f, V33 = 1f };
            //Console.WriteLine(paneXZ.V00+" " + paneXZ.V01 + " " + paneXZ.V02 + " " + paneXZ.V03);
            //Console.WriteLine(paneXZ.V10 + " " + paneXZ.V11 + " " + paneXZ.V12 + " " + paneXZ.V13);
            //Console.WriteLine(paneXZ.V20 + " " + paneXZ.V21 + " " + paneXZ.V22 + " " + paneXZ.V23);
            //Console.WriteLine(paneXZ.V30 + " " + paneXZ.V31 + " " + paneXZ.V32 + " " + paneXZ.V33);
            //рисуем
            DrawRobot(e.Graphics, new PointF(pictureBox2.Width/2, pictureBox2.Height / 3), paneXZ);
        }
        Class2.Vector4[] Project2(Class2.Matrix4x4 projectionMatrix, Class2.Vector4[] drRobot)// умножаем на матрицу проекции
        {
            var temp = new Class2.Vector4[drRobot.Length];
            for (int i = 0; i < drRobot.Length; i++)
            {
                temp[i] = projectionMatrix * drRobot[i];
            }
            return temp;
        }
        void DrawRobot(Graphics gr, PointF startPoint, Class2.Matrix4x4 projectionMatrix)
        {
            //проекция
            var pL = new Class2.Vector4[drRobotL.Length];
            var pR = new Class2.Vector4[drRobotR.Length];
            var pKoord = new Class2.Vector4[drRobotKoord.Length];
            var pRam = new Class2.Vector4[drRobotFL.Length];
            var pRoek = new Class2.Vector4[drRobot_proek.Length];
            var pRamY = new Class2.Vector4[drRobotFL_2.Length];

            pL = Project2(projectionMatrix, drRobotL);
            pR = Project2(projectionMatrix, drRobotR);
            pKoord = Project2(projectionMatrix, drRobotKoord);
            pRam = Project2(projectionMatrix, drRobotFL);
            pRoek = Project2(projectionMatrix, drRobot_proek);
            pRamY = Project2(projectionMatrix, drRobotFL_2);

            for (int i = 0; i < pLEnd2.Count; i++)
            {
                pLEnd2[i] = projectionMatrix * pLEnd2[i];
            }
            for (int i = 0; i < pREnd2.Count; i++)
            {
                pREnd2[i] = projectionMatrix * pREnd2[i];
            }
            //for (int i = 0; i < drRobotL.Length; i++)
            //{
            //    pL[i] = projectionMatrix * drRobotL[i];
            //}

            gr.SmoothingMode = SmoothingMode.AntiAlias;
            //сдвигаем
            gr.ResetTransform();
            gr.TranslateTransform(startPoint.X, startPoint.Y);

            var RamkaGrey = new Pen(Color.Gray, 2); //цвет и толщина
            var BackCY1 = new SolidBrush(BackColor);// кисть
            // рисуем рамку сзади
            if (pRamY[0].X < pRamY[1].X)
            {
                var path55 = new GraphicsPath();
                AddLine(path55, pRamY[0], pRamY[1], pRamY[2], pRamY[3], pRamY[0]);
                gr.DrawPath(RamkaGrey, path55);
                //Закрашиваем область
                gr.FillPath(BackCY1, path55);
            }
            else
            {
                var path55 = new GraphicsPath();
                AddLine(path55, pRamY[4], pRamY[5], pRamY[6], pRamY[7], pRamY[4]);
                gr.DrawPath(RamkaGrey, path55);
                //Закрашиваем область
                gr.FillPath(BackCY1, path55);
            }
            // рисуем рамку сбоку
            if (pRamY[8].X< pRamY[9].X)
            { 
            var path66 = new GraphicsPath();
            AddLine(path66, pRamY[8], pRamY[9], pRamY[10], pRamY[11], pRamY[8]);
            gr.DrawPath(RamkaGrey, path66);
            //Закрашиваем область
            gr.FillPath(BackCY1, path66);
            }
            else
            {
                var path66 = new GraphicsPath();
                AddLine(path66, pRamY[12], pRamY[13], pRamY[14], pRamY[15], pRamY[12]);
                gr.DrawPath(RamkaGrey, path66);
                //Закрашиваем область
                gr.FillPath(BackCY1, path66);

            }

            // рисуем рамку снизу
            var path44 = new GraphicsPath();
            //var ttt = new[] { pL[0], pR[0] };
            AddLine(path44, pRam[0], pRam[1], pRam[2], pRam[3],  pRam[0]);
            var myWind2 = new Pen(Color.Gray, 2);
            gr.DrawPath(myWind2, path44);
            //Закрашиваем область
            var BackC = new SolidBrush(BackColor);
            var BlackC = new SolidBrush(Color.Black);// кисть
            gr.FillPath(BackC, path44);

            //рисуем сетку снизу
            var index = 0;
            for (int i = 0; i < 10; i++)
            {
                path44 = new GraphicsPath();
                AddLine(path44, pRam[index + 4], pRam[index + 5]);
                myWind2 = new Pen(Color.Gray, 1);
                gr.DrawPath(myWind2, path44);
                index++;
                index++;
            }

            // проставляем цифры по Х
            var krX = (pRam[1].X - pRam[0].X) / 6;
            var krY = (pRam[1].Y - pRam[0].Y) / 6;
            // проставляем цифры по Y
            var kr2X = (pRam[3].X - pRam[0].X) / 6;
            var kr2Y = (pRam[3].Y - pRam[0].Y) / 6;
            float heid = 1.5f;
            var dlin = heid;

            var krXVal = dlin * 2 / 6;//шаг для увеличения значений
            for (var i = 0; i < 7; i++)
            {
                gr.DrawString((-dlin + i * krXVal).ToString("f2"), new Font("Arial", 10), BlackC, pRam[0].X + i * krX, pRam[0].Y + i * krY);
                gr.DrawString((-dlin + i * krXVal).ToString("f2"), new Font("Arial", 10), BlackC, pRam[0].X + i * kr2X, pRam[0].Y + i * kr2Y);
            }

            // Пишем X Y
            gr.DrawString("X", new Font("Arial", 10), BlackC, pRam[0].X + 3 * krX - kr2X, pRam[0].Y + 3 * krY - kr2Y);
            gr.DrawString("Y", new Font("Arial", 10), BlackC, pRam[0].X + 3 * kr2X- krX, pRam[0].Y + 3 * kr2Y - krY);

            if (Data.robot.LeftArm.DenavitHartenberg.Joints.Count > 1)
            {
                //рисуем левое звено
                var path = new GraphicsPath();
                AddLine(path, pL);
                Pen myWindL = new Pen(Color.Blue, sZveno);
                myWindL.EndCap = System.Drawing.Drawing2D.LineCap.RoundAnchor;
                //myWindL.SetLineCap(LineCap.RoundAnchor, LineCap.RoundAnchor, DashCap.Triangle);
                
                gr.DrawPath(myWindL, path);
                for (int i = 0; i < pL.Length; i++)
                {
                    gr.DrawEllipse(Pens.Blue, -diam / 2 + (float)pL[i].X, -diam / 2 + (float)pL[i].Y, (float)diam, (float)diam);// рисуем кружочки
                    gr.FillEllipse(new SolidBrush(Color.Blue), -diam / 2 + (float)pL[i].X, -diam / 2 + (float)pL[i].Y, (float)diam, (float)diam); //закрашиваем

                }
                //точки на дне
                gr.DrawEllipse(Pens.Gray, -diam / 2 + (float)pRoek[0].X, -diam / 2 + (float)pRoek[0].Y, (float)diam, (float)diam);// рисуем кружочки
                gr.FillEllipse(new SolidBrush(Color.Gray), -diam / 2 + (float)pRoek[0].X, -diam / 2 + (float)pRoek[0].Y, (float)diam, (float)diam); //закрашиваем

                //оставляем след
                if (Data.arStr[6] == "1")// 
                {
                    for (int i = 0; i < pLEnd.Count; i++)
                    {
                        gr.DrawEllipse(Pens.Blue, -diam / 2 + (float)pLEnd2[i].X, -diam / 2 + (float)pLEnd2[i].Y, (float)diam - 5, (float)diam - 6);// рисуем кружочки
                                                                                                                                                    //gr.FillEllipse(new SolidBrush(Color.Blue), -diam / 2 + pLEnd2[i].X, -diam / 2 + pLEnd2[i].Y, (float)diam-5, (float)diam-5); //закрашиваем
                    }

   
                }
            }
            if (Data.robot.RightArm.DenavitHartenberg.Joints.Count > 1)
            {
                //рисуем Правое звено
                var pathR = new GraphicsPath();
                AddLine(pathR, pR);
                Pen myWindR = new Pen(Color.Red, sZveno);
                gr.DrawPath(myWindR, pathR);

                for (int i = 0; i < pR.Length; i++)
                {
                    gr.DrawEllipse(Pens.Red, -diam / 2 + (float)pR[i].X, -diam / 2 + (float)pR[i].Y, (float)diam, (float)diam);// рисуем кружочки
                    gr.FillEllipse(new SolidBrush(Color.Red), -diam / 2 + (float)pR[i].X, -diam / 2 + (float)pR[i].Y, (float)diam, (float)diam); //закрашиваем
                }

                // Рисуем точки на дне
                gr.DrawEllipse(Pens.Gray, -diam / 2 + (float)pRoek[1].X, -diam / 2 + (float)pRoek[1].Y, (float)diam, (float)diam);// рисуем кружочки
                gr.FillEllipse(new SolidBrush(Color.Gray), -diam / 2 + (float)pRoek[1].X, -diam / 2 + (float)pRoek[1].Y, (float)diam, (float)diam); //закрашиваем

                //оставляем след
                if (Data.arStr[6] == "1")// 
                {
                  
                    for (int i = 0; i < pREnd.Count; i++)
                    {
                        gr.DrawEllipse(Pens.Red, -diam / 2 + (float)pREnd2[i].X, -diam / 2 + (float)pREnd2[i].Y, (float)diam - 5, (float)diam - 6);// рисуем кружочки
                                                                                                                                                   //gr.FillEllipse(new SolidBrush(Color.Blue), -diam / 2 + pLEnd2[i].X, -diam / 2 + pLEnd2[i].Y, (float)diam-5, (float)diam-5); //закрашиваем
                    }

                }
            }
            // выводим имена узлов
            //arStr = File.ReadAllLines("setting.txt");
            if (Data.arStr[2]=="1")
            { 
                for (int i = 0; i < pR.Length-1; i++)
                {
                    if((Data.robot.RightArm.DenavitHartenberg.Joints[i].D!=0) || (Data.robot.RightArm.DenavitHartenberg.Joints[i].A != 0))
                    gr.DrawString(Data.robot.RightArm.DenavitHartenberg.Joints[i].Name, new Font("Arial", 10), BlackC, (float)pR[i].X+10, (float)pR[i].Y);
         
                }
                for (int i = 0; i < pL.Length - 1; i++)
                {
                    if ((Data.robot.LeftArm.DenavitHartenberg.Joints[i].D != 0) || (Data.robot.LeftArm.DenavitHartenberg.Joints[i].A != 0))
                        gr.DrawString(Data.robot.LeftArm.DenavitHartenberg.Joints[i].Name, new Font("Arial", 10), BlackC, (float)pL[i].X+10, (float)pL[i].Y);

                }
            }
            if (Data.robot.RightArm.DenavitHartenberg.Joints.Count > 1)
                if (Data.robot.LeftArm.DenavitHartenberg.Joints.Count > 1)
                {
                    //Рисуем черную линию между манипуляторами
                    var path3 = new GraphicsPath();
                    var ttt = new[] { pL[0], pR[0] };
                    AddLine(path3, ttt);
                    Pen myWindBase = new Pen(Color.Black, 4);
                    // gr.DrawPath(myWind, path3);
                    gr.DrawLine(myWindBase, new PointF(pL[0].X, pL[0].Y), new PointF(pR[0].X, pR[0].Y));
                }
            // Смещаем координатрую ось вниз и влево
            var downHeight = 400;
            var downWidth = 290;            
            for (int i = 0; i < 4; i++) {
                pKoord[i].X = pKoord[i].X - downWidth;
                pKoord[i].Y = pKoord[i].Y + downHeight;
            }
            
            // рисуем ось X
            var path4= new GraphicsPath();
            //var ttt = new[] { pL[0], pR[0] };
            AddLine(path4, pKoord[0], pKoord[1]);
            Pen myWind = new Pen(Color.Green, 2);
            myWind.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            myWind.CustomEndCap = new AdjustableArrowCap(5, 5);

            gr.DrawPath(myWind, path4);
            gr.DrawString("X", new Font("Arial", 12), BlackC, pKoord[1].X, pKoord[1].Y-10);
            
            // рисуем ось Y
            path4 = new GraphicsPath();
            AddLine(path4, pKoord[0], pKoord[2]);
            myWind = new Pen(Color.Red, 2);
            myWind.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            myWind.CustomEndCap = new AdjustableArrowCap(5, 5);
            gr.DrawPath(myWind, path4);
            gr.DrawString("Y", new Font("Arial", 12), BlackC, pKoord[2].X, pKoord[2].Y-10);

            // рисуем ось Z
            path4 = new GraphicsPath();
            AddLine(path4, pKoord[0], pKoord[3]);
            //рисуем
            myWind = new Pen(Color.Blue, 2);
            myWind.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            myWind.CustomEndCap = new AdjustableArrowCap(5, 5);
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
            //tbSize.Value = 150;
            tbSize.Value = (int)tSize2;
            tbRoll.Value = 30;
            tbPitch.Value = 20;
            tbYaw.Value = 0;

            size = tbSize.Value;
            pitch = (float)(tbPitch.Value * Math.PI / 180);
            roll = (float)(tbRoll.Value * Math.PI / 180);
            yaw = (float)(tbYaw.Value * Math.PI / 180);           

            var positionL = Data.robot.LeftArm.FK(qL);
            var positionR = Data.robot.RightArm.FK(qR);
            CreateCube(size, position, yaw, pitch, roll, positionL, positionR, baseL, baseR);

            pictureBox2.Invalidate();
            Invalidate();

        }
        // прямая задача
        private void button_FK_Click(object sender, EventArgs e)
        {
            //label_FK.Text=(string)
            var positionL = Data.robot.LeftArm.FK(qL);
            var positionR = Data.robot.RightArm.FK(qR);

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
                
                 ress=Data.robot.LeftArm.IK(translation, rotation, qL);
                
            }
            else
            {
                 ress = Data.robot.LeftArm.IK(translation, new double[3], qL, false);
                
            }
           

                        var qL_old = qL;
            qL = ress.MotorAngles; // получем текущие углы двигателя

            koefPodgotovka(qL_old, "L");
            var positionL = Data.robot.LeftArm.FK(qL);
            var positionR = Data.robot.RightArm.FK(qR);
            // вычисление ошибки
            var point1 = new[] { positionL.EndEffectorX, positionL.EndEffectorY, positionL.EndEffectorZ };
            var error = Utils.MeasureError(translation, point1);
            textBoxLeftErr.Text = error.ToString("F3");
            //заполняем текущими значениями JointControl
            var controls = flowLayoutPanel3.Controls.OfType<JointControl>();// выбираем все элементы JointControl на панели flowLayoutPanel3
            var index = 0;
            foreach (var control in controls)
            {
                if (Data.robot.RightArm.DenavitHartenberg.Joints[index].TypeJoint == "Revol")
                    control.Value = Utils.RadiansToDegrees(qR[index]); // заменяем Value 
                if (Data.robot.RightArm.DenavitHartenberg.Joints[index].TypeJoint == "Prizm")
                    control.Value = qR[index] * 100; // заменяем Value 
                index++;
            }

            controls = flowLayoutPanel1.Controls.OfType<JointControl>();// выбираем все элементы JointControl на панели flowLayoutPanel3
            index = 0;
            foreach (var control in controls)
            {
                if (Data.robot.LeftArm.DenavitHartenberg.Joints[index].TypeJoint == "Revol")
                    control.Value = Utils.RadiansToDegrees(qL[index]); // заменяем Value 
                if (Data.robot.LeftArm.DenavitHartenberg.Joints[index].TypeJoint == "Prizm")
                    control.Value = qL[index] * 100; // заменяем Value 
                index++;
            }

        }

        private void Collision(ArmPosition positionL, ArmPosition positionR)
        {
            positionL = Data.robot.LeftArm.FK(qL);
            positionR = Data.robot.RightArm.FK(qR);
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

                if (control.Minimum > 0)
                {
                    if (TypeJoinL[index] == "Revol")
                    {
                        control.Value = control.Minimum;
                        qL[index] = Utils.DegreesToRadians(control.Minimum);
                    }
                    else
                    {
                        control.Value = control.Minimum;
                        qL[index] = control.Minimum/100;
                    }

                }
                else
                {
                    if (TypeJoinL[index] == "Prizm")
                    {
                        control.Value = qL[index];
                    }
                    else
                    {
                        control.Value = qL[index];

                    }


                }



  
                index++;
            }
            var positionL = Data.robot.LeftArm.FK(qL);
            var positionR = Data.robot.RightArm.FK(qR);
            CreateCube(size, position, yaw, pitch, roll, positionL, positionR, baseL, baseR);

            pictureBox2.Invalidate();
            Invalidate();
        }


        private void button4_RightFk_Click(object sender, EventArgs e)
        {
            var positionL = Data.robot.LeftArm.FK(qL);
            var positionR = Data.robot.RightArm.FK(qR);

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
            ArmPosition rightArmPosition;
            //double[] rotation = new[]  {0.0,0.0,0.0 };

            if (checkBox2.Checked)
            {
                var r1 = Convert.ToDouble(textBoxRightRoll.Text);
                var r2 = Convert.ToDouble(textBoxRightPich.Text);
                var r3 = Convert.ToDouble(textBoxRightYaw.Text);
                var rotation = Utils.DegreesToRadians(r1, r2, r3);

                ress = Data.robot.RightArm.IK(translation, rotation, qR);
              //  Data.robot.Connect();
               // var robotState = Data.robot.GetRobotState();
               // var leftPosition = robotState[0];
              //  var rightPosition = robotState[1];

                //var leftArmPosition = Data.robot.RightArm.GetPosition();
                //var currentMotorAngle = leftArmPosition.MotorAngles;


                //rightArmPosition = Data.robot.RightArm.MoveTo(translation, rotation, CommandType.Go, 2);
             //   Data.robot.Disconnect();

            }
            else
            {
                ress = Data.robot.RightArm.IK(translation, new double[3], qR, false);
               // rightArmPosition = Data.robot.RightArm.MoveTo(translation, CommandType.Go, 2);
            }
            
            var qR_old = qR;
            qR = ress.MotorAngles; // получем текущие углы двигателя
            koefPodgotovka(qR_old,"R");

            var positionL = Data.robot.LeftArm.FK(qL);
            var positionR = Data.robot.RightArm.FK(qR);
            // вычисление ошибки
            var point1 = new[] { positionR.EndEffectorX, positionR.EndEffectorY, positionR.EndEffectorZ };
            var error = Utils.MeasureError(translation, point1);
            textBoxRightErr.Text = error.ToString("F3");
            if (!isColidedCheckbox.Checked)
            {
           // Data.robot.Connect();
             //   if (checkBox2.Checked)
             //       rightArmPosition = Data.robot.RightArm.MoveTo(translation, rotation, CommandType.Go, 2);
              //  else
             //       rightArmPosition = Data.robot.RightArm.MoveTo(translation, CommandType.Go, 2);
            //    Data.robot.Disconnect();

            }
           



            //заполняем текущими значениями JointControl
            var controls = flowLayoutPanel3.Controls.OfType<JointControl>();// выбираем все элементы JointControl на панели flowLayoutPanel3
            var index = 0;
            foreach (var control in controls)
            {
                if (Data.robot.RightArm.DenavitHartenberg.Joints[index].TypeJoint=="Revol")
                control.Value = Utils.RadiansToDegrees(qR[index]); // заменяем Value 
                if (Data.robot.RightArm.DenavitHartenberg.Joints[index].TypeJoint == "Prizm")
                control.Value = qR[index]*100; // заменяем Value 
                index++;
            }

            controls = flowLayoutPanel1.Controls.OfType<JointControl>();// выбираем все элементы JointControl на панели flowLayoutPanel3
            index = 0;
            foreach (var control in controls)
            {
                if (Data.robot.LeftArm.DenavitHartenberg.Joints[index].TypeJoint == "Revol") 
                    control.Value = Utils.RadiansToDegrees(qL[index]); // заменяем Value 
                if (Data.robot.LeftArm.DenavitHartenberg.Joints[index].TypeJoint == "Prizm")
                    control.Value = qL[index] * 100; // заменяем Value 
                index++;
            }
        }

        public void koefPodgotovka(double[] old, string status)
        {

            koef_ = Convert.ToInt32(Data.arStr[4]);
            timeKoef = Convert.ToInt32(Data.arStr[5]);

            if (status == "R")
            {
                double[,] qR_new = new double[qR.Length, koef_ + 1];
                var koef = new double[qR.Length];
                for (var i = 0; i < qR.Length; i++)
                {
                    koef[i] = (qR[i] - old[i]) / koef_;
                }
                for (var i = 0; i < qR.Length; i++)
                {
                    for (var y = 0; y <= koef_; y++)
                    {
                        qR_new[i, y] = old[i] + y * koef[i];
                    }
                }
                if (koef_ != 1)
                {
                    for (var i = 0; i <= koef_; i++)
                    {
                    for (var y = 0; y < qR.Length; y++)
                    {
                        qR[y] = qR_new[y, i];
                    }
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(timeKoef);
                    movement();
                    }
                } else  movement();
            }
            if (status == "L")
            {
                double[,] qL_new = new double[qL.Length, koef_ + 1];
                var koef = new double[qL.Length];
                for (var i = 0; i < qL.Length; i++)
                {
                    koef[i] = (qL[i] - old[i]) / koef_;
                }
                for (var i = 0; i < qL.Length; i++)
                {
                    for (var y = 0; y <= koef_; y++)
                    {
                        qL_new[i, y] = old[i] + y * koef[i];
                    }
                }
                if (koef_!= 1) { 
                for (var i = 0; i <= koef_; i++)
                {
                    for (var y = 0; y < qL.Length; y++)
                    {
                        qL[y] = qL_new[y, i];
                    }
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(timeKoef);
                    movement();
                }
                }else
                    movement();
            }
            

        }
        public void movement()
          {

            var positionL = Data.robot.LeftArm.FK(qL);
            var positionR = Data.robot.RightArm.FK(qR);

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
            Data.robot = Robot.LoadFromFile("settings.xml");
            loadRobot();
          
            if (panel_Full.Visible)
            {

                button_EditRobot_Click(null, null);
            }
        }


         public void loadRobot() {
            //robot = Data.robotForm2;
            TypeJoinR = new string[16];
            TypeJoinL = new string[16];
            qL = new double[Data.robot.LeftArm.DenavitHartenberg.Joints.Count];// главная переменная текущего состояния левой руки
            qR = new double[Data.robot.RightArm.DenavitHartenberg.Joints.Count];// главная переменная текущего состояния правой руки
                                                                          
            drRobotL = new Class2.Vector4[Data.robot.LeftArm.DenavitHartenberg.Joints.Count + 1];// переменная для хранения значений для рисования левой руки
            drRobotR = new Class2.Vector4[Data.robot.RightArm.DenavitHartenberg.Joints.Count + 1];// переменная для хранения значений для рисования правой руки

            baseL = new[] { Data.robot.LeftArm.DenavitHartenberg.WorldFrame[0, 3], Data.robot.LeftArm.DenavitHartenberg.WorldFrame[1, 3], Data.robot.LeftArm.DenavitHartenberg.WorldFrame[2, 3] };// переменная хранящая базовое смещение относительно центра левого манипулятора
            baseR = new[] { Data.robot.RightArm.DenavitHartenberg.WorldFrame[0, 3], Data.robot.RightArm.DenavitHartenberg.WorldFrame[1, 3], Data.robot.RightArm.DenavitHartenberg.WorldFrame[2, 3] };// переменная хранящая базовое смещение относительно центра правого манипулятора


            flowLayoutPanel1.Controls.Clear();
            int counter = 0;
            foreach (var joint in Data.robot.LeftArm.DenavitHartenberg.Joints)
            {
                if (joint.Name != "None")
                {
                    double min;
                    double max;
                    if (joint.TypeJoint == "Revol")
                    {
                        min = Utils.RadiansToDegrees(joint.LowerLimit);
                        max = Utils.RadiansToDegrees(joint.UpperLimit);
                        
                    }
                    else
                    {
                        min = joint.LowerLimit*100;
                        max = joint.UpperLimit*100;

                        
                    }
                    var control = new JointControl(joint.Name, min, max);

                    if (min > 0)
                    {
                       
                        if (joint.TypeJoint == "Revol")
                        {
                            control.Value = (min);
                            qL[counter] = Utils.DegreesToRadians( min);
                        }
                        else
                        {
                            control.Value = min;
                            qL[counter] = min / 100;
                        }
                    }
                    else
                    {
                       
                        if (joint.TypeJoint == "Revol")
                        {
                            control.Value = Utils.RadiansToDegrees(joint.QCurrent);
                            qL[counter] = joint.QCurrent;

                        }
                        else
                        {
                            control.Value = joint.QCurrent*100;
                            qL[counter] = joint.QCurrent;

                        }

                    }


                    control.Name = joint.Name;
                    TypeJoinL[counter] = joint.TypeJoint;
                    control.ValueChanged += Control_ValueChanged;
                    control.Tag = counter;
                    

                    
                    control.Dock = DockStyle.Bottom;
                    flowLayoutPanel1.Controls.Add(control);

                    counter++;
                }
            }


            flowLayoutPanel3.Controls.Clear();
            //System.Threading.Thread.Sleep(1000);
            counter = 0;
            foreach (var jointR in Data.robot.RightArm.DenavitHartenberg.Joints)
            {
                if (jointR.Name != "None") 
                {

                    double min;
                    double max;
                    if (jointR.TypeJoint == "Revol")
                    {
                        min = Utils.RadiansToDegrees(jointR.LowerLimit);
                        max = Utils.RadiansToDegrees(jointR.UpperLimit);
                    }
                    else
                    {
                        min = jointR.LowerLimit*100;
                        max = jointR.UpperLimit * 100;
                    }

                var control = new JointControl(jointR.Name, min, max);
                    if (min > 0)
                    {
                        
                        if (jointR.TypeJoint == "Revol")
                        {
                            control.Value = (min);
                            qR[counter] = Utils.DegreesToRadians(min);

                        }
                        else {
                            control.Value = min;
                            qR[counter] = min / 100;
                        }
                    }
                    else
                    {
                        
                        if (jointR.TypeJoint == "Revol")
                        {
                            control.Value = Utils.RadiansToDegrees(jointR.QCurrent);
                            qR[counter] = jointR.QCurrent;

                        }
                        else
                        {
                            control.Value = jointR.QCurrent*100;
                            qR[counter] = jointR.QCurrent;

                        }
                        
                    }
                //control.Value = Utils.RadiansToDegrees(0); 
                TypeJoinR[counter] = jointR.TypeJoint;
                control.ValueChanged += Control_ValueChangedR;
                control.Tag = counter;         

                control.Dock = DockStyle.Bottom;
                flowLayoutPanel3.Controls.Add(control);

                counter++;
                }

            }

            var positionL = Data.robot.LeftArm.FK(qL);
            var positionR = Data.robot.RightArm.FK(qR);

            double vL_Length = 0.0;
            double vR_Length = 0.0;
            // длина от центра координат до начала первого звена
            vL_Length= Utils.Vector3Length(new[] { 0.0, 0.0, 0.0 },baseL );
            vR_Length = Utils.Vector3Length(new[] { 0.0, 0.0, 0.0 }, baseR );
            // длина от начала первого звена до начала второго
            vL_Length = vL_Length + Utils.Vector3Length(new[] { positionL.X[0], positionL.Y[0], positionL.Z[0] }, baseL);
            vR_Length = vR_Length + Utils.Vector3Length(new[] { positionR.X[0], positionR.Y[0], positionR.Z[0] }, baseR);
            
            // определяем длину робота для рисования размеров футера
            for (int i = 0; i < positionL.JointsCount - 1; i++)
            {
                double temp=0;
                if (Data.robot.LeftArm.DenavitHartenberg.Joints[i].TypeJoint == "Revol")
                {
                    temp = Utils.Vector3Length(new[] { positionL.X[i], positionL.Y[i], positionL.Z[i] }, new[] { positionL.X[i + 1], positionL.Y[i + 1], positionL.Z[i + 1] });
                }
                if (Data.robot.LeftArm.DenavitHartenberg.Joints[i].TypeJoint == "Prizm")
                {
                    
                    temp = Data.robot.LeftArm.DenavitHartenberg.Joints[i].UpperLimit;
                }
                vL_Length = vL_Length + temp;

            }
            for (int i = 0; i < positionR.JointsCount - 1; i++)
            {
                double temp = 0;
                if (Data.robot.RightArm.DenavitHartenberg.Joints[i].TypeJoint == "Revol")
                {
                    temp = Utils.Vector3Length(new[] { positionR.X[i], positionR.Y[i], positionR.Z[i] }, new[] { positionR.X[i + 1], positionR.Y[i + 1], positionR.Z[i + 1] });
                }
                if (Data.robot.RightArm.DenavitHartenberg.Joints[i].TypeJoint == "Prizm")
                {

                    temp = Data.robot.RightArm.DenavitHartenberg.Joints[i].UpperLimit;
                }
                vR_Length += temp;
            }
            // выбираем самое длинное звено
            heid = (vL_Length > vR_Length ? (float)vL_Length : (float)vR_Length);
            //heid *= 1.1f;
            if (heid != 0)
            {
                tSize2 = 200 / heid; //авто масштаб
                size = tSize2;
                tbSize.Value = (int)tSize2;
            }
            else
            {
                heid = 2;
                tSize2 = 200 / heid; //авто масштаб
                size = tSize2;
                tbSize.Value = (int)tSize2;

            }
        

            CreateCube(size, position, yaw, pitch, roll, positionL, positionR, baseL, baseR);
            pictureBox2.Invalidate();
            Invalidate();
            
         }

        public void button_EditRobot_Click(object sender, EventArgs e)// Кнопка редактировать робота
        {
            
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel3.Visible = false;
            pictureBox2.Visible = false;
            button2_Leftsbros.Visible = false;
            button_Rightsbros.Visible = false;
            panelLeft.Visible = false;
            panel1.Visible = false;           

            panel_Full.Visible = true;

            // заполняем TextBox из переменных robot.WorldFrame для левой и правой базы вращения
            double[] tempL = new double[16];
            double[] tempR = new double[16];
            int index = 0;
            
            for (int i = 0; i < 4; i++)
            {
                for (int y = 0; y < 4; y++)
                {
                    tempL[index] = Data.robot.LeftArm.DenavitHartenberg.WorldFrame[i, y];
                    tempR[index] = Data.robot.RightArm.DenavitHartenberg.WorldFrame[i, y];
                    index++;
                }
            }
           var controls = panel_baseL.Controls.OfType<TextBox>();
            
            foreach (var control in controls)
            {
                var i=control.Tag.ToString();
                control.Text = tempL[int.Parse(i)].ToString();
            }            
            controls = panel_baseR.Controls.OfType<TextBox>();
        
            foreach (var control in controls)
            {
                var i = control.Tag.ToString();
                control.Text = tempR[int.Parse(i)].ToString();
            }

            textBox_Port.Text = Data.robot.Port.ToString();
            textBox_IP.Text = Data.robot.IP;


                



            panel_Left.Controls.Clear();
            var counter = 0;
                       
            foreach (var jointL in Data.robot.LeftArm.DenavitHartenberg.Joints)
            {
                //double qtemp = 0;
                //if (Data.arStr[3] == "1")
                //{
                //    qtemp = qL[counter];
                //}
                //else
                //{
                //    qtemp = jointL.QCurrent;
                //}
                var control = new DHControl(jointL.A.ToString(), jointL.D.ToString(), jointL.Alpha.ToString(), jointL.Theta.ToString(), jointL.LowerLimit.ToString(), jointL.UpperLimit.ToString(), jointL.Name, qL[counter].ToString("F4"), jointL.TypeJoint, (counter).ToString());
                //jointL.QCurrent.ToString()
                control.Tag = counter;
                control.Dock = DockStyle.Bottom;
                panel_Left.Controls.Add(control);
                counter++;

            }
            panel_Right.Controls.Clear();
            counter = 0;
            foreach (var jointR in Data.robot.RightArm.DenavitHartenberg.Joints)
            {
                //double qtemp = 0;
                //if (Data.arStr[3] == "1")
                //{
                //    qtemp = qL[counter];
                //}
                //else
                //{
                //    qtemp = jointR.QCurrent;
                //}

                var control = new DHControl(jointR.A.ToString(), jointR.D.ToString(), jointR.Alpha.ToString(), jointR.Theta.ToString(), jointR.LowerLimit.ToString(), jointR.UpperLimit.ToString(), jointR.Name, qR[counter].ToString("F4"), jointR.TypeJoint, (counter).ToString());

                control.Tag = counter;
                control.Dock = DockStyle.Bottom;
                panel_Right.Controls.Add(control);
                counter++;

            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //Form_Setting newForm = new Form_Setting();
           // newForm.Show();

            Form_Setting newForm = new Form_Setting();
            newForm.Owner = this;
            newForm.ShowDialog();
        }

 

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button_SaveRobot_Click(object sender, EventArgs e)
        {

            //  var leftArmWorldFrame = new double[,] { { 0, 0, 1, 0.4 }, { 0, 1, 0, 0 }, { -1, 0, 0, 0 }, { 0, 0, 0, 1 } };
           //   var rightArmWorldFrame = new double[,] { { 0, 0, 1, -0.4 }, { 0, 1, 0, 0 }, { -1, 0, 0, 0 }, { 0, 0, 0, 1 } };
            var leftArmWorldFrame = new double[4, 4];
            var rightArmWorldFrame = new double[4, 4];



            //Сохраняем левую и правую базу вращения из TextBox в переменные leftArmWorldFrame и rightArmWorldFrame

            double[] tempL = new double[16];
            double[] tempR = new double[16];
            int index = 0;
            var controls3 = panel_baseL.Controls.OfType<TextBox>();
            
            foreach (var control in controls3)
            {
                var ii = control.Tag.ToString();
                tempL[int.Parse(ii)] = double.Parse(control.Text);
            }
            controls3 = panel_baseR.Controls.OfType<TextBox>();

            foreach (var control in controls3)
            {
                var ii = control.Tag.ToString();
                tempR[int.Parse(ii)] = double.Parse(control.Text);
            }
          
            for (int i = 0; i < 4; i++)
                for (int y = 0; y < 4; y++)
                {
                    leftArmWorldFrame[i, y] = tempL[index];
                    rightArmWorldFrame[i, y] = tempR[index];
                    index++;
                }
            //Конец Сохраняем левую и правую базу вращения из TextBox в переменные leftArmWorldFrame и rightArmWorldFrame


            var leftArm = new SerialLink(leftArmWorldFrame);
            var rightArm = new SerialLink(rightArmWorldFrame);

            // Заполняем данные DH из DHControl
            var controls = panel_Left.Controls.OfType<DHControl>();
            
            if (controls.Count() == 0) {
                leftArm.AddJoint(0, 0, 0, 0, 0, 0, "None", 0, "Revol");// Если манипулятор пустой, создаем одну техническую запись
              
            }
            index = 0;
            double QCurrent = 0;
            foreach (var control in controls)
            {
                
                var A = Convert.ToDouble(control.Value[0]);

                var D = Convert.ToDouble(control.Value[1]);
                var Alpha = Convert.ToDouble(control.Value[2]);
                var Theta = Convert.ToDouble(control.Value[3]);
                var LowerLimit = Convert.ToDouble(control.Value[4]);
                var UpperLimit = Convert.ToDouble(control.Value[5]);
                var Name = control.Value[6];
                //if ((Convert.ToDouble(control.Value[7]) >= LowerLimit) && (Convert.ToDouble(control.Value[7]) <= UpperLimit))
                //{
                //    QCurrent = Convert.ToDouble((control.Value[7]));
                //}
                //else
                //{
                //    QCurrent = LowerLimit;
                //}

                if (Convert.ToDouble(control.Value[7]) < LowerLimit)
                    QCurrent = LowerLimit;
                else if (Convert.ToDouble(control.Value[7]) > UpperLimit)
                    QCurrent = UpperLimit;
                else QCurrent = Convert.ToDouble(control.Value[7]);

                var TypeJoint = control.Value[8];





                //               a, d, alpha, theta, lowerLimit ,  upperLimit , string name ,  QCurrent, "Revol"
                leftArm.AddJoint(A, D, Alpha, Theta, LowerLimit, UpperLimit, Name, QCurrent, TypeJoint);
                index++;
            }

            controls = panel_Right.Controls.OfType<DHControl>();
            index = 0;
            if (controls.Count() == 0)
            {
                rightArm.AddJoint(0, 0, 0, 0, 0, 0, "None", 0, "Revol");// Если манипулятор пустой, создаем одну техническую запись
            }
            
            foreach (var control in controls)
            {
                var A = Convert.ToDouble(control.Value[0]);
                var D = Convert.ToDouble(control.Value[1]);
                var Alpha = Convert.ToDouble(control.Value[2]);
                var Theta = Convert.ToDouble(control.Value[3]);
                var LowerLimit = Convert.ToDouble(control.Value[4]);
                var UpperLimit = Convert.ToDouble(control.Value[5]);
                var Name = control.Value[6];
                //if ((Convert.ToDouble(control.Value[7]) >= LowerLimit) && (Convert.ToDouble(control.Value[7]) <= UpperLimit))
                //{
                //    QCurrent = Convert.ToDouble((control.Value[7]));
                //}
                //else
                //{
                //    QCurrent = LowerLimit;
                //}

                if (Convert.ToDouble(control.Value[7]) < LowerLimit)
                    QCurrent = LowerLimit;
                else if (Convert.ToDouble(control.Value[7]) > UpperLimit)
                    QCurrent = UpperLimit;
                else QCurrent = Convert.ToDouble(control.Value[7]);


                var TypeJoint = control.Value[8];

                
                //              a,     d, alpha, theta, lowerLimit ,  upperLimit , string name ,  false, "Revol"
                rightArm.AddJoint(A, D, Alpha, Theta, LowerLimit, UpperLimit, Name, QCurrent, TypeJoint);
                index++;
            }

            flowLayoutPanel1.Visible = true;
            flowLayoutPanel3.Visible = true;
            pictureBox2.Visible = true;
            button2_Leftsbros.Visible = true;
            button_Rightsbros.Visible = true;
            panelLeft.Visible = true;
            panel1.Visible = true;
            
            panel_Full.Visible = false;

            var IP = textBox_IP.Text;

            var port = textBox_Port.Text;
            Data.robot = new Robot(leftArm, rightArm, IP, Convert.ToInt32(port));
            loadRobot();
        }

        private void button_SaveFile_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = saveFileDialog1.FileName;
            int index = 0;
            foreach (var jointL in Data.robot.LeftArm.DenavitHartenberg.Joints)
            {               
                jointL.QCurrent = qL[index];
                index++;
            }
            index = 0;

            
            foreach (var jointR in Data.robot.RightArm.DenavitHartenberg.Joints)
            {
                jointR.QCurrent = qR[index];
                index++;
            }

            Data.robot.SaveToFile(filename);
        }

        private void button_OpenFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = openFileDialog1.FileName;
            Data.robot = Robot.LoadFromFile(filename);
            loadRobot();
            if (panel_Full.Visible)
            {

                button_EditRobot_Click(null, null);
            }
        }

        private void button_AddL_Click(object sender, EventArgs e)
        {
            var Tag_end = 0;
               var controls = panel_Left.Controls.OfType<DHControl>();
            foreach (var control2 in controls)
            {
                Tag_end= int.Parse(control2.Tag.ToString());
            }
                
          

                var control = new DHControl("0", "0", "0", "0", "0", "0", "Name", "0", "Revol", (Tag_end + 1).ToString());

                control.Tag = Tag_end+1;
                control.Dock = DockStyle.Bottom;
                panel_Left.Controls.Add(control);
               

            
        }

        private void button_AddR_Click(object sender, EventArgs e)
        {
            var Tag_end = 0;
            var controls = panel_Right.Controls.OfType<DHControl>();
            foreach (var control2 in controls)
            {
                Tag_end = int.Parse(control2.Tag.ToString());
            }



            var control = new DHControl("0", "0", "0", "0", "0", "0", "Name", "0", "Revol", (Tag_end+1).ToString());

            control.Tag = Tag_end+1;
            control.Dock = DockStyle.Bottom;
            panel_Right.Controls.Add(control);


        }

        private void button_RemoveR_Click(object sender, EventArgs e)
        {
            var Tag_end = 0;
            var controls = panel_Right.Controls.OfType<DHControl>();
            foreach (var control2 in controls)
            {
                Tag_end = int.Parse(control2.Tag.ToString());

            }
            foreach (var control2 in controls)
            {
                if (int.Parse(control2.Tag.ToString()) == Tag_end)
                { 
               
                control2.Dispose();
                  }
            }



        }

        private void buttonRemoveLZveno_Click(object sender, EventArgs e)
        {
            var Tag_end = 0;
            var controls = panel_Left.Controls.OfType<DHControl>();
            foreach (var control2 in controls)
            {
                Tag_end = int.Parse(control2.Tag.ToString());

            }
            foreach (var control2 in controls)
            {
                if (int.Parse(control2.Tag.ToString()) == Tag_end)
                {
                    control2.Dispose();
                }
            }
        }

        private void button_CreateRobot_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel3.Visible = false;
            pictureBox2.Visible = false;
            button2_Leftsbros.Visible = false;
            button_Rightsbros.Visible = false;
            panelLeft.Visible = false;
            panel1.Visible = false;
            button_SaveRobot.Visible = true;

            panel_Full.Visible = true;          

            panel_Left.Controls.Clear();            
            panel_Right.Controls.Clear();

            var Tag_end = 0;
            var controls = panel_Right.Controls.OfType<DHControl>();
            foreach (var control2 in controls)
            {
                Tag_end = int.Parse(control2.Tag.ToString());
            }



            var control = new DHControl("0", "0", "0", "0", "0", "0", "Name", "0", "Revol", "1");

            control.Tag = Tag_end + 1;
            control.Dock = DockStyle.Bottom;
            panel_Right.Controls.Add(control);

             Tag_end = 0;
             controls = panel_Left.Controls.OfType<DHControl>();
            foreach (var control2 in controls)
            {
                Tag_end = int.Parse(control2.Tag.ToString());
            }



             control = new DHControl("0", "0", "0", "0", "0", "0", "Name", "0", "Revol","1");

            control.Tag = Tag_end + 1;
            control.Dock = DockStyle.Bottom;
            panel_Left.Controls.Add(control);
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Visible = true;
            flowLayoutPanel3.Visible = true;
            pictureBox2.Visible = true;
            button2_Leftsbros.Visible = true;
            button_Rightsbros.Visible = true;
            panelLeft.Visible = true;
            panel1.Visible = true;

            panel_Full.Visible = false;
        }

        private void button_Rightsbros_Click(object sender, EventArgs e)
        {
            size = tbSize.Value;
            pitch = (float)(tbPitch.Value * Math.PI / 180);
            roll = (float)(tbRoll.Value * Math.PI / 180);
            yaw = (float)(tbYaw.Value * Math.PI / 180);

            for (int i = 0; i < qR.Length; i++)
            {
                qR[i] = 0;
                
            }

            var controls = flowLayoutPanel3.Controls.OfType<JointControl>();
            var index = 0;
            foreach (var control in controls)
            {
                if (control.Minimum > 0)
                {
                    if (TypeJoinR[index] == "Revol")
                    {
                        control.Value = control.Minimum;
                        qR[index] = Utils.DegreesToRadians(control.Minimum);
                    }
                    else
                    {
                        control.Value = control.Minimum;
                        qR[index] = control.Minimum / 100;

                    }
                    

                }
                else
                {
                    if (TypeJoinR[index] == "Prizm")
                    {
                        control.Value = qR[index];
                    }
                    else
                    {
                        control.Value = qR[index];

                    }
                    

                }

                

                index++;
            }
            var positionL = Data.robot.LeftArm.FK(qL);
            var positionR = Data.robot.RightArm.FK(qR);
            CreateCube(size, position, yaw, pitch, roll, positionL, positionR, baseL, baseR);

            pictureBox2.Invalidate();
            Invalidate();
        }

        private void textBox_BaseR00_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
        (e.KeyChar != '.') &&        (e.KeyChar != '-'))
            {
                e.Handled = true;
            }
        }

        private void button_ServerStart_Click(object sender, EventArgs e)
        {
            Application.DoEvents();
            // Запуск в работу
            server.Start();
            string readSmg = "Ожидание клиента";
            


            // Бесконечный цикл

            while (true)
            {
               Application.DoEvents();
                try
                {
                    // Подключение клиента
                    if (!server.Pending())
                    {
                     //   Application.DoEvents();
                        label_Server.Text = readSmg;
                        // Console.WriteLine(DateTime.MinValue);
                    }
                    else
                    {
                        Application.DoEvents();
                        TcpClient client = server.AcceptTcpClient();
                        NetworkStream stream = client.GetStream();
                        // Обмен данными
                        try
                        {
                           Application.DoEvents();
                            if (stream.CanRead)
                            {
                                string otvet = "Ok";
                                string GO="I";
                                byte[] myReadBuffer = new byte[1024];
                                StringBuilder myCompleteMessage = new StringBuilder();
                                int numberOfBytesRead = 0;
                                do
                                {
                                    numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                                    myCompleteMessage.AppendFormat("{0}", Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead));
                                    readSmg = Encoding.Default.GetString(myReadBuffer, 0, numberOfBytesRead);
                                    Application.DoEvents();
                                   // Console.Write(readSmg); // выводим на экран полученное сообщение в виде строки
                                    label_Server.Text = readSmg;
                                    ;
                                    if (readSmg.Substring(0, 2) == "L;")
                                    {
                                        GO = "L";
                                        readSmg = readSmg.Substring(2);
                                        
                                    }
                                    else {
                                        if (readSmg.Substring(0, 2) == "R;")
                                        {
                                            GO = "R";
                                            readSmg = readSmg.Substring(2);
                                            
                                        }
                                    }

                                    string[] words = readSmg.Split(';');
                                    //Convert.ToDouble
                                    System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                                    double[] doubleArray = words.Select<string, double>(s => Double.Parse(s)).ToArray<double>();

                                    int index = 0;
                                    int rez = 0;
                                    if (GO == "L")
                                    {
                                        if (doubleArray.Length != Data.robot.LeftArm.DenavitHartenberg.Joints.Count)
                                        {
                                            otvet = "The string does not match";
                                            rez = 1;
                                        }
                                        else
                                            foreach (var joint in Data.robot.LeftArm.DenavitHartenberg.Joints)
                                            {
                                                if (doubleArray[index] < joint.LowerLimit || doubleArray[index] > joint.UpperLimit)
                                                {
                                                    otvet = "Limin out";
                                                    rez = 1;
                                                }
                                                index++;
                                            }
                                        if (rez == 0)
                                        {
                                            var qL_old = qL; 
                                            qL = doubleArray;

                                            koefPodgotovka(qL_old, "L");

                                            
                                            var positionL = Data.robot.LeftArm.FK(qL);
                                            var positionR = Data.robot.RightArm.FK(qR);
                                            textBoxLeftX.Text = positionL.EndEffectorX.ToString("F3");
                                            textBoxLeftY.Text = positionL.EndEffectorY.ToString("F3");
                                            textBoxLeftZ.Text = positionL.EndEffectorZ.ToString("F3");

                                            textBoxLeftRoll.Text = Utils.RadiansToDegrees(positionL.Roll).ToString("F3");
                                            textBoxLeftPich.Text = Utils.RadiansToDegrees(positionL.Pitch).ToString("F3");
                                            textBoxLeftYaw.Text = Utils.RadiansToDegrees(positionL.Yaw).ToString("F3");
                                            
                                        }
                                    }
                                    if (GO == "R")
                                    {
                                        if (doubleArray.Length != Data.robot.RightArm.DenavitHartenberg.Joints.Count)
                                        {
                                            otvet = "The string does not match";
                                            rez = 1;
                                        }
                                        else
                                            foreach (var joint in Data.robot.RightArm.DenavitHartenberg.Joints)
                                            {
                                                if (doubleArray[index] < joint.LowerLimit || doubleArray[index] > joint.UpperLimit)
                                                {
                                                    otvet = "Limin out";
                                                    rez = 1;
                                                }
                                                index++;
                                            }
                                        if (rez == 0)
                                        {


                                            var qR_old = qR;
                                            qR = doubleArray;

                                            koefPodgotovka(qR_old, "R");

                                              
                                           
                                            var positionL = Data.robot.LeftArm.FK(qL);
                                            var positionR = Data.robot.RightArm.FK(qR);
                                            textBoxRightX.Text = positionR.EndEffectorX.ToString("F3");
                                            textBoxRightY.Text = positionR.EndEffectorY.ToString("F3");
                                            textBoxRightZ.Text = positionR.EndEffectorZ.ToString("F3");

                                            textBoxRightRoll.Text = Utils.RadiansToDegrees(positionR.Roll).ToString("F3");
                                            textBoxRightPich.Text = Utils.RadiansToDegrees(positionR.Pitch).ToString("F3");
                                            textBoxRightYaw.Text = Utils.RadiansToDegrees(positionR.Yaw).ToString("F3");
                                            
                                        }
                                    }

                                }
                                while (stream.DataAvailable);
                                //Byte But = (byte)255;


                                Byte[] responseData = Encoding.UTF8.GetBytes(otvet);
                                stream.Write(responseData, 0, responseData.Length);

                            }

                        }
                        catch 
                        {
                            Application.DoEvents();

                            while (stream.DataAvailable) ;
                            //Byte But = (byte)255;


                            Byte[] responseData = Encoding.UTF8.GetBytes("error");
                            stream.Write(responseData, 0, responseData.Length);

                        }
                        finally
                        {
                            Application.DoEvents();


                            stream.Close();
                            client.Close();
                            //server.Start();

                            ////заполняем текущими значениями JointControl
                            //var controls = flowLayoutPanel3.Controls.OfType<JointControl>();// выбираем все элементы JointControl на панели flowLayoutPanel3
                            //var index = 0;
                            //foreach (var control in controls)
                            //{
                            //    control.Value = Utils.RadiansToDegrees(qR[index]); // заменяем Value 
                            //    index++;
                            //}

                            //controls = flowLayoutPanel1.Controls.OfType<JointControl>();// выбираем все элементы JointControl на панели flowLayoutPanel3
                            //index = 0;
                            //foreach (var control in controls)
                            //{
                            //    control.Value = Utils.RadiansToDegrees(qL[index]); // заменяем Value 
                            //    index++;
                            //}

                        }
                    }
                }
                catch
                {
                    Application.DoEvents();
                    server.Stop();
                    break;
                }
            }
        }

        private void buttonServerStop_Click(object sender, EventArgs e)
        {
            server.Stop();
        }

        private void isColidedCheckbox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            // Console.WriteLine(e.X);
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = 1;
            firstX = e.X;
            firstY = e.X;
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
           if (mouseDown == 1){ 
            var SecX = tbRoll.Value + (firstX - e.X);
            var SecY = tbPitch.Value  + firstY + e.Y;
            //    tbRoll.Value= SecX;
            // tbRoll.Value;
            //Console.WriteLine("Roll " + tbRoll.Value);
            //Console.WriteLine("firstX " + firstX);
            //Console.WriteLine("e.X " + e.X);
            //    Console.WriteLine("SecX " + SecX); 

            size = tbSize.Value;
            roll = (float)(SecX * Math.PI / 180);
            pitch = (float)(tbPitch.Value * Math.PI / 180);
            yaw = (float)(tbYaw.Value * Math.PI / 180);


            var positionL = Data.robot.LeftArm.FK(qL);
            var positionR = Data.robot.RightArm.FK(qR);
            CreateCube(size, position, yaw, pitch, roll, positionL, positionR, baseL, baseR);

            pictureBox2.Invalidate();
            Invalidate();
            }
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown=0;
                }

        private void pictureBox3_Paint(object sender, PaintEventArgs e)
        {
            //создаем матрицу проекции на плоскость XY
          //  var paneXY = new Class2.Matrix4x4() { V00 = 1f, V11 = -1f, V33 = 1f };
           // var paneXZ = new Class2.Matrix4x4() { V00 = 1f, V12 = -1f, V33 = 1f };
            //рисуем
           // DrawRobot(e.Graphics, new PointF(30, 30), paneXY);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pLEnd2.Clear();
            pLEnd.Clear();
            pREnd2.Clear();
            pREnd.Clear();
            pictureBox2.Invalidate();

            Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            
            
        }
    }
    static class Data
    {
        public static string[] arStr { get; set; }
        public static Robot robotForm2 { get; set; }
        public static Robot robot { get; set; }

    }
}

   



