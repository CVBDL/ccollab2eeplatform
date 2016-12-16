namespace EagleEye.Reviews
{
    public class GenerateReviewCountByMonthCommand: ICommand
    {
        private Reviews _reviewsDataGenerator;

        public GenerateReviewCountByMonthCommand(Reviews reviewsDataGenerator)
        {
            _reviewsDataGenerator = reviewsDataGenerator;
        }

        public void Execute()
        {
            _reviewsDataGenerator.GenerateReviewCountByMonth();
        }
    }
}
