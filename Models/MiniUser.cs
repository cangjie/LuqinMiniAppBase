using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LuqinMiniAppBase.Models
{
    [Table("oa_users")]
    public class MiniUser
    {
        [Key]
        public int id { get; set; } = 0;

        public int user_id { get; set; } = 0;
        public string original_id { get; set; } = "";
        public string open_id { get; set; } = "";

        [NotMapped]
        public string sessionKey { get; set; } = "";
    }
}