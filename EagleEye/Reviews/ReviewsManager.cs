namespace EagleEye.Reviews
{
    public class ReviewsManager
    {
        ICommand _cmdReviewCountByMonth;
        ICommand _cmdReviewCountByProduct;
        ICommand _cmdReviewCountByEmployeeOfProduct;
        ICommand _cmdCodeCommentDensityUploaded;
        ICommand _cmdCodeCommentDensityChanged;
        ICommand _cmdCodeDefectDensityUploaded;

        public ReviewsManager
        (
            ICommand cmdReviewCountByMonth,
            ICommand cmdReviewCountByProduct,
            ICommand cmdReviewCountByEmployeeOfProduct,
            ICommand cmdCodeCommentDensityUploaded,
            ICommand cmdCodeCommentDensityChanged,
            ICommand cmdCodeDefectDensityUploaded
        )
        {
            _cmdReviewCountByMonth = cmdReviewCountByMonth;
            _cmdReviewCountByProduct = cmdReviewCountByProduct;
            _cmdReviewCountByEmployeeOfProduct = cmdReviewCountByEmployeeOfProduct;
            _cmdCodeCommentDensityUploaded = cmdCodeCommentDensityUploaded;
            _cmdCodeCommentDensityChanged = cmdCodeCommentDensityChanged;
            _cmdCodeDefectDensityUploaded = cmdCodeDefectDensityUploaded;
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

        public void GenerateCodeCommentDensityChanged()
        {
            _cmdCodeCommentDensityChanged.Execute();
        }

        public void GenerateCodeDefectDensityUploaded()
        {
            _cmdCodeDefectDensityUploaded.Execute();
        }
    }
}
