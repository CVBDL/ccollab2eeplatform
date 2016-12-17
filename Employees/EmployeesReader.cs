using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Employees
{
    public class EmployeesReader
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EmployeesReader));

        private const string EMPLOYEES_JSON_FILENAME = "ConfigurationFiles/employees.json";

        private static List<Employee> employees = null;

        /// <summary>
        /// Read employees from json file.
        /// </summary>
        /// <returns>Deserialized employees json object.</returns>
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

        /// <summary>
        /// Get employee's product name.
        /// </summary>
        /// <param name="loginName">Employee's login name.</param>
        /// <returns></returns>
        public static string GetEmployeeProductName(string loginName)
        {
            foreach (Employee employee in employees)
            {
                if (employee.LoginName == loginName)
                {
                    return employee.ProductName;
                }
            }

            return "";
        }
    }
}
