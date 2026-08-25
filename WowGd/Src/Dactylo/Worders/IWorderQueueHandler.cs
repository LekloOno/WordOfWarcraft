using WowGd.Src.Dactylo.Generators;

namespace WowGd.Src.Dactylo.Worders;

public interface IWorderQueueHandler
{
    void OnWordEnqueued(Word word);
}