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
            Database.DefaultConnectionFactory = new System.Data.Entity.Infrastructure.SqlConnectionFactory();
            this.Database.Initialize(false);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
        }
        public DbSet<District> District { get; set; }
        public DbSet<MPOfRoad> MPOfRoad { get; set; }
        public DbSet<MPOfResidence> MPOfResidence { get; set; }
        public DbSet<MPOfCountry> MPOfCountry { get; set; }
        public DbSet<SysUser> SysUser { get; set; }
        public DbSet<SysRole> SysRole { get; set; }
        public DbSet<SysPrivilige> SysPrivilige { get; set; }
        public DbSet<SysUser_SysRole> UserRole { get; set; }
        public DbSet<SysUser_District> UserDistrict { get; set; }
        public DbSet<SysRole_SysPrivilige> RolePrivilige { get; set; }
        public DbSet<MPOfCertificate> MPOfCertificate { get; set; }
        public DbSet<MPProduceTJ> MPProduce { get; set; }
        public DbSet<MPOfUploadFiles> MPOfUploadFiles { get; set; }
        public DbSet<RP> RP { get; set; }
        public DbSet<RPRepair> RPRepair { get; set; }
        public DbSet<RPOfUploadFiles> RPOfUploadFiles { get; set; }
        public DbSet<DMBZDic> DMBZDic { get; set; }
        public DbSet<PostcodeDic> PostcodeDic { get; set; }
        public DbSet<RPBZDic> RPBZDic { get; set; }
        public DbSet<RoadDic> RoadDic { get; set; }
        public DbSet<ResidenceDic> ResidenceDic { get; set; }
        public DbSet<ViligeDic> ViligeDic { get; set; }
        public DbSet<CommunityDic> CommunityDic { get; set; }
        public DbSet<RPPepairUploadFiles> RPPepairUploadFiles { get; set; }
        public DbSet<DirectionDic> DirectionDic { get; set; }
        public DbSet<ARCHIVEFILE> ArchiveFile { get; set; }
        public DbSet<SystemLog> SystemLog { get; set; }
    }
}