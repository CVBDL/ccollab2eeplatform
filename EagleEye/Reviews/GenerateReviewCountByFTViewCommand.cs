using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Reviews
{
    public class GenerateReviewCountByFTViewCommand : ICommand
    {
        private Reviews _reviewsDataGenerator;

        public GenerateReviewCountByFTViewCommand(Reviews reviewsDataGenerator)
        {
            _reviewsDataGenerator = reviewsDataGenerator;
        }

        public void Execute()
        {
            _reviewsDataGenerator.GenerateReviewCountForProduct("FTView", "ReviewCountForFTView");
        }
    }
}
