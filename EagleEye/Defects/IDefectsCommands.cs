namespace EagleEye.Defects
{
    public interface IDefectsCommands
    {
        void GenerateDefectCountByProduct();

        void GenerateDefectCountByCreatorFromProduct();

        void GenerateDefectCountOfTypeByProduct();

        void GenerateDefectCountOfTypeByCreatorFromProduct();
                
        void GenerateDefectCountByInjectionStageFromProduct();
        
        void GenerateDefectCountByTypeFromProduct();
        
        void GenerateDefectCountOfSeverityByProduct();
        
        void GenerateDefectCountOfSeverityByCreatorFromProduct();
    }
}
