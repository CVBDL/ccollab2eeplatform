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
    internal class Employee
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Employee));

 
        public string fullName;
        public string product;
        public string name;


        public static List<Employee> InifFromJson( string json)
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
