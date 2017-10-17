using ImaginCrud.Services.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.Service.Service
{
    class Program
    {

        static void Main(string[] args)
        {
            new MonitorManager().StartMonitor();
            Console.ReadKey();
        }


    }
}
