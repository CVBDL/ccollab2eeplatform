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
        ICommand _cmdDefectSeverityCountByCreator;
        ICommand _cmdDefectCountOfTypeByCreator;

        public DefectsManager
        (
            ICommand cmdDefectCountByProduct,
            ICommand cmdDefectSeverityByProduct,
            ICommand cmdDefectCountByInjectionStage,
            ICommand cmdDefectCountByType,
            ICommand cmdDefectsDistributionByType,
            ICommand cmdDefectCountByCreator,
            ICommand cmdDefectSeverityCountByCreator,
            ICommand cmdDefectCountOfTypeByCreator
        )
        {
            _cmdDefectCountByProduct = cmdDefectCountByProduct;
            _cmdDefectSeverityByProduct = cmdDefectSeverityByProduct;
            _cmdDefectCountByInjectionStage = cmdDefectCountByInjectionStage;
            _cmdDefectCountByType = cmdDefectCountByType;
            _cmdDefectsDistributionByType = cmdDefectsDistributionByType;
            _cmdDefectCountByCreator = cmdDefectCountByCreator;
            _cmdDefectSeverityCountByCreator = cmdDefectSeverityCountByCreator;
            _cmdDefectCountOfTypeByCreator = cmdDefectCountOfTypeByCreator;
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
        public void GenerateDefectSeverityCountByCreator()
        {
            _cmdDefectSeverityCountByCreator.Execute();
        }
        public void GenerateDefectCountOfTypeByCreator()
        {
            _cmdDefectCountOfTypeByCreator.Execute();
        }
    }
}
