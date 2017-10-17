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
    /// Fuentes de datos para campos de selección (drop down)
    /// </summary>
    public class FieldDataSource : BaseEntity
    {
        [Key]
        public int FieldDataSourceId { get; set; }
        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(150)]
        public string Description { get; set; }

        public List<FieldDataSourceDetail> FieldDataSourceDetails { get; set; }

    }
}
