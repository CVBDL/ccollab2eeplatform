using Ccollab;
using System.Collections.Generic;
using System.Linq;

namespace EagleEye.Reviews
{
    public class ReviewsStatistics
    {
        private List<ReviewRecord>  records = null;

        public ReviewsStatistics(List<ReviewRecord> records)
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

        private int totalCommentCount = -1;

        /// <summary>
        /// Comment count.
        /// </summary>
        public int TotalCommentCount
        {
            get
            {
                if (totalCommentCount < 0)
                {
                    totalCommentCount = records.Sum(record => record.CommentCount);
                }

                return totalCommentCount;
            }
        }

        private int totalDefectCount = -1;

        /// <summary>
        /// Defect count.
        /// </summary>
        public int TotalDefectCount
        {
            get
            {
                if (totalDefectCount < 0)
                {
                    totalDefectCount = records.Sum(record => record.DefectCount);
                }

                return totalDefectCount;
            }
        }

        private int totalLOC = -1;

        /// <summary>
        /// "LOC" for short.
        /// </summary>
        public int TotalLOC
        {
            get
            {
                if (totalLOC < 0)
                {
                    totalLOC = records.Sum(record => record.LOC);
                }

                return totalLOC;
            }
        }

        private int totalLOCChanged = -1;

        /// <summary>
        /// "LOCC" for short.
        /// </summary>
        public int TotalLOCChanged
        {
            get
            {
                if (totalLOCChanged < 0)
                {
                    totalLOCChanged = records.Sum(record => record.LOCChanged);
                }

                return totalLOCChanged;
            }
        }

        private double totalPersonTimeInSecond = -1;

        /// <summary>
        /// Total Person-Time
        /// </summary>
        public double TotalPersonTimeInSecond
        {
            get
            {
                if (totalPersonTimeInSecond < 0)
                {
                    totalPersonTimeInSecond = records.Sum(record => record.TotalPersonTimeInSecond);
                }

                return totalPersonTimeInSecond;
            }
        }
    }
}
