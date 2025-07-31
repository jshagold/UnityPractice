using Newtonsoft.Json;
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

        var json = JsonConvert.SerializeObject(skillData, Formatting.Indented);
        File.WriteAllText(path, json);
    }

    public ActiveSkill LoadActiveSkill(string skillId)
    {
        var path = GetPath(skillId);
        if (!File.Exists(path))
        {
            Debug.Log("Null Skill");
            return ActiveSkill.New(skillId);
        }

        var json = File.ReadAllText(path);
        Debug.Log("" +
            $"json: {json}\n" +
            $"convert: {JsonConvert.DeserializeObject<ActiveSkill>(json)!}\n");
        return JsonConvert.DeserializeObject<ActiveSkill>(json)!;
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