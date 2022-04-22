using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "SO's/Configs/Enemy", order = 3)]
public class EnemySO : DestroyableSO
{
    public float moveSpeed = 1.0f;
    public int XPValue = 1;
}
