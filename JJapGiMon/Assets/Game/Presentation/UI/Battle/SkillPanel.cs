using System;
using UnityEngine.UIElements;

/// <summary>
/// 스킬 선택 UI 컴포넌트. 버튼 클릭 시 선택된 스킬을 콜백.
/// </summary>
public sealed class SkillPanel
{
    private readonly VisualElement container;

    public SkillPanel(VisualElement container)
    {
        this.container = container;
    }

    public void Open(CharacterModel caster, Action<ActiveSkill> onPick)
    {
        container.Clear();
        foreach (var skill in new[] { caster.MainSkill, caster.Sub1Skill, caster.Sub2Skill })
        {
            if (skill == null) continue;
            var btn = new Button(() => onPick(skill)) { text = skill.skillName };
            container.Add(btn);
        }
        container.style.display = DisplayStyle.Flex;
    }

    public void Close()
    {
        container.Clear();
        container.style.display = DisplayStyle.None;
    }
}