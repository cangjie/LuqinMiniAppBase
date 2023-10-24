using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuqinMiniAppBase.Models
{
	[Table("camp_registration")]
	public class CampRegistration
	{
		[Key]
		public int		id                  {get; set;}
		public string	camp_name           {get; set;}
        public string   child_name          {get; set;}
        public string   child_gender        {get; set;}
        public string   child_age           {get; set;}
        public string   region              {get; set;}
        public string   child_length        {get; set;}
        public string   child_weight        {get; set;}
        public string   child_id_no         {get; set;}
        public string   child_disease       {get; set;}
        public string   child_allergy       {get; set;}
        public string   contact_pri_name    {get; set;}
        public string   contact_pri_cell    {get; set;}
        public string   contact_sec_name    {get; set;}
        public string   contact_sec_cell    {get; set;}


    }
}

