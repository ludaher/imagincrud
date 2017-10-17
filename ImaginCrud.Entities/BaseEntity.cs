using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace ImaginCrud.Entities
{
    /// <summary>
    /// Entidad base que permite que las entidades tengan propiedades en común
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Último usuario que modificó la entidad
        /// </summary>
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Fecha de última modificación de la entidad
        /// </summary>
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Fecha de de creación de la entidad
        /// </summary>
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string CreatedBy { get; set; }
        private DateTime _CreatedOn;
        /// <summary>
        /// Fecha de creación de la entidad
        /// </summary>
        public DateTime CreatedOn
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_CreatedOnString))
                    return _CreatedOn;
                return DateTime.ParseExact(_CreatedOnString, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            set
            {
                _CreatedOn = value;
            }
        }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        private string _CreatedOnString;
        /// <summary>
        /// Fecha de creación de la entidad como string
        /// </summary>
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string CreatedOnString
        {
            get
            {
                if (default(DateTime) != _CreatedOn)
                    return _CreatedOn.ToString("dd/MM/yyyy HH:mm:ss");
                return _CreatedOnString;
            }
            set
            {
                _CreatedOnString = value;
            }
        }
    }
}
