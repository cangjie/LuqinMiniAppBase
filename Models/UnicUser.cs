﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LuqinMiniAppBase.Models
{
    [Table("users")]
    public class UnicUser
    {
        [Key]
        public int id { get; set; }

        public string oa_union_id { get; set; }

       
        public int is_admin { get; set; } = 0;

        [NotMapped]
        public string sessionKey { get; set; } = "";

    }
}
