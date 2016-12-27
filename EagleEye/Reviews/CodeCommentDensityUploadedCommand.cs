namespace EagleEye.Reviews
{
    public class CodeCommentDensityUploadedCommand : ICommand
    {
        private Reviews _reviews;

        public CodeCommentDensityUploadedCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateCodeCommentDensityUploaded("CodeCommentDensityUploaded");
        }
    }
}
