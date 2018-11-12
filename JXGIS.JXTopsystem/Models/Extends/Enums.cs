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
        public static class TypeStr
        {
            public const string MP = "MP";
            public const string RP = "RP";
            public const string RPRepair = "RPRepair";
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
        }
        public static class RPRepairType
        {
            public const string Before = "维修前";
            public const string After = "维修后";
        }
        public static class CertificateType
        {
            public const string Placename = "地名证明";
            public const string MPZ = "门牌证";
        }
        public static class MPFileType
        {
            public const string ResidenceMP = "Residence";
            public const string RoadMP = "Road";
            public const string CountryMP = "Country";
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
        }
        public static class RPFileType
        {
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
            public const string Repair = "更换";
            public const string Change = "维修";
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