namespace EagleEye.Reviews
{
    public class CommentDensityChangedByMonthCommand : ICommand
    {
        private Reviews _reviews;

        public CommentDensityChangedByMonthCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateCommentDensityChangedByMonthFromProduct();
        }
    }
}
