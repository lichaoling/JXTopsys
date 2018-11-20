using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{

    public class OracleDBContext : DbContext
    {
        public static string conStr = System.Configuration.ConfigurationManager.ConnectionStrings["OracleDbContext"].ToString();
        public OracleDBContext() : base(conStr)
        {
            Database.DefaultConnectionFactory = new Oracle.ManagedDataAccess.EntityFramework.OracleConnectionFactory();
            this.Database.Initialize(false);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["OracleDbContext"].ToString();
            int indexOf = connectionString.IndexOf("USER ID", StringComparison.OrdinalIgnoreCase);
            string str = connectionString.Substring(indexOf);
            int startIndexOf = str.IndexOf("=", StringComparison.OrdinalIgnoreCase);
            int lastIndexOf = str.IndexOf(";", StringComparison.OrdinalIgnoreCase);
            string uid = str.Substring(startIndexOf + 1, lastIndexOf - startIndexOf - 1).Trim().ToUpper();
            modelBuilder.HasDefaultSchema(uid);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
        public DbSet<POI> POI { get; set; }

    }
}