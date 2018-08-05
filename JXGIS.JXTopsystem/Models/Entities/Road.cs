using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("TopCombineRoad")]
    //道路暂时不按照用户所在政区进行显示，先全部显示
    public class Road
    {
        [Column("dmid")]
        public Guid RoadID { get; set; }
        [Column("biaozhunmingcheng")]
        public string RoadName { get; set; }
        [Column("QiDian")]
        public string StartPoint { get; set; }
        [Column("ZhiDian")]
        public string EndPoint { get; set; }
        [Column("KuanDu")]
        public decimal? Width { get; set; }
        [Column("ChangDu")]
        public decimal? Length { get; set; }
        [Column("GeoData")]
        public DbGeography RoadGeom { get; set; }
        [Column("leibiemingcheng")]
        public string Category { get; set; }
        [Column("TuJingZhengQu")]
        public string PassBy { get; set; }
        [Column("qhid")]
        public string CountyID { get; set; }

        [Column("xzid")]
        public string NeighborhoodsID { get; set; }

        [Column("dilishitimiaoshu")]
        public string Description { get; set; }
    }
}