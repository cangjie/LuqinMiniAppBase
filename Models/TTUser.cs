using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LuqinMiniAppBase.Models
{
	[Table("tt_user")]
	public class TTUser
	{
		
		public string open_id { get; set; }
		public string app_id { get; set; }
		public int user_id { get; set; } = 0;
	}
}

