using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace OskalaAbo.Models
{
    public class ResetPasswordModel
    {
        public bool formRegisterNewPassword { get; set; } = false;

        [Required]
        [Display(Name = "Action")]
        public string a { get; set; }



        [Required]
        [RegexStringValidator("[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}")]
        [Display(Name = "JobID")]
        public string authid { get; set; }




        [Required]
        [RegexStringValidator("[0-9a-fA-F]{128}")]
        [Display(Name = "TokenID")]
        public string token { get; set; }



        [Required]
        [EmailAddress]
        [Display(Name = "Courrier électronique")]
        public string uemail { get; set; }



        [Required]
        [IntegerValidator(MinValue = 0)]
        [Display(Name = "UserID")]
        public int uid { get; set; }



        [Required]
        [IntegerValidator(MinValue = 0)]
        [Display(Name = "SiteID")]
        public int siteid { get; set; }



        [RegexStringValidator(@"^[a-zA-Z0-9,_,-,.,=,?,!,*,+,-,\/,@,%,$,#]{5,32}$")]
        [Display(Name = "NewPassword")]
        public string pwd { get; set; }
    }
}