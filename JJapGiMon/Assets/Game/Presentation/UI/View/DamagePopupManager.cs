using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;


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
    [SerializeField] private float floatDuration = 1f; // 떠오르는 시간
    [SerializeField] private Vector3 floatOffset = new Vector3(0, 1f, 0);

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
        // 팝업 인스턴스화
        var popup = Instantiate(popupPrefab, canvas.transform);
        var text = popup.GetComponentInChildren<TextMeshProUGUI>();
        text.text = damage.ToString();

        // 위치 설정
        popup.transform.position = worldPos;

        // 애니메이션 시작
        StartCoroutine(AnimatePopup(popup, text));
    }

    private IEnumerator AnimatePopup(GameObject popup, TextMeshProUGUI text)
    {
        float elapsed = 0f;
        var rect = popup.GetComponent<RectTransform>();
        var color = text.color;

        while (elapsed < floatDuration)
        {
            float t = elapsed / floatDuration;
            rect.position += floatOffset * (Time.deltaTime / floatDuration);
            text.color = new Color(color.r, color.g, color.b, 1f - t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(popup);
    }
}
