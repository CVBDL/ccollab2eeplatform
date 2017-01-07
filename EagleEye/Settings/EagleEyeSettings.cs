using System.Collections.Generic;

namespace EagleEye.Settings
{
    public class EagleEyeSettings
    {
        public string ApiRootEndpoint { get; set; }
        public Dictionary<string, ChartSettings> Charts { get; set; }
        public List<string> DefectInjectionStage { get; set; }
        public List<string> DefectSeverityTypes { get; set; }
        public List<string> DefectTypes { get; set; }
        public List<string> Products { get; set; }
        public List<ProductChartSettings> ReviewCountByCreator { get; set; }
        public List<ProductChartSettings> InspectionRateByMonth { get; set; }
        public List<ProductChartSettings> DefectDensityChangedByMonth { get; set; }
        public List<ProductChartSettings> CommentDensityChangedByMonth { get; set; }
        public List<ProductChartSettings> ReviewCountByMonth { get; set; }
    }

    public class ChartSettings
    {
        public string ChartId { get; set; }
    }

    public class ProductChartSettings
    {
        public string ProductName { get; set; }
        public string ChartId { get; set; }
    }
}
