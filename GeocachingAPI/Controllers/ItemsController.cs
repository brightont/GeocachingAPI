using GeocachesDAO;
using GeocachingAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace GeocachingAPI.Controllers
{
    public class ItemsController : ApiController
    {
        /// <summary>
        /// Regular get method
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Items> Get()
        {
            using (GeocachesEntities db = new GeocachesEntities())
            {
                return db.Items.ToList();
            }

        }

        /// <summary>
        /// Method that gets active items in a given geocache
        /// </summary>
        /// <param name="value">geocache id or name</param>
        /// <returns></returns>
        public IEnumerable<Items> GetActiveItems(string value)
        {
            // Since the problem didn't specify whether the geocache would be identified via its
            // id or name, we will do both.
            using (GeocachesEntities db = new GeocachesEntities())
            {
                // Get the guid from the geocache table that matches to the specified name. The name column in the Geocaches table has a unique index
                // and it is implied that all names are unique (similar to how the problem specified that all items are unique)

                // The default guid is 00000000-0000-0000-0000-000000000000. No geocache_ids should match that if the
                // specified geocache name doesn't exist
                Guid guid = db.Geocaches.Where(g => (g.name.Equals(value))).Select(g => g.id).FirstOrDefault();
                return db.Items.Where(i => (i.geocache_id.ToString().Equals(value) || i.geocache_id == guid)
                && i.end_time == null).ToList();
            }
            
        }

        /// <summary>
        /// Add an item to a geocache
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [Route("api/items/additem")]
        public IHttpActionResult PostAddItem(NewItem item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            GeocachesEntities db = new GeocachesEntities();
            
            // We want to check that there is an existing geocache matching either the name or id given
            // Both the geocache name and id cannot be null, otherwise we cannot add it to any geocache
            if (item.geocacheID == null && item.geocacheName == null)
            {
                return BadRequest("Both geocache name and geocache id cannot be null.");
            }

            // Get a guid that either matches to geocache ID or geocache name
            var guidQuery = db.Geocaches.Where(g => (g.id == item.geocacheID || g.name.Equals(item.geocacheName))).Select(g => g.id);
            if (guidQuery.Count() > 1)
            {
                return BadRequest("Entered a geocache id and geocache name that do not match to same geoecache.");
            }

            Guid guid = guidQuery.FirstOrDefault();
            if (guid == Guid.Empty)
            {
                return BadRequest("Both geocache name and geocache id are not valid.");
            }

            // Check if the name matches the requirements
            Regex matcher = new Regex("^([A-Za-z0-9 ]){1,50}$");
            if (item.name == null || !matcher.IsMatch(item.name))
            {
                return BadRequest("Name must be composed of alphanumeric or space characters. Maximum length of 50.");
            }

            // Check if name previously exists
            if (db.Items.Where(i => i.name.Equals(item.name)).Any())
            {
                return BadRequest("Item name already exists.");
            }

            db.Items.Add(new Items()
            {
                id = Guid.NewGuid(),
                name = item.name,
                geocache_id = guid,
                start_time = item.startTime,
                end_time = item.endTime

            });
            db.SaveChanges();
            db.Dispose();
            
            return Ok();
        }

        /// <summary>
        /// Move item to another active geocache
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [Route("api/items/moveitem")]
        public IHttpActionResult PostMoveItem([FromBody]MovableItem item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            GeocachesEntities db = new GeocachesEntities();

            // Users can identify an item either by its id or its name. Both options, however, cannot be null
            if (item.id == null && item.name == null)
            {
                return BadRequest("Both item id and name cannot be null.");
            }

            // We will use this query later, so save it. We are checking if there any items that match the name or id
            var query = db.Items.Where(i => (i.id == item.id || i.name.Equals(item.name)));
            if (!query.Any())
            {
                return BadRequest("Both item id and name are invalid.");
            }

            if (query.Count() > 1)
            {
                return BadRequest("Entered an item id and name that do not match to the same item.");
            }

            // Active items don't have an end time
            if (!query.Where(i => i.end_time == null).Any())
            {
                return BadRequest("Only active items can be moved.");
            }

            // We want to check that there is an existing geocache matching either the name or id given
            // Both the geocache name and id cannot be null, otherwise we cannot add it to any geocache
            if (item.geocacheName == null && item.geocacheID == null)
            {
                return BadRequest("Both geocache name and geocache id cannot be null.");
            }

            // Get a guid that either matches to geocache ID or geocache name
            var guidQuery = db.Geocaches.Where(g => (g.id == item.geocacheID || g.name.Equals(item.geocacheName))).Select(g => g.id);
            if (guidQuery.Count() > 1)
            {
                return BadRequest("Entered a geocache id and geocache name that do not match to same geoecache.");
            }

            Guid guid = guidQuery.FirstOrDefault();
            if (guid == Guid.Empty)
            {
                return BadRequest("Both geocache name and geocache id are not valid.");
            }

            // Check how many items are in the specific geocache. If it is more than 3, then we cannot 
            // add any more items
            if (db.Items.Where(i => i.geocache_id == guid).Count() >= 3)
            {
                return BadRequest("Cannot move item to desired geocache because there are already 3+ items in there.");
            }

           
            // There should only be a single result anyways bc both the id and the name must be unique
            var result = query.Single();
            result.geocache_id = guid;
            db.SaveChanges();
            db.Dispose();

            return Ok();
        }

    }
}
