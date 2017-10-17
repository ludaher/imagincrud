using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.Entities
{
    public class Form:BaseEntity
    {
        /// <summary>
        /// Identificador del formulario o producto
        /// </summary>
        [Key]
        public int FormId { get; set; }
        /// <summary>
        /// Idennificador del cliente
        /// </summary>
        [Required]
        public int CustomerId { get; set; }
        /// <summary>
        /// Cliente
        /// </summary>
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        /// <summary>
        /// Nombre del formulario o producto
        /// </summary>
        [Required]
        [Column(TypeName = "varchar")][StringLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// Descripción del formulario o producto
        /// </summary>
        [Column(TypeName = "varchar")][StringLength(150)]
        public string Description { get; set; }
        /// <summary>
        /// Indicador de producto activo
        /// </summary>
        [Required]
        public bool Active { get; set; }
        /// <summary>
        /// Ruta en la que se encuentra el archivo base para el diseño del formualrio o producto
        /// </summary>
        [Required]
        [Column(TypeName = "varchar")][StringLength(150)]
        public string TemplatePath { get; set; }
        /// <summary>
        /// Alto máximo del archivo base
        /// </summary>
        [Required]
        public int TemplateHeight { get; set; }
        /// <summary>
        /// Estado del formulario o producto
        /// </summary>
        [Required]
        public int ProductStatus { get; set; }
        /// <summary>
        /// Número de capturas requeridas (1-2)
        /// </summary>
        [Required]
        [DefaultValue(2)]
        public short RequiredCaptures { get; set; }
        /// <summary>
        /// Total de procesos actuales
        /// </summary>
        [NotMapped]
        public int TotalProcesses { get; set; }
        /// <summary>
        /// Total de procesos pendientes
        /// </summary>
        [NotMapped]
        public int ProcessesPending { get; set; }
        /// <summary>
        /// Total de procesos en captura
        /// </summary>
        [NotMapped]
        public int ProcessesInCapture { get; set; }
        /// <summary>
        /// Total de procesos capturados
        /// </summary>
        [NotMapped]
        public int ProcessesCaptured { get; set; }
        /// <summary>
        /// Total de procesos que se están validadno
        /// </summary>
        [NotMapped]
        public int ProcessesInValidate { get; set; }
        /// <summary>
        /// Total de procesos validados
        /// </summary>
        [NotMapped]
        public int ProcessesValidated { get; set; }

        [NotMapped]
        private const string CREATE_TABLE = ""
           + "IF  NOT EXISTS "
           + "(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Form_{0}]') AND type in (N'U')) "
           + "BEGIN "
           + "  CREATE TABLE Form_{0} "
           + "      (FormId int NOT NULL, "
           + "      TypingProcessId varchar(50), "
           + "      FormDataId int, "
           + "      ModifiedBy varchar(20), "
           + "      ModifiedOn DateTime, "
           + "      PRIMARY KEY (FormDataId)); "
           + "END";

        public string GetQueryToCreateTable()
        {
            var query = string.Format(CREATE_TABLE, this.FormId);
            return query;
        }

        
    }
}
