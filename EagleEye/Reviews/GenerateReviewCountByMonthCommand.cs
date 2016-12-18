namespace EagleEye.Reviews
{
    public class GenerateReviewCountByMonthCommand : ICommand
    {
        private Reviews _reviews;

        public GenerateReviewCountByMonthCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateReviewCountByMonth();
        }
    }
}
