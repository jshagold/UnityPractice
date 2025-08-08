using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// CharacterModel의 모든 상태 변화를 uGUI로 시각화합니다.
/// HP, EXP, 레벨, 스킬, 버프, 사망 처리 등 포함.
/// </summary>
public class CharacterView : MonoBehaviour
{
    private CharacterModel model;

    [Header("UI References (World Space Canvas)")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image portrait;

    [Header("Skill Icons")]
    [SerializeField] private Image mainSkillIcon;
    [SerializeField] private Image sub1SkillIcon;
    [SerializeField] private Image sub2SkillIcon;

    // 초기 위치
    public Vector3 cachedSpawnPos;
    // Animation
    private Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 외부에서 CharacterModel을 주입하고, UI 초기화 및 이벤트 구독을 수행합니다.
    /// </summary>
    public void Initialize(CharacterModel model, Vector3 spawnPos)
    {
        this.model = model;

        // 1) UI 텍스트·슬라이더 초기 값 세팅
        nameText.text     = model.DisplayName;
        levelText.text    = $"Lv {model.Level}";
        hpSlider.maxValue = model.MaxHp;
        hpSlider.value    = model.CurrentHp;
        expSlider.maxValue= model.MaxExp;
        expSlider.value   = model.CurrentExp;

        // 2) 스킬·버프 아이콘 세팅 (필요 시 메서드 호출)
        UpdateSkillIcon(mainSkillIcon, model.MainSkill);
        UpdateSkillIcon(sub1SkillIcon, model.Sub1Skill);
        UpdateSkillIcon(sub2SkillIcon, model.Sub2Skill);

        // 3) 모델 이벤트 구독
        model.OnHpChanged      += HandleHpChanged;
        model.OnExpChanged     += HandleExpChanged;
        model.OnLevelUp        += HandleLevelUp;
        model.OnMainSkillChanged += skill => UpdateSkillIcon(mainSkillIcon, skill);
        model.OnSub1SkillChanged  += skill => UpdateSkillIcon(sub1SkillIcon, skill);
        model.OnSub2SkillChanged  += skill => UpdateSkillIcon(sub2SkillIcon, skill);
        model.OnDeath          += HandleDeath;
        model.OnDamageTaken += HandleDamagePopup;

        cachedSpawnPos = spawnPos;
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
        model.OnDeath -= HandleDeath;
        model.OnDamageTaken -= HandleDamagePopup;
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
        //if (skill != null && skill.IconSprite != null)
        //{
        //    iconSlot.sprite = skill.IconSprite;
        //    iconSlot.color = Color.white;
        //}
        //else
        //{
        //    iconSlot.sprite = null;
        //    iconSlot.color = new Color(1, 1, 1, 0); // 숨김
        //}
    }

    private void HandleDeath()
    {
        var rend = GetComponent<SpriteRenderer>();
        if (rend != null)
            rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0.5f);
    }


    private void HandleDamagePopup(int damage)
    {
        Vector3 popupPos = transform.position + Vector3.up * 1.5f;
        DamagePopupManager.Instance.ShowDamage(popupPos, damage);
    }

    /// <summary>
    /// 현재 위치에서 targetPosition까지 duration 초 동안 선형 보간 이동합니다.
    /// </summary>
    public IEnumerator MoveToPosition(Vector3 targetPosition, float duration = 0.5f)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }


    /* Animation */
    public IEnumerator PlayAttackAnimation()
    {
        //animator.SetTrigger("Attack");
        //// 애니메이션 길이에 맞춰서 대기
        //yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        yield return new WaitForSeconds(1f);
    }

    public IEnumerator PlayHitAnimation()
    {
        //animator.SetTrigger("Hit");
        //yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        yield return new WaitForSeconds(0.5f);
    }

    // 지정된 위치까지 트윈으로 이동하고, 완료 시점까지 대기한다.
    public IEnumerator MoveToTween(Vector3 targetWorldPos, float duration = 0.5f)
    {
        Debug.Log("Move To Tween");
        //animator.SetTrigger("Run");

        Tween moveTween = transform.DOMove(targetWorldPos, duration)
            .SetEase(Ease.OutQuad);

        yield return moveTween.WaitForCompletion();

        //animator.SetTrigger("Idle");
    }
}