using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business
{
    public class MPStatisticUtils
    {
        /// <summary>
        /// 个人统计 提供所在窗口、经办人下拉列表框供筛选，提供自定义“开始时间“与”结束时间“进行统计查询
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="Window"></param>
        /// <param name="CreateUser"></param>
        /// <param name="CertificateType"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetMPBusinessUserTJ(int PageSize, int PageNum, DateTime? start, DateTime? end, string Window, string CreateUser, string CertificateType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                int count = 0;
                List<MPBusiness> query = new List<Models.Extends.MPBusiness>();

                var MPResidence = dbContext.MPOfResidence.Where(t => true);
                var MPRoad = dbContext.MPOfRoad.Where(t => true);
                var MPCountry = dbContext.MPOfCountry.Where(t => true);

                if (LoginUtils.CurrentUser.DistrictID != null && LoginUtils.CurrentUser.DistrictID.Count > 0 && !LoginUtils.CurrentUser.DistrictID.Contains("嘉兴市"))
                {
                    // 先删选出当前用户权限内的数据
                    var where1 = PredicateBuilder.False<MPOfResidence>();
                    var where2 = PredicateBuilder.False<MPOfRoad>();
                    var where3 = PredicateBuilder.False<MPOfCountry>();

                    foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                    {
                        where1 = where1.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                        where2 = where2.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                        where3 = where3.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                    }
                    MPResidence = MPResidence.Where(where1).Distinct();
                    MPRoad = MPRoad.Where(where2).Distinct();
                    MPCountry = MPCountry.Where(where3).Distinct();
                }

                #region 住宅类
                var queryResidence = from t in dbContext.MPOfCertificate
                                     join r in MPResidence
                                     on t.MPID equals r.ID
                                     where t.MPType == Enums.MPTypeCh.Residence
                                     select new MPBusiness
                                     {
                                         ID = t.ID,
                                         MPID = t.MPID,
                                         MPType = t.MPType,
                                         CreateTime = t.CreateTime,
                                         CreateUser = t.CreateUser,
                                         Window = t.Window,
                                         CertificateType = t.CertificateType,
                                         CountyID = r.CountyID,
                                         NeighborhoodsID = r.NeighborhoodsID,
                                         CommunityName = r.CommunityName,
                                         StandardAddress = r.StandardAddress,
                                         MPBZTime = r.BZTime
                                     };
                #endregion
                #region 道路类
                var queryRoad = from t in dbContext.MPOfCertificate
                                join r in MPRoad
                                on t.MPID equals r.ID
                                where t.MPType == Enums.MPTypeCh.Road
                                select new MPBusiness
                                {
                                    ID = t.ID,
                                    MPID = t.MPID,
                                    MPType = t.MPType,
                                    CreateTime = t.CreateTime,
                                    CreateUser = t.CreateUser,
                                    Window = t.Window,
                                    CertificateType = t.CertificateType,
                                    CountyID = r.CountyID,
                                    NeighborhoodsID = r.NeighborhoodsID,
                                    CommunityName = r.CommunityName,
                                    StandardAddress = r.StandardAddress,
                                    MPBZTime = r.BZTime
                                };
                #endregion
                #region 农村类
                var queryCountry = from t in dbContext.MPOfCertificate
                                   join r in MPCountry
                                   on t.MPID equals r.ID
                                   where t.MPType == Enums.MPTypeCh.Country
                                   select new MPBusiness
                                   {
                                       ID = t.ID,
                                       MPID = t.MPID,
                                       MPType = t.MPType,
                                       CreateTime = t.CreateTime,
                                       CreateUser = t.CreateUser,
                                       Window = t.Window,
                                       CertificateType = t.CertificateType,
                                       CountyID = r.CountyID,
                                       NeighborhoodsID = r.NeighborhoodsID,
                                       CommunityName = r.CommunityName,
                                       StandardAddress = r.StandardAddress,
                                       MPBZTime = r.BZTime
                                   };
                #endregion
                var All = queryResidence.Concat(queryRoad).Concat(queryCountry);


                if (start != null || end != null)
                {
                    if (start != null)
                        All = All.Where(t => t.CreateTime >= start);
                    if (end != null)
                        All = All.Where(t => t.CreateTime <= end);
                }

                if (!string.IsNullOrEmpty(Window))
                {

                    All = All.Where(t => t.Window.Contains(Window));
                }
                if (!string.IsNullOrEmpty(CreateUser))
                {
                    All = All.Where(t => t.CreateUser == CreateUser);
                }
                if (!string.IsNullOrEmpty(CertificateType))
                    All = All.Where(t => t.CertificateType == CertificateType);

                count = All.Count();
                query = All.OrderByDescending(t => t.CreateTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();

                var data = (from t in query
                            select new MPBusiness
                            {
                                ID = t.ID,
                                MPID = t.MPID,
                                MPType = t.MPType,
                                CreateTime = t.CreateTime,
                                CreateUser = t.CreateUser,
                                Window = t.Window,
                                Windows = !string.IsNullOrEmpty(t.Window) ? t.Window.Split(',').ToList() : null,
                                CertificateType = t.CertificateType,
                                CountyID = t.CountyID,
                                NeighborhoodsID = t.NeighborhoodsID,
                                CommunityName = t.CommunityName,
                                CountyName = !string.IsNullOrEmpty(t.CountyID) ? t.CountyID.Split('.').Last() : null,
                                NeighborhoodsName = !string.IsNullOrEmpty(t.NeighborhoodsID) ? t.NeighborhoodsID.Split('.').Last() : null,
                                StandardAddress = t.StandardAddress,
                                MPBZTime = t.MPBZTime
                            }).ToList();

                var personalInfo = (from t in All
                                    group t by t.CreateUser into g
                                    select new
                                    {
                                        userName = g.Key,
                                        total = g.Count(),
                                        MPZ = g.Where(t => t.CertificateType == Enums.CertificateType.MPZ).Count(),
                                        DMZM = g.Where(t => t.CertificateType == Enums.CertificateType.Placename).Count()
                                    }).ToList();

                return new Dictionary<string, object> {
                   { "PersonInfo",personalInfo},
                   { "Data",data},
                   { "Count",count}
                };
            }
        }

        /// <summary>
        /// 数量统计
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="DistrictID"></param>
        /// <param name="CertificateType"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetMPBusinessNumTJ(int PageSize, int PageNum, DateTime? start, DateTime? end, string DistrictID, string CertificateType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                int count = 0;
                //var MPResidence = dbContext.MPOfResidence as IEnumerable<MPOfResidence>;
                //var MPRoad = dbContext.MPOfRoad as IEnumerable<MPOfRoad>;
                //var MPCountry = dbContext.MPOfCountry as IEnumerable<MPOfCountry>;

                //if (LoginUtils.CurrentUser.DistrictID != null && LoginUtils.CurrentUser.DistrictID.Count > 0 && !LoginUtils.CurrentUser.DistrictID.Contains("嘉兴市"))
                //{
                //    // 先删选出当前用户权限内的数据
                //    var where1 = PredicateBuilder.False<MPOfResidence>();
                //    var where2 = PredicateBuilder.False<MPOfRoad>();
                //    var where3 = PredicateBuilder.False<MPOfCountry>();

                //    foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                //    {
                //        where1 = where1.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                //        where2 = where2.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                //        where3 = where3.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                //    }
                //    MPResidence = MPResidence.Where(where1.Compile()).Distinct();
                //    MPRoad = MPRoad.Where(where2.Compile()).Distinct();
                //    MPCountry = MPCountry.Where(where3.Compile()).Distinct();
                //}
                #region 住宅类
                var queryResidence = from t in dbContext.MPOfCertificate
                                     join r in dbContext.MPOfResidence
                                     on t.MPID equals r.ID
                                     where t.MPType == Enums.MPTypeCh.Residence
                                     select new MPBusiness
                                     {
                                         ID = t.ID,
                                         MPID = t.MPID,
                                         MPType = t.MPType,
                                         CreateTime = t.CreateTime,
                                         CreateUser = t.CreateUser,
                                         Window = t.Window,
                                         CertificateType = t.CertificateType,
                                         CountyID = r.CountyID,
                                         NeighborhoodsID = r.NeighborhoodsID,
                                         CommunityName = r.CommunityName,
                                         StandardAddress = r.StandardAddress,
                                         MPBZTime = r.BZTime
                                     };
                #endregion
                #region 道路类
                var queryRoad = from t in dbContext.MPOfCertificate
                                join r in dbContext.MPOfRoad
                                on t.MPID equals r.ID
                                where t.MPType == Enums.MPTypeCh.Road
                                select new MPBusiness
                                {
                                    ID = t.ID,
                                    MPID = t.MPID,
                                    MPType = t.MPType,
                                    CreateTime = t.CreateTime,
                                    CreateUser = t.CreateUser,
                                    Window = t.Window,
                                    CertificateType = t.CertificateType,
                                    CountyID = r.CountyID,
                                    NeighborhoodsID = r.NeighborhoodsID,
                                    CommunityName = r.CommunityName,
                                    StandardAddress = r.StandardAddress,
                                    MPBZTime = r.BZTime
                                };
                #endregion
                #region 农村类
                var queryCountry = from t in dbContext.MPOfCertificate
                                   join r in dbContext.MPOfCountry
                                   on t.MPID equals r.ID
                                   where t.MPType == Enums.MPTypeCh.Country
                                   select new MPBusiness
                                   {
                                       ID = t.ID,
                                       MPID = t.MPID,
                                       MPType = t.MPType,
                                       CreateTime = t.CreateTime,
                                       CreateUser = t.CreateUser,
                                       Window = t.Window,
                                       CertificateType = t.CertificateType,
                                       CountyID = r.CountyID,
                                       NeighborhoodsID = r.NeighborhoodsID,
                                       CommunityName = r.CommunityName,
                                       StandardAddress = r.StandardAddress,
                                       MPBZTime = r.BZTime
                                   };
                #endregion

                var concat = queryResidence.Concat(queryRoad).Concat(queryCountry);

                if (LoginUtils.CurrentUser.DistrictID != null && LoginUtils.CurrentUser.DistrictID.Count > 0 && !LoginUtils.CurrentUser.DistrictID.Contains("嘉兴市"))
                {
                    // 先删选出当前用户权限内的数据
                    var where = PredicateBuilder.False<MPBusiness>();

                    foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                    {
                        where = where.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                    }
                    concat = concat.Where(where).Distinct();
                }

                if (start != null || end != null)
                {
                    if (start != null)
                        concat = concat.Where(t => t.CreateTime >= start);
                    if (end != null)
                        concat = concat.Where(t => t.CreateTime <= end);
                }

                if (!string.IsNullOrEmpty(DistrictID))
                    concat = concat.Where(t => t.NeighborhoodsID.IndexOf(DistrictID + ".") == 0 || t.NeighborhoodsID == DistrictID);

                if (!string.IsNullOrEmpty(CertificateType))
                    concat = concat.Where(t => t.CertificateType == CertificateType);

                var result = from t in concat
                             group t by new { t.CountyID, t.NeighborhoodsID } into g
                             select new
                             {
                                 CountyID = g.Key.CountyID,
                                 NeighborhoodsID = g.Key.NeighborhoodsID,
                                 Total = g.Count(),
                                 MPZ = g.Where(t => t.CertificateType == Enums.CertificateType.MPZ).Count(),
                                 DMZM = g.Where(t => t.CertificateType == Enums.CertificateType.Placename).Count()
                             };
                count = result.Count();
                var query = result.OrderBy(t => t.CountyID).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                var data = (from t in query
                            select new
                            {
                                CountyName = !string.IsNullOrEmpty(t.CountyID) ? t.CountyID.Split('.').Last() : null,
                                NeighborhoodsName = !string.IsNullOrEmpty(t.CountyID) ? t.NeighborhoodsID.Split('.').Last() : null,
                                Total = t.Total,
                                MPZ = t.MPZ,
                                DMZM = t.DMZM
                            }).ToList();
                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };
            }
        }

        /// <summary>
        /// 获取各类门牌制作数量的统计数据
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="DistrictID"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetMPProduceTJ(int PageSize, int PageNum, string DistrictID, string CommunityName, DateTime? start, DateTime? end)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                int count = 0;
                var bigMPsize = dbContext.DMBZDic.Where(t => t.Type == "大门牌").Select(t => t.Size).ToList();
                var smallMPsize = dbContext.DMBZDic.Where(t => t.Type == "小门牌").Select(t => t.Size).ToList();

                #region 住宅
                var residenceMP = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.PLProduceID != null || t.LXProduceID != null);
                if (start != null || end != null)
                {
                    if (start != null)
                        residenceMP = residenceMP.Where(t => t.BZTime >= start);
                    if (end != null)
                        residenceMP = residenceMP.Where(t => t.BZTime <= end);
                }

                var lzdis = (from t in residenceMP
                             select new
                             {
                                 CountyID = t.CountyID,
                                 NeighborhoodsID = t.NeighborhoodsID,
                                 CommunityName = t.CommunityName,
                                 ResidenceName = t.ResidenceName,
                                 LZNumber = t.LZNumber,
                             }).Distinct();
                var lz = from t in lzdis
                         group t by new { t.CountyID, t.NeighborhoodsID, t.CommunityName } into g
                         select new Statistic
                         {
                             CountyID = g.Key.CountyID,
                             NeighborhoodsID = g.Key.NeighborhoodsID,
                             CommunityName = g.Key.CommunityName,
                             type = "楼幢牌",
                             Count = g.Count() * 2
                         };
                var dydis = (from t in residenceMP
                             select new
                             {
                                 CountyID = t.CountyID,
                                 NeighborhoodsID = t.NeighborhoodsID,
                                 CommunityName = t.CommunityName,
                                 ResidenceName = t.ResidenceName,
                                 LZNumber = t.LZNumber,
                                 DYNUmber = t.DYNumber,
                             }).Distinct();
                var dy = from t in dydis
                         group t by new { t.CountyID, t.NeighborhoodsID, t.CommunityName } into g
                         select new Statistic
                         {
                             CountyID = g.Key.CountyID,
                             NeighborhoodsID = g.Key.NeighborhoodsID,
                             CommunityName = g.Key.CommunityName,
                             type = "单元牌",
                             Count = g.Count()
                         };
                var hs = from t in residenceMP
                         group t by new { t.CountyID, t.NeighborhoodsID, t.CommunityName } into g
                         select new Statistic
                         {
                             CountyID = g.Key.CountyID,
                             NeighborhoodsID = g.Key.NeighborhoodsID,
                             CommunityName = g.Key.CommunityName,
                             type = "户室牌",
                             Count = g.Count()
                         };
                #endregion
                #region 道路
                var roadMP = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.PLProduceID != null || t.LXProduceID != null);

                if (start != null || end != null)
                {
                    if (start != null)
                        roadMP = roadMP.Where(t => t.BZTime >= start);
                    if (end != null)
                        roadMP = roadMP.Where(t => t.BZTime <= end);
                }

                var dmp = from t in roadMP
                          where bigMPsize.Contains(t.MPSize)
                          group t by new { t.CountyID, t.NeighborhoodsID, t.CommunityName } into g
                          select new Statistic
                          {
                              CountyID = g.Key.CountyID,
                              NeighborhoodsID = g.Key.NeighborhoodsID,
                              CommunityName = g.Key.CommunityName,
                              type = "大门牌",
                              Count = g.Count()
                          };
                var xmp = from t in roadMP
                          where smallMPsize.Contains(t.MPSize)
                          group t by new { t.CountyID, t.NeighborhoodsID, t.CommunityName } into g
                          select new Statistic
                          {
                              CountyID = g.Key.CountyID,
                              NeighborhoodsID = g.Key.NeighborhoodsID,
                              CommunityName = g.Key.CommunityName,
                              type = "小门牌",
                              Count = g.Count()
                          };
                #endregion
                #region 农村
                var countryMP = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.PLProduceID != null || t.LXProduceID != null);


                if (start != null || end != null)
                {
                    if (start != null)
                        countryMP = countryMP.Where(t => t.BZTime >= start);
                    if (end != null)
                        countryMP = countryMP.Where(t => t.BZTime <= end);
                }

                var ncmp = from t in countryMP
                           group t by new { t.CountyID, t.NeighborhoodsID, t.CommunityName } into g
                           select new Statistic
                           {
                               CountyID = g.Key.CountyID,
                               NeighborhoodsID = g.Key.NeighborhoodsID,
                               CommunityName = g.Key.CommunityName,
                               type = Enums.MPTypeCh.Country,
                               Count = g.Count()
                           };
                #endregion

                var rt = lz.Concat(dy).Concat(hs).Concat(dmp).Concat(xmp).Concat(hs);

                if (LoginUtils.CurrentUser.DistrictID != null && LoginUtils.CurrentUser.DistrictID.Count > 0 && !LoginUtils.CurrentUser.DistrictID.Contains("嘉兴市"))
                {
                    // 先删选出当前用户权限内的数据
                    var where = PredicateBuilder.False<Statistic>();

                    foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                    {
                        where = where.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                    }
                    rt = rt.Where(where).Distinct();
                }

                var result = from t in rt
                             group t by new { t.CountyID, t.NeighborhoodsID, t.CommunityName } into g
                             select new StatisticAll
                             {
                                 CountyID = g.Key.CountyID,
                                 NeighborhoodsID = g.Key.NeighborhoodsID,
                                 CommunityName = g.Key.CommunityName,
                                 DMP = g.Where(t => t.type == "大门牌").Select(t => t.Count).FirstOrDefault(),
                                 XMP = g.Where(t => t.type == "小门牌").Select(t => t.Count).FirstOrDefault(),
                                 LZP = g.Where(t => t.type == "楼幢牌").Select(t => t.Count).FirstOrDefault(),
                                 DYP = g.Where(t => t.type == "单元牌").Select(t => t.Count).FirstOrDefault(),
                                 HSP = g.Where(t => t.type == "户室牌").Select(t => t.Count).FirstOrDefault(),
                                 NCP = g.Where(t => t.type == Enums.MPTypeCh.Country).Select(t => t.Count).FirstOrDefault(),
                             };
                if (!string.IsNullOrEmpty(DistrictID))
                    result = result.Where(t => t.NeighborhoodsID.IndexOf(DistrictID + ".") == 0 || t.NeighborhoodsID == DistrictID);

                if (!string.IsNullOrEmpty(CommunityName))
                    result = result.Where(t => t.CommunityName == CommunityName);

                count = result.Count();
                var query = result.OrderBy(t => t.NeighborhoodsID).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                var data = (from t in query
                            select new StatisticAll
                            {
                                CountyID = t.CountyID,
                                NeighborhoodsID = t.NeighborhoodsID,
                                CountyName = !string.IsNullOrEmpty(t.CountyID) ? t.CountyID.Split('.').Last() : null,
                                NeighborhoodsName = !string.IsNullOrEmpty(t.NeighborhoodsID) ? t.NeighborhoodsID.Split('.').Last() : null,
                                CommunityName = t.CommunityName,
                                DMP = t.DMP,
                                XMP = t.XMP,
                                LZP = t.LZP,
                                DYP = t.DYP,
                                HSP = t.HSP,
                                NCP = t.NCP,
                                Sum = t.DMP + t.XMP + t.LZP + t.DYP + t.HSP + t.NCP
                            }).ToList();

                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };
            }
        }
    }
    class Statistic
    {
        public string CountyID { get; set; }
        public string CountyName { get; set; }
        public string NeighborhoodsID { get; set; }
        public string NeighborhoodsName { get; set; }
        public string CommunityName { get; set; }
        public int Count { get; set; }
        public string type { get; set; }
    }

    class StatisticAll
    {
        public string CountyID { get; set; }
        public string CountyName { get; set; }
        public string NeighborhoodsID { get; set; }
        public string NeighborhoodsName { get; set; }
        public string CommunityName { get; set; }
        public int LZP { get; set; }
        public int DYP { get; set; }
        public int HSP { get; set; }
        public int DMP { get; set; }
        public int XMP { get; set; }
        public int NCP { get; set; }
        public int Sum { get; set; }

    }
}