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
                    var p = FileController.GetUploadFilePath(Enums.FileType.RepairPhoto, RepairID, f.ID, f.Name, Enums.RPRepairType.Before);
                    RepairBeoforePic.Add(p);
                }


                var afterFiles = files.Where(t => t.RepairType == Enums.RPRepairType.After).ToList();
                foreach (var f in afterFiles)
                {
                    var p = FileController.GetUploadFilePath(Enums.FileType.RepairPhoto, RepairID, f.ID, f.Name, Enums.RPRepairType.After);
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
            ModifyRepairCountAndRepairFinish(RPID);//重新赋值
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
            ModifyRepairCountAndRepairFinish(RPID);//重新赋值
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
                        targetRP.BXFS = sourceData.BXFS;


                    }
                    else if (sourceData.RepairMode == Enums.RPRepairMode.Repair)//维修
                    {

                    }
                    else if (sourceData.RepairMode == Enums.RPRepairMode.Del)//拆回
                    {

                    }
                    Models.Entities.RPRepair rpRepair = new Models.Entities.RPRepair();
                    rpRepair.ID = sourceData.ID;
                    rpRepair.RPID = sourceData.RPID;
                    rpRepair.RepairParts = sourceData.RepairParts;
                    rpRepair.RepairFactory = sourceData.RepairFactory;
                    rpRepair.RepairContent = sourceData.RepairContent;
                    rpRepair.RepairMode = sourceData.RepairMode;
                    rpRepair.Model = sourceData.Model;
                    rpRepair.Material = sourceData.Material;
                    rpRepair.Size = sourceData.Size;
                    rpRepair.Manufacturers = sourceData.Manufacturers;
                    rpRepair.RepairTime = sourceData.RepairTime;
                    rpRepair.FinishRepaireTime = sourceData.FinishRepaireTime;
                    rpRepair.BXFS = sourceData.BXFS;
                    dbContext.RPRepair.Add(rpRepair);
                }
                else //这条维修记录存在，就修改一条维修记录
                {
                    ObjectReflection.ModifyByReflection(sourceData, targetRPR, Dic);
                }

                RPID = sourceData.RPID;
                dbContext.SaveChanges();
            }
            ModifyRepairCountAndRepairFinish(RPID);//重新赋值
        }

        /// <summary>
        /// 根据路牌ID对该条路牌的维修次数和是否所有维修都修复的值进行重新赋值
        /// </summary>
        /// <param name="RPID"></param>
        public static void ModifyRepairCountAndRepairFinish(string RPID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var rp = dbContext.RP.Where(t => t.ID == RPID).FirstOrDefault();
                var rpr = dbContext.RPRepair.Where(t => t.RPID == RPID).Where(t => t.RepairMode == Enums.RPRepairMode.Change || t.RepairMode == Enums.RPRepairMode.Repair);
                rp.RepairedCount = rpr.Count();
                rp.FinishRepaire = rpr.Where(t => t.FinishRepaireTime == null).Count() > 0 ? Enums.RPRepairFinish.No : Enums.RPRepairFinish.Yes;
                if (rp.FinishRepaire == Enums.RPRepairFinish.Yes)
                {
                    rp.FinishRepaireTime = rpr.Max(t => t.FinishRepaireTime);
                }
                else
                {
                    rp.FinishRepaireTime = null;
                }
                dbContext.SaveChanges();
            }
        }
    }
}