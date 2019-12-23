using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeocachingAPI.Models
{
    public class NewItem
    {
        public string name { get; set; }

        public Nullable<Guid> geocacheID { get; set; }

        public string geocacheName { get; set; }

        public DateTime startTime { get; set; }

        public Nullable<System.DateTime> endTime { get; set; }
    }
}