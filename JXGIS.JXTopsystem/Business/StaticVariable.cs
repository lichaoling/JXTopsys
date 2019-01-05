using JXGIS.JXTopsystem.Models.Extends;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Business
{
    public class StaticVariable
    {
        public static readonly string basePath = AppDomain.CurrentDomain.BaseDirectory;

        //***************门牌制作****************
        public static readonly string basePathMPProduce = Path.Combine(basePath, "Files", "MPProduce");
        public static readonly string PLMPProducePath_Full = Path.Combine(basePathMPProduce, "批量");
        public static readonly string LXMPProducePath_Full = Path.Combine(basePathMPProduce, "零星");

        //***************门牌证打印****************
        public static readonly string basePathMPPrint = Path.Combine(basePath, "Files", "MPPrint");
        public static readonly string DZZMtemplateFile = Path.Combine(basePathMPPrint, "地址证明模板.docx");
        public static readonly string MPZtemplateFile = Path.Combine(basePathMPPrint, "门牌证模板.docx");
        public static readonly string DZZMPrintPath = Path.Combine(basePathMPPrint, "地址证明");
        public static readonly string MPZPrintPath = Path.Combine(basePathMPPrint, "门牌证");
        public static readonly string TempPrintPath = Path.Combine(basePathMPPrint, "Temp");
        public static readonly string MergeFilePath = Path.Combine(basePathMPPrint, "Merge");
        public static readonly string MergeFile = Path.Combine(MergeFilePath, "merge.pdf");

        //***************门牌编制文件****************
        public static readonly string baseRelativePathMPFile = Path.Combine("Files", "MPFile");
        // 住宅门牌上传相对路径
        public static readonly string residenceMPRelativePath = Path.Combine(baseRelativePathMPFile, Enums.MPTypeCh.Residence);
        // 道路门牌上传相对路径
        public static readonly string roadMPRelativePath = Path.Combine(baseRelativePathMPFile, Enums.MPTypeCh.Road);
        // 农村门牌上传相对路径
        public static readonly string countryMPRelativePath = Path.Combine(baseRelativePathMPFile, Enums.MPTypeCh.Country);


        //***************路牌编制文件****************
        public static readonly string baseRelativePathRPFile = Path.Combine("Files", "RPFile");
        //路牌标志照片上传相对路径
        public static readonly string RPBZPhotoRelativePath = Path.Combine(baseRelativePathRPFile, "路牌照片");
        //路牌二维码照片上传相对路径
        public static readonly string RPQRCodeRelativePath = Path.Combine(baseRelativePathRPFile, "路牌二维码");
        //路牌维修前后照片上传相对路径
        public static readonly string RPRepairPhotoRelativePath = Path.Combine(baseRelativePathRPFile, "维修照片");


        public static readonly string basePathLogMessage = Path.Combine(basePath, "Logs");



        //***************文件归档****************
        public static readonly string basePathArchiveFile = Path.Combine(basePath, "Files", "ArchiveFiles");
        // 住宅门牌上传绝对路径
        public static readonly string residenceMPPath = Path.Combine(basePath, residenceMPRelativePath);
        // 道路门牌上传绝对路径
        public static readonly string roadMPPath = Path.Combine(basePath, roadMPRelativePath);
        // 农村门牌上传绝对路径
        public static readonly string countryMPPath = Path.Combine(basePath, countryMPRelativePath);
    }
}