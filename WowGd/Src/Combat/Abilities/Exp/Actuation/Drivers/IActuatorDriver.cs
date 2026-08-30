using System.Collections.Generic;
using System.Threading;

namespace WowGd.Src.Combat.Abilities.Exp.Actuation.Drivers;

/// <summary>
/// Part of the behavior of an IEntity, it corresponds to the way the entity interracts with actuators.
/// 
/// For a human player, it would typically a node that -
/// - fetches corresponding word bank to the request
/// - enables dactylography inputs
/// - enables specific UIs (or sub system can themselves enable )
/// - enter a word generation-dactylo sycle, and extract performance data from it
/// 
/// For an AI, it would simply be -
/// - eventually determine a stream rate modifier from word request word length, flavor, etc.
/// - start streaming packages at this rate.
/// </summary>
public interface IActuatorDriver
{
    IAsyncEnumerable<TypingPackage> StreamTypingAsync(WordRequest request, CancellationToken ct);
}