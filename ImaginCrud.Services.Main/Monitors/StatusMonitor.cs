using ImaginCrud.Entities;
using ImaginCrud.Logic;
using ImaginCrud.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.Services.Main.Monitors
{
    public class StatusMonitor : MonitorBase
    {
        private static string MAX_SECONDS_IN_CAPTURE = "MAX_SECONDS_IN_CAPTURE";
        private static string MAX_SECONDS_IN_VALIDATION = "MAX_SECONDS_IN_VALIDATION";

        private int _secondsInCapture;
        private int _secondsInValidation;
        public StatusMonitor()
        {
            _secondsInCapture = Convert.ToInt32(ConfigurationManager.AppSettings[MAX_SECONDS_IN_CAPTURE]);
            _secondsInValidation = Convert.ToInt32(ConfigurationManager.AppSettings[MAX_SECONDS_IN_VALIDATION]);
        }

        protected override void Run(object obj)
        {
            var state = (StateObjClass)obj;
            try
            {
                if (state.IsRunning) return;
                _CheckStatus(ProcessStatus.InCapture, ProcessStatus.Pending, _secondsInCapture);
                _CheckStatus(ProcessStatus.Validating, ProcessStatus.Captured, _secondsInValidation);
            }
            catch (Exception ex)
            {
                AppLogger.Logger.Error(ex.ToString());
            }
            state.IsRunning = false;
        }

        private void _CheckStatus(ProcessStatus actualStatus, ProcessStatus newStatus, int timeInSeconds)
        {

            var logicProcess = new TypingProcessLogic();
            var processes = logicProcess.FindWithParameters(null, new TypingProcess()
            {
                TypingStatus = (int)actualStatus
            });
            foreach (var process in processes)
            {
                if (process.ModifiedOn.HasValue == false || process.ModifiedOn.Value.AddSeconds(timeInSeconds) < DateTime.Now)
                {
                    process.ModifiedBy = Environment.UserName;
                    logicProcess.ChangeState(process
                        , newStatus
                        , string.Format("Cambio automático por tiempo de inactividad el {0} de status {1} a {2}", DateTime.Now, actualStatus.Description(), newStatus.Description()));
                }
            }
        }

    }
}
