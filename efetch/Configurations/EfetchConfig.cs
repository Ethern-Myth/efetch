using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace efetch.Configurations
{
    public class EfetchConfig
    {
        public string BaseUrl { get; set; } = "";
        public Dictionary<string, string> DefaultHeaders { get; set; } = new Dictionary<string, string>();
        public int RetryCount { get; set; } = 3;
        public Func<int, TimeSpan> RetryInterval { get; set; } = retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
    }
    
}
