using UnityEngine;
using UnityEngine.UI;
using System.Collections;


/// <summary>
/// 데미지 팝업을 uGUI로 표시합니다.
/// 화면 좌표에 Text를 띄우고 부드럽게 떠오르게 합니다.
/// </summary>
public class DamagePopupManager : MonoBehaviour
{
    public static DamagePopupManager Instance { get; private set; }

    [Header("Prefab & Canvas")]
    [SerializeField] private GameObject popupPrefab;   // Text 컴포넌트가 있는 UI 프리팹
    [SerializeField] private Canvas canvas;            // Screen Space - Overlay

    [Header("Animation Settings")]
    [SerializeField] private float floatDuration = 1f;
    [SerializeField] private Vector2 floatOffset = new Vector2(0, 50);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// worldPos 위치에서 damage 숫자 팝업을 보여 줍니다.
    /// </summary>
    public void ShowDamage(Vector3 worldPos, int damage)
    {
        // 스크린 좌표 계산
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // 팝업 인스턴스화
        var popupGO = Instantiate(popupPrefab, canvas.transform);
        var text = popupGO.GetComponentInChildren<Text>();
        text.text = damage.ToString();

        // 위치 설정
        var rect = popupGO.GetComponent<RectTransform>();
        rect.anchoredPosition = screenPos;

        // 애니메이션 시작
        StartCoroutine(AnimatePopup(rect, popupGO));
    }

    private IEnumerator AnimatePopup(RectTransform rect, GameObject popupGO)
    {
        float elapsed = 0f;
        var image = popupGO.GetComponentInChildren<Text>();
        Color originalColor = image.color;

        while (elapsed < floatDuration)
        {
            float t = elapsed / floatDuration;
            rect.anchoredPosition += floatOffset * Time.deltaTime / floatDuration;
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f - t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(popupGO);
    }
}
