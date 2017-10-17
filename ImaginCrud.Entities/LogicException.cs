using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.Entities
{
    public class LogicException : Exception
    {
        /// <summary>
        /// Excepción que permite enviar mensajes de error entre capas
        /// </summary>
        /// <param name="message"></param>
        public LogicException(string message) : base(message)
        {

        }
        public LogicException(string message, Exception ex) : base(message, ex)
        {

        }
    }
}
