using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using eeDataGenerator;

namespace ccollabDataGenerator
{
    public class ccollabDataGenerator : IeeDataGenerator
    {
        protected List<string[]> _reviewsRawData;
        protected List<string[]> _defectsRawData;

        public List<string[]> ReviewsRawData
        {
            get
            {
                return _reviewsRawData;
            }
        }

        public List<string[]> DefectsRawData
        {
            get
            {
                return _defectsRawData;
            }
        }

        public bool Execute(List<string> rawData)
        {
            _reviewsRawData = ReadInReviewsCsvFile(rawData[0]);

            // var reviewsCreatorLoginIndex = 9;
            // IEnumerable<string> reviewsQuery =
            //     from line in File.ReadAllLines(reviewsFileName)
            //     let fields = line.Split(',')
            //     where employees.Any(employee => employee.LoginName == fields[reviewsCreatorLoginIndex])
            //     select line;

            var defectsFileName = rawData[1];

            return true;
        }

        private List<string[]> ReadInReviewsCsvFile(string reviewsFileName)
        {
            return File.ReadAllLines(reviewsFileName)
                .Skip(1)
                .Select(line => line.Split(','))
                .ToList<string[]>();
        }
    }
}
