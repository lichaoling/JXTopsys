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
        public static RPDetails SearchRPRepairByID(string ID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                RPDetails data = RPSearchUtils.SearchRPByID(ID);
                var infos = dbContext.RPRepair.Where(t => t.RPID == ID).Where(t => t.IsFinish == Enums.RPRepairFinish.No).ToList();
                data.RepairInfos = infos;
                return data;
            }
        }

        public static void RepairOrChangeRP(RP rp, Models.Entities.RPRepair rpRepairInfo, int repairMode)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.RP.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == rp.ID).First();
                if (query == null)
                    throw new Exception("该条路牌已被注销，请重新查询并编辑！");
                dbContext.RP.Remove(query);

                if (rpRepairInfo.IsFinish == Enums.RPRepairFinish.Yes)   //如果是修复
                {
                    rpRepairInfo.FinishRepaireTime = rpRepairInfo.FinishRepaireTime == null ? DateTime.Now.Date : rpRepairInfo.FinishRepaireTime;
                    rpRepairInfo.FinishRepaireUser = LoginUtils.CurrentUser.UserName;
                    //更新这条维修消息
                    //检查这个路牌是否还有未修复的记录，如果没有了就更新这条路牌为结束维修
                }
                else  //如果是维修或更换
                {
                    if (repairMode == Enums.RPRepairMode.Change)  //更换
                    {
                        var CountyCode = SystemUtils.Districts.Where(t => t.ID == query.CountyID).Select(t => t.Code).FirstOrDefault();
                        var NeighborhoodsCode = SystemUtils.Districts.Where(t => t.ID == query.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                        var year = DateTime.Now.Year.ToString();
                        var AddressCoding = CountyCode + NeighborhoodsCode + year;
                        rp.AddressCoding = AddressCoding;
                        rp.Code = query.Code;
                        rp.CountyID = query.CountyID;
                        rp.NeighborhoodsID = query.NeighborhoodsID;
                        rp.CommunityID = query.CommunityID;
                        var Position = (rp.Lng != null && rp.Lat != null) ? (DbGeography.FromText($"POINT({rp.Lng},{rp.Lat})")) : null;
                        rp.Position = Position;
                        rp.FinishRepaire = Enums.RPRepairFinish.No;
                        rp.State = query.State;
                    }
                    if (repairMode == Enums.RPRepairMode.Repair)  //维修
                    {
                        rp = query;
                    }
                    rp.RepairedCount = query.RepairedCount + 1;

                    rpRepairInfo.ID = Guid.NewGuid().ToString();
                    rpRepairInfo.RPID = rp.ID;
                    rpRepairInfo.RepairTime = rpRepairInfo.RepairTime == null ? DateTime.Now.Date : rpRepairInfo.RepairTime;
                    rpRepairInfo.RepairUser = LoginUtils.CurrentUser.UserName;
                    rpRepairInfo.IsFinish = Enums.RPRepairFinish.No;
                    //新增这条维修消息
                    dbContext.RPRepair.Add(rpRepairInfo);
                }
                var unfinishCount = dbContext.RPRepair.Where(t => t.RPID == rp.ID).Where(t => t.IsFinish == Enums.RPRepairFinish.No).ToList();
                rp.FinishRepaire = unfinishCount.Count() > 0 ? Enums.RPRepairFinish.No : Enums.RPRepairFinish.Yes;
                dbContext.RP.Add(rp);
                dbContext.SaveChanges();
            }
        }
    }
}