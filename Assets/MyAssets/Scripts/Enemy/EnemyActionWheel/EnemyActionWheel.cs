using UnityEngine;

public static class EnemyActionWheel
{
    public struct WheelSlice
    {
        public EnemyActionType Action;
        public float StartAngle;
        public float EndAngle;
    }

    public static EnemyActionType PickAction(EnemyData data)
    {
        data.GetEffectiveWeights(out float attack, out float miss,
            out float heal, out float support);

        float total = attack + miss + heal + support;
        float roll = Random.Range(0f, total);

        if (roll < attack) return EnemyActionType.Attack;
        roll -= attack;
        if (roll < miss) return EnemyActionType.Miss;
        roll -= miss;
        if (roll < heal) return EnemyActionType.Heal;
        return EnemyActionType.Support;
    }

    public static WheelSlice[] BuildSlices(EnemyData data)
    {
        data.GetEffectiveWeights(out float attack, out float miss,
            out float heal, out float support);

        float total = attack + miss + heal + support;
        float attackDeg = attack / total * 360f;
        float missDeg = miss / total * 360f;
        float healDeg = heal / total * 360f;
        float supportDeg = support / total * 360f;

        return new WheelSlice[]
        {
            new WheelSlice { Action = EnemyActionType.Attack,
                StartAngle = 0f, EndAngle = attackDeg },
            new WheelSlice { Action = EnemyActionType.Miss,
                StartAngle = attackDeg, EndAngle = attackDeg + missDeg },
            new WheelSlice { Action = EnemyActionType.Heal,
                StartAngle = attackDeg + missDeg,
                EndAngle = attackDeg + missDeg + healDeg },
            new WheelSlice { Action = EnemyActionType.Support,
                StartAngle = attackDeg + missDeg + healDeg, EndAngle = 360f }
        };
    }

    public static float GetLandingAngle(WheelSlice[] slices, EnemyActionType action)
    {
        foreach (var slice in slices)
            if (slice.Action == action)
                return Random.Range(slice.StartAngle, slice.EndAngle);
        return 0f;
    }
}