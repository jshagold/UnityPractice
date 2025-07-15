using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string characterId;
    public string characterName;

    public int baseHp;
    public int baseAttack;
    public int baseDefense;
    public int baseSpeed;
}
