using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LuqinMiniAppBase.Models
{
    [Table("poster_qrcode_scan_log")]
    public class QrCodeScanLog
    {
        [Key]
        public int id { get; set; }

        public int poster_user_id { get; set; }
        public int scan_user_id { get; set; }
        public string original_id { get; set; } 
        public string open_id { get; set; }
        public int deal { get; set; } = 0;

        public DateTime create_date { get; set; }
        
    }
}
