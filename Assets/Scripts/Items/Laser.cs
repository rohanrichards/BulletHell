using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class Laser : WeaponBase
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Unlock()
    {
        base.Unlock();
    }

    public override IEnumerator Fire()
    {
        int pulseCount = 3;
        float pulsePause = 0.1f;
        for (int i = 0; i < pulseCount; i++)
        {
            weaponConfig.FireFunc(weaponConfig, bulletEntityPrefab);
            yield return new WaitForSeconds(pulsePause);
        }

        yield return new WaitForSeconds(1 / weaponConfig.ROF);
        StartCoroutine(Fire());
    }
}
