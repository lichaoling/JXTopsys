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

    }
    public class HomePage
    {
        public static Dictionary<string, object> GetHomePageData()
        {
            using (var dbContext = SystemUtils.NewEFDbContext)
            {
                if (LoginUtils.CurrentUser.DistrictIDList == null || LoginUtils.CurrentUser.DistrictIDList.Count == 0)
                    throw new Error("该用户没有任何数据权限，请联系管理员！");

                string mpsql1 = "", dmhzsql1 = "", dmzmsql1 = "", cjyjsql1 = "";
                string mpsql2 = "", dmhzsql2 = "", dmzmsql2 = "", cjyjsql2 = "";
                string mpsql3 = "", dmhzsql3 = "", dmzmsql3 = "", cjyjsql3 = "";
                // 先删选出当前用户权限内的数据
                if (!LoginUtils.CurrentUser.DistrictIDList.Contains("嘉兴市"))
                {
                    foreach (var userDID in LoginUtils.CurrentUser.DistrictIDList)
                    {
                        #region 待办事项
                        mpsql1 += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                left  join MPOFCOUNTRY a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                left  join MPOFRESIDENCE a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1
                                union 
                                select b.ID from BG_MPOFROAD b 
                                left  join MPOFROAD a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 ";

                        dmhzsql1 += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 ";

                        dmzmsql1 += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 ";

                        cjyjsql1 += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 ";

                        #endregion

                        #region 已办事项
                        mpsql2 += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                left  join MPOFCOUNTRY a on b.MPID=a.ID 
                                where b.IsFinish=1 and CHARINDEX({userDID}, a.NeighborhoodsID)=1
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                left  join MPOFRESIDENCE a on b.MPID=a.ID 
                                where b.IsFinish=1 and CHARINDEX({userDID}, a.NeighborhoodsID)=1
                                union 
                                select b.ID from BG_MPOFROAD b 
                                left  join MPOFROAD a on b.MPID=a.ID 
                                where b.IsFinish=1 and CHARINDEX({userDID}, a.NeighborhoodsID)=1
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=1 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=1 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=1 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 ";

                        dmhzsql2 += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=1 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=1 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=1 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=1 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 ";

                        dmzmsql2 += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=1 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=1 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=1 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 ";

                        cjyjsql2 += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=1 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 ";
                        #endregion
                    }
                }
                else
                {
                    #region 待办事项
                    mpsql1 += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                where b.IsFinish=0
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                where b.IsFinish=0
                                union 
                                select b.ID from BG_MPOFROAD b 
                                where b.IsFinish=0
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=0";

                    dmhzsql1 += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=0
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=0
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0";

                    dmzmsql1 += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=0";

                    cjyjsql1 += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=0";

                    #endregion

                    #region 已办事项
                    mpsql2 += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                where b.IsFinish=1
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                where b.IsFinish=1
                                union 
                                select b.ID from BG_MPOFROAD b 
                                where b.IsFinish=1
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=1
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=1
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=1";

                    dmhzsql2 += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=1
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=1
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=1
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=1";

                    dmzmsql2 += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=1
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=1
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=1";

                    cjyjsql2 += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=1";
                    #endregion
                }

                #region 个人已办事项
                mpsql3 += $@"union 
                                select b.ID from BG_MPOFCOUNTRY b 
                                where b.IsFinish=1 and b.checkuser={LoginUtils.CurrentUser.UserName}
                                union 
                                select b.ID from BG_MPOFRESIDENCE b 
                                where b.IsFinish=1 and b.checkuser={LoginUtils.CurrentUser.UserName}
                                union 
                                select b.ID from BG_MPOFROAD b 
                                where b.IsFinish=1 and b.checkuser={LoginUtils.CurrentUser.UserName}
                                union 
                                select b.ID from SB_MPOFCOUNTRY b 
                                where b.IsFinish=1 and b.checkuser={LoginUtils.CurrentUser.UserName}
                                union 
                                select b.ID from SB_MPOFRESIDENCE b 
                                where b.IsFinish=1 and b.checkuser={LoginUtils.CurrentUser.UserName}
                                union 
                                select b.ID from SB_MPOFROAD b 
                                where b.IsFinish=1 and b.checkuser={LoginUtils.CurrentUser.UserName} ";

                dmhzsql3 += $@"union 
                                   select b.ID from SB_DMOFBRIDGE b 
                                   where b.IsFinish=1 and b.checkuser={LoginUtils.CurrentUser.UserName}
                                   union 
                                   select b.ID from SB_DMOFBUILDING b 
                                   where b.IsFinish=1 and b.checkuser={LoginUtils.CurrentUser.UserName}
                                   union 
                                   select b.ID from SB_DMOFROAD b 
                                   where b.IsFinish=1 and b.checkuser={LoginUtils.CurrentUser.UserName}
                                   union 
                                   select b.ID from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=1 and b.checkuser={LoginUtils.CurrentUser.UserName} ";

                dmzmsql3 += $@"union 
                                   select b.ID from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=1 and b.checkuser={LoginUtils.CurrentUser.UserName}
                                   union 
                                   select b.ID from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=1 and b.checkuser={LoginUtils.CurrentUser.UserName}
                                   union 
                                   select b.ID from ZM_MPOFROAD b 
                                   where b.IsFinish=1 and b.checkuser={LoginUtils.CurrentUser.UserName} ";

                cjyjsql3 += $@"union 
                                   select b.ID from BA_DMOFZYSS b 
                                   where b.IsFinish=1 and b.checkuser={LoginUtils.CurrentUser.UserName} ";
                #endregion

                mpsql1 = mpsql1.Substring("union ".Length);
                dmhzsql1 = dmhzsql1.Substring("union ".Length);
                dmzmsql1 = dmzmsql1.Substring("union ".Length);
                cjyjsql1 = cjyjsql1.Substring("union ".Length);

                mpsql2 = mpsql2.Substring("union ".Length);
                dmhzsql2 = dmhzsql2.Substring("union ".Length);
                dmzmsql2 = dmzmsql2.Substring("union ".Length);
                cjyjsql2 = cjyjsql2.Substring("union ".Length);

                mpsql3 = mpsql3.Substring("union ".Length);
                dmhzsql3 = dmhzsql3.Substring("union ".Length);
                dmzmsql3 = dmzmsql3.Substring("union ".Length);
                cjyjsql3 = cjyjsql3.Substring("union ".Length);

                Items toDoItem = new Items();
                toDoItem.MP = dbContext.Database.SqlQuery<int>($"select count(1) from ({mpsql1}) s").FirstOrDefault();
                toDoItem.DMHZ = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmhzsql1}) s").FirstOrDefault();
                toDoItem.DMZM = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmzmsql1}) s").FirstOrDefault();
                toDoItem.CJYJ = dbContext.Database.SqlQuery<int>($"select count(1) from ({cjyjsql1}) s").FirstOrDefault();

                Items doneItem = new Items();
                doneItem.MP = dbContext.Database.SqlQuery<int>($"select count(1) from ({mpsql2}) s").FirstOrDefault();
                doneItem.DMHZ = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmhzsql2}) s").FirstOrDefault();
                doneItem.DMZM = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmzmsql2}) s").FirstOrDefault();
                doneItem.CJYJ = dbContext.Database.SqlQuery<int>($"select count(1) from ({cjyjsql2}) s").FirstOrDefault();

                Items PersonalDoneItem = new Items();
                PersonalDoneItem.MP = dbContext.Database.SqlQuery<int>($"select count(1) from ({mpsql3}) s").FirstOrDefault();
                PersonalDoneItem.DMHZ = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmhzsql3}) s").FirstOrDefault();
                PersonalDoneItem.DMZM = dbContext.Database.SqlQuery<int>($"select count(1) from ({dmzmsql3}) s").FirstOrDefault();
                PersonalDoneItem.CJYJ = dbContext.Database.SqlQuery<int>($"select count(1) from ({cjyjsql3}) s").FirstOrDefault();

                return new Dictionary<string, object> {
                   { "toDoItem",toDoItem},
                   { "doneItem",doneItem},
                   { "PersonalDoneItem",PersonalDoneItem}
                };
            }
        }

        public static Dictionary<string, object> GetTodoItems(int isFinish)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                string mpsql1 = "", dmhzsql1 = "", dmzmsql1 = "", cjyjsql1 = "";
                if (!LoginUtils.CurrentUser.DistrictIDList.Contains("嘉兴市"))
                {
                    foreach (var userDID in LoginUtils.CurrentUser.DistrictIDList)
                    {
                        #region 待办事项
                        mpsql1 += $@"union 
                                select b.ID,b.sbly,'门牌变更' YWLX,'农村门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from BG_MPOFCOUNTRY b 
                                left  join MPOFCOUNTRY a on b.MPID=a.ID 
                                where b.IsFinish={isFinish} and CHARINDEX({userDID}, a.NeighborhoodsID)=1
                                union 
                                select b.ID,b.sbly,'门牌变更' YWLX,'住宅门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from BG_MPOFRESIDENCE b 
                                left  join MPOFRESIDENCE a on b.MPID=a.ID 
                                where b.IsFinish={isFinish} and CHARINDEX({userDID}, a.NeighborhoodsID)=1
                                union 
                                select b.ID,b.sbly,'门牌变更' YWLX,'道路门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from BG_MPOFROAD b 
                                left  join MPOFROAD a on b.MPID=a.ID 
                                where b.IsFinish={isFinish} and CHARINDEX({userDID}, a.NeighborhoodsID)=1
                                union 
                                select b.ID,b.sbly,'门牌申请' YWLX,'农村门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from SB_MPOFCOUNTRY b 
                                where b.IsFinish={isFinish} and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                union 
                                select b.ID,b.sbly,'门牌申请' YWLX,'住宅门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from SB_MPOFRESIDENCE b 
                                where b.IsFinish={isFinish} and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                union 
                                select b.ID,b.sbly,'门牌申请' YWLX,'道路门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from SB_MPOFROAD b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 ";

                        dmhzsql1 += $@"union 
                                   select b.ID,b.sbly,'桥梁' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ from SB_DMOFBRIDGE b 
                                   where b.IsFinish={isFinish} and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID,b.sbly,'建筑物' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ from SB_DMOFBUILDING b 
                                   where b.IsFinish={isFinish} and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID,b.sbly,'道路街巷' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ from SB_DMOFROAD b 
                                   where b.IsFinish={isFinish} and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID,b.sbly,'居民点' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish={isFinish} and CHARINDEX({userDID}, b.NeighborhoodsID)=1 ";

                        dmzmsql1 += $@"union 
                                   select b.ID,b.sbly,'农村门牌' MPLX,b.VillageName MC,b.MPNumber HM,b.createtime SQSJ from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish={isFinish} and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID,b.sbly,'住宅门牌' MPLX,b.ResidenceName MC,'' HM,b.createtime SQSJ from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish={isFinish} and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID,b.sbly,'道路门牌' MPLX,b.RoadName MC,b.MPNumber HM,b.createtime SQSJ from ZM_MPOFROAD b 
                                   where b.IsFinish={isFinish} and CHARINDEX({userDID}, b.NeighborhoodsID)=1 ";

                        cjyjsql1 += $@"union 
                                   select b.ID,b.sbly,b.ZYSSType LB,b.SmallType DMLB,b.DMType XLLB,b.Name MC,b.createtime SQSJ from BA_DMOFZYSS b 
                                   where b.IsFinish={isFinish} and CHARINDEX({userDID}, b.NeighborhoodsID)=1 ";

                        #endregion
                    }
                }
                else
                {
                    #region 待办事项
                    mpsql1 += $@"union 
                                select b.ID,b.sbly,'门牌变更' YWLX,'农村门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from BG_MPOFCOUNTRY b 
                                where b.IsFinish={isFinish}
                                union 
                                select b.ID,b.sbly,'门牌变更' YWLX,'住宅门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from BG_MPOFRESIDENCE b 
                                where b.IsFinish={isFinish}
                                union 
                                select b.ID,b.sbly,'门牌变更' YWLX,'道路门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from BG_MPOFROAD b 
                                where b.IsFinish={isFinish}
                                union 
                                select b.ID,b.sbly,'门牌申请' YWLX,'农村门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from SB_MPOFCOUNTRY b 
                                where b.IsFinish={isFinish}
                                union 
                                select b.ID,b.sbly,'门牌申请' YWLX,'住宅门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from SB_MPOFRESIDENCE b 
                                where b.IsFinish={isFinish}
                                union 
                                select b.ID,b.sbly,'门牌申请' YWLX,'道路门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from SB_MPOFROAD b 
                                where b.IsFinish={isFinish}";

                    dmhzsql1 += $@"union 
                                   select b.ID,b.sbly,'桥梁' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ from SB_DMOFBRIDGE b 
                                   where b.IsFinish={isFinish}
                                   union 
                                   select b.ID,b.sbly,'建筑物' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ from SB_DMOFBUILDING b 
                                   where b.IsFinish={isFinish}
                                   union 
                                   select b.ID,b.sbly,'道路街巷' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ from SB_DMOFROAD b 
                                   where b.IsFinish={isFinish}
                                   union 
                                   select b.ID,b.sbly,'居民点' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish={isFinish}";

                    dmzmsql1 += $@"union 
                                   select b.ID,b.sbly,'农村门牌' MPLX,b.VillageName MC,b.MPNumber HM,b.createtime SQSJ from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish={isFinish}
                                   union 
                                   select b.ID,b.sbly,'住宅门牌' MPLX,b.ResidenceName MC,'' HM,b.createtime SQSJ from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish={isFinish}
                                   union 
                                   select b.ID,b.sbly,'道路门牌' MPLX,b.RoadName MC,b.MPNumber HM,b.createtime SQSJ from ZM_MPOFROAD b 
                                   where b.IsFinish={isFinish}";

                    cjyjsql1 += $@"union 
                                   select b.ID,b.sbly,b.ZYSSType LB,b.SmallType DMLB,b.DMType XLLB,b.Name MC,b.createtime SQSJ from BA_DMOFZYSS b 
                                   where b.IsFinish={isFinish}";

                    #endregion
                }
                mpsql1 = mpsql1.Substring("union ".Length);
                dmhzsql1 = dmhzsql1.Substring("union ".Length);
                dmzmsql1 = dmzmsql1.Substring("union ".Length);
                cjyjsql1 = cjyjsql1.Substring("union ".Length);

                List<MPItem> mps = db.Database.SqlQuery<MPItem>(mpsql1).ToList();
                List<DMHZItem> dmhzs = db.Database.SqlQuery<DMHZItem>(dmhzsql1).ToList();
                List<DMZMItem> dmzms = db.Database.SqlQuery<DMZMItem>(dmzmsql1).ToList();
                List<ZYSSItem> cjyjs = db.Database.SqlQuery<ZYSSItem>(cjyjsql1).ToList();

                return new Dictionary<string, object> {
                   { "mps",mps},
                   { "dmhzs",dmhzs},
                   { "dmzms",dmzms},
                   { "cjyjs",cjyjs},
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
                    var fcz = System.Web.HttpContext.Current.Request.Files.GetMultiple("fcz");
                    foreach (var f in fcz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZBG_ZZ, targetData.ID, Enums.SPFileCertificateTypes.FCZ);
                    }
                    var tdz = System.Web.HttpContext.Current.Request.Files.GetMultiple("tdz");
                    foreach (var f in tdz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZBG_ZZ, targetData.ID, Enums.SPFileCertificateTypes.TDZ);
                    }
                    var bdcz = System.Web.HttpContext.Current.Request.Files.GetMultiple("bdcz");
                    foreach (var f in bdcz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZBG_ZZ, targetData.ID, Enums.SPFileCertificateTypes.BDCZ);
                    }
                    var hj = System.Web.HttpContext.Current.Request.Files.GetMultiple("hj");
                    foreach (var f in hj)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZBG_ZZ, targetData.ID, Enums.SPFileCertificateTypes.HJ);
                    }
                    db.SaveChanges();
                }

            }
        }
        public static void CheckMPBGOfResidence(string ID, int State)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.BG_MPOFRESIDENCE.Find(ID);
                var o_entity = db.MPOfResidence.Find(entity.MPID);
                if (State == 1)
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
                entity.IsFinish = 1;
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
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZSQ_ZZ, targetData.ID, Enums.SPFileCertificateTypes.FCZ);
                    }
                    var tdz = System.Web.HttpContext.Current.Request.Files.GetMultiple("tdz");
                    foreach (var f in tdz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZSQ_ZZ, targetData.ID, Enums.SPFileCertificateTypes.TDZ);
                    }
                    var bdcz = System.Web.HttpContext.Current.Request.Files.GetMultiple("bdcz");
                    foreach (var f in bdcz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZSQ_ZZ, targetData.ID, Enums.SPFileCertificateTypes.BDCZ);
                    }
                    var hj = System.Web.HttpContext.Current.Request.Files.GetMultiple("hj");
                    foreach (var f in hj)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZSQ_ZZ, targetData.ID, Enums.SPFileCertificateTypes.HJ);
                    }
                    db.SaveChanges();
                }
            }
        }
        public static void CheckMPOfResidence(string ID, int State)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.SB_MPOFRESIDENCE.Find(ID);
                if (State == 1) //通过，新增
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
                    var MPNumber1 = n_entity.MPNumber == null ? "" : n_entity.MPNumber + "号";
                    var LZNumber1 = n_entity.LZNumber == null ? "" : n_entity.LZNumber + "幢";
                    var DYNumber1 = n_entity.DYNumber == null ? "" : n_entity.DYNumber + "单元";
                    var HSNumber1 = n_entity.HSNumber == null ? "" : n_entity.HSNumber + "室";
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
                entity.IsFinish = 1;
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
                    var fcz = System.Web.HttpContext.Current.Request.Files.GetMultiple("fcz");
                    foreach (var f in fcz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZBG_DL, targetData.ID, Enums.SPFileCertificateTypes.FCZ);
                    }
                    var tdz = System.Web.HttpContext.Current.Request.Files.GetMultiple("tdz");
                    foreach (var f in tdz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZBG_DL, targetData.ID, Enums.SPFileCertificateTypes.TDZ);
                    }
                    var yyzz = System.Web.HttpContext.Current.Request.Files.GetMultiple("yyzz");
                    foreach (var f in yyzz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZBG_DL, targetData.ID, Enums.SPFileCertificateTypes.YYZZ);
                    }
                    db.SaveChanges();
                }
            }
        }
        public static void CheckMPBGOfRoad(string ID, int State)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.BG_MPOFROAD.Find(ID);
                var o_entity = db.MPOfRoad.Find(entity.MPID);
                if (State == 1)
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
                entity.IsFinish = 1;
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
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZSQ_DL, targetData.ID, Enums.SPFileCertificateTypes.FCZ);
                    }
                    var tdz = System.Web.HttpContext.Current.Request.Files.GetMultiple("tdz");
                    foreach (var f in tdz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZSQ_DL, targetData.ID, Enums.SPFileCertificateTypes.TDZ);
                    }
                    var yyzz = System.Web.HttpContext.Current.Request.Files.GetMultiple("yyzz");
                    foreach (var f in yyzz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZSQ_DL, targetData.ID, Enums.SPFileCertificateTypes.YYZZ);
                    }
                    db.SaveChanges();
                }
            }
        }
        public static void CheckMPOfRoad(string ID, int State)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.SB_MPOFROAD.Find(ID);
                if (State == 1) //通过，新增
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
                    var CountyName = entity.NeighborhoodsID.Split('.')[1];
                    var NeighborhoodsName = entity.NeighborhoodsID.Split('.')[2];
                    var CommunityName = entity.CommunityName;
                    var StandardAddress = "嘉兴市" + CountyName + NeighborhoodsName + entity.RoadName + entity.MPNumber + "号";
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
                entity.IsFinish = 1;
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
                    var tdz = System.Web.HttpContext.Current.Request.Files.GetMultiple("tdz");
                    foreach (var f in tdz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZBG_NC, targetData.ID, Enums.SPFileCertificateTypes.TDZ);
                    }
                    var qqz = System.Web.HttpContext.Current.Request.Files.GetMultiple("qqz");
                    foreach (var f in qqz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZBG_NC, targetData.ID, Enums.SPFileCertificateTypes.QQZ);
                    }
                    db.SaveChanges();
                }
            }
        }
        public static void CheckMPBGOfCountry(string ID, int State)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.BG_MPOFCOUNTRY.Find(ID);
                var o_entity = db.MPOfCountry.Find(entity.MPID);
                if (State == 1)
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
                entity.IsFinish = 1;
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
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZSQ_NC, targetData.ID, Enums.SPFileCertificateTypes.TDZ);
                    }
                    var qqz = System.Web.HttpContext.Current.Request.Files.GetMultiple("qqz");
                    foreach (var f in qqz)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.MPZSQ_NC, targetData.ID, Enums.SPFileCertificateTypes.QQZ);
                    }
                    db.SaveChanges();
                }
            }
        }
        public static void CheckMPOfCountry(string ID, int State)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.SB_MPOFCOUNTRY.Find(ID);
                if (State == 1) //通过，新增
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
                    var CountyName = entity.NeighborhoodsID.Split('.')[1];
                    var NeighborhoodsName = entity.NeighborhoodsID.Split('.')[2];
                    var CommunityName = entity.CommunityName;
                    var HSNumber1 = entity.HSNumber == null ? string.Empty : entity.HSNumber + "室";
                    var StandardAddress = "嘉兴市" + CountyName + NeighborhoodsName + CommunityName + entity.VillageName + entity.MPNumber + "号" + HSNumber1;
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
                entity.IsFinish = 1;
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
        public static string CheckMPZMOfResidence(string ID, int State)
        {
            var mpid = string.Empty;
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.ZM_MPOFRESIDENCE.Find(ID);
                if (State == 1) //通过
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
                        var MPNumber1 = n_entity.MPNumber == null ? "" : n_entity.MPNumber + "号";
                        var LZNumber1 = n_entity.LZNumber == null ? "" : n_entity.LZNumber + "幢";
                        var DYNumber1 = n_entity.DYNumber == null ? "" : n_entity.DYNumber + "单元";
                        var HSNumber1 = n_entity.HSNumber == null ? "" : n_entity.HSNumber + "室";
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
        public static string CheckMPZMOfRoad(string ID, int State)
        {
            var mpid = string.Empty;
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.ZM_MPOFROAD.Find(ID);
                if (State == 1) //通过
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
                        var CountyName = entity.NeighborhoodsID.Split('.')[1];
                        var NeighborhoodsName = entity.NeighborhoodsID.Split('.')[2];
                        var CommunityName = entity.CommunityName;
                        var StandardAddress = "嘉兴市" + CountyName + NeighborhoodsName + entity.RoadName + entity.MPNumber + "号";
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
        public static string CheckMPZMOfCountry(string ID, int State)
        {
            var mpid = string.Empty;
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.ZM_MPOFCOUNTRY.Find(ID);
                if (State == 1) //通过
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
                        var CountyName = entity.NeighborhoodsID.Split('.')[1];
                        var NeighborhoodsName = entity.NeighborhoodsID.Split('.')[2];
                        var CommunityName = entity.CommunityName;
                        var HSNumber1 = entity.HSNumber == null ? string.Empty : entity.HSNumber + "室";
                        var StandardAddress = "嘉兴市" + CountyName + NeighborhoodsName + CommunityName + entity.VillageName + entity.MPNumber + "号" + HSNumber1;
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
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMBA_ZYSS, targetData.ID, Enums.SPFileCertificateTypes.SQB);
                    }
                    var sjt = System.Web.HttpContext.Current.Request.Files.GetMultiple("sjt");
                    foreach (var f in sjt)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMBA_ZYSS, targetData.ID, Enums.SPFileCertificateTypes.SJT);
                    }
                    var lxpfs = System.Web.HttpContext.Current.Request.Files.GetMultiple("lxpfs");
                    foreach (var f in lxpfs)
                    {
                        HttpPostedFileBase ff = new HttpPostedFileWrapper(f) as HttpPostedFileBase;
                        SPItemFileUtils.SaveFile(ff, Enums.SPFileBusinessTypes.DMBA_ZYSS, targetData.ID, Enums.SPFileCertificateTypes.LXPFS);
                    }
                    db.SaveChanges();
                }
            }
        }
        public static void CheckDMOfZYSS(string ID, int State)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var entity = db.BA_DMOFZYSS.Find(ID);
                if (State == 1) //通过，新增
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
                entity.IsFinish = 1;
                entity.CheckUser = LoginUtils.CurrentUser.UserName;
                db.SaveChanges();
            }
        }

        #endregion
    }
    public class MPItem
    {
        public string ID { get; set; }
        public string SBLY { get; set; }
        public string YWLX { get; set; }
        public string MPLX { get; set; }
        public string CQR { get; set; }
        public DateTime? SQSJ { get; set; }
    }
    public class DMHZItem
    {
        public string ID { get; set; }
        public string SBLY { get; set; }
        public string DMLB { get; set; }
        public string XLLB { get; set; }
        public string NYMC { get; set; }
        public DateTime? SQSJ { get; set; }
    }
    public class DMZMItem
    {
        public string ID { get; set; }
        public string SBLY { get; set; }
        public string MPLX { get; set; }
        public string MC { get; set; }
        public string HM { get; set; }
        public DateTime? SQSJ { get; set; }
    }
    public class ZYSSItem
    {
        public string ID { get; set; }
        public string SBLY { get; set; }
        public string LB { get; set; }
        public string DMLB { get; set; }
        public string XLLB { get; set; }
        public string MC { get; set; }
        public DateTime? SQSJ { get; set; }
    }
}