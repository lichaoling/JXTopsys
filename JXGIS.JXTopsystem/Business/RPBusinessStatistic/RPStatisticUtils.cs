﻿using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.RPBusinessStatistic
{
    public class RPStatisticUtils
    {
        /// <summary>
        /// 路牌数量统计
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="DistrictID"></param>
        /// <param name="CommunityName"></param>
        /// <param name="RoadName"></param>
        /// <param name="Model"></param>
        /// <param name="Material"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetRPNumTJ(int PageSize, int PageNum, string start, string end, string DistrictID, string CommunityName, string RoadName, string Model, string Material, string Size)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var query = dbContext.RP.Where(t => t.State == Enums.UseState.Enable);
                if (!string.IsNullOrEmpty(start) || !string.IsNullOrEmpty(end))
                {
                    if (!string.IsNullOrEmpty(start))
                    {
                        query = query.Where(t => String.Compare(t.BZTime.ToString(), start, StringComparison.Ordinal) >= 0);
                    }
                    if (!string.IsNullOrEmpty(end))
                    {
                        query = query.Where(t => String.Compare(t.BZTime.ToString(), end, StringComparison.Ordinal) <= 0);
                    }
                }

                if (!string.IsNullOrEmpty(DistrictID))
                    query = query.Where(t => t.NeighborhoodsID.IndexOf(DistrictID + '.') == 0 || t.NeighborhoodsID == DistrictID);

                if (!string.IsNullOrEmpty(CommunityName))
                    query = query.Where(t => t.CommunityName == CommunityName);

                if (!string.IsNullOrEmpty(RoadName))
                    query = query.Where(t => t.RoadName == RoadName);

                if (!string.IsNullOrEmpty(Model))
                    query = query.Where(t => t.Model == Model);

                if (!string.IsNullOrEmpty(Material))
                    query = query.Where(t => t.Material == Material);

                if (!string.IsNullOrEmpty(Size))
                    query = query.Where(t => t.Size == Size);

                var re = from t in query
                         group t by new { t.CountyID, t.NeighborhoodsID, t.CommunityName, t.RoadName, t.Model, t.Material, t.Size } into g
                         select new
                         {
                             CountyID = g.Key.CountyID,
                             NeighborhoodsID = g.Key.NeighborhoodsID,
                             CommunityName = g.Key.CommunityName,
                             RoadName = g.Key.RoadName,
                             Model = g.Key.Model,
                             Material = g.Key.Material,
                             Size = g.Key.Size,
                             Count = g.Count(),
                         };
                var count = re.Count();
                var result = re.Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                var data = from t in result
                           select new
                           {
                               CountyID = t.CountyID,
                               CountyName = t.CountyID.Split('.').Last(),
                               NeighborhoodsID = t.NeighborhoodsID,
                               NeighborhoodsName = t.NeighborhoodsID.Split('.').Last(),
                               CommunityName = t.CommunityName,
                               RoadName = t.RoadName,
                               Model = t.Model,
                               Material = t.Material,
                               Size = t.Size,
                               Count = t.Count,
                           };
                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };

            }
        }

        /// <summary>
        /// 路牌维护统计
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageNum"></param>
        /// <param name="DistrictID"></param>
        /// <param name="CommunityName"></param>
        /// <param name="RepairMode"></param>
        /// <param name="RepairedCount"></param>
        /// <param name="RepairParts"></param>
        /// <param name="RepairContent"></param>
        /// <param name="RepairFactory"></param>
        /// <param name="isFinishRepair"></param>
        /// <param name="FinishTimeStart"></param>
        /// <param name="FinishTimeEnd"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetRPRepairTJ(int PageSize, int PageNum, string DistrictID, string CommunityName, int RepairMode, int RepairedCount, string RepairParts, string RepairContent, string RepairFactory, int isFinishRepair, string FinishTimeStart, string FinishTimeEnd)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                IQueryable<Models.Entities.RPRepair> query = dbContext.RPRepair.Where(t => true);
                if (RepairMode != Enums.RPRepairMode.All)
                {
                    query = query.Where(t => t.RepairMode == RepairMode);
                }
                if (!string.IsNullOrEmpty(RepairParts))
                {
                    query = query.Where(t => t.RepairParts == RepairParts);
                }
                if (!string.IsNullOrEmpty(RepairContent))
                {
                    query = query.Where(t => t.RepairContent == RepairContent);
                }
                if (!string.IsNullOrEmpty(RepairFactory))
                {
                    query = query.Where(t => t.RepairFactory == RepairFactory);
                }
                if (isFinishRepair == Enums.Complete.Yes)//已修复,有修复的起止时间
                {
                    query = query.Where(t => t.IsFinish == Enums.Complete.Yes);
                    if (!string.IsNullOrEmpty(FinishTimeStart))
                    {
                        query = query.Where(t => String.Compare(t.FinishRepaireTime.ToString(), FinishTimeStart, StringComparison.Ordinal) >= 0);
                    }
                    if (!string.IsNullOrEmpty(FinishTimeEnd))
                    {
                        query = query.Where(t => String.Compare(t.FinishRepaireTime.ToString(), FinishTimeEnd, StringComparison.Ordinal) <= 0);
                    }
                }
                else if (isFinishRepair == Enums.Complete.NO)//未修复
                {
                    query = query.Where(t => t.IsFinish == Enums.Complete.NO);
                }
                var rpID = query.Select(t => t.RPID).Distinct().ToList();
                var rps = dbContext.RP.Where(t => rpID.Contains(t.ID));
                rps = rps.Where(t => t.RepairedCount == RepairedCount);
                if (!string.IsNullOrEmpty(DistrictID))
                    rps = rps.Where(t => t.NeighborhoodsID.IndexOf(DistrictID + '.') == 0 || t.NeighborhoodsID == DistrictID);
                if (!string.IsNullOrEmpty(CommunityName))
                    rps = rps.Where(t => t.CommunityName == CommunityName);


                var count = rps.Count();
                var data = rps.OrderByDescending(t => t.BZTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                var result = (from t in data
                              select new RPDetails
                              {
                                  ID = t.ID,
                                  CountyID = t.CountyID,
                                  NeighborhoodsID = t.NeighborhoodsID,
                                  CountyName = t.CountyID.Split('.').Last(),
                                  NeighborhoodsName = t.NeighborhoodsID.Split('.').Last(),
                                  CommunityName = t.CommunityName,
                                  RoadName = t.RoadName,
                                  Intersection = t.Intersection,
                                  Direction = t.Direction,
                                  BZTime = t.BZTime,
                                  CreateTime = t.CreateTime,
                                  RepairedCount = t.RepairedCount
                              }).ToList();
                //关联路牌照片 重组url
                List<RPDetails> rt = new List<RPDetails>();
                foreach (var r in result)
                {
                    var baseUrl = Path.Combine("Files", Enums.TypeStr.RP, Enums.RPFileType.BZPhoto, r.ID);
                    var files = dbContext.RPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.RPID == r.ID);
                    if (files.Count() > 0)
                    {
                        var filelst = (from t in files
                                       select new Pictures
                                       {
                                           FileID = t.ID,
                                           Name = t.Name,
                                           RelativePath = baseUrl + "/" + t.ID + t.FileEx,
                                           TRelativePath = baseUrl + "/t-" + t.ID + t.FileEx
                                       }).ToList();
                        r.RPBZPhoto = filelst;
                    }
                    rt.Add(r);
                }

                return new Dictionary<string, object> {
                   { "Data",rt},
                   { "Count",count}
                };

            }
        }
    }
}