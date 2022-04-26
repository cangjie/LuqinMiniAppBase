using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LuqinMiniAppBase.Models
{
    [Table("question")]
    public class Question
    {
        [Key]
        public int id { get; set; }

        public int user_id { get; set; }
        public string topic { get; set; }
        public string status { get; set; }
        public DateTime create_date { get; set; }
    }
}
