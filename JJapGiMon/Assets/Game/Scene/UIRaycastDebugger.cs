using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class UIRaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ���콺 Ŭ�� ��ġ�� PointerEventData ����
            var ped = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            // ��� ���� ����Ʈ
            var results = new List<RaycastResult>();
            // ��� UI Raycast ����
            EventSystem.current.RaycastAll(ped, results);

            Debug.Log("=== UI Raycast Results ===");
            foreach (var r in results)
                Debug.Log($"  {r.gameObject.name}");
        }
    }
}