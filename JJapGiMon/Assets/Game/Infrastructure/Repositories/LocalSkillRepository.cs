using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class LocalSkillRepository : ISkillRepository
{
    string GetPath(string id) => Path.Combine(Application.persistentDataPath, $"skill_{id}.json");



    public void Save(SkillData skillData)
    {
        var path = GetPath(skillData.skillId);
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            Formatting = Formatting.Indented
        };
        var json = JsonConvert.SerializeObject(skillData, settings);
        File.WriteAllText(path, json);
    }

    public ActiveSkill LoadActiveSkill(string skillId)
    {
        var path = GetPath(skillId);
        if (!File.Exists(path))
        {
            Debug.Log("Null Skill");
            ActiveSkill newSkill = ActiveSkill.New(skillId);
            Save(newSkill);
            return newSkill;
        }

        var json = File.ReadAllText(path);
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            Formatting = Formatting.Indented
        };

        var activeSkill = JsonConvert.DeserializeObject<ActiveSkill>(json, settings)!;
        var deserializeJson = JsonConvert.SerializeObject(activeSkill, settings);

        Debug.Log("" +
            $"json: {deserializeJson}\n");

        return JsonConvert.DeserializeObject<ActiveSkill>(json, settings)!;
    }

    public PassiveSkill LoadPassiveSkill(string skillId)
    {
        var path = GetPath(skillId);
        if (!File.Exists(path))
        {
            return PassiveSkill.New(skillId);
        }

        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<PassiveSkill>(json)!;
    }
}