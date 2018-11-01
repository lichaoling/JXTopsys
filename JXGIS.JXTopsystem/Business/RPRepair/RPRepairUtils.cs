using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Business.RPSearch;
using JXGIS.JXTopsystem.Controllers;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;
using static JXGIS.JXTopsystem.Controllers.FileController;

namespace JXGIS.JXTopsystem.Business.RPRepair
{
    public class RPRepairUtils
    {
        /// <summary>
        /// 根据路牌ID获取还未修复的维修记录
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static RPDetails SearchRPRepairByID(string ID, int RPRange)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                RPDetails data = RPSearchUtils.SearchRPByID(ID);

                var rpr = dbContext.RPRepair.Where(t => t.RPID == ID);
                List<Models.Entities.RPRepair> infos = new List<Models.Entities.RPRepair>();
                if (RPRange == Enums.RPRange.All)
                    infos = rpr.ToList();
                else if (RPRange == Enums.RPRange.YXF)
                    infos = rpr.Where(t => t.FinishRepaireTime != null).ToList();
                else
                    infos = rpr.Where(t => t.FinishRepaireTime == null).ToList();
                data.RepairInfos = infos;
                return data;
            }
        }
        public static Dictionary<string, object> GetNewRPRepair(string RPID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                RPDetails data = RPSearchUtils.SearchRPByID(RPID);
                var newGuid = Guid.NewGuid().ToString();
                return new Dictionary<string, object>()
                {
                    { "RPDetails",data },
                    { "NewGuid",newGuid }
                };
            }
        }

        /// <summary>
        /// 根据路牌维修ID获取维修记录详情
        /// </summary>
        /// <param name="RepairID"></param>
        /// <returns></returns>
        public static Dictionary<string, object> SearchRPRepairDetailByID(string RepairID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var RepairInfo = dbContext.RPRepair.Where(t => t.ID == RepairID).FirstOrDefault();
                if (RepairInfo == null)
                    throw new Exception("不存在该维修信息！");
                var RP = RPSearchUtils.SearchRPByID(RepairInfo.RPID);
                if (RP == null)
                    throw new Exception("维修信息所属路牌不存在！");

                List<Paths> RepairBeoforePic = new List<Paths>();
                List<Paths> RepairAfterPic = new List<Paths>();
                var files = dbContext.RPPepairUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.RPRepairID == RepairID);

                var beforeFiles = files.Where(t => t.RepairType == Enums.RPRepairType.Before).ToList();
                foreach (var f in beforeFiles)
                {
                    var p = FileController.GetUploadFilePath(Enums.RPFileType.RepairPhoto, RepairID, f.ID, f.Name, Enums.RPRepairType.Before);
                    RepairBeoforePic.Add(p);
                }


                var afterFiles = files.Where(t => t.RepairType == Enums.RPRepairType.After).ToList();
                foreach (var f in afterFiles)
                {
                    var p = FileController.GetUploadFilePath(Enums.RPFileType.RepairPhoto, RepairID, f.ID, f.Name, Enums.RPRepairType.After);
                    RepairAfterPic.Add(p);
                }

                return new Dictionary<string, object>()
                {
                    { "RP",RP},
                    { "RepairInfo",RepairInfo},
                    { "BeforePics",RepairBeoforePic},
                    { "AfterPics",RepairAfterPic}
                };
            }
        }
        public static void DeleteRPRepairByID(string RepairID)
        {
            string RPID = null;
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var RepairInfo = dbContext.RPRepair.Where(t => t.ID == RepairID).FirstOrDefault();
                RPID = RepairInfo.RPID;
                dbContext.RPRepair.Remove(RepairInfo);
                dbContext.SaveChanges();
            }
            MOdifyRepairCountAndRepairFinish(RPID);//重新赋值
        }
        public static void ModifyRPRepair(string oldDataJson)
        {
            string RPID = null;
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Entities.RPRepair>(oldDataJson);
                var targetData = dbContext.RPRepair.Where(t => t.ID == sourceData.ID).FirstOrDefault();
                if (targetData == null)
                    throw new Exception("该维修记录已经被删除！");
                var Dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(oldDataJson);
                ObjectReflection.ModifyByReflection(sourceData, targetData, Dic);
                dbContext.SaveChanges();
                RPID = targetData.RPID;
            }
            MOdifyRepairCountAndRepairFinish(RPID);//重新赋值
        }

        /// <summary>
        /// 新增一条维修或更换的维修记录+修改一条已经存在的维修记录
        /// </summary>
        /// <param name="oldDataJson"></param>
        public static void RepairOrChangeRP(string oldDataJson)
        {
            string RPID = null;
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<RPRepareInfos>(oldDataJson);
                var targetRP = dbContext.RP.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == sourceData.RPID).FirstOrDefault();
                if (targetRP == null)
                    throw new Exception("该路牌已被注销！");
                var Dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(oldDataJson);
                var targetRPR = dbContext.RPRepair.Where(t => t.ID == sourceData.ID).FirstOrDefault();

                if (targetRPR == null)//这条维修记录不存在，就新增一条维修或者更换的维修记录
                {
                    if (sourceData.RepairMode == Enums.RPRepairMode.Change)//更换
                    {
                        targetRP.Model = sourceData.Model;
                        targetRP.Material = sourceData.Material;
                        targetRP.Size = sourceData.Size;
                        targetRP.Manufacturers = sourceData.Manufacturers;
                    }
                    else if (sourceData.RepairMode == Enums.RPRepairMode.Repair)//维修
                    {

                    }
                    Models.Entities.RPRepair rpRepair = new Models.Entities.RPRepair();
                    rpRepair.ID = sourceData.ID;
                    rpRepair.RPID = sourceData.RPID;
                    rpRepair.RepairParts = sourceData.RepairParts;
                    rpRepair.RepairFactory = sourceData.RepairFactory;
                    rpRepair.RepairContent = sourceData.RepairContent;
                    rpRepair.RepairMode = sourceData.RepairMode;
                    rpRepair.RepairTime = sourceData.RepairTime;
                    rpRepair.FinishRepaireTime = sourceData.FinishRepaireTime;
                    dbContext.RPRepair.Add(rpRepair);
                }
                else //这条维修记录存在，就修改一条维修记录
                {
                    ObjectReflection.ModifyByReflection(sourceData, targetRPR, Dic);
                }

                RPID = sourceData.RPID;
                dbContext.SaveChanges();
            }
            MOdifyRepairCountAndRepairFinish(RPID);//重新赋值
        }

        public static void AddRPRepairContent(string content)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var data = dbContext.RPRepairContent.Where(t => t.RepairContent == content);
                if (data.Count() == 0)
                {
                    RPRepairContent d = new Models.Entities.RPRepairContent();
                    d.ID = Guid.NewGuid().ToString(); ;
                    d.RepairContent = content;
                    dbContext.RPRepairContent.Add(d);
                    dbContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 根据路牌ID对该条路牌的维修次数和是否所有维修都修复的值进行重新赋值
        /// </summary>
        /// <param name="RPID"></param>
        public static void MOdifyRepairCountAndRepairFinish(string RPID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var RP = dbContext.RP.Where(t => t.ID == RPID).FirstOrDefault();
                var rpr = dbContext.RPRepair.Where(t => t.RPID == RPID);
                RP.RepairedCount = rpr.Count();
                RP.FinishRepaire = rpr.Where(t => t.FinishRepaireTime == null).Count() > 0 ? Enums.RPRepairFinish.No : Enums.RPRepairFinish.Yes;
                dbContext.SaveChanges();
            }
        }
    }
}