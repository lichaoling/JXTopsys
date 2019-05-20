using JXGIS.JXTopsystem.Business.DataPush;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JXGIS.JXTopsystem.Controllers
{
    public class DataPushController : Controller
    {
        // GET: DataPush
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DataPush()
        {
            RtObj rt = null;
            try
            {
                //string url = System.Configuration.ConfigurationManager.AppSettings["PostPlaceInfoURL"];
                //Dictionary<string, string> dic = new Dictionary<string, string>() { { "baseInfoXml", @"<?xml version='1.0' encoding='UTF-8'?>
                //        <RECORD>
                //        <CALLINFO>
                //            <CALLER>地名</CALLER>
                //            <CALLOPERATE>ADD</CALLOPERATE>
                //            <CALLTIME>2018-08-10</CALLTIME>
                //        </CALLINFO>
                //            <PLACEID>wew54hs899sdf1sdfsd</PLACEID>
                //            <STREETID>330102001</STREETID>
                //            <EXTEND>备用字段</EXTEND>
                //            <MEMO>备注</MEMO>
                //        </RECORD>
                //        " }, { "acceptInfoXml", @"<?xml version='1.0' encoding='utf-8'?>
                //        <RECORDS>
                //         <Item name='BMMX0001' name_cn='地名代码'>asf212333</Item>
                //         <Item name='BMMX0002' name_cn='标准地名'>计量大厦</Item>
                //         <Item name='BMMX0003' name_cn='地名类别代码'>22190</Item>
                //         <Item name='BMMX0006' name_cn='罗马字母拼写'>JILIANGDASHA</Item>
                //         <Item name='BMMX0007' name_cn='专名'>计量</Item>
                //         <Item name='BMMX0009' name_cn='通名'>大厦</Item>
                //         <Item name='BMMX0018' name_cn='汉语拼音'>Jìliáng Dàshà</Item>
                //         <Item name='AZAA0001' name_cn='所属行政区划代码'>330102001</Item>
                //         <Item name='BMMX0042' name_cn='地名的含义'>地名的含义测试</Item>
                //         <Item name='BMMX0044' name_cn='所在（跨）行政区'>所在（跨）行政区测试</Item>
                //         <Item name='BMMX0047' name_cn='项目地理位置'>提示信息：“如：四至界限”</Item>
                //         <Item name='BNPX0017' name_cn='类型'>1</Item>
                //         <Item name='BNPX0018' name_cn='长途电话区号'>0527</Item>
                //         <Item name='BNPX0020' name_cn='邮政编码'>310000</Item>
                //         <Item name='BMMX0028' name_cn='地名是否规划'>1</Item>
                //         <Item name='BMMX0004' name_cn='原图名称'>原图名称</Item>
                //         <Item name='BYRX0001' name_cn='申办人'>申办人</Item>
                //         <Item name='BYRX0002' name_cn='申办人证件号码'>3645999999</Item>
                //         <Item name='BYRX0006' name_cn='申办人证件类型'>1</Item>
                //         <Item name='BYRX0008' name_cn='领取方式'>1</Item>
                //         <Item name='BYRX0009' name_cn='填报用户'>clx</Item>
                //        </RECORDS>" } };

                //DataPushUtils.Post(url, dic);
                DataPushUtils.DataPush_MP();
            }
            catch (Exception ex)
            {
                rt = new RtObj(ex);
            }
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(rt);
            return Content(s);
        }
    }
}