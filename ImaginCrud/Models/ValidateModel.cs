using ImaginCrud.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImaginCrud.Models
{
    public class ValidateModel
    {
        public string TypingProcessId { get; set; }
        public int FormId { get; set; }
        public long FormDataId { get; set; }
        public short CompletedSections { get; set; }
        public List<SectionModel> Sections { get; set; }
        public List<FormDataDetail> FormDetails
        {
            get
            {
                var detail = new List<FormDataDetail>();
                foreach (var section in Sections)
                {
                    foreach (var field in section.Fields)
                    {
                        detail.Add(new FormDataDetail()
                        {
                            Value = field.Value == null ? null : field.Value.ToString(),
                            FieldId = field.FieldId,
                            FormDataId = FormDataId
                        });
                    }
                }
                return detail;
            }
        }
        public void SetData(List<Section> sections, FormData formData, FormData capturedData1, FormData capturedData2)
        {
            Sections = sections.Select(x => AutoMapper.Mapper.Map<SectionModel>(x)).ToList();
            foreach (var section in Sections)
            {
                foreach (var field in section.Fields)
                {
                    var formDetail1 = capturedData1.FormDetails.FirstOrDefault(x => x.FieldId.Equals(field.FieldId));
                    var formDetail2 = capturedData2 == null ? (field.DobleCapture ? null : formDetail1) : capturedData2.FormDetails.FirstOrDefault(x => x.FieldId.Equals(field.FieldId));
                    field.ComparisionError = !_AreEquals(formDetail1, formDetail2);
                    var formDetail = formData == null ? null : formData.FormDetails.FirstOrDefault(x => x.FieldId.Equals(field.FieldId));
                    field.Value1 = formDetail1 == null ? null : formDetail1.Value;
                    field.Value2 = formDetail2 == null ? null : formDetail2.Value;
                    if (formDetail == null)
                    {
                        if (field.ComparisionError == false)
                            field.Value = formDetail1.Value;
                        continue;
                    }
                    field.Value = formDetail.Value;
                }
            }
            if (formData == null)
                return;
            FormDataId = formData.FormDataId;
        }

        private bool _AreEquals(FormDataDetail formDetail1, FormDataDetail formDetail2)
        {
            if (formDetail1 == null || formDetail2 == null)
                return false;
            if (string.IsNullOrWhiteSpace(formDetail1.Value) && string.IsNullOrWhiteSpace(formDetail2.Value))
                return true;
            if ((string.IsNullOrWhiteSpace(formDetail1.Value) && string.IsNullOrWhiteSpace(formDetail2.Value) == false)
                || (string.IsNullOrWhiteSpace(formDetail1.Value) == false && string.IsNullOrWhiteSpace(formDetail2.Value)))
                return false;
            if (formDetail1.Value.Trim().ToUpper().Equals(formDetail2.Value.Trim().ToUpper()) == false)
                return false;
            return true;
        }
    }
}
