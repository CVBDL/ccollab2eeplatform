using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using log4net;

namespace eeDataGenerator
{
    internal class Chart
    {
        public object[][] datatable { get; set; }

        public Chart(object[][] dt)
        {
            datatable = dt;
        }
    }

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

            var length = query.Count();

            List<object[]> datatable = new List<object[]>();

            datatable.Add(new object[] { "Month", "Count" });

            foreach (var date in query)
            {
                datatable.Add(new object[] { date.Month, date.Count });
                log.Info("Month: " + date.Month + ", Count: " + date.Count);
            }
            
            string json = JsonConvert.SerializeObject(new Chart(datatable.ToArray()));

            log.Info(json); // {"datatable":[["Month","Count"],["2016-08",1],["2016-09",2]]}

            // Expected format:
            // https://github.com/CVBDL/EagleEye-Docs/blob/master/rest-api/rest-api.md#edit-data-table
            // {
            //   "datatable": [
            //     ["Month", "Count"],
            //     ["2016-01", 20],
            //     ["2016-02", 30],
            //     ["2016-03", 25]
            //   ]
            // }

            log.Info("Generating: Review Count By Month ... Done.");
        }
    }
}
