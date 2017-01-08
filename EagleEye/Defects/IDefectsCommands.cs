namespace EagleEye.Defects
{
    public interface IDefectsCommands
    {
        void GenerateDefectCountByProduct(string settingsKey);

        void GenerateDefectCountByCreatorFromProduct();

        void GenerateDefectCountOfTypeByProduct(string settingsKey);

        void GenerateDefectCountOfTypeByCreatorFromProduct();
                
        void GenerateDefectCountByInjectionStageFromProduct();
        
        void GenerateDefectCountByTypeFromProduct();
        
        void GenerateDefectCountOfSeverityByProduct(string settingsKey);
        
        void GenerateDefectCountOfSeverityByCreatorFromProduct();
    }
}
