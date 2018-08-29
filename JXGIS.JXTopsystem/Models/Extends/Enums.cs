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
        };
        public static class MPProduce
        {
            public const int NO = 0;
            public const int Yes = 1;
        };
        public static class MPProduceComplete
        {
            public const int NO = 0;
            public const int Yes = 1;
        };
        public static class MPMail
        {
            public const int Yes = 1;
            public const int No = 2;
        }
        public static class MPType
        {
            public const int Residence = 1;
            public const int Road = 2;
            public const int Country = 3;
            public const int All = 0;
        }
        public static class CertificateType
        {
            public const int Placename = 1;
            public const int MPZ = 2;
            public const int All = 3;
        }
        public static class MPTypeStr
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
        public static class RPFilesType
        {
            public const string RPImages = "RPImages";
            public const string RPCodeImage = "RPCodeImage";

        }
        public static class RPRepairFinish
        {
            public const int Yes = 1;
            public const int No = 0;
        }

        public static class RPRepairMode
        {
            public const int Repair = 1;
            public const int Change = 2;
        }
    }
}