using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralingPiercingDoTProjectile : BulletBase
{
    protected override void Start()
    {
        base.Start();
    }

    IEnumerator DamageLoop()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Enemies"));
        List<Collider2D> hits = new List<Collider2D>();
        // get anything it's touching
        GetComponent<Collider2D>().OverlapCollider(filter, hits);
        foreach (Collider2D collision in hits)
        {
            if (collision.gameObject.tag == "Shootable")
            {
                IShootable controller = collision.gameObject.GetComponentInParent<IShootable>();
                controller.ApplyDamage(Damage);
            }
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(DamageLoop());
    }
}
