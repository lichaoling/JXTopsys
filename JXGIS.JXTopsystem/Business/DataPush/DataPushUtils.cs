using JXGIS.JXTopsystem.Models.Entities;
using JXGIS.JXTopsystem.Models.Extends.DataPush;
using JXGIS.JXTopsystem.Models.Extends.RtObj;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace JXGIS.JXTopsystem.Business.DataPush
{
    public class DataPushUtils
    {
        private static string GetBaseInfoXml(BaseInfo baseInfo)
        {
            /*
             * CALLER      模块名
             * CALLOPERATE  分为新增（ADD）变更（UPDATE）删除（DELETE）
             * CALLTIME  请求时间
             * PLACEID  数据唯一主键，格式为（区划+主键）
             * PLACEOLDID  原始数据主键，用于变更时查找原始数据
             * STREETID  区划值，到街道（9位）
             * STREETID  备用字段
             * MEMO  备注
             */
            string xml = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                        + "<RECORD>"
                            + "<CALLINFO>"
                                + "<CALLER>" + baseInfo.CALLER + "</CALLER>"
                                + "<CALLOPERATE>" + baseInfo.CALLOPERATE + "</CALLOPERATE>"
                                + "<CALLTIME>" + baseInfo.CALLTIME + "</CALLTIME>"
                            + "</CALLINFO>"
                            + "<PLACEID>" + baseInfo.PLACEID + "</PLACEID>"
                            + "<STREETID>" + baseInfo.STREETID + "</STREETID>"
                        + "</RECORD>";
            return xml;
        }

        private static string GetZSXXXml(Place place, string type)
        {
            var xml = "";
            if (type == place.zsxx.hy.hyzsxx.name)
                xml = $"<Item name='BMNX0017' name_cn='潮汐类型'>" + place.zsxx.hy.hyzsxx.BMNX0017 + "</Item>";
            if (type == place.zsxx.hy.hwzsxx.name)
                xml = $"<Item name='BMOX0028' name_cn='所在海洋'>" + place.zsxx.hy.hwzsxx.BMOX0028 + "</Item>";
            if (type == place.zsxx.hy.dyzsxx.name)
                xml = $"<Item name='BMQX0021' name_cn='面积（平方千米）'>" + place.zsxx.hy.dyzsxx.BMQX0021 + "</Item>";
            if (type == place.zsxx.hy.qdzsxx.name)
                xml = $"<Item name='BMRX0017' name_cn='类型'>" + place.zsxx.hy.qdzsxx.BMRX0017 + "</Item>"
                    + $"<Item name='BMRX0026' name_cn='面积（平方千米）'>" + place.zsxx.hy.qdzsxx.BMRX0026 + "</Item>";
            if (type == place.zsxx.hy.bdjjzsxx.name)
                xml = $"<Item name='BMSX0017' name_cn='类型'>" + place.zsxx.hy.bdjjzsxx.BMSX0017 + "</Item>"
                    + $"<Item name='BMSX0018' name_cn='面积（平方米）'>" + place.zsxx.hy.bdjjzsxx.BMSX0018 + "</Item>";
            if (type == place.zsxx.hy.tthazsxx.name)
                xml = $"<Item name='BMTX0022' name_cn='所在海洋'>" + place.zsxx.hy.tthazsxx.BMTX0022 + "</Item>";
            if (type == place.zsxx.sx.hlzsxx.name)
                xml = $"<Item name='AMEX0008' name_cn='长度（千米）'>" + place.zsxx.sx.hlzsxx.AMEX0008 + "</Item>";
            if (type == place.zsxx.sx.hewzsxx.name)
                xml = $"<Item name='BMYX0017' name_cn='所属河流'>" + place.zsxx.sx.hewzsxx.BMYX0017 + "</Item>";
            if (type == place.zsxx.sx.hkzsxx.name)
                xml = $"<Item name='BMZX0017' name_cn='类型'>" + place.zsxx.sx.hkzsxx.BMZX0017 + "</Item>";
            if (type == place.zsxx.sx.hpzsxx.name)
                xml = $"<Item name='BMFX0030' name_cn='类型1'>" + place.zsxx.sx.hpzsxx.BMFX0030 + "</Item>"
                    + $"<Item name='BMFX0031' name_cn='类型2'>" + place.zsxx.sx.hpzsxx.BMFX0031 + "</Item>";
            if (type == place.zsxx.sx.lddyzsxx.name)
                xml = $"<Item name='BLFX0017' name_cn='类型'>" + place.zsxx.sx.lddyzsxx.BLFX0017 + "</Item>"
                    + $"<Item name='BLFX0021' name_cn='面积（平方千米）'>" + place.zsxx.sx.lddyzsxx.BLFX0021 + "</Item>";
            if (type == place.zsxx.sx.qzsxx.name)
                xml = $"<Item name='BLIX0023' name_cn='类型'>" + place.zsxx.sx.qzsxx.BLIX0023 + "</Item>";
            if (type == place.zsxx.xzqy.xzqyzsxx.name)
                xml = $"<Item name='AMAX0013' name_cn='邮政编码'>" + place.zsxx.xzqy.xzqyzsxx.AMAX0013 + "</Item>"
                    + $"<Item name='AMAX0015' name_cn='总面积（平方千米）'>" + place.zsxx.xzqy.xzqyzsxx.AMAX0015 + "</Item>"
                    + $"<Item name='BMAX0030' name_cn='行政区划单位'>" + place.zsxx.xzqy.xzqyzsxx.BMAX0030 + "</Item>"
                    + $"<Item name='BMAX0031' name_cn='行政级别'>" + place.zsxx.xzqy.xzqyzsxx.BMAX0031 + "</Item>";
            if (type == place.zsxx.fxzqy.kqzsxx.name)
                xml = $"<Item name='BNHX0019' name_cn='驻地'>" + place.zsxx.fxzqy.kqzsxx.BNHX0019 + "</Item>";
            if (type == place.zsxx.fxzqy.nlmyzsxx.name)
                xml = $"<Item name='BNIX0017' name_cn='类型'>" + place.zsxx.fxzqy.nlmyzsxx.BNIX0017 + "</Item>"
                    + $"<Item name='BNIX0019' name_cn='面积（平方千米）'>" + place.zsxx.fxzqy.nlmyzsxx.BNIX0019 + "</Item>";
            if (type == place.zsxx.fxzqy.gyqkfqzsxx.name)
                xml = $"<Item name='BNJX0017' name_cn='类型'>" + place.zsxx.fxzqy.gyqkfqzsxx.BNJX0017 + "</Item>"
                    + $"<Item name='BNJX0019' name_cn='面积（平方千米）'>" + place.zsxx.fxzqy.gyqkfqzsxx.BNJX0019 + "</Item>";
            if (type == place.zsxx.fxzqy.bmqkazsxx.name)
                xml = $"<Item name='BNKX0017' name_cn='类型'>" + place.zsxx.fxzqy.bmqkazsxx.BNKX0017 + "</Item>"
                    + $"<Item name='BNKX0019' name_cn='面积（平方千米）'>" + place.zsxx.fxzqy.bmqkazsxx.BNKX0019 + "</Item>";
            if (type == place.zsxx.qzzz.qzzzzzzsxx.name)
                xml = $"<Item name='BNOX0021' name_cn='邮政编码'>" + place.zsxx.qzzz.qzzzzzzsxx.BNOX0021 + "</Item>";
            if (type == place.zsxx.jmd.jmdzsxx.name)
                xml = $"<Item name='BNPX0017' name_cn='类型'>" + place.zsxx.jmd.jmdzsxx.BNPX0017 + "</Item>"
                    + $"<Item name='BNPX0020' name_cn='邮政编码'>" + place.zsxx.jmd.jmdzsxx.BNPX0020 + "</Item>";
            if (type == place.zsxx.jtys.hghgzsxx.name)
                xml = $"<Item name='BXBX0017' name_cn='类型'>" + place.zsxx.jtys.hghgzsxx.BXBX0017 + "</Item>";
            if (type == place.zsxx.jtys.czscjz.name)
                xml = $"<Item name='BXCX0017' name_cn='类型'>" + place.zsxx.jtys.czscjz.BXCX0017 + "</Item>";
            if (type == place.zsxx.jtys.glzsxx.name)
                xml = $"<Item name='AMDX0010' name_cn='类型'>" + place.zsxx.jtys.glzsxx.AMDX0010 + "</Item>"
                    + $"<Item name='BMDX0029' name_cn='等级'>" + place.zsxx.jtys.glzsxx.BMDX0029 + "</Item>"
                    + $"<Item name='BMDX0033' name_cn='途径行政区'>" + place.zsxx.jtys.glzsxx.BMDX0033 + "</Item>";
            if (type == place.zsxx.jtys.tlzsxx.name)
                xml = $"<Item name='BXHX0017' name_cn='类型1'>" + place.zsxx.jtys.tlzsxx.BXHX0017 + "</Item>"
                    + $"<Item name='BXHX0018' name_cn='类型2'>" + place.zsxx.jtys.tlzsxx.BXHX0018 + "</Item>"
                    + $"<Item name='BXHX0022' name_cn='主要车站'>" + place.zsxx.jtys.tlzsxx.BXHX0022 + "</Item>"
                    + $"<Item name='BXHX0023' name_cn='途径行政区'>" + place.zsxx.jtys.tlzsxx.BXHX0023 + "</Item>";
            if (type == place.zsxx.jtys.hczzsxx.name)
                xml = $"<Item name='BXIX0017' name_cn='类型'>" + place.zsxx.jtys.hczzsxx.BXIX0017 + "</Item>";
            if (type == place.zsxx.jtys.hkgzsxx.name)
                xml = $"<Item name='BXKX0017' name_cn='类型'>" + place.zsxx.jtys.hkgzsxx.BXKX0017 + "</Item>";
            if (type == place.zsxx.jtys.dljxzsxx.name)
                xml = $"<Item name='BXNX0012' name_cn='道路等级'>" + place.zsxx.jtys.dljxzsxx.BXNX0012 + "</Item>";
            if (type == place.zsxx.jtys.qlzsxx.name)
                xml = $"<Item name='BXRX0017' name_cn='类型'>" + place.zsxx.jtys.qlzsxx.BXRX0017 + "</Item>"
                    + $"<Item name='BXRX0021' name_cn='最大载重量（吨）'>" + place.zsxx.jtys.qlzsxx.BXRX0021 + "</Item>"
                    + $"<Item name='BXRX0022' name_cn='长度（米）'>" + place.zsxx.jtys.qlzsxx.BXRX0022 + "</Item>";
            if (type == place.zsxx.jtys.sdzsxx.name)
                xml = $"<Item name='BXSX0017' name_cn='类型'>" + place.zsxx.jtys.sdzsxx.BXSX0017 + "</Item>"
                    + $"<Item name='BXSX0018' name_cn='长度（米）'>" + place.zsxx.jtys.sdzsxx.BXSX0018 + "</Item>";
            if (type == place.zsxx.jtys.hdlkzsxx.name)
                xml = $"<Item name='BXVX0017' name_cn='类型'>" + place.zsxx.jtys.hdlkzsxx.BXVX0017 + "</Item>";
            if (type == place.zsxx.lyjd.rwsjjndzsxx.name)
                xml = $"<Item name='BNQX0018' name_cn='管理单位'>" + place.zsxx.lyjd.rwsjjndzsxx.BNQX0018 + "</Item>"
                    + $"<Item name='BNQX0022' name_cn='人物或事件'>" + place.zsxx.lyjd.rwsjjndzsxx.BNQX0022 + "</Item>";
            if (type == place.zsxx.lyjd.gyfjqzsxx.name)
                xml = $"<Item name='BNSX0018' name_cn='管理单位'>" + place.zsxx.lyjd.gyfjqzsxx.BNSX0018 + "</Item>";
            if (type == place.zsxx.lyjd.zrbhqzsxx.name)
                xml = $"<Item name='BNTX0020' name_cn='管理单位'>" + place.zsxx.lyjd.zrbhqzsxx.BNTX0020 + "</Item>";
            if (type == place.zsxx.jzw.jzwzhzsxx.name)
                xml = $"<Item name='AMBX0010' name_cn='所在位置'>" + place.zsxx.jzw.jzwzhzsxx.AMBX0010 + "</Item>"
                    + $"<Item name='AMBX0020' name_cn='邮政编码'>" + place.zsxx.jzw.jzwzhzsxx.AMBX0020 + "</Item>";
            if (type == place.zsxx.jzw.fwzsxx.name)
                xml = $"<Item name='BNUX0011' name_cn='地址'>" + place.zsxx.jzw.fwzsxx.BNUX0011 + "</Item>"
                    + $"<Item name='BNUX0016' name_cn='邮政编码'>" + place.zsxx.jzw.fwzsxx.BNUX0016 + "</Item>";
            if (type == place.zsxx.jzw.ttbtzsxx.name)
                xml = $"<Item name='BNVX0032' name_cn='建筑结构'>" + place.zsxx.jzw.ttbtzsxx.BNVX0032 + "</Item>";
            if (type == place.zsxx.jzw.gctyczsxx.name)
                xml = $"<Item name='BNWX0013' name_cn='所在位置'>" + place.zsxx.jzw.gctyczsxx.BNWX0013 + "</Item>"
                    + $"<Item name='BNWX0014' name_cn='占地面积（平方米）'>" + place.zsxx.jzw.gctyczsxx.BNWX0014 + "</Item>";
            if (type == place.zsxx.dw.dwzsxx.name)
                xml = $"<Item name='BNYX0017' name_cn='地址'>" + place.zsxx.dw.dwzsxx.BNYX0017 + "</Item>"
                    + $"<Item name='BNYX0022' name_cn='邮政编码'>" + place.zsxx.dw.dwzsxx.BNYX0022 + "</Item>";
            if (type == place.zsxx.sldltx.skzsxx.name)
                xml = $"<Item name='BYCX0017' name_cn='面积（平方千米）'>" + place.zsxx.sldltx.skzsxx.BYCX0017 + "</Item>"
                    + $"<Item name='BYCX0021' name_cn='总库容（万立方米）'>" + place.zsxx.sldltx.skzsxx.BYCX0021 + "</Item>";
            if (type == place.zsxx.sldltx.fdzzsxx.name)
                xml = $"<Item name='BYMX0017' name_cn='类型'>" + place.zsxx.sldltx.fdzzsxx.BYMX0017 + "</Item>";

            return xml;
        }

        private static string GetAcceptInfoXml_Place(Place place, string type)
        {
            string xml = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                        + "<RECORDS>"
                            + "<Item name='BMMX0001' name_cn='地名代码'>" + place.BMMX0001 + "</Item>"
                            + "<Item name='BMMX0002' name_cn='标准地名'>" + place.BMMX0002 + "</Item>"
                            + "<Item name='BMMX0003' name_cn='地名类别代码'>" + place.BMMX0003 + "</Item>"
                            + "<Item name='BMMX0006' name_cn='罗马字母拼写'>" + place.BMMX0006 + "</Item>"
                            + "<Item name='BMMX0007' name_cn='专名'>" + place.BMMX0007 + "</Item>"
                            + "<Item name='BMMX0009' name_cn='通名'>" + place.BMMX0009 + "</Item>"
                            + "<Item name='BMMX0018' name_cn='汉语拼音'>" + place.BMMX0018 + "</Item>"
                            + "<Item name='AZAA0001' name_cn='所属行政区划代码'>" + place.AZAA0001 + "</Item>"
                            + "<Item name='BMMX0042' name_cn='地名的含义'>" + place.BMMX0042 + "</Item>"
                            + "<Item name='BMMX0044' name_cn='所在（跨）行政区'>" + place.BMMX0044 + "</Item>"
                            + "<Item name='BMMX0047' name_cn='项目地理位置'>" + place.BMMX0047 + "</Item>"
                            + GetZSXXXml(place, type)
                            + "<Item name='BYRX0001' name_cn='申办人'>" + place.BYRX0001 + "</Item>"
                            + "<Item name='BYRX0002' name_cn='申办人证件号码'>" + place.BYRX0002 + "</Item>"
                            + "<Item name='BYRX0006' name_cn='申办人证件类型'>" + place.BYRX0006 + "</Item>"
                            + "<Item name='BYRX0008' name_cn='领取方式'>" + place.BYRX0008 + "</Item>"
                            + "<Item name='BYRX0009' name_cn='填报用户'>" + place.BYRX0009 + "</Item>"
                            + "<Item name='BYRX0010' name_cn='填报时间'>" + place.BYRX0010 + "</Item>"
                            + "<Item name='BYZX0003' name_cn='收件人'>" + place.BYZX0003 + "</Item>"
                            + "<Item name='BYZX0005' name_cn='收件地址'>" + place.BYZX0005 + "</Item>"

                        + "</RECORDS>";
            return xml;
        }
        private static string GetAcceptInfoXml_Doorplate(Doorplate doorplate)
        {
            string xml = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                        + "<RECORDS>"
                            + "<Item name='BYUX0001' name_cn='街道（乡镇）代码'>" + doorplate.BYUX0001 + "</Item>"
                            + "<Item name='ALAA0001' name_cn='所属行政区划代码'>" + doorplate.ALAA0001 + "</Item>"
                            + "<Item name='BYUX0004' name_cn='产权人（产权单位）'>" + doorplate.BYUX0004 + "</Item>"
                            + "<Item name='BYUX0031' name_cn='门牌证号'>" + doorplate.BYUX0031 + "</Item>"
                            + "<Item name='BYUX0037' name_cn='当前门牌地址'>" + doorplate.BYUX0037 + "</Item>"
                            + "<Item name='BYUX0060' name_cn='门牌类型'>" + doorplate.BYUX0060 + "</Item>"
                            + "<Item name='BYRX0030' name_cn='门牌申请方式'>" + doorplate.BYRX0030 + "</Item>"
                            + "<Item name='BYRX0031' name_cn='门牌申请类型'>" + doorplate.BYRX0031 + "</Item>"
                            + "<Item name='BYRX0032' name_cn='申办人'>" + doorplate.BYRX0032 + "</Item>"
                            + "<Item name='BYRX0035' name_cn='填报用户'>" + doorplate.BYRX0035 + "</Item>"
                            + "<Item name='BYRX0038' name_cn='领取方式'>" + doorplate.BYRX0038 + "</Item>"
                        + "</RECORDS>";
            return xml;
        }
        private static string GetAcceptInfoXml_PlaceProve(PlaceProve placeProve)
        {
            string xml = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                        + "<RECORDS>"
                            + "<Item name='ALAA0001' name_cn='行政区划代码'>" + placeProve.ALAA0001 + "</Item>"
                            + "<Item name='BYWX0001' name_cn='产权人'>" + placeProve.BYWX0001 + "</Item>"
                            + "<Item name='BYWX0002' name_cn='证件号码'>" + placeProve.BYWX0002 + "</Item>"
                            + "<Item name='BYWX0003' name_cn='证件类型'>" + placeProve.BYWX0003 + "</Item>"
                            + "<Item name='BYWX0008' name_cn='历史地名地址'>" + placeProve.BYWX0008 + "</Item>"
                            + "<Item name='BYWX0009' name_cn='现地名地址'>" + placeProve.BYWX0009 + "</Item>"
                            + "<Item name='BYWX0012' name_cn='填报时间'>" + placeProve.BYWX0012 + "</Item>"
                            + "<Item name='BYWX0013' name_cn='填报用户'>" + placeProve.BYWX0013 + "</Item>"
                            + "<Item name='BYWX0014' name_cn='申办人'>" + placeProve.BYWX0014 + "</Item>"
                            + "<Item name='BYWX0015' name_cn='申办人联系电话'>" + placeProve.BYWX0015 + "</Item>"
                            + "<Item name='BYWX0016' name_cn='业务办理状态'>" + placeProve.BYWX0016 + "</Item>"
                            + "<Item name='BYWX0024' name_cn='结果领取方式'>" + placeProve.BYWX0024 + "</Item>"
                        + "</RECORDS>";
            return xml;
        }
        private static string GetAcceptInfoXml_PlaceOpinion(PlaceOpinion placeOpinion)
        {
            string xml = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                        + "<RECORDS>"
                            + "<Item name='ALAA0001' name_cn='行政区划代码'>" + placeOpinion.ALAA0001 + "</Item>"
                            + "<Item name='BYXX0001' name_cn='申请单位'>" + placeOpinion.BYXX0001 + "</Item>"
                            + "<Item name='BYXX0003' name_cn='联系人'>" + placeOpinion.BYXX0003 + "</Item>"
                            + "<Item name='BYXX0004' name_cn='联系电话'>" + placeOpinion.BYXX0004 + "</Item>"
                            + "<Item name='BYXX0005' name_cn='拟命名名称'>" + placeOpinion.BYXX0005 + "</Item>"
                            + "<Item name='BYXX0007' name_cn='用地地址'>" + placeOpinion.BYXX0007 + "</Item>"
                            + "<Item name='BYXX0008' name_cn='规模用途'>" + placeOpinion.BYXX0008 + "</Item>"
                            + "<Item name='BYXX0009' name_cn='拟命名理由及标准名称来历、含义'>" + placeOpinion.BYXX0009 + "</Item>"
                            + "<Item name='BYXX0011' name_cn='填报时间'>" + placeOpinion.BYXX0011 + "</Item>"
                            + "<Item name='BYXX0013' name_cn='填报用户'>" + placeOpinion.BYXX0013 + "</Item>"
                            + "<Item name='BYXX0017' name_cn='申请接收方式'>" + placeOpinion.BYXX0017 + "</Item>"
                            + "<Item name='BYXX0021' name_cn='结果领取方式'>" + placeOpinion.BYXX0021 + "</Item>"
                            + "<Item name='BMJXUUID' name_cn='UUID'>" + placeOpinion.BMJXUUID + "</Item>"
                            + "<Item name='BMJX0001' name_cn='PROJID'>" + placeOpinion.BMJX0001 + "</Item>"
                            + "<Item name='BMJX0002' name_cn='门牌证号/地名代码'>" + placeOpinion.BMJX0002 + "</Item>"
                            + "<Item name='BMJX0003' name_cn='附件文件名称'>" + placeOpinion.BMJX0003 + "</Item>"
                            + "<Item name='BMJX0004' name_cn='附件访问路径'>" + placeOpinion.BMJX0004 + "</Item>"
                            + "<Item name='BMJX0006' name_cn='文件大小'>" + placeOpinion.BMJX0006 + "</Item>"
                            + "<Item name='BMJX0007' name_cn='材料描述代码'>" + placeOpinion.BMJX0007 + "</Item>"
                            + "<Item name='BMJX0008' name_cn='业务类型代码'>" + placeOpinion.BMJX0008 + "</Item>"
                            + "<Item name='ZZZZ9999' name_cn='时间戳'>" + placeOpinion.ZZZZ9999 + "</Item>"
                        + "</RECORDS>";
            return xml;
        }
        private static void Post(string url, Dictionary<string, string> parameters)
        {
            //string url = System.Configuration.ConfigurationManager.AppSettings["PostPlaceInfoURL"];
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;//创建请求对象
            request.Method = "POST";//请求方式
            request.ContentType = "application/x-www-form-urlencoded";//链接类型
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                bool first = true;
                foreach (string key in parameters.Keys)
                {
                    if (!first)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        first = false;
                    }
                }
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                //写入请求流
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            var response = (HttpWebResponse)request.GetResponse();
            var st = response.GetResponseStream();   //获取响应的字符串流
            StreamReader sr = new StreamReader(st); //创建一个stream读取流
            string html = sr.ReadToEnd();   //从头读到尾，放到字符串html

            var msg = Newtonsoft.Json.JsonConvert.DeserializeObject<PostMsg>(html);
            if (msg == null)
                throw new Error("信息推送失败！");
            else if (msg.code != "01")
                throw new Error(msg.msg);
        }

        public static void MPOfCountryDataPush(MPOfCountry mp)
        {
            BaseInfo baseInfo = new BaseInfo();
            baseInfo.CALLER = "门牌";
            baseInfo.CALLOPERATE = mp.LastModifyTime != null ? "UPDATE" : (mp.State == 1 ? "ADD" : "DELETE");
            baseInfo.CALLTIME = DateTime.Now.ToString("yyyy-MM-dd");
            baseInfo.PLACEID = "3304" + mp.AddressCoding.Substring(0, 5) + mp.ID;
            baseInfo.STREETID = "3304" + mp.AddressCoding.Substring(0, 5);
            string BaseInfoXml = GetBaseInfoXml(baseInfo);

            Doorplate doorplate = new Doorplate();
            doorplate.BYUX0001 = mp.AddressCoding.Substring(2, 3);
            doorplate.ALAA0001 = "3304" + mp.AddressCoding.Substring(0, 2);
            doorplate.BYUX0004 = mp.PropertyOwner;
            doorplate.BYUX0031 = mp.AddressCoding;
            doorplate.BYUX0037 = mp.StandardAddress;
            doorplate.BYUX0060 = "农村门牌";
            doorplate.BYRX0032 = mp.Applicant;
            doorplate.BYRX0035 = mp.CreateUser;
            doorplate.BYRX0038 = mp.MPMail == 0 ? "窗口领取" : "邮递到家";//查询代码表
            string AcceptInfoXml = GetAcceptInfoXml_Doorplate(doorplate);

            string url = System.Configuration.ConfigurationManager.AppSettings["PostDoorplateInfoURL"];
            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                {"baseInfoXml", BaseInfoXml},
                { "acceptInfoXml", AcceptInfoXml}
            };
            Post(url, dic);
        }
    }

    public class PostMsg
    {
        public string code { get; set; }
        public string msg { get; set; }
    }

    public class BaseInfo
    {
        /// <summary>
        /// 模块名
        /// </summary>
        public string CALLER { get; set; }
        /// <summary>
        /// 新增（ADD）变更（UPDATE）删除（DELETE）
        /// </summary>
        public string CALLOPERATE { get; set; }
        /// <summary>
        /// 请求时间
        /// </summary>
        public string CALLTIME { get; set; }
        /// <summary>
        /// 数据唯一主键，格式为（区划+主键）
        /// </summary>
        public string PLACEID { get; set; }
        /// <summary>
        /// 区划值，到街道（9位）
        /// </summary>
        public string STREETID { get; set; }
        /// <summary>
        /// 备用字段
        /// </summary>
        public string EXTEND { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string MEMO { get; set; }
    }
    public class Place
    {
        public string DMLB { get; set; }
        /// <summary>
        /// 地名代码
        /// </summary>
        public string BMMX0001 { get; set; }
        /// <summary>
        /// 标准地名
        /// </summary>
        public string BMMX0002 { get; set; }
        /// <summary>
        /// 地名类别代码
        /// </summary>
        public string BMMX0003 { get; set; }
        /// <summary>
        /// 罗马字母拼写
        /// </summary>
        public string BMMX0006 { get; set; }
        /// <summary>
        /// 专名
        /// </summary>
        public string BMMX0007 { get; set; }
        /// <summary>
        ///通名
        /// </summary>
        public string BMMX0009 { get; set; }
        /// <summary>
        /// 汉语拼音
        /// </summary>
        public string BMMX0018 { get; set; }
        /// <summary>
        /// 所属行政区划代码
        /// </summary>
        public string AZAA0001 { get; set; }
        /// <summary>
        /// 地名的含义
        /// </summary>
        public string BMMX0042 { get; set; }
        /// <summary>
        /// 所在（跨）行政区
        /// </summary>
        public string BMMX0044 { get; set; }
        /// <summary>
        /// 项目地理位置  四至界限
        /// </summary>
        public string BMMX0047 { get; set; }
        //////////////////地名核准业务信息
        /// <summary>
        /// 申办人
        /// </summary>
        public string BYRX0001 { get; set; }
        /// <summary>
        /// 申办人证件号码
        /// </summary>
        public string BYRX0002 { get; set; }
        /// <summary>
        /// 申办人证件类型
        /// </summary>
        public string BYRX0006 { get; set; }
        /// <summary>
        /// 领取方式
        /// </summary>
        public string BYRX0008 { get; set; }
        /// <summary>
        /// 填报用户
        /// </summary>
        public string BYRX0009 { get; set; }
        /// <summary>
        /// 填报时间
        /// </summary>
        public string BYRX0010 { get; set; }
        //////////////////////邮递信息
        /// <summary>
        /// 收件人
        /// </summary>
        public string BYZX0003 { get; set; }
        /// <summary>
        /// 收件地址
        /// </summary>
        public string BYZX0005 { get; set; }

        public ZSXX zsxx { get; set; }
    }

    /// <summary>
    /// 地名分类信息
    /// </summary>
    public class ZSXX
    {
        public DW dw { get; set; }
        public FXZQY fxzqy { get; set; }
        public HY hy { get; set; }
        public JMD jmd { get; set; }
        public JTYS jtys { get; set; }
        public JZW jzw { get; set; }
        public LYJD lyjd { get; set; }
        public QZZZ qzzz { get; set; }
        public SLDLTX sldltx { get; set; }
        public SX sx { get; set; }
        public XZQY xzqy { get; set; }
    }
    public class Doorplate
    {
        /// <summary>
        /// 街道（乡镇）代码
        /// </summary>
        public string BYUX0001 { get; set; }
        /// <summary>
        /// 所属行政区划代码
        /// </summary>
        public string ALAA0001 { get; set; }
        /// <summary>
        /// 产权人（产权单位）
        /// </summary>
        public string BYUX0004 { get; set; }
        /// <summary>
        /// 门牌证号
        /// </summary>
        public string BYUX0031 { get; set; }
        /// <summary>
        /// 当前门牌地址
        /// </summary>
        public string BYUX0037 { get; set; }
        /// <summary>
        /// 门牌类型
        /// </summary>
        public string BYUX0060 { get; set; }
        /// <summary>
        /// 门牌类型
        /// </summary>
        public string BYRX0030 { get; set; }
        /// <summary>
        /// 门牌申请类型
        /// </summary>
        public string BYRX0031 { get; set; }
        /// <summary>
        /// 申办人
        /// </summary>
        public string BYRX0032 { get; set; }
        /// <summary>
        /// 填报用户
        /// </summary>
        public string BYRX0035 { get; set; }
        /// <summary>
        /// 领取方式
        /// </summary>
        public string BYRX0038 { get; set; }
    }
    public class PlaceProve
    {
        /// <summary>
        /// 行政区划代码
        /// </summary>
        public string ALAA0001 { get; set; }
        /// <summary>
        /// 产权人
        /// </summary>
        public string BYWX0001 { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string BYWX0002 { get; set; }
        /// <summary>
        ///证件类型
        /// </summary>
        public string BYWX0003 { get; set; }
        /// <summary>
        /// 历史地名地址
        /// </summary>
        public string BYWX0008 { get; set; }
        /// <summary>
        /// 现地名地址
        /// </summary>
        public string BYWX0009 { get; set; }
        /// <summary>
        /// 填报时间
        /// </summary>
        public string BYWX0012 { get; set; }
        /// <summary>
        /// 填报用户
        /// </summary>
        public string BYWX0013 { get; set; }
        /// <summary>
        /// 申办人
        /// </summary>
        public string BYWX0014 { get; set; }
        /// <summary>
        /// 申办人联系电话
        /// </summary>
        public string BYWX0015 { get; set; }
        /// <summary>
        /// 业务办理状态
        /// </summary>
        public string BYWX0016 { get; set; }
        /// <summary>
        /// 结果领取方式
        /// </summary>
        public string BYWX0024 { get; set; }
    }
    public class PlaceOpinion
    {
        /// <summary>
        /// 行政区划代码
        /// </summary>
        public string ALAA0001 { get; set; }
        /// <summary>
        /// 申请单位
        /// </summary>
        public string BYXX0001 { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string BYXX0003 { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string BYXX0004 { get; set; }
        /// <summary>
        /// 拟命名名称
        /// </summary>
        public string BYXX0005 { get; set; }
        /// <summary>
        /// 汉语拼音
        /// </summary>
        public string BYXX0006 { get; set; }
        /// <summary>
        /// 用地地址
        /// </summary>
        public string BYXX0007 { get; set; }
        /// <summary>
        /// 规模用途
        /// </summary>
        public string BYXX0008 { get; set; }
        /// <summary>
        /// 拟命名理由及标准名称来历、含义
        /// </summary>
        public string BYXX0009 { get; set; }
        /// <summary>
        /// 填报时间
        /// </summary>
        public string BYXX0011 { get; set; }
        /// <summary>
        /// 填报用户
        /// </summary>
        public string BYXX0013 { get; set; }
        /// <summary>
        /// 申请接收方式
        /// </summary>
        public string BYXX0017 { get; set; }
        /// <summary>
        /// 结果领取方式
        /// </summary>
        public string BYXX0021 { get; set; }
        /// <summary>
        /// 主键ID
        /// </summary>
        public string BMJXUUID { get; set; }
        /// <summary>
        /// PROJID外键，数据ID
        /// </summary>
        public string BMJX0001 { get; set; }
        /// <summary>
        ///门牌证号/地名代码
        /// </summary>
        public string BMJX0002 { get; set; }
        /// <summary>
        /// 附件文件名称
        /// </summary>
        public string BMJX0003 { get; set; }
        /// <summary>
        /// 附件访问路径
        /// </summary>
        public string BMJX0004 { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public string BMJX0006 { get; set; }
        /// <summary>
        /// 材料描述代码
        /// </summary>
        public string BMJX0007 { get; set; }
        /// <summary>
        /// 业务类型代码
        /// </summary>
        public string BMJX0008 { get; set; }
        /// <summary>
        /// ZZZZ9999
        /// </summary>
        public string ZZZZ9999 { get; set; }
    }
}