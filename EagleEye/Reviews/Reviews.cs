using Ccollab;
using EagleEye.Settings;
using Employees;
using log4net;
using Newtonsoft.Json;
using System;
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
                            .Select(
                                group => new
                                {
                                    CreatorProductName = group.Key,
                                    TotalCommentCount  = group.Sum(record => record.CommentCount),
                                    TotalDefectCount   = group.Sum(record => record.DefectCount),
                                    TotalLOC           = group.Sum(record => record.LOC),
                                    TotalLOCChanged    = group.Sum(record => record.LOCChanged)
                                }
                            );

                foreach (var item in query)
                {
                    if (EagleEyeSettingsReader.Settings.Products.IndexOf(item.CreatorProductName) < 0) continue;
                    
                    DensityStatistics stat = new DensityStatistics();
                    stat.TotalCommentCount = item.TotalCommentCount;
                    stat.TotalDefectCount  = item.TotalDefectCount;
                    stat.TotalLOC          = item.TotalLOC;
                    stat.TotalLOCChanged   = item.TotalLOCChanged;

                    densityStatisticsByProduct.Add(item.CreatorProductName, stat);
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
                    .OrderBy(group => group.Key)
                    .Select(group => new { ReviewCreationYYYYMM = group.Key, Count = group.Count() });

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
                    .Select(group => new { ProductName = group.Key, ReviewCount = group.Count() });

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
        /// Generate review count by creator.
        /// Including several charts:
        ///     1. Review count by creator of ViewPoint.
        ///     2. Review count by creator of FTView.
        ///     3. (All other products)...
        /// </summary>
        public void GenerateReviewCountByCreator()
        {
            foreach (var item in EagleEyeSettingsReader.Settings.ReviewCountByCreator)
            {
                List<ReviewRecord> datasource = null;

                // for all products
                if (item.ProductName == "*")
                {
                    datasource = FilteredEmployeesReviewsData;
                }
                else if (EagleEyeSettingsReader.Settings.Products.IndexOf(item.ProductName) != -1)
                {
                    datasource = FilteredEmployeesReviewsData
                                .Where(record => record.CreatorProductName == item.ProductName)
                                .ToList();
                }

                if (datasource != null && !string.IsNullOrEmpty(item.ChartId))
                {
                    log.Info("Generating: Review Count By Creator For " + (item.ProductName == "*" ? "All Products" : item.ProductName) + " ...");

                    ReviewCountByCreator(datasource, item);

                    log.Info("Generating: Review Count By Creator For " + (item.ProductName == "*" ? "All Products" : item.ProductName) + " ... Done");
                }
            }
        }

        /// <summary>
        /// Generate review count by creator for a specific product
        /// </summary>
        /// <param name="datasource"></param>
        /// <param name="settingsKey"></param>
        private void ReviewCountByCreator(List<ReviewRecord> datasource, ProductChartSettings chartSettings)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Creator", "Count"],
            //     ["Patrick Zhong", 16],
            //     ["Merlin Mo", 16]
            //   ]
            // }

            Dictionary<string, int> creator2count = new Dictionary<string, int>();

            // collect all employees of the product
            foreach (Employee employee in EmployeesReader.GetEmployeesByProduct(chartSettings.ProductName))
            {
                creator2count.Add(employee.LoginName, 0);
            }

            var query = datasource
                        .GroupBy(record => record.CreatorLogin)
                        .Select(group => group);

            foreach (var item in query)
            {
                string creatorLoginName = item.Key;

                if (creator2count.ContainsKey(creatorLoginName))
                {
                    List<ReviewRecord> records = item.ToList();

                    ReviewsStatistics stat = new ReviewsStatistics(records);
                    int count = stat.Count;

                    creator2count[creatorLoginName] = count;
                }
            }

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Creator", "Count" };
            datatable.Add(header);

            foreach (KeyValuePair<string, int> item in creator2count)
            {
                datatable.Add(new List<object> { EmployeesReader.GetEmployeeFullNameByLoginName(item.Key), item.Value });
            }
            
            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(chartSettings.ChartId, json);
        }

        /// <summary>
        /// All.xlsx -> Summary -> Code Defect Density by Product -> Code Comment Density(Uploaded)
        /// </summary>
        /// <param name="settingsKey"></param>
        public void GenerateCommentDensityUploadedByProduct(string settingsKey)
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
                    // Formula: TotalCommentCount * 1000 / TotalLOC
                    double density = 0;
                    if (stat.TotalLOC != 0)
                    {
                        density = (stat.TotalCommentCount * 1000) / stat.TotalLOC;
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
        public void GenerateCommentDensityChangedByProduct(string settingsKey)
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
                    // Formula: TotalCommentCount * 1000 / TotalLOCC
                    double density = 0;
                    if (stat.TotalLOCChanged != 0)
                    {
                        density = (stat.TotalCommentCount * 1000) / stat.TotalLOCChanged;
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
        public void GenerateDefectDensityUploadedByProduct(string settingsKey)
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
                    // Formula: TotalDefectCount * 1000 / TotalLOC
                    double density = 0;
                    if (stat.TotalLOC != 0)
                    {
                        density = (stat.TotalDefectCount * 1000) / stat.TotalLOC;
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
        public void GenerateDefectDensityChangedByProduct(string settingsKey)
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
                    // Formula: TotalDefectCount * 1000 / TotalLOCC
                    double density = 0;
                    if (stat.TotalLOCChanged != 0)
                    {
                        density = (stat.TotalDefectCount * 1000) / stat.TotalLOCChanged;
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

        public void GenerateDefectDensityChangedByMonthFromProduct()
        {
            foreach (var item in EagleEyeSettingsReader.Settings.DefectDensityChangedByMonth)
            {
                List<ReviewRecord> datasource = null;

                // for all products
                if (item.ProductName == "*")
                {
                    datasource = FilteredEmployeesReviewsData;
                }
                else if (EagleEyeSettingsReader.Settings.Products.IndexOf(item.ProductName) != -1)
                {
                    datasource = FilteredEmployeesReviewsData
                                .Where(record => record.CreatorProductName == item.ProductName)
                                .ToList();
                }

                if (datasource != null && !string.IsNullOrEmpty(item.ChartId))
                {
                    log.Info("Generating: Code defect density (changed) for " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ...");

                    DefectDensityChangedByMonth(datasource, item.ChartId);

                    log.Info("Generating: Code defect density (changed) for " + (item.ProductName == "*" ? "all products" : item.ProductName) + " ... Done");
                }
            }
        }

        /// <summary>
        /// Formula:
        ///     Density = (TotalDefectCount * 1000) / TotalLOCChanged
        /// </summary>
        /// <param name="datasource"></param>
        /// <param name="settingsKey"></param>
        private void DefectDensityChangedByMonth(List<ReviewRecord> datasource, string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Month", "Defects/KLOCC"],
            //     ["2016-01", 20],
            //     ["2016-02", 30]
            //   ]
            // }
            
            var query = datasource
                        .GroupBy(record => record.ReviewCreationYear + "-" + record.ReviewCreationMonth)
                        .OrderBy(group => group.Key)
                        .Select(group => group);

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Month", "Defects/KLOCC" };
            datatable.Add(header);

            foreach (var item in query)
            {
                List<ReviewRecord> records = item.ToList();
                
                ReviewsStatistics stat = new ReviewsStatistics(records);
                var totalDefectCount = stat.TotalDefectCount;
                var totalLOCChanged = stat.TotalLOCChanged;

                double density = 0;
                if (totalLOCChanged != 0)
                {
                    density = totalDefectCount * 1000 / totalLOCChanged;
                }

                datatable.Add(new List<object> { item.Key, density });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);
        }

        /// <summary>
        /// Generate Inspection Rate By Month Chart.
        /// Including several charts:
        ///     1. Inspection rate by month for all products
        ///     2. Inspection rate by month for a specific product, like ViewPoint.
        /// </summary>
        public void GenerateInspectionRateByMonthFromProduct()
        {
            foreach (var item in EagleEyeSettingsReader.Settings.InspectionRateByMonth)
            {
                List<ReviewRecord> datasource = null;

                // for all products
                if (item.ProductName == "*")
                {
                    datasource = FilteredEmployeesReviewsData;
                }
                else if (EagleEyeSettingsReader.Settings.Products.IndexOf(item.ProductName) != -1)
                {
                    datasource = FilteredEmployeesReviewsData
                                .Where(record => record.CreatorProductName == item.ProductName)
                                .ToList();
                }

                if (datasource != null && !string.IsNullOrEmpty(item.ChartId))
                {
                    log.Info("Generating: Inspection Rate By Month For " + (item.ProductName == "*" ? "All Products" : item.ProductName) + " ...");

                    InspectionRateByMonth(datasource, item.ChartId);

                    log.Info("Generating: Inspection Rate By Month For " + (item.ProductName == "*" ? "All Products" : item.ProductName) + " ... Done");
                }
            }
        }

        /// <summary>
        /// Formula:
        ///     InspectionRate = (LOCC) / (TotalPersonTime * 1000)
        /// </summary>
        /// <param name="settingsKey"></param>
        private void InspectionRateByMonth(List<ReviewRecord> datasource, string settingsKey)
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Month", "Rate"],
            //     ["2016-01", 0.18],
            //     ["2016-02", 0.6]
            //   ]
            // }
            
            var query = datasource
                        .GroupBy(record => record.ReviewCreationYear + "-" + record.ReviewCreationMonth)
                        .OrderBy(group => group.Key)
                        .Select(group => group);
            
            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Month", "Rate" };
            datatable.Add(header);

            foreach (var item in query)
            {
                List<ReviewRecord> records = item.ToList();

                ReviewsStatistics stat = new ReviewsStatistics(records);
                double totalPersonTimeInHour = stat.TotalPersonTimeInSecond / (60 * 60);

                double inspectionRate = 0;
                if (totalPersonTimeInHour != 0)
                {
                    inspectionRate = stat.TotalLOCChanged / (totalPersonTimeInHour * 1000);
                }
                
                datatable.Add(new List<object> { item.Key, inspectionRate });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye(settingsKey, json);
        }
    }
}
