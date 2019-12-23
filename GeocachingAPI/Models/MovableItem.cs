using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeocachingAPI.Models
{
    public class MovableItem
    {
        public Nullable<Guid> id { get; set; }

        public string name { get; set; }

        public Nullable<Guid> geocacheID { get; set; }

        public string geocacheName { get; set; }
    }
}