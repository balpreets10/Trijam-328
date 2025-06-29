using System;

using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject PlayerPrefab;

    private PlayerController playerController;
    private PlayerView playerView;
    private PlayerData playerData;

    private void Awake()
    {
        playerView = Instantiate(PlayerPrefab, transform).GetComponent<PlayerView>();
        playerView.Deactivate();
    }

    public PlayerController GetPlayerController()
    {
        playerController = new PlayerController(playerView, playerData);
        return playerController;
    }

    internal void SetPlayerData(PlayerData playerData)
    {
        this.playerData = playerData;
    }
}
