using System;
using System.Collections;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    public Image fill;
    public UnityEvent OnLoadingComplete;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnEnable()
    {
        OnLoadingComplete.AddListener(Deactivate);
    }

    private void OnDisable()
    {
        OnLoadingComplete.RemoveAllListeners();
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
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
        OnLoadingComplete.Invoke();
    }
}
