public interface IStageRepository
{
    /// <summary>
    /// 스테이지 데이터를 저장합니다.
    /// </summary>
    /// <param name="saveData">저장할 스테이지 데이터</param>
    void Save(StageData saveData);
    
    /// <summary>
    /// 저장된 스테이지 데이터를 로드합니다.
    /// </summary>
    /// <returns>로드된 스테이지 데이터, 없으면 null</returns>
    StageData Load();
    
    /// <summary>
    /// 저장된 스테이지 데이터를 삭제합니다.
    /// </summary>
    void Delete();
    
    /// <summary>
    /// 저장 파일이 존재하는지 확인합니다.
    /// </summary>
    /// <returns>저장 파일 존재 여부</returns>
    bool HasSaveData();
}