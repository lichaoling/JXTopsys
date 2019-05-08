//using iTextSharp.text.pdf.parser;
using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Business.MPModify;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business.Schedule
{
    public class Items
    {
        public int MP { get; set; }
        public int DMHZ { get; set; }
        public int DMZM { get; set; }
        public int CJYJ { get; set; }
        public int MP_YC { get; set; }
        public int MP_ZXSB { get; set; }
        public int MP_ZLB { get; set; }

        public int DMHZ_YC { get; set; }
        public int DMHZ_ZXSB { get; set; }
        public int DMHZ_ZLB { get; set; }

        public int DMZM_YC { get; set; }
        public int DMZM_ZXSB { get; set; }
        public int DMZM_ZLB { get; set; }

        public int CJYJ_YC { get; set; }
        public int CJYJ_ZXSB { get; set; }
        public int CJYJ_ZLB { get; set; }


    }
    public class HomePage
    {
        public static Dictionary<string, object> GetHomePageTotalData()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                if (LoginUtils.CurrentUser.DistrictIDList == null || LoginUtils.CurrentUser.DistrictIDList.Count == 0)
                    throw new Error("该用户没有任何数据权限，请联系管理员！");

                string mpsql_yc = "", mpsql_zxsb = "", mpsql_zlb = "", dmhzsql_yc = "", dmhzsql_zxsb = "", dmhzsql_zlb = "", dmzmsql_yc = "", dmzmsql_zxsb = "", dmzmsql_zlb = "", cjyjsql_yc = "", cjyjsql_zxsb = "", cjyjsql_zlb = "";
                string mpsql1_yc = "", mpsql1_zxsb = "", mpsql1_zlb = "", dmhzsql1_yc = "", dmhzsql1_zxsb = "", dmhzsql1_zlb = "", dmzmsql1_yc = "", dmzmsql1_zxsb = "", dmzmsql1_zlb = "", cjyjsql1_yc = "", cjyjsql1_zxsb = "", cjyjsql1_zlb = "";

                //string mpsql3 = "", dmhzsql3 = "", dmzmsql3 = "", cjyjsql3 = "";
                // 先删选出当前用户权限内的数据
                if (!LoginUtils.CurrentUser.DistrictIDList.Contains("嘉兴市"))
                {
                    foreach (var userDID in LoginUtils.CurrentUser.DistrictIDList)
                    {
                        #region 待办事项
                        mpsql_yc += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                left  join MPOFCOUNTRY a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}'
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                left  join MPOFRESIDENCE a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}'
                                union 
                                select b.ID from BG_MPOFROAD b 
                                left  join MPOFROAD a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}'
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}'
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}'
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}' ";
                        mpsql_zxsb += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                left  join MPOFCOUNTRY a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}'
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                left  join MPOFRESIDENCE a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}'
                                union 
                                select b.ID from BG_MPOFROAD b 
                                left  join MPOFROAD a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}'
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}'
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}'
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}' ";
                        mpsql_zlb += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                left  join MPOFCOUNTRY a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}'
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                left  join MPOFRESIDENCE a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}'
                                union 
                                select b.ID from BG_MPOFROAD b 
                                left  join MPOFROAD a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}'
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}'
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}'
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}' ";
                        dmhzsql_yc += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.yc}' ";
                        dmhzsql_zxsb += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.zxsb}' ";
                        dmhzsql_zlb += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.zlb}' ";

                        dmzmsql_yc += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.yc}' ";
                        dmzmsql_zxsb += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.zxsb}' ";
                        dmzmsql_zlb += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.zlb}' ";
                        cjyjsql_yc += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.yc}' ";
                        cjyjsql_zxsb += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.zxsb}' ";
                        cjyjsql_zlb += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.zlb}' ";

                        #endregion
                    }
                }
                else
                {
                    #region 待办事项
                    mpsql_yc += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}'
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}'
                                union 
                                select b.ID from BG_MPOFROAD b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}'
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}'
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}'
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' ";
                    mpsql_zxsb += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}'
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}'
                                union 
                                select b.ID from BG_MPOFROAD b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}'
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}'
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}'
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' ";
                    mpsql_zlb += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}'
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}'
                                union 
                                select b.ID from BG_MPOFROAD b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}'
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}'
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}'
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' ";
                    dmhzsql_yc += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' ";
                    dmhzsql_zxsb += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' ";
                    dmhzsql_zlb += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' ";

                    dmzmsql_yc += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' ";
                    dmzmsql_zxsb += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' ";
                    dmzmsql_zlb += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' ";

                    cjyjsql_yc += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' ";
                    cjyjsql_zxsb += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' ";
                    cjyjsql_zlb += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' ";

                    #endregion
                }
                #region 已办事项
                mpsql1_yc += $@"union
                                select b.ID from BG_MPOFCOUNTRY b
                                where b.IsFinish = 1 and b.sbly = '{Enums.SBLY.yc}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                union
                                select b.ID from BG_MPOFRESIDENCE b
                                where b.IsFinish = 1 and b.sbly = '{Enums.SBLY.yc}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                union
                                select b.ID from BG_MPOFROAD b
                                where b.IsFinish = 1 and b.sbly = '{Enums.SBLY.yc}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                union
                                select b.ID from SB_MPOFCOUNTRY b
                                where b.IsFinish = 1 and b.sbly = '{Enums.SBLY.yc}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                union
                                select b.ID from SB_MPOFRESIDENCE b
                                where b.IsFinish = 1 and b.sbly = '{Enums.SBLY.yc}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                union
                                select b.ID from SB_MPOFROAD b
                                where b.IsFinish = 1 and b.sbly = '{Enums.SBLY.yc}' and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";
                mpsql1_zxsb += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                where b.IsFinish=1 and b.sbly='{Enums.SBLY.zxsb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                where b.IsFinish=1 and b.sbly='{Enums.SBLY.zxsb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                union 
                                select b.ID from BG_MPOFROAD b 
                                where b.IsFinish=1 and b.sbly='{Enums.SBLY.zxsb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=1 and b.sbly='{Enums.SBLY.zxsb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=1 and b.sbly='{Enums.SBLY.zxsb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=1 and b.sbly='{Enums.SBLY.zxsb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";
                mpsql1_zlb += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                where b.IsFinish=1 and b.sbly='{Enums.SBLY.zlb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                where b.IsFinish=1 and b.sbly='{Enums.SBLY.zlb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                union 
                                select b.ID from BG_MPOFROAD b 
                                where b.IsFinish=1 and b.sbly='{Enums.SBLY.zlb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=1 and b.sbly='{Enums.SBLY.zlb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=1 and b.sbly='{Enums.SBLY.zlb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=1 and b.sbly='{Enums.SBLY.zlb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";
                dmhzsql1_yc += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.yc}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.yc}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.yc}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.yc}' and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";
                dmhzsql1_zxsb += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.zxsb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.zxsb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.zxsb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.zxsb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";
                dmhzsql1_zlb += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.zlb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.zlb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.zlb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.zlb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";

                dmzmsql1_yc += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.yc}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.yc}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.yc}' and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";
                dmzmsql1_zxsb += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.zxsb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.zxsb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";
                dmzmsql1_zlb += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.zlb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.zlb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.zlb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";

                cjyjsql1_yc += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.yc}' and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";
                cjyjsql1_zxsb += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.zxsb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";
                cjyjsql1_zlb += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=1 and b.sbly='{Enums.SBLY.zlb}' and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";
                #endregion
                #region 个人已办事项
                //mpsql3 += $@"union 
                //                select b.ID from BG_MPOFCOUNTRY b 
                //                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                union 
                //                select b.ID from BG_MPOFRESIDENCE b 
                //                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                union 
                //                select b.ID from BG_MPOFROAD b 
                //                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                union 
                //                select b.ID from SB_MPOFCOUNTRY b 
                //                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                union 
                //                select b.ID from SB_MPOFRESIDENCE b 
                //                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                union 
                //                select b.ID from SB_MPOFROAD b 
                //                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";

                //dmhzsql3 += $@"union 
                //                   select b.ID from SB_DMOFBRIDGE b 
                //                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                   union 
                //                   select b.ID from SB_DMOFBUILDING b 
                //                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                   union 
                //                   select b.ID from SB_DMOFROAD b 
                //                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                   union 
                //                   select b.ID from SB_DMOFSETTLEMENT b 
                //                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";

                //dmzmsql3 += $@"union 
                //                   select b.ID from ZM_MPOFCOUNTRY b 
                //                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                   union 
                //                   select b.ID from ZM_MPOFRESIDENCE b 
                //                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                   union 
                //                   select b.ID from ZM_MPOFROAD b 
                //                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";

                //cjyjsql3 += $@"union 
                //                   select b.ID from BA_DMOFZYSS b 
                //                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";
                #endregion

                mpsql_yc = mpsql_yc.Substring("union ".Length);
                mpsql_zxsb = mpsql_zxsb.Substring("union ".Length);
                mpsql_zlb = mpsql_zlb.Substring("union ".Length);
                dmhzsql_yc = dmhzsql_yc.Substring("union ".Length);
                dmhzsql_zxsb = dmhzsql_zxsb.Substring("union ".Length);
                dmhzsql_zlb = dmhzsql_zlb.Substring("union ".Length);
                dmzmsql_yc = dmzmsql_yc.Substring("union ".Length);
                dmzmsql_zxsb = dmzmsql_zxsb.Substring("union ".Length);
                dmzmsql_zlb = dmzmsql_zlb.Substring("union ".Length);
                cjyjsql_yc = cjyjsql_yc.Substring("union ".Length);
                cjyjsql_zxsb = cjyjsql_zxsb.Substring("union ".Length);
                cjyjsql_zlb = cjyjsql_zlb.Substring("union ".Length);

                mpsql1_yc = mpsql1_yc.Substring("union ".Length);
                mpsql1_zxsb = mpsql1_zxsb.Substring("union ".Length);
                mpsql1_zlb = mpsql1_zlb.Substring("union ".Length);
                dmhzsql1_yc = dmhzsql1_yc.Substring("union ".Length);
                dmhzsql1_zxsb = dmhzsql1_zxsb.Substring("union ".Length);
                dmhzsql1_zlb = dmhzsql1_zlb.Substring("union ".Length);
                dmzmsql1_yc = dmzmsql1_yc.Substring("union ".Length);
                dmzmsql1_zxsb = dmzmsql1_zxsb.Substring("union ".Length);
                dmzmsql1_zlb = dmzmsql1_zlb.Substring("union ".Length);
                cjyjsql1_yc = cjyjsql1_yc.Substring("union ".Length);
                cjyjsql1_zxsb = cjyjsql1_zxsb.Substring("union ".Length);
                cjyjsql1_zlb = cjyjsql1_zlb.Substring("union ".Length);

                //mpsql3 = mpsql3.Substring("union ".Length);
                //dmhzsql3 = dmhzsql3.Substring("union ".Length);
                //dmzmsql3 = dmzmsql3.Substring("union ".Length);
                //cjyjsql3 = cjyjsql3.Substring("union ".Length);

                Items toDoItem = new Items();
                toDoItem.MP_YC = dbContext.Database.SqlQuery<int>($"select count(1) from ({mpsql_yc}) s").FirstOrDefault();
                toDoItem.MP_ZXSB = dbContext.Database.SqlQuery<int>($"select count(1) from ({mpsql_zxsb}) s").FirstOrDefault();
                toDoItem.MP_ZLB = dbContext.Database.SqlQuery<int>($"select count(1) from ({mpsql_zlb}) s").FirstOrDefault();

                toDoItem.DMHZ_YC = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmhzsql_yc}) s").FirstOrDefault();
                toDoItem.DMHZ_ZXSB = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmhzsql_zxsb}) s").FirstOrDefault();
                toDoItem.DMHZ_ZLB = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmhzsql_zlb}) s").FirstOrDefault();

                toDoItem.DMZM_YC = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmzmsql_yc}) s").FirstOrDefault();
                toDoItem.DMZM_ZXSB = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmzmsql_zxsb}) s").FirstOrDefault();
                toDoItem.DMZM_ZLB = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmzmsql_zlb}) s").FirstOrDefault();

                toDoItem.CJYJ_YC = dbContext.Database.SqlQuery<int>($"select count(1) from ({cjyjsql_yc}) s").FirstOrDefault();
                toDoItem.CJYJ_ZXSB = dbContext.Database.SqlQuery<int>($"select count(1) from ({cjyjsql_zxsb}) s").FirstOrDefault();
                toDoItem.CJYJ_ZLB = dbContext.Database.SqlQuery<int>($"select count(1) from ({cjyjsql_zlb}) s").FirstOrDefault();


                Items doneItem = new Items();
                doneItem.MP_YC = dbContext.Database.SqlQuery<int>($"select count(1) from ({mpsql1_yc}) s").FirstOrDefault();
                doneItem.MP_ZXSB = dbContext.Database.SqlQuery<int>($"select count(1) from ({mpsql1_zxsb}) s").FirstOrDefault();
                doneItem.MP_ZLB = dbContext.Database.SqlQuery<int>($"select count(1) from ({mpsql1_zlb}) s").FirstOrDefault();

                doneItem.DMHZ_YC = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmhzsql1_yc}) s").FirstOrDefault();
                doneItem.DMHZ_ZXSB = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmhzsql1_zxsb}) s").FirstOrDefault();
                doneItem.DMHZ_ZLB = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmhzsql1_zlb}) s").FirstOrDefault();

                doneItem.DMZM_YC = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmzmsql1_yc}) s").FirstOrDefault();
                doneItem.DMZM_ZXSB = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmzmsql1_zxsb}) s").FirstOrDefault();
                doneItem.DMZM_ZLB = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmzmsql1_zlb}) s").FirstOrDefault();

                doneItem.CJYJ_YC = dbContext.Database.SqlQuery<int>($"select count(1) from ({cjyjsql1_yc}) s").FirstOrDefault();
                doneItem.CJYJ_ZXSB = dbContext.Database.SqlQuery<int>($"select count(1) from ({cjyjsql1_zxsb}) s").FirstOrDefault();
                doneItem.CJYJ_ZLB = dbContext.Database.SqlQuery<int>($"select count(1) from ({cjyjsql1_zlb}) s").FirstOrDefault();

                //Items PersonalDoneItem = new Items();
                //PersonalDoneItem.MP = dbContext.Database.SqlQuery<int>($"select count(1) from ({mpsql3}) s").FirstOrDefault();
                //PersonalDoneItem.DMHZ = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmhzsql3}) s").FirstOrDefault();
                //PersonalDoneItem.DMZM = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmzmsql3}) s").FirstOrDefault();
                //PersonalDoneItem.CJYJ = dbContext.Database.SqlQuery<int>($"select count(1) from ({cjyjsql3}) s").FirstOrDefault();

                return new Dictionary<string, object> {
                   { "toDoItem",toDoItem},
                   { "doneItem",doneItem},
                   //{ "PersonalDoneItem",PersonalDoneItem}
                };
            }
        }
        public static Dictionary<string, object> GetHomePageDetailData(DateTime start, DateTime end)
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                if (LoginUtils.CurrentUser.DistrictIDList == null || LoginUtils.CurrentUser.DistrictIDList.Count == 0)
                    throw new Error("该用户没有任何数据权限，请联系管理员！");

                string mpsql_yc = "", mpsql_zxsb = "", mpsql_zlb = "", dmhzsql_yc = "", dmhzsql_zxsb = "", dmhzsql_zlb = "", dmzmsql_yc = "", dmzmsql_zxsb = "", dmzmsql_zlb = "", cjyjsql_yc = "", cjyjsql_zxsb = "", cjyjsql_zlb = "";
                string mpsql1_yc = "", mpsql1_zxsb = "", mpsql1_zlb = "", dmhzsql1_yc = "", dmhzsql1_zxsb = "", dmhzsql1_zlb = "", dmzmsql1_yc = "", dmzmsql1_zxsb = "", dmzmsql1_zlb = "", cjyjsql1_yc = "", cjyjsql1_zxsb = "", cjyjsql1_zlb = "";

                // 先删选出当前用户权限内的数据
                if (!LoginUtils.CurrentUser.DistrictIDList.Contains("嘉兴市"))
                {
                    foreach (var userDID in LoginUtils.CurrentUser.DistrictIDList)
                    {
                        #region 待办事项
                        mpsql_yc += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                left  join MPOFCOUNTRY a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                left  join MPOFRESIDENCE a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFROAD b 
                                left  join MPOFROAD a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                        mpsql_zxsb += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                left  join MPOFCOUNTRY a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                left  join MPOFRESIDENCE a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFROAD b 
                                left  join MPOFROAD a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                        mpsql_zlb += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                left  join MPOFCOUNTRY a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                left  join MPOFRESIDENCE a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFROAD b 
                                left  join MPOFROAD a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                        dmhzsql_yc += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                        dmhzsql_zxsb += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                        dmhzsql_zlb += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";

                        dmzmsql_yc += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                        dmzmsql_zxsb += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                        dmzmsql_zlb += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                        cjyjsql_yc += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                        cjyjsql_zxsb += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                        cjyjsql_zlb += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";

                        #endregion
                    }
                }
                else
                {
                    #region 待办事项
                    mpsql_yc += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFROAD b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                    mpsql_zxsb += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFROAD b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                    mpsql_zlb += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFROAD b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                    dmhzsql_yc += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                    dmhzsql_zxsb += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                    dmhzsql_zlb += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";

                    dmzmsql_yc += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                    dmzmsql_zxsb += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                    dmzmsql_zlb += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";

                    cjyjsql_yc += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                    cjyjsql_zxsb += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                    cjyjsql_zlb += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=0 and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";

                    #endregion
                }
                #region 已办事项
                mpsql1_yc += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                left  join MPOFCOUNTRY a on b.MPID=a.ID 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                left  join MPOFRESIDENCE a on b.MPID=a.ID 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFROAD b 
                                left  join MPOFROAD a on b.MPID=a.ID 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                mpsql1_zxsb += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                left  join MPOFCOUNTRY a on b.MPID=a.ID 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                left  join MPOFRESIDENCE a on b.MPID=a.ID 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFROAD b 
                                left  join MPOFROAD a on b.MPID=a.ID 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                mpsql1_zlb += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                left  join MPOFCOUNTRY a on b.MPID=a.ID 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                left  join MPOFRESIDENCE a on b.MPID=a.ID 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from BG_MPOFROAD b 
                                left  join MPOFROAD a on b.MPID=a.ID 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";


                dmhzsql1_yc += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                dmhzsql1_zxsb += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                dmhzsql1_zlb += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";


                dmzmsql1_yc += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                dmzmsql1_zxsb += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                dmzmsql1_zlb += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";


                cjyjsql1_yc += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.yc}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                cjyjsql1_zxsb += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zxsb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                cjyjsql1_zlb += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{Enums.SBLY.zlb}' and b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";
                #endregion

                #region 个人已办事项
                //mpsql3 += $@"union 
                //                select b.ID from BG_MPOFCOUNTRY b 
                //                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                union 
                //                select b.ID from BG_MPOFRESIDENCE b 
                //                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                union 
                //                select b.ID from BG_MPOFROAD b 
                //                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                union 
                //                select b.ID from SB_MPOFCOUNTRY b 
                //                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                union 
                //                select b.ID from SB_MPOFRESIDENCE b 
                //                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                union 
                //                select b.ID from SB_MPOFROAD b 
                //                where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";

                //dmhzsql3 += $@"union 
                //                   select b.ID from SB_DMOFBRIDGE b 
                //                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                   union 
                //                   select b.ID from SB_DMOFBUILDING b 
                //                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                   union 
                //                   select b.ID from SB_DMOFROAD b 
                //                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                   union 
                //                   select b.ID from SB_DMOFSETTLEMENT b 
                //                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";

                //dmzmsql3 += $@"union 
                //                   select b.ID from ZM_MPOFCOUNTRY b 
                //                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                   union 
                //                   select b.ID from ZM_MPOFRESIDENCE b 
                //                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}'
                //                   union 
                //                   select b.ID from ZM_MPOFROAD b 
                //                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";

                //cjyjsql3 += $@"union 
                //                   select b.ID from BA_DMOFZYSS b 
                //                   where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' ";
                #endregion

                mpsql_yc = mpsql_yc.Substring("union ".Length);
                mpsql_zxsb = mpsql_zxsb.Substring("union ".Length);
                mpsql_zlb = mpsql_zlb.Substring("union ".Length);
                dmhzsql_yc = dmhzsql_yc.Substring("union ".Length);
                dmhzsql_zxsb = dmhzsql_zxsb.Substring("union ".Length);
                dmhzsql_zlb = dmhzsql_zlb.Substring("union ".Length);
                dmzmsql_yc = dmzmsql_yc.Substring("union ".Length);
                dmzmsql_zxsb = dmzmsql_zxsb.Substring("union ".Length);
                dmzmsql_zlb = dmzmsql_zlb.Substring("union ".Length);
                cjyjsql_yc = cjyjsql_yc.Substring("union ".Length);
                cjyjsql_zxsb = cjyjsql_zxsb.Substring("union ".Length);
                cjyjsql_zlb = cjyjsql_zlb.Substring("union ".Length);

                mpsql1_yc = mpsql1_yc.Substring("union ".Length);
                mpsql1_zxsb = mpsql1_zxsb.Substring("union ".Length);
                mpsql1_zlb = mpsql1_zlb.Substring("union ".Length);
                dmhzsql1_yc = dmhzsql1_yc.Substring("union ".Length);
                dmhzsql1_zxsb = dmhzsql1_zxsb.Substring("union ".Length);
                dmhzsql1_zlb = dmhzsql1_zlb.Substring("union ".Length);
                dmzmsql1_yc = dmzmsql1_yc.Substring("union ".Length);
                dmzmsql1_zxsb = dmzmsql1_zxsb.Substring("union ".Length);
                dmzmsql1_zlb = dmzmsql1_zlb.Substring("union ".Length);
                cjyjsql1_yc = cjyjsql1_yc.Substring("union ".Length);
                cjyjsql1_zxsb = cjyjsql1_zxsb.Substring("union ".Length);
                cjyjsql1_zlb = cjyjsql1_zlb.Substring("union ".Length);


                Items toDoItem = new Items();
                toDoItem.MP_YC = dbContext.Database.SqlQuery<int>($"select count(1) from ({mpsql_yc}) s").FirstOrDefault();
                toDoItem.MP_ZXSB = dbContext.Database.SqlQuery<int>($"select count(1) from ({mpsql_zxsb}) s").FirstOrDefault();
                toDoItem.MP_ZLB = dbContext.Database.SqlQuery<int>($"select count(1) from ({mpsql_zlb}) s").FirstOrDefault();

                toDoItem.DMHZ_YC = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmhzsql_yc}) s").FirstOrDefault();
                toDoItem.DMHZ_ZXSB = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmhzsql_zxsb}) s").FirstOrDefault();
                toDoItem.DMHZ_ZLB = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmhzsql_zlb}) s").FirstOrDefault();

                toDoItem.DMZM_YC = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmzmsql_yc}) s").FirstOrDefault();
                toDoItem.DMZM_ZXSB = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmzmsql_zxsb}) s").FirstOrDefault();
                toDoItem.DMZM_ZLB = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmzmsql_zlb}) s").FirstOrDefault();

                toDoItem.CJYJ_YC = dbContext.Database.SqlQuery<int>($"select count(1) from ({cjyjsql_yc}) s").FirstOrDefault();
                toDoItem.CJYJ_ZXSB = dbContext.Database.SqlQuery<int>($"select count(1) from ({cjyjsql_zxsb}) s").FirstOrDefault();
                toDoItem.CJYJ_ZLB = dbContext.Database.SqlQuery<int>($"select count(1) from ({cjyjsql_zlb}) s").FirstOrDefault();


                Items doneItem = new Items();
                doneItem.MP_YC = dbContext.Database.SqlQuery<int>($"select count(1) from ({mpsql1_yc}) s").FirstOrDefault();
                doneItem.MP_ZXSB = dbContext.Database.SqlQuery<int>($"select count(1) from ({mpsql1_zxsb}) s").FirstOrDefault();
                doneItem.MP_ZLB = dbContext.Database.SqlQuery<int>($"select count(1) from ({mpsql1_zlb}) s").FirstOrDefault();

                doneItem.DMHZ_YC = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmhzsql1_yc}) s").FirstOrDefault();
                doneItem.DMHZ_ZXSB = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmhzsql1_zxsb}) s").FirstOrDefault();
                doneItem.DMHZ_ZLB = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmhzsql1_zlb}) s").FirstOrDefault();

                doneItem.DMZM_YC = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmzmsql1_yc}) s").FirstOrDefault();
                doneItem.DMZM_ZXSB = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmzmsql1_zxsb}) s").FirstOrDefault();
                doneItem.DMZM_ZLB = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmzmsql1_zlb}) s").FirstOrDefault();

                doneItem.CJYJ_YC = dbContext.Database.SqlQuery<int>($"select count(1) from ({cjyjsql1_yc}) s").FirstOrDefault();
                doneItem.CJYJ_ZXSB = dbContext.Database.SqlQuery<int>($"select count(1) from ({cjyjsql1_zxsb}) s").FirstOrDefault();
                doneItem.CJYJ_ZLB = dbContext.Database.SqlQuery<int>($"select count(1) from ({cjyjsql1_zlb}) s").FirstOrDefault();


                Items totalDoneItem = new Items();
                totalDoneItem.MP = doneItem.MP_YC + doneItem.MP_ZXSB + doneItem.MP_ZLB;
                totalDoneItem.DMHZ = doneItem.DMHZ_YC + doneItem.DMHZ_ZXSB + doneItem.DMHZ_ZLB;
                totalDoneItem.DMZM = doneItem.DMZM_YC + doneItem.DMZM_ZXSB + doneItem.DMZM_ZLB;
                totalDoneItem.CJYJ = doneItem.CJYJ_YC + doneItem.CJYJ_ZXSB + doneItem.CJYJ_ZLB;

                Items totalTodoItem = new Items();
                totalTodoItem.MP = toDoItem.MP_YC + toDoItem.MP_ZXSB + toDoItem.MP_ZLB;
                totalTodoItem.DMHZ = toDoItem.DMHZ_YC + toDoItem.DMHZ_ZXSB + toDoItem.DMHZ_ZLB;
                totalTodoItem.DMZM = toDoItem.DMZM_YC + toDoItem.DMZM_ZXSB + toDoItem.DMZM_ZLB;
                totalTodoItem.CJYJ = toDoItem.CJYJ_YC + toDoItem.CJYJ_ZXSB + toDoItem.CJYJ_ZLB;


                return new Dictionary<string, object> {
                   { "toDoItem",toDoItem},
                   { "doneItem",doneItem},
                   { "totalDoneItem",totalDoneItem},
                   { "totalTodoItem",totalTodoItem},
                };
            }
        }
        public static Dictionary<string, object> GetTodoItems(int pageNum, int pageSize, string sbly, string lx)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                string mpsql1 = "", dmhzsql1 = "", dmzmsql1 = "", cjyjsql1 = "";
                string mpsql_c = "", dmhzsql_c = "", dmzmsql_c = "", cjyjsql_c = "";
                string mpsql_d = "", dmhzsql_d = "", dmzmsql_d = "", cjyjsql_d = "";
                if (!LoginUtils.CurrentUser.DistrictIDList.Contains("嘉兴市"))
                {
                    foreach (var userDID in LoginUtils.CurrentUser.DistrictIDList)
                    {
                        #region 待办事项
                        mpsql1 += $@"union 
                                select b.ID,b.sbly,'门牌变更' YWLX,'农村门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BG_NC}' sign,b.Opinion from BG_MPOFCOUNTRY b 
                                left  join MPOFCOUNTRY a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{sbly}'
                                union 
                                select b.ID,b.sbly,'门牌变更' YWLX,'住宅门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BG_ZZ}' sign,b.Opinion from BG_MPOFRESIDENCE b 
                                left  join MPOFRESIDENCE a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{sbly}'
                                union 
                                select b.ID,b.sbly,'门牌变更' YWLX,'道路门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BG_DL}' sign,b.Opinion from BG_MPOFROAD b 
                                left  join MPOFROAD a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1 and b.sbly='{sbly}'
                                union 
                                select b.ID,b.sbly,'门牌申请' YWLX,'农村门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BZ_NC}' sign,b.Opinion from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{sbly}'
                                union 
                                select b.ID,b.sbly,'门牌申请' YWLX,'住宅门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BZ_ZZ}' sign,b.Opinion from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{sbly}'
                                union 
                                select b.ID,b.sbly,'门牌申请' YWLX,'道路门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BZ_DL}' sign,b.Opinion from SB_MPOFROAD b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{sbly}' ";

                        dmhzsql1 += $@"union 
                                   select b.ID,b.sbly,'桥梁' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.DMHZ_QL}' sign,b.Opinion from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{sbly}'
                                   union 
                                   select b.ID,b.sbly,'建筑物' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.DMHZ_JZW}' sign,b.Opinion from SB_DMOFBUILDING b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{sbly}'
                                   union 
                                   select b.ID,b.sbly,'道路街巷' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.DMHZ_DLJX}' sign,b.Opinion from SB_DMOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{sbly}'
                                   union 
                                   select b.ID,b.sbly,'居民点' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.DMHZ_JMD}' sign,b.Opinion from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{sbly}' ";

                        dmzmsql1 += $@"union 
                                   select b.ID,b.sbly,'农村门牌' MPLX,b.VillageName MC,b.MPNumber HM,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.DMZM_NC}' sign,b.Opinion from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{sbly}'
                                   union 
                                   select b.ID,b.sbly,'住宅门牌' MPLX,b.ResidenceName MC,'' HM,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.DMZM_ZZ}' sign,b.Opinion  from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 and b.sbly='{sbly}'
                                   union 
                                   select b.ID,b.sbly,'道路门牌' MPLX,b.RoadName MC,b.MPNumber HM,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.DMZM_DL}' sign,b.Opinion  from ZM_MPOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{sbly}' ";

                        cjyjsql1 += $@"union 
                                   select b.ID,b.sbly,b.ZYSSType LB,b.SmallType DMLB,b.DMType XLLB,b.Name MC,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.CJYJ_ZYSS}' sign,b.Opinion from BA_DMOFZYSS b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1  and b.sbly='{sbly}' ";

                        #endregion
                    }
                }
                else
                {
                    #region 待办事项
                    mpsql1 += $@"union 
                                select b.ID,b.sbly,'门牌变更' YWLX,'农村门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BG_NC}' sign,b.Opinion from BG_MPOFCOUNTRY b 
                                where b.IsFinish=0 and b.sbly='{sbly}'                                                 
                                union                                                                                  
                                select b.ID,b.sbly,'门牌变更' YWLX,'住宅门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BG_ZZ}' sign,b.Opinion from BG_MPOFRESIDENCE b 
                                where b.IsFinish=0 and b.sbly='{sbly}'                                                 
                                union                                                                                       
                                select b.ID,b.sbly,'门牌变更' YWLX,'道路门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BG_DL}' sign,b.Opinion from BG_MPOFROAD b 
                                where b.IsFinish=0 and b.sbly='{sbly}'                                                
                                union                                                                                 
                                select b.ID,b.sbly,'门牌申请' YWLX,'农村门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BZ_NC}' sign,b.Opinion from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0 and b.sbly='{sbly}'                                                
                                union                                                                                 
                                select b.ID,b.sbly,'门牌申请' YWLX,'住宅门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BZ_ZZ}' sign,b.Opinion from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0 and b.sbly='{sbly}'                                                  
                                union                                                                                   
                                select b.ID,b.sbly,'门牌申请' YWLX,'道路门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BZ_DL}' sign,b.Opinion from SB_MPOFROAD b 
                                where b.IsFinish=0 and b.sbly='{sbly}' ";

                    dmhzsql1 += $@"union 
                                   select b.ID,b.sbly,'桥梁' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.DMHZ_QL}' sign,b.Opinion from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0 and b.sbly='{sbly}'
                                   union 
                                   select b.ID,b.sbly,'建筑物' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.DMHZ_JZW}' sign,b.Opinion from SB_DMOFBUILDING b 
                                   where b.IsFinish=0 and b.sbly='{sbly}'
                                   union 
                                   select b.ID,b.sbly,'道路街巷' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.DMHZ_DLJX}' sign,b.Opinion from SB_DMOFROAD b 
                                   where b.IsFinish=0 and b.sbly='{sbly}'
                                   union 
                                   select b.ID,b.sbly,'居民点' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.DMHZ_JMD}' sign,b.Opinion from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0 and b.sbly='{sbly}' ";

                    dmzmsql1 += $@"union 
                                   select b.ID,b.sbly,'农村门牌' MPLX,b.VillageName MC,b.MPNumber HM,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.DMZM_NC}' sign,b.Opinion  from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0 and b.sbly='{sbly}'
                                   union 
                                   select b.ID,b.sbly,'住宅门牌' MPLX,b.ResidenceName MC,'' HM,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.DMZM_ZZ}' sign,b.Opinion  from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0 and b.sbly='{sbly}'
                                   union 
                                   select b.ID,b.sbly,'道路门牌' MPLX,b.RoadName MC,b.MPNumber HM,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.DMZM_DL}' sign,b.Opinion from ZM_MPOFROAD b 
                                   where b.IsFinish=0 and b.sbly='{sbly}' ";

                    cjyjsql1 += $@"union 
                                   select b.ID,b.sbly,b.ZYSSType LB,b.SmallType DMLB,b.DMType XLLB,b.Name MC,b.createtime SQSJ,'{Enums.SPFileBusinessTypes.CJYJ_ZYSS}' sign,b.Opinion from BA_DMOFZYSS b 
                                   where b.IsFinish=0 and b.sbly='{sbly}' ";

                    #endregion
                }
                mpsql1 = mpsql1.Substring("union ".Length);
                dmhzsql1 = dmhzsql1.Substring("union ".Length);
                dmzmsql1 = dmzmsql1.Substring("union ".Length);
                cjyjsql1 = cjyjsql1.Substring("union ".Length);

                mpsql_c = $@"select count(1) from ({mpsql1}) as a";
                dmhzsql_c = $@"select count(1) from ({dmhzsql1}) as a";
                dmzmsql_c = $@"select count(1) from ({dmzmsql1}) as a";
                cjyjsql_c = $@"select count(1) from ({cjyjsql1}) as a";

                mpsql_d = $@"select * from ( 
　　　　select *, ROW_NUMBER() OVER(Order by sbly) AS RowId from ({mpsql1}) as b) as c
      where RowId between ({pageNum} - 1) * {pageSize} and {pageNum} * {pageSize} ";
                dmhzsql_d = $@"select * from ( 
　　　　select *, ROW_NUMBER() OVER(Order by sbly) AS RowId from ({dmhzsql1}) as b) as c
      where RowId between ({pageNum} - 1) * {pageSize} and {pageNum} * {pageSize} ";
                dmzmsql_d = $@"select * from ( 
　　　　select *, ROW_NUMBER() OVER(Order by sbly) AS RowId from ({dmzmsql1}) as b) as c
      where RowId between ({pageNum} - 1) * {pageSize} and {pageNum} * {pageSize} ";
                cjyjsql_d = $@"select * from ( 
　　　　select *, ROW_NUMBER() OVER(Order by sbly) AS RowId from ({cjyjsql1}) as b) as c
      where RowId between ({pageNum} - 1) * {pageSize} and {pageNum} * {pageSize} ";

                object data = null;
                int count = 0;
                if (lx == Enums.SXLX.cjyj)
                {
                    data = db.Database.SqlQuery<ZYSSItem>(cjyjsql_d).ToList();
                    count = db.Database.SqlQuery<int>(cjyjsql_c).FirstOrDefault();
                }
                else if (lx == Enums.SXLX.mp)
                {
                    data = db.Database.SqlQuery<MPItem>(mpsql_d).ToList();
                    count = db.Database.SqlQuery<int>(mpsql_c).FirstOrDefault();
                }
                else if (lx == Enums.SXLX.dmhz)
                {
                    data = db.Database.SqlQuery<DMHZItem>(dmhzsql_d).ToList();
                    count = db.Database.SqlQuery<int>(dmhzsql_c).FirstOrDefault();
                }
                else if (lx == Enums.SXLX.dmzm)
                {
                    data = db.Database.SqlQuery<DMZMItem>(dmzmsql_d).ToList();
                    count = db.Database.SqlQuery<int>(dmzmsql_c).FirstOrDefault();
                }

                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };
            }
        }
        public static object GetDoneItems(int pageNum, int pageSize, string sbly, string lx, DateTime start, DateTime end)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                string mpsql1 = "", dmhzsql1 = "", dmzmsql1 = "", cjyjsql1 = "";
                string mpsql_c = "", dmhzsql_c = "", dmzmsql_c = "", cjyjsql_c = "";
                string mpsql_d = "", dmhzsql_d = "", dmzmsql_d = "", cjyjsql_d = "";

                #region 已办事项
                mpsql1 += $@"union 
                        select b.ID,b.sbly,'门牌变更' YWLX,'农村门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,b.FinishTime SPSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BG_NC}' sign,b.Opinion from BG_MPOFCOUNTRY b 
                        where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{sbly}' and b.FinishTime>'{(start).ToString("yyyy/MM/dd")}' and b.FinishTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                        union 
                        select b.ID,b.sbly,'门牌变更' YWLX,'住宅门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,b.FinishTime SPSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BG_NC}' sign,b.Opinion from BG_MPOFRESIDENCE b 
                        where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{sbly}' and b.FinishTime>b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                        union 
                        select b.ID,b.sbly,'门牌变更' YWLX,'道路门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,b.FinishTime SPSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BG_NC}' sign,b.Opinion from BG_MPOFROAD b 
                        where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{sbly}' and b.FinishTime>b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                        union 
                        select b.ID,b.sbly,'门牌申请' YWLX,'农村门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,b.FinishTime SPSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BZ_NC}' sign,b.Opinion from SB_MPOFCOUNTRY b 
                        where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{sbly}' and b.FinishTime>b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                        union 
                        select b.ID,b.sbly,'门牌申请' YWLX,'住宅门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,b.FinishTime SPSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BZ_ZZ}' sign,b.Opinion from SB_MPOFRESIDENCE b 
                        where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{sbly}' and b.FinishTime>b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                        union 
                        select b.ID,b.sbly,'门牌申请' YWLX,'道路门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ,b.FinishTime SPSJ,'{Enums.SPFileBusinessTypes.HFMPZ_BZ_DL}' sign,b.Opinion from SB_MPOFROAD b 
                        where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{sbly}' and b.FinishTime>b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";

                dmhzsql1 += $@"union 
                           select b.ID,b.sbly,'桥梁' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ,b.FinishTime SPSJ,'{Enums.SPFileBusinessTypes.DMHZ_QL}' sign,b.Opinion from SB_DMOFBRIDGE b 
                           where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{sbly}' and b.FinishTime>b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                           union 
                           select b.ID,b.sbly,'建筑物' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ,b.FinishTime SPSJ,'{Enums.SPFileBusinessTypes.DMHZ_JZW}' sign,b.Opinion from SB_DMOFBUILDING b 
                           where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{sbly}' and b.FinishTime>b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                           union 
                           select b.ID,b.sbly,'道路街巷' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ,b.FinishTime SPSJ,'{Enums.SPFileBusinessTypes.DMHZ_DLJX}' sign,b.Opinion from SB_DMOFROAD b 
                           where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{sbly}' and b.FinishTime>b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                           union 
                           select b.ID,b.sbly,'居民点' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ,b.FinishTime SPSJ,'{Enums.SPFileBusinessTypes.DMHZ_JMD}' sign,b.Opinion from SB_DMOFSETTLEMENT b 
                           where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{sbly}' and b.FinishTime>b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";

                dmzmsql1 += $@"union 
                           select b.ID,b.sbly,'农村门牌' MPLX,b.VillageName MC,b.MPNumber HM,b.createtime SQSJ,b.FinishTime SPSJ,'{Enums.SPFileBusinessTypes.DMZM_NC}' sign,b.Opinion from ZM_MPOFCOUNTRY b 
                           where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{sbly}' and b.FinishTime>b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                           union 
                           select b.ID,b.sbly,'住宅门牌' MPLX,b.ResidenceName MC,'' HM,b.createtime SQSJ,b.FinishTime SPSJ,'{Enums.SPFileBusinessTypes.DMZM_ZZ}' sign,b.Opinion from ZM_MPOFRESIDENCE b 
                           where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{sbly}' and b.FinishTime>b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}'
                           union 
                           select b.ID,b.sbly,'道路门牌' MPLX,b.RoadName MC,b.MPNumber HM,b.createtime SQSJ,b.FinishTime SPSJ,'{Enums.SPFileBusinessTypes.DMZM_DL}' sign,b.Opinion from ZM_MPOFROAD b 
                           where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{sbly}' and b.FinishTime>b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";

                cjyjsql1 += $@"union 
                           select b.ID,b.sbly,b.ZYSSType LB,b.SmallType DMLB,b.DMType XLLB,b.Name MC,b.createtime SQSJ,b.FinishTime SPSJ,'{Enums.SPFileBusinessTypes.CJYJ_ZYSS}' sign,b.Opinion from BA_DMOFZYSS b 
                           where b.IsFinish=1 and b.checkuser='{LoginUtils.CurrentUser.UserName}' and b.sbly='{sbly}' and b.FinishTime>b.CreateTime>'{(start).ToString("yyyy/MM/dd")}' and b.CreateTime<'{(end.AddDays(1)).ToString("yyyy/MM/dd")}' ";

                #endregion

                mpsql1 = mpsql1.Substring("union ".Length);
                dmhzsql1 = dmhzsql1.Substring("union ".Length);
                dmzmsql1 = dmzmsql1.Substring("union ".Length);
                cjyjsql1 = cjyjsql1.Substring("union ".Length);

                mpsql_c = $@"select count(1) from ({mpsql1}) as a";
                dmhzsql_c = $@"select count(1) from ({dmhzsql1}) as a";
                dmzmsql_c = $@"select count(1) from ({dmzmsql1}) as a";
                cjyjsql_c = $@"select count(1) from ({cjyjsql1}) as a";

                mpsql_d = $@"select * from ( 
　　　　select *, ROW_NUMBER() OVER(Order by sbly) AS RowId from ({mpsql1}) as b) as c
      where RowId between ({pageNum} - 1) * {pageSize} and {pageNum} * {pageSize} ";
                dmhzsql_d = $@"select * from ( 
　　　　select *, ROW_NUMBER() OVER(Order by sbly) AS RowId from ({dmhzsql1}) as b) as c
      where RowId between ({pageNum} - 1) * {pageSize} and {pageNum} * {pageSize} ";
                dmzmsql_d = $@"select * from ( 
　　　　select *, ROW_NUMBER() OVER(Order by sbly) AS RowId from ({dmzmsql1}) as b) as c
      where RowId between ({pageNum} - 1) * {pageSize} and {pageNum} * {pageSize} ";
                cjyjsql_d = $@"select * from ( 
　　　　select *, ROW_NUMBER() OVER(Order by sbly) AS RowId from ({cjyjsql1}) as b) as c
      where RowId between ({pageNum} - 1) * {pageSize} and {pageNum} * {pageSize} ";

                object data = null;
                int count = 0;
                if (lx == Enums.SXLX.cjyj)
                {
                    data = db.Database.SqlQuery<ZYSSItem>(cjyjsql_d).ToList();
                    count = db.Database.SqlQuery<int>(cjyjsql_c).FirstOrDefault();
                }
                else if (lx == Enums.SXLX.mp)
                {
                    data = db.Database.SqlQuery<MPItem>(mpsql_d).ToList();
                    count = db.Database.SqlQuery<int>(mpsql_c).FirstOrDefault();
                }
                else if (lx == Enums.SXLX.dmhz)
                {
                    data = db.Database.SqlQuery<DMHZItem>(dmhzsql_d).ToList();
                    count = db.Database.SqlQuery<int>(dmhzsql_c).FirstOrDefault();
                }
                else if (lx == Enums.SXLX.dmzm)
                {
                    data = db.Database.SqlQuery<DMZMItem>(dmzmsql_d).ToList();
                    count = db.Database.SqlQuery<int>(dmzmsql_c).FirstOrDefault();
                }

                return new Dictionary<string, object> {
                   { "Data",data},
                   { "Count",count}
                };
            }
        }

        #region 住宅门牌变更
        public static BG_MPOFRESIDENCE GetMPBGOfResidenceInitData(string ID)
        {
            BG_MPOFRESIDENCE entity = null;
            if (!string.IsNullOrEmpty(ID))
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    entity = db.BG_MPOFRESIDENCE.Find(ID);
                    if (entity == null) throw new Error("未能找到指定的数据！");
                    // 查找原门牌
                    if (!string.IsNullOrEmpty(entity.MPID))
                    {
                        entity.O_Entity = db.MPOfResidence.Find(entity.MPID);
                    }
                    entity.fcz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.FCZ);
                    entity.tdz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.TDZ);
                    entity.bdcz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.BDCZ);
                    entity.hj = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.HJ);

                }
            }
            return entity;
        }
        public static void ModifyMPBGOfResidence(string ID, string Json)
        {
            BG_MPOFRESIDENCE targetData = null;
            using (var db = SystemUtils.NewEFDbContext)
            {
                targetData = db.BG_MPOFRESIDENCE.Find(ID);
                if (targetData != null)
                {
                    ObjectReflection.ModifyEntity<BG_MPOFRESIDENCE>(targetData, Json);

                    if (targetData.CertificateType == Enums.SPFileCertificateTypes.FCZ)
                    {

                        var fcz = System.Web.HttpContext.Current.Request.Files.GetMultiple("fcz");
                        foreach (var f in fcz)
                        {
                            HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                            SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BG_ZZ, targetData.ID, Enums.SPFileCertificateTypes.FCZ);
                        }
                    }

                    if (targetData.CertificateType == Enums.SPFileCertificateTypes.TDZ)
                    {
                        var tdz = System.Web.HttpContext.Current.Request.Files.GetMultiple("tdz");
                        foreach (var f in tdz)
                        {
                            HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                            SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BG_ZZ, targetData.ID, Enums.SPFileCertificateTypes.TDZ);
                        }
                    }

                    if (targetData.CertificateType == Enums.SPFileCertificateTypes.BDCZ)
                    {
                        var bdcz = System.Web.HttpContext.Current.Request.Files.GetMultiple("bdcz");
                        foreach (var f in bdcz)
                        {
                            HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                            SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BG_ZZ, targetData.ID, Enums.SPFileCertificateTypes.BDCZ);
                        }
                    }

                    if (targetData.CertificateType == Enums.SPFileCertificateTypes.HJ)
                    {
                        var hj = System.Web.HttpContext.Current.Request.Files.GetMultiple("hj");
                        foreach (var f in hj)
                        {
                            HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                            SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BG_ZZ, targetData.ID, Enums.SPFileCertificateTypes.HJ);
                        }
                    }
                    db.SaveChanges();
                }

            }
        }
        public static void CheckMPBGOfResidence(string ID, string State, string Opinion)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.BG_MPOFRESIDENCE.Find(ID);
                var o_entity = db.MPOfResidence.Find(entity.MPID);
                if (State == Enums.SPState.tg)
                {
                    //拷贝文件
                    var files = db.FILE.Where(t => t.IsValid == 1 && t.FormID == ID).ToList();
                    foreach (var file in files)
                    {
                        string sourceFile = Path.Combine(StaticVariable.basePathSBFile, file.BusinessType, file.FormID, file.CertificateType, file.FileName);
                        string targetFile = Path.Combine(StaticVariable.basePath, StaticVariable.residenceMPRelativePath, entity.MPID, file.FileName);

                        MPOfUploadFiles mpfile = new MPOfUploadFiles();
                        mpfile.ID = file.ID;
                        mpfile.MPID = entity.MPID;
                        mpfile.Name = file.FileOrginalName;
                        mpfile.FileEx = Path.GetExtension(sourceFile);
                        mpfile.DocType = file.CertificateType;
                        mpfile.State = 1;
                        mpfile.CreateTime = DateTime.Now;
                        db.MPOfUploadFiles.Add(mpfile);

                        System.IO.File.Copy(sourceFile, targetFile, true);
                    }

                    //信息更改
                    o_entity.PropertyOwner = entity.PropertyOwner;
                    o_entity.IDType = entity.IDType;
                    o_entity.IDNumber = entity.IDNumber;
                    if (entity.CertificateType == Enums.SPFileCertificateTypes.FCZ)
                    {
                        o_entity.FCZAddress = entity.CertificateAddress;
                        o_entity.FCZNumber = entity.CertificateNumber;
                    }
                    else if (entity.CertificateType == Enums.SPFileCertificateTypes.TDZ)
                    {
                        o_entity.TDZAddress = entity.CertificateAddress;
                        o_entity.TDZNumber = entity.CertificateNumber;
                    }
                    else if (entity.CertificateType == Enums.SPFileCertificateTypes.BDCZ)
                    {
                        o_entity.BDCZAddress = entity.CertificateAddress;
                        o_entity.BDCZNumber = entity.CertificateNumber;
                    }
                    else if (entity.CertificateType == Enums.SPFileCertificateTypes.HJ)
                    {
                        o_entity.HJAddress = entity.CertificateAddress;
                        o_entity.HJNumber = entity.CertificateNumber;
                    }
                    o_entity.Applicant = entity.Applicant;
                    o_entity.ApplicantPhone = entity.ApplicantPhone;
                    o_entity.SBLY = entity.SBLY;
                    o_entity.ProjID = entity.ProjID;
                    o_entity.LastModifyTime = DateTime.Now;
                    o_entity.LastModifyUser = LoginUtils.CurrentUser.UserName;
                }
                else
                {

                }
                entity.State = State;
                entity.Opinion = Opinion;
                entity.IsFinish = 1;
                entity.FinishTime = DateTime.Now;
                entity.CheckUser = LoginUtils.CurrentUser.UserName;
                db.SaveChanges();
            }
        }
        #endregion

        #region 住宅门牌申报
        public static SB_MPOFRESIDENCE GetMPOfResidenceInitData(string ID)
        {
            SB_MPOFRESIDENCE entity = null;
            if (!string.IsNullOrEmpty(ID))
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    entity = db.SB_MPOFRESIDENCE.Find(ID);
                    if (entity == null) throw new Error("未能找到指定的数据！");
                    entity.fcz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.FCZ);
                    entity.tdz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.TDZ);
                    entity.bdcz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.BDCZ);
                    entity.hj = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.HJ);
                }
            }
            return entity;
        }
        public static void ModifyMPOfResidence(string ID, string Json)
        {
            SB_MPOFRESIDENCE targetData = null;
            using (var db = SystemUtils.NewEFDbContext)
            {
                targetData = db.SB_MPOFRESIDENCE.Find(ID);
                if (targetData != null)
                {
                    ObjectReflection.ModifyEntity<SB_MPOFRESIDENCE>(targetData, Json);

                    #region 重复性检查
                    if (!ResidenceMPModify.CheckResidenceMPIsAvailable(targetData.ID, targetData.CountyID, targetData.NeighborhoodsID, targetData.CommunityName, targetData.ResidenceName, targetData.MPNumber, null, targetData.HSNumber, targetData.LZNumber, targetData.DYNumber))
                        throw new Error("该门牌已经存在，请检查！");
                    #endregion

                    var fcz = System.Web.HttpContext.Current.Request.Files.GetMultiple("fcz");
                    foreach (var f in fcz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BZ_ZZ, targetData.ID, Enums.SPFileCertificateTypes.FCZ);
                    }
                    var tdz = System.Web.HttpContext.Current.Request.Files.GetMultiple("tdz");
                    foreach (var f in tdz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BZ_ZZ, targetData.ID, Enums.SPFileCertificateTypes.TDZ);
                    }
                    var bdcz = System.Web.HttpContext.Current.Request.Files.GetMultiple("bdcz");
                    foreach (var f in bdcz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BZ_ZZ, targetData.ID, Enums.SPFileCertificateTypes.BDCZ);
                    }
                    var hj = System.Web.HttpContext.Current.Request.Files.GetMultiple("hj");
                    foreach (var f in hj)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BZ_ZZ, targetData.ID, Enums.SPFileCertificateTypes.HJ);
                    }
                    db.SaveChanges();
                }
            }
        }
        public static void CheckMPOfResidence(string ID, string State, string Opinion)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.SB_MPOFRESIDENCE.Find(ID);
                if (State == Enums.SPState.tg) //通过，新增
                {
                    MPOfResidence n_entity = new Models.Entities.MPOfResidence();
                    n_entity.ID = Guid.NewGuid().ToString();
                    n_entity.CountyID =entity.CountyID;
                    n_entity.NeighborhoodsID = entity.NeighborhoodsID;
                    n_entity.CommunityName = entity.CommunityName;
                    n_entity.MPNumber = entity.MPNumber;
                    n_entity.ResidenceName = entity.ResidenceName;
                    n_entity.LZNumber = entity.LZNumber;
                    n_entity.DYNumber = entity.DYNumber;
                    n_entity.DYPosition = (entity.DYPositionX != null && entity.DYPositionY != null && entity.DYPositionX != 0 && entity.DYPositionY != 0) ? (DbGeography.FromText($"POINT({entity.DYPositionX} {entity.DYPositionY})")) : null;
                    n_entity.HSNumber = entity.HSNumber;
                    n_entity.Postcode = entity.Postcode;
                    n_entity.PropertyOwner = entity.PropertyOwner;
                    n_entity.IDType = entity.IDType;
                    n_entity.IDNumber = entity.IDNumber;
                    n_entity.FCZAddress = entity.FCZAddress;
                    n_entity.FCZNumber = entity.FCZNumber;
                    n_entity.TDZAddress = entity.TDZAddress;
                    n_entity.TDZNumber = entity.TDZNumber;
                    n_entity.BDCZAddress = entity.BDCZAddress;
                    n_entity.BDCZNumber = entity.BDCZNumber;
                    n_entity.HJAddress = entity.HJAddress;
                    n_entity.HJNumber = entity.HJNumber;
                    n_entity.OtherAddress = entity.OtherAddress;
                    n_entity.Applicant = entity.Applicant;
                    n_entity.ApplicantPhone = entity.ApplicantPhone;
                    n_entity.BZTime = DateTime.Now;
                    n_entity.AddType = Enums.MPAddType.LX;
                    n_entity.MPProduce = Enums.MPProduce.NO;
                    n_entity.MPZPrintComplete = Enums.Complete.NO;
                    n_entity.DZZMPrintComplete = Enums.Complete.NO;
                    n_entity.State = Enums.UseState.Enable;
                    n_entity.CreateTime = DateTime.Now;
                    n_entity.CreateUser = LoginUtils.CurrentUser.UserName;
                    n_entity.SBLY = entity.SBLY;
                    n_entity.ProjID = entity.ProjID;

                    #region 地址编码前10位拼接
                    var CountyCode = db.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == n_entity.CountyID).Select(t => t.Code).FirstOrDefault();
                    var NeighborhoodsCode = db.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == n_entity.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                    var mpCategory = SystemUtils.Config.MPCategory.Residence.Value.ToString();
                    var year = entity.BZTime == null ? DateTime.Now.Year.ToString() : ((DateTime)(entity.BZTime)).Year.ToString();
                    var dm = CountyCode + NeighborhoodsCode + year + mpCategory;
                    SqlParameter idx = new SqlParameter("@idx", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output };
                    db.Database.ExecuteSqlCommand("exec getcode @code,@idx output;", new SqlParameter("@code", dm), idx);
                    var index = (int)idx.Value;
                    n_entity.AddressCoding = dm + index.ToString().PadLeft(5, '0');
                    #endregion

                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new CommunityDic();
                    CommunityDic.CountyID = n_entity.CountyID;
                    CommunityDic.NeighborhoodsID = n_entity.NeighborhoodsID;
                    CommunityDic.CommunityName = n_entity.CommunityName;
                    DicUtils.AddCommunityDic(CommunityDic);
                    #endregion

                    #region 检查这个行政区下小区名称是否在字典表中存在，若不存在就新增
                    var ResidenceDic = new ResidenceDic();
                    ResidenceDic.CountyID = n_entity.CountyID;
                    ResidenceDic.NeighborhoodsID = n_entity.NeighborhoodsID;
                    ResidenceDic.CommunityName = n_entity.CommunityName;
                    ResidenceDic.ResidenceName = n_entity.ResidenceName;
                    n_entity.ResidenceID = DicUtils.AddResidenceDic(ResidenceDic);
                    #endregion

                    #region 标准地址拼接 市辖区+镇街道+村社区+小区名+门牌号+宿舍名+幢号+单元号+户室号
                    var CountyName = n_entity.NeighborhoodsID.Split('.')[1];
                    var NeighborhoodsName = n_entity.NeighborhoodsID.Split('.')[2];
                    var CommunityName = n_entity.CommunityName;
                    var MPNumber1 = string.IsNullOrEmpty(n_entity.MPNumber) ? "" : n_entity.MPNumber + "号";
                    var LZNumber1 = string.IsNullOrEmpty(n_entity.LZNumber) ? "" : n_entity.LZNumber + "幢";
                    var DYNumber1 = string.IsNullOrEmpty(n_entity.DYNumber) ? "" : n_entity.DYNumber + "单元";
                    var HSNumber1 = string.IsNullOrEmpty(n_entity.HSNumber) ? "" : n_entity.HSNumber + "室";
                    var StandardAddress = "嘉兴市" + CountyName + NeighborhoodsName + n_entity.ResidenceName + LZNumber1 + MPNumber1 + DYNumber1 + HSNumber1;
                    n_entity.StandardAddress = StandardAddress;
                    #endregion

                    db.MPOfResidence.Add(n_entity);

                    //拷贝文件
                    var files = db.FILE.Where(t => t.IsValid == 1 && t.FormID == ID).ToList();
                    foreach (var file in files)
                    {
                        string sourceFile = Path.Combine(StaticVariable.basePathSBFile, file.BusinessType, file.FormID, file.CertificateType, file.FileName);
                        string targetFile = Path.Combine(StaticVariable.basePath, StaticVariable.residenceMPRelativePath, n_entity.ID, file.FileName);

                        MPOfUploadFiles mpfile = new MPOfUploadFiles();
                        mpfile.ID = file.ID;
                        mpfile.MPID = n_entity.ID;
                        mpfile.Name = file.FileOrginalName;
                        mpfile.FileEx = Path.GetExtension(sourceFile);
                        mpfile.DocType = file.CertificateType;
                        mpfile.State = 1;
                        mpfile.CreateTime = DateTime.Now;
                        db.MPOfUploadFiles.Add(mpfile);
                        System.IO.File.Copy(sourceFile, targetFile, true);
                    }

                }
                else
                { }
                entity.State = State;
                entity.Opinion = Opinion;
                entity.IsFinish = 1;
                entity.FinishTime = DateTime.Now;
                entity.CheckUser = LoginUtils.CurrentUser.UserName;
                db.SaveChanges();
            }
        }
        #endregion

        #region 道路门牌变更
        public static BG_MPOFROAD GetMPBGOfRoadInitData(string ID)
        {
            BG_MPOFROAD entity = null;
            if (!string.IsNullOrEmpty(ID))
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    entity = db.BG_MPOFROAD.Find(ID);
                    if (entity == null) throw new Error("未能找到指定的数据！");
                    // 查找原门牌
                    if (!string.IsNullOrEmpty(entity.MPID))
                    {
                        entity.O_Entity = db.MPOfRoad.Find(entity.MPID);
                    }
                    entity.fcz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.FCZ);
                    entity.tdz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.TDZ);
                    entity.yyzz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.YYZZ);

                }
            }
            return entity;
        }
        public static void ModifyMPBGOfRoad(string ID, string Json)
        {
            BG_MPOFROAD targetData = null;
            using (var db = SystemUtils.NewEFDbContext)
            {
                targetData = db.BG_MPOFROAD.Find(ID);
                if (targetData != null)
                {
                    ObjectReflection.ModifyEntity<BG_MPOFROAD>(targetData, Json);

                    if (targetData.CertificateType == Enums.SPFileCertificateTypes.FCZ)
                    {
                        var fcz = System.Web.HttpContext.Current.Request.Files.GetMultiple("fcz");
                        foreach (var f in fcz)
                        {
                            HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                            SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BG_DL, targetData.ID, Enums.SPFileCertificateTypes.FCZ);
                        }
                    }

                    if (targetData.CertificateType == Enums.SPFileCertificateTypes.TDZ)
                    {
                        var tdz = System.Web.HttpContext.Current.Request.Files.GetMultiple("tdz");
                        foreach (var f in tdz)
                        {
                            HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                            SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BG_DL, targetData.ID, Enums.SPFileCertificateTypes.TDZ);
                        }
                    }

                    if (targetData.CertificateType == Enums.SPFileCertificateTypes.YYZZ)
                    {
                        var yyzz = System.Web.HttpContext.Current.Request.Files.GetMultiple("yyzz");
                        foreach (var f in yyzz)
                        {
                            HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                            SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BG_DL, targetData.ID, Enums.SPFileCertificateTypes.YYZZ);
                        }
                    }
                    db.SaveChanges();
                }
            }
        }
        public static void CheckMPBGOfRoad(string ID, string State, string Opinion)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.BG_MPOFROAD.Find(ID);
                var o_entity = db.MPOfRoad.Find(entity.MPID);
                if (State == Enums.SPState.tg)
                {
                    //拷贝文件
                    var files = db.FILE.Where(t => t.IsValid == 1 && t.FormID == ID).ToList();
                    foreach (var file in files)
                    {
                        string sourceFile = Path.Combine(StaticVariable.basePathSBFile, file.BusinessType, file.FormID, file.CertificateType, file.FileName);
                        string targetFile = Path.Combine(StaticVariable.basePath, StaticVariable.roadMPRelativePath, entity.MPID, file.FileName);

                        MPOfUploadFiles mpfile = new MPOfUploadFiles();
                        mpfile.ID = file.ID;
                        mpfile.MPID = entity.MPID;
                        mpfile.Name = file.FileOrginalName;
                        mpfile.FileEx = Path.GetExtension(sourceFile);
                        mpfile.DocType = file.CertificateType;
                        mpfile.State = 1;
                        mpfile.CreateTime = DateTime.Now;
                        db.MPOfUploadFiles.Add(mpfile);

                        System.IO.File.Copy(sourceFile, targetFile, true);
                    }

                    //信息更改
                    o_entity.PropertyOwner = entity.PropertyOwner;
                    o_entity.IDType = entity.IDType;
                    o_entity.IDNumber = entity.IDNumber;
                    if (entity.CertificateType == Enums.SPFileCertificateTypes.FCZ)
                    {
                        o_entity.FCZAddress = entity.CertificateAddress;
                        o_entity.FCZNumber = entity.CertificateNumber;
                    }
                    else if (entity.CertificateType == Enums.SPFileCertificateTypes.TDZ)
                    {
                        o_entity.TDZAddress = entity.CertificateAddress;
                        o_entity.TDZNumber = entity.CertificateNumber;
                    }
                    else if (entity.CertificateType == Enums.SPFileCertificateTypes.BDCZ)
                    {
                        o_entity.YYZZAddress = entity.CertificateAddress;
                        o_entity.YYZZNumber = entity.CertificateNumber;
                    }

                    o_entity.Applicant = entity.Applicant;
                    o_entity.ApplicantPhone = entity.ApplicantPhone;
                    o_entity.SBLY = entity.SBLY;
                    o_entity.ProjID = entity.ProjID;
                    o_entity.LastModifyTime = DateTime.Now;
                    o_entity.LastModifyUser = LoginUtils.CurrentUser.UserName;
                }
                else
                {

                }
                entity.State = State;
                entity.Opinion = Opinion;
                entity.IsFinish = 1;
                entity.FinishTime = DateTime.Now;
                entity.CheckUser = LoginUtils.CurrentUser.UserName;
                db.SaveChanges();
            }
        }
        #endregion

        #region 道路门牌申报
        public static SB_MPOFROAD GetMPOfRoadInitData(string ID)
        {
            SB_MPOFROAD entity = null;
            if (!string.IsNullOrEmpty(ID))
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    entity = db.SB_MPOFROAD.Find(ID);
                    if (entity == null) throw new Error("未能找到指定的数据！");
                    entity.fcz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.FCZ);
                    entity.tdz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.TDZ);
                    entity.yyzz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.YYZZ);
                }
            }
            return entity;
        }
        public static void ModifyMPOfRoad(string ID, string Json)
        {
            SB_MPOFROAD targetData = null;
            using (var db = SystemUtils.NewEFDbContext)
            {
                targetData = db.SB_MPOFROAD.Find(ID);
                if (targetData != null)
                {
                    ObjectReflection.ModifyEntity<SB_MPOFROAD>(targetData, Json);

                    #region 重复性检查
                    if (!RoadMPModify.CheckRoadMPIsAvailable(targetData.ID, targetData.CountyID, targetData.NeighborhoodsID, targetData.CommunityName, targetData.RoadName, targetData.MPNumber))
                        throw new Error("该门牌已经存在，请检查！");
                    #endregion

                    var fcz = System.Web.HttpContext.Current.Request.Files.GetMultiple("fcz");
                    foreach (var f in fcz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BZ_DL, targetData.ID, Enums.SPFileCertificateTypes.FCZ);
                    }
                    var tdz = System.Web.HttpContext.Current.Request.Files.GetMultiple("tdz");
                    foreach (var f in tdz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BZ_DL, targetData.ID, Enums.SPFileCertificateTypes.TDZ);
                    }
                    var yyzz = System.Web.HttpContext.Current.Request.Files.GetMultiple("yyzz");
                    foreach (var f in yyzz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BZ_DL, targetData.ID, Enums.SPFileCertificateTypes.YYZZ);
                    }
                    db.SaveChanges();
                }
            }
        }
        public static void CheckMPOfRoad(string ID, string State, string Opinion)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.SB_MPOFROAD.Find(ID);
                if (State == Enums.SPState.tg) //通过，新增
                {
                    MPOfRoad n_entity = new Models.Entities.MPOfRoad();
                    n_entity.ID = Guid.NewGuid().ToString();
                    n_entity.CountyID = entity.CountyID;
                    n_entity.NeighborhoodsID = entity.NeighborhoodsID;
                    n_entity.CommunityName = entity.CommunityName;
                    n_entity.RoadName = entity.RoadName;
                    n_entity.ShopName = entity.ShopName;
                    n_entity.MPNumber = entity.MPNumber;
                    n_entity.MPPosition = (entity.MPPositionX != 0 && entity.MPPositionY != 0 && entity.MPPositionX != null && entity.MPPositionY != null) ? (DbGeography.FromText($"POINT({entity.MPPositionX} {entity.MPPositionY})")) : null;
                    #region 门牌号码类型 单双号判断赋值
                    if (!string.IsNullOrEmpty(entity.MPNumber))
                    {
                        int num = 0;
                        bool result = int.TryParse(entity.MPNumber, out num);
                        if (result)
                            n_entity.MPNumberType = num % 2 == 1 ? Enums.MPNumberType.Odd : Enums.MPNumberType.Even;
                        else
                            n_entity.MPNumberType = Enums.MPNumberType.Other;
                    }
                    #endregion
                    n_entity.OriginalMPAddress = entity.OriginalMPAddress;
                    n_entity.Postcode = entity.Postcode;
                    n_entity.PropertyOwner = entity.PropertyOwner;
                    n_entity.IDType = entity.IDType;
                    n_entity.IDNumber = entity.IDNumber;
                    n_entity.FCZAddress = entity.FCZAddress;
                    n_entity.FCZNumber = entity.FCZNumber;
                    n_entity.TDZAddress = entity.TDZAddress;
                    n_entity.TDZNumber = entity.TDZNumber;
                    n_entity.YYZZAddress = entity.YYZZAddress;
                    n_entity.YYZZNumber = entity.YYZZNumber;
                    n_entity.OtherAddress = entity.OtherAddress;
                    n_entity.Applicant = entity.Applicant;
                    n_entity.ApplicantPhone = entity.ApplicantPhone;
                    n_entity.BZTime = DateTime.Now;
                    n_entity.AddType = Enums.MPAddType.LX;
                    n_entity.MPProduce = Enums.MPProduce.NO;
                    n_entity.MPZPrintComplete = Enums.Complete.NO;
                    n_entity.DZZMPrintComplete = Enums.Complete.NO;
                    n_entity.State = Enums.UseState.Enable;
                    n_entity.CreateTime = DateTime.Now;
                    n_entity.CreateUser = LoginUtils.CurrentUser.UserName;
                    n_entity.SBLY = entity.SBLY;
                    n_entity.ProjID = entity.ProjID;

                    #region 地址编码前10位拼接
                    var CountyCode = db.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == n_entity.CountyID).Select(t => t.Code).FirstOrDefault();
                    var NeighborhoodsCode = db.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == n_entity.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                    var mpCategory = SystemUtils.Config.MPCategory.Residence.Value.ToString();
                    var year = DateTime.Now.Year.ToString();
                    var dm = CountyCode + NeighborhoodsCode + year + mpCategory;
                    SqlParameter idx = new SqlParameter("@idx", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output };
                    db.Database.ExecuteSqlCommand("exec getcode @code,@idx output;", new SqlParameter("@code", dm), idx);
                    var index = (int)idx.Value;
                    n_entity.AddressCoding = dm + index.ToString().PadLeft(5, '0');
                    #endregion

                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new CommunityDic();
                    CommunityDic.CountyID = n_entity.CountyID;
                    CommunityDic.NeighborhoodsID = n_entity.NeighborhoodsID;
                    CommunityDic.CommunityName = n_entity.CommunityName;
                    DicUtils.AddCommunityDic(CommunityDic);
                    #endregion

                    #region 标准地址拼接 市辖区+镇街道+村社区+小区名+门牌号+宿舍名+幢号+单元号+户室号
                    var CountyName = n_entity.NeighborhoodsID.Split('.')[1];
                    var NeighborhoodsName = n_entity.NeighborhoodsID.Split('.')[2];
                    var CommunityName = n_entity.CommunityName;
                    var StandardAddress = "嘉兴市" + CountyName + NeighborhoodsName + n_entity.RoadName + n_entity.MPNumber + "号";
                    n_entity.StandardAddress = StandardAddress;
                    #endregion

                    db.MPOfRoad.Add(n_entity);

                    //拷贝文件
                    var files = db.FILE.Where(t => t.IsValid == 1 && t.FormID == ID).ToList();
                    foreach (var file in files)
                    {
                        string sourceFile = Path.Combine(StaticVariable.basePathSBFile, file.BusinessType, file.FormID, file.CertificateType, file.FileName);
                        string targetFile = Path.Combine(StaticVariable.basePath, StaticVariable.roadMPRelativePath, n_entity.ID, file.FileName);

                        MPOfUploadFiles mpfile = new MPOfUploadFiles();
                        mpfile.ID = file.ID;
                        mpfile.MPID = n_entity.ID;
                        mpfile.Name = file.FileOrginalName;
                        mpfile.FileEx = Path.GetExtension(sourceFile);
                        mpfile.DocType = file.CertificateType;
                        mpfile.State = 1;
                        mpfile.CreateTime = DateTime.Now;
                        db.MPOfUploadFiles.Add(mpfile);

                        System.IO.File.Copy(sourceFile, targetFile, true);
                    }
                }
                else
                { }
                entity.State = State;
                entity.Opinion = Opinion;
                entity.IsFinish = 1;
                entity.FinishTime = DateTime.Now;
                entity.CheckUser = LoginUtils.CurrentUser.UserName;
                db.SaveChanges();
            }
        }
        #endregion

        #region 农村门牌变更
        public static BG_MPOFCOUNTRY GetMPBGOfCountryInitData(string ID)
        {
            BG_MPOFCOUNTRY entity = null;
            if (!string.IsNullOrEmpty(ID))
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    entity = db.BG_MPOFCOUNTRY.Find(ID);
                    if (entity == null) throw new Error("未能找到指定的数据！");
                    // 查找原门牌
                    if (!string.IsNullOrEmpty(entity.MPID))
                    {
                        entity.O_Entity = db.MPOfCountry.Find(entity.MPID);
                    }
                    entity.tdz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.TDZ);
                    entity.qqz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.QQZ);

                }
            }
            return entity;
        }
        public static void ModifyPBGOfCountry(string ID, string Json)
        {
            BG_MPOFCOUNTRY targetData = null;
            using (var db = SystemUtils.NewEFDbContext)
            {
                targetData = db.BG_MPOFCOUNTRY.Find(ID);
                if (targetData != null)
                {
                    ObjectReflection.ModifyEntity<BG_MPOFCOUNTRY>(targetData, Json);

                    if (targetData.CertificateType == Enums.SPFileCertificateTypes.TDZ)
                    {
                        var tdz = System.Web.HttpContext.Current.Request.Files.GetMultiple("tdz");
                        foreach (var f in tdz)
                        {
                            HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                            SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BG_NC, targetData.ID, Enums.SPFileCertificateTypes.TDZ);
                        }
                    }

                    if (targetData.CertificateType == Enums.SPFileCertificateTypes.QQZ)
                    {
                        var qqz = System.Web.HttpContext.Current.Request.Files.GetMultiple("qqz");
                        foreach (var f in qqz)
                        {
                            HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                            SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BG_NC, targetData.ID, Enums.SPFileCertificateTypes.QQZ);
                        }
                    }
                    db.SaveChanges();
                }
            }
        }
        public static void CheckMPBGOfCountry(string ID, string State, string Opinion)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.BG_MPOFCOUNTRY.Find(ID);
                var o_entity = db.MPOfCountry.Find(entity.MPID);
                if (State == Enums.SPState.tg)
                {
                    //拷贝文件
                    var files = db.FILE.Where(t => t.IsValid == 1 && t.FormID == ID).ToList();
                    foreach (var file in files)
                    {
                        string sourceFile = Path.Combine(StaticVariable.basePathSBFile, file.BusinessType, file.FormID, file.CertificateType, file.FileName);
                        string targetFile = Path.Combine(StaticVariable.basePath, StaticVariable.countryMPRelativePath, entity.MPID, file.FileName);

                        MPOfUploadFiles mpfile = new MPOfUploadFiles();
                        mpfile.ID = file.ID;
                        mpfile.MPID = entity.MPID;
                        mpfile.Name = file.FileOrginalName;
                        mpfile.FileEx = Path.GetExtension(sourceFile);
                        mpfile.DocType = file.CertificateType;
                        mpfile.State = 1;
                        mpfile.CreateTime = DateTime.Now;
                        db.MPOfUploadFiles.Add(mpfile);

                        System.IO.File.Copy(sourceFile, targetFile, true);
                    }

                    //信息更改
                    o_entity.PropertyOwner = entity.PropertyOwner;
                    o_entity.IDType = entity.IDType;
                    o_entity.IDNumber = entity.IDNumber;
                    if (entity.CertificateType == Enums.SPFileCertificateTypes.TDZ)
                    {
                        o_entity.TDZAddress = entity.CertificateAddress;
                        o_entity.TDZNumber = entity.CertificateNumber;
                    }
                    else if (entity.CertificateType == Enums.SPFileCertificateTypes.QQZ)
                    {
                        o_entity.QQZAddress = entity.CertificateAddress;
                        o_entity.QQZNumber = entity.CertificateNumber;
                    }

                    o_entity.Applicant = entity.Applicant;
                    o_entity.ApplicantPhone = entity.ApplicantPhone;
                    o_entity.SBLY = entity.SBLY;
                    o_entity.ProjID = entity.ProjID;
                    o_entity.LastModifyTime = DateTime.Now;
                    o_entity.LastModifyUser = LoginUtils.CurrentUser.UserName;
                }
                else
                {

                }
                entity.State = State;
                entity.Opinion = Opinion;
                entity.IsFinish = 1;
                entity.FinishTime = DateTime.Now;
                entity.CheckUser = LoginUtils.CurrentUser.UserName;
                db.SaveChanges();
            }
        }
        #endregion

        #region 农村门牌申报
        public static SB_MPOFCOUNTRY GetMPOfCountryInitData(string ID)
        {
            SB_MPOFCOUNTRY entity = null;
            if (!string.IsNullOrEmpty(ID))
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    entity = db.SB_MPOFCOUNTRY.Find(ID);
                    if (entity == null) throw new Error("未能找到指定的数据！");
                    entity.tdz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.TDZ);
                    entity.qqz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.QQZ);
                }
            }
            return entity;
        }
        public static void ModifyMPOfCountry(string ID, string Json)
        {
            SB_MPOFCOUNTRY targetData = null;
            using (var db = SystemUtils.NewEFDbContext)
            {
                targetData = db.SB_MPOFCOUNTRY.Find(ID);
                if (targetData != null)
                {
                    ObjectReflection.ModifyEntity<SB_MPOFCOUNTRY>(targetData, Json);

                    #region 重复性检查
                    if (!CountryMPModify.CheckCountryMPIsAvailable(targetData.ID, targetData.CountyID, targetData.NeighborhoodsID, targetData.CommunityName, targetData.VillageName, targetData.MPNumber, targetData.HSNumber))
                        throw new Error("该门牌已经存在，请检查！");
                    #endregion

                    var tdz = System.Web.HttpContext.Current.Request.Files.GetMultiple("tdz");
                    foreach (var f in tdz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BZ_NC, targetData.ID, Enums.SPFileCertificateTypes.TDZ);
                    }
                    var qqz = System.Web.HttpContext.Current.Request.Files.GetMultiple("qqz");
                    foreach (var f in qqz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.HFMPZ_BZ_NC, targetData.ID, Enums.SPFileCertificateTypes.QQZ);
                    }
                    db.SaveChanges();
                }
            }
        }
        public static void CheckMPOfCountry(string ID, string State, string Opinion)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.SB_MPOFCOUNTRY.Find(ID);
                if (State == Enums.SPState.tg) //通过，新增
                {
                    MPOfCountry n_entity = new Models.Entities.MPOfCountry();
                    n_entity.ID = Guid.NewGuid().ToString();
                    n_entity.CountyID = entity.CountyID;
                    n_entity.NeighborhoodsID = entity.NeighborhoodsID;
                    n_entity.CommunityName = entity.CommunityName;
                    n_entity.ViligeName = entity.VillageName;
                    n_entity.MPNumber = entity.MPNumber;
                    n_entity.MPPosition = (entity.MPPositionX != 0 && entity.MPPositionY != 0 && entity.MPPositionX != null && entity.MPPositionY != null) ? (DbGeography.FromText($"POINT({entity.MPPositionX} {entity.MPPositionY})")) : null;
                    n_entity.OriginalMPAddress = entity.OriginalMPAddress;
                    n_entity.HSNumber = entity.HSNumber;
                    n_entity.Postcode = entity.Postcode;
                    n_entity.PropertyOwner = entity.PropertyOwner;
                    n_entity.IDType = entity.IDType;
                    n_entity.IDNumber = entity.IDNumber;
                    n_entity.TDZAddress = entity.TDZAddress;
                    n_entity.TDZNumber = entity.TDZNumber;
                    n_entity.QQZAddress = entity.QQZAddress;
                    n_entity.QQZNumber = entity.QQZNumber;
                    n_entity.OtherAddress = entity.OtherAddress;
                    n_entity.Applicant = entity.Applicant;
                    n_entity.ApplicantPhone = entity.ApplicantPhone;
                    n_entity.BZTime = DateTime.Now;
                    n_entity.AddType = Enums.MPAddType.LX;
                    n_entity.MPProduce = Enums.MPProduce.NO;
                    n_entity.MPZPrintComplete = Enums.Complete.NO;
                    n_entity.DZZMPrintComplete = Enums.Complete.NO;
                    n_entity.State = Enums.UseState.Enable;
                    n_entity.CreateTime = DateTime.Now;
                    n_entity.CreateUser = LoginUtils.CurrentUser.UserName;
                    n_entity.SBLY = entity.SBLY;
                    n_entity.ProjID = entity.ProjID;

                    #region 地址编码前10位拼接
                    var CountyCode = db.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == n_entity.CountyID).Select(t => t.Code).FirstOrDefault();
                    var NeighborhoodsCode = db.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == n_entity.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                    var mpCategory = SystemUtils.Config.MPCategory.Residence.Value.ToString();
                    var year = DateTime.Now.Year.ToString();
                    var dm = CountyCode + NeighborhoodsCode + year + mpCategory;
                    SqlParameter idx = new SqlParameter("@idx", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output };
                    db.Database.ExecuteSqlCommand("exec getcode @code,@idx output;", new SqlParameter("@code", dm), idx);
                    var index = (int)idx.Value;
                    n_entity.AddressCoding = dm + index.ToString().PadLeft(5, '0');
                    #endregion

                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new CommunityDic();
                    CommunityDic.CountyID = n_entity.CountyID;
                    CommunityDic.NeighborhoodsID = n_entity.NeighborhoodsID;
                    CommunityDic.CommunityName = n_entity.CommunityName;
                    DicUtils.AddCommunityDic(CommunityDic);
                    #endregion

                    #region 标准地址拼接 市辖区+镇街道+村社区+小区名+门牌号+宿舍名+幢号+单元号+户室号
                    var CountyName = n_entity.NeighborhoodsID.Split('.')[1];
                    var NeighborhoodsName = n_entity.NeighborhoodsID.Split('.')[2];
                    var CommunityName = n_entity.CommunityName;
                    var HSNumber1 = string.IsNullOrEmpty(n_entity.HSNumber) ? string.Empty : n_entity.HSNumber + "室";
                    var StandardAddress = "嘉兴市" + CountyName + NeighborhoodsName + CommunityName + n_entity.ViligeName + n_entity.MPNumber + "号" + HSNumber1;
                    n_entity.StandardAddress = StandardAddress;
                    #endregion

                    db.MPOfCountry.Add(n_entity);

                    //拷贝文件
                    var files = db.FILE.Where(t => t.IsValid == 1 && t.FormID == ID).ToList();
                    foreach (var file in files)
                    {
                        string sourceFile = Path.Combine(StaticVariable.basePathSBFile, file.BusinessType, file.FormID, file.CertificateType, file.FileName);
                        string targetFile = Path.Combine(StaticVariable.basePath, StaticVariable.countryMPRelativePath, n_entity.ID, file.FileName);

                        MPOfUploadFiles mpfile = new MPOfUploadFiles();
                        mpfile.ID = file.ID;
                        mpfile.MPID = n_entity.ID;
                        mpfile.Name = file.FileOrginalName;
                        mpfile.FileEx = Path.GetExtension(sourceFile);
                        mpfile.DocType = file.CertificateType;
                        mpfile.State = 1;
                        mpfile.CreateTime = DateTime.Now;
                        db.MPOfUploadFiles.Add(mpfile);

                        System.IO.File.Copy(sourceFile, targetFile, true);
                    }
                }
                else
                { }
                entity.State = State;
                entity.Opinion = Opinion;
                entity.IsFinish = 1;
                entity.FinishTime = DateTime.Now;
                entity.CheckUser = LoginUtils.CurrentUser.UserName;
                db.SaveChanges();
            }
        }
        #endregion

        #region 住宅门牌地名证明
        public static ZM_MPOFRESIDENCE GetDMZMOfResidenceInitData(string ID)
        {
            ZM_MPOFRESIDENCE entity = null;
            if (!string.IsNullOrEmpty(ID))
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    entity = db.ZM_MPOFRESIDENCE.Find(ID);
                    if (entity == null) throw new Error("未能找到指定的数据！");
                    entity.fcz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.FCZ);
                    entity.tdz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.TDZ);
                    entity.bdcz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.BDCZ);
                    entity.hj = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.HJ);
                }
            }
            return entity;
        }
        public static void ModifyMPZMOfResidence(string ID, string Json)
        {
            ZM_MPOFRESIDENCE targetData = null;
            using (var db = SystemUtils.NewEFDbContext)
            {
                targetData = db.ZM_MPOFRESIDENCE.Find(ID);
                if (targetData != null)
                {
                    ObjectReflection.ModifyEntity<ZM_MPOFRESIDENCE>(targetData, Json);

                    var fcz = System.Web.HttpContext.Current.Request.Files.GetMultiple("fcz");
                    foreach (var f in fcz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMZM_ZZ, targetData.ID, Enums.SPFileCertificateTypes.FCZ);
                    }
                    var tdz = System.Web.HttpContext.Current.Request.Files.GetMultiple("tdz");
                    foreach (var f in tdz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMZM_ZZ, targetData.ID, Enums.SPFileCertificateTypes.TDZ);
                    }
                    var bdcz = System.Web.HttpContext.Current.Request.Files.GetMultiple("bdcz");
                    foreach (var f in bdcz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMZM_ZZ, targetData.ID, Enums.SPFileCertificateTypes.BDCZ);
                    }
                    var hj = System.Web.HttpContext.Current.Request.Files.GetMultiple("hj");
                    foreach (var f in hj)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMZM_ZZ, targetData.ID, Enums.SPFileCertificateTypes.HJ);
                    }
                    db.SaveChanges();
                }
            }
        }
        public static string CheckMPZMOfResidence(string ID, string State, string Opinion)
        {
            var mpid = string.Empty;
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.ZM_MPOFRESIDENCE.Find(ID);
                if (State == Enums.SPState.tg)//通过
                {
                    var isnew = ResidenceMPModify.CheckResidenceMPIsAvailable(entity.ID, entity.CountyID, entity.NeighborhoodsID, entity.CommunityName, entity.ResidenceName, entity.MPNumber, null, entity.HSNumber, entity.LZNumber, entity.DYNumber);
                    if (isnew)//新门牌，需新增
                    {
                        MPOfResidence n_entity = new Models.Entities.MPOfResidence();
                        n_entity.ID = Guid.NewGuid().ToString();
                        n_entity.CountyID = entity.CountyID;
                        n_entity.NeighborhoodsID = entity.NeighborhoodsID;
                        n_entity.CommunityName = entity.CommunityName;
                        n_entity.MPNumber = entity.MPNumber;
                        n_entity.ResidenceName = entity.ResidenceName;
                        n_entity.LZNumber = entity.LZNumber;
                        n_entity.DYNumber = entity.DYNumber;
                        n_entity.DYPosition = entity.DYPositionX != null && entity.DYPositionY != null ? (DbGeography.FromText($"POINT({entity.DYPositionX} {entity.DYPositionY})")) : null;
                        n_entity.HSNumber = entity.HSNumber;
                        n_entity.Postcode = entity.Postcode;
                        n_entity.PropertyOwner = entity.PropertyOwner;
                        n_entity.IDType = entity.IDType;
                        n_entity.IDNumber = entity.IDNumber;
                        n_entity.FCZAddress = entity.FCZAddress;
                        n_entity.FCZNumber = entity.FCZNumber;
                        n_entity.TDZAddress = entity.TDZAddress;
                        n_entity.TDZNumber = entity.TDZNumber;
                        n_entity.BDCZAddress = entity.BDCZAddress;
                        n_entity.BDCZNumber = entity.BDCZNumber;
                        n_entity.HJAddress = entity.HJAddress;
                        n_entity.HJNumber = entity.HJNumber;
                        n_entity.OtherAddress = entity.OtherAddress;
                        n_entity.Applicant = entity.Applicant;
                        n_entity.ApplicantPhone = entity.ApplicantPhone;
                        n_entity.BZTime = DateTime.Now;
                        n_entity.AddType = Enums.MPAddType.LX;
                        n_entity.MPProduce = Enums.MPProduce.NO;
                        n_entity.MPZPrintComplete = Enums.Complete.NO;
                        n_entity.DZZMPrintComplete = Enums.Complete.NO;
                        n_entity.State = Enums.UseState.Enable;
                        n_entity.CreateTime = DateTime.Now;
                        n_entity.CreateUser = LoginUtils.CurrentUser.UserName;
                        n_entity.SBLY = entity.SBLY;
                        n_entity.ProjID = entity.ProjID;

                        #region 地址编码前10位拼接
                        var CountyCode = db.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == n_entity.CountyID).Select(t => t.Code).FirstOrDefault();
                        var NeighborhoodsCode = db.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == n_entity.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                        var mpCategory = SystemUtils.Config.MPCategory.Residence.Value.ToString();
                        var year = entity.BZTime == null ? DateTime.Now.Year.ToString() : ((DateTime)(entity.BZTime)).Year.ToString();
                        var dm = CountyCode + NeighborhoodsCode + year + mpCategory;
                        SqlParameter idx = new SqlParameter("@idx", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output };
                        db.Database.ExecuteSqlCommand("exec getcode @code,@idx output;", new SqlParameter("@code", dm), idx);
                        var index = (int)idx.Value;
                        n_entity.AddressCoding = dm + index.ToString().PadLeft(5, '0');
                        #endregion

                        #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                        var CommunityDic = new CommunityDic();
                        CommunityDic.CountyID = n_entity.CountyID;
                        CommunityDic.NeighborhoodsID = n_entity.NeighborhoodsID;
                        CommunityDic.CommunityName = n_entity.CommunityName;
                        DicUtils.AddCommunityDic(CommunityDic);
                        #endregion

                        #region 检查这个行政区下小区名称是否在字典表中存在，若不存在就新增
                        var ResidenceDic = new ResidenceDic();
                        ResidenceDic.CountyID = n_entity.CountyID;
                        ResidenceDic.NeighborhoodsID = n_entity.NeighborhoodsID;
                        ResidenceDic.CommunityName = n_entity.CommunityName;
                        ResidenceDic.ResidenceName = n_entity.ResidenceName;
                        n_entity.ResidenceID = DicUtils.AddResidenceDic(ResidenceDic);
                        #endregion

                        #region 标准地址拼接 市辖区+镇街道+村社区+小区名+门牌号+宿舍名+幢号+单元号+户室号
                        var CountyName = n_entity.NeighborhoodsID.Split('.')[1];
                        var NeighborhoodsName = n_entity.NeighborhoodsID.Split('.')[2];
                        var CommunityName = n_entity.CommunityName;
                        var MPNumber1 = string.IsNullOrEmpty(n_entity.MPNumber) ? "" : n_entity.MPNumber + "号";
                        var LZNumber1 = string.IsNullOrEmpty(n_entity.LZNumber) ? "" : n_entity.LZNumber + "幢";
                        var DYNumber1 = string.IsNullOrEmpty(n_entity.DYNumber) ? "" : n_entity.DYNumber + "单元";
                        var HSNumber1 = string.IsNullOrEmpty(n_entity.HSNumber) ? "" : n_entity.HSNumber + "室";
                        var StandardAddress = "嘉兴市" + CountyName + NeighborhoodsName + n_entity.ResidenceName + LZNumber1 + MPNumber1 + DYNumber1 + HSNumber1;
                        n_entity.StandardAddress = StandardAddress;
                        #endregion

                        db.MPOfResidence.Add(n_entity);

                        mpid = n_entity.ID;
                    }
                    else//老门牌
                    {
                        var o_entity = db.MPOfResidence.Where(t => t.CountyID == entity.CountyID && t.NeighborhoodsID == entity.NeighborhoodsID && t.CommunityName == entity.CommunityName && t.ResidenceName == entity.ResidenceName && t.MPNumber == entity.MPNumber && t.HSNumber == entity.HSNumber && t.LZNumber == entity.LZNumber && t.DYNumber == entity.DYNumber).FirstOrDefault();
                        o_entity.FCZAddress = entity.FCZAddress == null ? o_entity.FCZAddress : entity.FCZAddress;
                        o_entity.FCZNumber = entity.FCZNumber == null ? o_entity.FCZNumber : entity.FCZNumber;
                        o_entity.TDZAddress = entity.TDZAddress == null ? o_entity.TDZAddress : entity.TDZAddress;
                        o_entity.TDZNumber = entity.TDZNumber == null ? o_entity.TDZNumber : entity.TDZNumber;
                        o_entity.BDCZAddress = entity.BDCZAddress == null ? o_entity.BDCZAddress : entity.BDCZAddress;
                        o_entity.BDCZNumber = entity.BDCZNumber == null ? o_entity.BDCZNumber : entity.BDCZNumber;
                        o_entity.HJAddress = entity.HJAddress == null ? o_entity.HJAddress : entity.HJAddress;
                        o_entity.HJNumber = entity.HJNumber == null ? o_entity.HJNumber : entity.HJNumber;
                        o_entity.OtherAddress = entity.OtherAddress == null ? o_entity.OtherAddress : entity.OtherAddress;

                        mpid = o_entity.ID;
                    }
                    //拷贝文件
                    var files = db.FILE.Where(t => t.IsValid == 1 && t.FormID == ID).ToList();
                    foreach (var file in files)
                    {
                        string sourceFile = Path.Combine(StaticVariable.basePathSBFile, file.BusinessType, file.FormID, file.CertificateType, file.FileName);
                        string targetFile = Path.Combine(StaticVariable.basePath, StaticVariable.residenceMPRelativePath, mpid, file.FileName);

                        MPOfUploadFiles mpfile = new MPOfUploadFiles();
                        mpfile.ID = file.ID;
                        mpfile.MPID = mpid;
                        mpfile.Name = file.FileOrginalName;
                        mpfile.FileEx = Path.GetExtension(sourceFile);
                        mpfile.DocType = file.CertificateType;
                        mpfile.State = 1;
                        mpfile.CreateTime = DateTime.Now;
                        db.MPOfUploadFiles.Add(mpfile);
                        System.IO.File.Copy(sourceFile, targetFile, true);
                    }
                    //保存后显示打印按钮
                }
                entity.State = State;
                entity.Opinion = Opinion;
                entity.IsFinish = 1;
                entity.FinishTime = DateTime.Now;
                entity.CheckUser = LoginUtils.CurrentUser.UserName;
                db.SaveChanges();
            }
            return mpid;
        }
        #endregion

        #region 道路门牌地名证明
        public static ZM_MPOFROAD GetDMZMOfRoadInitData(string ID)
        {
            ZM_MPOFROAD entity = null;
            if (!string.IsNullOrEmpty(ID))
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    entity = db.ZM_MPOFROAD.Find(ID);
                    if (entity == null) throw new Error("未能找到指定的数据！");
                    entity.fcz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.FCZ);
                    entity.tdz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.TDZ);
                    entity.yyzz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.YYZZ);
                }
            }
            return entity;
        }
        public static void ModifyMPZMOfRoad(string ID, string Json)
        {
            ZM_MPOFROAD targetData = null;
            using (var db = SystemUtils.NewEFDbContext)
            {
                targetData = db.ZM_MPOFROAD.Find(ID);
                if (targetData != null)
                {
                    ObjectReflection.ModifyEntity<ZM_MPOFROAD>(targetData, Json);

                    var fcz = System.Web.HttpContext.Current.Request.Files.GetMultiple("fcz");
                    foreach (var f in fcz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMZM_DL, targetData.ID, Enums.SPFileCertificateTypes.FCZ);
                    }
                    var tdz = System.Web.HttpContext.Current.Request.Files.GetMultiple("tdz");
                    foreach (var f in tdz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMZM_DL, targetData.ID, Enums.SPFileCertificateTypes.TDZ);
                    }
                    var yyzz = System.Web.HttpContext.Current.Request.Files.GetMultiple("yyzz");
                    foreach (var f in yyzz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMZM_DL, targetData.ID, Enums.SPFileCertificateTypes.YYZZ);
                    }
                    db.SaveChanges();
                }
            }
        }
        public static string CheckMPZMOfRoad(string ID, string State, string Opinion)
        {
            var mpid = string.Empty;
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.ZM_MPOFROAD.Find(ID);
                if (State == Enums.SPState.tg) //通过
                {
                    var isnew = RoadMPModify.CheckRoadMPIsAvailable(entity.ID, entity.CountyID, entity.NeighborhoodsID, entity.CommunityName, entity.RoadName, entity.MPNumber);
                    if (isnew)//新门牌，需新增
                    {
                        MPOfRoad n_entity = new Models.Entities.MPOfRoad();
                        n_entity.ID = Guid.NewGuid().ToString();
                        n_entity.CountyID = entity.CountyID;
                        n_entity.NeighborhoodsID = entity.NeighborhoodsID;
                        n_entity.CommunityName = entity.CommunityName;
                        n_entity.RoadName = entity.RoadName;
                        n_entity.ShopName = entity.ShopName;
                        n_entity.MPNumber = entity.MPNumber;
                        n_entity.MPPosition = (entity.MPPositionX != 0 && entity.MPPositionY != 0 && entity.MPPositionX != null && entity.MPPositionY != null) ? (DbGeography.FromText($"POINT({entity.MPPositionX} {entity.MPPositionY})")) : null;
                        #region 门牌号码类型 单双号判断赋值
                        if (!string.IsNullOrEmpty(entity.MPNumber))
                        {
                            int num = 0;
                            bool result = int.TryParse(entity.MPNumber, out num);
                            if (result)
                                n_entity.MPNumberType = num % 2 == 1 ? Enums.MPNumberType.Odd : Enums.MPNumberType.Even;
                            else
                                n_entity.MPNumberType = Enums.MPNumberType.Other;
                        }
                        #endregion
                        n_entity.OriginalMPAddress = entity.OriginalMPAddress;
                        n_entity.Postcode = entity.Postcode;
                        n_entity.PropertyOwner = entity.PropertyOwner;
                        n_entity.IDType = entity.IDType;
                        n_entity.IDNumber = entity.IDNumber;
                        n_entity.FCZAddress = entity.FCZAddress;
                        n_entity.FCZNumber = entity.FCZNumber;
                        n_entity.TDZAddress = entity.TDZAddress;
                        n_entity.TDZNumber = entity.TDZNumber;
                        n_entity.YYZZAddress = entity.YYZZAddress;
                        n_entity.YYZZNumber = entity.YYZZNumber;
                        n_entity.OtherAddress = entity.OtherAddress;
                        n_entity.Applicant = entity.Applicant;
                        n_entity.ApplicantPhone = entity.ApplicantPhone;
                        n_entity.BZTime = DateTime.Now;
                        n_entity.AddType = Enums.MPAddType.LX;
                        n_entity.MPProduce = Enums.MPProduce.NO;
                        n_entity.MPZPrintComplete = Enums.Complete.NO;
                        n_entity.DZZMPrintComplete = Enums.Complete.NO;
                        n_entity.State = Enums.UseState.Enable;
                        n_entity.CreateTime = DateTime.Now;
                        n_entity.CreateUser = LoginUtils.CurrentUser.UserName;
                        n_entity.SBLY = entity.SBLY;
                        n_entity.ProjID = entity.ProjID;

                        #region 地址编码前10位拼接
                        var CountyCode = db.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == n_entity.CountyID).Select(t => t.Code).FirstOrDefault();
                        var NeighborhoodsCode = db.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == n_entity.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                        var mpCategory = SystemUtils.Config.MPCategory.Residence.Value.ToString();
                        var year = DateTime.Now.Year.ToString();
                        var dm = CountyCode + NeighborhoodsCode + year + mpCategory;
                        SqlParameter idx = new SqlParameter("@idx", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output };
                        db.Database.ExecuteSqlCommand("exec getcode @code,@idx output;", new SqlParameter("@code", dm), idx);
                        var index = (int)idx.Value;
                        n_entity.AddressCoding = dm + index.ToString().PadLeft(5, '0');
                        #endregion

                        #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                        var CommunityDic = new CommunityDic();
                        CommunityDic.CountyID = n_entity.CountyID;
                        CommunityDic.NeighborhoodsID = n_entity.NeighborhoodsID;
                        CommunityDic.CommunityName = n_entity.CommunityName;
                        DicUtils.AddCommunityDic(CommunityDic);
                        #endregion

                        #region 标准地址拼接 市辖区+镇街道+村社区+小区名+门牌号+宿舍名+幢号+单元号+户室号
                        var CountyName = n_entity.NeighborhoodsID.Split('.')[1];
                        var NeighborhoodsName = n_entity.NeighborhoodsID.Split('.')[2];
                        var CommunityName = n_entity.CommunityName;
                        var StandardAddress = "嘉兴市" + CountyName + NeighborhoodsName + n_entity.RoadName + n_entity.MPNumber + "号";
                        n_entity.StandardAddress = StandardAddress;
                        #endregion

                        db.MPOfRoad.Add(n_entity);

                        mpid = n_entity.ID;
                    }
                    else//老门牌
                    {
                        var o_entity = db.MPOfRoad.Where(t => t.CountyID == entity.CountyID && t.NeighborhoodsID == entity.NeighborhoodsID && t.CommunityName == entity.CommunityName && t.RoadName == entity.RoadName && t.MPNumber == entity.MPNumber).FirstOrDefault();
                        o_entity.FCZAddress = entity.FCZAddress == null ? o_entity.FCZAddress : entity.FCZAddress;
                        o_entity.FCZNumber = entity.FCZNumber == null ? o_entity.FCZNumber : entity.FCZNumber;
                        o_entity.TDZAddress = entity.TDZAddress == null ? o_entity.TDZAddress : entity.TDZAddress;
                        o_entity.TDZNumber = entity.TDZNumber == null ? o_entity.TDZNumber : entity.TDZNumber;
                        o_entity.YYZZAddress = entity.YYZZAddress == null ? o_entity.YYZZAddress : entity.YYZZAddress;
                        o_entity.YYZZNumber = entity.YYZZNumber == null ? o_entity.YYZZNumber : entity.YYZZNumber;
                        o_entity.OtherAddress = entity.OtherAddress == null ? o_entity.OtherAddress : entity.OtherAddress;

                        mpid = o_entity.ID;
                    }
                    //拷贝文件
                    var files = db.FILE.Where(t => t.IsValid == 1 && t.FormID == ID).ToList();
                    foreach (var file in files)
                    {
                        string sourceFile = Path.Combine(StaticVariable.basePathSBFile, file.BusinessType, file.FormID, file.CertificateType, file.FileName);
                        string targetFile = Path.Combine(StaticVariable.basePath, StaticVariable.roadMPRelativePath, mpid, file.FileName);

                        MPOfUploadFiles mpfile = new MPOfUploadFiles();
                        mpfile.ID = file.ID;
                        mpfile.MPID = mpid;
                        mpfile.Name = file.FileOrginalName;
                        mpfile.FileEx = Path.GetExtension(sourceFile);
                        mpfile.DocType = file.CertificateType;
                        mpfile.State = 1;
                        mpfile.CreateTime = DateTime.Now;
                        db.MPOfUploadFiles.Add(mpfile);
                        System.IO.File.Copy(sourceFile, targetFile, true);
                    }
                    //保存后显示打印按钮

                }
                entity.State = State;
                entity.Opinion = Opinion;
                entity.IsFinish = 1;
                entity.FinishTime = DateTime.Now;
                entity.CheckUser = LoginUtils.CurrentUser.UserName;
                db.SaveChanges();
            }
            return mpid;
        }
        #endregion

        #region 农村门牌地名证明
        public static ZM_MPOFCOUNTRY GetDMZMOfCountryInitData(string ID)
        {
            ZM_MPOFCOUNTRY entity = null;
            if (!string.IsNullOrEmpty(ID))
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    entity = db.ZM_MPOFCOUNTRY.Find(ID);
                    if (entity == null) throw new Error("未能找到指定的数据！");
                    entity.tdz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.TDZ);
                    entity.qqz = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.QQZ);
                }
            }
            return entity;
        }
        public static void ModifyMPZMOfCountry(string ID, string Json)
        {
            ZM_MPOFCOUNTRY targetData = null;
            using (var db = SystemUtils.NewEFDbContext)
            {
                targetData = db.ZM_MPOFCOUNTRY.Find(ID);
                if (targetData != null)
                {
                    ObjectReflection.ModifyEntity<ZM_MPOFCOUNTRY>(targetData, Json);

                    var tdz = System.Web.HttpContext.Current.Request.Files.GetMultiple("tdz");
                    foreach (var f in tdz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMZM_NC, targetData.ID, Enums.SPFileCertificateTypes.TDZ);
                    }
                    var qqz = System.Web.HttpContext.Current.Request.Files.GetMultiple("qqz");
                    foreach (var f in qqz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMZM_NC, targetData.ID, Enums.SPFileCertificateTypes.YYZZ);
                    }
                    db.SaveChanges();
                }
            }
        }
        public static string CheckMPZMOfCountry(string ID, string State, string Opinion)
        {
            var mpid = string.Empty;
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.ZM_MPOFCOUNTRY.Find(ID);
                if (State == Enums.SPState.tg) //通过
                {
                    var isnew = CountryMPModify.CheckCountryMPIsAvailable(entity.ID, entity.CountyID, entity.NeighborhoodsID, entity.CommunityName, entity.VillageName, entity.MPNumber, entity.HSNumber);
                    if (isnew)//新门牌，需新增
                    {
                        MPOfCountry n_entity = new Models.Entities.MPOfCountry();
                        n_entity.ID = Guid.NewGuid().ToString();
                        n_entity.CountyID = entity.CountyID;
                        n_entity.NeighborhoodsID = entity.NeighborhoodsID;
                        n_entity.CommunityName = entity.CommunityName;
                        n_entity.ViligeName = entity.VillageName;
                        n_entity.MPNumber = entity.MPNumber;
                        n_entity.MPPosition = (entity.MPPositionX != 0 && entity.MPPositionY != 0 && entity.MPPositionX != null && entity.MPPositionY != null) ? (DbGeography.FromText($"POINT({entity.MPPositionX} {entity.MPPositionY})")) : null;
                        n_entity.OriginalMPAddress = entity.OriginalMPAddress;
                        n_entity.HSNumber = entity.HSNumber;
                        n_entity.Postcode = entity.Postcode;
                        n_entity.PropertyOwner = entity.PropertyOwner;
                        n_entity.IDType = entity.IDType;
                        n_entity.IDNumber = entity.IDNumber;
                        n_entity.TDZAddress = entity.TDZAddress;
                        n_entity.TDZNumber = entity.TDZNumber;
                        n_entity.QQZAddress = entity.QQZAddress;
                        n_entity.QQZNumber = entity.QQZNumber;
                        n_entity.OtherAddress = entity.OtherAddress;
                        n_entity.Applicant = entity.Applicant;
                        n_entity.ApplicantPhone = entity.ApplicantPhone;
                        n_entity.BZTime = DateTime.Now;
                        n_entity.AddType = Enums.MPAddType.LX;
                        n_entity.MPProduce = Enums.MPProduce.NO;
                        n_entity.MPZPrintComplete = Enums.Complete.NO;
                        n_entity.DZZMPrintComplete = Enums.Complete.NO;
                        n_entity.State = Enums.UseState.Enable;
                        n_entity.CreateTime = DateTime.Now;
                        n_entity.CreateUser = LoginUtils.CurrentUser.UserName;
                        n_entity.SBLY = entity.SBLY;
                        n_entity.ProjID = entity.ProjID;

                        #region 地址编码前10位拼接
                        var CountyCode = db.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == n_entity.CountyID).Select(t => t.Code).FirstOrDefault();
                        var NeighborhoodsCode = db.District.Where(t => t.State == Enums.UseState.Enable).Where(t => t.ID == n_entity.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();
                        var mpCategory = SystemUtils.Config.MPCategory.Residence.Value.ToString();
                        var year = DateTime.Now.Year.ToString();
                        var dm = CountyCode + NeighborhoodsCode + year + mpCategory;
                        SqlParameter idx = new SqlParameter("@idx", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output };
                        db.Database.ExecuteSqlCommand("exec getcode @code,@idx output;", new SqlParameter("@code", dm), idx);
                        var index = (int)idx.Value;
                        n_entity.AddressCoding = dm + index.ToString().PadLeft(5, '0');
                        #endregion

                        #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                        var CommunityDic = new CommunityDic();
                        CommunityDic.CountyID = n_entity.CountyID;
                        CommunityDic.NeighborhoodsID = n_entity.NeighborhoodsID;
                        CommunityDic.CommunityName = n_entity.CommunityName;
                        DicUtils.AddCommunityDic(CommunityDic);
                        #endregion

                        #region 标准地址拼接 市辖区+镇街道+村社区+小区名+门牌号+宿舍名+幢号+单元号+户室号
                        var CountyName = n_entity.NeighborhoodsID.Split('.')[1];
                        var NeighborhoodsName = n_entity.NeighborhoodsID.Split('.')[2];
                        var CommunityName = n_entity.CommunityName;
                        var HSNumber1 = string.IsNullOrEmpty(n_entity.HSNumber) ? string.Empty : n_entity.HSNumber + "室";
                        var StandardAddress = "嘉兴市" + CountyName + NeighborhoodsName + CommunityName + n_entity.ViligeName + n_entity.MPNumber + "号" + HSNumber1;
                        n_entity.StandardAddress = StandardAddress;
                        #endregion

                        db.MPOfCountry.Add(n_entity);

                        mpid = n_entity.ID;
                    }
                    else//老门牌
                    {
                        var o_entity = db.MPOfCountry.Where(t => t.CountyID == entity.CountyID && t.NeighborhoodsID == entity.NeighborhoodsID && t.CommunityName == entity.CommunityName && t.ViligeName == entity.VillageName && t.MPNumber == entity.MPNumber && t.HSNumber == entity.HSNumber).FirstOrDefault();
                        o_entity.TDZAddress = entity.TDZAddress == null ? o_entity.TDZAddress : entity.TDZAddress;
                        o_entity.TDZNumber = entity.TDZNumber == null ? o_entity.TDZNumber : entity.TDZNumber;
                        o_entity.QQZAddress = entity.QQZAddress == null ? o_entity.QQZAddress : entity.QQZAddress;
                        o_entity.QQZNumber = entity.QQZNumber == null ? o_entity.QQZNumber : entity.QQZNumber;
                        o_entity.OtherAddress = entity.OtherAddress == null ? o_entity.OtherAddress : entity.OtherAddress;

                        mpid = o_entity.ID;
                    }
                    //拷贝文件
                    var files = db.FILE.Where(t => t.IsValid == 1 && t.FormID == ID).ToList();
                    foreach (var file in files)
                    {
                        string sourceFile = Path.Combine(StaticVariable.basePathSBFile, file.BusinessType, file.FormID, file.CertificateType, file.FileName);
                        string targetFile = Path.Combine(StaticVariable.basePath, StaticVariable.countryMPRelativePath, mpid, file.FileName);

                        MPOfUploadFiles mpfile = new MPOfUploadFiles();
                        mpfile.ID = file.ID;
                        mpfile.MPID = mpid;
                        mpfile.Name = file.FileOrginalName;
                        mpfile.FileEx = Path.GetExtension(sourceFile);
                        mpfile.DocType = file.CertificateType;
                        mpfile.State = 1;
                        mpfile.CreateTime = DateTime.Now;
                        db.MPOfUploadFiles.Add(mpfile);
                        System.IO.File.Copy(sourceFile, targetFile, true);
                    }
                    //保存后显示打印按钮
                }
                entity.State = State;
                entity.Opinion = Opinion;
                entity.IsFinish = 1;
                entity.FinishTime = DateTime.Now;
                entity.CheckUser = LoginUtils.CurrentUser.UserName;
                db.SaveChanges();
            }
            return mpid;
        }
        #endregion

        #region 出具意见
        public static BA_DMOFZYSS GetDMBAOfZYSSInitData(string ID)
        {
            BA_DMOFZYSS entity = null;
            if (!string.IsNullOrEmpty(ID))
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    entity = db.BA_DMOFZYSS.Find(ID);
                    if (entity == null) throw new Error("未能找到指定的数据！");
                    entity.sqb = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.SQB);
                    entity.sjt = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.SJT);
                    entity.lxpfs = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.LXPFS);
                }
            }
            return entity;
        }
        public static void ModifyDMOfZYSS(string ID, string Json)
        {
            BA_DMOFZYSS targetData = null;
            using (var db = SystemUtils.NewEFDbContext)
            {
                targetData = db.BA_DMOFZYSS.Find(ID);
                if (targetData != null)
                {
                    ObjectReflection.ModifyEntity<BA_DMOFZYSS>(targetData, Json);

                    var sqb = System.Web.HttpContext.Current.Request.Files.GetMultiple("sqb");
                    foreach (var f in sqb)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.CJYJ_ZYSS, targetData.ID, Enums.SPFileCertificateTypes.SQB);
                    }
                    var sjt = System.Web.HttpContext.Current.Request.Files.GetMultiple("sjt");
                    foreach (var f in sjt)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.CJYJ_ZYSS, targetData.ID, Enums.SPFileCertificateTypes.SJT);
                    }
                    var lxpfs = System.Web.HttpContext.Current.Request.Files.GetMultiple("lxpfs");
                    foreach (var f in lxpfs)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.CJYJ_ZYSS, targetData.ID, Enums.SPFileCertificateTypes.LXPFS);
                    }
                    db.SaveChanges();
                }
            }
        }
        public static void CheckDMOfZYSS(string ID, string State, string Opinion)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.BA_DMOFZYSS.Find(ID);
                if (State == Enums.SPState.tg) //通过，新增
                {
                    DMOFZYSS n_entity = new Models.Entities.DMOFZYSS();
                    n_entity.ID = Guid.NewGuid().ToString();
                    n_entity.CountyID = entity.CountyID;
                    n_entity.NeighborhoodsID = entity.NeighborhoodsID;
                    n_entity.CommunityName = entity.CommunityName;
                    n_entity.Name = entity.Name;
                    n_entity.Pinyin = entity.Pinyin;
                    n_entity.ZMPinyin = entity.ZMPinyin;
                    n_entity.ZYSSType = entity.ZYSSType;
                    n_entity.SmallType = entity.SmallType;
                    n_entity.DMType = entity.DMType;
                    n_entity.XYDM = entity.XYDM;
                    n_entity.XMAddress = entity.XMAddress;
                    n_entity.East = entity.East;
                    n_entity.South = entity.South;
                    n_entity.West = entity.West;
                    n_entity.North = entity.North;
                    n_entity.Geom = (entity.Lng != 0 && entity.Lat != 0 && entity.Lng != null && entity.Lat != null) ? (DbGeography.FromText($"POINT({entity.Lng} {entity.Lat})")) : null;
                    n_entity.DLST = entity.DLST;
                    n_entity.DMHY = entity.DMHY;
                    n_entity.Applicant = entity.Applicant;
                    n_entity.Telephone = entity.Telephone;
                    n_entity.ContractAddress = entity.ContractAddress;
                    n_entity.SBDW = entity.SBDW;
                    n_entity.ZGDW = entity.ZGDW;
                    n_entity.ApplicantDate = entity.ApplicantDate;
                    n_entity.RecordDate = entity.RecordDate;
                    n_entity.State = Enums.UseState.Enable;
                    n_entity.CreateTime = DateTime.Now;
                    n_entity.CreateUser = LoginUtils.CurrentUser.UserName;
                    n_entity.SBLY = entity.SBLY;
                    n_entity.ProjID = entity.ProjID;

                    #region 检查这个行政区下社区名是否在字典表中存在，若不存在就新增
                    var CommunityDic = new CommunityDic();
                    CommunityDic.CountyID = n_entity.CountyID;
                    CommunityDic.NeighborhoodsID = n_entity.NeighborhoodsID;
                    CommunityDic.CommunityName = n_entity.CommunityName;
                    DicUtils.AddCommunityDic(CommunityDic);
                    #endregion

                    db.ZYSS.Add(n_entity);

                    //拷贝文件
                    var files = db.FILE.Where(t => t.IsValid == 1 && t.FormID == ID).ToList();
                    foreach (var file in files)
                    {
                        string sourceFile = Path.Combine(StaticVariable.basePathSBFile, file.BusinessType, file.FormID, file.CertificateType, file.FileName);
                        string targetFile = Path.Combine(StaticVariable.basePath, StaticVariable.ProfessionalDMRelativePath, n_entity.ID, file.FileName);
                        DMOfUploadFiles mpfile = new DMOfUploadFiles();
                        mpfile.ID = file.ID;
                        mpfile.DMID = n_entity.ID;
                        mpfile.Name = file.FileOrginalName;
                        mpfile.FileEx = Path.GetExtension(sourceFile);
                        mpfile.DocType = file.CertificateType;
                        mpfile.State = 1;
                        mpfile.CreateTime = DateTime.Now;
                        db.DMOfUploadFiles.Add(mpfile);
                        System.IO.File.Copy(sourceFile, targetFile, true);
                    }
                }
                else
                { }
                entity.State = State;
                entity.Opinion = Opinion;
                entity.IsFinish = 1;
                entity.FinishTime = DateTime.Now;
                entity.CheckUser = LoginUtils.CurrentUser.UserName;
                db.SaveChanges();
            }
        }

        #endregion

        #region 居民点申报
        public static SB_DMOFSETTLEMENT GetDMSBOfSettlementInitData(string ID)
        {
            SB_DMOFSETTLEMENT entity = null;
            if (!string.IsNullOrEmpty(ID))
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    if (entity == null) throw new Error("未能找到指定的数据！");
                    entity.jsydxkzs = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.JSYDXKZ);
                    entity.jsgcghxkzs = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.JSGCGHXKZ);
                    entity.zpmts = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.ZPMT);
                    entity.xgts = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.XGT);
                }
            }
            return entity;
        }
        public static void ModifyDMOfSettlement(string ID, string Json)
        {
            SB_DMOFSETTLEMENT targetData = null;
            using (var db = SystemUtils.NewEFDbContext)
            {
                targetData = db.SB_DMOFSETTLEMENT.Find(ID);
                if (targetData != null)
                {
                    ObjectReflection.ModifyEntity<SB_DMOFSETTLEMENT>(targetData, Json);

                    var jsydxkzs = System.Web.HttpContext.Current.Request.Files.GetMultiple("jsydxkzs");
                    foreach (var f in jsydxkzs)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMHZ_JMD, targetData.ID, Enums.SPFileCertificateTypes.JSYDXKZ);
                    }
                    var jsgcghxkzs = System.Web.HttpContext.Current.Request.Files.GetMultiple("jsgcghxkzs");
                    foreach (var f in jsgcghxkzs)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMHZ_JMD, targetData.ID, Enums.SPFileCertificateTypes.JSGCGHXKZ);
                    }
                    var zpmts = System.Web.HttpContext.Current.Request.Files.GetMultiple("zpmts");
                    foreach (var f in zpmts)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMHZ_JMD, targetData.ID, Enums.SPFileCertificateTypes.ZPMT);
                    }
                    var xgts = System.Web.HttpContext.Current.Request.Files.GetMultiple("xgts");
                    foreach (var f in xgts)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMHZ_JMD, targetData.ID, Enums.SPFileCertificateTypes.XGT);
                    }
                    db.SaveChanges();
                }
            }
        }
        #endregion

        #region 建筑物申报
        public static SB_DMOFBUILDING GetDMSBOfBuildingInitData(string ID)
        {
            SB_DMOFBUILDING entity = null;
            if (!string.IsNullOrEmpty(ID))
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    if (entity == null) throw new Error("未能找到指定的数据！");
                    entity.jsydxkzs = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.JSYDXKZ);
                    entity.jsgcghxkzs = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.JSGCGHXKZ);
                    entity.zpmts = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.ZPMT);
                    entity.xgts = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.XGT);
                }
            }
            return entity;
        }
        public static void ModifyDMOfBuilding(string ID, string Json)
        {
            SB_DMOFBUILDING targetData = null;
            using (var db = SystemUtils.NewEFDbContext)
            {
                targetData = db.SB_DMOFBUILDING.Find(ID);
                if (targetData != null)
                {
                    ObjectReflection.ModifyEntity<SB_DMOFBUILDING>(targetData, Json);

                    var jsydxkzs = System.Web.HttpContext.Current.Request.Files.GetMultiple("jsydxkzs");
                    foreach (var f in jsydxkzs)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMHZ_JZW, targetData.ID, Enums.SPFileCertificateTypes.JSYDXKZ);
                    }
                    var jsgcghxkzs = System.Web.HttpContext.Current.Request.Files.GetMultiple("jsgcghxkzs");
                    foreach (var f in jsgcghxkzs)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMHZ_JZW, targetData.ID, Enums.SPFileCertificateTypes.JSGCGHXKZ);
                    }
                    var zpmts = System.Web.HttpContext.Current.Request.Files.GetMultiple("zpmts");
                    foreach (var f in zpmts)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMHZ_JZW, targetData.ID, Enums.SPFileCertificateTypes.ZPMT);
                    }
                    var xgts = System.Web.HttpContext.Current.Request.Files.GetMultiple("xgts");
                    foreach (var f in xgts)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMHZ_JZW, targetData.ID, Enums.SPFileCertificateTypes.XGT);
                    }
                    db.SaveChanges();
                }
            }
        }
        #endregion

        #region 道路街巷申报
        public static SB_DMOFROAD GetDMSBOfRoadInitData(string ID)
        {
            SB_DMOFROAD entity = null;
            if (!string.IsNullOrEmpty(ID))
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    if (entity == null) throw new Error("未能找到指定的数据！");
                    entity.lxpfss = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.LXPFS);
                    entity.dltzs = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.DLTZ);
                }
            }
            return entity;
        }
        public static void ModifyDMOfRoad(string ID, string Json)
        {
            SB_DMOFROAD targetData = null;
            using (var db = SystemUtils.NewEFDbContext)
            {
                targetData = db.SB_DMOFROAD.Find(ID);
                if (targetData != null)
                {
                    ObjectReflection.ModifyEntity<SB_DMOFROAD>(targetData, Json);

                    var lxpfss = System.Web.HttpContext.Current.Request.Files.GetMultiple("lxpfss");
                    foreach (var f in lxpfss)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMHZ_DLJX, targetData.ID, Enums.SPFileCertificateTypes.LXPFS);
                    }
                    var dltzs = System.Web.HttpContext.Current.Request.Files.GetMultiple("dltzs");
                    foreach (var f in dltzs)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMHZ_DLJX, targetData.ID, Enums.SPFileCertificateTypes.DLTZ);
                    }
                    db.SaveChanges();
                }
            }
        }
        #endregion

        #region 桥梁申报
        public static SB_DMOFBRIDGE GetDMSBOfBridgeInitData(string ID)
        {
            SB_DMOFBRIDGE entity = null;
            if (!string.IsNullOrEmpty(ID))
            {
                using (var db = SystemUtils.NewEFDbContext)
                {
                    if (entity == null) throw new Error("未能找到指定的数据！");
                    entity.lxpfss = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.LXPFS);
                    entity.qltzs = SPItemFileUtils.GetFiles(ID, Enums.SPFileCertificateTypes.QLTZ);
                }
            }
            return entity;
        }
        public static void ModifyDMOfBridge(string ID, string Json)
        {
            SB_DMOFBRIDGE targetData = null;
            using (var db = SystemUtils.NewEFDbContext)
            {
                targetData = db.SB_DMOFBRIDGE.Find(ID);
                if (targetData != null)
                {
                    ObjectReflection.ModifyEntity<SB_DMOFBRIDGE>(targetData, Json);

                    var lxpfss = System.Web.HttpContext.Current.Request.Files.GetMultiple("lxpfss");
                    foreach (var f in lxpfss)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMHZ_QL, targetData.ID, Enums.SPFileCertificateTypes.LXPFS);
                    }
                    var dltzs = System.Web.HttpContext.Current.Request.Files.GetMultiple("dltzs");
                    foreach (var f in dltzs)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMHZ_QL, targetData.ID, Enums.SPFileCertificateTypes.DLTZ);
                    }
                    db.SaveChanges();
                }
            }
        }
        #endregion

    }

    public class ItemFather
    {
        public string ID { get; set; }
        public string SBLY { get; set; }
        public DateTime? SQSJ { get; set; }
        public DateTime? SPSJ { get; set; }
        public string SIGN { get; set; }
    }
    public class MPItem : ItemFather
    {
        public string YWLX { get; set; }
        public string MPLX { get; set; }
        public string CQR { get; set; }
    }
    public class DMHZItem : ItemFather
    {
        public string DMLB { get; set; }
        public string XLLB { get; set; }
        public string NYMC { get; set; }
    }
    public class DMZMItem : ItemFather
    {
        public string MPLX { get; set; }
        public string MC { get; set; }
        public string HM { get; set; }
    }
    public class ZYSSItem : ItemFather
    {
        public string LB { get; set; }
        public string DMLB { get; set; }
        public string XLLB { get; set; }
        public string MC { get; set; }
    }
}