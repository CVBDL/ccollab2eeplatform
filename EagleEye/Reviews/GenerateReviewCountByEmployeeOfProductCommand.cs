namespace EagleEye.Reviews
{
    public class GenerateReviewCountByEmployeeOfProductCommand : ICommand
    {
        private Reviews _reviews;

        public GenerateReviewCountByEmployeeOfProductCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateReviewCountByEmployeeOfProduct();
        }
    }
}
