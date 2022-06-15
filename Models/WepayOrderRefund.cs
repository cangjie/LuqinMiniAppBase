using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuqinMiniAppBase.Models
{
    [Table("wepay_order_refund")]
    public class WepayOrderRefund
    {
        [Key]
        public int id { get; set; }
        public string wepay_out_trade_no { get; set; }
        public int amount { get; set; }
        public int state { get; set; }
        public int oper_user_id { get; set; }
    }
}
