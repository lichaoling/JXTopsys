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
            public const int LX = 1;
            public const int PL = 0;
        }
        public static class TypeStr
        {
            public const string MP = "MP";
            public const string RP = "RP";
            public const string RPRepair = "RPRepair";
        }

        public static class TypeInt
        {
            public const int Residence = 1;
            public const int Road = 2;
            public const int Country = 3;
            public const int Community = 4;
            public const int RP = 5;
            public const int All = 0;
        }
        public static class RPRepairType
        {
            public const int Before = 0;
            public const int After = 1;
        }
        public static class CertificateType
        {
            public const int Placename = 1;
            public const int MPZ = 2;
            public const int All = 0;
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

        }
        public static class RPFileType
        {
            public const string BZPhoto = "BZPhoto";
            public const string QRCode = "QRCode";
            public const string RepairPhoto = "RepairPhoto";

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
            public const int Repair = 1;
            public const int Change = 2;
            public const int All = 3;
        }
        public static class RPRange
        {
            public const int YXF = 1;
            public const int WXF = 2;
            public const int All = 0;
        }
    }
}