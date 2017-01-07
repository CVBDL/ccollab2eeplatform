namespace EagleEye.Defects
{
    public class DefectsManager
    {
        ICommand _cmdDefectCountByProduct;
        ICommand _cmdDefectSeverityByProduct;
        ICommand _cmdDefectCountByInjectionStage;
        ICommand _cmdDefectCountByType;
        ICommand _cmdDefectsDistributionByType;
        ICommand _cmdDefectCountByCreator;

        public DefectsManager
        (
            ICommand cmdDefectCountByProduct,
            ICommand cmdDefectSeverityByProduct,
            ICommand cmdDefectCountByInjectionStage,
            ICommand cmdDefectCountByType,
            ICommand cmdDefectsDistributionByType,
            ICommand cmdDefectCountByCreator
        )
        {
            _cmdDefectCountByProduct = cmdDefectCountByProduct;
            _cmdDefectSeverityByProduct = cmdDefectSeverityByProduct;
            _cmdDefectCountByInjectionStage = cmdDefectCountByInjectionStage;
            _cmdDefectCountByType = cmdDefectCountByType;
            _cmdDefectsDistributionByType = cmdDefectsDistributionByType;
            _cmdDefectCountByCreator = cmdDefectCountByCreator;
        }

        public void GenerateDefectCountByProduct()
        {
            _cmdDefectCountByProduct.Execute();
        }

        public void GenerateDefectSeverityByProduct()
        {
            _cmdDefectSeverityByProduct.Execute();
        }

        public void GenerateDefectCountByInjectionStage()
        {
            _cmdDefectCountByInjectionStage.Execute();
        }

        public void GenerateDefectCountByType()
        {
            _cmdDefectCountByType.Execute();
        }

        public void GenerateDefectsDistributionByType()
        {
            _cmdDefectsDistributionByType.Execute();
        }

        public void GenerateDefectCountByCreator()
        {
            _cmdDefectCountByCreator.Execute();
        }
    }
}
