using System;
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
            public const string FCZ = "FCZ";
            public const string TDZ = "TDZ";
            public const string BDCZ = "BDCZ";
            public const string HJ = "HJ";
            public const string YYZZ = "YYZZ";
            public const string QQZ = "QQZ";
            public const string SQB = "SQB";

            //专业设施地名
            public const string SBBG = "申报表格";
            public const string LXPFWJ = "立项批复文件";
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
            public const int All = 3;
        }
        public static class RPRange
        {
            public const int YXF = 1;
            public const int WXF = 0;
            public const int All = 2;
        }
    }
}