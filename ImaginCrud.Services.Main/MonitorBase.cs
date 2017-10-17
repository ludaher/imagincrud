using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImaginCrud.Services.Main
{
    public abstract class MonitorBase
    {        
        protected abstract void Run(object obj);
        private int _intervalInSeconds;
        private Thread _monitorThread ;
        
        private void _InitializeTimer()
        {
            System.Threading.TimerCallback TimerDelegate =
                new System.Threading.TimerCallback(Run);
            // Create a timer that calls a procedure every 2 seconds.
            // Note: There is no Start method; the timer starts running as soon as 
            // the instance is created.
            var state = new StateObjClass();
            state.IsRunning = false;
            System.Threading.Timer TimerItem =
                new Timer(TimerDelegate, state, TimeSpan.Zero, TimeSpan.FromSeconds(_intervalInSeconds));
            state.TimerReference = TimerItem;
        }
        public void Start(int intervalInSeconds)
        {
            _monitorThread = new Thread(_InitializeTimer);
            _intervalInSeconds = intervalInSeconds;
            _monitorThread.Start();
        }
    }
    public class StateObjClass
    {
        // Used to hold parameters for calls to TimerTask.
        public bool IsRunning { get; set; }
        public System.Threading.Timer TimerReference { get; set; }
        public bool TimerCanceled { get; set; }
    }
}
