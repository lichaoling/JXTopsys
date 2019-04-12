//using iTextSharp.text.pdf.parser;
using JXGIS.JXTopsystem.Business.Common;
using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using System;
using System.Collections.Generic;
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

        public static Dictionary<string, object> GetTodoItems()
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
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1
                                union 
                                select b.ID,b.sbly,'门牌变更' YWLX,'住宅门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from BG_MPOFRESIDENCE b 
                                left  join MPOFRESIDENCE a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1
                                union 
                                select b.ID,b.sbly,'门牌变更' YWLX,'道路门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from BG_MPOFROAD b 
                                left  join MPOFROAD a on b.MPID=a.ID 
                                where b.IsFinish=0 and CHARINDEX({userDID}, a.NeighborhoodsID)=1
                                union 
                                select b.ID,b.sbly,'门牌申请' YWLX,'农村门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                union 
                                select b.ID,b.sbly,'门牌申请' YWLX,'住宅门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                union 
                                select b.ID,b.sbly,'门牌申请' YWLX,'道路门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from SB_MPOFROAD b 
                                where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 ";

                        dmhzsql1 += $@"union 
                                   select b.ID,b.sbly,'桥梁' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID,b.sbly,'建筑物' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ from SB_DMOFBUILDING b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID,b.sbly,'道路街巷' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ from SB_DMOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID,b.sbly,'居民点' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 ";

                        dmzmsql1 += $@"union 
                                   select b.ID,b.sbly,'农村门牌' MPLX,b.VillageName MC,b.MPNumber HM,b.createtime SQSJ from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID,b.sbly,'住宅门牌' MPLX,b.ResidenceName MC,'' HM,b.createtime SQSJ from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1
                                   union 
                                   select b.ID,b.sbly,'道路门牌' MPLX,b.RoadName MC,b.MPNumber HM,b.createtime SQSJ from ZM_MPOFROAD b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 ";

                        cjyjsql1 += $@"union 
                                   select b.ID,b.sbly,b.ZYSSType LB,b.SmallType DMLB,b.DMType XLLB,b.Name MC,b.createtime SQSJ from BA_DMOFZYSS b 
                                   where b.IsFinish=0 and CHARINDEX({userDID}, b.NeighborhoodsID)=1 ";

                        #endregion
                    }
                }
                else
                {
                    #region 待办事项
                    mpsql1 += $@"union 
                                select b.ID,b.sbly,'门牌变更' YWLX,'农村门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from BG_MPOFCOUNTRY b 
                                where b.IsFinish=0
                                union 
                                select b.ID,b.sbly,'门牌变更' YWLX,'住宅门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from BG_MPOFRESIDENCE b 
                                where b.IsFinish=0
                                union 
                                select b.ID,b.sbly,'门牌变更' YWLX,'道路门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from BG_MPOFROAD b 
                                where b.IsFinish=0
                                union 
                                select b.ID,b.sbly,'门牌申请' YWLX,'农村门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from SB_MPOFCOUNTRY b 
                                where b.IsFinish=0
                                union 
                                select b.ID,b.sbly,'门牌申请' YWLX,'住宅门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from SB_MPOFRESIDENCE b 
                                where b.IsFinish=0
                                union 
                                select b.ID,b.sbly,'门牌申请' YWLX,'道路门牌' MPLX,b.PropertyOwner CQR,b.createtime SQSJ from SB_MPOFROAD b 
                                where b.IsFinish=0";

                    dmhzsql1 += $@"union 
                                   select b.ID,b.sbly,'桥梁' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ from SB_DMOFBRIDGE b 
                                   where b.IsFinish=0
                                   union 
                                   select b.ID,b.sbly,'建筑物' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ from SB_DMOFBUILDING b 
                                   where b.IsFinish=0
                                   union 
                                   select b.ID,b.sbly,'道路街巷' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ from SB_DMOFROAD b 
                                   where b.IsFinish=0
                                   union 
                                   select b.ID,b.sbly,'居民点' DMLB,b.type XLLB,b.name1 NYMC,b.createtime SQSJ from SB_DMOFSETTLEMENT b 
                                   where b.IsFinish=0";

                    dmzmsql1 += $@"union 
                                   select b.ID,b.sbly,'农村门牌' MPLX,b.VillageName MC,b.MPNumber HM,b.createtime SQSJ from ZM_MPOFCOUNTRY b 
                                   where b.IsFinish=0
                                   union 
                                   select b.ID,b.sbly,'住宅门牌' MPLX,b.ResidenceName MC,'' HM,b.createtime SQSJ from ZM_MPOFRESIDENCE b 
                                   where b.IsFinish=0
                                   union 
                                   select b.ID,b.sbly,'道路门牌' MPLX,b.RoadName MC,b.MPNumber HM,b.createtime SQSJ from ZM_MPOFROAD b 
                                   where b.IsFinish=0";

                    cjyjsql1 += $@"union 
                                   select b.ID,b.sbly,b.ZYSSType LB,b.SmallType DMLB,b.DMType XLLB,b.Name MC,b.createtime SQSJ from BA_DMOFZYSS b 
                                   where b.IsFinish=0";

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
                    o_entity.LastModifyUser= LoginUtils.CurrentUser.UserName;
                }
                else
                {

                }
                entity.IsFinish = 1;
                entity.CheckUser = LoginUtils.CurrentUser.UserName;

            }
        }


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