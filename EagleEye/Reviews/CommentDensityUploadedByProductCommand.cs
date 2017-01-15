namespace EagleEye.Reviews
{
    public class CommentDensityUploadedByProductCommand : ICommand
    {
        private Reviews _reviews;

        public CommentDensityUploadedByProductCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateCommentDensityUploadedByProduct();
        }
    }
}
