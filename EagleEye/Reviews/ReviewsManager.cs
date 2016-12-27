namespace EagleEye.Reviews
{
    public class ReviewsManager
    {
        ICommand _cmdReviewCountByMonth;
        ICommand _cmdReviewCountByProduct;
        ICommand _cmdReviewCountByEmployeeOfProduct;
        ICommand _cmdCodeCommentDensityUploaded;
        ICommand _cmdCodeCommentDensityChanged;
        ICommand _cmdCodeDefectDensityUploaded;
        ICommand _cmdCodeDefectDensityChanged;

        public ReviewsManager
        (
            ICommand cmdReviewCountByMonth,
            ICommand cmdReviewCountByProduct,
            ICommand cmdReviewCountByEmployeeOfProduct,
            ICommand cmdCodeCommentDensityUploaded,
            ICommand cmdCodeCommentDensityChanged,
            ICommand cmdCodeDefectDensityUploaded,
            ICommand cmdCodeDefectDensityChanged
        )
        {
            _cmdReviewCountByMonth = cmdReviewCountByMonth;
            _cmdReviewCountByProduct = cmdReviewCountByProduct;
            _cmdReviewCountByEmployeeOfProduct = cmdReviewCountByEmployeeOfProduct;
            _cmdCodeCommentDensityUploaded = cmdCodeCommentDensityUploaded;
            _cmdCodeCommentDensityChanged = cmdCodeCommentDensityChanged;
            _cmdCodeDefectDensityUploaded = cmdCodeDefectDensityUploaded;
            _cmdCodeDefectDensityChanged = cmdCodeDefectDensityChanged;
    }

        public void GenerateReviewCountByMonth()
        {
            _cmdReviewCountByMonth.Execute();
        }

        public void GenerateReviewCountByProduct()
        {
            _cmdReviewCountByProduct.Execute();
        }

        public void GenerateReviewCountByEmployeeOfProduct()
        {
            _cmdReviewCountByEmployeeOfProduct.Execute();
        }

        public void GenerateCodeCommentDensityUploaded()
        {
            _cmdCodeCommentDensityUploaded.Execute();
        }

        public void GenerateCodeCommentDensityChanged()
        {
            _cmdCodeCommentDensityChanged.Execute();
        }

        public void GenerateCodeDefectDensityUploaded()
        {
            _cmdCodeDefectDensityUploaded.Execute();
        }

        public void GenerateCodeDefectDensityChanged()
        {
            _cmdCodeDefectDensityChanged.Execute();
        }
    }
}
