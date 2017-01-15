using Employees;
using System;

namespace Ccollab
{
    public class ReviewRecord : ICcollabRecord
    {
        private string[] row = null;

        private string creatorProductName = string.Empty;
        private string reviewCreationYear = string.Empty;
        private string reviewCreationMonth = string.Empty;
        private string reviewCreationDay = string.Empty;
        
        public ReviewRecord(string[] row)
        {
            this.row = row;
        }

        /// <summary>
        /// Creator login name, e.g., "pzhong".
        /// </summary>
        public string CreatorLogin
        {
            get
            {
                return row[ReviewCsvColumnIndex.CreatorLogin];
            }
        }

        /// <summary>
        /// Creator full name, e.g., "Patrick Zhong".
        /// Note: it's not used for display in chart, we're using employee full name from employees.json
        /// </summary>
        public string CreatorFullName
        {
            get
            {
                return row[ReviewCsvColumnIndex.CreatorFullName];
            }
        }

        /// <summary>
        /// Creator product name, e.g., "ViewPoint".
        /// </summary>
        public string CreatorProductName
        {
            get
            {
                if (string.IsNullOrEmpty(creatorProductName))
                {
                    creatorProductName = EmployeesReader.GetEmployeeProductName(CreatorLogin);
                }

                return creatorProductName;
            }
        }

        /// <summary>
        /// review creation date, e.g., "2016-09-30 23:33 UTC".
        /// </summary>
        public string ReviewCreationDate
        {
            get
            {
                return row[ReviewCsvColumnIndex.ReviewCreationDate];
            }
        }
        
        /// <summary>
        /// Year of review creation date, e.g., "2016".
        /// </summary>
        public string ReviewCreationYear
        {
            get
            {
                if (string.IsNullOrEmpty(reviewCreationYear))
                {
                    try
                    {
                        reviewCreationYear = ReviewCreationDate.Substring(0, 4);
                    }
                    catch (Exception)
                    {
                        reviewCreationYear = string.Empty;
                    }
                }

                return reviewCreationYear;
            }
        }
        
        /// <summary>
        /// Month of review creation date, e.g., "09".
        /// </summary>
        public string ReviewCreationMonth
        {
            get
            {
                if (string.IsNullOrEmpty(reviewCreationMonth))
                {
                    try
                    {
                        reviewCreationMonth = ReviewCreationDate.Substring(5, 2);
                    }
                    catch (Exception)
                    {
                        reviewCreationMonth = string.Empty;
                    }
                }

                return reviewCreationMonth;
            }
        }

        /// <summary>
        /// Day of review creation date, e.g., "31".
        /// </summary>
        public string ReviewCreationDay
        {
            get
            {
                if (string.IsNullOrEmpty(reviewCreationDay))
                {
                    try
                    {
                        reviewCreationDay = ReviewCreationDate.Substring(8, 2);
                    }
                    catch (Exception)
                    {
                        reviewCreationDay = string.Empty;
                    }
                }

                return reviewCreationDay;
            }
        }

        /// <summary>
        /// Review comment count, e.g., 15.
        /// </summary>
        public int CommentCount
        {
            get
            {
                int commentCount = 0;
                if (int.TryParse(row[ReviewCsvColumnIndex.CommentCount], out commentCount))
                {
                    return commentCount;
                }

                return 0;
            }
        }

        /// <summary>
        /// Review defect count, e.g., 6.
        /// </summary>
        public int DefectCount
        {
            get
            {
                int defectCount = 0;
                if (int.TryParse(row[ReviewCsvColumnIndex.DefectCount], out defectCount))
                {
                    return defectCount;
                }

                return 0;
            }
        }

        /// <summary>
        /// Review total lines of code, e.g., 15061.
        /// </summary>
        public int LOC
        {
            get
            {
                int lineOfCode = 0;
                if (int.TryParse(row[ReviewCsvColumnIndex.LOC], out lineOfCode))
                {
                    return lineOfCode;
                }

                return 0;
            }
        }

        /// <summary>
        /// Review total lines of code changed, e.g., 28.
        /// </summary>
        public int LOCChanged
        {
            get
            {
                int lineOfCodeChanged = 0;
                if (int.TryParse(row[ReviewCsvColumnIndex.LOCChanged], out lineOfCodeChanged))
                {
                    return lineOfCodeChanged;
                }

                return 0;
            }
        }

        /// <summary>
        /// Review total person time, e.g., "1:01:41".
        /// </summary>
        public double TotalPersonTimeInSecond
        {
            get
            {
                double seconds = 0;
                try
                {
                    seconds = TimeSpan.Parse(row[ReviewCsvColumnIndex.TotalPersonTime]).TotalSeconds;
                }
                catch (Exception)
                {
                    seconds = 0;
                }

                return seconds;
            }
        }
    }
}
