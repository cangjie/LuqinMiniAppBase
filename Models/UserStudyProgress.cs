using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LuqinMiniAppBase.Models
{
    [Table("user_study_progress")]
    public class UserStudyProgress
    {
        [Key]
        public int id { get; set; }

        public int user_id { get; set; }
        public int media_subtitle_id { get; set; }
        public int progress { get; set; }
        public DateTime update_date { get; set; }

        [ForeignKey("media_subtitle_id")]
        public MediaSubTitle mediaSubTitle { get; set; }
    }
}
