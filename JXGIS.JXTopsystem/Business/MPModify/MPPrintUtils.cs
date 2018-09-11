using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.MPPrintUtils
{
    public class MPPrintUtils
    {
        /// <summary>
        /// 地名证明打印或门牌证打印
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="MPType"></param>
        /// <param name="CertificateType"></param>
        public static List<MPCertificate> MPCertificatePrint(List<string> IDs, int MPType, int CertificateType)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                List<MPCertificate> certificates = new List<Models.Extends.MPCertificate>();
                List<MPOfCertificate> mpOfCertificates = new List<Models.Entities.MPOfCertificate>();

                foreach (var ID in IDs)
                {
                    MPOfCertificate mpCertificate = new Models.Entities.MPOfCertificate();
                    var GUID = Guid.NewGuid().ToString();
                    var CreateTime = DateTime.Now.Date;
                    mpCertificate.ID = GUID;
                    mpCertificate.MPID = ID;
                    mpCertificate.CreateTime = CreateTime;
                    mpCertificate.CreateUser = LoginUtils.CurrentUser.UserName;
                    mpCertificate.MPType = MPType;
                    mpCertificate.CertificateType = CertificateType;
                    mpCertificate.Window = string.Join(",", LoginUtils.CurrentUser.Window);
                    mpOfCertificates.Add(mpCertificate);

                    MPCertificate certificate = new Models.Extends.MPCertificate();
                    certificate.ID = ID;
                    if (MPType == Enums.TypeInt.Residence)
                    {
                        var mpOfResidence = dbContext.MPOfResidence.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).First();
                        if (mpOfResidence == null)
                            throw new Exception($"ID为{ID}的道路门牌已经注销，请重新查询！");

                        certificate.PropertyOwner = mpOfResidence.PropertyOwner;
                        certificate.StandardAddress = mpOfResidence.StandardAddress;
                        certificate.FCZAddress = mpOfResidence.FCZAddress;
                        certificate.TDZAddress = mpOfResidence.TDZAddress;
                        certificate.BDCZAddress = mpOfResidence.BDCZAddress;
                        certificate.HJAddress = mpOfResidence.HJAddress;
                        certificate.OtherAddress = mpOfResidence.OtherAddress;
                        certificate.CountyID = mpOfResidence.CountyID;
                        certificate.CountyName = mpOfResidence.CountyID.Split('.').Last();
                        certificate.NeighborhoodsID = mpOfResidence.NeighborhoodsID;
                        certificate.NeighborhoodsName = mpOfResidence.NeighborhoodsID.Split('.').Last();
                        certificate.CommunityName = mpOfResidence.CommunityName;
                        certificate.MPNumber = mpOfResidence.MPNumber;
                        certificate.ResidenceName = mpOfResidence.ResidenceName;
                        certificate.Dormitory = mpOfResidence.Dormitory;
                        certificate.LZNumber = mpOfResidence.LZNumber;
                        certificate.DYNumber = mpOfResidence.DYNumber;
                        certificate.HSNumber = mpOfResidence.HSNumber;
                        certificate.AddressCoding = mpOfResidence.AddressCoding;

                        if (CertificateType == Enums.CertificateType.MPZ)
                            mpOfResidence.MPZPrintComplete = Enums.Complete.Yes;
                        else if (CertificateType == Enums.CertificateType.Placename)
                            mpOfResidence.DZZMPrintComplete = Enums.Complete.Yes;

                        BaseUtils.UpdateAddressCode(mpOfResidence, null, null, null, Enums.TypeInt.Residence);
                    }
                    else if (MPType == Enums.TypeInt.Road)
                    {
                        var mpOfRoad = dbContext.MPOfRoad.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).First();
                        if (mpOfRoad == null)
                            throw new Exception($"ID为{ID}的道路门牌已经注销，请重新查询！");

                        certificate.PropertyOwner = mpOfRoad.PropertyOwner;
                        certificate.StandardAddress = mpOfRoad.StandardAddress;
                        certificate.FCZAddress = mpOfRoad.FCZAddress;
                        certificate.TDZAddress = mpOfRoad.TDZAddress;
                        certificate.YYZZAddress = mpOfRoad.YYZZAddress;
                        certificate.OtherAddress = mpOfRoad.OtherAddress;
                        certificate.CountyID = mpOfRoad.CountyID;
                        certificate.CountyName = mpOfRoad.CountyID.Split('.').Last();
                        certificate.NeighborhoodsID = mpOfRoad.NeighborhoodsID;
                        certificate.NeighborhoodsName = mpOfRoad.NeighborhoodsID.Split('.').Last();
                        certificate.CommunityName = mpOfRoad.CommunityName;
                        certificate.RoadName = mpOfRoad.RoadName;
                        certificate.MPNumber = mpOfRoad.MPNumber;
                        certificate.OriginalMPAddress = mpOfRoad.OriginalMPAddress;
                        certificate.ResidenceName = mpOfRoad.ResidenceName;
                        certificate.AddressCoding = mpOfRoad.AddressCoding;

                        if (CertificateType == Enums.CertificateType.MPZ)
                            mpOfRoad.MPZPrintComplete = Enums.Complete.Yes;
                        else if (CertificateType == Enums.CertificateType.Placename)
                            mpOfRoad.DZZMPrintComplete = Enums.Complete.Yes;

                        BaseUtils.UpdateAddressCode(null, mpOfRoad, null, null, Enums.TypeInt.Road);
                    }
                    else if (MPType == Enums.TypeInt.Country)
                    {
                        var mpOfCounty = dbContext.MPOfCountry.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).First();
                        if (mpOfCounty == null)
                            throw new Exception($"ID为{ID}的农村门牌已经注销，请重新查询！");

                        certificate.PropertyOwner = mpOfCounty.PropertyOwner;
                        certificate.StandardAddress = mpOfCounty.StandardAddress;
                        certificate.TDZAddress = mpOfCounty.TDZAddress;
                        certificate.QQZAddress = mpOfCounty.QQZAddress;
                        certificate.OtherAddress = mpOfCounty.OtherAddress;
                        certificate.CountyID = mpOfCounty.CountyID;
                        certificate.CountyName = mpOfCounty.CountyID.Split('.').Last();
                        certificate.NeighborhoodsID = mpOfCounty.NeighborhoodsID;
                        certificate.NeighborhoodsName = mpOfCounty.NeighborhoodsID.Split('.').Last();
                        certificate.CommunityName = mpOfCounty.CommunityName;
                        certificate.ViligeName = mpOfCounty.ViligeName;
                        certificate.MPNumber = mpOfCounty.MPNumber;
                        certificate.HSNumber = mpOfCounty.HSNumber;
                        certificate.AddressCoding = mpOfCounty.AddressCoding;

                        if (CertificateType == Enums.CertificateType.MPZ)
                            mpOfCounty.MPZPrintComplete = Enums.Complete.Yes;
                        else if (CertificateType == Enums.CertificateType.Placename)
                            mpOfCounty.DZZMPrintComplete = Enums.Complete.Yes;

                        BaseUtils.UpdateAddressCode(null, null, mpOfCounty, null, Enums.TypeInt.Country);
                    }
                    certificates.Add(certificate);
                }
                dbContext.MPOfCertificate.AddRange(mpOfCertificates);
                dbContext.SaveChanges();
                return certificates;
            }
        }
    }
}