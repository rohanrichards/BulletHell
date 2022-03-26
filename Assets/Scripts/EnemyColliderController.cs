using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColliderController : MonoBehaviour
{
    EnemyBase parentController;

    private void Start()
    {
        parentController = gameObject.GetComponentInParent<EnemyBase>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform.parent && collision.gameObject.transform.parent.CompareTag("Player"))
        {
            parentController.StartAttacking();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.transform.parent && collision.gameObject.transform.parent.CompareTag("Player"))
        {
            parentController.StopAttacking();
        }
    }
}
