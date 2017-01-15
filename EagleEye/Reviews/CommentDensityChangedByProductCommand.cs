namespace EagleEye.Reviews
{
    public class CommentDensityChangedByProductCommand : ICommand
    {
        private Reviews _reviews;

        public CommentDensityChangedByProductCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateCommentDensityChangedByProduct();
        }
    }
}
