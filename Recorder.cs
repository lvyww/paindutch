using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace TypeB
{
    static internal class Recorder
    {
        public enum RecorderState
        {
            Stopped = 0,
            Recording = 1,
            Playing = 2
        }
        public struct RecItem
        {
            public int key;
            public long time;
            public int keystate;
            public int modifier;
 
        }

        static public  RecorderState State = RecorderState.Stopped;
        static Stopwatch Rs = new Stopwatch();
        static public List<RecItem> RecItems = new List< RecItem> ();

        static public void Start()
        {
            if (State !=Recorder.RecorderState.Playing)
            {
                Rs.Start();
                State = RecorderState.Recording;
            }
 
        }
        static public void Reset()
        {
            if (State != Recorder.RecorderState.Playing)
            {
                RecItems.Clear();
                Rs.Reset();
   //             Rs.Stop();
                State = Recorder.RecorderState.Stopped;
            }
        }

        static public void Stop ()
        {
            if (State != Recorder.RecorderState.Playing)
            {
                Rs.Stop();
                State = Recorder.RecorderState.Stopped;
            }

        }

        

    }


}
