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
    public class FormData : BaseEntity
    {
        /// <summary>
        /// Identificador de la captura del proceso
        /// </summary>
        [Key]
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
        public List<FormDataDetail> FormDetails { get; set; }


        public string GetQueryToAddaRowInForm( string columns, string values)
        {
            var query = $"DELETE FROM [ImaginCrud].[dbo].[Form_{FormId}] WHERE TypingProcessId = '{TypingProcessId}'; "
           + $" INSERT INTO[dbo].[Form_{FormId}]"
           + "           ([FormId]"
           + "           ,[TypingProcessId]"
           + "           ,[FormDataId]"
           + "           ,[ModifiedBy]"
           + "           ,[ModifiedOn]"
           + $"           ,{columns})"
           + "     VALUES"
           + $"           ('{FormId}'"
           + $"           ,'{TypingProcessId}'"
           + $"           ,'{FormDataId}'"
           + $"           ,'{ModifiedBy}'"
           + $"           ,'{((ModifiedOn.HasValue)?ModifiedOn.Value.ToString("yyyy-MM-dd"):DateTime.Now.ToString("yyyy-MM-dd"))}'"
           + $"           ,{values}); ";
            return query;
        }

        public string GetQueryToDeleteProcessInSection(int sectionId)
        {
            return $"DELETE FROM [ImaginCrud].[dbo].[Form_{FormId}_S_{sectionId}] WHERE TypingProcessId = '{TypingProcessId}'; ";

        }

        public string GetQueryToAddaRowInSection(int sectionId, string columns, string values)
        {
            var query = ""
           + $" INSERT INTO[dbo].[Form_{FormId}_S_{sectionId}]"
           + "           ([FormDataId]"
           + "           ,[TypingProcessId]"
           + $"           ,{columns})"
           + "     VALUES"
           + $"           ({FormDataId}"
           + $"           ,'{TypingProcessId}'"
           + $"           ,{values}); ";
            return query;
        }
    }
}
