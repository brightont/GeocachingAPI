using GeocachesDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GeocachingAPI.Controllers
{
    public class GeocachesController : ApiController
    {
        /// <summary>
        /// Regular get method
        /// </summary>
        /// <returns></returns>
        public List<Geocaches> Get()
        {
         
            using (GeocachesEntities db = new GeocachesEntities())
            {
                return db.Geocaches.ToList();
            }
        }

        
    }
}
