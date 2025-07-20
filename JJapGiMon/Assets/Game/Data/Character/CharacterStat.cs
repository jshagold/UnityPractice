using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    [Header("Progression")]
    public int level;
    public int currentExp;

    [Header("Core")]
    public int health;
    public int strength;
    public int toughness;
    public float agility;

    [Header("Sub")]
    [Range(0f, 100f)] public float evasionRate;
    [Range(0f, 100f)] public float criticalRate;

    public CharacterStats(int level, int currentExp, int health, int strength, int toughness, float agility, float evasionRate, float criticalRate)
    {
        this.level = level;
        this.currentExp = currentExp;
        this.health = health;
        this.strength = strength;
        this.toughness = toughness;
        this.agility = agility;
        this.evasionRate = evasionRate;
        this.criticalRate = criticalRate;
    }
}