using System.Collections;
using System.Collections.Generic;
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
        if (!playerBody)
        {
            playerBody = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Rigidbody2D>();
        }

        float arcSegment = arcSize / ProjectileCount;
        for (int i = 0; i < ProjectileCount; i++)
        {
            Vector2 circleposition = Random.insideUnitCircle * radius;

            float rotationOffset = (arcSegment / 2) + (arcSegment * i);
            Vector3 originOffset = new Vector3(circleposition.x, circleposition.y, 0);
            Vector3 rotationOrigin = playerBody.transform.rotation.eulerAngles;
            Vector3 rotation = rotationOrigin + new Vector3(0, 0, (-arcSize / 2) + rotationOffset);
            BulletBase.Create(bulletPrefab, playerBody.transform, originOffset, Quaternion.Euler(rotation), bulletConfig, this);
        }
        yield return new WaitForSeconds(1 / RateOfFire);
        StartCoroutine(Fire());
    }
}
