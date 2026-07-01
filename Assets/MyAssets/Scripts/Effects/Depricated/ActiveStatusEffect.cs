using UnityEngine;

[System.Serializable]
public class ActiveStatusEffect
{
    public StatusEffectType Type;
    public int RemainingTurns;
    public int Magnitude; //Damage per tick for Burn, bonus dmg for Shock
}