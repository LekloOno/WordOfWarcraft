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

## Overview

The CombatCore is responsible for handling every combat related interractions that produce mutation of related systems.

It uses a pipeline system, that notably exposes interception mechanisms that can be used to apply modifications to generic behaviors.

The system receives a request draft, transforms it into an actual request and runs all interceptors that match this request.

These interceptors comes in different shapes :
- Replacements : replace the whole incoming request by another
- Modifications : modifies the incoming request (on the possibly replaced request, not the original)
- Additions : adds some requests to the queue (on the original request - to commit against the replaced request, interceptors can use the fact hook)

A request can be resolved, after replacement and modifications, into a fact.

A fact is a statement on the actual mutations of the subsequential systems.

The system will run all interceptors that match the fact, and then, broadcast the fact for silent listeners, like UI/UX elements.

### Request types

The system handles :
- One shot changes :
  - Resource requests (health, focus, stamina, cooldowns ..)
  - Movement requests

- Lifetime bound changes, through status requests :
  - Interceptors of the pipeline
  - Statistics changes
  - Context specific data (invocations, temporary resources, etc.)

The reason why these two are separated, is as mentionned, because some type of interractions are definitive, while others are a state that must be reversible.

#### One shots

For example, resource changes are immediate and definitive changes. Cases where we would want to revert such a change are exceptions that can be handled with explicit operations.  
Removing 10 mana removes 10 mana, end of the story. If you want a refund of such a cost, you could have a hook that looks for mana costs, and refund it.

Same goes for a movement request. It is instantaneous, and definite. Many movements can be expressed through a ponctual request, and contributors that needs a lifetime, like a time-duration dash, or a charge, is responsible for handling its own lifetime. For example, a charge that tries to reach a target should handle the target detection itself, and detach from the movement system independantly. The lifetime is part of the movement contribution's logic, since such logic can imply very complex interractions, that also dynamically change the nature of the contribution. Making movement contribution a primitive of another system that handles the lifetime would probably not be a helpfull abstraction.

#### Lifetime

But statistics changes or interceptors for example have an explicit lifetime that they don't handle internally, as the nature of a statistic modification or interception mechanism remains fairly simple.

Statistics should always be able to return to their base value, otherwise, there's a leak.  
So any statistic change is bound to a handle that must be kept and disposed properly.

Same goes for interceptors. Adding a special modifier that intercepts and modifies some requests, is - a special modifier. Which means again, if we can't return to the normal behavior, there's a leak.

Context specific data are also explictly temporary. If the player changes passive, class, cast a special spells, some states data should be dropped.

#### Status

So a status is a special data structure, that can hold these, and manages their lifetime.

Lifetime is either handled automatically, through its expiration model and/or level model, or manually through remove requests.

### Sources

Besides, the system handles sources in a rich way.

Sources can be expressed as a tree of sources, which kinds can be specified through tags.

For example, a spell is decomposed in multiple phases that can lead to an effect. Essentially, a spell releases launches, which eventually releases the trigger from effects holders, which release effects.

A spell itself has its base definition, but can run multiple concurrent instances of itself (through pending projectiles for example). So a spell has also spell instances.

So, the source tree of an effect could be -

```
System
├── Entity 1
│   ├── Spell 1
│   │   ├── Instance 1
│   │   │   ├── Launch 1
│   │   │   │   └── Effects Holder 1
│   │   │   │       ├── Effect 1
│   │   │   │       └── Effect 2
│   │   │   └── Launch 2
│   │   │       └── ...
│   │   └── Instance 2
│   │       └── ...
│   └── Spell 2
│       └── Instance 1
│           └── ...
└── Entity 2
    └── ...
```

Entity, Spell, etc, are shared unique tags, the number following would be an assigned source id.

Usual abilities sources would thus usually be an "Effect" tagged source, but a passive could be simply a "Passive" or "Status" tagged source, in thus a reduced source tree -
```
System
├── Entity 1
│   └── Passive 1
│       └── Status 1
│           └── Status Instance 1
... 
```

Sources are attached to the facts they produced. Which allows to express for example "from effect 1 fact, refund all costs of the related instance" which corresponds to tracking the tree up to its instance, and down to any effect tagged as a cost.

A source can be dropped when its lifetime is considered over. When an entity is dropped, when a status instance is dropped, when a spell has fully ended, etc.

Handling such lifetime is gonna be a challenge. A source can be dropped when its own drop signal has been triggered, and once all of its children have dropped too.
For example, an entity dying doesn't necessarilly mean the corresponding entity source is dropped. It might have pending projectiles that explictly states they should remain even when their entity is dead.

So defining how we handle this lifecycle is an important piece.

In an ideal world with no memory limit, we would drop no nodes, but that's of course not possible.

One hint is that "Instances" aren't just a different tag, but could be a special semantic of a source. Indeed, there's no reason to want to access what happened during an arbitrary X-th instance of a given spell/status/etc. If we take a refund passive on special event, trying to express a refund on an arbitrary instance seems odd, we would probably always want to refund some cost on the instance that triggered the event. This means whenever an instance ends, we can drop it once everything directly related to it and its consequences is resolved.

Non-instance sources on the other hand are probably rarely dropped as we might still want to know informations about structural sources.

Besides, to depict different dropping mechanism, maybe use an enum of signals to send to children, like a kill signal, that can be more or less restrictive. The less restrictive signal would let the children free when they can, and would wait for them to finally free itself, where a harder kill would force all children to instantaneously free.

Maybe there's inspiration to take from linux process system ?

Which would also suggest sources are more than simple nodes in a tree, but might have to carry the (possibly asynchronous) termination logic.

Sources isn't enough as is by the way.

For example, if we take a passive that provokes AoE damage whenever the player teleports, at the teleport destination point, the source of the damage would logically be the passive instance. But we might also want to know what teleport spell provoked it.

So we might want both a cause, and a source, both being ultimately source nodes. The source here would be the status instance (or any sub source in the tree of the status instance that holds the effect), while the cause would be the spell instance. Most if the time, the cause and the source would be the same, but not necessarilly.

Besides, a bit in depth of the "tags". The goal of tags is to allow to keep the set of source types open, while still allowing hooks to filter over them.
How we can represent tags is with a registry of unique strings, where each registered string is represented by a bit flag in a 64 bits mask (so 64 possible tags).
This would allow for very fast fetch, and "contains" or "exclude" checks.

To avoid the 64 tags limit, we could split a tag in a tree way. "Root.Node.Leaf" would be a tag, where each layer is a 64 bits mask, in a recursive way.
Maybe a tag itself would be the recursive structure. A tag is an index, associated to a unique string, and a subset of tags.

No - instead of recursive, we can have a growing set of mask chunks (a growing array of 64 bits masks, that "combines" in one infinitely long mask)