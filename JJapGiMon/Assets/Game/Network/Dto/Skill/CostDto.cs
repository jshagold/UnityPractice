[System.Serializable]
public class CostDto        // 확장 대비: 여러 비용 타입 추가 가능
{
    public double health;   // null 허용 → double? 로도 가능
}