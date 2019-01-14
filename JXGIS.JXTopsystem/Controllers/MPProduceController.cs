using JXGIS.JXTopsystem.App_Start;
using JXGIS.JXTopsystem.Business;
using JXGIS.JXTopsystem.Business.MPProduce;
using JXGIS.JXTopsystem.Models.Extends;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JXGIS.JXTopsystem.Controllers
{
    public class MPProduceController : Controller
    {
        [LoggerFilter(Description = "获取已制作的零星门牌")]
        public ContentResult GetProducedLXMP(int PageSize, int PageNum, string MPType)
        {
            RtObj rt = null;
            try
            {
                var r = MPProduceUtils.GetProducedLXMP(PageSize, PageNum, MPType);
                rt = new RtObj(r);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd";
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt, timeConverter);
            return Content(s);
        }
        [LoggerFilter(Description = "获取未制作的零星门牌")]
        public ContentResult GetNotProducedLXMP(int PageSize, int PageNum, string MPType)
        {
            RtObj rt = null;
            try
            {
                var r = MPProduceUtils.GetNotProducedLXMP(PageSize, PageNum, MPType);
                rt = new RtObj(r);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd";
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt, timeConverter);
            return Content(s);
        }

        public JsonResult GetConditionForProduceLXMP(List<string> MPIDs, string MPType)
        {
            RtObj rt = null;
            try
            {
                Session["_ProduceLXMP_MPIDs"] = MPIDs;
                Session["_ProduceLXMP_MPType"] = MPType;
                using (var db = SystemUtils.NewEFDbContext)
                {
                    List<LXMPHZ> lxmphzs = new List<LXMPHZ>();
                    var LXProduceID = DateTime.Now.ToString("yyyyMMddhhmmss");

                    if (MPType == Enums.MPTypeCh.Road)
                    {
                        var sql = @"update MPOfRoad set LXProduceID=@LXProduceID,MPProduceUser=@MPProduceUser,MPProduceTime=@MPProduceTime where State=1 and AddType='零星' and MPProduce=1 and id in (@MPIDs)";
                        var c = db.Database.ExecuteSqlCommand(sql, new SqlParameter("@LXProduceID", LXProduceID), new SqlParameter("@MPProduceUser", LoginUtils.CurrentUser.UserName), new SqlParameter("@MPProduceTime", DateTime.Now), new SqlParameter("@MPIDs", string.Join(",", MPIDs)));
                    }
                    else if (MPType == Enums.MPTypeCh.Country)
                    {
                        var sql = @"update MPOfCountry set LXProduceID=@LXProduceID,MPProduceUser=@MPProduceUser,MPProduceTime=@MPProduceTime where State=1 and AddType='零星' and MPProduce=1 and id in (@MPIDs)";
                        var c = db.Database.ExecuteSqlCommand(sql, new SqlParameter("@LXProduceID", LXProduceID), new SqlParameter("@MPProduceUser", LoginUtils.CurrentUser.UserName), new SqlParameter("@MPProduceTime", DateTime.Now), new SqlParameter("@MPIDs", string.Join(",", MPIDs)));
                    }
                    else
                        throw new Exception("未知的错误类型！");
                }
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt, JsonRequestBehavior.AllowGet);
        }

        [LoggerFilter(Description = "批量制作零星门牌")]
        public ActionResult ProduceLXMP()
        {
            RtObj rt = null;
            try
            {
                var MPIDs = Session["_ProduceLXMP_MPIDs"] != null ? (List<string>)Session["_ProduceLXMP_MPIDs"] : null;
                var MPType = Session["_ProduceLXMP_MPType"] != null ? Session["_ProduceLXMP_MPType"].ToString() : null;
                var ms = MPProduceUtils.ProduceLXMP(MPIDs, MPType);
                Session["_ProduceLXMP_MPIDs"] = null;
                Session["_ProduceLXMP_MPType"] = null;
                string fileName = $"零星门牌制作清单.doc";
                return File(ms, "application/vnd.ms-word", fileName);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd";
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt, timeConverter);
            return Json(s, JsonRequestBehavior.AllowGet);
        }
        [LoggerFilter(Description = "获取批量制作完的零星门牌")]
        public ActionResult GetProducedLXMPDetails(string LXProduceID /*ProducedLXMPList producedLXMPList*/)
        {
            RtObj rt = null;
            try
            {
                var r = MPProduceUtils.GetProducedLXMPDetails(LXProduceID);
                return File(r, "application/vnd.ms-word", LXProduceID + ".doc");
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
                return Json(rt, JsonRequestBehavior.AllowGet);
            }
        }

        [LoggerFilter(Description = "获取已制作的批量门牌")]
        public ContentResult GetProducedPLMP(int PageSize, int PageNum, string MPType)
        {
            RtObj rt = null;
            try
            {
                var r = MPProduceUtils.GetProducedPLMP(PageSize, PageNum, MPType);
                rt = new RtObj(r);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd";
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt, timeConverter);
            return Content(s);
        }
        [LoggerFilter(Description = "获取未制作的批量门牌")]
        public ContentResult GetNotProducedPLMP(int PageSize, int PageNum, string MPType)
        {
            RtObj rt = null;
            try
            {
                var r = MPProduceUtils.GetNotProducedPLMP(PageSize, PageNum, MPType);
                rt = new RtObj(r);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd";
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt, timeConverter);
            return Content(s);
        }
        public JsonResult GetConditionForProducePLMP(List<string> PLIDs, string MPType)
        {
            RtObj rt = null;
            try
            {

                using (var db = SystemUtils.NewEFDbContext)
                {
                    List<LXMPHZ> lxmphzs = new List<LXMPHZ>();
                    var PLProduceID = DateTime.Now.ToString("yyyyMMddhhmmss");
                    //List<string> IDs = new List<string>();
                    if (MPType == Enums.MPTypeCh.Residence)
                    {
                        //var sql1 = @"select id from MPOfResidence where State=1 and AddType='批量' and PLProduceID is null and plid in (@plid)";
                        //IDs = db.Database.SqlQuery<string>(sql1, new SqlParameter("@plid", string.Join(",", PLIDs))).ToList();

                        var sql = @"update MPOfResidence set PLProduceID=@PLProduceID,MPProduceUser=@MPProduceUser,MPProduceTime=@MPProduceTime where State=1 and AddType='批量' and PLProduceID is null and plid in (@plid)";
                        var c = db.Database.ExecuteSqlCommand(sql, new SqlParameter("@PLProduceID", PLProduceID), new SqlParameter("@MPProduceUser", LoginUtils.CurrentUser.UserName), new SqlParameter("@MPProduceTime", DateTime.Now), new SqlParameter("@plid", string.Join(",", PLIDs)));

                    }
                    else if (MPType == Enums.MPTypeCh.Road)
                    {
                        //var sql1 = @"select id from MPOfRoad where State=1 and AddType='批量' and PLProduceID is null and plid in (@plid)";
                        //IDs = db.Database.SqlQuery<string>(sql1, new SqlParameter("@plid", string.Join(",", PLIDs))).ToList();

                        var sql = @"update MPOfRoad set PLProduceID=@PLProduceID,MPProduceUser=@MPProduceUser,MPProduceTime=@MPProduceTime where State=1 and AddType='批量' and PLProduceID is null and plid in (@plid)";
                        var c = db.Database.ExecuteSqlCommand(sql, new SqlParameter("@PLProduceID", PLProduceID), new SqlParameter("@MPProduceUser", LoginUtils.CurrentUser.UserName), new SqlParameter("@MPProduceTime", DateTime.Now), new SqlParameter("@plid", string.Join(",", PLIDs)));
                    }
                    else if (MPType == Enums.MPTypeCh.Country)
                    {
                        //var sql1 = @"select id from MPOfCountry where State=1 and AddType='批量' and PLProduceID is null and plid in (@plid)";
                        //IDs = db.Database.SqlQuery<string>(sql1, new SqlParameter("@plid", string.Join(",", PLIDs))).ToList();

                        var sql = @"update MPOfCountry set PLProduceID=@PLProduceID,MPProduceUser=@MPProduceUser,MPProduceTime=@MPProduceTime where State=1 and AddType='批量' and PLProduceID is null and plid in (@plid)";
                        var c = db.Database.ExecuteSqlCommand(sql, new SqlParameter("@PLProduceID", PLProduceID), new SqlParameter("@MPProduceUser", LoginUtils.CurrentUser.UserName), new SqlParameter("@MPProduceTime", DateTime.Now), new SqlParameter("@plid", string.Join(",", PLIDs)));
                    }
                    else
                        throw new Exception("未知的错误类型！");
                    Session["_ProducePLMP_PLIDs"] = PLIDs;
                    Session["_ProducePLMP_MPType"] = MPType;
                }
                rt = new RtObj();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            return Json(rt, JsonRequestBehavior.AllowGet);
        }
        [LoggerFilter(Description = "批量制作批量导入的门牌")]
        public ActionResult ProducePLMP()
        {
            RtObj rt = null;
            try
            {
                var PLIDs = Session["_ProducePLMP_PLIDs"] != null ? (List<string>)Session["_ProducePLMP_PLIDs"] : null;
                var MPType = Session["_ProducePLMP_MPType"] != null ? Session["_ProducePLMP_MPType"].ToString() : null;
                var ms = MPProduceUtils.ProducePLMP(PLIDs, MPType);
                Session["_ProduceLXMP_list"] = null;
                Session["_ProducePLMP_MPType"] = null;
                string fileName = $"批量门牌制作清单.doc";
                return File(ms, "application/vnd.ms-word", fileName);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd";
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt, timeConverter);
            return Json(s, JsonRequestBehavior.AllowGet);
        }
        [LoggerFilter(Description = "获取批量制作完的批量导入门牌")]
        public JsonResult GetProducedPLMPDetails(string PLProduceID/* ProducedPLMPList producedPLMPList*/)
        {
            RtObj rt = null;
            try
            {
                var r = MPProduceUtils.GetProducedPLMPDetails(PLProduceID);
                rt = new RtObj(r);
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd";
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt, timeConverter);
            return Json(s);
        }


        //public JsonResult CreateTabToWord()
        //{
        //    RtObj rt = null;
        //    try
        //    {
        //        MPProduceUtils.CreateTabToWord2();
        //        rt = new RtObj();
        //    }
        //    catch (Exception ex)
        //    {
        //        rt = new RtObj(ex);
        //    }
        //    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
        //    timeConverter.DateTimeFormat = "yyyy-MM-dd";
        //    var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt, timeConverter);
        //    return Json(s);

        //}
    }
}