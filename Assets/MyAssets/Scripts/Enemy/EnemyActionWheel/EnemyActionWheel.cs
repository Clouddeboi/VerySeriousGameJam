using UnityEngine;

public static class EnemyActionWheel
{
    //Returns the chosen action AND the angle range it occupies, so the visual wheel can match exactly
    public struct WheelSlice
    {
        public EnemyActionType Action;
        public float StartAngle;
        public float EndAngle;
    }

    public static EnemyActionType PickAction(EnemyData data)
    {
        float total = data.AttackWeight + data.MissWeight + data.HealWeight;
        float roll = Random.Range(0f, total);

        if (roll < data.AttackWeight) return EnemyActionType.Attack;
        roll -= data.AttackWeight;

        if (roll < data.MissWeight) return EnemyActionType.Miss;

        return EnemyActionType.Heal;
    }

    //Builds the pie slices in a fixed order (Attack, Miss, Heal) so the visual wheel layout is consistent
    public static WheelSlice[] BuildSlices(EnemyData data)
    {
        float total = data.AttackWeight + data.MissWeight + data.HealWeight;
        float attackDeg = data.AttackWeight / total * 360f;
        float missDeg = data.MissWeight / total * 360f;
        float healDeg = data.HealWeight / total * 360f;

        return new WheelSlice[]
        {
            new WheelSlice { Action = EnemyActionType.Attack, StartAngle = 0f, EndAngle = attackDeg },
            new WheelSlice { Action = EnemyActionType.Miss, StartAngle = attackDeg, EndAngle = attackDeg + missDeg },
            new WheelSlice { Action = EnemyActionType.Heal, StartAngle = attackDeg + missDeg, EndAngle = 360f }
        };
    }

    //Given a chosen action, finds the matching slice and returns a random angle within it,
    //this is the angle the spinning hand needs to land on for the visual to match the logical result
    public static float GetLandingAngle(WheelSlice[] slices, EnemyActionType chosenAction)
    {
        foreach (var slice in slices)
        {
            if (slice.Action == chosenAction)
                return Random.Range(slice.StartAngle, slice.EndAngle);
        }
        return 0f;
    }
}