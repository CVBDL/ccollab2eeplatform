using Ccollab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Defects
{
    public class DefectStatistics
    {
        private List<DefectRecord> records = null;

        public DefectStatistics(List<DefectRecord> records)
        {
            this.records = records;
        }

        /// <summary>
        /// Reviews count
        /// </summary>
        public int Count
        {
            get
            {
                return records.Count;
            }
        }
    }
}
