public class CharacterFactory
{


    public CharacterModel Create(string CharacterId)
    {
        CharacterData data = new CharacterData();
        CharacterSaveData saveData = new CharacterSaveData();
        
        // TODO DataRepository로 데이터 가져와야함
        
        return new CharacterModel(data, saveData);
    }
}