using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImaginCrud.Models
{
    public class ClientModel
    {
        /// <summary>
        /// Nombre de la empresa cliente
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Identificación del cliente
        /// </summary>
        public string Identification { get; set; }
        /// <summary>
        /// Tipo de identificación del cliente
        /// </summary>
        public string TypeOfIdentification { get; set; }
        /// <summary>
        /// Descripción de la empresa
        /// </summary>
        public string Description { get; set; } 
    }
}
