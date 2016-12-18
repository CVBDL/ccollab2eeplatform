namespace EagleEye.Reviews
{
    public class GenerateReviewCountByProductCommand : ICommand
    {
        private Reviews _reviewsDataGenerator;

        public GenerateReviewCountByProductCommand(Reviews reviewsDataGenerator)
        {
            _reviewsDataGenerator = reviewsDataGenerator;
        }

        public void Execute()
        {
            _reviewsDataGenerator.GenerateReviewCountByProduct();
        }
    }
}
