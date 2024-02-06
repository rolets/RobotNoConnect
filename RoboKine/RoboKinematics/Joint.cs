using System.Xml.Serialization;

namespace RoboKinematics
{
    [XmlRoot(Namespace = "", ElementName = "Joint")]
    public class Joint
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public double A { get; set; }
        [XmlAttribute]
        public double D { get; set; }
        [XmlAttribute]
        public double Alpha { get; set; }
        [XmlAttribute]
        public double Theta { get; set; }
        [XmlAttribute]
        public double LowerLimit { get; set; }
        [XmlAttribute]
        public double UpperLimit { get; set; }
        [XmlAttribute]
        public double QCurrent { get; set; }
        [XmlAttribute]
        public string TypeJoint { get; set; }


        public Joint(double a, double d, double alpha, double theta, double lowerLimit, double upperLimit, string name, double qCurrent, string typeJoint)
        {
            A = a;
            D = d;
            Alpha = alpha;
            Theta = theta;
            LowerLimit = lowerLimit;
            UpperLimit = upperLimit;
            Name = name;
            QCurrent = qCurrent;
            TypeJoint = typeJoint;
        }

        internal Joint() : this(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, string.Empty, double.NaN, string.Empty)
        {
        }
    }
}
