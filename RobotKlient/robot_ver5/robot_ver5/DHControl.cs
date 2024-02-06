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
    public partial class DHControl : UserControl
    {
        // private double _value = 0;

        //private string _A;

       // public event EventHandler<EventArgs> ValueChanged;

        [Browsable(true)]
        public string[] Value
        {
            get { return new string[] { textBox_A.Text, textBox_D.Text, textBox_alpha.Text, textBox_theta.Text, textBox_lowerLimit.Text, textBox_upperLimit.Text, textBox_stringName.Text, textBox_Current.Text, comboBox_Type.Text};  }
            set
            {            }
        }
        private string _a="5";
        public string A 
        
        {
            get { return _a; }
            set
            { _a = value; }

        }


        public DHControl (string A, string D, string alpha, string theta, string lowerLimit, string upperLimit, string stringName, string qcur, string Revol,string nomer)
        {
            InitializeComponent();

            textBox_A.Text = A;
            textBox_D.Text = D;
            textBox_alpha.Text = alpha;
            textBox_theta.Text = theta;
            textBox_lowerLimit.Text = lowerLimit;
            textBox_upperLimit.Text = upperLimit;
            textBox_stringName.Text = stringName;
            //if ((Convert.ToDouble(qcur) >= Convert.ToDouble(lowerLimit)) && (Convert.ToDouble(qcur) <= Convert.ToDouble(upperLimit)))
            //{
            //    textBox_Current.Text = qcur;
            //}
            //else {
            //    textBox_Current.Text = "0";
            //}
            if (Convert.ToDouble(qcur) < Convert.ToDouble(lowerLimit))
                textBox_Current.Text = lowerLimit;
            else if (Convert.ToDouble(qcur) > Convert.ToDouble(upperLimit))
                textBox_Current.Text = upperLimit;
            else textBox_Current.Text = qcur;

            comboBox_Type.Text = Revol;
            label_Namber.Text = nomer;

            
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void textBox_A_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
        (e.KeyChar != '.') && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }
        }


    }
}
