namespace EagleEye.Defects
{
    public interface IDefectsCommands
    {
        void GenerateDefectCountByProduct();
        void GenerateDefectCountBySeverity();
        void GenerateDefectCountByInjectionStage();
        void GenerateDefectCountByType();
    }
}
