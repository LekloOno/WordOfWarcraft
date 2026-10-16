# CombatCore (no Godot reference on purpose)

Run the tests:  `cd tests && dotnet run`

## Reading order
1. `src/Primitives`                             - the vocabulary: ids, Role/Source/Origin, tags, BucketedValue.
2. `src/Messages.cs`                            - drafts (what an emitter asks), requests (stamped), facts (final, plain data).
3. `src/Interceptors.cs`                        - Replacement / Addition / Modifier + RequestFilter.
4. `src/Ports.cs`                               - everything the core needs from the game (clock, entities, resources, stats).
5. `src/Status.cs`                              - status definition/instance, level + time models, merge ops.
6. `src/SpellInstance.cs`                       - one run of a spell; stamps source + cause on every request.
7. `src/Authority.cs`                           - the pipeline: fuse -> replace -> add -> modify -> resolve -> publish.
8. `tests/Mock.cs`, `tests/Program.cs`          - fake entities (the ports) and the scenarios, readable as a spec.

## Deliberately NOT here yet
Movement requests/contributions, flag registers (Silenced/Dead), EndedBy / consequence contexts, WhileRunning (stackable
spells), per-entity subscription index, per-source status instances, network serialization.
