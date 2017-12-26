using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FlyingDutchman.Models
{
    public class Statistic
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public int DAU { get; set; }
        public int BuyNum { get; set; }
        public int AddNum { get; set; }
        public int CollectNum { get; set; }
        public int TotalNum { get; set; }
        public int TotalGood { get; set; }
    }



}