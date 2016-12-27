namespace EagleEye.Reviews
{
    public class CodeDefectDensityUploadedCommand : ICommand
    {
        private Reviews _reviews;

        public CodeDefectDensityUploadedCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateCodeDefectDensityUploaded("CodeDefectDensityUploaded");
        }
    }
}
