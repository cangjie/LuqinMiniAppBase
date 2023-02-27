using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LuqinMiniAppBase.Models
{
    [Table("reserve")]
    public class Reserve
	{
		[Key]
		public int id { get; set; }
		public int act_id { get; set; }
		public int user_id { get; set; }
		public string filled_name { get; set; }
		public string filled_cell { get; set; }
		public DateTime create_date { get; set; }

		[NotMapped]
		public string memo { get; set; }
	}
}

