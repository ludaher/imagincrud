using ImaginCrud.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImaginCrud.Models
{
    public class SearchFormsModel : SearchModel<Form>
    {
        public List<Customer> Customers { get; set; }
    }
}