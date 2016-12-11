using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using log4net;
using log4net.Config;


namespace EagleEye
{
    internal class Employee
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Employee));

        public static string EMPLOYEES_FILE_NAME = "employees.json";

        public string FullName;
        public string ProductName;
        public string LoginName;


        /// <summary>
        /// Read employees list from `employees.json` file.
        /// </summary>
        /// <returns>Employee list.</returns>
        public static List<Employee> InitFromJson()
        {
            string json = String.Empty;

            StreamReader sr = new StreamReader(EMPLOYEES_FILE_NAME, Encoding.Default);
            using (sr)
            {
                json = sr.ReadToEnd();
            }

            if (string.IsNullOrWhiteSpace(json))
                return null;

            List<Employee> employees = null;
            try
            {
                employees = JsonConvert.DeserializeObject<List<Employee>>(json);
            }
            catch (Exception exp)
            {
                log.Error(string.Format("Failed to load from json: {0}", exp.Message));
                throw;
            }

            return employees;
        }
    }
}
