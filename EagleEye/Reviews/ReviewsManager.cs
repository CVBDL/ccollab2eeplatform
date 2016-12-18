namespace EagleEye.Reviews
{
    public class ReviewsManager
    {
        ICommand _cmdGenerateReviewCountByMonth;
        ICommand _cmdGenerateReviewCountByProduct;
        ICommand _cmdGenerateReviewCountByViewPoint;
        ICommand _cmdGenerateReviewCountByFTView;

        public ReviewsManager
        (
            ICommand cmdGenerateReviewCountByMonth,
            ICommand cmdGenerateReviewCountByProduct,
            ICommand cmdGenerateReviewCountByViewPoint,
            ICommand cmdGenerateReviewCountByFTView
        )
        {
            _cmdGenerateReviewCountByMonth = cmdGenerateReviewCountByMonth;
            _cmdGenerateReviewCountByProduct = cmdGenerateReviewCountByProduct;
            _cmdGenerateReviewCountByViewPoint = cmdGenerateReviewCountByViewPoint;
            _cmdGenerateReviewCountByFTView = cmdGenerateReviewCountByFTView;
        }

        public void GenerateReviewCountByMonth()
        {
            _cmdGenerateReviewCountByMonth.Execute();
        }

        public void GenerateReviewCountByProduct()
        {
            _cmdGenerateReviewCountByProduct.Execute();
        }

        public void GenerateReviewCountByViewPoint()
        {
            _cmdGenerateReviewCountByViewPoint.Execute();
        }

        public void GenerateReviewCountByFTView()
        {
            _cmdGenerateReviewCountByFTView.Execute();
        }
    }
}
