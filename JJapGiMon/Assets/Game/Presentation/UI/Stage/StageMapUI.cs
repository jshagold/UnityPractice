using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class StageMapUI : MonoBehaviour
{

    [SerializeField] private StageManager stageManager;

    [Header("Config")]
    // 설정은 StageController에서 관리

    [Header("UI Refs")]
    [SerializeField] private RectTransform mapRoot;        // UIRootCanvas 하위 Panel 등
    [SerializeField] private Button startRoomButtonPrefab;      // (TMP)Text 포함 프리팹
    [SerializeField] private Button battleRoomButtonPrefab;      // (TMP)Text 포함 프리팹
    [SerializeField] private Button eventRoomButtonPrefab;      // (TMP)Text 포함 프리팹
    [SerializeField] private Button bossRoomButtonPrefab;      // (TMP)Text 포함 프리팹
    [SerializeField] private Image connectionImagePrefab;  // 얇은 Image(선으로 사용)

    [Header("Colors")]
    [SerializeField] private Color startRoomColor = Color.green;
    [SerializeField] private Color battleRoomColor = Color.red;
    [SerializeField] private Color eventRoomColor = Color.blue;
    [SerializeField] private Color bossRoomColor = Color.cyan;
    [SerializeField] private Color goalRoomColor = Color.yellow;    // 목표 방 색상
    [SerializeField] private Color failRoomColor = Color.gray;      // 실패 방 색상

    private StageNode rootNode;
    private StageNode currentNode;
    private readonly Dictionary<StageNode, Button> roomButtons = new();

    // 이벤트 정의
    public event Action<StageNode> OnNodeClicked;

    private void OnEnable()
    {
        stageManager.OnStageGenerated += RenderMap;
    }

    private void OnDisable()
    {
        stageManager.OnStageGenerated -= RenderMap;
    }

    private void Start()
    {
        // 컨트롤러에서 초기화하므로 여기서는 아무것도 하지 않음
    }

    private void Update()
    {

    }


    // 맵 렌더링
    private void RenderMap(int stageId)
    {
        // TODO stage id에 따른 배경 생성
        CreateBackground(stageId);

        if (rootNode == null) return;

        // 트리 구조를 순회하며 노드들을 렌더링
        var nodePositions = CalculateNodePositions();
        
        foreach (var kvp in nodePositions)
        {
            var node = kvp.Key;
            var position = kvp.Value;
            CreateRoomButton(node, position);
        }

        // 연결선 그리기
        DrawConnections(nodePositions);
    }

    // 배경 생성
    private void CreateBackground(int stageId)
    {

    }

    // 노드 위치 계산
    private Dictionary<StageNode, Vector2> CalculateNodePositions()
    {
        var positions = new Dictionary<StageNode, Vector2>();
        var depthGroups = new Dictionary<int, List<StageNode>>();
        
        // 트리를 순회하며 depth별로 노드들을 그룹화
        CollectNodesByDepth(rootNode, depthGroups, new HashSet<StageNode>());
        
        // 각 depth별로 노드들의 위치 계산
        foreach (var kvp in depthGroups)
        {
            int depth = kvp.Key;
            var nodes = kvp.Value;
            
            for (int i = 0; i < nodes.Count; i++)
            {
                float xPos = depth * 200f;
                float yPos = (i - (nodes.Count - 1) * 0.5f) * 150f;
                positions[nodes[i]] = new Vector2(xPos, yPos);
            }
        }
        
        return positions;
    }

    private void CollectNodesByDepth(StageNode node, Dictionary<int, List<StageNode>> depthGroups, HashSet<StageNode> visited)
    {
        if (node == null || visited.Contains(node)) return;
        
        visited.Add(node);
        
        if (!depthGroups.ContainsKey(node.depth))
            depthGroups[node.depth] = new List<StageNode>();
        
        depthGroups[node.depth].Add(node);
        
        foreach (var child in node.children)
        {
            CollectNodesByDepth(child, depthGroups, visited);
        }
    }

    // 방 UI 생성
    private void CreateRoomButton(StageNode node, Vector2 position)
    {
        var button = node.type switch
        {
            StageRoomType.Start => Instantiate(startRoomButtonPrefab, mapRoot),
            StageRoomType.Battle => Instantiate(battleRoomButtonPrefab, mapRoot),
            StageRoomType.Event => Instantiate(eventRoomButtonPrefab, mapRoot),
            StageRoomType.Boss => Instantiate(bossRoomButtonPrefab, mapRoot),
            _ => Instantiate(startRoomButtonPrefab, mapRoot),
        };

        // var button = Instantiate(roomButtonPrefab, mapRoot);
        var rectTransform = button.GetComponent<RectTransform>();
        
        // 위치 설정
        rectTransform.anchoredPosition = position;

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

    // 연결선 UI 생성
    private void DrawConnections(Dictionary<StageNode, Vector2> nodePositions)
    {
        // 모든 노드의 연결선 그리기
        foreach (var node in nodePositions.Keys)
        {
            if (node.children != null)
            {
                foreach (var child in node.children)
                {
                    if (nodePositions.ContainsKey(child))
                    {
                        DrawConnection(nodePositions[node], nodePositions[child]);
                    }
                }
            }
        }
    }

    // 연결선 UI 생성
    private void DrawConnection(Vector2 startPos, Vector2 endPos)
    {
        var connection = Instantiate(connectionImagePrefab, mapRoot);
        var rectTransform = connection.GetComponent<RectTransform>();
        
        // 연결선 위치와 회전 계산
        Vector2 direction = endPos - startPos;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        rectTransform.anchoredPosition = startPos + direction * 0.5f;
        rectTransform.sizeDelta = new Vector2(distance, 2f);
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }


    // 현재 노드 업데이트 (컨트롤러에서 호출)
    public void UpdateCurrentNode(StageNode newNode)
    {
        currentNode = newNode;
        UpdateButtonStates();
    }

    // 노드 상태 업데이트
    private void UpdateButtonStates()
    {
        foreach (var kvp in roomButtons)
        {
            var node = kvp.Key;
            var button = kvp.Value;
            
            // 현재 노드 강조
            if (node == currentNode)
            {
                button.transform.localScale = Vector3.one * 1.2f;
            }
            else
            {
                button.transform.localScale = Vector3.one;
            }
            
            // 접근 가능한 노드만 활성화
            bool isAccessible = IsNodeAccessible(node);
            button.interactable = isAccessible;
            
            // 비활성화된 노드는 회색 처리
            var image = button.GetComponent<Image>();
            if (image != null && !isAccessible)
            {
                image.color = Color.gray;
            }
        }
    }

    // 노드 접근 가능 여부 확인
    private bool IsNodeAccessible(StageNode node)
    {
        if (currentNode == null) return false;
        
        // 현재 노드의 자식들만 접근 가능
        return currentNode.children.Contains(node);
    }

    // 노드 클릭 이벤트
    private void OnClickNode(StageNode node)
    {
        OnNodeClicked?.Invoke(node);
    }

    private string GetRoomDisplayText(StageNode node)
    {
        if (node == null) return "Unknown";
        
        switch (node.type)
        {
            case StageRoomType.Start:
                return "시작";
            case StageRoomType.Battle:
                return "전투";
            case StageRoomType.Event:
                return "이벤트";
            case StageRoomType.Boss:
                return node.state == StageStateType.SUCCESS ? "목표" : "보스";
            default:
                return "방";
        }
    }

    private Color GetRoomColor(StageNode node)
    {
        if (node == null) return Color.white;
        
        switch (node.type)
        {
            case StageRoomType.Start:
                return startRoomColor;
            case StageRoomType.Battle:
                return battleRoomColor;
            case StageRoomType.Event:
                return eventRoomColor;
            case StageRoomType.Boss:
                return node.state == StageStateType.SUCCESS ? goalRoomColor : bossRoomColor;
            default:
                return Color.white;
        }
    }

    private void Clear()
    {
        // 기존 UI 요소들 제거
        foreach (var button in roomButtons.Values)
        {
            if (button != null)
                DestroyImmediate(button.gameObject);
        }
        roomButtons.Clear();
        
        // 연결선들도 제거
        var connections = mapRoot.GetComponentsInChildren<Image>();
        foreach (var connection in connections)
        {
            if (connection != connectionImagePrefab)
                DestroyImmediate(connection.gameObject);
        }
    }
}