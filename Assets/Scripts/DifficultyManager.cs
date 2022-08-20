using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public AnimationCurve difficultyCurve;
    public float secondsPlayedOffset;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetDifficultyMod()
    {
        float percentage = ((Time.timeSinceLevelLoad + secondsPlayedOffset) / GameManager.instance.GameLengthInSeconds);
        return difficultyCurve.Evaluate(percentage);
    }
}
