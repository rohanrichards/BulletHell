using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IShootable
{
    public void ApplyDamage(float damage);
    public void KillSelf();
}
