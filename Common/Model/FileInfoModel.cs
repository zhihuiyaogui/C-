using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class FileInfoModel
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string Ext { get; set; }
        public string Path { get; set; }
        public string MimeType { get; set; }
        public string Describe { get; set; }
    }
}
