using Employees;
using System;
using System.Collections.Generic;

namespace Ccollab
{
    public class ReviewRecord : ICcollabRecord
    {
        private string[] row = null;

        private string creatorProductName = string.Empty;
        private string reviewCreationYear = string.Empty;
        private string reviewCreationMonth = string.Empty;
        private string reviewCreationDay = string.Empty;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="row">Parsed CSV row, split columns into string array.</param>
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
        /// Review comment count, e.g., "15".
        /// </summary>
        public string CommentCount
        {
            get
            {
                return row[ReviewCsvColumnIndex.CommentCount];
            }
        }

        /// <summary>
        /// Review defect count, e.g., "6".
        /// </summary>
        public string DefectCount
        {
            get
            {
                return row[ReviewCsvColumnIndex.DefectCount];
            }
        }

        /// <summary>
        /// Review total lines of code, e.g., "15061".
        /// </summary>
        public string LOC
        {
            get
            {
                return row[ReviewCsvColumnIndex.LOC];
            }
        }

        /// <summary>
        /// Review total lines of code changed, e.g., "28".
        /// </summary>
        public string LOCChanged
        {
            get
            {
                return row[ReviewCsvColumnIndex.LOCChanged];
            }
        }
    }
}
