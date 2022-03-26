using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupGenerator : MonoBehaviour
{
    public GameObject XPOrbPrefab;
    public GameObject chestPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateXPOrb(Transform location, int value)
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Pickups"));
        filter.useTriggers = true;
        List<Collider2D> hits = new List<Collider2D>();
        Physics2D.OverlapCircle(location.position, 0.1f, filter, hits);
        bool placed = false;
        if (hits.Count > 0)
        {
            foreach(Collider2D hit in hits)
            {
                if(hit.gameObject.GetComponent<XPOrbController>())
                {
                    placed = true;
                    hit.gameObject.GetComponent<XPOrbController>().value += value;
                    break;
                }
            }
        }
        if(!placed)
        {
            GameObject newPickup = PickupBase.Create(XPOrbPrefab, location);
            newPickup.GetComponent<XPOrbController>().value = value;
        }
    }

    public void CreateChest(Transform location)
    {
        GameObject newPickup = PickupBase.Create(chestPrefab, location);
    }
}
