using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LuqinMiniAppBase.Models
{
    [Table("syncsns")]
    public class SyncSns
    {
        [Key]
        public int syncsns_id { get; set; }

        public string syncsns_content { get; set; }
    }
}
