namespace WowGd.Src.Dactylo.Worders;

public interface IWorderStreamHandler
{
    void OnCompleted(string written, string target, int correct);
    void OnWordStarted(string word, int remainingWords);
}