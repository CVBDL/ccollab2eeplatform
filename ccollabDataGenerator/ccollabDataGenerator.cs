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
            _reviewsRawData = ReadInCsvFile(rawData[0]);
            _defectsRawData = ReadInCsvFile(rawData[1]);

            return true;
        }

        private List<string[]> ReadInCsvFile(string fileName)
        {
            return File.ReadAllLines(fileName)
                .Skip(1)
                .Select(line => line.Split(','))
                .ToList<string[]>();
        }
    }
}
