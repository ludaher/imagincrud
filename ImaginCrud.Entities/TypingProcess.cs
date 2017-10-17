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
    /// Procesos a los que se les realiza la captura
    /// </summary>
    public class TypingProcess : BaseEntity
    {
        /// <summary>
        /// Identificador de procesos 
        /// </summary>
        [Key]
        [Column(Order = 10, TypeName = "varchar")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(50)]
        public string TypingProcessId { get; set; }
        /// <summary>
        /// Estado del proceso
        /// </summary>
        [Required]
        public int TypingStatus { get; set; }
        /// <summary>
        /// Formulario al que pertenece el proceso de captura
        /// </summary>
        [ForeignKey("FormId")]
        public Form Form { get; set; }
        /// <summary>
        /// Formulario al que pertenece el proceso de captura
        /// </summary>
        [Key]
        [Column(Order = 20)]
        public int FormId { get; set; }
        /// <summary>
        /// Observaciones del proceso
        /// </summary>
        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(250)]
        public string Observations { get; set; }
        /// <summary>
        /// prioridad del proceso
        /// </summary>
        [Required]
        public int Priority { get; set; }
        /// <summary>
        /// Fecha en la que se carga el proceso
        /// </summary>
        [Required]
        public DateTime? ProductionDate{ get; set; }
        /// <summary>
        /// Capturas realizadas del proceso.
        /// </summary>
        [NotMapped]
        public int CapturedRows { get; set; }
        /// <summary>
        /// Lista para filtrar registros por campo
        /// </summary>
        [NotMapped]
        public List<FormDataDetail> FieldsToSearch { get; set; }

    }
}
