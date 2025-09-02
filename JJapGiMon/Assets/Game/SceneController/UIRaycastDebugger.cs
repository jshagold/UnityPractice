using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class UIRaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 클릭 위치로 PointerEventData 생성
            var ped = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            // 결과 담을 리스트
            var results = new List<RaycastResult>();
            // 모든 UI Raycast 실행
            EventSystem.current.RaycastAll(ped, results);

            Debug.Log("=== UI Raycast Results ===");
            foreach (var r in results)
                Debug.Log($"  {r.gameObject.name}");
        }
    }
}