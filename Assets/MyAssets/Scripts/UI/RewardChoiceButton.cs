using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardChoiceButton : MonoBehaviour
{
    public TMP_Text NameText;
    public Button Button;
    public Image ButtonImage;

    private RewardData reward;
    private Action<RewardData> onClick;

    public void Setup(RewardData data, Action<RewardData> callback)
    {
        reward = data;
        onClick = callback;
        NameText.text = data.DisplayName;

        if (data.Icon != null)
            ButtonImage.sprite = data.Icon;

        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(() => onClick?.Invoke(reward));
    }
}