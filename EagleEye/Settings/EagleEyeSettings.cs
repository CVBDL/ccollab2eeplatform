using System.Collections.Generic;

namespace EagleEye.Settings
{
    public class EagleEyeSettings
    {
        public string ApiRootEndpoint { get; set; }
        public Dictionary<string, ChartSettings> Charts { get; set; }
        public List<string> DefectSeverityTypes { get; set; }
    }

    public class ChartSettings
    {
        public string ChartId { get; set; }
    }
}
