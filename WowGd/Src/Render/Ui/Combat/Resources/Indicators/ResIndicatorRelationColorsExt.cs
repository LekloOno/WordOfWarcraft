using System;
using WowGd.Src.Combat.Abilities.Targeting.Payload;

namespace WowGd.Src.Render.Ui.Combat.Resources.Indicators;

public static class ResIndicatorRelationColorsExt
{
    public static T GetColor<T>(this IResIndicatorRelationColors<T> self, TargetRelation relation)
        where T: IResIndicatorColor =>
    relation switch
    {
        TargetRelation.Self => self.Self,
        TargetRelation.Ally => self.Ally,
        TargetRelation.Enemy => self.Enemy,
        _ => throw new IndexOutOfRangeException(),
    };
}