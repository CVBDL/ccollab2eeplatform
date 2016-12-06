using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using log4net;

namespace eeDataGenerator
{
    /// <summary>
    /// JSON payload format used to update chart datatable to EagleEye platform.
    /// https://github.com/CVBDL/EagleEye-Docs/blob/master/rest-api/rest-api.md#edit-data-table
    /// </summary>
    internal class Chart
    {
        public object[][] datatable { get; set; }

        public Chart(object[][] dt)
        {
            datatable = dt;
        }
    }

    /// <summary>
    /// Application level settings JSON format.
    /// </summary>
    internal class ApplicationSettings
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ApplicationSettings));

        public static string APPLICATION_FILE_NAME = "application-settings.json";

        public string EagleEyeApiRootEndpoint;

        public Dictionary<string, ChartItem> Charts;

        public static ApplicationSettings InitFromJson()
        {
            string json = String.Empty;

            StreamReader sr = new StreamReader(APPLICATION_FILE_NAME, Encoding.Default);
            using (sr)
            {
                json = sr.ReadToEnd();
            }

            if (string.IsNullOrWhiteSpace(json))
                return null;

            ApplicationSettings application = null;
            try
            {
                application = JsonConvert.DeserializeObject<ApplicationSettings>(json);
            }
            catch (Exception exp)
            {
                log.Error(string.Format("Failed to load from json: {0}", exp.Message));
                throw;
            }

            return application;
        }
    }

    /// <summary>
    /// A single chart settings JSON format.
    /// </summary>
    internal class ChartItem
    {
        public string ChartId;
    }
}
