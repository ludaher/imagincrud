using ImaginCrud.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImaginCrud.Models
{
    public class PriorityModel
    {
        public PriorityModel()
        {
            Files = new List<HttpPostedFileBase>();
        }
        public List<HttpPostedFileBase> Files { get; set; }
        public int FormId { get; set; }
        public string FileName { get; set; }
        public ProcessPriorities Priority { get; set; }
        public int SuccessProcesses { get; set; }
        public int ErrorProcesses { get; set; }

        public string ErrorMessages { get; set; }
    }
}