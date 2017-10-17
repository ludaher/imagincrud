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
    /// Sección del formulario o producto
    /// </summary>
    public class Section : BaseEntity
    {
        /// <summary>
        /// Identificador de la sección
        /// </summary>
        [Key]
        public int SectionId { get; set; }
        /// <summary>
        /// Formulario al que pertenece
        /// </summary>
        [ForeignKey("FormId")]
        public Form Form { get; set; }
        /// <summary>
        /// Formulario al que pertenece
        /// </summary>
        [Required]
        public int FormId { get; set; }
        /// <summary>
        /// Nombre de la sección
        /// </summary>
        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string SectionName { get; set; }
        /// <summary>
        /// Posición de la sección en el formulario
        /// </summary>
        [Required]
        public int Position { get; set; }
        /// <summary>
        /// Indicador de tabla, si es verdadero la sección se muestra como un grid
        /// </summary>
        [Required]
        public bool IsTable { get; set; }
        /// <summary>
        /// Numero de filas de la tabla, si el indicador de tabla es verdadero
        /// </summary>
        [Required]
        public int NumberOfRows { get; set; }
        /// <summary>
        /// Lista de campos de la sección
        /// </summary>
        public List<Field> Fields { get; set; }

        [NotMapped]
        private const string CREATE_TABLE = ""
           + "IF  NOT EXISTS "
           + "(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Form_{0}_S_{1}]') AND type in (N'U')) "
           + "BEGIN "
           + "  CREATE TABLE Form_{0}_S_{1} "
           + "      (ID int NOT NULL identity, "
           + "      FormDataId int, "
           + "      TypingProcessId varchar(50), "
           + "      PRIMARY KEY (ID)); "
           + "END";

        public string GetQueryToCreateTable()
        {
            var query = string.Format(CREATE_TABLE, this.FormId, this.SectionId);
            return query;
        }
    }
}
