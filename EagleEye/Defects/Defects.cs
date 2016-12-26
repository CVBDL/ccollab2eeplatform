using Ccollab;
using EagleEye.Settings;
using Employees;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EagleEye.Defects
{
    public class Defects : EagleEyeDataGeneratorDecorator, IDefectsCommands
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Defects));

        /// <summary>
        /// Index of "Creator Login" column in defects.csv file
        /// </summary>
        private const int indexCreatorLogin = 8;

        /// <summary>
        /// Index of "Type_CVB" column in defects.csv file
        /// </summary>
        private const int indexType = 22;

        /// <summary>
        /// Index of "Injection Stage" column in defects.csv file
        /// </summary>
        private const int indexInjectionStage = 23;

        /// <summary>
        /// Holds filtered raw defects data of local employees.
        /// </summary>
        private List<string[]> filteredEmployeesDefectsData = null;

        public Defects(ICcollabDataSource ccollabDataGenerator) : base(ccollabDataGenerator) { }

        /// <summary>
        /// Filter raw defects data of local employees.
        /// </summary>
        public List<string[]> FilteredEmployeesDefectsData
        {
            get
            {
                if (filteredEmployeesDefectsData == null)
                {
                    IEnumerable<string[]> defectsQuery =
                        from row in GetDefectsRawData()
                        where EmployeesReader.Employees.Any(employee => employee.LoginName == row[indexCreatorLogin])
                        select row;

                    filteredEmployeesDefectsData = defectsQuery.ToList<string[]>();
                }

                return filteredEmployeesDefectsData;
            }
        }

        public void GenerateDefectCountByProduct()
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Product", "Count"],
            //     ["Team1", 20],
            //     ["Team2", 16]
            //   ]
            // }

            log.Info("Generating: Defect Count By Product ...");
            
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
                from row in FilteredEmployeesDefectsData
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

            Save2EagleEye("DefectCountByProduct", json);

            log.Info("Generating: Defect Count By Product ... Done.");
        }

        public void GenerateDefectCountBySeverity()
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Product", "Major", "Minor"],
            //     ["Team1", 20, 3],
            //     ["Team2", 16, 0]
            //   ]
            // }

            log.Info("Generating: Defect Severity Count By Product ...");

            EagleEyeSettings settings = EagleEyeSettingsReader.Settings;

            Dictionary<string, List<int>> product2severitycount = new Dictionary<string, List<int>>();

            // collect all products
            foreach (Employee employee in EmployeesReader.Employees)
            {
                if (!product2severitycount.ContainsKey(employee.ProductName))
                {
                    product2severitycount.Add(employee.ProductName, new List<int>(new int[settings.DefectSeverityTypes.Count]));
                }
            }
            var query =
                from row in FilteredEmployeesDefectsData
                let productName = EmployeesReader.GetEmployeeProductName(row[indexCreatorLogin])
                group row by productName into productGroup
                select productGroup;

            int defectSeverityIndex = 20;

            foreach (var item in query)
            {
                if (!product2severitycount.ContainsKey(item.Key)) continue;

                foreach (var defect in item)
                {
                    int index = settings.DefectSeverityTypes.IndexOf(defect[defectSeverityIndex]);

                    if (index >= 0)
                    {
                        product2severitycount[item.Key][index] += 1;
                    }
                }
            }

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Product" }.Concat(settings.DefectSeverityTypes).ToList();
            datatable.Add(header);

            foreach (KeyValuePair<string, List<int>> item in product2severitycount)
            {
                List<object> row = new List<object> { item.Key };

                foreach (int count in item.Value)
                {
                    row.Add(count);
                }

                datatable.Add(row);
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye("DefectCountBySeverity", json);

            log.Info("Generating: Defect Severity Count By Product ... Done.");
        }

        public void GenerateDefectCountByInjectionStage()
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["InjectionStage", "Count"],
            //     ["code/unit test", 20],
            //     ["design", 16],
            //     ["requirements", 16]
            //     ["integration/test", 16]
            //   ]
            // }

            log.Info("Generating: Defect Count By Injection Stage ...");

            Dictionary<string, int> injectionstage2count = new Dictionary<string, int>();

            // collect all products
            foreach (var injectionStage in EagleEyeSettingsReader.Settings.DefectInjectionStage)
            {
                injectionstage2count.Add(injectionStage.ToLower(), 0);
            }

            var query =
                from row in FilteredEmployeesDefectsData
                group row by row[indexInjectionStage] into injectionStageGroup
                select new { InjectionStage = injectionStageGroup.Key.ToLower(), Count = injectionStageGroup.Count() };

            foreach (var item in query)
            {
                if (injectionstage2count.ContainsKey(item.InjectionStage))
                {
                    injectionstage2count[item.InjectionStage] = item.Count;
                }
            }

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "InjectionStage", "Count" };
            datatable.Add(header);

            foreach (KeyValuePair<string, int> item in injectionstage2count)
            {
                datatable.Add(new List<object> { item.Key, item.Value });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye("DefectCountByInjectionStage", json);

            log.Info("Generating: Defect Count By Injection Stage ... Done.");
        }

        public void GenerateDefectCountByType()
        {
            // Expected data table format:
            // {
            //    "datatable": [
            //     ["Type", "Count"],
            //     ["algorithm/logic", 20],
            //     ["build", 16],
            //     ...
            //   ]
            // }

            log.Info("Generating: Defect Count By Type ...");

            Dictionary<string, int> type2count = new Dictionary<string, int>();

            // collect all products
            foreach (var type in EagleEyeSettingsReader.Settings.DefectTypes)
            {
                type2count.Add(type.ToLower(), 0);
            }

            var query =
                from row in FilteredEmployeesDefectsData
                group row by row[indexType] into typeGroup
                select new { Type = typeGroup.Key.ToLower(), Count = typeGroup.Count() };

            foreach (var item in query)
            {
                if (type2count.ContainsKey(item.Type))
                {
                    type2count[item.Type] = item.Count;
                }
            }

            List<List<object>> datatable = new List<List<object>>();

            List<object> header = new List<object> { "Type", "Count" };
            datatable.Add(header);

            foreach (KeyValuePair<string, int> item in type2count)
            {
                datatable.Add(new List<object> { item.Key, item.Value });
            }

            string json = JsonConvert.SerializeObject(new Chart(datatable));

            Save2EagleEye("DefectCountByType", json);

            log.Info("Generating: Defect Count By Type ... Done.");
        }
    }
}
