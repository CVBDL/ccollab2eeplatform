using EagleEye.Settings;
using Employees;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace EagleEye.Reviews
{
    public class ReviewsDataGenerator
    {
        ICommand _generateReviewCountByMonthCommand;

        public ReviewsDataGenerator(ICommand generateReviewCountByMonthCommand)
        {
            _generateReviewCountByMonthCommand = generateReviewCountByMonthCommand;
        }

        public void GenerateReviewCountByMonth()
        {
            _generateReviewCountByMonthCommand.Execute();
        }
    }
}
