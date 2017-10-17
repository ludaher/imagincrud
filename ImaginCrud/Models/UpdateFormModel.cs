using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ImaginCrud.Util;
using System.Web;
using ImaginCrud.Entities;
using ImaginCrud.Logic;

namespace ImaginCrud.Models
{
    public class UpdateFormModel
    {
        public int FormId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TemplatePath { get; set; }
        public bool Active { get; set; }
        public int CustomerId { get; set; }
        public int PdfHeight { get; set; }
        public List<FormSectionModel> Sections { get; set; }
    }
    public class FormSectionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PdfPosition { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }

    }
    public class SectionModel
    {
        public int SectionId { get; set; }
        public int FormId { get; set; }
        public string TypingProcessId { get; set; }
        public string SectionName { get; set; }
        public int Position { get; set; }
        public bool IsTable { get; set; }
        public int NumberOfRows { get; set; }
        public List<FieldModel> Fields { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }

    public class FieldModel
    {
        public int FieldId { get; set; }
        public int SectionId { get; set; }
        public string FieldName { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public bool ComparisionError { get; set; }
        public string DefaultValue { get; set; }
        public int FieldTypeId { get; set; }
        public bool Required { get; set; }
        public bool DobleCapture { get; set; }
        public string Options
        {
            get
            {
                if (DataList == (int)Entities.DefaultDataLists.Custom)
                {
                    if (OptionList == null) return string.Empty;
                    var stringOptions = OptionList.Select(x => string.Format("{0}:{1}", x.value, x.label));
                    return string.Join(";", stringOptions);
                }
                else
                {
                    //var fieldDataSource = new FieldDataSourceLogic().FindById(DataList);
                    //if(fieldDataSource==null)
                    return string.Format("#{0}", (int)DataList);
                    //var stringOptions = fieldDataSource.FieldDataSourceDetails.Select(x => string.Format("{0}:{1}", x.Value, x.Label));
                    //return string.Join(";", stringOptions);
                }
            }
        }
        public int? ParentFieldId { get; set; }
        public string Validation { get; set; }
        public int Size { get; set; }
        public int MaxLength { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOnString { get; set; }
        public string CreatedBy { get; set; }
        public int DataList { get; set; }
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
        private List<OptionModel> _OptionList;

        public List<OptionModel> OptionList
        {
            get
            {
                if (DataList == (int)Entities.DefaultDataLists.Custom)
                    return _OptionList;
                var fieldDataSource = new FieldDataSourceLogic().FindById(DataList);
                if (fieldDataSource == null)
                    return null;
                return fieldDataSource.FieldDataSourceDetails.Select(x => new OptionModel() { value = x.Value, label =  x.Label }).ToList();
            }
            set
            {
                _OptionList = value;
            }
        }
        //public List<string> MultiselectValues
        //{
        //    get
        //    {
        //        if (string.IsNullOrWhiteSpace(Value))
        //            return new List<string>();
        //        return Value.Split(',').ToList();
        //    }
        //    set
        //    {
        //        if (value == null)
        //            return;
        //        Value = string.Join(",", value);
        //    }
        //}
        public List<List<string>> MultipleRowMultiselectLabels
        { get { return MultipleRowMultiselectValues; } }
        public List<List<string>> MultipleRowMultiselectValues
        {
            get
            {
                if ((FieldTypeId != (int)FieldTypes.MultiSelect && FieldTypeId != (int)FieldTypes.Select))
                    return null;
                if (string.IsNullOrWhiteSpace(Value))
                    return new List<List<string>>();
                var multipleValue = Value.Split('|').ToList();
                return multipleValue.Select(x => x.Split(',').ToList()).ToList();
            }
            set
            {
                if ((FieldTypeId != (int)FieldTypes.MultiSelect && FieldTypeId != (int)FieldTypes.Select) || value == null)
                    return;
                var joinSelectValues = value.Select(x => string.Join(",", x)).ToList();
                Value = string.Join("|", joinSelectValues);
            }
        }
        public List<List<string>> MultipleRowMultiselectValues1
        {
            get
            {
                if ((FieldTypeId != (int)FieldTypes.MultiSelect && FieldTypeId != (int)FieldTypes.Select))
                    return null;
                if (string.IsNullOrWhiteSpace(Value1))
                    return new List<List<string>>();
                var multipleValue = Value1.Split('|').ToList();
                return multipleValue.Select(x => x.Split(',').ToList()).ToList();
            }
            set
            {
                if ((FieldTypeId != (int)FieldTypes.MultiSelect && FieldTypeId != (int)FieldTypes.Select) || value == null)
                    return;
                var joinSelectValues = value.Select(x => string.Join(",", x)).ToList();
                Value1 = string.Join("|", joinSelectValues);
            }
        }
        public List<List<string>> MultipleRowMultiselectValues2
        {
            get
            {
                if ((FieldTypeId != (int)FieldTypes.MultiSelect && FieldTypeId != (int)FieldTypes.Select))
                    return null;
                if (string.IsNullOrWhiteSpace(Value2))
                    return new List<List<string>>();
                var multipleValue = Value2.Split('|').ToList();
                return multipleValue.Select(x => x.Split(',').ToList()).ToList();
            }
            set
            {
                if ((FieldTypeId != (int)FieldTypes.MultiSelect && FieldTypeId != (int)FieldTypes.Select) || value == null)
                    return;
                var joinSelectValues = value.Select(x => string.Join(",", x)).ToList();
                Value2 = string.Join("|", joinSelectValues);
            }
        }
        public List<string> MultipleRowValue
        {
            get
            {
                if (FieldTypeId == (int)FieldTypes.MultiSelect || FieldTypeId == (int)FieldTypes.Select)
                    return null;
                if (string.IsNullOrWhiteSpace(Value))
                    return new List<string>();
                return Value.Split('|').ToList();
            }
            set
            {
                if (FieldTypeId == (int)FieldTypes.MultiSelect || FieldTypeId == (int)FieldTypes.Select || value == null)
                    return;
                Value = string.Join("|", value);
            }
        }
        public List<string> MultipleRowValue1
        {
            get
            {
                if (FieldTypeId == (int)FieldTypes.MultiSelect || FieldTypeId == (int)FieldTypes.Select)
                    return null;
                if (string.IsNullOrWhiteSpace(Value1))
                    return new List<string>();
                return Value1.Split('|').ToList();
            }
            set
            {
                if (FieldTypeId == (int)FieldTypes.MultiSelect || FieldTypeId == (int)FieldTypes.Select || value == null)
                    return;
                Value1 = string.Join("|", value);
            }
        }
        public List<string> MultipleRowValue2
        {
            get
            {
                if (FieldTypeId == (int)FieldTypes.MultiSelect || FieldTypeId == (int)FieldTypes.Select)
                    return null;
                if (string.IsNullOrWhiteSpace(Value2))
                    return new List<string>();
                return Value2.Split('|').ToList();
            }
            set
            {
                if (FieldTypeId == (int)FieldTypes.MultiSelect || FieldTypeId == (int)FieldTypes.Select || value == null)
                    return;
                Value2 = string.Join("|", value);
            }
        }
    }

    public class OptionModel
    {
        public string value { get; set; }
        public string label { get; set; }

    }
}