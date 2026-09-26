- [Concepts de base](#concepts-de-base)
  - [Physique d'une entité](#physique-dune-entité)
    - [Friction](#friction)
      - [Fonctionnement de la friction](#fonctionnement-de-la-friction)
  - [Contributeurs](#contributeurs)
  - [Canaux de contribution](#canaux-de-contribution)
    - [Exemples](#exemples)
      - [AdditiveChannel](#additivechannel)
      - [D'autres exemples divers](#dautres-exemples-divers)
  - [Gestionnaire de canaux](#gestionnaire-de-canaux)
  - [Canaux et interractions](#canaux-et-interractions)
    - [Canaux (MovementChannels)](#canaux-movementchannels)
      - [God Channel](#god-channel)
      - [Internal Channel](#internal-channel)
      - [External Channel](#external-channel)
      - [Warp Channel](#warp-channel)
    - [Etats (MovementStatus)](#etats-movementstatus)
      - [Airborne](#airborne)
      - [Anchored](#anchored)
- [InternalChannel](#internalchannel)
  - [InternalContributor](#internalcontributor)
  - [IInternalLayer](#iinternallayer)
  - [Règles de layer actif](#règles-de-layer-actif)
  - [Sémantique de priorité](#sémantique-de-priorité)
- [Gestion des états (StatusQueryRegister)](#gestion-des-états-statusqueryregister)
  - [Registre de requêteurs](#registre-de-requêteurs)
  - [Registre de requêtes](#registre-de-requêtes)
  - [Liens avec les canaux](#liens-avec-les-canaux)
- [Détails de contrats \& d'implémentations](#détails-de-contrats--dimplémentations)
  - [RunChannels](#runchannels)
    - [Remove Queue](#remove-queue)
  - [Cycle de vie d'un contributeur](#cycle-de-vie-dun-contributeur)
    - [AddContributor strictness](#addcontributor-strictness)
    - [AddContributor/RemoveContributor result](#addcontributorremovecontributor-result)
    - [Exemples d'usage](#exemples-dusage)

# Concepts de base

## Physique d'une entité

La physique d'une entité fonctionne fondamentalement à partir de deux sources de mouvements - l'**inertie**, et les **forces brutes**. Voir [IBody](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/IBody.cs).

L'**inertie** est une force qui peut évoluer dans le temps soit par accélération extérieure (avec `Accelerate`) soit par effet de la simulation physique elle-même - Par exemple, une collision avec un mur pourrait stopper l'inertie. Elle n'est jamais modifiée directement, autrement que par ces deux biais.

Les **forces brutes** sont des forces, indépendantes de l'inertie, qui s'appliquent strictement à l'entité, construites et consommées pour chaque tick.
C'est un buffer, vidé à chaque nouveau tick, rempli par l'extérieur avec `AddRawForce`.

L'inertie et les forces brutes sont combinées pour former la vélocité finale de l'entité à chaque tick.
> Elles sont donc toutes deux "delta-relative" puisque la vélocité l'est : pour déplacer une entité, à chaque tick, on la déplace de sa `vélocité * Δt` et ici `vélocité = forces brutes + inertie`.

### Friction

Les entités standards intègrent généralement un méchanisme de friction. C'est simplement une autre modification de *l'inertie*, généralement donc une *décélération*.

> Pour expliquer pleinement le fonctionnement de la friction, il faut mentionner le canal interne de mouvement (Internal).  
> Je reste volontairement très vague sur ce qu'est un canal, celui-ci en particuliers. On rentrera dans les détails des canaux par la suite.

#### Fonctionnement de la friction

La friction est multiplicative. Elle est obtenue selon le schéma suivant :

1. Le **facteur** de friction de base est [récupéré via le canal de mouvements interne](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/EntityMover.cs#L82) (Internal).

2. Ensuite, chaque contributeur peut [apporter un modificateur multiplicatif](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/EntityMover.cs#L86-L87). Cette nature multiplicative est définie par la fonction `Add` de [Contribution](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Contribution.cs).

3. La friction finale de cette aggrégation est utilisée pour [calculer la **force** de friction initiale](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/EntityMover.cs#L103).

4. Pour des raisons peu évidentes à expliquer sans rentrer dans des détails très spécifiques* (voir la note en bas), cette force de friction est [réduite si les conditions le permettent](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/EntityMover.cs#L111-L115).

5. La friction résultante peut être [ajoutée à l'inertie](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/EntityMover.cs#L94-L95).

> \* Le problème vient du fait que côté game design, on aimerait pouvoir définir le taux de friction et la vitesse d'une entité de manière indépendante.  
> En pratique, ces deux sont pourtant inhéremment dépendantes : la friction réduit ultimement la vitesse.  
> Ce qu'on aimerait, c'est qu'une plus haute friction rende simplement les mouvements plus snappy, sans pour autant impacter la vitesse.  
> Donc pour pleinement traduire cette intention de GD, il faut la prendre en compte explicitement dans l'implémentation de la friction.  
> Ici, la solution implémentée est de calculer la portion "ignorable" de la friction, selon l'inertie actuelle de l'entité et son intention de mouvements, et de retirer cette portion de la friction finale.


## Contributeurs

La finalité du système, ce qui produit du mouvement, ce sont les [IContributor(s)](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/IContributor.cs).

Leur rôle est de produire une [Contribution](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Contribution.cs) de mouvement.

Ils peuvent contribuer à -
- **l'inertie**, de manière additive.
- **la force brute**, de manière additive.
- **la friction**, de manière multiplicative, un facteur.

En plus de ça, ils peuvent définir des hooks pour réagir à l'ouverture/fermeture du canal dans lequel ils sont enregistrés.

## Canaux de contribution

Ces contributeurs sont placés dans des [IContributorChannel](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/IContributorChannel.cs).

Le rôle d'un canal de contribution est de définir -
1. `AddContributor(... uint priority)` - Comment il organise ses contributeurs.
2. `AddContributor`/`RemoveContributor` - Comment il reçoit ou relâche des contributeurs.
3. `GetContribution` - Comment il agrège les contributions de ses contributeurs.
4. `Close`/`Open` - Ce qu'il doit faire vis à vis de ses contributeurs quand il est fermé/ouvert.

La notion de priorité `priority` n'est pas (nécessairement) numérique, elle ne doit pas être interprété comme un entier, un poids, mais plutôt comme une signature de bits générique. C'est au type de canal de définir exactement la sémantique de cette signature.

Voir par exemple [InternalPriority](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Internal/InternalPriority.cs).

### Exemples

#### [AdditiveChannel](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Additive/AdditiveChannel.cs)

Le type de canal implémenté le plus simple est l'[AdditiveChannel](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Additive/AdditiveChannel.cs).

1. Il organise ses contributeurs sous forme d'une liste non ordonnée (d'où l'optimisation du [swap remove](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Additive/AdditiveChannel.cs#L36-L46)) - l'indexation n'a pas d'importance donc `priority` n'a aucune sémantique.

2. Il ajoute et retire ses contributeurs simplement en les ajoutant/retirant de la liste.

3. Son résultat de contribution, c'est simplement l'addition des contributions de tous ses contributeurs, sans discrimination particulière.

4. Lorsqu'il est fermé/ouvert, il en notifie tous ses contributeurs.

#### D'autres exemples divers

On pourrait imaginer d'autres cas -
- Priorité numérique - un canal qui ne prend toujours la contribution que d'un contributeur, celui qui a la plus haute priorité numérique, `priority` serait alors bien interprété comme un entier dans ce cas.
- Stack - un canal qui prends ses contributeur en pile, c'est celui au sommet qui donne sa contribution. `priority` n'aurait donc pas de sémantique, ou bien pourquoi pas permettre de se protéger des `x` prochains empilements ?..
- ...

J'expliquerai plus loin le fonctionnement d'un [InternalChannel](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Internal/InternalChannel.cs), qui lui est beaucoup plus avancé, et donne une sémantique très précise à `priority` ([InternalPriority](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Internal/InternalPriority.cs)).

## Gestionnaire de canaux

Le gestionnaire des canaux, c'est le [IEntityMover](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/IEntityMover.cs). C'est à lui que sont envoyés les requêtes d'[ajout](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/IEntityMover.cs#L20)/[retrait](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/IEntityMover.cs#L21) de contributeurs, pour un canal donné, et c'est lui qui gère l'ouverture/fermeture de ces différents canaux au travers de son [StatusQueryRegister](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Status/StatusQueryRegister.cs).

L'implémentation actuelle est définie dans [EntityMover](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/EntityMover.cs). On reviendra en détail dessus ensuite.

## Canaux et interractions

### Canaux ([MovementChannels](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/MovementChannels.cs))

On peut admettre autant de canaux que souhaités, l'architecture est générique et souple à ce sujet, mais actuellement, il y a 4 canaux explicites.

#### God Channel

C'est simplement un canal qui ne peut jamais être fermé.

#### Internal Channel

Dans l'idée, ce sont les "pieds" (miam) de l'entité. Ce sont les mouvements physiques provoqués en interne de l'entité.

Ce n'est pas forcément des mouvements intentionnels pour autant - par exemple, un "fear" (capacité qui fait qu'une entité court dans tous les sens, elle n'a plus le contrôle) serait logiquement un contributeur de ce canal, qui prendrait le dessus sur les mouvements de bases du joueur.

Globalement, on s'attend à y retrouver -
- Des effets comme un fear.
- Le suivi de tâcle (quand une entité en tâcle une autre, elle le suit automatiquement)
- Les mouvements de base de l'entité.

Parce que ces trois là sont les "pieds" de l'entité.

#### External Channel

Il s'agit de forces physiques externes, comme une poussée/tirée.  
Une explosion, un vent, un syphon, etc.

#### Warp Channel

Il s'agit de forces magiques. L'idée est de pouvoir discriminer différentes sources de forces externes.
Par exemple, dans wakfu, le fait d'être stabilisé empêche d'être poussé/tiré, mais pas d'être téléporté.

On pourrait donc définir une téléportation comme un contributeur du canal Warp. Si le canal externe est fermé, mais pas le canal warp, la téléportation fonctionne donc toujours.


### Etats ([MovementStatus](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Status/MovementStatus.cs))

Les états sont un moyen d'abstraire avec des idées purement "game design" la manière dont on souhaite interragir avec les canaux.

Des notions explicite de jeux. Des outils que le joueur manipulera directement.

Les canaux de mouvements sont plus une approche technique, bien qu'ils introduisent déjà quelques concepts de jeux en soit.

Les trois premiers états sont par exemple implicitement des mapping directs des canaux de mouvements.
- **Immobilized** ferme le canal interne.
- **Stabilized** ferme le canal externe.
- **Unwarped** ferme le canal warp.

Les deux états suivants sont moins directs.

#### Airborne

Une cible peut être considérée comme "dans les airs".
Celà à pour effet de modifier le contributeur utilisé par le canal interne, en effet, celui-ci définit des contributeurs liés au mouvements au sol, ainsi que des contributeurs liés aux mouvements dans les airs.

#### Anchored

C'est un override de **airborne**. Le canal interne n'utilise ses contributeurs aériens que si l'entité est dans les airs (airborne) **ET** n'est pas anchrée (anchored).

# [InternalChannel](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Internal/InternalChannel.cs)

Le canal interne est un canal à la structure et sémantique très particulière.

Il gère deux choses - la séparation air/sol, et la séparation en layer de priorités.

```
                    IEntityMover
                        |
                    +---+-------+---+---+---+---+-..
                    |               |   |   |   |   |
                    |               .. other channels ..
            Internal Channel        
                    |
+-------------------+-------------------+     current_solver
| Air               | Ground            |      +----+----+
+-------------------+-------------------+           |
|     - contribution solvers -          |           |
+-------------------+-------------------+           |
| ...               | ...               |           |
| ? Air Override    | ? Ground Override |           |
| ? Air Tackle      | ? Ground Tackle   |  <--------+
|   Air Base        |   Ground Base     |
+-------------------+-------------------+
```

De base, la plupart des entités n'auront par exemple pas ou peu de contrôle dans les airs, le fait d'être projeté dans les airs constituera donc pour elles un crowd-control, au même titre qu'un root, stun, etc.

## [InternalContributor](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Internal/InternalContributor.cs)

Le contrôle aérien comme au sol ont la même structure - un [InternalContributor](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Internal/InternalContributor.cs).

Ces `InternalContributor`s organisent des [`IInternalLayer`](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Internal/IInternalLayer.cs) ([`InternalLayer`](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Internal/InternalLayer.cs)).

## [IInternalLayer](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Internal/IInternalLayer.cs)

On a plus tôt mentionné -
> "Globalement, on s'attend à y retrouver -
> - Des effets comme un fear.
> - Le suivi de tâcle (quand une entité en tâcle une autre, elle le suit automatiquement)
> - Les mouvements de base de l'entité."

Ces layers correspondent à ces attentes -
- **override** -> fear et autres.
- **tackle** -> suivi de tâcle.
- **base** -> mouvements de base.

> En soit, rien ne nous empêcherait de laisser ces layers plus ouverts.  
> En particuliers, on pourrait peut-être bénéficier de définir "override" comme une pile de layer, plutôt qu'un layer unique.  
> Actuellement, le dernier override est toujours le gagnant, et écrase le précédent, s'il y en avait un.

Un layer est aussi responsable de définir
- une friction de base.
- une "vitesse maximum" d'intention `MaxSpeed` (c'est une valeur purement indicative, pas de contrat particuliers quant aux contributions que peut produire un layer).
- une "intention de direction" `WishDir`.

Ces propriétés sont utilisées pour le calcul de [friction](#friction) d'une entité. 

## Règles de layer actif

Il y a toujours un unique layer actif à la fois, selon les règles suivantes -
1. L'`InternalContributor` actif dépend de -
   - Si elle n'est pas **airborne** ou qu'elle est **anchored** -> **ground**.
   - Sinon -> **air**.
2. Le layer actif au seint de cet `InternalContributor` dépend de -
   - S'il y a un layer override -> **override**.
   - Sinon, s'il y a un layer tackle -> **tackle**.
   - Sinon -> **base** (il y a toujours une base).

Dans ce système, le [BodyMover](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Internal/BodyMover.cs) est donc devenu un `InternalLayer`. 

## Sémantique de priorité

Le layer interne offre donc une interface pour interpréter et former explicitement un `uint priority` permettant d'interragir avec cette architecture de canal, [InternalPriority](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Internal/InternalPriority.cs).

En combinant les bons flags, on peut indiquer si le contributeur que l'on souhaite ajouter/retirer concerne les mouvements aériens ou au sol (`InternalPriority.Grounded`), et s'il concerne la base (`InternalPriority.Base`), le tâcle (`InternalPriority.Tackle`) ou l'override (`InternalPriority.Override`).

Les interprétations sont outillées avec [InternalPriorityExt](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Internal/InternalPriorityExt.cs), par exemple utilisées dans l'implémentation du [`AddContributor`](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Internal/InternalChannel.cs#L33-L69).

Et le `BodyMover` s'enregistre [par exemple ici](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/Internal/BodyMover.cs#L30-L33) avec cette sémantique.


# Gestion des états ([StatusQueryRegister](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Status/StatusQueryRegister.cs))

La façon dont les états interragissent avec les canaux de mouvements découle du fonctionnement [StatusQueryRegister](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Status/StatusQueryRegister.cs).

Son fonctionnement est plutôt simple.

Tous les états sont par défaut inactifs. N'importe qui peut demander à lever/relâcher des [MovementStatus](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Status/MovementStatus.cs) via [`Query`](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Status/StatusQueryRegister.cs#L17-L26)/[`Unquery`](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Status/StatusQueryRegister.cs#L28-L37).

## Registre de requêteurs

Quand un requêteur demande à lever un état, il est enregistré dans un [registre de requêteurs](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Status/StatusQueryRegister.cs#L10).

Ce registre de requêteurs permet de garantir deux choses :
1. Un requêteur ne peut faire qu'une requête à la fois.
2. Une requête est toujours annulée de manière atomique - ce qui est annulé correspondra toujours effectievement à ce qui a été fait, et il n'est pas possible d'annuler quelque chose qui n'a pas été fait.

Ainsi :
1. Une nouvelle requête est [refusée si le requêteur est déjà présent](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Status/StatusQueryRegister.cs#L19-L20) dans le registre.
2. L'annulation de requête se fait par simple identité du requêteur. Si le requêteur n'est pas présent dans le registre, [rien n'est fait](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Status/StatusQueryRegister.cs#L30-L31). Sinon, on peut retrouver la requête qui avait été [faîtes par identité, et l'annuler](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Status/StatusQueryRegister.cs#L30-L34).

## Registre de requêtes

Comme dit précedemment, les états finaux sont par défaut tous désactivés : tous les flag du [`MovementStatus` actif](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Status/StatusQueryRegister.cs#L9) sont à `0`.

Pour chaque flag, le `StatusQueryRegister` associe un [compteur de requêtes](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Status/StatusQueryRegister.cs#L11-L12).

`Query` et `Unquery` des `MovementStatus`, c'est donc respectivement incrémenter et décrémenter les compteurs correspondant.

Le `MovementStatus` final résultant est donc un dérivé de ces compteurs : Si un compteur est à 1 ou plus, le flag d'état correspondant est à `1`, sinon, il est à `0`.

Le tout est maintenu de manière dynamique avec [des évènements](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Status/StatusQueryRegister.cs#L14-L15), castés lorsqu'un changement d'état se produit.

> Pour comprendre d'où sort l'itérateur sur les bitflags, c'est une petite singerie de manipulation de bits que j'ai vomi et externalisé en [ce petit outil](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Tools/SetBitEnumerator.cs), puisque fréquemment utilisé dans tout le système de mouvement.  
> Prenez en plein les yeux ......


## Liens avec les canaux

Le [`EntityMover`](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/EntityMover.cs#L38-L39) peut donc réagir aux signaux émis par son `StatusQueryRegister`.

Il ouvre/ferme les canaux associés, et met à jour son canal interne - [voir ici](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/EntityMover.cs#L50-L78).


# Détails de contrats & d'implémentations

## [RunChannels](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/EntityMover.cs#L80-L96)

`RunChannels` est la fonction principale du `EntityMover`, qui réalise l'aggrégation de tous ses canaux actifs pour construire la contribution finale. Elle est appelé à [chaque tick par son entité](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Entities/Entity.cs#L97-L100).

### [Remove Queue](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/EntityMover.cs#L159-L161)

Dans cette fonction, on peut voir les lignes.
```cs
while (_removeQueued.TryDequeue(out (MovementChannels, IContributor) queued))
    RemoveContributor(queued.Item1, queued.Item2);
```

Cette [queue](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/EntityMover.cs#L159-L161) permet aux contributeurs de s'auto-désabonner à un canal, en réaction à la récupération de leur contribution.

Puisque l'implémentation exacte de l'organisation des canaux de mouvements est libre, elle peut donc aussi être une structure de donnée pour laquelle il ne serait pas safe de retirer des éléments alors que l'on itère dedans - par exemple, les `AdditiveChannels`.

Si un contributeur veut donc se désabonner en réaction à une contribution, il peut utiliser `QueueRemoveContributor` en toute sécurité.

C'est ce que fait par exemple le Dash quand [sa durée est écoulée](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Combat/Abilities/Launch/EffectsHolders/Effects/Dash.cs#L26-L27).


## Cycle de vie d'un contributeur

### [AddContributor](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/EntityMover.cs#L120) strictness
Pour ajouter un contributeur, il est possible de préciser la "strictness" d'ajout.

Un ajout strict indique que le contributeur ne devrait être ajouté [que si le canal de destination est déjà ouvert](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/EntityMover.cs#L127-L130).

Celà permet de définir une abilité qui ne prendrait par exemple tout simplement pas effet si son canal est entravé.

### [AddContributor](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/EntityMover.cs#L120)/[RemoveContributor](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/EntityMover.cs#L142) result

D'autres part, la réponse d'un ajout/retrait de contributeur est précise, permettant d'en dériver des comportements variés.

C'est un [ChannelSubResult](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/ChannelSubResult.cs) :
- Success signifie que le contributeur a bien été ajouté.
- ChannelOpenend signifie que le canal cible était bien ouvert.

### Exemples d'usage

Combiné avec les hook `OnChannelClosed`/`OnChanelOpened` du [IContributor](https://github.com/LekloOno/WordOfWarcraft/blob/main/WowGd/Src/Physics/Movement/Channels/IContributor.cs), cette strictness et result permettent de définir des cycles de vie variés.

Par exemple, on pourrait définir un mouvement qui, si son canal destination (donc probablement le canal externe) est fermé, s'ajoute malgré tout, et attend 5 secondes pour s'activer.

Si dans les 5 secondes, le canal est ouvert, le mouvement se lance, sinon, le mouvement est annulé, il se retire du canal.

```cs
void Initialize(IEntityMover mover)
{
    // avec strict, on ajoute le contributeur quoi qu'il arrive.
    ChannelSubResult result = mover.AddContributor(MovementChannels.External, this, priority, strict: false);

    // Le mouvement ne pouvait pas être ajouté.
    if (!result.HasFlag(ChannelSubResult.Sucess))
        return;

    bool instantStart = result.HasFlag(ChannelSubResult.ChannelOpened);
    if (instantStart)
        Start();
    else
    {
        // simplified timer mechanism for illustration sake
        Timer = new Timer(5f);
        Timer.Timeout += Cancel;
    }
}

void OnChannelOpened()
{
    if (!instantStart)
        Start();
}

void Cancel() =>
    mover.RemoveContributor(MovementChannels.External, this);

void Start()
{
    //...
}

// ...
```