using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Reviews
{
    public interface IReviewsCommands
    {
        void GenerateReviewCountByMonth();
        void GenerateReviewCountByProduct();
        void GenerateReviewCountForProduct(string productName, string settingsKey);
    }
}
