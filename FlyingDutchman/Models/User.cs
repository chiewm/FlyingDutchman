using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FlyingDutchman.Models
{
    public class User
    {
        
        public int Id { get; set; }
        public string Ip { get; set; }
        public string Operation { get; set; }
        public string Good { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        
        public DateTime? CreatedOn { get; set; }
    }
 

}