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
            //var xml = $"";
            //if (type == ZSXX.HY.BMNX.name)
            //    xml = $"<Item name='BMMX0047' name_cn='项目地理位置'>" + place.zsxx.HY.BMNX.BMNX0017 + "</Item>"
            //        +
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
/// 海洋专属信息
/// </summary>
public class HYZSXX
{
    public static string name = "海洋";
    /// <summary>
    /// 潮汐类型
    /// </summary>
    public string BMNX0017 { get; set; }
}
/// <summary>
/// 海湾专属信息
/// </summary>
public class HWZSXX
{
    public static string name = "海湾";
    /// <summary>
    /// 所在海洋
    /// </summary>
    public string BMOX0028 { get; set; }
}
/// <summary>
/// 岛屿专属信息
/// </summary>
public class DYZSXX
{
    public static string name = "岛屿";
    /// <summary>
    /// 面积（平方千米）
    /// </summary>
    public string BMQX0021 { get; set; }
}
/// <summary>
/// 群岛专属信息
/// </summary>
public class QDZSXX
{
    public static string name = "群岛";
    /// <summary>
    /// 类型 详见：BL102，群岛类型代码表
    /// </summary>
    public string BMRX0017 { get; set; }
    /// <summary>
    /// 面积（平方千米）
    /// </summary>
    public string BMRX0026 { get; set; }
}
/// <summary>
/// 半岛、岬角专属信息
/// </summary>
public class BDJJZSXX
{
    public static string name = "半岛、岬角";
    /// <summary>
    /// 类型  详见：BL103，半岛、岬角类型代码表
    /// </summary>
    public string BMSX0017 { get; set; }
    /// <summary>
    /// 面积（平方米）
    /// </summary>
    public string BMSX0018 { get; set; }
}
/// <summary>
/// 滩涂、海岸专属信息
/// </summary>
public class TTHAZSXX
{
    public static string name = "半滩涂、海岸";
    /// <summary>
    /// 所在海洋
    /// </summary>
    public string BMTX0022 { get; set; }
}

/// <summary>
///海域类
/// </summary>
public class HY
{
    public HYZSXX hyzsxx { get; set; }
    public HWZSXX hwzsxx { get; set; }
    public DYZSXX dyzsxx { get; set; }
    public QDZSXX qdzsxx { get; set; }
    public BDJJZSXX bdjjzsxx { get; set; }
    public TTHAZSXX tthazsxx { get; set; }
}
/////////////////////////////////////////////////////////////

/// <summary>
/// 河流专属信息
/// </summary>
public class HLZSXX
{
    public static string name = "河流";
    /// <summary>
    /// 长度（千米）
    /// </summary>
    public string AMEX0008 { get; set; }
    /// <summary>
    /// 河流级别
    /// </summary>
    public string BMEX0028 { get; set; }
    /// <summary>
    /// 河流类别1
    /// </summary>
    public string BMEX0029 { get; set; }
    /// <summary>
    /// 河流类别2
    /// </summary>
    public string BMEX0030 { get; set; }
    /// <summary>
    /// 河流类别3
    /// </summary>
    public string BMEX0031 { get; set; }
}

/// <summary>
/// 河湾专属信息
/// </summary>
public class HEWZSXX
{
    public string name = "河湾";
    /// <summary>
    /// 所属河流
    /// </summary>
    public string BMYX0017 { get; set; }
}
/// <summary>
/// 河口专属信息
/// </summary>
public class HEKZSXX
{
    public static string name = "河口";
    /// <summary>
    /// 类型
    /// </summary>
    public string BMZX0017 { get; set; }
}
/// <summary>
/// 湖泊专属信息
/// </summary>
public class HPZSXX
{
    public static string name = "湖泊";
    /// <summary>
    /// 类型1
    /// </summary>
    public string BMFX0030 { get; set; }
    /// <summary>
    /// 类型2
    /// </summary>
    public string BMFX0031 { get; set; }
}
/// <summary>
/// 陆地岛屿专属信息
/// </summary>
public class LDDYZSXX
{
    public static string name = "陆地岛屿";
    /// <summary>
    /// 类型
    /// </summary>
    public string BLFX0017 { get; set; }
    /// <summary>
    /// 面积（平方千米）
    /// </summary>
    public string BLFX0021 { get; set; }
}
/// <summary>
/// 泉专属信息
/// </summary>
public class QZSXX
{
    public static string name = "泉";
    /// <summary>
    /// 类型
    /// </summary>
    public string BLIX0023 { get; set; }
}

/// <summary>
///（陆地）水系类
/// </summary>
public class SX
{
    public HLZSXX hlzsxx { get; set; }
    public HWZSXX hwzsxx { get; set; }
    public HEKZSXX hkzsxx { get; set; }
    public HPZSXX hpzsxx { get; set; }
    public LDDYZSXX lddyzsxx { get; set; }
    public QZSXX qzsxx { get; set; }
}

////////////////////////////////////////////////////////////////
/// <summary>
/// 行政区域专属信息
/// </summary>
public class XZQYZSXX
{
    public static string name = "行政区域";
    /// <summary>
    /// 邮政编码
    /// </summary>
    public string AMAX0013 { get; set; }
    /// <summary>
    /// 总面积（平方千米）
    /// </summary>
    public string AMAX0015 { get; set; }
    /// <summary>
    /// 行政区划单位
    /// </summary>
    public string BMAX0030 { get; set; }
    /// <summary>
    ///行政级别
    /// </summary>
    public string BMAX0031 { get; set; }
}
/// <summary>
///行政区域类
/// </summary>
public class XZQY
{
    public XZQYZSXX xzqyzsxx { get; set; }
}


/////////////////////////////////////////////////////////

/// <summary>
/// 矿区专属信息
/// </summary>
public class KQZSXX
{
    public static string name = "矿区";
    /// <summary>
    /// 驻地
    /// </summary>
    public string BNHX0019 { get; set; }
}
/// <summary>
/// 农林牧渔专属信息
/// </summary>
public class NLMYZSXX
{
    public static string name = "农林牧渔";
    /// <summary>
    /// 类型
    /// </summary>
    public string BNIX0017 { get; set; }
    /// <summary>
    /// 面积（平方千米）
    /// </summary>
    public string BNIX0019 { get; set; }
}
/// <summary>
/// 工业区、开发区专属信息
/// </summary>
public class GYQKFQZSXX
{
    public static string name = "工业务、开发区";
    /// <summary>
    /// 类型
    /// </summary>
    public string BNJX0017 { get; set; }
    /// <summary>
    /// 面积（平方千米）
    /// </summary>
    public string BNJX0019 { get; set; }
}
/// <summary>
/// 边贸区口岸专属信息
/// </summary>
public class BMQKAZSXX
{
    public static string name = "边贸区口岸";
    /// <summary>
    /// 类型
    /// </summary>
    public string BNKX0017 { get; set; }
    /// <summary>
    /// 面积（平方千米）
    /// </summary>
    public string BNKX0019 { get; set; }
}
/// <summary>
/// 非行政区域
/// </summary>
public class FXZQY
{
    public KQZSXX kqzsxx { get; set; }
    public NLMYZSXX nlmyzsxx { get; set; }
    public GYQKFQZSXX gyqkfqzsxx { get; set; }
    public BMQKAZSXX bmqkazsxx { get; set; }
}

////////////////////////////////////////////////
/// <summary>
/// 群众自治组织专属信息
/// </summary>
public class QZZZZZZSXX
{
    public static string name = "群众自治组织";
    /// <summary>
    /// 邮政编码
    /// </summary>
    public string BNOX0021 { get; set; }
}
/// <summary>
/// 群众自治类
/// </summary>
public class QZZZ
{
    public QZZZZZZSXX qzzzzzzsxx { get; set; }
}

/////////////////////////////////////////////////////////
/// <summary>
/// 居民点专属信息
/// </summary>
public class JMDZSXX
{
    public static string name = "居民点";
    /// <summary>
    /// 类型
    /// </summary>
    public string BNPX0017 { get; set; }
    /// <summary>
    /// 邮政编码
    /// </summary>
    public string BNPX0020 { get; set; }
}
/// <summary>
/// 居民点类
/// </summary>
public class JMD
{
    public JMDZSXX jmdzsxx { get; set; }
}
/////////////////////////////////////////////////////////
/// <summary>
/// 海港、河港专属信息
/// </summary>
public class HWHWZSXX
{
    public static string name = "海港、河港";
    /// <summary>
    /// 类型
    /// </summary>
    public string BXBX0017 { get; set; }
}
/// <summary>
/// 船闸升船机站专属信息
/// </summary>
public class CZSCJZZSXX
{
    public static string name = "船闸升船机站";
    /// <summary>
    /// 类型
    /// </summary>
    public string BXCX0017 { get; set; }
}
/// <summary>
/// 公路专属信息
/// </summary>
public class GLZSXX
{
    public static string name = "公路";
    /// <summary>
    /// 类型
    /// </summary>
    public string AMDX0010 { get; set; }
    /// <summary>
    /// 等级
    /// </summary>
    public string BMDX0029 { get; set; }
    /// <summary>
    /// 途径行政区
    /// </summary>
    public string BMDX0033 { get; set; }
}
/// <summary>
/// 铁路专属信息
/// </summary>
public class TLZSXX
{
    public static string name = "铁路";
    /// <summary>
    /// 类型1
    /// </summary>
    public string BXHX0017 { get; set; }
    /// <summary>
    /// 类型2
    /// </summary>
    public string BXHX0018 { get; set; }
    /// <summary>
    /// 主要车站
    /// </summary>
    public string BXHX0022 { get; set; }
    /// <summary>
    /// 途径行政区
    /// </summary>
    public string BXHX0023 { get; set; }

}
/// <summary>
/// 火车站专属信息
/// </summary>
public class HCZZSXX
{
    public static string name = "火车站";
    /// <summary>
    /// 类型
    /// </summary>
    public string BXIX0017 { get; set; }
}
/// <summary>
/// 航空港专属信息
/// </summary>
public class HKGZSXX
{
    public static string name = "航空港";
    /// <summary>
    /// 类型
    /// </summary>
    public string BXKX0017 { get; set; }
}
/// <summary>
/// 道路街巷专属信息
/// </summary>
public class DLJXZSXX
{
    public static string name = "道路街巷";
    /// <summary>
    /// 道路等级
    /// </summary>
    public string BXNX0012 { get; set; }
}
/// <summary>
/// 桥梁专属信息
/// </summary>
public class QLZSXX
{
    public static string name = "桥梁";
    /// <summary>
    ///类型
    /// </summary>
    public string BXRX0017 { get; set; }
    /// <summary>
    ///最大载重量（吨）
    /// </summary>
    public string BXRX0021 { get; set; }
    /// <summary>
    ///长度（米）
    /// </summary>
    public string BXRX0022 { get; set; }
}
/// <summary>
/// 隧道专属信息
/// </summary>
public class SDZSXX
{
    public static string name = "隧道";
    /// <summary>
    ///类型
    /// </summary>
    public string BXSX0017 { get; set; }
    /// <summary>
    ///长度（米）
    /// </summary>
    public string BXSX0018 { get; set; }
}
/// <summary>
/// 环岛路口专属信息
/// </summary>
public class HDLKZSXX
{
    public static string name = "环岛路口";
    /// <summary>
    ///类型
    /// </summary>
    public string BXVX0017 { get; set; }
}
/// <summary>
/// 交通运输设施类
/// </summary>
public class JTYS
{
    public HWHWZSXX hwhkzsxx { get; set; }

}

public class ZSXX
{



    /// <summary>
    ///纪念地与旅游景点类
    /// </summary>
    public class LYJD
    {
        /// <summary>
        /// 人物事件纪念地专属信息
        /// </summary>
        public class BNQX
        {
            public static string name = "人物事件纪念地";
            /// <summary>
            ///管理单位
            /// </summary>
            public string BNQX0018 { get; set; }
            /// <summary>
            ///人物或事件
            /// </summary>
            public string BNQX0022 { get; set; }
        }
        /// <summary>
        /// 公园风景区专属信息
        /// </summary>
        public class BNSX
        {
            public static string name = "公园风景区";
            /// <summary>
            ///管理单位
            /// </summary>
            public string BNSX0018 { get; set; }
        }
        /// <summary>
        /// 自然保护区专属信息
        /// </summary>
        public class BNTX
        {
            public static string name = "自然保护区";
            /// <summary>
            ///管理单位
            /// </summary>
            public string BNTX0020 { get; set; }
        }
    }

    /// <summary>
    /// 建筑物类
    /// </summary>
    public class JZW
    {
        /// <summary>
        /// 建筑物综合专属信息
        /// </summary>
        public class AMBX
        {
            public static string name = "建筑物综合";
            /// <summary>
            ///所在位置
            /// </summary>
            public string AMBX0010 { get; set; }
            /// <summary>
            ///邮政编码
            /// </summary>
            public string AMBX0020 { get; set; }
        }
        /// <summary>
        /// 房屋专属信息
        /// </summary>
        public class BNUX
        {
            public static string name = "房屋";
            /// <summary>
            /// 地址
            /// </summary>
            public string BNUX0011 { get; set; }
        }
        /// <summary>
        /// 亭台碑塔专属信息
        /// </summary>
        public class BNVX
        {
            public static string name = "亭台碑塔";
            /// <summary>
            /// 建筑结构
            /// </summary>
            public string BNVX0032 { get; set; }
        }
        /// <summary>
        /// 广场体育场专属信息
        /// </summary>
        public class BNWX
        {
            public static string name = "广场体育场";
            /// <summary>
            /// 所在位置
            /// </summary>
            public string BNWX0013 { get; set; }
            /// <summary>
            /// 占地面积（平方米）
            /// </summary>
            public string BNWX0014 { get; set; }
        }
    }

    /// <summary>
    /// 单位类
    /// </summary>
    public class DW
    {
        /// <summary>
        /// 单位专属信息
        /// </summary>
        public class BNYX
        {
            public static string name = "单位";
            /// <summary>
            /// 地址
            /// </summary>
            public string BNYX0017 { get; set; }
            /// <summary>
            /// 邮政编码
            /// </summary>
            public string BNYX0022 { get; set; }
        }
    }

    /// <summary>
    /// 水利、电力、通讯设施类
    /// </summary>
    public class SLDLTX
    {
        /// <summary>
        /// 水库专属信息
        /// </summary>
        public class BYCX
        {
            public static string name = "水库";
            /// <summary>
            /// 面积（平方千米）
            /// </summary>
            public string BYCX0017 { get; set; }
            /// <summary>
            /// 总库容（万立方米）
            /// </summary>
            public string BYCX0021 { get; set; }
        }
        /// <summary>
        ///发电站专属信息
        /// </summary>
        public class BYMX
        {
            public static string name = "发电站";
            /// <summary>
            /// 类型
            /// </summary>
            public string BYMX0017 { get; set; }
        }
    }
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