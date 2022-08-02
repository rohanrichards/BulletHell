using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class Shotgun : WeaponBase
{
    public float arcSize = 10f;
    public float radius = 0.5f;
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
        FMODUnity.RuntimeManager.PlayOneShot(this.TriggerEvent, ECSPlayerController.getPlayerLocationVector());

        weaponConfig.FireFunc(weaponConfig, bulletEntityPrefab);
        yield return new WaitForSeconds(1 / weaponConfig.ROF);
        StartCoroutine(Fire());
    }
}
