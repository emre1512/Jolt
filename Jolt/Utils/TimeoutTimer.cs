using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JoltHttp.Utils
{
    public class TimeoutTimer
    {

        private int timeout;

        public TimeoutTimer(int timeoutValue)
        {
            timeout = timeoutValue;
            Thread thread1 = new Thread(new ThreadStart(StartTimer));
        }

        public void StartTimer()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.Elapsed.TotalSeconds < timeout);

            stopwatch.Stop();

        }

    }
}
