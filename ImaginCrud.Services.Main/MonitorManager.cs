using ImaginCrud.Services.Main.Monitors;
using ImaginCrud.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImaginCrud.Services.Main
{
    public class MonitorManager
    {
        private static string FTP_PROCESS_INTERVAL = "FTP_PROCESS_INTERVAL_SECONDS";
        private static string FORM_STATUS_PROCESS_INTERVAL = "FORM_STATUS_PROCESS_INTERVAL_SECONDS";

        private FtpMonitor _ftpMonitor;
        private StatusMonitor _statusMonitor;
        public MonitorManager()
        {
            _ftpMonitor = new FtpMonitor();
            _statusMonitor = new StatusMonitor();
        }

        public void StartMonitor()
        {
            AppLogger.Logger.Info("Inicio de monitoreo");
            var ftpInterval = Convert.ToInt32(ConfigurationManager.AppSettings[FTP_PROCESS_INTERVAL]);
            var formStatusInterval = Convert.ToInt32(ConfigurationManager.AppSettings[FORM_STATUS_PROCESS_INTERVAL]);
            _ftpMonitor.Start(ftpInterval);
            _statusMonitor.Start(formStatusInterval);
        }
    }
}
