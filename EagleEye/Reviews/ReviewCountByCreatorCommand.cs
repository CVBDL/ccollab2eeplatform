namespace EagleEye.Reviews
{
    public class ReviewCountByCreatorCommand : ICommand
    {
        private Reviews _reviews;

        public ReviewCountByCreatorCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateReviewCountByCreatorFromProduct();
        }
    }
}
