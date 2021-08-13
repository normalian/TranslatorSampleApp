using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorSampleApp.Models
{
    public class TranslationInput
    {
        public Input[] inputs { get; set; }
    }

    public class Input
    {
        public Source source { get; set; }
        public Target[] targets { get; set; }
    }

    public class Source
    {
        public string sourceUrl { get; set; }
        public string storageSource { get; set; }
        public string language { get; set; }
    }

    public class Target
    {
        public string targetUrl { get; set; }
        public string storageSource { get; set; }
        public string category { get; set; }
        public string language { get; set; }
    }
}
