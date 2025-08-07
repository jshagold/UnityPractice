using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;


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
    [SerializeField] private float floatDuration = 1f; // �������� �ð�
    [SerializeField] private Vector3 floatOffset = new Vector3(0, 1f, 0);

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
        // �˾� �ν��Ͻ�ȭ
        var popup = Instantiate(popupPrefab, canvas.transform);
        var text = popup.GetComponentInChildren<TextMeshProUGUI>();
        text.text = damage.ToString();

        // ��ġ ����
        popup.transform.position = worldPos;

        // �ִϸ��̼� ����
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
