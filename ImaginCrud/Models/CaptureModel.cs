using ImaginCrud.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImaginCrud.Models
{
    public class CaptureModel
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
        public void SetData(List<Section> sections, FormData formData, FormData capturedData1)
        {
            Sections = sections.Select(x => AutoMapper.Mapper.Map<SectionModel>(x)).ToList();
            if (formData != null)
            {
                FormDataId = formData.FormDataId;

            }
            foreach (var section in Sections)
            {
                foreach (var field in section.Fields)
                {
                    var formDetail = formData == null ? null : formData.FormDetails.FirstOrDefault(x => x.FieldId.Equals(field.FieldId));

                    if (field.DobleCapture == false)
                    {
                        if (capturedData1 == null)
                        {
                            if (formDetail != null)
                                field.Value = formDetail.Value;
                            continue;
                        }
                        var formDetailCapture1 = capturedData1.FormDetails.FirstOrDefault(x => x.FieldId.Equals(field.FieldId));
                        if (formDetailCapture1 != null)
                            field.Value = formDetailCapture1.Value;
                        continue;
                    }
                    if (formDetail == null)
                        continue;
                    field.Value = formDetail.Value;
                    continue;
                }
            }
        }
    }
}
