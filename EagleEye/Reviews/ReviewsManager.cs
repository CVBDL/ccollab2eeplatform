namespace EagleEye.Reviews
{
    public class ReviewsManager
    {
        ICommand _cmdGenerateReviewCountByMonth;
        ICommand _cmdGenerateReviewCountByProduct;
        ICommand _cmdGenerateReviewCountByEmployeeOfProduct;

        public ReviewsManager
        (
            ICommand cmdGenerateReviewCountByMonth,
            ICommand cmdGenerateReviewCountByProduct,
            ICommand cmdGenerateReviewCountByEmployeeOfProduct
        )
        {
            _cmdGenerateReviewCountByMonth = cmdGenerateReviewCountByMonth;
            _cmdGenerateReviewCountByProduct = cmdGenerateReviewCountByProduct;
            _cmdGenerateReviewCountByEmployeeOfProduct = cmdGenerateReviewCountByEmployeeOfProduct;
        }

        public void GenerateReviewCountByMonth()
        {
            _cmdGenerateReviewCountByMonth.Execute();
        }

        public void GenerateReviewCountByProduct()
        {
            _cmdGenerateReviewCountByProduct.Execute();
        }

        public void GenerateReviewCountByEmployeeOfProduct()
        {
            _cmdGenerateReviewCountByEmployeeOfProduct.Execute();
        }
    }
}
