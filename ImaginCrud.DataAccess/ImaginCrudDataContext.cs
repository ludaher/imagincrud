using ImaginCrud.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImaginCrud.DataAccess
{
    public class ImaginCrudDataContext : DbContext
    {
        public ImaginCrudDataContext() : base(System.Configuration.ConfigurationManager.
                ConnectionStrings["ImaginCrudConnection"].ConnectionString)
        {
            
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormData> FormData { get; set; }
        public DbSet<FormDataDetail> FormDataDetails { get; set; }
        public DbSet<FormDataHistory> FormDataHistories { get; set; }
        public DbSet<FormDataHistoryDetail> FormDataHistoryDetails { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<UserInForm> UsersInForms { get; set; }
        public DbSet<TypingProcess> TypingProcesss { get; set; }
        public DbSet<ProductStatusHistory> ProductStatusHistory { get; set; }
        public DbSet<FieldDataSource> FieldDataSources { get; set; }
        public DbSet<FieldDataSourceDetail> FieldDataSourceDetails { get; set; }
    }
}
