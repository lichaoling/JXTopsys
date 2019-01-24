using JXGIS.JXTopsystem.Models.Extends.DataPush;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace JXGIS.JXTopsystem.Business.DataPush
{
    public class DataPushUtils
    {
        public static string GetBaseInfoXml(string CALLER, string CALLOPERATE, string PLACEID, string PLACEOLDID, string STREETID)
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
                                + "<CALLER>" + CALLER + "</CALLER>"
                                + "<CALLOPERATE>" + CALLOPERATE + "</CALLOPERATE>"
                                + "<CALLTIME>" + DateTime.Now.ToString("yyyy-mm-dd") + "</CALLTIME>"
                            + "</CALLINFO>"
                            + "<PLACEID>" + PLACEID + "</PLACEID>"
                            + "<STREETID>" + STREETID + "</STREETID>"
                        + "</RECORD>";
            return xml;
        }

        public static void GetZSXXXml(Place place, string type)
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


        }

        public string GetAcceptInfoXml_Place(Place place, string type)
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
                        + "</RECORDS>";
            return xml;
        }
        //public string GetAcceptInfoXml_Doorplate(Doorplate doorplate)
        //{
        //    string xml = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
        //                + "<RECORDS>"
        //                    + "<Item name='BMMX0001' name_cn='街道（乡镇）代码'>" + doorplate.BMMX0001 + "</Item>"
        //                    + "<Item name='ALAA0001' name_cn='所属行政区划代码'>" + ALAA0001 + "</Item>"
        //                    + "<Item name='BYUX0004' name_cn='产权人（产权单位）'>" + BYUX0004 + "</Item>"
        //                    + "<Item name='BYUX0031' name_cn='门牌证号'>" + BYUX0031 + "</Item>"
        //                    + "<Item name='BYUX0037' name_cn='当前门牌地址'>" + BYUX0037 + "</Item>"
        //                    + "<Item name='BYUX0060' name_cn='门牌类型'>" + BYUX0060 + "</Item>"
        //                    + "<Item name='BYRX0030' name_cn='门牌申请方式'>" + BYRX0030 + "</Item>"
        //                    + "<Item name='BYRX0031' name_cn='门牌申请方式'>" + BYRX0031 + "</Item>"

        //                + "</RECORDS>";
        //    return xml;
        //}
    }
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
    public string BYWX0001 { get; set; }
    /// <summary>
    /// 联系人
    /// </summary>
    public string BYWX0003 { get; set; }
    /// <summary>
    /// 联系电话
    /// </summary>
    public string BYWX0004 { get; set; }
    /// <summary>
    /// 拟命名名称
    /// </summary>
    public string BYWX0005 { get; set; }
    /// <summary>
    /// 汉语拼音
    /// </summary>
    public string BYWX0006 { get; set; }
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