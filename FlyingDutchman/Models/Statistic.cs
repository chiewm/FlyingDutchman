using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FlyingDutchman.Models
{
    public class Statistic
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }

    }

    public class StatisticDBContext : DbContext
    {
        public DbSet<Statistic> Statistics { get; set; }
    }


}