using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LuqinMiniAppBase.Models
{
	[Table("visa")]
	public class VisaCity
	{
		[Key]
		public int id { get; set; }

		public string openId { get; set; }
		public string child_name { get; set; }
		public string visa_city { get; set; }
		public string memo { get; set; }

	}
}

