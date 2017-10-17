using ImaginCrud.Services.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.Service.Tester
{
    class ImaginCrudService : ServiceBase
    {
        static void Main()
        {
            ServiceBase.Run(new ImaginCrudService());
        }

        public ImaginCrudService()
        {
            InitializeComponent();
            new MonitorManager().StartMonitor();
        }

        private void InitializeComponent()
        {
            // 
            // ImaginCrudService
            // 
            this.ServiceName = "Alcaze ImaginCrud";

        }
    }
}
