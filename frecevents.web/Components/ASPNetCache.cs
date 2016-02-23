using frecevents.web.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace frecevents.web.Components
{
    public class ASPNetCache : ICache
    {
        public object Get(string key)
        {
            return HttpContext.Current.Cache["eventlist"];
        }

        public void Set(string key, object value)
        {
            if(HttpContext.Current.Cache[key]!=null)
            {
                HttpContext.Current.Cache[key] = value;
            }
            else
            {
                HttpContext.Current.Cache.Add(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, 
                    System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
            }
        }

        public void Clear(string key)
        {
            HttpContext.Current.Cache.Remove(key);
        }
    }
}
