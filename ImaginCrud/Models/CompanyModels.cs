using ImaginCrud.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImaginCrud.Models
{
    public class FormModel: SearchModel<Form>
    {
        public Customer Customer { get; set; }
        public List<Customer> Customers { get; set; }
    }
}
