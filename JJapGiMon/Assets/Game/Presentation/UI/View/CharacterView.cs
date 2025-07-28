using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CharacterModel�� ���� ��ȭ�� uGUI�� �ð�ȭ�մϴ�.
/// ��ũ��Ʈ�� ĳ���� �����տ� ���̰�, �ܺ� Canvas�� �����ϼ���.
/// </summary>
[RequireComponent(typeof(CharacterModel))]
public class CharacterView : MonoBehaviour
{
    [SerializeField] private CharacterModel model;
    [Header("UI References (World Space Canvas)")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Transform buffContainer;  // ��Ʈ Transform (HorizontalLayoutGroup)
    [SerializeField] private GameObject buffIconPrefab; // ������ Image ������

    private void Awake()
    {
        model.OnHpChanged += OnHpChanged;
        model.OnDeath += OnDeath;
        model.OnBuffsChanged += OnBuffsChanged;

        // �ʱ� �ʱ�ȭ
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

    // ���� ó�� TODO
    //private void OnBuffsChanged(IEnumerable<StatusEffect> buffs)
    //{
    //    // ���� ������ ����
    //    foreach (Transform child in buffContainer)
    //        Destroy(child.gameObject);

    //    // ���ο� ������ ����
    //    foreach (var buff in buffs)
    //    {
    //        var go = Instantiate(buffIconPrefab, buffContainer);
    //        var img = go.GetComponent<Image>();
    //        if (img != null && buff.IconSprite != null)
    //            img.sprite = buff.IconSprite;
    //    }
    //}

    // ��� ó��
    private void OnDeath()
    {
        // ���� ó��
        var rend = GetComponent<SpriteRenderer>();
        if (rend != null)
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0.5f);
    }
}