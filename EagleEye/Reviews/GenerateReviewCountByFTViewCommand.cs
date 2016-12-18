namespace EagleEye.Reviews
{
    public class GenerateReviewCountByFTViewCommand : ICommand
    {
        private Reviews _reviews;

        public GenerateReviewCountByFTViewCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateReviewCountForProduct("FTView", "ReviewCountForFTView");
        }
    }
}
