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
        protected List<string> _reviewsRawData;
        protected List<string> _defectsRawData;

        public List<string> ReviewsRawData
        {
            get
            {
                return _reviewsRawData;
            }
        }

        public List<string> DefectsRawData
        {
            get
            {
                return _defectsRawData;
            }
        }

        public bool Execute(List<string> rawData)
        {
            var reviewsFileName = rawData[0];
            var defectsFileName = rawData[1];

            IEnumerable<string> reviewsQuery =
                from line in File.ReadAllLines(reviewsFileName)
                let fields = line.Split(',')
                where fields[9] == "pzhong"
                select line;

            _reviewsRawData = reviewsQuery.ToList<string>();

            return true;
        }

    }
}
