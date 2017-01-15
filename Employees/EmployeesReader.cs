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

        private static readonly string SETTINGS_FILENAME = "ConfigurationFiles/employees.json";

        private static List<Employee> employees = null;

        public static List<Employee> Employees
        {
            get
            {
                if (employees == null)
                {
                    string json = string.Empty;

                    StreamReader sr = new StreamReader(SETTINGS_FILENAME, Encoding.UTF8);
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
                        employees = JsonConvert.DeserializeObject<List<Employee>>(json);
                    }
                    catch (Exception e)
                    {
                        log.Error(string.Format("An error occurred when deserializing file: {0}", SETTINGS_FILENAME));
                        log.Error(string.Format("Exception: {0}", e.Message));
                    }
                }

                return employees;
            }
        }

        /// <summary>
        /// Get the product name a employee belongs to.
        /// </summary>
        /// <param name="loginName">Code collaborator login name.</param>
        /// <returns></returns>
        public static string GetEmployeeProductName(string loginName)
        {
            foreach (Employee employee in Employees)
            {
                if (employee.LoginName == loginName)
                {
                    return employee.ProductName;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Get all the employees belongs to a product.
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        public static List<Employee> GetEmployeesByProduct(string productName)
        {
            List<Employee> employees = new List<Employee>();

            foreach (Employee employee in Employees)
            {
                if (employee.ProductName == productName)
                {
                    employees.Add(employee);
                }
            }

            return employees;
        }

        /// <summary>
        /// Get a employee's full name by his/her login name.
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public static string GetEmployeeFullNameByLoginName(string loginName)
        {
            foreach (Employee employee in Employees)
            {
                if (employee.LoginName == loginName)
                {
                    return employee.FullName;
                }
            }

            return string.Empty;
        }
    }
}
