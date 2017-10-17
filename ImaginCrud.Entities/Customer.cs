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
    /// Clientes de la aplicación
    /// Un cliente puede tener diferentes productos registrados en el sistema
    /// </summary>
    public class Customer : BaseEntity
    {
        /// <summary>
        /// Identificador del cliente
        /// </summary>
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CustomerId { get; set; }
        /// <summary>
        /// Nombre del cliente
        /// </summary>
        [Required]
        [Column(TypeName = "varchar")][StringLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// Dirección del cliente
        /// </summary>
        [Column(TypeName = "varchar")][StringLength(50)]
        public string Address { get; set; }
        /// <summary>
        /// Teléfono del cliente
        /// </summary>
        [Column(TypeName = "varchar")][StringLength(50)]
        public string Phone { get; set; }
        /// <summary>
        /// Descripción del cliente
        /// </summary>
        [Column(TypeName = "varchar")][StringLength(150)]
        public string Description { get; set; }
    }
}
