using System;

using UnityEngine;

[Serializable]
public class PlayerData
{
    public float maxHealth;
    public float Health;
    public float MovementSpeed = 5f;
    public float baseForce = 1f;
    public float pushbackDistance;
    public float addedForce;
    [Range(0f, 1f)] public float addForceFactor;

    internal void ReduceHealth(float v)
    {
        Health -= v;
    }
}
