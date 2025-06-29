using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayGame : MonoBehaviour
{
    public Button button;
    public static UnityAction OnPlayClick;
    private void OnEnable()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        OnPlayClick.Invoke();
    }
}
