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
    /// Historico de estados de procesos
    /// </summary>
    public class ProductStatusHistory
    {
        [Key]
        public int ProductStatusHistoryId { get; set; }
        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string TypingProcessId { get; set; }
        [Required]
        public int FormId { get; set; }
        [ForeignKey("TypingProcessId,FormId")]
        public TypingProcess TypingProcess { get; set; }
        [Required]
        public int TypingStatus { get; set; }
        [NotMapped]
        public string TypingStatusDescription { get { return ((ProcessStatus)TypingStatus).Description(); } }
        [Required]
        public DateTime ModifiedOn { get; set; }
        [NotMapped]
        public string ModifiedOnString
        {
            get
            {
                return ModifiedOn.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }
        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string ModifiedBy { get; set; }
        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(250)]
        public string Observations { get; set; }
    }
}
