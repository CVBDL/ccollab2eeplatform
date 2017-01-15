using Ccollab;
using EagleEye.Settings;
using Employees;
using log4net;
using System.Collections.Generic;
using System.Linq;

namespace EagleEye
{
    public abstract class EagleEyeDataGeneratorDecorator<T> : ICcollabDataSource where T : ICcollabRecord
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EagleEyeDataGeneratorDecorator<T>));

        private ICcollabDataSource _ccollabDataGenerator;

        public EagleEyeDataGeneratorDecorator(ICcollabDataSource ccollabDataGenerator)
        {
            _ccollabDataGenerator = ccollabDataGenerator;
        }
        
        public List<ReviewRecord> GetReviewsRawData()
        {
            return _ccollabDataGenerator.GetReviewsRawData();
        }

        public List<DefectRecord> GetDefectsRawData()
        {
            return _ccollabDataGenerator.GetDefectsRawData();
        }

        protected List<T> GetValidRecords(List<T> source)
        {
            return source
                .Where(record => EmployeesReader.Employees.Any(employee => employee.LoginName == record.CreatorLogin))
                .Select(record => record)
                .ToList();
        }

        protected List<T> GetRecordsByProduct(List<T> source, string productName)
        {
            List<T> records = null;

            // for all products
            if (productName == "*")
            {
                records = source;
            }
            else if (EagleEyeSettingsReader.Settings.Products.IndexOf(productName) != -1)
            {
                records = source
                    .Where(record => record.CreatorProductName == productName)
                    .ToList();
            }

            return records;
        }
    }
}
