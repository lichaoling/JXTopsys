using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    public class POIImageUrl
    {
        public static string ThumbnailUrl = "http://www.jxmap.gov.cn/ZJDituWebService/Service.asmx/GetPoiImageName?path={0}";
        public static string ImageUrl = "http://www.jxmap.gov.cn/ZJDituWebService/Service.asmx/GetPoiImagePathIndex?path={0}&imageIndex={1}";
    }
    public class LngLat
    {
        public LngLat() { }
        public LngLat(double lng, double lat)
        {
            this.lat = lat;
            this.lng = lng;
        }
        public double lat { get; set; }
        public double lng { get; set; }
    }


    [Table("POI")]
    public class POI
    {

        [Key, Column("FEATUREGUID")]
        public string FEATUREGUID { get; set; }

        [Column("PAC")]
        public int PAC { get; set; }

        [Column("FCODE")]
        public string FCODE { get; set; }

        [Column("NAME")]
        public string NAME { get; set; }

        [Column("SHORTNAME")]
        public string SHORTNAME { get; set; }

        [Column("ALIASNAME")]
        public string ALIASNAME { get; set; }

        [Column("ADDRESS")]
        public string ADDRESS { get; set; }

        [Column("CENTERX")]
        public double CENTERX { get; set; }

        [Column("CENTERY")]
        public double CENTERY { get; set; }

        [Column("TYPE")]
        public string TYPE { get; set; }

        [Column("PHONE")]
        public string PHONE { get; set; }

        [Column("WEBSITE")]
        public string WEBSITE { get; set; }

        [Column("PHOTO")]
        public string PHOTO { get; set; }

        [Column("FSCALE")]
        public int FSCALE { get; set; }

        [Column("STYLENAME")]
        public string STYLENAME { get; set; }

        [NotMapped]
        public LngLat LNGLAT { get { return new LngLat(this.CENTERX, this.CENTERY); } }

        [NotMapped]
        public double X { get { return this.CENTERX; } }

        [NotMapped]
        public double Y { get { return this.CENTERY; } }

        [NotMapped]
        public string[] PHOTOS
        {
            get { return string.IsNullOrWhiteSpace(this.PHOTO) ? null : this.PHOTO.Split(','); }
        }

        [NotMapped]
        public object PHOTOURL
        {
            get
            {
                int photonum = -1;
                if (!string.IsNullOrWhiteSpace(this.PHOTO) && int.TryParse(this.PHOTO, out photonum))
                {
                    return new
                    {
                        thumbnail = string.Format(POIImageUrl.ThumbnailUrl, this.PHOTO),
                        img = string.Format(POIImageUrl.ImageUrl, this.PHOTO, 0)
                    };
                }
                return null;
            }
        }

        [NotMapped]
        public double DISTANCE { get; set; } = 0;
    }
}