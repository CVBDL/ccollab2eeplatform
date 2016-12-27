namespace EagleEye.Reviews
{
    public class ReviewsManager
    {
        ICommand _cmdReviewCountByMonth;
        ICommand _cmdReviewCountByProduct;
        ICommand _cmdReviewCountByEmployeeOfProduct;
        ICommand _cmdCodeCommentDensityUploaded;

        public ReviewsManager
        (
            ICommand cmdReviewCountByMonth,
            ICommand cmdReviewCountByProduct,
            ICommand cmdReviewCountByEmployeeOfProduct,
            ICommand cmdCodeCommentDensityUploaded
        )
        {
            _cmdReviewCountByMonth = cmdReviewCountByMonth;
            _cmdReviewCountByProduct = cmdReviewCountByProduct;
            _cmdReviewCountByEmployeeOfProduct = cmdReviewCountByEmployeeOfProduct;
            _cmdCodeCommentDensityUploaded = cmdCodeCommentDensityUploaded;
    }

        public void GenerateReviewCountByMonth()
        {
            _cmdReviewCountByMonth.Execute();
        }

        public void GenerateReviewCountByProduct()
        {
            _cmdReviewCountByProduct.Execute();
        }

        public void GenerateReviewCountByEmployeeOfProduct()
        {
            _cmdReviewCountByEmployeeOfProduct.Execute();
        }

        public void GenerateCodeCommentDensityUploaded()
        {
            _cmdCodeCommentDensityUploaded.Execute();
        }
    }
}
