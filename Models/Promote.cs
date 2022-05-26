using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LuqinMiniAppBase.Models
{
    [Table("promote_log")]
    public class Promote
    {
        [Key]
        public int id { get; set; }

        public string original_id { get; set; }
        public int promote_user_id { get; set; }
        public string promote_open_id { get; set; }
        public int follow_user_id { get; set; }
        public string follow_open_id { get; set; }
        public DateTime create_date { get; set; } = DateTime.Now;

        
        
    }
    [NotMapped]
    public class PromoteResult
    {
       
        public DateTime date { get; set; }
        public int userId { get; set; }
        public int followNum { get; set; } = 0;
        public int scanNum { get; set; } = 0;
    }
}
