using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public AnimationCurve difficultyCurve;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetDifficultyMod()
    {
        float percentage = (Time.timeSinceLevelLoad / gameManager.GameLengthInSeconds);
        return difficultyCurve.Evaluate(percentage);
    }
}
