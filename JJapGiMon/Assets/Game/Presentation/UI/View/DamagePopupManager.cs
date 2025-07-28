using UnityEngine;
using UnityEngine.UI;
using System.Collections;


/// <summary>
/// ������ �˾��� uGUI�� ǥ���մϴ�.
/// ȭ�� ��ǥ�� Text�� ���� �ε巴�� �������� �մϴ�.
/// </summary>
public class DamagePopupManager : MonoBehaviour
{
    public static DamagePopupManager Instance { get; private set; }

    [Header("Prefab & Canvas")]
    [SerializeField] private GameObject popupPrefab;   // Text ������Ʈ�� �ִ� UI ������
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
    /// worldPos ��ġ���� damage ���� �˾��� ���� �ݴϴ�.
    /// </summary>
    public void ShowDamage(Vector3 worldPos, int damage)
    {
        // ��ũ�� ��ǥ ���
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // �˾� �ν��Ͻ�ȭ
        var popupGO = Instantiate(popupPrefab, canvas.transform);
        var text = popupGO.GetComponentInChildren<Text>();
        text.text = damage.ToString();

        // ��ġ ����
        var rect = popupGO.GetComponent<RectTransform>();
        rect.anchoredPosition = screenPos;

        // �ִϸ��̼� ����
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
