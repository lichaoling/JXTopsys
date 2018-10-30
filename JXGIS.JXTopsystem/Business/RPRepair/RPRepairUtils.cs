﻿using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Business.RPSearch;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

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

                var rps = dbContext.RPRepair.Where(t => t.RPID == ID);
                List<Models.Entities.RPRepair> infos = new List<Models.Entities.RPRepair>();
                if (RPRange == Enums.RPRange.All)
                    infos = rps.ToList();
                else if (RPRange == Enums.RPRange.YXF)
                    infos = rps.Where(t => t.IsFinish == Enums.RPRepairFinish.Yes).ToList();
                else
                    infos = rps.Where(t => t.IsFinish == Enums.RPRepairFinish.No).ToList();
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
        public static Models.Entities.RPRepair SearchRPRepairDetailByID(string RepairID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var RepairInfo = dbContext.RPRepair.Where(t => t.ID == RepairID).FirstOrDefault();
                if (RepairInfo == null)
                    throw new Exception("不存在该维修信息！");
                return RepairInfo;
            }
        }

        //public static void RepairOrChangeRP(string ID, string Model, string Size, string Material, string Manufacturers, Models.Entities.RPRepair rpRepairInfo, int repairMode)
        //{
        //    using (var dbContext = SystemUtils.NewEFDbContext)
        //    {
        //        var query = dbContext.RP.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == ID).FirstOrDefault();
        //        if (query == null)
        //            throw new Exception("该条路牌已被注销，请重新查询并编辑！");

        //        if (rpRepairInfo.IsFinish == Enums.RPRepairFinish.Yes)   //如果是修复
        //        {
        //            rpRepairInfo.FinishRepaireTime = rpRepairInfo.FinishRepaireTime == null ? DateTime.Now.Date : rpRepairInfo.FinishRepaireTime;
        //            rpRepairInfo.FinishRepaireUser = LoginUtils.CurrentUser.UserName;
        //            var unfinishCount = dbContext.RPRepair.Where(t => t.RPID == query.ID).Where(t => t.IsFinish == Enums.RPRepairFinish.No).ToList();
        //            query.FinishRepaire = unfinishCount.Count() > 0 ? Enums.RPRepairFinish.No : Enums.RPRepairFinish.Yes;
        //        }
        //        else  //如果是维修或更换
        //        {
        //            if (repairMode == Enums.RPRepairMode.Change)  //更换
        //            {
        //                query.Model = Model;
        //                query.Size = Size;
        //                query.Material = Material;
        //                query.Manufacturers = Manufacturers;
        //            }
        //            if (repairMode == Enums.RPRepairMode.Repair)  //维修
        //            {

        //            }
        //            query.RepairedCount++;
        //            query.FinishRepaire = Enums.RPRepairFinish.No;
        //            rpRepairInfo.ID = Guid.NewGuid().ToString();
        //            rpRepairInfo.RPID = query.ID;
        //            AddRPRepairContent(rpRepairInfo.RepairContent);
        //            rpRepairInfo.RepairTime = rpRepairInfo.RepairTime == null ? DateTime.Now.Date : rpRepairInfo.RepairTime;
        //            rpRepairInfo.RepairUser = LoginUtils.CurrentUser.UserName;
        //            rpRepairInfo.IsFinish = Enums.RPRepairFinish.No;
        //            dbContext.RPRepair.Add(rpRepairInfo);
        //        }
        //        dbContext.SaveChanges();
        //    }
        //}

        public static void RepairOrChangeRP(string oldDataJson)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var sourceData = Newtonsoft.Json.JsonConvert.DeserializeObject<RPRepareInfos>(oldDataJson);
                var targetRP = dbContext.RP.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == sourceData.ID).FirstOrDefault();
                var Dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(oldDataJson);
                if (sourceData.RepairMode == Enums.RPRepairMode.Change)//更换
                {
                    ObjectReflection.ModifyByReflection(sourceData, targetRP, Dic);
                }
                else if (sourceData.RepairMode == Enums.RPRepairMode.Repair)//维修
                {

                }

                targetRP.RepairedCount++;
                if (sourceData.FinishRepaireTime != null)
                    targetRP.FinishRepaire = Enums.RPRepairFinish.Yes;
                else
                    targetRP.FinishRepaire = Enums.RPRepairFinish.No;

                Models.Entities.RPRepair rpRepair = new Models.Entities.RPRepair();
                rpRepair.ID = sourceData.RepairID;
                rpRepair.RepairParts = sourceData.RepairParts;
                rpRepair.RepairFactory = sourceData.RepairFactory;
                rpRepair.RepairContent = sourceData.RepairContent;
                rpRepair.RepairMode = sourceData.RepairMode;
                rpRepair.RepairTime = sourceData.RepairTime;
                rpRepair.FinishRepaireTime = sourceData.FinishRepaireTime;
                dbContext.RPRepair.Add(rpRepair);
                dbContext.SaveChanges();
            }
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
    }
}