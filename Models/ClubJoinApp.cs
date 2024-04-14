using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LuqinMiniAppBase.Models
{
	public class ClubJoinApp
	{
		public int club_id { get; set; }
		public int user_id { get; set; }
		public string cell { get; set; }
		public string real_name { get; set; }
		public string gender { get; set; }
		public string memo { get; set; }

	}
}

