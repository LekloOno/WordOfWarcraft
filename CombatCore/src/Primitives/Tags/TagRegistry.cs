using System.Text;

namespace CombatCore.Primitives.Tags;

public static class TagRegistry<TDomain>
{
    private sealed class Snapshot(string[] names, int[] parents)
    {
        public readonly string[] Names = names;
        public readonly int[] Parents = parents;
    }

    private static readonly object Gate = new();
    private static readonly List<string> NameList = [];
    private static readonly List<int> ParentList = [];
    private static readonly Dictionary<string, int> ByName = new(StringComparer.Ordinal);
    private static HashSet<string>? _roots;
    private static bool _frozen;
    // Readers never lock: they read an immutable snapshot that is replaced after each registration.
    private static Snapshot _snapshot = new([], []);

    public static int Count => Volatile.Read(ref _snapshot).Names.Length;
    public static bool IsFrozen => Volatile.Read(ref _frozen);

    /// <summary>Optional typo guard: once declared, a name whose first segment is not a declared root is rejected ("Spel.Fire").</summary>
    public static void DeclareRoots(params string[] roots)
    {
        lock (Gate)
        {
            var set = new HashSet<string>(roots, StringComparer.Ordinal);
            foreach (var existing in NameList.Where(n => !n.Contains('.')))
                if (!set.Contains(existing))
                    throw new InvalidOperationException($"Tag '{existing}' is already registered as a root that is not declared.");
            _roots = set;
        }
    }

    public static Tag<TDomain> Register(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        lock (Gate)
        {
            if (ByName.TryGetValue(name, out var existing)) return new Tag<TDomain>(existing);
            if (_frozen) throw new InvalidOperationException($"Tag registry is frozen: '{name}' was not registered before Freeze().");

            var segments = name.Split('.');
            if (segments.Any(s => s.Length == 0)) throw new ArgumentException($"Invalid tag name '{name}': empty segment.", nameof(name));
            if (_roots is not null && !_roots.Contains(segments[0]))
                throw new InvalidOperationException($"Tag '{name}': root '{segments[0]}' is not declared.");

            int parent = -1;
            var prefix = new StringBuilder();
            foreach (var segment in segments)
            {
                if (prefix.Length > 0) prefix.Append('.');
                prefix.Append(segment);
                var path = prefix.ToString();
                if (!ByName.TryGetValue(path, out var index))
                {
                    index = NameList.Count;
                    NameList.Add(path);
                    ParentList.Add(parent);
                    ByName[path] = index;
                }
                parent = index;   // a parent is always registered before its children: parent index < child index
            }

            Volatile.Write(ref _snapshot, new Snapshot([.. NameList], [.. ParentList]));
            return new Tag<TDomain>(parent);
        }
    }

    /// <summary>
    /// The startup entry point: registers every name sorted ordinally, on an empty registry, so the bit layout only depends
    /// on the set of names.
    /// </summary>
    public static void RegisterAll(IEnumerable<string> names)
    {
        lock (Gate)
        {
            if (NameList.Count != 0)
                throw new InvalidOperationException("RegisterAll must run on an empty registry, or the layout would depend on registration order.");
            foreach (var name in names.Distinct(StringComparer.Ordinal).OrderBy(n => n, StringComparer.Ordinal))
                Register(name);
        }
    }

    public static void Freeze() { lock (Gate) _frozen = true; }

    public static bool TryFind(string name, out Tag<TDomain> tag)
    {
        lock (Gate)
        {
            var found = ByName.TryGetValue(name, out var index);
            tag = new Tag<TDomain>(found ? index : -1);
            return found;
        }
    }

    /// <summary>Hash of the sorted names (FNV-1a 64). Two processes with the same fingerprint share the same layout.</summary>
    public static ulong Fingerprint
    {
        get
        {
            ulong hash = 14695981039346656037UL;
            foreach (var name in Volatile.Read(ref _snapshot).Names.OrderBy(n => n, StringComparer.Ordinal))
            {
                foreach (var b in Encoding.UTF8.GetBytes(name)) { hash ^= b; hash *= 1099511628211UL; }
                hash ^= 0x0A; hash *= 1099511628211UL;
            }
            return hash;
        }
    }

    public static string NameOf(int index) => Volatile.Read(ref _snapshot).Names[index];
    public static int ParentOf(int index) => Volatile.Read(ref _snapshot).Parents[index];

    public static bool IsSameOrDescendant(int index, int ancestor)
    {
        var parents = Volatile.Read(ref _snapshot).Parents;
        for (var i = index; i >= 0; i = parents[i])
            if (i == ancestor) return true;
        return false;
    }

    /// <summary>Tests only. Each test should rather use its own marker type, which gives it a private registry.</summary>
    public static void ResetForTests()
    {
        lock (Gate)
        {
            NameList.Clear(); ParentList.Clear(); ByName.Clear();
            _roots = null; _frozen = false;
            Volatile.Write(ref _snapshot, new Snapshot([], []));
        }
    }
}