namespace EagleEye.Reviews
{
    public class ReviewCountByProductCommand : ICommand
    {
        private Reviews _reviews;

        public ReviewCountByProductCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateReviewCountByProduct("ReviewCountByProduct");
        }
    }
}
