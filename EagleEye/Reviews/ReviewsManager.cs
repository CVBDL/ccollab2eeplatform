namespace EagleEye.Reviews
{
    public class ReviewsManager
    {
        ICommand _generateReviewCountByMonthCommand;

        public ReviewsManager(ICommand generateReviewCountByMonthCommand)
        {
            _generateReviewCountByMonthCommand = generateReviewCountByMonthCommand;
        }

        public void GenerateReviewCountByMonth()
        {
            _generateReviewCountByMonthCommand.Execute();
        }
    }
}
