using System;
using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using VContainer;

public class PlayGame : MonoBehaviour
{
    public Image fill;

    public Button button;
    public TextMeshProUGUI currentLevelText;
    public static UnityAction OnPlayClick;
    [Inject] ILevelBuilder levelBuilder;

    private void Awake()
    {
        button.enabled = false;
        button.gameObject.SetActive(false);

    }
    private void OnEnable()
    {
        if (levelBuilder != null) levelBuilder.OnLevelGenerated += OnLevelGenerated;
        PlayerController.OnPlayerDied += OnPlayerDied;
        if (button != null) button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        if (levelBuilder != null) levelBuilder.OnLevelGenerated -= OnLevelGenerated;
        PlayerController.OnPlayerDied -= OnPlayerDied;

        if (button != null) button.onClick.RemoveListener(OnClick);
    }

    private void Start()
    {
        StartCoroutine(Load());
    }

    private IEnumerator Load()
    {
        for (float i = 0; i < 1; i += 0.01f)
        {
            fill.fillAmount = i;
            yield return null;
        }
        fill.fillAmount = 1f;
        fill.gameObject.SetActive(false);
        button.gameObject.SetActive(true);
    }

    private void OnPlayerDied()
    {
        button.enabled = true;
    }

    private void OnLevelGenerated(int level)
    {
        Debug.Log("enabling");
        currentLevelText.text = "Play\nLevel - " + level.ToString();
        button.enabled = true;
    }

    private void OnClick()
    {
        OnPlayClick.Invoke();
        button.enabled = false;
    }
}
