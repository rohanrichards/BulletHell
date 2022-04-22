using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Destroyable", menuName = "SO's/Configs/Destroyable", order = 3)]
public class DestroyableSO : ScriptableObject
{
    public float baseHealth = 3.0f;
    public float currentHealth = 3.0f;
}
