namespace EagleEye.Reviews
{
    public class GenerateReviewCountByViewPointCommand : ICommand
    {
        private Reviews _reviews;

        public GenerateReviewCountByViewPointCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateReviewCountForProduct("ViewPoint", "ReviewCountForViewPoint");
        }
    }
}
