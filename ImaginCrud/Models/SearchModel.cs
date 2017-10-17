using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImaginCrud.Models
{
    public class SearchModel<T>
    {
        public T EntityToFind { get; set; }
        public string SortOrder { get; set; }
        public bool Descendant { get; set; }
        public IEnumerable<T> ListData { get; set; }
        public int Page { get; set; }
        public int ItemsByPage { get; set; }
        public int TotalPages { get; set; }

    }
}