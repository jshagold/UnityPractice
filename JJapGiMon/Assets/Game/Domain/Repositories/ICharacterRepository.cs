public interface ICharacterRepository
{
    CharacterSaveData Load(string characterId);

    void Save(CharacterSaveData saveData);
}