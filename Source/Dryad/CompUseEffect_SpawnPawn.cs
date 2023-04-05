using RimWorld;
using RimWorld.Planet;
using Verse;

namespace Dryad;

public class CompUseEffect_SpawnPawn : CompUseEffect
{
    public override float OrderPriority => 1000f;

    public CompProperties_SpawnPawn SpawnerProps => props as CompProperties_SpawnPawn;

    public virtual void DoSpawn(Pawn usedBy)
    {
        var pawn = PawnGenerator.GeneratePawn(SpawnerProps.pawnKind);
        if (pawn == null)
        {
            return;
        }

        pawn.SetFaction(GetFaction());
        GenPlace.TryPlaceThing(pawn, parent.Position, parent.Map, ThingPlaceMode.Near);
        if (SpawnerProps.sendMessage)
        {
            Messages.Message("Dryad awakes".Translate(pawn.Name.ToStringFull), new GlobalTargetInfo(pawn),
                MessageTypeDefOf.NeutralEvent);
        }
    }

    public override void DoEffect(Pawn usedBy)
    {
        base.DoEffect(usedBy);
        for (var i = 0; i < SpawnerProps.amount; i++)
        {
            DoSpawn(usedBy);
        }
    }

    public Faction GetFaction()
    {
        if (SpawnerProps.usePlayerFaction)
        {
            return Faction.OfPlayer;
        }

        return SpawnerProps.forcedFaction == null
            ? parent.Faction
            : FactionUtility.DefaultFactionFrom(SpawnerProps.forcedFaction);
    }
}