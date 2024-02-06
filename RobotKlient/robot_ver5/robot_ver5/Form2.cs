using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace robot_ver5
{
    
    public partial class Form_Setting : Form
    { 
        public Form_Setting()
        {
            InitializeComponent();
        }

        private void Form_Setting_Load(object sender, EventArgs e)
        {
           
            
                string s = Data.arStr[2];
                if (s == "1") 
                        {
                          checkBox_showName.Checked=true;
                        }
                        else 
                        { 
                            checkBox_showName.Checked=false; 
                        }
             s = Data.arStr[3];
            if (s == "1")
            {
                checkBox1.Checked = true;
            }
            else
            {
                checkBox1.Checked = false;
            }
            textBox1.Text=Data.arStr[4];
            textBox2.Text=Data.arStr[5];
            s = Data.arStr[6];
            if (s == "1")
            {
                checkBox2.Checked = true;
            }
            else
            {
                checkBox2.Checked = false;
            }

        }

        private void button1_Save_Click(object sender, EventArgs e)
        {
            
            if (checkBox_showName.Checked)
                {
                    Data.arStr[2] = "1";
                }
                else
                {
                    Data.arStr[2] = "0";
                }
            if (checkBox1.Checked)
            {
                Data.arStr[3] = "1";
            }
            else
            {
                Data.arStr[3] = "0";
            }

            Data.arStr[4] = textBox1.Text;
            Data.arStr[5] = textBox2.Text;
            if (checkBox2.Checked)
            {
                Data.arStr[6] = "1";
            }
            else
            {
                Data.arStr[6] = "0";
            }
            File.WriteAllLines("setting.txt", Data.arStr, Encoding.Default);
            Console.WriteLine(Data.robot.LeftArm.DenavitHartenberg.Joints.Count());
            //Data.robot.SaveToFile("settings222.xml");
            this.Close();


        }
    }
}
