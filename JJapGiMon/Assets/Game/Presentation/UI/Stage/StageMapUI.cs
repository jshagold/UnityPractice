using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class StageMapUI : MonoBehaviour
{
    [Header("Config")]
    // 설정은 StageController에서 관리

    [Header("UI Refs")]
    [SerializeField] private RectTransform mapRoot;        // UIRootCanvas 하위 Panel 등
    [SerializeField] private Button roomButtonPrefab;      // (TMP)Text 포함 프리팹
    [SerializeField] private Image connectionImagePrefab;  // 얇은 Image(선으로 사용)

    [Header("Colors")]
    [SerializeField] private Color startRoomColor = Color.green;
    [SerializeField] private Color battleRoomColor = Color.red;
    [SerializeField] private Color eventRoomColor = Color.blue;
    [SerializeField] private Color bossRoomColor = Color.cyan;
    [SerializeField] private Color goalRoomColor = Color.yellow;    // 목표 방 색상
    [SerializeField] private Color failRoomColor = Color.gray;      // 실패 방 색상

    private StageMapModel model;
    private StageNode currentNode;
    private readonly Dictionary<StageNode, Button> roomButtons = new();

    private void Start()
    {
        // 컨트롤러에서 초기화하므로 여기서는 아무것도 하지 않음
    }

    /// <summary>
    /// 컨트롤러에서 호출하여 맵 업데이트
    /// </summary>
    public void UpdateMap(StageMapModel model, StageNode currentNode)
    {
        this.model = model;
        this.currentNode = currentNode;
        Clear();
        RenderMap();
        UpdateButtonStates();
    }

    /// <summary>
    /// 현재 노드 업데이트 (컨트롤러에서 호출)
    /// </summary>
    public void UpdateCurrentNode(StageNode newNode)
    {
        currentNode = newNode;
        UpdateButtonStates();
    }

    private void RenderMap()
    {
        // 각 depth별로 노드들을 렌더링
        for (int depth = 0; depth < model.levels.Count; depth++)
        {
            var level = model.levels[depth];
            for (int index = 0; index < level.Count; index++)
            {
                var node = level[index];
                CreateRoomButton(node, depth, index);
            }
        }

        // 연결선 그리기
        DrawConnections();
    }

    private void CreateRoomButton(StageNode node, int depth, int index)
    {
        var button = Instantiate(roomButtonPrefab, mapRoot);
        var rectTransform = button.GetComponent<RectTransform>();
        
        // 위치 설정 (depth에 따라 x축, index에 따라 y축)
        float xPos = depth * 200f;
        float yPos = (index - (model.levels[depth].Count - 1) * 0.5f) * 150f;
        rectTransform.anchoredPosition = new Vector2(xPos, yPos);

        // 텍스트 설정
        var text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = GetRoomDisplayText(node);
        }

        // 색상 설정
        var image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = GetRoomColor(node);
        }

        // 클릭 이벤트 설정
        button.onClick.AddListener(() => OnClickNode(node));
        
        roomButtons[node] = button;
    }

    private string GetRoomDisplayText(StageNode node)
    {
        if (node.depth == model.length) // 마지막 방
        {
            return node.isGoal ? "목표" : "실패";
        }
        
        return node.type switch
        {
            StageRoomType.Start => "시작",
            StageRoomType.Battle => "전투",
            StageRoomType.Event => "이벤트",
            StageRoomType.Boss => "보스",
            _ => "???"
        };
    }

    private Color GetRoomColor(StageNode node)
    {
        if (node.depth == model.length) // 마지막 방
        {
            return node.isGoal ? goalRoomColor : failRoomColor;
        }
        
        return node.type switch
        {
            StageRoomType.Start => startRoomColor,
            StageRoomType.Battle => battleRoomColor,
            StageRoomType.Event => eventRoomColor,
            StageRoomType.Boss => bossRoomColor,
            _ => Color.white
        };
    }

    private void DrawConnections()
    {
        // 각 노드에서 자식 노드로 연결선 그리기
        foreach (var kvp in roomButtons)
        {
            var parentNode = kvp.Key;
            var parentButton = kvp.Value;
            
            foreach (var childNode in parentNode.children)
            {
                if (roomButtons.TryGetValue(childNode, out var childButton))
                {
                    CreateConnectionLine(parentButton, childButton);
                }
            }
        }
    }

    private void CreateConnectionLine(Button from, Button to)
    {
        var line = Instantiate(connectionImagePrefab, mapRoot);
        var rectTransform = line.GetComponent<RectTransform>();
        
        // 연결선 위치와 크기 계산
        Vector2 fromPos = from.GetComponent<RectTransform>().anchoredPosition;
        Vector2 toPos = to.GetComponent<RectTransform>().anchoredPosition;
        Vector2 center = (fromPos + toPos) * 0.5f;
        Vector2 direction = (toPos - fromPos).normalized;
        float distance = Vector2.Distance(fromPos, toPos);
        
        rectTransform.anchoredPosition = center;
        rectTransform.sizeDelta = new Vector2(distance, 2f);
        rectTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
    }

    // --- Events --- //
    public event Action<StageNode> OnNodeClicked; // 노드 클릭 이벤트

    private void OnClickNode(StageNode node)
    {
        // 노드 클릭 이벤트 발생
        OnNodeClicked?.Invoke(node);
    }

    private void UpdateButtonStates()
    {
        // 모든 버튼을 비활성화
        foreach (var button in roomButtons.Values)
        {
            button.interactable = false;
        }

        // 현재 노드의 자식들만 활성화
        foreach (var child in currentNode.children)
        {
            if (roomButtons.TryGetValue(child, out var button))
            {
                button.interactable = true;
            }
        }
    }

    private void Clear()
    {
        roomButtons.Clear();
        for (int i = mapRoot.childCount - 1; i >= 0; i--)
            Destroy(mapRoot.GetChild(i).gameObject);
    }
}