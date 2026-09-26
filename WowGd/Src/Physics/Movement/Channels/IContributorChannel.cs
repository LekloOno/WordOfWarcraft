namespace WowGd.Src.Physics.Movement.Channels;

/// <summary>
/// Describe a channel of movement contributors, and how such contributors' contributions are aggregated.
/// 
/// For example, a simple list that combines all contributions additively.
/// </summary>
public interface IContributorChannel
{
    Contribution GetContribution(EntityMover mover, float delta);

    void Close();
    void Open();

    bool AddContributor(IContributor contributor, uint priority);
    bool RemoveContributor(IContributor contributor);
}