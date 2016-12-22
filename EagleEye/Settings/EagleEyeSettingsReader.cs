using log4net;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace EagleEye.Settings
{
    public class EagleEyeSettingsReader
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EagleEyeSettingsReader));

        private const string EAGLEEYE_SETTINGS_JSON_FILENAME = "ConfigurationFiles/eagleeye-settings.json";

        private static EagleEyeSettings settings = null;

        /// <summary>
        /// Read application settings from json file.
        /// </summary>
        /// <returns>Deserialized application settings json object.</returns>
        public static EagleEyeSettings GetEagleEyeSettings()
        {

            if (settings == null)
            {
                string json = string.Empty;

                StreamReader sr = new StreamReader(EAGLEEYE_SETTINGS_JSON_FILENAME, Encoding.UTF8);
                using (sr)
                {
                    json = sr.ReadToEnd();
                }

                if (string.IsNullOrWhiteSpace(json))
                {
                    return null;
                }

                try
                {
                    settings = JsonConvert.DeserializeObject<EagleEyeSettings>(json);
                }
                catch (Exception exp)
                {
                    log.Error(string.Format("Failed to load from json: {0}", exp.Message));
                }
            }

            return settings;
        }

        public static EagleEyeSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    string json = string.Empty;

                    StreamReader sr = new StreamReader(EAGLEEYE_SETTINGS_JSON_FILENAME, Encoding.UTF8);
                    using (sr)
                    {
                        json = sr.ReadToEnd();
                    }

                    if (string.IsNullOrWhiteSpace(json))
                        return null;

                    try
                    {
                        settings = JsonConvert.DeserializeObject<EagleEyeSettings>(json);
                    }
                    catch (Exception exp)
                    {
                        log.Error(string.Format("Failed to load from json: {0}", exp.Message));
                        throw;
                    }
                }

                return settings;
            }
        }
    }
}
