using System;
using System.Threading.Tasks;
using RoboKinematics;


namespace RoboMain
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var leftArmWorldFrame = new double[,] { { 0, 0, -1, -0.2 }, { 0, 1, 0, 0 }, { 1, 0, 0, 0 }, { 0, 0, 0, 1 } };
            var rightArmWorldFrame = new double[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };

            var leftArm = new SerialLink(leftArmWorldFrame);
            var rightArm = new SerialLink(rightArmWorldFrame);


            leftArm.AddJoint(-0.2, 0.2, -1.5708, 0, -1.5708, 0.1571, "L.ShoulderF");
            leftArm.AddJoint(0, 0, -1.5708, -1.5708, 0, 2.0944, "L.ShoulderS");
            leftArm.AddJoint(0, -0.47, -1.5708, -1.5708, -1.5708, 1.5708, "L.ElbowR");
            leftArm.AddJoint(0, 0, -1.5708, 0, -2.0944, 0.0873, "L.Elbow");
            leftArm.AddJoint(0, 0.2, 1.5708, 0, -1.3963, 1.3963, "L.WristR");
            leftArm.AddJoint(0.5, 0, 0, 1.5708, -1.5708, 1.57, "L.WristS");

            rightArm.AddJoint(-0.2, -0.2, -1.5708, 0, -1.5708, 0.1571, "R.ShoulderF");
            rightArm.AddJoint(0, 0, -1.5708, -1.5708, -2.0944, 0, "R.ShoulderS");
            rightArm.AddJoint(0, -0.47, -1.5708, -1.5708, -1.5708, 1.5708, "R.ElbowR");
            rightArm.AddJoint(0, 0, -1.5708, 0, -2.0944, 0.0873, "R.Elbow");
            rightArm.AddJoint(0, 0.2, 1.5708, 0, -1.3963, 1.3963, "R.WristR");
            rightArm.AddJoint(0.2, 0, 0, 1.5708, -1.5708, 1.57, "R.WristS");

            Robot robot = new Robot(leftArm, rightArm, "127.0.0.1", 10099);
            robot.SaveToFile("settings.xml");


            var q0 = Utils.DegreesToRadians(0, 0, 0, 0, 0, 0);
            var translation = new[] { -0.16, 0.65, -0.15 };
            var rotation = Utils.DegreesToRadians(-11.48, -18.66, -20.84);

            var armPosition = robot.LeftArm.IK(translation, rotation, q0);    // Решаем обратную задачу кинематики
            Console.WriteLine("Solve IK: {0}", string.Join(", ", armPosition.MotorAngles));

            armPosition = robot.LeftArm.FK(armPosition.MotorAngles);

            Console.WriteLine("Original RPY: {0}", string.Join(", ", rotation));
            Console.WriteLine("FK RPY: {0}, {1}, {2}", armPosition.Roll, armPosition.Pitch, armPosition.Yaw);
            var trajectory = robot.LeftArm.JointTrajectory(q0, armPosition.MotorAngles, 15);

            Console.WriteLine("Нажмите любую кнопку что бы вернутся в исходную позицию");
            Console.ReadKey(true);

            await robot.Connect();                                                          // Подключаемся к роботу
            robot.LeftArm.SetJointAngles(Utils.DegreesToRadians(0, 0, 0, 0, 0, 0));   // Задаем углы двигателей в градусах
             
            Console.WriteLine("Нажмите любую кнопку что бы запустить движение по траектории");
            Console.ReadKey(true);
            
            robot.LeftArm.RunJointTrajectory(trajectory);
            // или robot.LeftArm.RunJointTrajectory(q0, currentMotorAngle, 15); где 15 это колличество шагов

            Console.WriteLine("Нажмите любую кнопку что бы получить текущее положение двигатеелей");
            Console.ReadKey(true);

            
            var leftArmPosition = await robot.LeftArm.GetPosition();
            var currentMotorAngle = leftArmPosition.MotorAngles;                     // Получаем положение двигателей но не перемещаем манипулятор
            Console.WriteLine("Robot Joints: " + string.Join(", ", Utils.RadiansToDegrees(currentMotorAngle))); // Выводим значение углов в градусах
            Console.WriteLine("\tX = {0}, Y = {1}, Z = {2}", leftArmPosition.EndEffectorX, leftArmPosition.EndEffectorY,
                leftArmPosition.EndEffectorZ);

            Console.ReadKey(true);
            leftArmPosition = await robot.LeftArm.MoveTo(translation);        // Перемещаем левую руку в координаты

            Console.WriteLine("MoveTo: " + string.Join(", ", leftArmPosition.MotorAngles));   // На выходе получаем положение двигателей

            var position = robot.LeftArm.FK(currentMotorAngle);                  // Решаем прямую задачу кинематики
            Console.WriteLine("X: " + position.EndEffectorX);
            Console.WriteLine("Y: " + position.EndEffectorY);
            Console.WriteLine("Z: " + position.EndEffectorZ);
            Console.WriteLine("Нажмите любую кнопку...");
            Console.ReadKey(true);
        }
    }
}
