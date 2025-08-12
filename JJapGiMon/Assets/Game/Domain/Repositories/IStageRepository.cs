public interface IStageRepository
{
    StageData Load();
    void Save(StageData saveData);
    void Delete(StageData saveData);
}