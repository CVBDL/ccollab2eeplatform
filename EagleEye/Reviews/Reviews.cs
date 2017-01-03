using Ccollab;
using EagleEye.Settings;
using Employees;
using log4net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace EagleEye.Reviews
{
    public class Reviews : EagleEyeDataGeneratorDecorator, IReviewsCommands
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Reviews));
        
        public Reviews(ICcollabDataSource ccollabDataGenerator) : base(ccollabDataGenerator) { }

        /// <summary>
        /// Holds filtered raw reviews data of local employees.
        /// </summary>
        private List<ReviewRecord> filteredEmployeesReviewsData = null;

        /// <summary>
        /// Filter raw reviews data of local employees.
        /// </summary>
        public List<ReviewRecord> FilteredEmployeesReviewsData
        {
            get
            {
                if (filteredEmployeesReviewsData == null)
                {
                    var query = GetReviewsRawData()
                                .Where(record => EmployeesReader.Employees.Any(employee => employee.LoginName == record.CreatorLogin))
                                .Select(record => record);
                    
                    filteredEmployeesReviewsData = query.ToList();
                }

                return filteredEmployeesReviewsData;
            }
        }

        private Dictionary<string, DensityStatistics> densityStatisticsByProduct = null;

        /// <summary>
        /// Provided density related statistics.
        /// </summary>
        private Dictionary<string, DensityStatistics> DensityStatisticsByProduct
        {
            get
            {
                if (densityStatisticsByProduct != null)
                {
                    return densityStatisticsByProduct;
                }

                densityStatisticsByProduct = new Dictionary<string, DensityStatistics>();
                
                var query = FilteredEmployeesReviewsData
                        .GroupBy(record => record.CreatorProductName)
                        .Select(grp => grp);

                foreach (var group in query)
                {
                    if (EagleEyeSettingsReader.Settings.Products.IndexOf(group.Key) < 0) continue;

                    long totalComments = 0;
                    long totalDefects = 0;
                    long totalLineOfCode = 0;
                    long totalLineOfCodeChanged = 0;

                    foreach (var record in group)
                    {
                        long commentCount = 0;
                        if (long.TryParse(record.CommentCount, out commentCount))
                        {
                            totalComments += commentCount;
                        }

                        long defectCount = 0;
                        if (long.TryParse(record.DefectCount, out defectCount))
                        {
                            totalDefects += defectCount;
                        }

                        long lineOfCode = 0;
                        if (long.TryParse(record.LOC, out lineOfCode))
                        {
                            totalLineOfCode += lineOfCode;
                        }

                        long lineOfCodeChanged = 0;
                        if (long.TryParse(record.LOCChanged, out lineOfCodeChanged))
                        {
                            totalLineOfCodeChanged += lineOfCodeChanged;
                        }
                    }

                    DensityStatistics stat = new DensityStatistics();
                    stat.TotalComments = totalComments;
                    stat.TotalDefects = totalDefects;
                    stat.LineOfCode = totalLineOfCode;
                    stat.LineOfCodeChanged = totalLineOfCodeChanged;

                    densityStatisticsByProduct.Add(group.Key, stat);
                }

                return densityStatisticsByProduct;
            }
        }

        /// <summary>
        /// Generate review count by month.
        /// </summary>
        public void GenerateReviewCountByMonth(string settingsKey)
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
            
            var query = FilteredEmployeesReviewsData
                    .GroupBy(record => record.ReviewCreationYear + "-" + record.ReviewCreationMonth)
                    .OrderBy(grp => grp.Key)
                    .Select(grp => new { ReviewCreationYYYYMM = grp.Key, Count = grp.Count() });

            List<List<object>> datatable = new List<List<object>>();

            // data table header
            List<object> header = new List<object> { "Month", "Count" };
            datatable.Add(header);

            foreach (var item in query)
            {
                datatable.Add(new List<object> { item.ReviewCreationYYYYMM, item.Count });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);

            log.Info("Generating: Review Count By Month ... Done.");
        }

        /// <summary>
        /// Generate review count by product.
        /// </summary>
        public void GenerateReviewCountByProduct(string settingsKey)
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
            
            foreach (var product in EagleEyeSettingsReader.Settings.Products)
            {
                product2count.Add(product, 0);
            }
            
            var query = FilteredEmployeesReviewsData
                    .GroupBy(record => record.CreatorProductName)
                    .Select(grp => new { ProductName = grp.Key, ReviewCount = grp.Count() });

            foreach (var item in query)
            {
                if (product2count.ContainsKey(item.ProductName))
                {
                    product2count[item.ProductName] = item.ReviewCount;
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

            Save2EagleEye(settingsKey, json);

            log.Info("Generating: Review Count By Product ... Done.");
        }

        /// <summary>
        /// Generate review count of employees inside a specific product team.
        /// </summary>
        public void GenerateReviewCountByEmployeeOfProduct()
        {
            foreach (var item in EagleEyeSettingsReader.Settings.ReviewCountByEmployeeOfProduct)
            {
                ReviewCountByEmployeeOfProduct(item.ChartSettingsKey, item.ProductName);
            }
        }

        /// <summary>
        /// Generate review count submitted by employees belongs to the given product.
        /// </summary>
        /// <param name="settingsKey">EagleEye settings key name.</param>
        /// <param name="productName">Product name.</param>
        private void ReviewCountByEmployeeOfProduct(string settingsKey, string productName)
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

            var query = FilteredEmployeesReviewsData
                    .Where(record => record.CreatorProductName == productName)
                    .GroupBy(record => record.CreatorLogin)
                    .Select(grp => new { LoginName = grp.Key, ReviewCount = grp.Count() });

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

            Save2EagleEye(settingsKey, json);

            log.Info("Generating: Review Count For " + productName + " ... Done.");
        }

        /// <summary>
        /// All.xlsx -> Summary -> Code Defect Density by Product -> Code Comment Density(Uploaded)
        /// </summary>
        /// <param name="settingsKey"></param>
        public void GenerateCodeCommentDensityUploaded(string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Product", "Code Comment Density(Uploaded)"],
            //     ["Team1", 0.1],
            //     ["Team2", 0.034]
            //   ]
            // }

            log.Info("Generating: Code Comment Density (Uploaded) ...");

            Dictionary<string, double> product2density = new Dictionary<string, double>();
            
            foreach (var product in EagleEyeSettingsReader.Settings.Products)
            {
                DensityStatistics stat = null;
                if (DensityStatisticsByProduct.TryGetValue(product, out stat))
                {
                    // Formula: TotalComments * 1000 / TotalLOC
                    double density = 0;
                    if (stat.LineOfCode != 0)
                    {
                        density = (stat.TotalComments * 1000) / stat.LineOfCode;
                    }

                    product2density.Add(product, density);
                }
            }

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Product", "Code Comment Density(Uploaded)" };
            datatable.Add(header);

            foreach (KeyValuePair<string, double> item in product2density)
            {
                datatable.Add(new List<object> { item.Key, item.Value });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);

            log.Info("Generating: Code Comment Density (Uploaded) ... Done");
        }

        /// <summary>
        /// All.xlsx -> Summary -> Code Defect Density by Product -> Code Comment Density(Changed)
        /// </summary>
        /// <param name="settingsKey"></param>
        public void GenerateCodeCommentDensityChanged(string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Product", "Code Comment Density(Changed)"],
            //     ["Team1", 0.1],
            //     ["Team2", 0.034]
            //   ]
            // }

            log.Info("Generating: Code Comment Density (Changed) ...");

            Dictionary<string, double> product2density = new Dictionary<string, double>();
            
            foreach (var product in EagleEyeSettingsReader.Settings.Products)
            {
                DensityStatistics stat = null;
                if (DensityStatisticsByProduct.TryGetValue(product, out stat))
                {
                    // Formula: TotalComments * 1000 / TotalLOCC
                    double density = 0;
                    if (stat.LineOfCodeChanged != 0)
                    {
                        density = (stat.TotalComments * 1000) / stat.LineOfCodeChanged;
                    }

                    product2density.Add(product, density);
                }
            }
            
            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Product", "Code Comment Density(Changed)" };
            datatable.Add(header);

            foreach (KeyValuePair<string, double> item in product2density)
            {
                datatable.Add(new List<object> { item.Key, item.Value });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);

            log.Info("Generating: Code Comment Density (Changed) ... Done");
        }

        /// <summary>
        /// All.xlsx -> Summary -> Code Defect Density by Product -> Code Defect Density(Uploaded)
        /// </summary>
        /// <param name="settingsKey"></param>
        public void GenerateCodeDefectDensityUploaded(string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Product", "Code Defect Density(Uploaded)"],
            //     ["Team1", 0.1],
            //     ["Team2", 0.034]
            //   ]
            // }

            log.Info("Generating: Code Defect Density (Uploaded) ...");

            Dictionary<string, double> product2density = new Dictionary<string, double>();
            
            foreach (var product in EagleEyeSettingsReader.Settings.Products)
            {
                DensityStatistics stat = null;
                if (DensityStatisticsByProduct.TryGetValue(product, out stat))
                {
                    // Formula: TotalDefects * 1000 / TotalLOC
                    double density = 0;
                    if (stat.LineOfCode != 0)
                    {
                        density = (stat.TotalDefects * 1000) / stat.LineOfCode;
                    }

                    product2density.Add(product, density);
                }
            }

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Product", "Code Defect Density(Uploaded)" };
            datatable.Add(header);

            foreach (KeyValuePair<string, double> item in product2density)
            {
                datatable.Add(new List<object> { item.Key, item.Value });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);

            log.Info("Generating: Code Defect Density (Uploaded) ... Done");
        }

        /// <summary>
        /// All.xlsx -> Summary -> Code Defect Density by Product -> Code Defect Density(Changed)
        /// </summary>
        /// <param name="settingsKey"></param>
        public void GenerateCodeDefectDensityChanged(string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Product", "Code Defect Density(Changed)"],
            //     ["Team1", 0.1],
            //     ["Team2", 0.034]
            //   ]
            // }

            log.Info("Generating: Code Defect Density (Changed) ...");

            Dictionary<string, double> product2density = new Dictionary<string, double>();
            
            foreach (var product in EagleEyeSettingsReader.Settings.Products)
            {
                DensityStatistics stat = null;
                if (DensityStatisticsByProduct.TryGetValue(product, out stat))
                {
                    // Formula: TotalDefects * 1000 / TotalLOCC
                    double density = 0;
                    if (stat.LineOfCodeChanged != 0)
                    {
                        density = (stat.TotalDefects * 1000) / stat.LineOfCodeChanged;
                    }

                    product2density.Add(product, density);
                }
            }

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Product", "Code Defect Density(Changed)" };
            datatable.Add(header);

            foreach (KeyValuePair<string, double> item in product2density)
            {
                datatable.Add(new List<object> { item.Key, item.Value });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);

            log.Info("Generating: Code Comment Density (Defect) ... Done");
        }
    }
}
