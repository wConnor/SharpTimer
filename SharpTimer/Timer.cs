using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTimer
{
    internal class Timer
    {
        public Timer(int hours, int minutes, int seconds)
        {
            _hours = hours;
            _minutes = minutes;
            _seconds = seconds;
        }

        public int _hours { get; set; }
        public int _minutes { get; set; }
        public int _seconds { get; set; }


    }
}
