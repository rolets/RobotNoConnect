using System;
using System.Runtime.InteropServices;

namespace robot_ver5
{    
    class ClassJoy
    {
        [DllImport("winmm.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 joyGetPosEx(Int32 uJoyID, ref JOYINFOEX pji);
        public struct JOYINFOEX
        {
            public Int32 dwSize;
            public Int32 dwFlags;
            public Int32 dwXpos;
            public Int32 dwYpos;
            public Int32 dwZpos;
            public Int32 dwRpos;
            public Int32 dwUpos;
            public Int32 dwVpos;
            public Int32 dwButtons;
            public Int32 dwButtonNumber;
            public Int32 dwPOV;
            public Int32 dwReserved1;
            public Int32 dwReserved2;
        }

        public static Int32 JOY_RETURNX = 0x00000001;
        public static Int32 JOY_RETURNY = 0x00000002;
        public static Int32 JOY_RETURNZ = 0x00000004;
        public static Int32 JOY_RETURNR = 0x00000008;
        public static Int32 JOY_RETURNU = 0x00000010;
        public static Int32 JOY_RETURNV = 0x00000020;
        public static Int32 JOY_RETURNPOV = 0x00000040;
        public static Int32 JOY_RETURNBUTTONS = 0x00000080;
        public static Int32 JOY_RETURNALL = (JOY_RETURNX | JOY_RETURNY | JOY_RETURNZ | JOY_RETURNR | JOY_RETURNU | JOY_RETURNV | JOY_RETURNPOV | JOY_RETURNBUTTONS);
    }
}
