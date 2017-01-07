namespace EagleEye.Reviews
{
    public class ReviewCountByMonthCommand : ICommand
    {
        private Reviews _reviews;

        public ReviewCountByMonthCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateReviewCountByMonthFromProduct();
        }
    }
}
