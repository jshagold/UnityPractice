public class CharacterFactory
{
    private readonly ICharacterRepository _characterRepository;

    public CharacterFactory(ICharacterRepository characterRepository)
    {
        _characterRepository = characterRepository;
    }

    public CharacterModel Create(string characterId)
    {
        CharacterData data = new CharacterData();
        CharacterSaveData saveData = CharacterSaveData.New(characterId);
        
        return new CharacterModel(data, saveData);
    }
}