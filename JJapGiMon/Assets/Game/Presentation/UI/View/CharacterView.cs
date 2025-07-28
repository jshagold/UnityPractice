using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// CharacterModel�� ���� ��ȭ�� uGUI�� �ð�ȭ�մϴ�.
/// �� ĳ���� GameObject�� �ٿ� ���.
/// </summary>
[RequireComponent(typeof(CharacterModel))]
public class CharacterView : MonoBehaviour
{
    [SerializeField] private CharacterModel model;
    [Header("UI References (World Space Canvas)")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Transform buffContainer;  // HorizontalLayoutGroup�� �پ� �ִ� �θ�
    [SerializeField] private GameObject buffIconPrefab; // Image�� �ִ� ������ ������ ������

    private void Awake()
    {
        // �̺�Ʈ ����
        model.OnHpChanged += UpdateHp;
        model.OnBuffsChanged += UpdateBuffs;
        model.OnDied += HandleDeath;

        // �ʱ� �� ����
        UpdateHp(model.CurrentStat.hp, model.CurrentStat.maxHp);
        UpdateBuffs(model.Buffs);
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ����
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
        // ���� ������ ����
        foreach (Transform child in buffContainer)
            Destroy(child.gameObject);

        // ���� ������ ����
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
        // ���� ó��
        var rend = GetComponent<SpriteRenderer>();
        if (rend != null)
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0.5f);
    }
}