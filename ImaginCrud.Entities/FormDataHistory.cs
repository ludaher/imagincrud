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
    /// Captura de un proceso
    /// </summary>
    public class FormDataHistory : BaseEntity
    {
        /// <summary>
        /// Identificador de la captura del proceso
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long FormDataId { get; set; }
        /// <summary>
        /// Proceso al que se le realiza la captura
        /// </summary>
        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string TypingProcessId { get; set; }
        /// <summary>
        /// Formulario o producto al que pertenece
        /// </summary>
        [Required]
        public int FormId { get; set; }
        /// <summary>
        /// Proceso al que se le realiza la captura
        /// </summary>
        [ForeignKey("TypingProcessId,FormId")]
        public TypingProcess TypingProcess { get; set; }
        /// <summary>
        /// Tipo de captura
        /// </summary>
        [Required]
        public int RegisterType { get; set; }
        /// <summary>
        /// Total de secciones capturadas
        /// </summary>
        [Required]
        public int CompletedSections { get; set; }
        /// <summary>
        /// Indicador de captura completada
        /// </summary>
        public bool Completed { get; set; }
        /// <summary>
        /// Detalle de captra (Valores de los campos)
        /// </summary>
        public List<FormDataHistoryDetail> FormDetails { get; set; }

    }
}
