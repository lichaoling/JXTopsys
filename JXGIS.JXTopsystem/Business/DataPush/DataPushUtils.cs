﻿using JXGIS.JXTopsystem.Models.Entities;
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
                            + "<PLACEOLDID>" + baseInfo.PLACEOLDID + "</PLACEOLDID>"
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
                xml = $"<Item name='AMEX0008' name_cn='长度（千米）'>" + place.zsxx.sx.hlzsxx.AMEX0008 + "</Item>"
                    + $"<Item name='BMEX0028' name_cn='河流级别'>" + place.zsxx.sx.hlzsxx.BMEX0028 + "</Item>"
                    + $"<Item name='BMEX0029' name_cn='河流类别1'>" + place.zsxx.sx.hlzsxx.BMEX0029 + "</Item>"
                    + $"<Item name='BMEX0030' name_cn='河流类别2'>" + place.zsxx.sx.hlzsxx.BMEX0030 + "</Item>"
                    + $"<Item name='BMEX0031' name_cn='河流类别3'>" + place.zsxx.sx.hlzsxx.BMEX0031 + "</Item>";
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


            //***************************居民点*********************************
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

            //***************************道路街巷*********************************
            if (type == place.zsxx.jtys.dljxzsxx.name)
                xml = $"<Item name='BXNX0012' name_cn='道路等级'>" + place.zsxx.jtys.dljxzsxx.BXNX0012 + "</Item>";

            //***************************桥梁*********************************
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

            //***************************房屋*********************************
            if (type == place.zsxx.jzw.fwzsxx.name)
                xml = $"<Item name='BNUX0011' name_cn='地址'>" + place.zsxx.jzw.fwzsxx.BNUX0011 + "</Item>"
                    + $"<Item name='BNUX0016' name_cn='邮政编码'>" + place.zsxx.jzw.fwzsxx.BNUX0016 + "</Item>";


            if (type == place.zsxx.jzw.ttbtzsxx.name)
                xml = $"<Item name='BNVX0032' name_cn='建筑结构'>" + place.zsxx.jzw.ttbtzsxx.BNVX0032 + "</Item>";

            //***************************广场体育场*********************************
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

        private static string GetDocumentXml(Document doc)
        {
            string xml = $"<Item name='BMJXUUID' name_cn='UUID'>" + doc.BMJXUUID + "</Item>"
                + $"<Item name='BMJX0001' name_cn='PROJID'>" + doc.BMJX0001 + "</Item>"
                + $"<Item name='BMJX0002' name_cn='门牌证号/地名代码'>" + doc.BMJX0002 + "</Item>"
                + $"<Item name='BMJX0003' name_cn='附件文件名称'>" + doc.BMJX0003 + "</Item>"
                + $"<Item name='BMJX0004' name_cn='附件访问路径'>" + doc.BMJX0004 + "</Item>"
                + $"<Item name='BMJX0006' name_cn='文件大小'>" + doc.BMJX0006 + "</Item>"
                + $"<Item name='BMJX0007' name_cn='材料描述代码'>" + doc.BMJX0007 + "</Item>"
                + $"<Item name='BMJX0008' name_cn='业务类型代码'>" + doc.BMJX0008 + "</Item>"
                + $"<Item name='ZZZZ9999' name_cn='时间戳'>" + doc.ZZZZ9999 + "</Item>";
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
                            + "<Item name='BYUX0004' name_cn='社区（居、村）名称'>" + doorplate.BYUX0003 + "</Item>"
                            + "<Item name='BYUX0004' name_cn='产权人（产权单位）'>" + doorplate.BYUX0004 + "</Item>"
                            + "<Item name='BYUX0036' name_cn='证件号码'>" + doorplate.BYUX0036 + "</Item>"
                            + "<Item name='BYUX0058' name_cn='证件类型'>" + doorplate.BYUX0058 + "</Item>"
                            + "<Item name='BYUX0009' name_cn='村'>" + doorplate.BYUX0009 + "</Item>"
                            + "<Item name='BYUX0010' name_cn='村单位'>" + doorplate.BYUX0010 + "</Item>"
                            + "<Item name='BYUX0011' name_cn='村居'>" + doorplate.BYUX0011 + "</Item>"
                            + "<Item name='BYUX0012' name_cn='村居单位'>" + doorplate.BYUX0012 + "</Item>"
                            + "<Item name='BYUX0013' name_cn='村号'>" + doorplate.BYUX0013 + "</Item>"
                            + "<Item name='BYUX0014' name_cn='村号单位'>" + doorplate.BYUX0014 + "</Item>"
                            + "<Item name='BYUX0015' name_cn='所在道路'>" + doorplate.BYUX0015 + "</Item>"
                            + "<Item name='BYUX0016' name_cn='道路级别（单位）'>" + doorplate.BYUX0016 + "</Item>"
                            + "<Item name='BYUX0019' name_cn='道路号'>" + doorplate.BYUX0019 + "</Item>"
                            + "<Item name='BYUX0019' name_cn='道路号级别'>" + doorplate.BYUX0020 + "</Item>"
                            + "<Item name='BYUX0021' name_cn='小区名称'>" + doorplate.BYUX0021 + "</Item>"
                            + "<Item name='BYUX0022' name_cn='小区级别'>" + doorplate.BYUX0022 + "</Item>"
                            + "<Item name='BYUX0025' name_cn='幢'>" + doorplate.BYUX0025 + "</Item>"
                            + "<Item name='BYUX0026' name_cn='幢级别'>" + doorplate.BYUX0026 + "</Item>"
                            + "<Item name='BYUX0027' name_cn='单元'>" + doorplate.BYUX0027 + "</Item>"
                            + "<Item name='BYUX0028' name_cn='单元级别'>" + doorplate.BYUX0028 + "</Item>"
                            + "<Item name='BYUX0029' name_cn='室、号'>" + doorplate.BYUX0029 + "</Item>"
                            + "<Item name='BYUX0030' name_cn='室、号级别'>" + doorplate.BYUX0030 + "</Item>"
                            + "<Item name='BYUX0031' name_cn='门牌证号'>" + doorplate.BYUX0031 + "</Item>"
                            + "<Item name='BYUX0037' name_cn='当前门牌地址'>" + doorplate.BYUX0037 + "</Item>"
                            + "<Item name='BYUX0060' name_cn='门牌类型'>" + doorplate.BYUX0060 + "</Item>"
                            + "<Item name='BYRX0030' name_cn='门牌申请方式'>" + doorplate.BYRX0030 + "</Item>"
                            + "<Item name='BYRX0031' name_cn='门牌申请类型'>" + doorplate.BYRX0031 + "</Item>"
                            + "<Item name='BYRX0032' name_cn='申办人'>" + doorplate.BYRX0032 + "</Item>"
                            + "<Item name='BYRX0033' name_cn='申办人证件号码'>" + doorplate.BYRX0033 + "</Item>"
                            + "<Item name='BYRX0039' name_cn='申办人证件类型'>" + doorplate.BYRX0039 + "</Item>"
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
                        //+ "<Item name='BMJXUUID' name_cn='UUID'>" + placeOpinion.BMJXUUID + "</Item>"
                        //+ "<Item name='BMJX0001' name_cn='PROJID'>" + placeOpinion.BMJX0001 + "</Item>"
                        //+ "<Item name='BMJX0002' name_cn='门牌证号/地名代码'>" + placeOpinion.BMJX0002 + "</Item>"
                        //+ "<Item name='BMJX0003' name_cn='附件文件名称'>" + placeOpinion.BMJX0003 + "</Item>"
                        //+ "<Item name='BMJX0004' name_cn='附件访问路径'>" + placeOpinion.BMJX0004 + "</Item>"
                        //+ "<Item name='BMJX0006' name_cn='文件大小'>" + placeOpinion.BMJX0006 + "</Item>"
                        //+ "<Item name='BMJX0007' name_cn='材料描述代码'>" + placeOpinion.BMJX0007 + "</Item>"
                        //+ "<Item name='BMJX0008' name_cn='业务类型代码'>" + placeOpinion.BMJX0008 + "</Item>"
                        //+ "<Item name='ZZZZ9999' name_cn='时间戳'>" + placeOpinion.ZZZZ9999 + "</Item>"
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
            baseInfo.CALLOPERATE = mp.DataPushStatus != 1 && mp.LastModifyTime != null ? "UPDATE" : (mp.State == 1 ? "ADD" : "DELETE");
            baseInfo.PLACEOLDID = mp.ID;
            baseInfo.CALLTIME = DateTime.Now.ToString("yyyy-MM-dd");
            baseInfo.PLACEID = "3304" + mp.AddressCoding.Substring(0, 5) + mp.ID;
            baseInfo.STREETID = "3304" + mp.AddressCoding.Substring(0, 5);
            string BaseInfoXml = GetBaseInfoXml(baseInfo);

            Doorplate doorplate = new Doorplate();
            doorplate.BYUX0001 = "3304" + mp.AddressCoding.Substring(0, 5);
            doorplate.ALAA0001 = "3304" + mp.AddressCoding.Substring(0, 2);

            doorplate.BYUX0004 = mp.PropertyOwner == null ? "无" : mp.PropertyOwner;
            doorplate.BYUX0036 = mp.IDNumber == null ? "无" : mp.IDNumber;
            doorplate.BYUX0058 = mp.IDType == "居民身份证" ? "1" : mp.IDType == "统一社会信用代码证" ? "3" : "2";

            doorplate.BYUX0003 = mp.CommunityName;
            doorplate.BYUX0009 = mp.CommunityName;
            doorplate.BYUX0010 = mp.CommunityName != null ? "3" : null;
            doorplate.BYUX0011 = mp.ViligeName;
            doorplate.BYUX0012 = mp.ViligeName == null ? null : "5";
            doorplate.BYUX0013 = mp.MPNumber;
            doorplate.BYUX0014 = "1";

            doorplate.BYUX0031 = mp.AddressCoding;
            doorplate.BYUX0037 = mp.StandardAddress;
            doorplate.BYUX0060 = "2";
            doorplate.BYRX0030 = "2";//1单位（委托）申请、2个人（委托）申请、3农居（委托）申请
            doorplate.BYRX0031 = "1";//1首次申请 2补领申请  3其他

            doorplate.BYRX0032 = mp.Applicant == null ? "无" : mp.Applicant;
            doorplate.BYRX0033 = "无";
            doorplate.BYRX0039 = "2";

            doorplate.BYRX0035 = mp.CreateUser;
            doorplate.BYRX0038 = mp.MPMail == 0 ? "3" : "1";//查询代码表
            string AcceptInfoXml = GetAcceptInfoXml_Doorplate(doorplate);

            string url = System.Configuration.ConfigurationManager.AppSettings["PostDoorplateInfoURL"];
            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                {"baseInfoXml", BaseInfoXml},
                { "acceptInfoXml", AcceptInfoXml}
            };
            Post(url, dic);
        }
        public static void MPOfResidenceDataPush(MPOfResidence mp)
        {
            BaseInfo baseInfo = new BaseInfo();
            baseInfo.CALLER = "门牌";
            baseInfo.CALLOPERATE = mp.LastModifyTime != null ? "UPDATE" : (mp.State == 1 ? "ADD" : "DELETE");
            baseInfo.PLACEOLDID = mp.ID;
            baseInfo.CALLTIME = DateTime.Now.ToString("yyyy-MM-dd");
            baseInfo.PLACEID = "3304" + mp.AddressCoding.Substring(0, 5) + mp.ID;
            baseInfo.STREETID = "3304" + mp.AddressCoding.Substring(0, 5);
            string BaseInfoXml = GetBaseInfoXml(baseInfo);

            Doorplate doorplate = new Doorplate();
            doorplate.BYUX0001 = "3304" + mp.AddressCoding.Substring(0, 5);
            doorplate.ALAA0001 = "3304" + mp.AddressCoding.Substring(0, 2);

            doorplate.BYUX0004 = mp.PropertyOwner == null ? "无" : mp.PropertyOwner;
            doorplate.BYUX0036 = mp.IDNumber == null ? "无" : mp.IDNumber;
            doorplate.BYUX0058 = mp.IDType == "居民身份证" ? "1" : mp.IDType == "统一社会信用代码证" ? "3" : "2";

            doorplate.BYUX0003 = mp.CommunityName;
            doorplate.BYUX0021 = mp.ResidenceName;
            doorplate.BYUX0022 = mp.ResidenceName.Contains("公寓") ? "3" : mp.ResidenceName.Contains("大厦") ? "4" : mp.ResidenceName.Contains("广场") ? "8" : mp.ResidenceName.Contains("家园") ? "11" : mp.ResidenceName.Contains("公馆") ? "13" : mp.ResidenceName.Contains("名苑") ? "14" : mp.ResidenceName.Contains("商厦") ? "9" : "1";
            doorplate.BYUX0025 = mp.LZNumber;
            doorplate.BYUX0026 = mp.LZNumber == null ? null : "1";
            doorplate.BYUX0027 = mp.DYNumber;
            doorplate.BYUX0028 = mp.DYNumber == null ? null : "1";
            doorplate.BYUX0029 = mp.HSNumber;
            doorplate.BYUX0030 = mp.HSNumber == null ? null : "1";

            doorplate.BYUX0031 = mp.AddressCoding;
            doorplate.BYUX0037 = mp.StandardAddress;
            doorplate.BYUX0060 = "3";
            doorplate.BYRX0030 = "1";//1单位（委托）申请、2个人（委托）申请、3农居（委托）申请
            doorplate.BYRX0031 = "1";//1首次申请 2补领申请  3其他

            doorplate.BYRX0032 = mp.Applicant == null ? "无" : mp.Applicant;
            doorplate.BYRX0033 = "无";
            doorplate.BYRX0039 = "2";

            doorplate.BYRX0035 = mp.CreateUser;
            doorplate.BYRX0038 = "3";//查询代码表
            string AcceptInfoXml = GetAcceptInfoXml_Doorplate(doorplate);

            string url = System.Configuration.ConfigurationManager.AppSettings["PostDoorplateInfoURL"];
            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                {"baseInfoXml", BaseInfoXml},
                { "acceptInfoXml", AcceptInfoXml}
            };
            Post(url, dic);
        }
        public static void MPOfRoadDataPush(MPOfRoad mp)
        {
            BaseInfo baseInfo = new BaseInfo();
            baseInfo.CALLER = "门牌";
            baseInfo.CALLOPERATE = mp.LastModifyTime != null ? "UPDATE" : (mp.State == 1 ? "ADD" : "DELETE");
            baseInfo.PLACEOLDID = mp.ID;
            baseInfo.CALLTIME = DateTime.Now.ToString("yyyy-MM-dd");
            baseInfo.PLACEID = "3304" + mp.AddressCoding.Substring(0, 5) + mp.ID;
            baseInfo.STREETID = "3304" + mp.AddressCoding.Substring(0, 5);
            string BaseInfoXml = GetBaseInfoXml(baseInfo);

            Doorplate doorplate = new Doorplate();
            doorplate.BYUX0001 = "3304" + mp.AddressCoding.Substring(0, 5);
            doorplate.ALAA0001 = "3304" + mp.AddressCoding.Substring(0, 2);

            doorplate.BYUX0004 = mp.PropertyOwner == null ? "无" : mp.PropertyOwner;
            doorplate.BYUX0036 = mp.IDNumber == null ? "无" : mp.IDNumber;
            doorplate.BYUX0058 = mp.IDType == "居民身份证" ? "1" : mp.IDType == "统一社会信用代码证" ? "3" : "2";

            doorplate.BYUX0003 = mp.CommunityName;
            doorplate.BYUX0015 = mp.RoadName;
            doorplate.BYUX0016 = mp.RoadName.Contains("弄") ? "4" : mp.RoadName.Contains("街") ? "3" : mp.RoadName.Contains("巷") ? "6" : "1";
            doorplate.BYUX0019 = mp.MPNumber;
            doorplate.BYUX0020 = "1";

            doorplate.BYUX0031 = mp.AddressCoding;
            doorplate.BYUX0037 = mp.StandardAddress;
            doorplate.BYUX0060 = "1";
            doorplate.BYRX0030 = "1";//1单位（委托）申请、2个人（委托）申请、3农居（委托）申请
            doorplate.BYRX0031 = "1";//1首次申请 2补领申请  3其他

            doorplate.BYRX0032 = mp.Applicant == null ? "无" : mp.Applicant;
            doorplate.BYRX0033 = "无";
            doorplate.BYRX0039 = "2";

            doorplate.BYRX0035 = mp.CreateUser;
            doorplate.BYRX0038 = mp.MPMail == 0 ? "3" : "1";//查询代码表
            string AcceptInfoXml = GetAcceptInfoXml_Doorplate(doorplate);

            string url = System.Configuration.ConfigurationManager.AppSettings["PostDoorplateInfoURL"];
            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                {"baseInfoXml", BaseInfoXml},
                { "acceptInfoXml", AcceptInfoXml}
            };
            Post(url, dic);
        }
        public static void PlaceProveDataPush(MPOfCertificate mc)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                BaseInfo baseInfo = new BaseInfo();
                baseInfo.CALLER = "地名证明";
                baseInfo.CALLOPERATE = "ADD";
                baseInfo.CALLTIME = DateTime.Now.ToString("yyyy-MM-dd");

                PlaceProve placeProve = new PlaceProve();

                if (mc.MPType == "住宅门牌")
                {
                    var mp = db.MPOfResidence.Where(t => t.ID == mc.MPID).FirstOrDefault();
                    baseInfo.PLACEID = "3304" + mp.AddressCoding.Substring(0, 5) + mp.ID;
                    baseInfo.STREETID = "3304" + mp.AddressCoding.Substring(0, 5);

                    placeProve.ALAA0001 = "3304" + mp.AddressCoding.Substring(0, 2);
                    placeProve.BYWX0001 = mp.PropertyOwner == null ? "无" : mp.PropertyOwner;
                    placeProve.BYWX0002 = mp.IDNumber == null ? "无" : mp.IDNumber;
                    placeProve.BYWX0003 = mp.IDType == null ? "4" : mp.IDType == "统一社会信用代码证" ? "3" : "1";
                    placeProve.BYWX0008 = mp.FCZAddress == null && mp.TDZAddress == null && mp.BDCZAddress == null && mp.HJAddress == null && mp.OtherAddress == null ? "无" : mp.FCZAddress + mp.TDZAddress + mp.BDCZAddress + mp.HJAddress + mp.OtherAddress;
                    placeProve.BYWX0009 = mp.StandardAddress;
                    placeProve.BYWX0012 = mc.CreateTime.ToString("yyyy-MM-dd");
                    placeProve.BYWX0013 = mc.CreateUser;
                    placeProve.BYWX0014 = mp.Applicant == null ? "无" : mp.Applicant;
                    placeProve.BYWX0015 = mp.ApplicantPhone == null ? "无" : mp.ApplicantPhone;
                    placeProve.BYWX0016 = "109";
                    placeProve.BYWX0024 = "2";
                }
                else if (mc.MPType == "道路门牌")
                {
                    var mp = db.MPOfRoad.Where(t => t.ID == mc.MPID).FirstOrDefault();
                    baseInfo.PLACEID = "3304" + mp.AddressCoding.Substring(0, 5) + mp.ID;
                    baseInfo.STREETID = "3304" + mp.AddressCoding.Substring(0, 5);

                    placeProve.ALAA0001 = "3304" + mp.AddressCoding.Substring(0, 2);
                    placeProve.BYWX0001 = mp.PropertyOwner == null ? "无" : mp.PropertyOwner;
                    placeProve.BYWX0002 = mp.IDNumber == null ? "无" : mp.IDNumber;
                    placeProve.BYWX0003 = mp.IDType == null ? "4" : mp.IDType == "统一社会信用代码证" ? "3" : "1";
                    placeProve.BYWX0008 = mp.FCZAddress == null && mp.TDZAddress == null && mp.YYZZAddress == null && mp.OtherAddress == null ? "无" : mp.FCZAddress + mp.TDZAddress + mp.YYZZAddress + mp.OtherAddress;
                    placeProve.BYWX0009 = mp.StandardAddress;
                    placeProve.BYWX0012 = mc.CreateTime.ToString("yyyy-MM-dd"); ;
                    placeProve.BYWX0013 = mc.CreateUser;
                    placeProve.BYWX0014 = mp.Applicant == null ? "无" : mp.Applicant;
                    placeProve.BYWX0015 = mp.ApplicantPhone == null ? "无" : mp.ApplicantPhone;
                    placeProve.BYWX0016 = "109";
                    placeProve.BYWX0024 = "2";
                }
                else if (mc.MPType == "农村门牌")
                {
                    var mp = db.MPOfCountry.Where(t => t.ID == mc.MPID).FirstOrDefault();
                    baseInfo.PLACEID = "3304" + mp.AddressCoding.Substring(0, 5) + mp.ID;
                    baseInfo.STREETID = "3304" + mp.AddressCoding.Substring(0, 5);

                    placeProve.ALAA0001 = "3304" + mp.AddressCoding.Substring(0, 2);
                    placeProve.BYWX0001 = mp.PropertyOwner == null ? "无" : mp.PropertyOwner;
                    placeProve.BYWX0002 = mp.IDNumber == null ? "无" : mp.IDNumber;
                    placeProve.BYWX0003 = mp.IDType == null ? "4" : mp.IDType == "统一社会信用代码证" ? "3" : "1";
                    placeProve.BYWX0008 = mp.TDZAddress == null && mp.QQZAddress == null && mp.OtherAddress == null ? "无" : mp.TDZAddress + mp.QQZAddress + mp.OtherAddress;
                    placeProve.BYWX0009 = mp.StandardAddress;
                    placeProve.BYWX0012 = mc.CreateTime.ToString("yyyy-MM-dd"); ;
                    placeProve.BYWX0013 = mc.CreateUser;
                    placeProve.BYWX0014 = mp.Applicant == null ? "无" : mp.Applicant;
                    placeProve.BYWX0015 = mp.ApplicantPhone == null ? "无" : mp.ApplicantPhone;
                    placeProve.BYWX0016 = "109";
                    placeProve.BYWX0024 = "2";
                }
                string BaseInfoXml = GetBaseInfoXml(baseInfo);
                string AcceptInfoXml = GetAcceptInfoXml_PlaceProve(placeProve);

                string url = System.Configuration.ConfigurationManager.AppSettings["PostPlaceProveInfoURL"];
                Dictionary<string, string> dic = new Dictionary<string, string>()
                    {
                        {"baseInfoXml", BaseInfoXml},
                        { "acceptInfoXml", AcceptInfoXml}
                    };
                Post(url, dic);
            }
        }
        public static void PlaceOpinionDataPush(DMOFZYSS zyss)
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                BaseInfo baseInfo = new BaseInfo();
                baseInfo.CALLER = "出具意见";
                baseInfo.CALLOPERATE = zyss.LastModifyTime != null ? "UPDATE" : (zyss.State == 1 ? "ADD" : "DELETE");
                baseInfo.CALLTIME = DateTime.Now.ToString("yyyy-MM-dd");

                var countyCode = db.District.Where(t => t.ID == zyss.CountyID).Select(t => t.Code).FirstOrDefault();
                var neighborhoodsCode = db.District.Where(t => t.ID == zyss.NeighborhoodsID).Select(t => t.Code).FirstOrDefault();

                baseInfo.PLACEID = "3304" + countyCode + neighborhoodsCode + zyss.ID;
                baseInfo.STREETID = "3304" + countyCode + neighborhoodsCode;
                string BaseInfoXml = GetBaseInfoXml(baseInfo);

                PlaceOpinion placeOpinion = new PlaceOpinion();
                placeOpinion.ALAA0001 = "3304" + countyCode;
                placeOpinion.BYXX0001 = zyss.SBDW == null ? "无" : zyss.SBDW;
                placeOpinion.BYXX0003 = zyss.Applicant == null ? "无" : zyss.Applicant;
                placeOpinion.BYXX0004 = zyss.Telephone == null ? "无" : zyss.Telephone;
                placeOpinion.BYXX0005 = zyss.Name;
                placeOpinion.BYXX0007 = zyss.XMAddress == null ? "无" : zyss.XMAddress;
                placeOpinion.BYXX0008 = "无";
                placeOpinion.BYXX0009 = zyss.DMHY == null ? "无" : zyss.DMHY;
                placeOpinion.BYXX0011 = ((DateTime)zyss.CreateTime).ToString("yyyy-MM-dd");
                placeOpinion.BYXX0013 = zyss.CreateUser;
                placeOpinion.BYXX0017 = zyss.SBLY == "网上申报" ? "3" : "1";
                placeOpinion.BYXX0021 = "现场送达";

                string AcceptInfoXml = GetAcceptInfoXml_PlaceOpinion(placeOpinion);
                string url = System.Configuration.ConfigurationManager.AppSettings["PostPlaceOpinionInfoURL"];
                Dictionary<string, string> dic = new Dictionary<string, string>()
                    {
                        {"baseInfoXml", BaseInfoXml},
                        { "acceptInfoXml", AcceptInfoXml}
                    };
                Post(url, dic);

            }
        }



        public static void DataPush_MP()
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                //var mpres = db.MPOfResidence.Where(t => t.DataPushStatus == 0 && (t.State == 1 || (t.DataPushTime != null && t.State == 2))).Take(2).ToList();
                //foreach (var mp in mpres)
                //{
                //    MPOfResidenceDataPush(mp);
                //    //mp.DataPushStatus = 1;
                //    //mp.DataPushTime = DateTime.Now;
                //}
                //var mpros = db.MPOfRoad.Where(t => t.DataPushStatus == 0 && (t.State == 1 || (t.DataPushTime != null && t.State == 2))).Take(2).ToList();
                //foreach (var mp in mpros)
                //{
                //    MPOfRoadDataPush(mp);
                //    //mp.DataPushStatus = 1;
                //    //mp.DataPushTime = DateTime.Now;
                //}
                //var mpcos = db.MPOfCountry.Where(t => t.DataPushStatus == 0 && (t.State == 1 || (t.DataPushTime != null && t.State == 2))).Take(2).ToList();
                //foreach (var mp in mpcos)
                //{
                //    MPOfCountryDataPush(mp);
                //    //mp.DataPushStatus = 1;
                //    //mp.DataPushTime = DateTime.Now;
                //}

                var mp = db.MPOfCountry.Where(t => t.ID == "00002fb2-efb5-459e-9f11-24eadae02562").FirstOrDefault();
                BaseInfo baseInfo = new BaseInfo();
                baseInfo.CALLER = "门牌";
                baseInfo.CALLOPERATE = "DELETE";
                baseInfo.PLACEOLDID = "3304" + mp.AddressCoding.Substring(0, 5) + mp.ID;
                baseInfo.CALLTIME = DateTime.Now.ToString("yyyy-MM-dd");
                baseInfo.PLACEID = "3304" + mp.AddressCoding.Substring(0, 5) + mp.ID;
                baseInfo.STREETID = "3304" + mp.AddressCoding.Substring(0, 5);
                string BaseInfoXml = GetBaseInfoXml(baseInfo);

                Doorplate doorplate = new Doorplate();
                doorplate.BYUX0001 = "3304" + mp.AddressCoding.Substring(0, 5);
                doorplate.ALAA0001 = "3304" + mp.AddressCoding.Substring(0, 2);

                doorplate.BYUX0004 = mp.PropertyOwner == null ? "无" : mp.PropertyOwner;
                doorplate.BYUX0036 = mp.IDNumber == null ? "无" : mp.IDNumber;
                doorplate.BYUX0058 = mp.IDType == "居民身份证" ? "1" : mp.IDType == "统一社会信用代码证" ? "3" : "2";

                doorplate.BYUX0003 = mp.CommunityName;
                doorplate.BYUX0009 = mp.CommunityName;
                doorplate.BYUX0010 = mp.CommunityName != null ? "3" : null;
                doorplate.BYUX0011 = mp.ViligeName;
                doorplate.BYUX0012 = mp.ViligeName == null ? null : "5";
                doorplate.BYUX0013 = mp.MPNumber+"22222";
                doorplate.BYUX0014 = "1";

                doorplate.BYUX0031 = mp.AddressCoding;
                doorplate.BYUX0037 = mp.StandardAddress;
                doorplate.BYUX0060 = "2";
                doorplate.BYRX0030 = "2";//1单位（委托）申请、2个人（委托）申请、3农居（委托）申请
                doorplate.BYRX0031 = "1";//1首次申请 2补领申请  3其他

                doorplate.BYRX0032 = mp.Applicant == null ? "无" : mp.Applicant;
                doorplate.BYRX0033 = "无";
                doorplate.BYRX0039 = "2";

                doorplate.BYRX0035 = mp.CreateUser;
                doorplate.BYRX0038 = mp.MPMail == 0 ? "3" : "1";//查询代码表
                string AcceptInfoXml = GetAcceptInfoXml_Doorplate(doorplate);

                string url = System.Configuration.ConfigurationManager.AppSettings["PostDoorplateInfoURL"];
                Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                {"baseInfoXml", BaseInfoXml},
                { "acceptInfoXml", AcceptInfoXml}
            };
                Post(url, dic);

                db.SaveChanges();
            }
        }

        public static void DataPush_DMZM()
        {
            using (var db = SystemUtils.NewEFDbContext)
            {
                var mpces = db.MPOfCertificate.Where(t => t.CreateUser != "sdmb").Where(t => t.DataPushStatus == 0).Take(10).ToList();
                foreach (var mc in mpces)
                {
                    PlaceProveDataPush(mc);
                    //mc.DataPushStatus = 1;
                    //mc.DataPushTime = DateTime.Now;
                }
                db.SaveChanges();
            }
        }

        public static void DataPush_ZYSS()
        {

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
        /// 原始数据主键，用于变更时查找原始数据
        /// </summary>
        public string PLACEOLDID { get; set; }
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
        public JMD jmd { get; set; }//
        public JTYS jtys { get; set; }//
        public JZW jzw { get; set; }//
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
        /// BYUX0003
        /// </summary>
        public string BYUX0003 { get; set; }
        /// <summary>
        /// 村
        /// </summary>
        public string BYUX0009 { get; set; }
        /// <summary>
        /// 村单位 1行政村 2自然村 3村
        /// </summary>
        public string BYUX0010 { get; set; }
        /// <summary>
        /// 村居
        /// </summary>
        public string BYUX0011 { get; set; }
        /// <summary>
        /// 村居单位 1区 3苑 5村
        /// </summary>
        public string BYUX0012 { get; set; }
        /// <summary>
        /// 村号
        /// </summary>
        public string BYUX0013 { get; set; }
        /// <summary>
        /// 村号单位  1号 2室
        /// </summary>
        public string BYUX0014 { get; set; }
        /// <summary>
        /// 所在道路
        /// </summary>
        public string BYUX0015 { get; set; }
        /// <summary>
        /// 道路级别单位 1路 3街 4弄 5里 6巷 13区 14道
        /// </summary>
        public string BYUX0016 { get; set; }
        /// <summary>
        /// 道路弄
        /// </summary>
        public string BYUX0017 { get; set; }
        /// <summary>
        /// 弄级别 1弄 2巷 3号 4路
        /// </summary>
        public string BYUX0018 { get; set; }
        /// <summary>
        /// 道路号 
        /// </summary>
        public string BYUX0019 { get; set; }
        /// <summary>
        /// 道路号级别 1号 2室 3层 4座 5楼
        /// </summary>
        public string BYUX0020 { get; set; }
        /// <summary>
        /// 小区名称
        /// </summary>
        public string BYUX0021 { get; set; }
        /// <summary>
        /// 小区级别 1小区 2花园 3公寓 4大厦 5新村 6苑 7中心 8广场 9商厦 10庭 11家园 12园 13公馆 14名苑 15城
        /// </summary>
        public string BYUX0022 { get; set; }
        /// <summary>
        /// 苑、花苑
        /// </summary>
        public string BYUX0023 { get; set; }
        /// <summary>
        /// 苑级别
        /// </summary>
        public string BYUX0024 { get; set; }
        /// <summary>
        /// 幢
        /// </summary>
        public string BYUX0025 { get; set; }
        /// <summary>
        /// 幢级别 1幢 3号楼
        /// </summary>
        public string BYUX0026 { get; set; }
        /// <summary>
        /// 单元 1单元 2层 3区 4号
        /// </summary>
        public string BYUX0027 { get; set; }
        /// <summary>
        /// 单元级别
        /// </summary>
        public string BYUX0028 { get; set; }
        /// <summary>
        /// 室、号
        /// </summary>
        public string BYUX0029 { get; set; }
        /// <summary>
        /// 室、号级别  1室  2号
        /// </summary>
        public string BYUX0030 { get; set; }

        /// <summary>
        ///产权人身份证号码
        /// </summary>
        public string BYUX0036 { get; set; }
        /// <summary>
        /// 证件类型 1身份证 2户籍证明 3营业执照（五证合一）4组织机构代码证
        /// </summary>
        public string BYUX0058 { get; set; }

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
        ///申办人证件号码
        /// </summary>
        public string BYRX0033 { get; set; }
        /// <summary>
        /// 申办人证件类型 1身份证 2户籍证明 3营业执照（五证合一）4组织机构代码证
        /// </summary>
        public string BYRX0039 { get; set; }

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
        ///// <summary>
        ///// 主键ID
        ///// </summary>
        //public string BMJXUUID { get; set; }
        ///// <summary>
        ///// PROJID外键，数据ID
        ///// </summary>
        //public string BMJX0001 { get; set; }
        ///// <summary>
        /////门牌证号/地名代码
        ///// </summary>
        //public string BMJX0002 { get; set; }
        ///// <summary>
        ///// 附件文件名称
        ///// </summary>
        //public string BMJX0003 { get; set; }
        ///// <summary>
        ///// 附件访问路径
        ///// </summary>
        //public string BMJX0004 { get; set; }
        ///// <summary>
        ///// 文件大小
        ///// </summary>
        //public string BMJX0006 { get; set; }
        ///// <summary>
        ///// 材料描述代码
        ///// </summary>
        //public string BMJX0007 { get; set; }
        ///// <summary>
        ///// 业务类型代码
        ///// </summary>
        //public string BMJX0008 { get; set; }
        ///// <summary>
        ///// ZZZZ9999
        ///// </summary>
        //public string ZZZZ9999 { get; set; }
    }
    public class Document
    {
        /// <summary>
        /// UUID
        /// </summary>
        public string BMJXUUID { get; set; }
        /// <summary>
        /// PROJID
        /// </summary>
        public string BMJX0001 { get; set; }
        /// <summary>
        /// 门牌证号/地名代码
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
        /// 时间戳
        /// </summary>
        public string ZZZZ9999 { get; set; }
    }

}