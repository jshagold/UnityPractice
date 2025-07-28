using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CharacterModel의 상태 변화를 uGUI로 시각화합니다.
/// 스크립트를 캐릭터 프리팹에 붙이고, 외부 Canvas와 연결하세요.
/// </summary>
[RequireComponent(typeof(CharacterModel))]
public class CharacterView : MonoBehaviour
{
    [SerializeField] private CharacterModel model;
    [Header("UI References (World Space Canvas)")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Transform buffContainer;  // 루트 Transform (HorizontalLayoutGroup)
    [SerializeField] private GameObject buffIconPrefab; // 간단한 Image 프리팹

    private void Awake()
    {
        model.OnHpChanged += OnHpChanged;
        model.OnDeath += OnDeath;
        model.OnBuffsChanged += OnBuffsChanged;

        // 초기 초기화
        OnHpChanged(model.CurrentHp, model.MaxHp);
        OnBuffsChanged(model.Buffs);
    }

    private void OnDestroy()
    {
        model.OnHpChanged -= OnHpChanged;
        model.OnDeath -= OnDeath;
        model.OnBuffsChanged -= OnBuffsChanged;
    }

    private void OnHpChanged(int current, int max)
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = max;
            hpSlider.value = current;
        }
    }

    // 버프 처리 TODO
    //private void OnBuffsChanged(IEnumerable<StatusEffect> buffs)
    //{
    //    // 기존 아이콘 제거
    //    foreach (Transform child in buffContainer)
    //        Destroy(child.gameObject);

    //    // 새로운 아이콘 생성
    //    foreach (var buff in buffs)
    //    {
    //        var go = Instantiate(buffIconPrefab, buffContainer);
    //        var img = go.GetComponent<Image>();
    //        if (img != null && buff.IconSprite != null)
    //            img.sprite = buff.IconSprite;
    //    }
    //}

    // 사망 처리
    private void OnDeath()
    {
        // 투명 처리
        var rend = GetComponent<SpriteRenderer>();
        if (rend != null)
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0.5f);
    }
}