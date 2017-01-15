using Employees;
using System;

namespace Ccollab
{
    public class DefectRecord : ICcollabRecord
    {
        private string[] row = null;

        private string creatorProductName = string.Empty;
        private string reviewCreationYear = string.Empty;
        private string reviewCreationMonth = string.Empty;
        private string reviewCreationDay = string.Empty;
        
        public DefectRecord(string[] row)
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
                return row[DefectCsvColumnIndex.CreatorLogin];
            }
        }

        /// <summary>
        /// Creator full name, e.g., "Patrick Zhong".
        /// </summary>
        public string CreatorFullName
        {
            get
            {
                return row[DefectCsvColumnIndex.CreatorFullName];
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
                return row[DefectCsvColumnIndex.ReviewCreationDate];
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
        /// Defect severity, e.g., "Minor".
        /// </summary>
        public string Severity
        {
            get
            {
                return row[DefectCsvColumnIndex.Severity];
            }
        }

        /// <summary>
        /// Defect type, e.g., "algorithm/logic".
        /// </summary>
        public string Type
        {
            get
            {
                return row[DefectCsvColumnIndex.Type];
            }
        }

        /// <summary>
        /// Defect injection stage, e.g., "Code/Unit Test".
        /// </summary>
        public string InjectionStage
        {
            get
            {
                return row[DefectCsvColumnIndex.InjectionStage];
            }
        }
    }
}
