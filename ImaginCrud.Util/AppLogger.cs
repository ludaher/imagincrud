using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.Util
{
    public static class AppLogger
    {
        private static ILog _logger;
        public static ILog Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = log4net.LogManager.GetLogger("ImaginCrud");
                }
                return _logger;
            }
        }
        
    }
}
