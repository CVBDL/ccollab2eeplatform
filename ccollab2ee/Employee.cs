using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using log4net;
using log4net.Config;


namespace CcollabLauncher
{
    internal class Employee
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Employee));

 
        public string FullName;
        public string ProductName;
        public string LoginName;


        public static List<Employee> InitFromJson( string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            List<Employee> developers = null;
            try
            {
                developers = JsonConvert.DeserializeObject<List<Employee>>(json);
            }
            catch (Exception exp)
            {
                log.Error(string.Format("Failed to load from json: {0}", exp.Message));
                throw;
            }

            return developers;
        }
    }
}
