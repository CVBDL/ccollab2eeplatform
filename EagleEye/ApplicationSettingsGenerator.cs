using System;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using log4net;

namespace EagleEye
{
    public class ApplicationSettingsGenerator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ApplicationSettingsGenerator));

        private static readonly string APPLICATION_SETTINGS_JSON_FILENAME = "application-settings.json";

        private static ApplicationSettings settings = null;

        /// <summary>
        /// Read application settings from json file.
        /// </summary>
        /// <returns>Deserialized application settings json object.</returns>
        public static ApplicationSettings GetApplicationSettings()
        {

            if (settings == null)
            {
                string json = String.Empty;

                StreamReader sr = new StreamReader(APPLICATION_SETTINGS_JSON_FILENAME, Encoding.UTF8);
                using (sr)
                {
                    json = sr.ReadToEnd();
                }

                if (String.IsNullOrWhiteSpace(json))
                    return null;

                try
                {
                    settings = JsonConvert.DeserializeObject<ApplicationSettings>(json);
                }
                catch (Exception exp)
                {
                    log.Error(String.Format("Failed to load from json: {0}", exp.Message));
                    throw;
                }
            }

            return settings;
        }
    }
}
