using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImaginCrud.Util;

namespace ImaginCrud.Entities
{
    /// <summary>
    /// Campos de formularios
    /// </summary>
    public class Field : BaseEntity
    {
        /// <summary>
        /// Identificador del campo
        /// </summary>
        [Key]
        public int FieldId { get; set; }
        /// <summary>
        /// Sección a la que pertenece el campo
        /// </summary>
        [Required]
        public int SectionId { get; set; }
        /// <summary>
        /// Sección a la que pertenece el campo
        /// </summary>
        [ForeignKey("SectionId")]
        public Section Section { get; set; }
        /// <summary>
        /// Nombre del campo
        /// </summary>
        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string FieldName { get; set; }
        /// <summary>
        /// Texto que se muestra en el campo
        /// </summary>
        [Required]
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string Title { get; set; }
        /// <summary>
        /// Valor por defecto del campo
        /// </summary>
        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Tipo de campo (Ver enumeración)
        /// </summary>
        [Required]
        public int FieldTypeId { get; set; }
        /// <summary>
        /// Indicador de campo obligatorio
        /// </summary>
        [Required]
        public bool Required { get; set; }
        /// <summary>
        /// Indica si al campo se le debe aplicar doble captura
        /// </summary>
        [Required]
        public bool DobleCapture { get; set; }
        /// <summary>
        /// Posibles opciones del campo separadas por coma
        /// </summary>
        [Column(TypeName = "varchar")]
        [StringLength(250)]
        public string Options { get; set; }
        /// <summary>
        /// Campo del que depende
        /// </summary>
        public int? ParentFieldId { get; set; }
        /// <summary>
        /// Campo del que depende
        /// </summary>
        [ForeignKey("ParentFieldId")]
        public Field ParentField { get; set; }
        /// <summary>
        /// Validación que aplica al campo, para textos (email, URL)
        /// </summary>
        [Column(TypeName = "varchar")]
        [StringLength(100)]
        public string Validation { get; set; }
        /// <summary>
        /// Tamaño del campo dentro del formulario (sección), un número del 1 al 12 para bootstrap
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// Tamaño máximo del campo
        /// </summary>
        public int MaxLength { get; set; }
        /// <summary>
        /// Orden en el que se muestra el campo en el formulario (sección)
        /// </summary>
        [Required]
        public int OrderInForm { get; set; }
        /// <summary>
        /// Tipo de campo de selección
        /// </summary>
        [NotMapped]
        public int DataList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Options) == false && Options.StartsWith("#"))
                {
                    return Convert.ToInt32(Options.Replace("#", ""));
                }
                return (int)DefaultDataLists.Custom;
            }
        }
        /// <summary>
        /// Lista de posibles opciones del campo
        /// </summary>
        [NotMapped]
        public List<Option> OptionList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Options) || Options.StartsWith("#"))
                    return null;
                var list = Options.Split(';')
                    .Select(x => new Option() { value = x.Split(':')[0], label = x.Split(':')[1] })
                    .ToList();
                return list;
            }
        }
        [NotMapped]
        public long MinNumber
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Validation)) return 0;
                var substring = Validation.GetStringInBetween("ng-min = '", "' ng-max = '", false, false);
                if (string.IsNullOrWhiteSpace(substring)) return 0;
                return Convert.ToInt64(substring);
            }
        }

        [NotMapped]
        public long MaxNumber
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Validation)) return 0;
                var substring = Validation.GetStringInBetween("' ng-max = '", "' ", false, false);
                if (string.IsNullOrWhiteSpace(substring)) return 0;
                return Convert.ToInt64(substring);
            }
        }


        [NotMapped]
        private const string CREATE_COLUMN_IN_FORM = ""
           + "IF COL_LENGTH('[dbo].[Form_{0}]', '{1}') IS NULL "
           + "  BEGIN "
           + "      ALTER TABLE [dbo].[Form_{0}] "
           + "         ADD {1} {2}"
           + " END "
           + "ELSE "
           + "  BEGIN "
           + "      ALTER TABLE [dbo].[Form_{0}] "
           + "         ALTER COLUMN {1} {2}"
           + " END;";
        private const string CREATE_COLUMN_IN_SECTION = ""
           + "IF COL_LENGTH('[dbo].[Form_{0}_S_{3}]', '{1}') IS NULL "
           + "  BEGIN "
           + "      ALTER TABLE [dbo].[Form_{0}_S_{3}] "
           + "         ADD {1} {2}"
           + " END "
             + "ELSE "
           + "  BEGIN "
           + "      ALTER TABLE [dbo].[Form_{0}_S_{3}] "
           + "         ALTER COLUMN {1} {2}"
           + " END;";

        public string GetQueryToCreateColumnInForm(int formId)
        {
            var query = string.Format(CREATE_COLUMN_IN_FORM, formId, this.FieldName, _GetColumnDetailToCreate());
            return query;
        }

        public string GetQueryToCreateColumnInSection(int formId)
        {
            var query = string.Format(CREATE_COLUMN_IN_SECTION, formId, this.FieldName, _GetColumnDetailToCreate(), SectionId);
            return query;
        }


        public string GetQueryToUpdateColumnNameInForm(int formId, string oldColumnName)
        {
            var query = string.Format("IF COL_LENGTH('[dbo].[Form_{0}]', '{1}') IS NOT NULL BEGIN EXEC sp_RENAME '[dbo].[Form_{0}].{1}', '{2}' , 'COLUMN' END", formId, oldColumnName, FieldName);
            return query;
        }

        public string GetQueryToUpdateColumnNameInSection(int formId, string oldColumnName)
        {
            var query = string.Format("IF COL_LENGTH('[dbo].[Form_{0}_S_{3}]', '{1}') IS NOT NULL BEGIN EXEC sp_RENAME '[dbo].[Form_{0}_S_{3}].{1}', '{2}' , 'COLUMN' END", formId, oldColumnName, FieldName,SectionId);
            return query;
        }

        private string _GetColumnDetailToCreate()
        {
            string detail = string.Empty;
            switch (this.FieldTypeId)
            {
                case (int)FieldTypes.Number:
                    {
                        if (MaxNumber > 0)
                        {
                            var digits = Math.Floor(Math.Log10(MaxNumber) + 1);
                            detail = $" decimal({digits + 2},2) ";
                        }
                        else
                            detail = " decimal(18,0) ";
                        break;
                    }
                case (int)FieldTypes.Checkbox:
                    {
                        detail = " bit ";
                        break;
                    }
                case (int)FieldTypes.DateTime:
                    {
                        detail = " datetime ";
                        break;
                    }
                case (int)FieldTypes.Select:
                    {
                        detail = " varchar(100) ";
                        break;
                    }
                case (int)FieldTypes.Text:
                    {
                        detail = $" varchar({MaxLength}) ";
                        break;
                    }

                default:
                    {
                        detail = " varchar(max) ";
                        break;
                    }
            }
            if (Required == true)
                detail += " not null ";
            else
                detail += " null ";
            return detail;
        }

    }
    /// <summary>
    /// Opción del campo
    /// </summary>
    public class Option
    {
        public string label { get; set; }
        public string value { get; set; }
    }
}
