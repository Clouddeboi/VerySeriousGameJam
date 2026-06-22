using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RewardChoiceUI : MonoBehaviour
{
    public GameObject PanelRoot;
    public RewardChoiceButton[] ChoiceButtons;

    private Action<RewardData> onChosen;

    public void Show(List<RewardData> choices, Action<RewardData> onChosenCallback)
    {
        onChosen = onChosenCallback;
        PanelRoot.SetActive(true);

        for (int i = 0; i < ChoiceButtons.Length; i++)
        {
            if (i < choices.Count)
            {
                ChoiceButtons[i].gameObject.SetActive(true);
                ChoiceButtons[i].Setup(choices[i], HandleChoice);
            }
            else
            {
                ChoiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void HandleChoice(RewardData reward)
    {
        PanelRoot.SetActive(false);
        onChosen?.Invoke(reward);
    }
}