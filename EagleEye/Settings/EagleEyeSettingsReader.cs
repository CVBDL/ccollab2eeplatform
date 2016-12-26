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

        private static readonly string EAGLEEYE_SETTINGS_FILENAME = "ConfigurationFiles/eagleeye-settings.json";

        private static EagleEyeSettings settings = null;

        public static EagleEyeSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    string json = string.Empty;

                    StreamReader sr = new StreamReader(EAGLEEYE_SETTINGS_FILENAME, Encoding.UTF8);
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
                        return null;
                    }
                }

                return settings;
            }
        }
    }
}
