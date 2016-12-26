namespace EagleEye.Reviews
{
    public class ReviewsManager
    {
        ICommand _cmdReviewCountByMonth;
        ICommand _cmdReviewCountByProduct;
        ICommand _cmdReviewCountByEmployeeOfProduct;

        public ReviewsManager
        (
            ICommand cmdReviewCountByMonth,
            ICommand cmdReviewCountByProduct,
            ICommand cmdReviewCountByEmployeeOfProduct
        )
        {
            _cmdReviewCountByMonth = cmdReviewCountByMonth;
            _cmdReviewCountByProduct = cmdReviewCountByProduct;
            _cmdReviewCountByEmployeeOfProduct = cmdReviewCountByEmployeeOfProduct;
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
    }
}
