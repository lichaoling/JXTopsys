﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Extends
{
    public class Enums
    {
        public static class UseState
        {
            public const int Delete = 0;
            public const int Enable = 1;
            public const int Cancel = 2;
        }
        public static class MPProduce
        {
            public const int NO = 0;
            public const int Yes = 1;
        }
        public static class Complete
        {
            public const int NO = 0;
            public const int Yes = 1;
            public const int All = 2;
        }
        public static class MPMail
        {
            public const int Yes = 1;
            public const int No = 0;
        }
        public static class MPAddType
        {
            public const string LX = "零星";
            public const string PL = "批量";
        }
        public static class MPTypeCh
        {
            public const string Residence = "住宅门牌";
            public const string Road = "道路门牌";
            public const string Country = "农村门牌";
        }
        public static class PlaceNameTypeCH
        {
            public const string ZYSS = "专业设施地名";
        }

        public static class TypeInt
        {
            public const int Residence = 1;
            public const int Road = 2;
            public const int Country = 3;
            public const int Community = 4;
            public const int RP = 5;
            public const int MP = 6;
            public const int All = 0;
            public const int PlaceName = 7;
        }
        public static class RPRepairType
        {
            public const string Before = "维修前";
            public const string After = "维修后";
        }
        public static class CertificateType
        {
            public const string Placename = "地址证明";
            public const string MPZ = "门牌证";
        }
        public static class DocType
        {
            public const string FCZ = "房产证";
            public const string TDZ = "土地证";
            public const string BDCZ = "不动产证";
            public const string HJ = "户籍";
            public const string YYZZ = "营业执照";
            public const string QQZ = "确权证";
            public const string SQB = "申请表";

            public const string JSYDXKZ = "建设用地许可证";
            public const string JSGCGHXKZ = "建设工程规划许可证";
            public const string ZPMT = "总平面图";
            public const string XGT = "效果图";
            public const string DLTZ = "道路图纸";
            public const string QLTZ = "桥梁图纸";

            //专业设施地名
            //public const string SBBG = "申报表格";
            public const string LXPFS = "立项批复书";
            public const string SJT = "设计图";
        }
        public static class FileType
        {
            public const string Residence = "Residence";
            public const string Road = "Road";
            public const string Country = "Country";
            public const string BZPhoto = "BZPhoto";
            public const string QRCode = "QRCode";
            public const string RepairPhoto = "RPRepairPhoto";
        }
        public static class RPRepairFinish
        {
            public const int Yes = 1;
            public const int No = 0;
        }
        public static class MPNumberType
        {
            public const int Odd = 1;
            public const int Even = 2;
            public const int Other = 3;
        }
        public static class RPRepairMode
        {
            public const string Repair = "维修";
            public const string Change = "更换";
            public const string Del = "拆回";
            public const int All = 3;
        }
        public static class RPRange
        {
            public const int YXF = 1;
            public const int WXF = 0;
            public const int All = 2;
        }

        public class SPFileCertificateTypes
        {
            public const string QQZ = "确权证";
            public const string TDZ = "土地证";
            public const string FCZ = "房产证";
            public const string BDCZ = "不动产证";
            public const string HJ = "户籍";
            public const string YYZZ = "营业执照";
            //public const string QSZJ = "权属证件";

            public const string JSYDXKZ = "建设用地许可证";
            public const string JSGCGHXKZ = "建设工程规划许可证";
            public const string ZPMT = "总平面图";
            public const string XGT = "效果图";

            public const string LXPFS = "立项批复书";
            public const string DLTZ = "道路图纸";
            public const string QLTZ = "桥梁图纸";

            public const string SQB = "申请表";
            public const string SJT = "设计图";
        }

        public class SPFileBusinessTypes
        {
            public const string MPZSQ_DL = "门牌证申请_道路类";
            public const string MPZSQ_ZZ = "门牌证申请_住宅类";
            public const string MPZSQ_NC = "门牌证申请_农村";

            public const string MPZBG_DL = "门牌证变更_道路类";
            public const string MPZBG_ZZ = "门牌证变更_住宅类";
            public const string MPZBG_NC = "门牌证变更_农村类";

            public const string DMZM_DL = "地名证明_道路类";
            public const string DMZM_ZZ = "地名证明_住宅类";
            public const string DMZM_NC = "地名证明_农村类";

            public const string DMMM_JMD = "地名命名_居名点类";
            public const string DMMM_DLJX = "地名命名_道路街巷类";
            public const string DMMM_JZW = "地名命名_建筑物类";
            public const string DMMM_QL = "地名命名_桥梁类";

            public const string DMBA_ZYSS = "地名备案_专业设施地名";


            public const string DMGM_JMD = "地名更名_居名点类";
        }
    }
}