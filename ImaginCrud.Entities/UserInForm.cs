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
    /// Permisos de usuarios en los formularios
    /// </summary>
    public class UserInForm
    {
        /// <summary>
        /// Identificador del permiso
        /// </summary>
        [Key]
        [Column(Order = 10)]
        public int UserFunction { get; set; }
        /// <summary>
        /// Nombre del usuario
        /// </summary>
        [Key]
        [Column(Order = 20, TypeName = "varchar")]
        [StringLength(50)]
        public string UserName { get; set; }
        /// <summary>
        /// Formulario al que se le asigna el usuario
        /// </summary>
        [Key]
        [Column(Order = 30)]
        public int FormId { get; set; }
        [ForeignKey("FormId")]
        public Form Form { get; set; }

    }
}
