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
    /// Detalle de la captura (Valor de los campos)
    /// </summary>
    public class FormDataDetail
    {
        /// <summary>
        /// Campo capturado
        /// </summary>
        [Key]
        [Column(Order = 10)]
        public int FieldId { get; set; }
        /// <summary>
        /// Captura a la que pertenece
        /// </summary>
        [Key]
        [Column(Order = 20)]
        public long FormDataId { get; set; }
        /// <summary>
        /// Captura a la que pertenece
        /// </summary>
        [ForeignKey("FormDataId")]
        public FormData FormData { get; set; }
        /// <summary>
        /// Valor capturado del campo
        /// </summary>
        [Column(TypeName = "varchar")]
        [StringLength(2000)]
        public string Value { get; set; }

    }
}
