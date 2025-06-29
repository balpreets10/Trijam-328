using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public LoadingPanel loadingPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (loadingPanel != null) loadingPanel.gameObject.SetActive(true);
    }
}
