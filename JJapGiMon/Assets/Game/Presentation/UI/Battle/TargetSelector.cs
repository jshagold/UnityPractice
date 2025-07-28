using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

/// <summary>
/// �� Ÿ�� ���� UI ������Ʈ. SingleEnemy�� UI�� ó��.
/// </summary>
public sealed class TargetSelector
{
    private readonly VisualElement container;

    public TargetSelector(VisualElement container)
    {
        this.container = container;
    }
    
    public void Open(ActiveSkill skill, List<CharacterModel> enemies, Action<List<CharacterModel>> onPick)
    {
        container.Clear();

        if(skill.TargetType == SkillTargeting.SingleEnemy)
        {
            foreach (var enemy in enemies)
            {
                if(enemy.IsDead) continue;
                var btn = new Button(() => onPick(new List<CharacterModel> { enemy })) { text = enemy.DisplayName };
                container.Add(btn);
            }
            container.style.display = DisplayStyle.Flex;
        }
        else
        {
            // Multi-target�� InputManager���� ó���ϹǷ� UI�� ����� ����
            onPick(skill.TargetType == SkillTargeting.None
                ? new List<CharacterModel> { /* caster ó�� */ }
                : new List<CharacterModel>(enemies));
        }
    }
    public void Close()
    {
        container.Clear();
        container.style.display = DisplayStyle.None;
    }
}