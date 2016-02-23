using frecevents.web.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frecevents.web
{
    public class Root
    {
        private static IDataAccess _data;

        public static IDataAccess Data
        {
            get
            {
                if (_data == null)
                {
                    _data = new frecevents.web.Components.SqlDataAccess();
                }
                return _data;
            }
        }

        private static ICache _cache;

        public static ICache Cache
        {
            get
            {
                if (_cache == null)
                {
                    _cache = new frecevents.web.Components.ASPNetCache();
                }
                return _cache;
            }
        }
    }
}
