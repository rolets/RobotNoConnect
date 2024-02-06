using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace robot_ver5
{
    public partial class Form_NewRobot : Form
    {
       
        public Form_NewRobot()
        {
            InitializeComponent();
            
        }

        private void Form_NewRobot_Load(object sender, EventArgs e)
        {

            
            var counter = 0;
            foreach (var jointR in Data.robot.RightArm.DenavitHartenberg.Joints)
            {


                var control = new DHControl(jointR.A.ToString(), jointR.D.ToString(), jointR.Alpha.ToString(), jointR.Theta.ToString(), jointR.LowerLimit.ToString(), jointR.UpperLimit.ToString(), jointR.Name, jointR.QCurrent.ToString(), jointR.TypeJoint, counter + 1.ToString());

                control.Tag = counter;
                control.Dock = DockStyle.Bottom;
                panel_Right.Controls.Add(control);
                counter++;

            }

            counter = 0;
            foreach (var jointL in Data.robot.LeftArm.DenavitHartenberg.Joints)
            {


                var control = new DHControl(jointL.A.ToString(), jointL.D.ToString(), jointL.Alpha.ToString(), jointL.Theta.ToString(), jointL.LowerLimit.ToString(), jointL.UpperLimit.ToString(), jointL.Name, jointL.QCurrent.ToString(), jointL.TypeJoint, counter+1.ToString());

                control.Tag = counter;
                control.Dock = DockStyle.Bottom;
                panel_Left.Controls.Add(control);
                counter++;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var controls = panel_Right.Controls.OfType<DHControl>();
            var index = 0;
            foreach (var control in controls)
            {
                //Console.WriteLine(control.A_Val);
                Data.robot.RightArm.DenavitHartenberg.Joints[index].A = Convert.ToDouble(control.Value[0]);
                Data.robot.RightArm.DenavitHartenberg.Joints[index].D = Convert.ToDouble(control.Value[1]);
                Data.robot.RightArm.DenavitHartenberg.Joints[index].Alpha = Convert.ToDouble(control.Value[2]);
                Data.robot.RightArm.DenavitHartenberg.Joints[index].Theta = Convert.ToDouble(control.Value[3]);
                Data.robot.RightArm.DenavitHartenberg.Joints[index].LowerLimit = Convert.ToDouble(control.Value[4]);
                Data.robot.RightArm.DenavitHartenberg.Joints[index].UpperLimit = Convert.ToDouble(control.Value[5]);
                Data.robot.RightArm.DenavitHartenberg.Joints[index].Name = control.Value[6];
                ////Data.robotForm2.RightArm.DenavitHartenberg.Joints[index].RotationDirection = int.Parse(control.A_Val);
                //Data.robotForm2.RightArm.DenavitHartenberg.Joints[index].TypeJoint = control.A_Val;
                index++;
            }
             controls = panel_Left.Controls.OfType<DHControl>();
             index = 0;
            foreach (var control in controls)
            {
                //Console.WriteLine(control.A_Val);
                
                Data.robot.LeftArm.DenavitHartenberg.Joints[index].A = Convert.ToDouble(control.Value[0]);
                Data.robot.LeftArm.DenavitHartenberg.Joints[index].D = Convert.ToDouble(control.Value[1]);
                Data.robot.LeftArm.DenavitHartenberg.Joints[index].Alpha = Convert.ToDouble(control.Value[2]);
                Data.robot.LeftArm.DenavitHartenberg.Joints[index].Theta = Convert.ToDouble(control.Value[3]);
                Data.robot.LeftArm.DenavitHartenberg.Joints[index].LowerLimit = Convert.ToDouble(control.Value[4]);
                Data.robot.LeftArm.DenavitHartenberg.Joints[index].UpperLimit = Convert.ToDouble(control.Value[5]);
                Data.robot.LeftArm.DenavitHartenberg.Joints[index].Name = control.Value[6];
                ////Data.robotForm2.RightArm.DenavitHartenberg.Joints[index].RotationDirection = int.Parse(control.A_Val);
                //Data.robotForm2.RightArm.DenavitHartenberg.Joints[index].TypeJoint = control.A_Val;
                index++;
            }


            //form1.zagruzra0();
            //Form1.zagruzra();
            //Form1 form1 = new Form1();
      


        }

        private void button2_Click(object sender, EventArgs e)
        {
           
        }
    }

}
