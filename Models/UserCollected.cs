using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LuqinMiniAppBase.Models
{
	[Table("user_collected")]
	public class UserCollected
	{
		[Key]
		public int id { get; set; }

		public string wechat_union_id { get; set; }
		
		public string cell { get; set; }
		public string real_name { get; set; }
		public string gender { get; set; }
		public int is_admin { get; set; } = 0;

    }
}

