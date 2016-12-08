using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using log4net;

namespace eeDataGenerator
{
    /// <summary>
    /// Application level settings JSON format.
    /// </summary>
    internal class ApplicationSettings
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ApplicationSettings));

        public static string APPLICATION_FILE_NAME = "application-settings.json";

        public string EagleEyeApiRootEndpoint;

        public Dictionary<string, ChartSettings> Charts;

        /// <summary>
        /// Read application settings from `employees.json` file.
        /// </summary>
        /// <returns>Application setttings.</returns>
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
}
