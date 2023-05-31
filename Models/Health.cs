using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuqinMiniAppBase.Models
{
	[Table("health")]
	public class Health
	{
		public int id { get; set; }
		public string name { get; set; }
		public string gender { get; set; }
		public string disease { get; set; }
		public string depression_level { get; set; }
		public int use_drug { get; set; }
		public string drugs_memo { get; set; }
		public int special_diet { get; set; }
		public string diet_memo { get; set; }
		public int need_others_service { get; set; }
		public string service_memo { get; set; }

    }
}

