namespace EagleEye.Defects
{
    public class DefectsManager
    {
        ICommand _cmdDefectCountByProduct;
        ICommand _cmdDefectCountByInjectionStage;
        ICommand _cmdDefectCountByType;
        ICommand _cmdDefectCountByCreator;
        ICommand _cmdDefectCountOfSeverityByProduct;
        ICommand _cmdDefectCountOfTypeByProduct;
        ICommand _cmdDefectCountOfSeverityByCreator;
        ICommand _cmdDefectCountOfTypeByCreator;

        public DefectsManager
        (
            ICommand cmdDefectCountByProduct,
            ICommand cmdDefectCountByInjectionStage,
            ICommand cmdDefectCountByType,
            ICommand cmdDefectCountByCreator,
            ICommand cmdDefectCountOfTypeByProduct,
            ICommand cmdDefectCountOfTypeByCreator,
            ICommand cmdDefectCountOfSeverityByProduct,
            ICommand cmdDefectCountOfSeverityByCreator
        )
        {
            _cmdDefectCountByProduct = cmdDefectCountByProduct;
            _cmdDefectCountByInjectionStage = cmdDefectCountByInjectionStage;
            _cmdDefectCountByType = cmdDefectCountByType;
            _cmdDefectCountOfSeverityByProduct = cmdDefectCountOfSeverityByProduct;
            _cmdDefectCountOfTypeByProduct = cmdDefectCountOfTypeByProduct;
            _cmdDefectCountByCreator = cmdDefectCountByCreator;
            _cmdDefectCountOfSeverityByCreator = cmdDefectCountOfSeverityByCreator;
            _cmdDefectCountOfTypeByCreator = cmdDefectCountOfTypeByCreator;
        }

        public void GenerateDefectCountByProduct()
        {
            _cmdDefectCountByProduct.Execute();
        }
        public void GenerateDefectSeverityByProduct()
        {
            _cmdDefectCountOfSeverityByProduct.Execute();
        }
        public void GenerateDefectCountByInjectionStage()
        {
            _cmdDefectCountByInjectionStage.Execute();
        }
        public void GenerateDefectCountByType()
        {
            _cmdDefectCountByType.Execute();
        }
        public void GenerateDefectCountOfTypeByProduct()
        {
            _cmdDefectCountOfTypeByProduct.Execute();
        }
        public void GenerateDefectCountByCreator()
        {
            _cmdDefectCountByCreator.Execute();
        }
        public void GenerateDefectSeverityCountByCreator()
        {
            _cmdDefectCountOfSeverityByCreator.Execute();
        }
        public void GenerateDefectCountOfTypeByCreator()
        {
            _cmdDefectCountOfTypeByCreator.Execute();
        }
    }
}
