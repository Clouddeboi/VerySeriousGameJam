using System;
using System.Collections.Generic;
using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    public RewardData[] RewardPool;
    public CurrencySystem Currency;
    public Health PlayerHealth;
    public SlotMachineSystem SlotMachine;
    public RewardChoiceUI ChoiceUI;

    public List<RewardData> GenerateChoices(int count = 3)
    {
        var pool = new List<RewardData>(RewardPool);
        var choices = new List<RewardData>();

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = UnityEngine.Random.Range(0, pool.Count);
            choices.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return choices;
    }

    public void PresentChoices(Action onComplete)
    {
        var choices = GenerateChoices();
        ChoiceUI.Show(choices, reward =>
        {
            ApplyReward(reward);
            onComplete?.Invoke();
        });
    }

    public void ApplyReward(RewardData reward)
    {
        switch (reward.Type)
        {
            case RewardType.Gold:
                Currency.AddGold(reward.GoldAmount);
                break;
            case RewardType.Heal:
                PlayerHealth.Heal(reward.HealAmount);
                break;
            case RewardType.WeightAdjustment:
                if (reward.TargetElement != null)
                    SlotMachine.AdjustElementWeight(reward.TargetElement, reward.WeightDelta);
                else if (reward.TargetWeapon != null)
                    SlotMachine.AdjustWeaponWeight(reward.TargetWeapon, reward.WeightDelta);
                break;
        }
        Debug.Log($"[Reward] Applied: {reward.DisplayName}");
    }
}