using ImaginCrud.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImaginCrud.Models
{
    public class AssignedFormModel
    {
        public Form Form { get; set; }
        public int PendingCount { get; set; }
        public int CompletedCount { get; set; }
        public List<TypingProcess> TypingProcesses { get; set; }

    }
}