using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace JXGIS.JXTopsystem.Business.Common
{
    public class BaseUtils
    {
        #region 对文件进行操作
        /// <summary>
        /// 保存文件，检查文件是否重名，并返回文件名列表
        /// </summary>
        /// <param name="files"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        private static List<string> SaveMPFiles(IList<HttpPostedFile> files, string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            List<string> UploadNames = new List<string>();

            foreach (var file in files)
            {
                if (file.ContentLength > 0)
                {
                    int counter = 1;
                    string filename = file.FileName;
                    string path = Path.Combine(directory, filename);
                    string extension = Path.GetExtension(path);
                    string newFullPath = path;
                    while (System.IO.File.Exists(newFullPath))
                    {
                        string newFilename = $"{Path.GetFileNameWithoutExtension(path)}({counter}){extension}";
                        newFullPath = Path.Combine(directory, newFilename);
                        counter++;
                    }
                    UploadNames.Add(Path.GetFileName(newFullPath));
                    MemoryStream m = new MemoryStream();
                    FileStream fs = new FileStream(newFullPath, FileMode.OpenOrCreate);
                    BinaryWriter w = new BinaryWriter(fs);
                    w.Write(m.ToArray());
                    fs.Close();
                    m.Close();
                }
            }
            return UploadNames;
        }
        /// <summary>
        /// 将所有的文件存到文件夹，并将记录存入MPOfUploadFiles表中，首先要获取到每个文件的文件名和证件类型，取当前这个门牌ID，再生成一个GUID，存入数据库
        /// </summary>
        /// <param name="files">文件集合</param>
        /// <param name="MPID">门牌ID</param>
        /// <param name="DocType">证件类型</param>
        /// <param name="MPTypeStr">门牌类型</param>
        /// <returns></returns>
        private static void SaveMPFilesByID(IList<HttpPostedFile> files, string MPID, string DocType, string MPTypeStr)
        {
            var directory = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Files", "MPFile", MPTypeStr, MPID, DocType);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                foreach (var file in files)
                {
                    if (file.ContentLength > 0)
                    {
                        var guid = Guid.NewGuid().ToString();
                        string filename = file.FileName;
                        string extension = Path.GetExtension(filename);
                        string newfilename = guid + extension;
                        string fullPath = Path.Combine(directory, newfilename);
                        MemoryStream m = new MemoryStream();
                        FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate);
                        BinaryWriter w = new BinaryWriter(fs);
                        w.Write(m.ToArray());
                        fs.Close();
                        m.Close();
                        MPOfUploadFiles data = new MPOfUploadFiles();
                        data.ID = guid;
                        data.MPID = MPID;
                        data.Name = filename;
                        data.FileEx = extension;
                        data.DocType = DocType;
                        data.State = Enums.UseState.Enable;
                        dbContext.MPOfUploadFiles.Add(data);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        /// <summary>
        /// 从表中所有文件中删选出不在当前文件中的一些数据并逻辑删除，新增的一些文件存进去
        /// </summary>
        /// <param name="CurrentIDs">当前所有文件的ID/param>
        /// <param name="AdddedFiles">新增加的文件</param>
        /// <param name="MPID"></param>
        /// <param name="DocType"></param>
        /// <param name="MPTypeStr"></param>
        private static void UpdateMPFilesByID(List<string> CurrentIDs, IList<HttpPostedFile> AdddedFiles, string MPID, string DocType, string MPTypeStr)
        {
            var directory = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Files", "MPFile", MPTypeStr, MPID, DocType);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                if (CurrentIDs != null && CurrentIDs.Count() > 0)
                {
                    string sql = $@"update [TopSystemDB].[dbo].[MPOFUPLOADFILES]  
                                    set [State]={Enums.UseState.Delete} 
                                    where [ID] not in ({string.Join(",", CurrentIDs)}) 
                                    and [State]={Enums.UseState.Enable} 
                                    and [MPID]='{MPID}' 
                                    and [DocType]='{DocType}'";
                    var rt = dbContext.Database.ExecuteSqlCommand(sql);
                }
                SaveMPFilesByID(AdddedFiles, MPID, DocType, MPTypeStr);
            }
        }
        #endregion

        /// <summary>
        /// 正则表达式检查字符串是否是数字
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool CheckIsNumber(string number)
        {
            const string pattern = "^[1-9]*[1-9][0-9]*$";
            Regex rx = new Regex(pattern);
            return rx.IsMatch(number);
        }
        /// <summary>
        /// 正则表达式检查字符串是否是电话号码或手机号
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool CheckIsPhone(string number)
        {
            const string patternPhone = @"^1(3|4|5|7|8)\d{9}$";
            const string patternTel = @"^(\(\d{3,4}\)|\d{3,4}-|\s)?\d{7,14}$";
            Regex rxPhone = new Regex(patternPhone);
            Regex rxTel = new Regex(patternTel);
            return rxPhone.IsMatch(number) || rxTel.IsMatch(number);
        }
        /// <summary>
        /// 检查是否为日期格式  2018年7月25日
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool CheckIsDatetime(ref string date)
        {
            try
            {
                date = Convert.ToDateTime(date).ToString("d");
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 检查门牌规格是否在几个规定的规格里面
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CheckMPSize(ref string data)
        {
            data = data.ToLower();
            var MPSizes = new string[] { "40*60CM", "30*20CM", "21*15MM", "18*14MM", "15*10MM" };
            return MPSizes.Contains(data);
        }
        public static bool CheckPostcode(string data)
        {
            const string pattern = @"^\d{6}$";
            Regex rx = new Regex(pattern);
            return data.StartsWith("3140") && rx.IsMatch(data);
        }
        public static bool CheckMPProdece(string data, ref int MPProdece)
        {
            var t = true;
            if (data.Equals("是"))
                MPProdece = 1;
            else if (data.Equals("否"))
                MPProdece = 2;
            else
                t = false;
            return t;
        }

        public static string GetGUID()
        {
            return Guid.NewGuid().ToString();
        }

        public static void UpdateAddressCode(MPOfResidence zz, MPOfRoad dl, MPOfCountry nc, RP rp, int type)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                var dis = dbContext.District.Where(t => t.State == Enums.UseState.Enable);
                if (type == Enums.TypeInt.Residence)
                {
                    var CountyCode = zz.AddressCoding.Substring(0, 2);
                    var NeighborhoodsCode = zz.AddressCoding.Substring(2, 3);
                    var county = dis.Where(t => t.ID == zz.CountyID).FirstOrDefault();
                    var neighbor = dis.Where(t => t.ID == zz.NeighborhoodsID).FirstOrDefault();
                    if (county == null || neighbor == null || county.ID != zz.CountyID || neighbor.ID != zz.NeighborhoodsID)
                    {
                        var othercode = zz.AddressCoding.Substring(zz.AddressCoding.Length - 9);
                        CountyCode = dis.Where(t => t.ID == zz.CountyID).Select(t => t.Code).FirstOrDefault();
                        NeighborhoodsCode = dis.Where(t => t.ID == zz.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                        zz.AddressCoding = CountyCode + NeighborhoodsCode + othercode;
                    }
                }
                else if (type == Enums.TypeInt.Road)
                {
                    var CountyCode = dl.AddressCoding.Substring(0, 2);
                    var NeighborhoodsCode = dl.AddressCoding.Substring(2, 3);
                    var county = dis.Where(t => t.Code == CountyCode).FirstOrDefault();
                    var neighbor = dis.Where(t => t.Code == NeighborhoodsCode).FirstOrDefault();
                    if (county == null || neighbor == null || county.ID != dl.CountyID || neighbor.ID != dl.NeighborhoodsID)
                    {
                        var othercode = dl.AddressCoding.Substring(dl.AddressCoding.Length - 9);
                        CountyCode = dis.Where(t => t.ID == dl.CountyID).Select(t => t.Code).FirstOrDefault();
                        NeighborhoodsCode = dis.Where(t => t.ID == dl.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                        dl.AddressCoding = CountyCode + NeighborhoodsCode + othercode;
                    }
                }
                else if (type == Enums.TypeInt.Country)
                {
                    var CountyCode = nc.AddressCoding.Substring(0, 2);
                    var NeighborhoodsCode = nc.AddressCoding.Substring(2, 3);
                    var county = dis.Where(t => t.Code == CountyCode).FirstOrDefault();
                    var neighbor = dis.Where(t => t.Code == NeighborhoodsCode).FirstOrDefault();
                    if (county == null || neighbor == null || county.ID != nc.CountyID || neighbor.ID != nc.NeighborhoodsID)
                    {
                        var othercode = nc.AddressCoding.Substring(nc.AddressCoding.Length - 9);
                        CountyCode = dis.Where(t => t.ID == nc.CountyID).Select(t => t.Code).FirstOrDefault();
                        NeighborhoodsCode = dis.Where(t => t.ID == nc.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                        nc.AddressCoding = CountyCode + NeighborhoodsCode + othercode;
                    }
                }
                else if (type == Enums.TypeInt.RP)
                {
                    var CountyCode = rp.AddressCoding.Substring(0, 2);
                    var NeighborhoodsCode = rp.AddressCoding.Substring(2, 3);
                    var county = dis.Where(t => t.Code == CountyCode).FirstOrDefault();
                    var neighbor = dis.Where(t => t.Code == NeighborhoodsCode).FirstOrDefault();
                    if (county == null || neighbor == null || county.ID != rp.CountyID || neighbor.ID != rp.NeighborhoodsID)
                    {
                        var othercode = rp.AddressCoding.Substring(rp.AddressCoding.Length - 7);
                        CountyCode = dis.Where(t => t.ID == rp.CountyID).Select(t => t.Code).FirstOrDefault();
                        NeighborhoodsCode = dis.Where(t => t.ID == rp.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                        rp.AddressCoding = CountyCode + NeighborhoodsCode + othercode;
                    }
                }
                dbContext.SaveChanges();
            }
        }

        public static IQueryable<T> DataFilterWithTown<T>(IQueryable<T> entity) where T : IBaseEntityWithNeighborhoodsID
        {
            if (LoginUtils.CurrentUser.DistrictIDList == null || LoginUtils.CurrentUser.DistrictIDList.Count == 0)
                throw new Exception("该用户没有任何数据权限，请联系管理员！");
            // 先删选出当前用户权限内的数据
            if (!LoginUtils.CurrentUser.DistrictIDList.Contains("嘉兴市"))
            {
                var where = PredicateBuilder.False<T>();
                foreach (var userDID in LoginUtils.CurrentUser.DistrictIDList)
                {
                    where = where.Or(t => t.NeighborhoodsID.IndexOf(userDID + ".") == 0 || t.NeighborhoodsID == userDID);
                }
                entity = entity.Where(where);
            }
            return entity;
        }

        public static IQueryable<T> DataFilterWithDist<T>(IQueryable<T> entity) where T : IBaseEntityWitDistrictID
        {
            if (LoginUtils.CurrentUser.DistrictIDList == null || LoginUtils.CurrentUser.DistrictIDList.Count == 0)
                throw new Exception("该用户没有任何数据权限，请联系管理员！");
            // 先删选出当前用户权限内的数据
            if (!LoginUtils.CurrentUser.DistrictIDList.Contains("嘉兴市"))
            {
                var where = PredicateBuilder.False<T>();
                foreach (var userDID in LoginUtils.CurrentUser.DistrictIDList)
                {
                    where = where.Or(t => t.DistrictID.IndexOf(userDID + ".") == 0 || t.DistrictID == userDID);
                }
                entity = entity.Where(where);
            }
            return entity;
        }

    }
}