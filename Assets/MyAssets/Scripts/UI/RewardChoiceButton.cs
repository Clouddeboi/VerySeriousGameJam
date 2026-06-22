using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardChoiceButton : MonoBehaviour
{
    public TMP_Text NameText;
    public Button Button;

    private RewardData reward;
    private Action<RewardData> onClick;

    public void Setup(RewardData data, Action<RewardData> callback)
    {
        reward = data;
        onClick = callback;
        NameText.text = data.DisplayName;

        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(() => onClick?.Invoke(reward));
    }
}