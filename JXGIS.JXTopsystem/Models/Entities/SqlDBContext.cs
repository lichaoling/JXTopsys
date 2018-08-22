using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    public class SqlDBContext : DbContext
    {
        public static string conStr = System.Configuration.ConfigurationManager.ConnectionStrings["TopSystemEntities"].ToString();
        public SqlDBContext() : base(conStr)
        {
            this.Database.Initialize(false);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
        }
        public DbSet<District> District { get; set; }
        //public DbSet<Road> Road { get; set; }
        public DbSet<MPOfRoad> MPOfRoad { get; set; }
        public DbSet<MPOfResidence> MPOFResidence { get; set; }
        public DbSet<MPOfCountry> MPOfCountry { get; set; }
        public DbSet<SysUser> SysUser { get; set; }
        public DbSet<SysRole> SysRole { get; set; }
        public DbSet<SysUser_SysRole> UserRole { get; set; }
        public DbSet<MPOfCertificate> MPOfCertificate { get; set; }
        public DbSet<MPProduce> MPProduce { get; set; }
        public DbSet<MPOfUploadFiles> MPOfUploadFiles { get; set; }
        public DbSet<RP> RP { get; set; }
        public DbSet<RPRepair> RPRepair { get; set; }
        public DbSet<RPOfUploadFiles> RPOfUploadFiles { get; set; }
        public DbSet<DMBZDic> DMBZDic { get; set; }

    }
}