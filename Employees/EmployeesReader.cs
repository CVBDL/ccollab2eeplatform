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

        private static List<Employee> _employees = null;

        /// <summary>
        /// Read employees from json file.
        /// </summary>
        /// <returns>Deserialized employees json object.</returns>
        public static List<Employee> GetEmployees()
        {

            if (_employees == null)
            {
                string json = string.Empty;

                StreamReader sr = new StreamReader(EMPLOYEES_JSON_FILENAME, Encoding.UTF8);
                using (sr)
                {
                    json = sr.ReadToEnd();
                }

                if (string.IsNullOrWhiteSpace(json))
                    return null;
                
                try
                {
                    _employees = JsonConvert.DeserializeObject<List<Employee>>(json);
                }
                catch (Exception exp)
                {
                    log.Error(string.Format("Failed to load from json: {0}", exp.Message));
                    throw;
                }
            }

            return _employees;
        }

        /// <summary>
        /// Get employee's product name.
        /// </summary>
        /// <param name="loginName">Employee's login name.</param>
        /// <returns></returns>
        public static string GetEmployeeProductName(string loginName)
        {
            List<Employee> employees = GetEmployees();

            foreach (Employee employee in employees)
            {
                if (employee.LoginName == loginName)
                {
                    return employee.ProductName;
                }
            }

            return "";
        }

        public static List<Employee> GetEmployeesByProduct(string productName)
        {
            List<Employee> employees = GetEmployees();

            List<Employee> employeesOfProduct = new List<Employee>();

            foreach (Employee employee in employees)
            {
                if (employee.ProductName == productName)
                {
                    employeesOfProduct.Add(employee);
                }
            }

            return employeesOfProduct;
        }

        public static string GetEmployeeFullNameByLoginName(string loginName)
        {
            List<Employee> employees = GetEmployees();

            string fullName = string.Empty;

            foreach (Employee employee in employees)
            {
                if (employee.LoginName == loginName)
                {
                    return employee.FullName;
                }
            }

            return fullName;
        }
    }
}
