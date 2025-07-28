using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

/// <summary>
/// 적 타겟 선택 UI 컴포넌트. SingleEnemy만 UI로 처리.
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
            // Multi-target은 InputManager에서 처리하므로 UI는 띄우지 않음
            onPick(skill.TargetType == SkillTargeting.None
                ? new List<CharacterModel> { /* caster 처리 */ }
                : new List<CharacterModel>(enemies));
        }
    }
    public void Close()
    {
        container.Clear();
        container.style.display = DisplayStyle.None;
    }
}