using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LuqinMiniAppBase.Models
{
    [Table("media_subtitle")]
    public class MediaSubTitle
    {
        [Key]
        public int id { get; set; }
        //[ForeignKey("media_id")]
        //[Column("media_id")]
        public int media_id { get; set; }
        public string title { get; set; }
        public int start_position { get; set; }
        public int end_position { get; set; }

        public static explicit operator MediaSubTitle(UserStudyProgress v)
        {
            throw new NotImplementedException();
        }

        //[ForeignKey("media_subtitle_id")]
        //public List<UserStudyProgress> userStudyProgress { get; set; }

        //public Media media { get; set; }

    }
}
