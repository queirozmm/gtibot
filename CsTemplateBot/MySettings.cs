using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.TalkService;
using Takenet.Mpa.MpaTranslateLime;

namespace CsTemplateBot
{
    [DataContract]
    public class MySettings
    {
        [DataMember(Name = "mpa")]
        public MPASettings MPASettings { get; set; }

        [DataMember(Name = "talkService")]
        public Settings TalkServiceSettings { get; set; }
     
        [DataMember(Name = "metrics")]
        public MetricsSettings Metrics { get; set; }
    }

    [DataContract]
    public class MetricsSettings
    {
        [DataMember(Name = "httpEndpointPort")]
        public int HttpEndpointPort { get; set; }

        [DataMember(Name = "reportingConnectionString")]
        public string ReportingConnectionString { get; set; }

        [DataMember(Name = "reportingFrequencyInMinutes")]
        public int ReportingFrequencyInMinutes { get; set; }
    }
   
}