using WowGd.Src.Dactylo.Generators;

namespace WowGd.Src.Combat.Abilities.Actuation.Drivers.Player;

public interface IPlayerDactyloDriverHandler
{
    void OnWordsInitialized(WordRequest req, Word[] words);
    void OnWordPushed(WordRequest req, Word word);
    void OnWordCompleted(WordRequest req, Word word);
    void OnStopped();
}