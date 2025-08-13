using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class LocalStageRepository : IStageRepository
{
    private const string STAGE_SAVE_FILENAME = "current_stage.json";
    
    private readonly string saveFilePath;
    
    public LocalStageRepository()
    {
        // Unity의 Application.persistentDataPath를 사용하여 플랫폼별 저장 경로 설정
        saveFilePath = Path.Combine(Application.persistentDataPath, STAGE_SAVE_FILENAME);
    }
    
    /// <summary>
    /// 현재 스테이지 데이터를 JSON 파일로 저장합니다.
    /// </summary>
    /// <param name="saveData">저장할 스테이지 데이터</param>
    public void Save(StageData saveData)
    {
        if (saveData == null)
        {
            Debug.LogError("저장할 스테이지 데이터가 null입니다.");
            return;
        }
        
        try
        {
            // JSON 설정 (들여쓰기 포함, null 값 포함)
            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Include,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            
            // StageData를 JSON으로 직렬화
            string json = JsonConvert.SerializeObject(saveData, jsonSettings);
            
            // 파일에 저장 (덮어쓰기)
            File.WriteAllText(saveFilePath, json);
            
            Debug.Log($"스테이지 데이터 저장 완료: {saveFilePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"스테이지 데이터 저장 중 오류 발생: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// 저장된 스테이지 데이터를 로드합니다.
    /// </summary>
    /// <returns>로드된 스테이지 데이터, 없으면 null</returns>
    public StageData Load()
    {
        try
        {
            if (!File.Exists(saveFilePath))
            {
                Debug.Log("저장된 스테이지 데이터가 없습니다.");
                return null;
            }
            
            string json = File.ReadAllText(saveFilePath);
            
            // JSON 설정
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            
            // JSON을 StageData로 역직렬화
            StageData stageData = JsonConvert.DeserializeObject<StageData>(json, jsonSettings);
            
            // 노드 맵 초기화
            if (stageData != null)
            {
                stageData.InitializeNodeMap();
            }
            
            Debug.Log($"스테이지 데이터 로드 완료: {saveFilePath}");
            return stageData;
        }
        catch (Exception ex)
        {
            Debug.LogError($"스테이지 데이터 로드 중 오류 발생: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 저장된 스테이지 데이터를 삭제합니다.
    /// </summary>
    public void Delete()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
                Debug.Log($"스테이지 데이터 삭제 완료: {saveFilePath}");
            }
            else
            {
                Debug.LogWarning("삭제할 저장 파일이 존재하지 않습니다.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"스테이지 데이터 삭제 중 오류 발생: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// 저장 파일이 존재하는지 확인합니다.
    /// </summary>
    /// <returns>저장 파일 존재 여부</returns>
    public bool HasSaveData()
    {
        return File.Exists(saveFilePath);
    }
}