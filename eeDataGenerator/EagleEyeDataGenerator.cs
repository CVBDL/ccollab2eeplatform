using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace eeDataGenerator
{
    public class EagleEyeDataGenerator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EagleEyeDataGenerator));

        private List<string[]> FilteredEmployeesReviewsData;
        private List<string[]> FilteredEmployeesDefectsData;
        private List<Employee> Employees;

        public bool Execute(IeeDataGenerator ccollab)
        {
            Employees = Employee.InitFromJson();

            FilteredEmployeesReviewsData = FilterEmployeesReviewData(ccollab.ReviewsRawData);

            GenerateReviewCountByMonth();

            return true;
        }

        private List<string[]> FilterEmployeesReviewData(List<string[]> ReviewsRawData)
        {
            var reviewsCreatorLoginIndex = 9;

            IEnumerable<string[]> reviewsQuery =
                from row in ReviewsRawData
                where Employees.Any(employee => employee.LoginName == row[reviewsCreatorLoginIndex])
                select row;

            return reviewsQuery.ToList<string[]>();
        }

        private void GenerateReviewCountByMonth()
        {
            log.Info("Generating: Review Count By Month ...");

            var query =
                from row in FilteredEmployeesReviewsData
                group row by row[2].Substring(0, 7) into month
                select new { Month = month.Key, Count = month.Count() };

            foreach (var date in query)
            {
                log.Info("Month: " + date.Month + ", Count: " + date.Count);
            }

            log.Info("Generating: Review Count By Month ... Done.");
        }
    }
}
