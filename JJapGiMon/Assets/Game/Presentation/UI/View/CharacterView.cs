using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// CharacterModel의 상태 변화를 uGUI로 시각화합니다.
/// 각 캐릭터 GameObject에 붙여 사용.
/// </summary>
[RequireComponent(typeof(CharacterModel))]
public class CharacterView : MonoBehaviour
{
    [SerializeField] private CharacterModel model;
    [Header("UI References (World Space Canvas)")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Transform buffContainer;  // HorizontalLayoutGroup이 붙어 있는 부모
    [SerializeField] private GameObject buffIconPrefab; // Image만 있는 간단한 아이콘 프리팹

    private void Awake()
    {
        // 이벤트 구독
        model.OnHpChanged += UpdateHp;
        model.OnBuffsChanged += UpdateBuffs;
        model.OnDied += HandleDeath;

        // 초기 값 설정
        UpdateHp(model.CurrentStat.hp, model.CurrentStat.maxHp);
        UpdateBuffs(model.Buffs);
    }

    private void OnDestroy()
    {
        // 이벤트 해제
        model.OnHpChanged -= UpdateHp;
        model.OnBuffsChanged -= UpdateBuffs;
        model.OnDied -= HandleDeath;
    }

    private void UpdateHp(int current, int max)
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = max;
            hpSlider.value = current;
        }
    }

    private void UpdateBuffs(IEnumerable<StatusEffect> buffs)
    {
        // 기존 아이콘 제거
        foreach (Transform child in buffContainer)
            Destroy(child.gameObject);

        // 새로 아이콘 생성
        foreach (var buff in buffs)
        {
            var iconGO = Instantiate(buffIconPrefab, buffContainer);
            var img = iconGO.GetComponent<Image>();
            if (img != null)
                img.sprite = buff.IconSprite;
        }
    }

    private void HandleDeath()
    {
        // 투명 처리
        var rend = GetComponent<SpriteRenderer>();
        if (rend != null)
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0.5f);
    }
}