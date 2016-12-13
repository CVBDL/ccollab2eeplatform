using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using log4net;

namespace EagleEye
{
    class EmployeesGenerator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EmployeesGenerator));

        private static readonly string EMPLOYEES_JSON_FILENAME = "employees.json";

        private static List<Employee> employees = null;

        public static List<Employee> GetEmployees()
        {

            if (employees == null)
            {
                string json = String.Empty;

                StreamReader sr = new StreamReader(EMPLOYEES_JSON_FILENAME, Encoding.UTF8);
                using (sr)
                {
                    json = sr.ReadToEnd();
                }

                if (String.IsNullOrWhiteSpace(json))
                    return null;
                
                try
                {
                    employees = JsonConvert.DeserializeObject<List<Employee>>(json);
                }
                catch (Exception exp)
                {
                    log.Error(String.Format("Failed to load from json: {0}", exp.Message));
                    throw;
                }
            }

            return employees;
        }
    }
}
