using Ccollab;
using EagleEye.Settings;
using Employees;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace EagleEye.Reviews
{
    public class Reviews : EagleEyeDataGeneratorDecorator, IReviewsCommands
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Reviews));

        /// <summary>
        /// Index of "Review Creation Date" column in reviews.csv file
        /// </summary>
        private const int indexReviewCreationDate = 2;

        /// <summary>
        /// Index of "Creator Login" column in reviews.csv file
        /// </summary>
        private const int indexCreatorLogin = 9;

        /// <summary>
        /// Holds filtered raw reviews data of local employees.
        /// </summary>
        private List<string[]> filteredEmployeesReviewsData = null;

        public Reviews(ICcollabDataSource ccollabDataGenerator) : base(ccollabDataGenerator) { }

        /// <summary>
        /// Filter raw reviews data of local employees.
        /// </summary>
        public List<string[]> FilteredEmployeesReviewsData
        {
            get
            {
                if (filteredEmployeesReviewsData == null && EmployeesReader.Employees != null)
                {
                    IEnumerable<string[]> reviewsQuery =
                        from row in GetReviewsRawData()
                        where EmployeesReader.Employees.Any(employee => employee.LoginName == row[indexCreatorLogin])
                        select row;

                    filteredEmployeesReviewsData = reviewsQuery.ToList<string[]>();
                }

                return filteredEmployeesReviewsData;
            }
        }

        /// <summary>
        /// Generate review count by month.
        /// </summary>
        public void GenerateReviewCountByMonth()
        {
            log.Info("Generating: Review Count By Month ...");

            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Month", "Count"],
            //     ["2016-01", 20],
            //     ["2016-02", 30]
            //   ]
            // }

            // `Review Creation Date` column data format is "2016-09-30 23:33 UTC"
            var query =
                from row in FilteredEmployeesReviewsData
                group row by row[indexReviewCreationDate].Substring(0, 7) into month
                orderby month.Key ascending
                select new { Month = month.Key, Count = month.Count() };

            List<List<object>> datatable = new List<List<object>>();

            // data table header
            List<object> header = new List<object> { "Month", "Count" };
            datatable.Add(header);

            foreach (var item in query)
            {
                datatable.Add(new List<object> { item.Month, item.Count });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));
            Console.WriteLine(json);

            Save2EagleEye("ReviewCountByMonth", json);

            log.Info("Generating: Review Count By Month ... Done.");
        }

        /// <summary>
        /// Generate review count by product.
        /// </summary>
        public void GenerateReviewCountByProduct()
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Product", "Count"],
            //     ["Team1", 20],
            //     ["Team2", 16]
            //   ]
            // }

            log.Info("Generating: Review Count By Product ...");
            
            Dictionary<string, int> product2count = new Dictionary<string, int>();

            // collect all products
            foreach (Employee employee in EmployeesReader.Employees)
            {
                if (!product2count.ContainsKey(employee.ProductName))
                {
                    product2count.Add(employee.ProductName, 0);
                }
            }

            var query =
                from row in FilteredEmployeesReviewsData
                let productName = EmployeesReader.GetEmployeeProductName(row[indexCreatorLogin])
                group row by productName into productGroup
                select new { ProductName = productGroup.Key, DefectCount = productGroup.Count() };

            foreach (var item in query)
            {
                if (product2count.ContainsKey(item.ProductName))
                {
                    product2count[item.ProductName] = item.DefectCount;
                }
            }

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Product", "Count" };
            datatable.Add(header);

            foreach (KeyValuePair<string, int> item in product2count)
            {
                datatable.Add(new List<object> { item.Key, item.Value });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));
            Console.WriteLine(json);

            Save2EagleEye("ReviewCountByProduct", json);

            log.Info("Generating: Review Count By Product ... Done.");
        }

        /// <summary>
        /// Generate review count of employees inside a specific product team.
        /// </summary>
        public void GenerateReviewCountByEmployeeOfProduct()
        {
            foreach (var item in EagleEyeSettingsReader.Settings.ReviewCountByEmployeeOfProduct)
            {
                ReviewCountByEmployeeOfProduct(item.ProductName, item.ChartSettingsKey);
            }
        }

        /// <summary>
        /// Generate review count submitted by employees belongs to the given product.
        /// </summary>
        /// <param name="productName">Product name.</param>
        /// <param name="settingsKey">EagleEye settings key name.</param>
        private void ReviewCountByEmployeeOfProduct(string productName, string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["EmployeeName", "ReviewCount"],
            //     ["Patrick Zhong", 16],
            //     ["Merlin Mo", 16]
            //   ]
            // }

            log.Info("Generating: Review Count For " + productName + " ...");

            Dictionary<string, int> employee2count = new Dictionary<string, int>();

            // collect all employees of the product
            foreach (Employee employee in EmployeesReader.GetEmployeesByProduct(productName))
            {
                employee2count.Add(employee.LoginName, 0);
            }

            var query =
                from row in FilteredEmployeesReviewsData
                let _productName = EmployeesReader.GetEmployeeProductName(row[indexCreatorLogin])
                where _productName == productName
                group row by row[indexCreatorLogin] into employeeReviewsGroup
                select new { LoginName = employeeReviewsGroup.Key, ReviewCount = employeeReviewsGroup.Count() };

            foreach (var item in query)
            {
                if (employee2count.ContainsKey(item.LoginName))
                {
                    employee2count[item.LoginName] = item.ReviewCount;
                }
            }

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "EmployeeName", "ReviewCount" };
            datatable.Add(header);

            foreach (KeyValuePair<string, int> item in employee2count)
            {
                datatable.Add(new List<object> { EmployeesReader.GetEmployeeFullNameByLoginName(item.Key), item.Value });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));
            Console.WriteLine(json);

            Save2EagleEye(settingsKey, json);

            log.Info("Generating: Review Count For " + productName + " ... Done.");
        }
    }
}
