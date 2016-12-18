using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Reviews
{
    public class GenerateReviewCountByViewPointCommand : ICommand
    {
        private Reviews _reviewsDataGenerator;

        public GenerateReviewCountByViewPointCommand(Reviews reviewsDataGenerator)
        {
            _reviewsDataGenerator = reviewsDataGenerator;
        }

        public void Execute()
        {
            _reviewsDataGenerator.GenerateReviewCountForProduct("ViewPoint", "ReviewCountForViewPoint");
        }
    }
}
