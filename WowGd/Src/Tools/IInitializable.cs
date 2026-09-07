using System.Threading.Tasks;

namespace WowGd.Src.Tools;

public interface IInitializable
{
    Task Initialization { get; }
}