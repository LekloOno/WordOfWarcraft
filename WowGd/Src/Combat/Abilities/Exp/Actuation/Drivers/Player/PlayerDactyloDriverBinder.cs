namespace WowGd.Src.Combat.Abilities.Exp.Actuation.Drivers.Player;

public static class WordBinder
{
    public static void Bind(this IPlayerDactyloDriverHandler handler, PlayerDactyloDriver driver)
    {
        driver.WordsInitialized += handler.OnWordsInitialized;
        driver.WordPushed       += handler.OnWordPushed;
        driver.WordCompleted    += handler.OnWordCompleted;
        driver.Stopped          += handler.OnStopped;
    }

    public static void Unbind(this IPlayerDactyloDriverHandler handler, PlayerDactyloDriver driver)
    {
        driver.WordsInitialized -= handler.OnWordsInitialized;
        driver.WordPushed       -= handler.OnWordPushed;
        driver.WordCompleted    -= handler.OnWordCompleted;
        driver.Stopped          -= handler.OnStopped;
    }
}