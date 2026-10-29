// One alias per domain, so core code reads "RequestTagSet" instead of "TagSet<RequestTagDomain>".
// (Global usings do not cross project boundaries: the game and the tests declare their own.)
global using RequestTag = CombatCore.Primitives.Tags.Tag<CombatCore.Primitives.Tags.RequestTagDomain>;
global using RequestTagSet = CombatCore.Primitives.Tags.TagSet<CombatCore.Primitives.Tags.RequestTagDomain>;
global using GrantTag = CombatCore.Primitives.Tags.Tag<CombatCore.Primitives.Tags.GrantTagDomain>;
global using GrantTagSet = CombatCore.Primitives.Tags.TagSet<CombatCore.Primitives.Tags.GrantTagDomain>;
global using SourceTag = CombatCore.Primitives.Tags.Tag<CombatCore.Primitives.Tags.SourceTagDomain>;
global using SourceTagSet = CombatCore.Primitives.Tags.TagSet<CombatCore.Primitives.Tags.SourceTagDomain>;
