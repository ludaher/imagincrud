using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.Entities
{
    /// <summary>
    /// Elemento de la fuente de datos para campos de selección (drop down)
    /// </summary>
    public class FieldDataSourceDetail
    {
        /// <summary>
        /// Identificador del detalle
        /// </summary>
        [Key]
        public int FieldDataSourceDetailId { get; set; }
        /// <summary>
        /// Fuente de datos
        /// </summary>
        public int FieldDataSourceId { get; set; }
        /// <summary>
        /// Fuente de datos
        /// </summary>
        [ForeignKey("FieldDataSourceId")]
        public FieldDataSource FormData { get; set; }
        /// <summary>
        /// Valor del elemento
        /// </summary>
        [Column(TypeName = "varchar")]
        [StringLength(150)]
        public string Value { get; set; }
        /// <summary>
        /// Texto del elemento
        /// </summary>
        [Column(TypeName = "varchar")]
        [StringLength(250)]
        public string Label { get; set; }

    }
}
