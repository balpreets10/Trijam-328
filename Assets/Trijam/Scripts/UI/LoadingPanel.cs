using System;
using System.Collections;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
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


}
