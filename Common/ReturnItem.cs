using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ReturnItem<T>
    {
        public ReturnItem()
        {
            Costtime = "0";
            TimerStart();//启动计时
        }
        public int Code { get; set; }

        public string Msg { get; set; }
        public string mac { get; set; }

        public T Data { get; set; }

        public string Costtime { get; set; }
        Stopwatch watch = null;
        public void TimerStart()
        {
            watch = new Stopwatch();
            watch.Reset();
            watch.Start();
        }
        public void TimerEnd()
        {
            watch.Stop();
            double elapsedMilliseconds = watch.ElapsedMilliseconds;
            Costtime = elapsedMilliseconds.ToString();
        }
        public int Count { get; set; }
    }
}
