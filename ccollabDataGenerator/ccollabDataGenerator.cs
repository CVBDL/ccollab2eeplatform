using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eeDataGenerator;

namespace ccollabDataGenerator
{
    public class ccollabDataGenerator : IeeDataGenerator
    {
        protected List<string> _rawData;

        public List<string> RawData
        {
            get
            {
                return _rawData;
            }
        }

        public bool Execute(List<string> rawData)
        {
            _rawData = rawData;

            return true;
        }

    }
}
