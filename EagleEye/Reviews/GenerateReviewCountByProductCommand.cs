namespace EagleEye.Reviews
{
    public class GenerateReviewCountByProductCommand : ICommand
    {
        private Reviews _reviews;

        public GenerateReviewCountByProductCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateReviewCountByProduct();
        }
    }
}
