using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Controllers;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.RPSearch
{
    public class RPSearchUtils
    {
        public static Dictionary<string, object> SearchRP(int PageSize, int PageNum, string DistrictID, string RoadName, string Intersection, string Model, string Size, string Material, string Manufacturers, string FrontTagline, string BackTagline, DateTime? start, DateTime? end, int UseState)
        {
            int count = 0;
            List<RPDetails> data = null;
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var q = dbContext.RP.Where(t => t.State == UseState);
                // 先删选出当前用户权限内的数据
                var where = PredicateBuilder.False<RP>();
                foreach (var userDID in LoginUtils.CurrentUser.DistrictID)
                {
                    where = where.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                }
                var query = q.Where(where.Compile());

                //行政区划筛选
                if (!(string.IsNullOrEmpty(DistrictID) || DistrictID == "嘉兴市"))
                {
                    query = query.Where(t => t.CountyID == DistrictID || t.NeighborhoodsID == DistrictID);
                }

                //交叉路口筛选
                if (!string.IsNullOrEmpty(Intersection))
                {
                    query = query.Where(t => t.Intersection == Intersection);
                }

                //样式筛选
                if (!string.IsNullOrEmpty(Model))
                {
                    query = query.Where(t => t.Model == Model);
                }
                //规格筛选
                if (!string.IsNullOrEmpty(Size))
                {
                    query = query.Where(t => t.Size == Size);
                }
                //材质筛选
                if (!string.IsNullOrEmpty(Material))
                {
                    query = query.Where(t => t.Material == Material);
                }
                //生产厂家筛选
                if (!string.IsNullOrEmpty(Manufacturers))
                {
                    query = query.Where(t => t.Manufacturers == Manufacturers);
                }
                //正面宣传语筛选
                if (!string.IsNullOrEmpty(FrontTagline))
                {
                    query = query.Where(t => t.FrontTagline.Contains(FrontTagline));
                }
                //反面宣传语筛选
                if (!string.IsNullOrEmpty(BackTagline))
                {
                    query = query.Where(t => t.BackTagline.Contains(BackTagline));
                }
                //设置时间筛选
                //if (!string.IsNullOrEmpty(start) || !string.IsNullOrEmpty(end))
                //{
                //    if (!string.IsNullOrEmpty(start))
                //    {
                //        query = query.Where(t => String.Compare(t.BZTime.ToString(), start, StringComparison.Ordinal) >= 0);
                //    }
                //    if (!string.IsNullOrEmpty(end))
                //    {
                //        query = query.Where(t => String.Compare(t.BZTime.ToString(), end, StringComparison.Ordinal) <= 0);
                //    }
                //}
                if (start != null || end != null)
                {
                    if (start != null)
                        query = query.Where(t => t.BZTime >= start);
                    if (end != null)
                        query = query.Where(t => t.BZTime <= end);
                }
                //道路名称筛选
                if (!string.IsNullOrEmpty(RoadName))
                {
                    query = query.Where(t => t.RoadName.Contains(RoadName));
                }
                count = query.Count();
                //如果是导出，就返回所有
                if (PageNum == -1 && PageSize == -1)
                {
                    query = query.OrderByDescending(t => t.BZTime).ToList();
                }
                //如果是分页查询，就分页返回
                else
                {
                    query = query.OrderByDescending(t => t.BZTime).Skip(PageSize * (PageNum - 1)).Take(PageSize).ToList();
                }

                data = (from t in query
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
                            RepairedCount = t.RepairedCount,
                            Lat = t.Position.Latitude,
                            Lng = t.Position.Longitude
                        }).ToList();

                //关联路牌照片 重组url
                List<RPDetails> rt = new List<RPDetails>();
                foreach (var d in data)
                {
                    var baseUrl = Path.Combine("Files", Enums.TypeStr.RP, Enums.RPFileType.BZPhoto, d.ID);
                    var files = dbContext.RPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.RPID == d.ID);
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
                        d.RPBZPhoto = filelst;
                    }
                    rt.Add(d);
                }

                return new Dictionary<string, object> {
                   { "Data",rt},
                   { "Count",count}
                };
            }
        }

        public static RPDetails SearchRPByID(string RPID)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var rp = dbContext.RP.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == RPID).FirstOrDefault();
                if (rp == null)
                    throw new Exception("该路牌已经被注销！");
                var data = new RPDetails(rp);

                var files = dbContext.RPOfUploadFiles.Where(t => t.State == Enums.UseState.Enable).Where(t => t.RPID == RPID);
                var baseUrl_QRCode = FileController.RPQRCodeRelativePath;
                if (files.Count() > 0)
                {
                    var baseUrl_BZ = Path.Combine(FileController.RPBZPhotoRelativePath, RPID);

                    var filelst = (from t in files
                                   select new Pictures
                                   {
                                       FileID = t.ID,
                                       Name = t.Name,
                                       RelativePath = baseUrl_BZ + "/" + t.ID + t.FileEx,
                                       TRelativePath = baseUrl_BZ + "/t-" + t.ID + t.FileEx
                                   }).ToList();
                    data.RPBZPhoto = filelst;
                }
                data.CodeFile = new Pictures()
                {
                    RelativePath = baseUrl_QRCode + "/" + data.Code + ".jpg",
                    TRelativePath = baseUrl_QRCode + "/t-" + data.Code + ".jpg",
                };
                data.CountyName = data.CountyID.Split('.').Last();
                data.NeighborhoodsName = data.NeighborhoodsID.Split('.').Last();
                data.Lat = data.Position.Latitude;
                data.Lng = data.Position.Longitude;
                return data;
            }
        }
    }
}