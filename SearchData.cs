using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngine
{
    public class SearchData
    {
        public string Word { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int Distance { get; set; }
        public int Frequency { get; set; }
    }
}
