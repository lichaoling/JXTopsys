﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JXGIS.JXTopsystem.Models.Entities
{
    [Table("RPOFUPLOADFILES")]
    public class RPOfUploadFiles
    {
        [Key]
        public string ID { get; set; }
        public string RPID { get; set; }

        public string Name { get; set; }
        public string FileType { get; set; }
        public int State { get; set; }
    }
}