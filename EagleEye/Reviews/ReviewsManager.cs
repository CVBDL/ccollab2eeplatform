namespace EagleEye.Reviews
{
    public class ReviewsManager
    {
        ICommand _cmdGenerateReviewCountByMonth;
        ICommand _cmdGenerateReviewCountByProduct;

        public ReviewsManager(
            ICommand cmdGenerateReviewCountByMonth,
            ICommand cmdGenerateReviewCountByProduct
        )
        {
            _cmdGenerateReviewCountByMonth = cmdGenerateReviewCountByMonth;
            _cmdGenerateReviewCountByProduct = cmdGenerateReviewCountByProduct;
        }

        public void GenerateReviewCountByMonth()
        {
            _cmdGenerateReviewCountByMonth.Execute();
        }

        public void GenerateReviewCountByProduct()
        {
            _cmdGenerateReviewCountByProduct.Execute();
        }
    }
}
