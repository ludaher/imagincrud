using ImaginCrud.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImaginCrud.Models
{
    public class AssignUsersModel: SearchModel<AssignUser>
    {
        public Form Form { get; set; }
    }

    public class AssignUser: User
    {
        public int FormId { get; set; }
        public bool IsTypist { get; set; }
        public bool IsValidator { get; set; }
    }
}