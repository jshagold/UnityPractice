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
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            Formatting = Formatting.Indented
        };

        return JsonConvert.DeserializeObject<CharacterSaveData>(json, settings)!;
    }

    public void Save(CharacterSaveData saveData)
    {
        var path = GetPath(saveData.Id);
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            Formatting = Formatting.Indented
        };

        var json = JsonConvert.SerializeObject(saveData, Formatting.Indented, settings);
        File.WriteAllText(path, json);
    }
}