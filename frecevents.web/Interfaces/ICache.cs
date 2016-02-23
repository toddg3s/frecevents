using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frecevents.web.Interfaces
{
    public interface ICache
    {
        object Get(string key);
        void Set(string key, object value);
        void Clear(string key);
    }
}
