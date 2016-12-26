namespace EagleEye.Reviews
{
    public class ReviewCountByEmployeeOfProductCommand : ICommand
    {
        private Reviews _reviews;

        public ReviewCountByEmployeeOfProductCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateReviewCountByEmployeeOfProduct();
        }
    }
}
