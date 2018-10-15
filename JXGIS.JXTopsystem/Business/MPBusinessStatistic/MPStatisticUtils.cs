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
        public static Dictionary<string, object> GetMPBusinessUserTJ(int PageSize, int PageNum, DateTime? start, DateTime? end, string Window, string CreateUser, int CertificateType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                int count = 0;
                List<MPBusiness> query = new List<Models.Extends.MPBusiness>();

                //var MPResidence = dbContext.MPOfResidence as IEnumerable<MPOfResidence>;
                //var MPRoad = dbContext.MPOfRoad as IEnumerable<MPOfRoad>;
                //var MPCountry = dbContext.MPOfCountry as IEnumerable<MPOfCountry>;

                ////if (LoginUtils.CurrentUser.DistrictID != null && LoginUtils.CurrentUser.DistrictID.Count > 0 && !LoginUtils.CurrentUser.DistrictID.Contains("嘉兴市"))
                ////{
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
                ////}

                #region 住宅类
                var queryResidence = from t in dbContext.MPOfCertificate

                                     join r in dbContext.MPOfResidence
                                     on t.MPID equals r.ID into rr
                                     from rt in rr.DefaultIfEmpty()

                                     where t.MPType == Enums.TypeInt.Residence
                                     select new MPBusiness
                                     {
                                         ID = t.ID,
                                         MPID = t.MPID,
                                         MPType = t.MPType,
                                         MPTypeName = "住宅门牌",
                                         //CertificateTypeName = t.CertificateType == Enums.CertificateType.Placename ? "地址证明开具" : "门牌证打印",
                                         CreateTime = t.CreateTime,
                                         CreateUser = t.CreateUser,
                                         Window = t.Window,
                                         //Windows = t.Window.Split(',').ToList(),
                                         CertificateType = t.CertificateType,
                                         CountyID = rt.CountyID,
                                         NeighborhoodsID = rt.NeighborhoodsID,
                                         CommunityName = rt.CommunityName,
                                         StandardAddress = rt.StandardAddress,
                                         MPBZTime = rt.BZTime
                                     };
                #endregion
                #region 道路类
                var queryRoad = from t in dbContext.MPOfCertificate

                                join r in dbContext.MPOfRoad
                                on t.MPID equals r.ID into rr
                                from rt in rr.DefaultIfEmpty()

                                where t.MPType == Enums.TypeInt.Road
                                select new MPBusiness
                                {
                                    ID = t.ID,
                                    MPID = t.MPID,
                                    MPType = t.MPType,
                                    MPTypeName = "道路门牌",
                                    //CertificateTypeName = t.CertificateType == Enums.CertificateType.Placename ? "地址证明开具" : "门牌证打印",
                                    CreateTime = t.CreateTime,
                                    CreateUser = t.CreateUser,
                                    Window = t.Window,
                                    //Windows = t.Window.Split(',').ToList(),
                                    CertificateType = t.CertificateType,
                                    CountyID = rt.CountyID,
                                    NeighborhoodsID = rt.NeighborhoodsID,
                                    CommunityName = rt.CommunityName,
                                    StandardAddress = rt.StandardAddress,
                                    MPBZTime = rt.BZTime
                                };
                #endregion
                #region 农村类
                var queryCountry = from t in dbContext.MPOfCertificate

                                   join r in dbContext.MPOfCountry
                                   on t.MPID equals r.ID into rr
                                   from rt in rr.DefaultIfEmpty()

                                   where t.MPType == Enums.TypeInt.Country
                                   select new MPBusiness
                                   {
                                       ID = t.ID,
                                       MPID = t.MPID,
                                       MPType = t.MPType,
                                       MPTypeName = "农村门牌",
                                       //CertificateTypeName = t.CertificateType == Enums.CertificateType.Placename ? "地址证明开具" : "门牌证打印",
                                       CreateTime = t.CreateTime,
                                       CreateUser = t.CreateUser,
                                       Window = t.Window,
                                       //Windows = t.Window.Split(',').ToList(),
                                       CertificateType = t.CertificateType,
                                       CountyID = rt.CountyID,
                                       NeighborhoodsID = rt.NeighborhoodsID,
                                       CommunityName = rt.CommunityName,
                                       StandardAddress = rt.StandardAddress,
                                       MPBZTime = rt.BZTime
                                   };
                #endregion
                var All = queryResidence.Concat(queryRoad).Concat(queryCountry);

                if (start != null || end != null)
                {
                    if (start != null)
                        All = All.Where(t => t.MPBZTime >= start);
                    if (end != null)
                        All = All.Where(t => t.MPBZTime <= end);
                }

                if (!string.IsNullOrEmpty(Window))
                {
                    All = All.Where(t => t.Windows.Contains(Window));
                }
                if (!string.IsNullOrEmpty(CreateUser))
                {
                    All = All.Where(t => t.CreateUser == CreateUser);
                }
                if (CertificateType != Enums.CertificateType.All)
                    All = All.Where(t => t.CertificateType == CertificateType);

                count = All.Count();
                query = All.OrderByDescending(t => t.CreateTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();

                var data = (from t in query
                            select new MPBusiness
                            {
                                ID = t.ID,
                                MPID = t.MPID,
                                MPType = t.MPType,
                                MPTypeName = t.MPTypeName,
                                CreateTime = t.CreateTime,
                                CreateUser = t.CreateUser,
                                Window = t.Window,
                                Windows = !string.IsNullOrEmpty(t.Window) ? t.Window.Split(',').ToList() : null,
                                CertificateType = t.CertificateType,
                                CertificateTypeName = t.CertificateType == Enums.CertificateType.Placename ? "地址证明开具" : "门牌证打印",
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
        public static Dictionary<string, object> GetMPBusinessNumTJ(int PageSize, int PageNum, DateTime? start, DateTime? end, string DistrictID, int CertificateType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                int count = 0;
                #region 住宅类
                var queryResidence = from t in dbContext.MPOfCertificate

                                     join r in dbContext.MPOfResidence
                                     on t.MPID equals r.ID into rr
                                     from rt in rr.DefaultIfEmpty()

                                     where t.MPType == Enums.TypeInt.Residence
                                     select new MPBusiness
                                     {
                                         ID = t.ID,
                                         MPID = t.MPID,
                                         MPType = t.MPType,
                                         MPTypeName = "住宅门牌",
                                         //CertificateTypeName = t.CertificateType == Enums.CertificateType.Placename ? "地址证明开具" : "门牌证打印",
                                         CreateTime = t.CreateTime,
                                         CreateUser = t.CreateUser,
                                         Window = t.Window,
                                         //Windows = t.Window.Split(',').ToList(),
                                         CertificateType = t.CertificateType,
                                         CountyID = rt.CountyID,
                                         NeighborhoodsID = rt.NeighborhoodsID,
                                         CommunityName = rt.CommunityName,
                                         StandardAddress = rt.StandardAddress,
                                         MPBZTime = rt.BZTime
                                     };
                #endregion
                #region 道路类
                var queryRoad = from t in dbContext.MPOfCertificate

                                join r in dbContext.MPOfRoad
                                on t.MPID equals r.ID into rr
                                from rt in rr.DefaultIfEmpty()

                                where t.MPType == Enums.TypeInt.Road
                                select new MPBusiness
                                {
                                    ID = t.ID,
                                    MPID = t.MPID,
                                    MPType = t.MPType,
                                    MPTypeName = "道路门牌",
                                    // CertificateTypeName = t.CertificateType == Enums.CertificateType.Placename ? "地址证明开具" : "门牌证打印",
                                    CreateTime = t.CreateTime,
                                    CreateUser = t.CreateUser,
                                    Window = t.Window,
                                    //Windows = t.Window.Split(',').ToList(),
                                    CertificateType = t.CertificateType,
                                    CountyID = rt.CountyID,
                                    NeighborhoodsID = rt.NeighborhoodsID,
                                    CommunityName = rt.CommunityName,
                                    StandardAddress = rt.StandardAddress,
                                    MPBZTime = rt.BZTime
                                };
                #endregion
                #region 农村类
                var queryCountry = from t in dbContext.MPOfCertificate

                                   join r in dbContext.MPOfCountry
                                   on t.MPID equals r.ID into rr
                                   from rt in rr.DefaultIfEmpty()

                                   where t.MPType == Enums.TypeInt.Country
                                   select new MPBusiness
                                   {
                                       ID = t.ID,
                                       MPID = t.MPID,
                                       MPType = t.MPType,
                                       MPTypeName = "农村门牌",
                                       //CertificateTypeName = t.CertificateType == Enums.CertificateType.Placename ? "地址证明开具" : "门牌证打印",
                                       CreateTime = t.CreateTime,
                                       CreateUser = t.CreateUser,
                                       Window = t.Window,
                                       //Windows = t.Window.Split(',').ToList(),
                                       CertificateType = t.CertificateType,
                                       CountyID = rt.CountyID,
                                       NeighborhoodsID = rt.NeighborhoodsID,
                                       CommunityName = rt.CommunityName,
                                       StandardAddress = rt.StandardAddress,
                                       MPBZTime = rt.BZTime
                                   };
                #endregion
                var All = queryResidence.Concat(queryRoad).Concat(queryCountry);
                //// 先删选出当前用户权限内的数据
                //var where = PredicateBuilder.False<MPBusiness>();
                //foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                //{
                //    where = where.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                //}
                //var queryAll = All.Where(where.Compile());

                if (start != null || end != null)
                {
                    if (start != null)
                        All = All.Where(t => t.MPBZTime >= start);
                    if (end != null)
                        All = All.Where(t => t.MPBZTime <= end);
                }

                if (!string.IsNullOrEmpty(DistrictID))
                    All = All.Where(t => t.NeighborhoodsID.IndexOf(DistrictID + '.') == 0 || t.NeighborhoodsID == DistrictID);

                if (CertificateType != Enums.CertificateType.All)
                    All = All.Where(t => t.CertificateType == CertificateType);

                var result = from t in All
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
                var query = result.OrderByDescending(t => t.CountyID).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
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
        public static Dictionary<string, object> GetMPProduceTJ(int PageSize, int PageNum, string DistrictID,string CommunityName,DateTime? start,DateTime? end)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                int count = 0;
                var bigMPsize = dbContext.DMBZDic.Where(t => t.Type == "大门牌").Select(t => t.Size).ToList();
                var smallMPsize = dbContext.DMBZDic.Where(t => t.Type == "小门牌").Select(t => t.Size).ToList();

                #region 住宅
                var residenceMP = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.MPProduceComplete == Enums.Complete.Yes);

                // 先删选出当前用户权限内的数据
                var where = PredicateBuilder.False<MPOfResidence>();
                foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                {
                    where = where.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                }
                var zz = residenceMP.Where(where.Compile());

                if (start != null || end != null)
                {
                    if (start != null)
                        zz = zz.Where(t => t.BZTime >= start);
                    if (end != null)
                        zz = zz.Where(t => t.BZTime <= end);
                }

                var lzdis = (from t in zz
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
                var dydis = (from t in zz
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
                var hs = from t in zz
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
                var roadMP = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.MPProduceComplete == Enums.Complete.Yes);

                // 先删选出当前用户权限内的数据
                var whereRoad = PredicateBuilder.False<MPOfRoad>();
                foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                {
                    whereRoad = whereRoad.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                }
                var dl = roadMP.Where(whereRoad.Compile());

                if (start != null || end != null)
                {
                    if (start != null)
                        dl = dl.Where(t => t.BZTime >= start);
                    if (end != null)
                        dl = dl.Where(t => t.BZTime <= end);
                }

                var dmp = from t in dl
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
                var xmp = from t in dl
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
                var countryMP = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.MPProduceComplete == Enums.Complete.Yes);

                // 先删选出当前用户权限内的数据
                var whereCountry = PredicateBuilder.False<MPOfCountry>();
                foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                {
                    whereCountry = whereCountry.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                }
                var nc = countryMP.Where(whereCountry.Compile());

                if (start != null || end != null)
                {
                    if (start != null)
                        nc = nc.Where(t => t.BZTime >= start);
                    if (end != null)
                        nc = nc.Where(t => t.BZTime <= end);
                }

                var ncmp = from t in nc
                           group t by new { t.CountyID, t.NeighborhoodsID, t.CommunityName } into g
                           select new Statistic
                           {
                               CountyID = g.Key.CountyID,
                               NeighborhoodsID = g.Key.NeighborhoodsID,
                               CommunityName = g.Key.CommunityName,
                               type = "农村门牌",
                               Count = g.Count()
                           };
                #endregion

                var all = lz.Concat(dy).Concat(hs).Concat(dmp).Concat(xmp).Concat(hs);

                var result = from t in all
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
                                 NCP = g.Where(t => t.type == "农村门牌").Select(t => t.Count).FirstOrDefault(),
                             };
                if (!string.IsNullOrEmpty(DistrictID))
                    result = result.Where(t => t.NeighborhoodsID.IndexOf(DistrictID + '.') == 0 || t.NeighborhoodsID == DistrictID);

                if (!string.IsNullOrEmpty(CommunityName))
                    result = result.Where(t => t.CommunityName == CommunityName);

                count = result.Count();
                var query = result.Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                var data = (from t in query
                            select new StatisticAll
                            {
                                CountyName = !string.IsNullOrEmpty(t.CountyID)? t.CountyID.Split('.').Last():null,
                                NeighborhoodsID = !string.IsNullOrEmpty(t.NeighborhoodsID) ? t.NeighborhoodsID.Split('.').Last():null,
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