using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CharacterModel의 모든 상태 변화를 uGUI로 시각화합니다.
/// HP, EXP, 레벨, 스킬, 버프, 사망 처리 등 포함.
/// </summary>
[RequireComponent(typeof(CharacterModel))]
public class CharacterView : MonoBehaviour
{
    [SerializeField] private CharacterModel model;

    [Header("UI References (World Space Canvas)")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider expSlider;
    [SerializeField] private Text levelText;
    [SerializeField] private Text nameText;

    [Header("Skill Icons")]
    [SerializeField] private Image mainSkillIcon;
    [SerializeField] private Image sub1SkillIcon;
    [SerializeField] private Image sub2SkillIcon;

    [Header("Buffs")]
    [SerializeField] private Transform buffContainer;   // HorizontalLayoutGroup
    [SerializeField] private GameObject buffIconPrefab; // Image 프리팹

    // 버프 트래킹
    //private readonly Dictionary<StatusEffect, GameObject> buffIcons = new();

    private void Awake()
    {
        if (model == null) model = GetComponent<CharacterModel>();

        // 초기 UI 설정
        nameText.text = model.DisplayName;
        levelText.text = $"Lv {model.Level}";
        hpSlider.maxValue = model.MaxHp;
        hpSlider.value = model.CurrentHp;
        expSlider.maxValue = model.MaxExp;
        expSlider.value = model.CurrentExp;

        UpdateSkillIcon(mainSkillIcon, model.MainSkill);
        UpdateSkillIcon(sub1SkillIcon, model.Sub1Skill);
        UpdateSkillIcon(sub2SkillIcon, model.Sub2Skill);

        // 초기 버프 표시
        //foreach (var buff in model.Buffs)
        //    AddBuffIcon(buff);

        // 이벤트 구독
        model.OnHpChanged += HandleHpChanged;
        model.OnExpChanged += HandleExpChanged;
        model.OnLevelUp += HandleLevelUp;
        model.OnMainSkillChanged += skill => UpdateSkillIcon(mainSkillIcon, skill);
        model.OnSub1SkillChanged += skill => UpdateSkillIcon(sub1SkillIcon, skill);
        model.OnSub2SkillChanged += skill => UpdateSkillIcon(sub2SkillIcon, skill);
        //model.OnBuffAdded += AddBuffIcon;
        //model.OnBuffRemoved += RemoveBuffIcon;
        model.OnDeath += HandleDeath;
    }

    private void OnDestroy()
    {
        // 이벤트 해제
        model.OnHpChanged -= HandleHpChanged;
        model.OnExpChanged -= HandleExpChanged;
        model.OnLevelUp -= HandleLevelUp;
        model.OnMainSkillChanged -= skill => UpdateSkillIcon(mainSkillIcon, skill);
        model.OnSub1SkillChanged -= skill => UpdateSkillIcon(sub1SkillIcon, skill);
        model.OnSub2SkillChanged -= skill => UpdateSkillIcon(sub2SkillIcon, skill);
        //model.OnBuffAdded -= AddBuffIcon;
        //model.OnBuffRemoved -= RemoveBuffIcon;
        model.OnDeath -= HandleDeath;
    }

    private void HandleHpChanged(int current, int max)
    {
        hpSlider.maxValue = max;
        hpSlider.value = current;
    }

    private void HandleExpChanged(int currentExp, int expToNext)
    {
        expSlider.maxValue = expToNext;
        expSlider.value = currentExp;
    }

    private void HandleLevelUp(int newLevel)
    {
        levelText.text = $"Lv {newLevel}";
    }

    private void UpdateSkillIcon(Image iconSlot, ActiveSkill? skill)
    {
        if (iconSlot == null) return;
        if (skill != null && skill.IconSprite != null)
        {
            iconSlot.sprite = skill.IconSprite;
            iconSlot.color = Color.white;
        }
        else
        {
            iconSlot.sprite = null;
            iconSlot.color = new Color(1, 1, 1, 0); // 숨김
        }
    }

    //private void AddBuffIcon(StatusEffect buff)
    //{
    //    if (buffIconPrefab == null || buffContainer == null) return;
    //    if (buffIcons.ContainsKey(buff)) return;

    //    var go = Instantiate(buffIconPrefab, buffContainer);
    //    var img = go.GetComponent<Image>();
    //    if (img != null && buff.IconSprite != null)
    //        img.sprite = buff.IconSprite;
    //    buffIcons[buff] = go;
    //}

    //private void RemoveBuffIcon(StatusEffect buff)
    //{
    //    if (buffIcons.TryGetValue(buff, out var go))
    //    {
    //        Destroy(go);
    //        buffIcons.Remove(buff);
    //    }
    //}

    private void HandleDeath()
    {
        var rend = GetComponent<SpriteRenderer>();
        if (rend != null)
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0.5f);
    }
}