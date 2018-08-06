using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.MPCertificate
{
    public class MPCertificateUtils
    {
        /// <summary>
        /// 地名证明打印或门牌证打印
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="MPType"></param>
        /// <param name="CertificateType"></param>
        public static void MPCertificatePrint(string ID, int MPType, int CertificateType)
        {
            using (var dbContenxt = SystemUtils.NewEFDbContext)
            {
                MPOfCertificate mpCertificate = new Models.Entities.MPOfCertificate();
                var GUID = Guid.NewGuid().ToString();
                var CreateTime = DateTime.Now.Date;
                mpCertificate.ID = GUID;
                mpCertificate.MPID = ID;
                mpCertificate.CreateTime = CreateTime;
                mpCertificate.CreateUser = LoginUtils.CurrentUser.UserID;
                mpCertificate.MPType = MPType;
                mpCertificate.CertificateType = CertificateType;
                //受理窗口属于？？ 待完成

                dbContenxt.MPOfCertificate.Add(mpCertificate);
                dbContenxt.SaveChanges();
            }
        }

        /// <summary>
        /// 根据ID获取这条门牌数据的社区ID
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="MPType"></param>
        /// <returns></returns>
        public static string getDistrictID(string ID, int MPType)
        {
            string DistrictID = "";
            if (MPType == Enums.MPType.Residence)//住宅
            {
                DistrictID = SystemUtils.NewEFDbContext.MPOFResidence.Where(t => t.ID == ID).Select(t => t.CommunityID).FirstOrDefault();
            }
            else if (MPType == Enums.MPType.Road)//道路
            {
                DistrictID = SystemUtils.NewEFDbContext.MPOfRoad.Where(t => t.ID == ID).Select(t => t.CommunityID).FirstOrDefault();
            }
            else if (MPType == Enums.MPType.Country)//农村
            {
                DistrictID = SystemUtils.NewEFDbContext.MPOfCountry.Where(t => t.ID == ID).Select(t => t.CommunityID).FirstOrDefault();
            }
            return DistrictID;
        }

        /// <summary>
        /// 地名证明或门牌证浏览
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="MPType"></param>
        /// <param name="CertificateType"></param>
        /// <returns></returns>
        public static Models.Extends.MPCertificate MPCertificateQuery(string ID, int MPType, int CertificateType)
        {
            Models.Extends.MPCertificate query = new Models.Extends.MPCertificate();
            using (var dbContenxt = SystemUtils.NewEFDbContext)
            {
                if (MPType == Enums.MPType.Residence)//住宅
                {
                    var mpOfResidence = dbContenxt.MPOFResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID);
                    if (mpOfResidence.Count() == 0)
                        throw new Exception("该门牌已经注销，请重新查询！");
                    if (CertificateType == Enums.CertificateType.Placename)//地名证明浏览
                    {
                        var rt = mpOfResidence.FirstOrDefault();
                        query.ID = rt.ID;
                        query.PropertyOwner = rt.PropertyOwner;
                        query.StandardAddress = rt.StandardAddress;
                        query.FCZAddress = rt.FCZAddress;
                        query.TDZAddress = rt.TDZAddress;
                        query.HJAddress = rt.HJAddress;
                        query.OtherAddress = rt.OtherAddress;
                    }
                    else if (CertificateType == Enums.CertificateType.MPZ) //门牌证浏览
                    {
                        var q = (from t in dbContenxt.MPOFResidence
                                 join d in dbContenxt.Road
                                 on t.RoadID == null ? t.RoadID : t.RoadID.ToLower() equals d.RoadID.ToString().ToLower() into dd
                                 from dt in dd.DefaultIfEmpty()
                                 where t.State == Enums.UseState.Enable && t.ID == ID
                                 select new Models.Extends.MPCertificate
                                 {
                                     ID = t.ID,
                                     CountyID = t.CountyID,
                                     NeighborhoodsID = t.NeighborhoodsID,
                                     CommunityID = t.CommunityID,
                                     ResidenceName = t.ResidenceName,
                                     RoadName = dt.RoadName,
                                     MPNumber = t.MPNumber,
                                     Dormitory = t.Dormitory,
                                     LZNumber = t.LZNumber,
                                     DYNumber = t.DYNumber,
                                     HSNumber = t.HSNumber,
                                     PropertyOwner = t.PropertyOwner,
                                     AddressCoding = t.AddressCoding
                                 }).ToList();

                        query = (from t in q
                                 join a in SystemUtils.Districts
                                   on t.CountyID equals a.ID into aa
                                 from at in aa.DefaultIfEmpty()

                                 join b in SystemUtils.Districts
                                 on t.NeighborhoodsID equals b.ID into bb
                                 from bt in bb.DefaultIfEmpty()

                                 join c in SystemUtils.Districts
                                 on t.CommunityID equals c.ID into cc
                                 from ct in cc.DefaultIfEmpty()
                                 select new Models.Extends.MPCertificate
                                 {
                                     ID = t.ID,
                                     CountyName = at.Name,
                                     NeighborhoodsName = bt.Name,
                                     CommunityName = ct.Name,
                                     ResidenceName = t.ResidenceName,
                                     RoadName = t.RoadName,
                                     MPNumber = t.MPNumber,
                                     Dormitory = t.Dormitory,
                                     LZNumber = t.LZNumber,
                                     DYNumber = t.DYNumber,
                                     HSNumber = t.HSNumber,
                                     PropertyOwner = t.PropertyOwner,
                                     AddressCoding = t.AddressCoding
                                 }).FirstOrDefault();
                    }
                    else
                    {
                        throw new Exception("办理业务未知！");
                    }

                }
                else if (MPType == Enums.MPType.Road)//道路
                {
                    var mpOfRoad = dbContenxt.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID);
                    if (mpOfRoad.Count() == 0)
                        throw new Exception("该道路门牌已经注销，请重新查询！");
                    if (CertificateType == Enums.CertificateType.Placename)//地名证明浏览
                    {
                        var rt = mpOfRoad.FirstOrDefault();
                        query.ID = rt.ID;
                        query.PropertyOwner = rt.PropertyOwner;
                        query.StandardAddress = rt.StandardAddress;
                        query.FCZAddress = rt.FCZAddress;
                        query.TDZAddress = rt.TDZAddress;
                        query.YYZZAddress = rt.YYZZAddress;
                        query.OtherAddress = rt.OtherAddress;
                    }
                    else if (CertificateType == Enums.CertificateType.MPZ) //门牌证浏览
                    {
                        var q = (from t in dbContenxt.MPOfRoad
                                 join d in dbContenxt.Road
                                 on t.RoadID == null ? t.RoadID : t.RoadID.ToLower() equals d.RoadID.ToString().ToLower() into dd
                                 from dt in dd.DefaultIfEmpty()

                                 where t.State == Enums.UseState.Enable && t.ID == ID
                                 select new Models.Extends.MPCertificate
                                 {
                                     ID = t.ID,
                                     CountyID = t.CountyID,
                                     NeighborhoodsID = t.NeighborhoodsID,
                                     CommunityID = t.CommunityID,
                                     RoadName = dt.RoadName,
                                     MPNumber = t.MPNumber,
                                     OriginalNumber = t.OriginalNumber,
                                     PropertyOwner = t.PropertyOwner,
                                     AddressCoding = t.AddressCoding
                                 }).ToList();

                        query = (from t in q
                                 join a in SystemUtils.Districts
                                 on t.CountyID equals a.ID into aa
                                 from at in aa.DefaultIfEmpty()

                                 join b in SystemUtils.Districts
                                 on t.NeighborhoodsID equals b.ID into bb
                                 from bt in bb.DefaultIfEmpty()

                                 join c in SystemUtils.Districts
                                 on t.CommunityID equals c.ID into cc
                                 from ct in cc.DefaultIfEmpty()
                                 select new Models.Extends.MPCertificate
                                 {
                                     ID = t.ID,
                                     CountyName = at.Name,
                                     NeighborhoodsName = bt.Name,
                                     CommunityName = ct.Name,
                                     RoadName = t.RoadName,
                                     MPNumber = t.MPNumber,
                                     OriginalNumber = t.OriginalNumber,
                                     PropertyOwner = t.PropertyOwner,
                                     AddressCoding = t.AddressCoding
                                 }).FirstOrDefault();
                    }
                }
                else if (MPType == Enums.MPType.Country)//农村
                {
                    var mpOfCounty = dbContenxt.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID);
                    if (mpOfCounty.Count() == 0)
                        throw new Exception("该门牌已经注销，请重新查询！");
                    if (CertificateType == Enums.CertificateType.Placename)//地名证明浏览
                    {
                        var rt = mpOfCounty.FirstOrDefault();
                        query.ID = rt.ID;
                        query.PropertyOwner = rt.PropertyOwner;
                        query.StandardAddress = rt.StandardAddress;
                        query.TDZAddress = rt.TDZAddress;
                        query.QQZAddress = rt.QQZAddress;
                        query.OtherAddress = rt.OtherAddress;
                    }
                    else if (CertificateType == Enums.CertificateType.MPZ) //门牌证浏览
                    {
                        var q = (from t in dbContenxt.MPOfCountry
                                 where t.State == Enums.UseState.Enable && t.ID == ID
                                 select new Models.Extends.MPCertificate
                                 {
                                     ID = t.ID,
                                     CountyID = t.CountyID,
                                     NeighborhoodsID = t.NeighborhoodsID,
                                     CommunityID = t.CommunityID,
                                     ViligeName = t.ViligeName,
                                     MPNumber = t.MPNumber,
                                     OriginalNumber = t.OriginalNumber,
                                     PropertyOwner = t.PropertyOwner,
                                     AddressCoding = t.AddressCoding
                                 }).ToList();

                        query = (from t in q
                                 join a in SystemUtils.Districts
                                 on t.CountyID equals a.ID into aa
                                 from at in aa.DefaultIfEmpty()

                                 join b in SystemUtils.Districts
                                 on t.NeighborhoodsID equals b.ID into bb
                                 from bt in bb.DefaultIfEmpty()

                                 join c in SystemUtils.Districts
                                 on t.CommunityID equals c.ID into cc
                                 from ct in cc.DefaultIfEmpty()
                                 select new Models.Extends.MPCertificate
                                 {
                                     ID = t.ID,
                                     CountyName = at.Name,
                                     NeighborhoodsName = bt.Name,
                                     CommunityName = ct.Name,
                                     ViligeName = t.ViligeName,
                                     MPNumber = t.MPNumber,
                                     OriginalNumber = t.OriginalNumber,
                                     PropertyOwner = t.PropertyOwner,
                                     AddressCoding = t.AddressCoding
                                 }).FirstOrDefault();
                    }
                }
                else
                {
                    throw new Exception("门牌类型不存在！");
                }
                return query;
            }
        }
    }
}