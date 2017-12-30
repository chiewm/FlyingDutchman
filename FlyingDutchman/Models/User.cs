using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlyingDutchman.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Ip { get; set; }
        public string Operation { get; set; }
        public string Good { get; set; }
        public bool Sex { get; set; }
        public string Address { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        
        public DateTime? CreatedOn { get; set; }
    }
 

}