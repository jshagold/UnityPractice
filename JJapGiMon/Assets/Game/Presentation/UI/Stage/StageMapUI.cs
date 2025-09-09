using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

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

    [Header("Player Marker")]
    [SerializeField] private RectTransform playerMarkerPrefab;
    [SerializeField] private Vector2 markerOffset = new Vector2(0f, 40f);
    [SerializeField] private float markerMoveDuration = 0.25f;
    [SerializeField] private Ease markerMoveEase = Ease.InOutSine;
    private RectTransform playerMarker;
    private Tween markerMoveTween;


    [Header("Colors")]
    [SerializeField] private Color startRoomColor = Color.green;
    [SerializeField] private Color battleRoomColor = Color.red;
    [SerializeField] private Color eventRoomColor = Color.blue;
    [SerializeField] private Color bossRoomColor = Color.cyan;
    [SerializeField] private Color goalRoomColor = Color.yellow;    // 목표 방 색상
    [SerializeField] private Color failRoomColor = Color.gray;      // 실패 방 색상

    private StageNode rootNode;
    private StageNode currentNode;
    private readonly Dictionary<int, Button> roomButtons = new();

    // 이벤트 정의
    public event Action<StageNode> OnNodeClicked;

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        markerMoveTween?.Kill();
        markerMoveTween = null;
    }

    private void Start()
    {
        // 컨트롤러에서 초기화하므로 여기서는 아무것도 하지 않음
    }

    private void Update()
    {
        
    }


    // 맵 렌더링
    public void RenderMap(StageNode inputRootNode)
    {
        this.rootNode = inputRootNode;
        if (inputRootNode == null) return;
        Debug.Log("RenderMap");

        // 기존 UI 요소들 정리
        Clear();

        // 디버깅: 받은 rootNode 정보 출력
        Debug.Log($"받은 rootNode - Depth: {inputRootNode.depth}, Type: {inputRootNode.type}, Children Count: {inputRootNode.children?.Count ?? 0}");

        // 트리 구조를 순회하며 노드들을 렌더링
        var nodePositions = CalculateNodePositions();
        
        Debug.Log($"계산된 노드 위치 개수: {nodePositions.Count}");
        
        foreach (var kvp in nodePositions)
        {
            var node = kvp.Key;
            var position = kvp.Value;
            Debug.Log($"노드 생성: {node.roomName} (Depth: {node.depth}, Type: {node.type}) at {position}");
            CreateRoomButton(node, position);
        }

        // 연결선 그리기
        DrawConnections(nodePositions);
    }

    // 노드 위치 계산
    private Dictionary<StageNode, Vector2> CalculateNodePositions()
    {
        var positions = new Dictionary<StageNode, Vector2>();
        var depthGroups = new Dictionary<int, List<StageNode>>();
        
        // 트리를 순회하며 depth별로 노드들을 그룹화
        CollectNodesByDepth(rootNode, depthGroups, new HashSet<StageNode>());
        
        // 전체 맵의 크기 계산
        int maxDepth = depthGroups.Keys.Max();
        int minDepth = depthGroups.Keys.Min();
        float totalWidth = (maxDepth - minDepth) * 200f;
        
        // 맵의 중심점 계산
        float mapCenterX = totalWidth * 0.5f;
        
        Debug.Log($"맵 크기: {minDepth} ~ {maxDepth}, 총 너비: {totalWidth}, 중심점: {mapCenterX}");
        
        // 각 depth별로 노드들의 위치 계산
        foreach (var kvp in depthGroups)
        {
            int depth = kvp.Key;
            var nodes = kvp.Value;
            Debug.Log($"Depth {kvp.Key}: {kvp.Value.Count}개 노드");
            
            for (int i = 0; i < nodes.Count; i++)
            {
                // X 위치: depth에 따라 계산하되, 맵의 중심을 0으로 맞춤
                float xPos = (depth - minDepth) * 200f - mapCenterX;
                
                // Y 위치: 각 depth 내에서 노드들을 세로로 배치
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
        
        Debug.Log($"노드 수집: {node.roomName} (ID: {node.nodeId}, Depth: {node.depth}, Children: {node.children?.Count ?? 0})");
        
        // 자식 노드들을 순회 (순서 유지)
        if (node.children != null)
        {
            foreach (var child in node.children)
            {
                CollectNodesByDepth(child, depthGroups, visited);
            }
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
        
        roomButtons[node.nodeId] = button;
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
                return node.roomName;
            case StageRoomType.Battle:
                return node.roomName;
            case StageRoomType.Event:
                return node.roomName;
            case StageRoomType.Boss:
                return node.roomName;
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
                return bossRoomColor;
            default:
                return Color.white;
        }
    }


    // --- 캐릭터 마커 ---
    public void SetCurrentNode(StageNode node)
    {
        currentNode = node;
        EnsureMarker();

        Debug.Log($"SetCurrentNode: {node}");
        Debug.Log($"roomButtons Count: {roomButtons.Count}");

        foreach (var btn in roomButtons)
        {
            Debug.Log($"SetCurrentNode: {btn.Key} {btn.Value.name}");
        }

        if(node != null && roomButtons.TryGetValue(node.nodeId, out var button))
        {
            Debug.Log($"SetCurrentNode: {node.roomName} {button.name}");
            var rectTransform = button.GetComponent<RectTransform>();
            SnapMarkerTo(rectTransform);
        }
    }

    public void SetCurrentNodeById(int nodeId, StageGraph stageGraph)
    {
        if(stageGraph == null) return;

        var node = stageGraph.GetNodeById(nodeId);
        if(node != null) SetCurrentNode(node);
    }

    // 트윈으로 마커 이동
    public void MoveMarkerTo(StageNode node, bool withAnimation = true)
    {
        if(node == null) return;
        if(!roomButtons.TryGetValue(node.nodeId, out var button)) return;

        EnsureMarker();

        var targetRectTransform = button.GetComponent<RectTransform>();
        var targetPosition = targetRectTransform.anchoredPosition + markerOffset;

        markerMoveTween?.Kill();

        if(withAnimation)
        {
            markerMoveTween = playerMarker.DOAnchorPos(targetPosition, markerMoveDuration)
                .SetEase(markerMoveEase);
        }
        else
        {
            playerMarker.anchoredPosition = targetPosition;
        }
        currentNode = node;

    }

    private void EnsureMarker()
    {
        if(playerMarker == null && playerMarkerPrefab != null)
        {
            playerMarker = Instantiate(playerMarkerPrefab, mapRoot);
            playerMarker.gameObject.SetActive(true);
        }
    }

    private void SnapMarkerTo(RectTransform target)
    {
        if(playerMarker == null) return;
        playerMarker.anchoredPosition = target.anchoredPosition + markerOffset;
    }
    // --- 캐릭터 마커 ---
 



    private void Clear()
    {
        // 기존 UI 요소들 제거
        foreach (var button in roomButtons.Values)
        {
            if (button != null)
                DestroyImmediate(button.gameObject);
        }
        roomButtons.Clear();
        
        // 연결선들도 제거 (mapRoot의 모든 자식 중에서 connectionImagePrefab이 아닌 것들)
        var connections = mapRoot.GetComponentsInChildren<Image>();
        foreach (var connection in connections)
        {
            if (connection != connectionImagePrefab)
                DestroyImmediate(connection.gameObject);
        }
        
        Debug.Log("UI 요소들 정리 완료");
    }
}