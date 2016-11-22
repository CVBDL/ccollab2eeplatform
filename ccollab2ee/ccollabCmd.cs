using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using log4net;


namespace CcollabLauncher
{
    internal class ccollabCmd
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ccollabCmd));

        public string cmdName;
        public string args;
        public string relUrl;

        public static List<ccollabCmd> InifFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            List<ccollabCmd> ccollabCmds = null; 
            try
            {
                ccollabCmds = JsonConvert.DeserializeObject<List<ccollabCmd>>(json);
            }
            catch(Exception exp)
            {
                log.Error(string.Format("Failed to load from json: {0}", exp.Message));
                throw;
            }

            return ccollabCmds;
        }
    }
}
