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

        private ICcollabDataSource ccollabDataSource;

        public EagleEyeDataGeneratorDecorator(ICcollabDataSource ccollabDataGenerator)
        {
            ccollabDataSource = ccollabDataGenerator;
        }
        
        public List<ReviewRecord> GetReviewsRawData()
        {
            return ccollabDataSource.GetReviewsRawData();
        }

        public List<DefectRecord> GetDefectsRawData()
        {
            return ccollabDataSource.GetDefectsRawData();
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
            List<T> records = new List<T>();

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
