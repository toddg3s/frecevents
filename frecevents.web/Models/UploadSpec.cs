using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frecevents.web.Models
{
    public class UploadSpec : ModelBase
    {
        public string Filedata { get; set; }
        public bool Fileuploaded { get; set; }
        public string DataType { get; set; }
        public List<string> Fields { get; set; }
        public bool Headers { get; set; }
        public Dictionary<string, string> Mapping { get; set; }
    }
}
