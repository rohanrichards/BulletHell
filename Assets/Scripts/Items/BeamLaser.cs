using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamLaser : WeaponBase
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
/*        if (!playerBody)
        {
            playerBody = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Rigidbody2D>();
        }

        float arcSize = 315;
        float arcSegment = arcSize / ProjectileCount;
        float offsetWidth = 1f;
        float offsetSegment = offsetWidth / ProjectileCount;
        for (int i = 0; i < ProjectileCount; i++)
        {

            float rotationOffset = (arcSegment / 2) + (arcSegment * i);
            float offset = (offsetSegment / 2) + (offsetSegment * i);
            Vector3 originOffset = playerBody.transform.up + (playerBody.transform.right * ((offsetWidth / 2) - offset));
            Vector3 rotationOrigin = playerBody.transform.rotation.eulerAngles;
            Vector3 offsetVector = new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            Vector3 rotation = rotationOrigin + offsetVector;
            //BulletBase.Create(bulletPrefab, playerBody.transform, originOffset, Quaternion.Euler(rotation), offsetVector, bulletConfig, this);
        }*/
        yield return new WaitForSeconds(1 / weaponConfig.ROF + weaponConfig.Lifespan);
        StartCoroutine(Fire());
    }
}
