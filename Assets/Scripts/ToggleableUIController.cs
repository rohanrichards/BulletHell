using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleableUIController : MonoBehaviour
{
    protected bool visible = false;
    protected CanvasGroup group;

    public virtual void Start()
    {
        group = gameObject.GetComponent<CanvasGroup>();
        if (!group)
        {
            group = new CanvasGroup();
            group.transform.SetParent(gameObject.transform);
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (visible)
        {
            group.alpha = 1;
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        else
        {
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }

    public virtual void Show()
    {
        visible = true;
    }

    public virtual void Hide()
    {
        visible = false;
    }
}
