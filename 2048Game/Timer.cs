using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace _2048Game
{
    class Timer: DispatcherTimer
    {
        private int _Time;

        public string GetTime
        {
            get
            {
                int iHour = _Time / 3600;
                int iMin = (_Time % 3600) / 60;
                int iSec = (_Time % 60);

                return String.Format("{0}:{1}:{2}",
                    iHour, iMin >= 10 ? iMin.ToString() : "0" + iMin.ToString(),
                    iSec >= 10 ? iSec.ToString() : "0" + iSec.ToString());
            }
        }

        public Timer(MainWindow mainWindow)
        {
            Interval = new TimeSpan(0, 0, 1);
            Tick += (s, e) =>
            {
                _Time += 1;
                mainWindow.UpdateTimer();
            };
        }

        public new void Start()
        {
            _Time = 0;
            base.Start();
        }

        public void Resume()
        {
            base.Start();
        }
    }
}
