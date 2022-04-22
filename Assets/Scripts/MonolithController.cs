using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class MonolithController : MonoBehaviour
{
    public List<SpriteRenderer> runes;
    public Light2D[] lights;
    public float lerpSpeed;
    private CircleCollider2D boomCollider;
    private ParticleSystem particles;
    private Animation animationController;

    private Color curColor;
    private Color targetColor;
    private Color currentLightColor;
    private Color targetLightColor;
    private float currentCharge = 0;
    private float targetCharge = 0;
    public bool isBroken = false;

    private Color baseColor = new Color(0.005f, 0.536f, 1);
    private float t = 0;

    // Start is called before the first frame update
    void Start()
    {
        boomCollider = transform.Find("BOOMCollider").gameObject.GetComponentInChildren<CircleCollider2D>(true);
        particles = GetComponentInChildren<ParticleSystem>();
        animationController = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBroken)
        {
            t += lerpSpeed * Time.deltaTime;
            curColor = Color.Lerp(curColor, targetColor, t);
            currentLightColor = Color.Lerp(currentLightColor, targetLightColor, t);
            currentCharge = Mathf.Lerp(currentCharge, targetCharge, t);

            foreach (var r in runes)
            {
                r.color = curColor;
            }

            foreach (var l in lights)
            {
                l.color = currentLightColor;
            }

            if(currentCharge >= 0.9)
            {
                StartCoroutine(DoTheBoom());                
            }
        }
    }

    void StartCharging()
    {
        t = 0;
        targetColor = new Color(1, 1, 1, 1);
        targetLightColor = baseColor;
        targetCharge = 1;
    }

    void StopCharging()
    {
        t = 0;
        targetColor = new Color(1, 1, 1, 0);
        targetLightColor = new Color(0, 0, 0);
        targetCharge = 0;
    }

    IEnumerator DoTheBoom()
    {
        boomCollider.enabled = true;
        particles.Play();
        animationController.Play();

        isBroken = true;
        curColor = new Color(0, 0, 0, 0);
        currentLightColor = new Color(0, 0, 0);
        currentCharge = 0;

        foreach (var r in runes)
        {
            r.color = curColor;
        }

        foreach (var l in lights)
        {
            l.color = currentLightColor;
        }

        yield return new WaitForSeconds(2);
        particles.Stop();
        boomCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.transform.parent && collision.gameObject.transform.parent.tag == "Player")
        {
            StartCharging();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.transform.parent && collision.gameObject.transform.parent.tag == "Player")
        {
            StopCharging();
        }
    }
}
