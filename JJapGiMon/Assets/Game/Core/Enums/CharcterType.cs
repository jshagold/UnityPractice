public sealed record CharacterType(int Id, string Name)
{
    // 인간
    public static readonly CharacterType Human = new(1, "인간");
    
    // 동물
    public static readonly CharacterType Animal = new(2, "동물");
    
    // 기계
    public static readonly CharacterType Machine = new(3, "기계");

    
}