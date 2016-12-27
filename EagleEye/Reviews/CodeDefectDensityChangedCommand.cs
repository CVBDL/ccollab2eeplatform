namespace EagleEye.Reviews
{
    public class CodeDefectDensityChangedCommand : ICommand
    {
        private Reviews _reviews;

        public CodeDefectDensityChangedCommand(Reviews reviews)
        {
            _reviews = reviews;
        }

        public void Execute()
        {
            _reviews.GenerateCodeDefectDensityChanged("CodeDefectDensityChanged");
        }
    }
}
