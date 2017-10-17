using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.Entities
{
    /// <summary>
    /// Datos de paginación de una consulta
    /// </summary>
    public class Pagining
    {
        #region paginación
        /// <summary>
        /// Página de la consulta, 0 es todas
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Elementos por página
        /// </summary>
        public int ItemsByPage { get; set; }
        /// <summary>
        /// Total de elementos de la consulta
        /// </summary>
        public int TotalItems { get; set; }
        /// <summary>
        /// Columna por la que se ordena la columna
        /// </summary>
        public string SortBy { get; set; }
        /// <summary>
        /// Indica si se ordena descendientemente
        /// </summary>
        public bool IsDescendentOrder { get; set; }
        #endregion
    }
}
