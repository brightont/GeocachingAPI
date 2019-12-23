TO VIEW ALL GEOCACHES
{url}/api/geocaches

TO VIEW ALL ITEMS
{url}/api/items

TO VIEW ALL ACTIVE ITEMS
{url}/api/items/{id or name}
Example: {url}/api/items/57776BA6-537D-496A-A8C0-6779D67BB87D OR {url}/api/items/geocache3

TO ADD AN ITEM
{url}/api/items/additem
body {
    "name": [string],
    "geocacheID": [guid or null],
    "geocacheName": [name or null],
    "startTime": [datetime],
    "endTime": [datetime or null]
}
Example Body:
body {
    "name": "item7",
    "geocacheID": "57776BA6-537D-496A-A8C0-6779D67BB87D",
    "geocacheName": "geocache3",
    "startTime": "2019-12-21T14:17:42.98",
    "endTime": null
}
-GeocacheID and GeocacheName cannot both be null. It will throw an error. Both options are presented if you only know one or the other
-GeocacheID and GeocacheName must match to the same geocache. If you don't know both, you can use just one.

TO MOVE AN ITEM TO ACTIVE GEOCACHE
{url}/api/items/moveitem
body {
  "id": [guid or null],
  "name": [string or null],
  "geocacheID": [guid or null],
  "geocacheName": [string or null]
}

Example Body:
body {
  "id": "837F30C7-6337-4552-AE5F-3465B4471765",
  "name": "item2",
  "geocacheID": "59A36383-FCF1-416D-A812-E9A3F6060BFD",
  "geocacheName": "geocache2"
}

-GeocacheID and GeocacheName cannot both be null. It will throw an error. Both options are presented if you only know one or the other. Same goes for id/name.
-GeocacheID and GeocacheName must match to the same geocache. If you don't know both, you can use just one. Same goes for id/name.

General Notes:
-To determine if a geocache item is active, its end time will be NULL because it has not happened yet.
-For all post requests, I tested them using Postman. 
-I have MSSQL, and created my own db called Geocaches and populated the data. The primary keys are the ids and there are unique indexes on the names. The implied foreign key of the Items table is geocache_id.