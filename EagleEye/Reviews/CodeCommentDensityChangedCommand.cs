namespace EagleEye.Reviews
{
    public class CodeCommentDensityChangedCommand : ICommand
    {
        private Reviews _reviews;

        public CodeCommentDensityChangedCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateCodeCommentDensityChanged("CodeCommentDensityChanged");
        }
    }
}
