using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Dryad;

public class HediffComp_SpawnOnDeathAnimalWild : HediffComp
{
    public HediffCompProperties_SpawnOnDeathAnimalWild Props => (HediffCompProperties_SpawnOnDeathAnimalWild)props;

    public override void Notify_PawnDied()
    {
        if (Props.destroyBody)
        {
            Pawn.Corpse.Destroy();
        }
    }

    public override void Notify_PawnKilled()
    {
        if (Props.mote != null || Props.fleck != null)
        {
            var drawPos = Pawn.DrawPos;
            for (var i = 0; i < Props.moteCount; i++)
            {
                var vector = Rand.InsideUnitCircle * Props.moteOffsetRange.RandomInRange * Rand.Sign;
                var vector2 = new Vector3(drawPos.x + vector.x, drawPos.y, drawPos.z + vector.y);
                if (Props.mote != null)
                {
                    MoteMaker.MakeStaticMote(vector2, Pawn.Map, Props.mote);
                }
                else
                {
                    FleckMaker.Static(vector2, Pawn.Map, Props.fleck);
                }
            }
        }

        if (Props.filth != null)
        {
            FilthMaker.TryMakeFilth(Pawn.Position, Pawn.Map, Props.filth, Props.filthCount);
        }

        Props.sound?.PlayOneShot(SoundInfo.InMap(Pawn));
        var pawn = PawnGenerator.GeneratePawn(Props.pawnKind);
        if (pawn == null)
        {
            return;
        }

        pawn.SetFaction(GetFaction());
        GenPlace.TryPlaceThing(pawn, Pawn.Position, Pawn.Map, ThingPlaceMode.Near);
    }

    public Faction GetFaction()
    {
        if (Props.usePlayerFaction)
        {
            return FactionUtility.DefaultFactionFrom(Props.forcedFaction);
        }

        return Props.forcedFaction == null ? Faction.OfPlayer : FactionUtility.DefaultFactionFrom(Props.forcedFaction);
    }
}