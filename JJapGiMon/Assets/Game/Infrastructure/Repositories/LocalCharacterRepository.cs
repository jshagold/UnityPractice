using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class LocalCharacterRepository : ICharacterRepository
{
    string GetPath(string id) => Path.Combine(Application.persistentDataPath, $"char_{id}.json");

    public CharacterSaveData Load(string characterId)
    {
        var path = GetPath(characterId);
        if(!File.Exists(path))
        {
            return CharacterSaveData.New(characterId);
        }

        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<CharacterSaveData>(json)!;
    }

    public void Save(CharacterSaveData saveData)
    {
        var path = GetPath(saveData.Id);
        
        var json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(path, json);
    }
}