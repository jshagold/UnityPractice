using System;
using UnityEngine.UIElements;

/// <summary>
/// ��ų ���� UI ������Ʈ. ��ư Ŭ�� �� ���õ� ��ų�� �ݹ�.
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