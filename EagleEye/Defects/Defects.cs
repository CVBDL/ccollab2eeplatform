using Ccollab;
using EagleEye.EEPlatformApi;
using EagleEye.GVizApi;
using EagleEye.Settings;
using Employees;
using log4net;
using System.Collections.Generic;
using System.Linq;

namespace EagleEye.Defects
{
    public class Defects : EagleEyeDataGeneratorDecorator<DefectRecord>, IDefectsCommands
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Defects));
        
        public Defects(ICcollabDataSource ccollabDataGenerator) : base(ccollabDataGenerator) { }

        private List<DefectRecord> validRecords = null;

        /// <summary>
        /// From the raw review records, only the ones which are created by employees in
        /// `employees.json` are valid.
        /// </summary>
        /// <returns></returns>
        public List<DefectRecord> GetValidRecords()
        {
            if (validRecords == null)
            {
                validRecords = GetValidRecords(GetDefectsRawData());
            }

            return validRecords;
        }

        /// <summary>
        /// Get all defect records for one/all product.
        /// </summary>
        /// <param name="productName">Product name, like: "ViewPoint".</param>
        /// <returns>List of defect records.</returns>
        public List<DefectRecord> GetRecordsByProduct(string productName)
        {
            return GetRecordsByProduct(GetValidRecords(), productName);
        }

        /// <summary>
        /// Expected data table format:
        /// {
        ///   "datatable": [
        ///     ["Product", "Count"],
        ///     ["Team1", 20],
        ///     ["Team2", 16]
        ///   ]
        /// }
        /// </summary>
        public void GenerateDefectCountByProduct()
        {
            log.Info("Generating: Defect count by product ...");

            DataTable dataTable = new DataTable();

            dataTable.AddColumn(new Column("string", "Product"));
            dataTable.AddColumn(new Column("number", "Count"));

            var query = EagleEyeSettingsReader.Settings.Products
                .GroupJoin(
                    GetValidRecords(),
                    product => product,
                    record => record.CreatorProductName,
                    (product, matching) => new { ProductName = product, DefectCount = matching.Count() }
                );

            foreach (var item in query)
            {
                Row row = new Row(item.ProductName, item.DefectCount);
                dataTable.AddRow(row);
            }

            EagleEyePlatformApi.EditDataTable(EagleEyeSettingsReader.Settings.DefectCountByProduct.ChartId, dataTable);
            
            log.Info("Generating: Defect count by product ... Done.");
        }

        /// <summary>
        /// Expected data table format:
        /// {
        ///   "datatable": [
        ///     ["Product", "Major", "Minor"],
        ///     ["Team1", 20, 3],
        ///     ["Team2", 16, 0]
        ///   ]
        /// }
        /// </summary>
        public void GenerateDefectCountOfSeverityByProduct()
        {
            log.Info("Generating: Defect severity count by product ...");

            DataTable dataTable = new DataTable();

            dataTable.AddColumn(new Column("string", "Product"));

            foreach (string severity in EagleEyeSettingsReader.Settings.DefectSeverityTypes)
            {
                dataTable.AddColumn(new Column("number", severity));
            }

            var query = EagleEyeSettingsReader.Settings.Products
                .GroupJoin(
                    GetValidRecords(),
                    product => product,
                    record => record.CreatorProductName,
                    (product, matching) => new { ProductName = product, Records = matching.ToList() }
                );

            foreach (var item in query)
            {
                Row row = new Row();
                row.AddCell(new Cell(item.ProductName));

                foreach (string severity in EagleEyeSettingsReader.Settings.DefectSeverityTypes)
                {
                    int count = item.Records.Count(record => record.Severity == severity);
                    row.AddCell(new Cell(count));
                }
                
                dataTable.AddRow(row);
            }

            EagleEyePlatformApi.EditDataTable(EagleEyeSettingsReader.Settings.DefectCountBySeverity.ChartId, dataTable);
            
            log.Info("Generating: Defect severity count by product ... Done.");
        }

        public void GenerateDefectCountOfSeverityByCreatorFromProduct()
        {
            foreach (var setting in EagleEyeSettingsReader.Settings.DefectSeverityCountByCreator)
            {
                List<DefectRecord> records = GetRecordsByProduct(setting.ProductName);
                List<Employee> creators = EmployeesReader.GetEmployeesByProduct(setting.ProductName);

                if (records != null && !string.IsNullOrWhiteSpace(setting.ChartId))
                {
                    log.Info("Generating: Defect severity count by injection stage from " + (setting.ProductName == "*" ? "all products" : setting.ProductName) + " ...");
                    
                    DataTable dataTable = DefectCountOfSeverityByCreator(records, creators);
                    EagleEyePlatformApi.EditDataTable(setting.ChartId, dataTable);

                    log.Info("Generating: Defect severity count by injection stage from " + (setting.ProductName == "*" ? "all products" : setting.ProductName) + " ... Done");
                }
            }
        }

        /// <summary>
        /// Expected data table format:
        /// {
        ///   "datatable": [
        ///     ["Creator", "Major", "Minor"],
        ///     ["Patrick", 20, 3],
        ///     ["Lily", 16, 0]
        ///   ]
        /// }
        /// </summary>
        /// <param name="source"></param>
        /// <param name="creators"></param>
        /// <returns></returns>
        private DataTable DefectCountOfSeverityByCreator(List<DefectRecord> source, List<Employee> creators)
        {
            DataTable dataTable = new DataTable();

            dataTable.AddColumn(new Column("string", "Creator"));

            foreach (string severity in EagleEyeSettingsReader.Settings.DefectSeverityTypes)
            {
                dataTable.AddColumn(new Column("number", severity));
            }

            var query = creators
                .GroupJoin(
                    source,
                    employee => employee.LoginName,
                    record => record.CreatorLogin,
                    (employee, matching) => new { Creator = employee.FullName, Records = matching.ToList() }
                );

            foreach (var item in query)
            {
                Row row = new Row();
                row.AddCell(new Cell(item.Creator));

                foreach (string severity in EagleEyeSettingsReader.Settings.DefectSeverityTypes)
                {
                    int count = item.Records.Count(record => record.Severity == severity);
                    row.AddCell(new Cell(count));
                }

                dataTable.AddRow(row);
            }

            return dataTable;
        }

        public void GenerateDefectCountByInjectionStageFromProduct()
        {
            foreach (var setting in EagleEyeSettingsReader.Settings.DefectCountByInjectionStage)
            {
                List<DefectRecord> records = GetRecordsByProduct(setting.ProductName);

                if (records != null && !string.IsNullOrWhiteSpace(setting.ChartId))
                {
                    log.Info("Generating: Defect count by injection stage from " + (setting.ProductName == "*" ? "all products" : setting.ProductName) + " ...");
                    
                    DataTable dataTable = DefectCountByInjectionStage(records);
                    EagleEyePlatformApi.EditDataTable(setting.ChartId, dataTable);

                    log.Info("Generating: Defect count by injection stage from " + (setting.ProductName == "*" ? "all products" : setting.ProductName) + " ... Done");
                }
            }
        }

        /// <summary>
        /// Expected data table format:
        /// {
        ///   "datatable": [
        ///     ["InjectionStage", "Count"],
        ///     ["code/unit test", 20],
        ///     ["design", 16],
        ///     ["requirements", 16],
        ///     ["integration/test", 16]
        ///   ]
        /// }
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private DataTable DefectCountByInjectionStage(List<DefectRecord> source)
        {
            DataTable dataTable = new DataTable();

            dataTable.AddColumn(new Column("string", "InjectionStage"));
            dataTable.AddColumn(new Column("number", "Count"));

            var query = EagleEyeSettingsReader.Settings.DefectInjectionStage
                .GroupJoin(
                    source,
                    injectionStage => injectionStage,
                    record => record.InjectionStage,
                    (injectionStage, matching) => new { InjectionStage = injectionStage, Count = matching.Count() }
                );
            
            foreach (var item in query)
            {
                Row row = new Row(item.InjectionStage, item.Count);
                dataTable.AddRow(row);
            }

            return dataTable;
        }

        public void GenerateDefectCountByTypeFromProduct()
        {
            foreach (var setting in EagleEyeSettingsReader.Settings.DefectCountByType)
            {
                List<DefectRecord> records = GetRecordsByProduct(setting.ProductName);

                if (records != null && !string.IsNullOrWhiteSpace(setting.ChartId))
                {
                    log.Info("Generating: Defect count by type from " + (setting.ProductName == "*" ? "all products" : setting.ProductName) + " ...");
                    
                    DataTable dataTable = DefectCountByType(records);
                    EagleEyePlatformApi.EditDataTable(setting.ChartId, dataTable);

                    log.Info("Generating: Defect count by type from " + (setting.ProductName == "*" ? "all products" : setting.ProductName) + " ... Done");
                }
            }
        }

        /// <summary>
        /// Expected data table format:
        /// {
        ///   "datatable": [
        ///     ["Type", "Count"],
        ///     ["algorithm/logic", 20],
        ///     ["build", 16],
        ///     ...
        ///   ]
        /// }
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private DataTable DefectCountByType(List<DefectRecord> source)
        {
            DataTable dataTable = new DataTable();

            dataTable.AddColumn(new Column("string", "Type"));
            dataTable.AddColumn(new Column("number", "Count"));

            var query = EagleEyeSettingsReader.Settings.DefectTypes
                .GroupJoin(
                    source,
                    type => type,
                    record => record.Type,
                    (type, matching) => new { Type = type, Count = matching.Count() }
                );

            foreach (var item in query)
            {
                Row row = new Row(item.Type, item.Count);
                dataTable.AddRow(row);
            }

            return dataTable;
        }

        public void GenerateDefectCountOfTypeByCreatorFromProduct()
        {
            foreach (var setting in EagleEyeSettingsReader.Settings.DefectCountOfTypeByCreator)
            {
                List<DefectRecord> records = GetRecordsByProduct(setting.ProductName);
                List<Employee> creators = EmployeesReader.GetEmployeesByProduct(setting.ProductName);

                if (records != null && !string.IsNullOrWhiteSpace(setting.ChartId))
                {
                    log.Info("Generating: Defect count of type by creator from " + (setting.ProductName == "*" ? "all products" : setting.ProductName) + " ...");
                    
                    DataTable dataTable = DefectCountOfTypeByCreator(records, creators);
                    EagleEyePlatformApi.EditDataTable(setting.ChartId, dataTable);

                    log.Info("Generating: Defect count of type by creator from " + (setting.ProductName == "*" ? "all products" : setting.ProductName) + " ... Done");
                }
            }
        }

        /// <summary>
        /// Expected data table format:
        /// {
        ///   "datatable": [
        ///     ["Creator", "algorithm/logic", "build", ..., "testing"],
        ///     ["Patrick Zhong", 3, 1, ..., 0]
        ///   ]
        /// }
        /// </summary>
        /// <param name="source"></param>
        /// <param name="creators"></param>
        /// <returns></returns>
        private DataTable DefectCountOfTypeByCreator(List<DefectRecord> source, List<Employee> creators)
        {
            DataTable dataTable = new DataTable();

            dataTable.AddColumn(new Column("string", "Creator"));

            foreach (string type in EagleEyeSettingsReader.Settings.DefectTypes)
            {
                dataTable.AddColumn(new Column("number", type));
            }

            var query = creators
                .GroupJoin(
                    source,
                    employee => employee.LoginName,
                    record => record.CreatorLogin,
                    (employee, matching) => new { Creator = employee.FullName, Records = matching.ToList() }
                );

            foreach (var item in query)
            {
                Row row = new Row();
                row.AddCell(new Cell(item.Creator));

                foreach (string type in EagleEyeSettingsReader.Settings.DefectTypes)
                {
                    int count = item.Records.Count(record => record.Type == type);
                    row.AddCell(new Cell(count));
                }

                dataTable.AddRow(row);
            }

            return dataTable;
        }

        /// <summary>
        /// Expected data table format:
        /// {
        ///   "datatable": [
        ///     ["Product", "algorithm/logic", "build", ..., "testing"],
        ///     ["ViewPoint", 1, 3, ..., 0],
        ///     ...
        ///   ]
        /// }
        /// </summary>
        public void GenerateDefectCountOfTypeByProduct()
        {
            log.Info("Generating: Defect count of type by product ...");

            DataTable dataTable = new DataTable();

            dataTable.AddColumn(new Column("string", "Product"));

            foreach (string type in EagleEyeSettingsReader.Settings.DefectTypes)
            {
                dataTable.AddColumn(new Column("number", type));
            }

            var query = EagleEyeSettingsReader.Settings.Products
                .GroupJoin(
                    GetValidRecords(),
                    product => product,
                    record => record.CreatorProductName,
                    (product, matching) => new { ProductName = product, Records = matching.ToList() }
                );

            foreach (var item in query)
            {
                Row row = new Row();
                row.AddCell(new Cell(item.ProductName));

                foreach (string type in EagleEyeSettingsReader.Settings.DefectTypes)
                {
                    int count = item.Records.Count(record => record.Type == type);
                    row.AddCell(new Cell(count));
                }

                dataTable.AddRow(row);
            }
            
            EagleEyePlatformApi.EditDataTable(EagleEyeSettingsReader.Settings.DefectsDistributionByType.ChartId, dataTable);
            
            log.Info("Generating: Defect count of type by product ... Done.");
        }

        public void GenerateDefectCountByCreatorFromProduct()
        {
            foreach (var setting in EagleEyeSettingsReader.Settings.DefectCountByCreator)
            {
                List<DefectRecord> records = GetRecordsByProduct(setting.ProductName);
                List<Employee> creators = EmployeesReader.GetEmployeesByProduct(setting.ProductName);

                if (records != null && !string.IsNullOrWhiteSpace(setting.ChartId))
                {
                    log.Info("Generating: Defect count by creator from " + (setting.ProductName == "*" ? "all products" : setting.ProductName) + " ...");
                    
                    DataTable dataTable = DefectCountByCreator(records, creators);
                    EagleEyePlatformApi.EditDataTable(setting.ChartId, dataTable);

                    log.Info("Generating: Defect count by creator from " + (setting.ProductName == "*" ? "all products" : setting.ProductName) + " ... Done");
                }
            }
        }

        /// <summary>
        /// Expected data table format:
        /// {
        ///   "datatable": [
        ///     ["Creator", "Count"],
        ///     ["Patrick Zhong", 16],
        ///     ["Merlin Mo", 16]
        ///   ]
        /// }
        /// </summary>
        /// <param name="source"></param>
        /// <param name="creators"></param>
        /// <returns></returns>
        private DataTable DefectCountByCreator(List<DefectRecord> source, List<Employee> creators)
        {
            DataTable dataTable = new DataTable();

            dataTable.AddColumn(new Column("string", "Creator"));
            dataTable.AddColumn(new Column("number", "Count"));
            
            var query = creators
                .GroupJoin(
                    source,
                    employee => employee.LoginName,
                    record => record.CreatorLogin,
                    (employee, matching) => new { Creator = employee.FullName, Count = matching.Count() }
                );

            foreach (var item in query)
            {
                Row row = new Row(item.Creator, item.Count);
                dataTable.AddRow(row);
            }

            return dataTable;
        }
    }
}
