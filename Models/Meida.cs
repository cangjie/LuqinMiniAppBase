using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LuqinMiniAppBase.Models
{
    [Table("media")]
    public class Media
    {
        [Key]
        public int id { get; set; }

        public string name { get; set; } = "";
        public string type { get; set; } = "";
        public string path { get; set; } = "";
        public int free { get; set; } = 0;
        public string thumb { get; set; } = "";

        public int duration { get; set; } = 0;

        public int file_size { get; set; } = 0;

        [ForeignKey("media_id")]
        public List<MediaSubTitle> mediaSubTitles { get; set; }

    }
}
