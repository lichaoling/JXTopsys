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
        public static Dictionary<string, object> GetMPBusinessUserTJ(int PageSize, int PageNum, string start, string end, string Window, string CreateUser, int CertificateType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                int count = 0;
                List<MPBusiness> query = new List<Models.Extends.MPBusiness>();
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
                                         CertificateTypeName = t.CertificateType == Enums.CertificateType.Placename ? "地址证明开具" : "门牌证打印",
                                         CreateTime = t.CreateTime,
                                         CreateUser = t.CreateUser,
                                         Window = t.Window,
                                         Windows = t.Window.Split(',').ToList(),
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
                                    CertificateTypeName = t.CertificateType == Enums.CertificateType.Placename ? "地址证明开具" : "门牌证打印",
                                    CreateTime = t.CreateTime,
                                    CreateUser = t.CreateUser,
                                    Window = t.Window,
                                    Windows = t.Window.Split(',').ToList(),
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
                                       CertificateTypeName = t.CertificateType == Enums.CertificateType.Placename ? "地址证明开具" : "门牌证打印",
                                       CreateTime = t.CreateTime,
                                       CreateUser = t.CreateUser,
                                       Window = t.Window,
                                       Windows = t.Window.Split(',').ToList(),
                                       CertificateType = t.CertificateType,
                                       CountyID = rt.CountyID,
                                       NeighborhoodsID = rt.NeighborhoodsID,
                                       CommunityName = rt.CommunityName,
                                       StandardAddress = rt.StandardAddress,
                                       MPBZTime = rt.BZTime
                                   };
                #endregion
                var All = queryResidence.Concat(queryRoad).Concat(queryCountry);
                // 先删选出当前用户权限内的数据
                var where = PredicateBuilder.False<MPBusiness>();
                foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                {
                    where = where.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                }
                var queryAll = All.Where(where.Compile());

                if (!string.IsNullOrEmpty(start) || !string.IsNullOrEmpty(end))
                {
                    if (!string.IsNullOrEmpty(start))
                    {
                        queryAll = queryAll.Where(t => String.Compare(t.MPBZTime.ToString(), start, StringComparison.Ordinal) >= 0);
                    }
                    if (!string.IsNullOrEmpty(end))
                    {
                        queryAll = queryAll.Where(t => String.Compare(t.MPBZTime.ToString(), end, StringComparison.Ordinal) <= 0);
                    }
                }

                if (!string.IsNullOrEmpty(Window))
                {
                    queryAll = queryAll.Where(t => t.Windows.Contains(Window));
                }
                if (!string.IsNullOrEmpty(CreateUser))
                {
                    queryAll = queryAll.Where(t => t.CreateUser == CreateUser);
                }
                if (CertificateType != Enums.CertificateType.All)
                    queryAll = queryAll.Where(t => t.CertificateType == CertificateType);

                count = queryAll.Count();
                query = queryAll.OrderByDescending(t => t.CreateTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();

                var data = (from t in query
                            select new MPBusiness
                            {
                                ID = t.ID,
                                MPID = t.MPID,
                                MPType = t.MPType,
                                MPTypeName = t.MPTypeName,
                                CertificateTypeName = t.CertificateTypeName,
                                CreateTime = t.CreateTime,
                                CreateUser = t.CreateUser,
                                Window = t.Window,
                                CertificateType = t.CertificateType,
                                CountyID = t.CountyID,
                                NeighborhoodsID = t.NeighborhoodsID,
                                CommunityName = t.CommunityName,
                                CountyName = t.CountyID.Split('.').Last(),
                                NeighborhoodsName = t.NeighborhoodsID.Split('.').Last(),
                                StandardAddress = t.StandardAddress,
                                MPBZTime = t.MPBZTime
                            }).ToList();
                return new Dictionary<string, object> {
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
        public static Dictionary<string, object> GetMPBusinessNumTJ(int PageSize, int PageNum, string start, string end, string DistrictID, int CertificateType)
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
                                         CertificateTypeName = t.CertificateType == Enums.CertificateType.Placename ? "地址证明开具" : "门牌证打印",
                                         CreateTime = t.CreateTime,
                                         CreateUser = t.CreateUser,
                                         Window = t.Window,
                                         Windows = t.Window.Split(',').ToList(),
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
                                    CertificateTypeName = t.CertificateType == Enums.CertificateType.Placename ? "地址证明开具" : "门牌证打印",
                                    CreateTime = t.CreateTime,
                                    CreateUser = t.CreateUser,
                                    Window = t.Window,
                                    Windows = t.Window.Split(',').ToList(),
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
                                       CertificateTypeName = t.CertificateType == Enums.CertificateType.Placename ? "地址证明开具" : "门牌证打印",
                                       CreateTime = t.CreateTime,
                                       CreateUser = t.CreateUser,
                                       Window = t.Window,
                                       Windows = t.Window.Split(',').ToList(),
                                       CertificateType = t.CertificateType,
                                       CountyID = rt.CountyID,
                                       NeighborhoodsID = rt.NeighborhoodsID,
                                       CommunityName = rt.CommunityName,
                                       StandardAddress = rt.StandardAddress,
                                       MPBZTime = rt.BZTime
                                   };
                #endregion
                var All = queryResidence.Concat(queryRoad).Concat(queryCountry);
                // 先删选出当前用户权限内的数据
                var where = PredicateBuilder.False<MPBusiness>();
                foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                {
                    where = where.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                }
                var queryAll = All.Where(where.Compile());


                if (!string.IsNullOrEmpty(start) || !string.IsNullOrEmpty(end))
                {
                    if (!string.IsNullOrEmpty(start))
                    {
                        queryAll = queryAll.Where(t => String.Compare(t.MPBZTime.ToString(), start, StringComparison.Ordinal) >= 0);
                    }
                    if (!string.IsNullOrEmpty(end))
                    {
                        queryAll = queryAll.Where(t => String.Compare(t.MPBZTime.ToString(), end, StringComparison.Ordinal) <= 0);
                    }
                }

                if (!string.IsNullOrEmpty(DistrictID))
                    queryAll = queryAll.Where(t => t.NeighborhoodsID.IndexOf(DistrictID + '.') == 0 || t.NeighborhoodsID == DistrictID);

                if (CertificateType != Enums.CertificateType.All)
                    queryAll = queryAll.Where(t => t.CertificateType == CertificateType);

                var result = from t in queryAll
                             group t by new { t.CountyID, t.NeighborhoodsID, t.CertificateTypeName } into g
                             select new
                             {
                                 CountyID = g.Key.CountyID,
                                 NeighborhoodsID = g.Key.NeighborhoodsID,
                                 CertificateTypeName = g.Key.CertificateTypeName,
                                 Count = g.Count()
                             };
                count = result.Count();
                var query = result.Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                var data = (from t in query
                            select new
                            {
                                CountyName = t.CountyID.Split('.').Last(),
                                NeighborhoodsName = t.NeighborhoodsID.Split('.').Last(),
                                CertificateTypeName = t.CertificateTypeName,
                                Count = t.Count
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
        public static Dictionary<string, object> GetMPProduceTJ(int PageSize, int PageNum, string DistrictID)
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

                count = result.Count();
                var query = result.Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                var data = (from t in query
                            select new StatisticAll
                            {
                                CountyName = t.CountyID.Split('.').Last(),
                                NeighborhoodsID = t.NeighborhoodsID.Split('.').Last(),
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