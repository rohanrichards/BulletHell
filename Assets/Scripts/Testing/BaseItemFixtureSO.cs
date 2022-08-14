using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Items_", menuName = "BulletHellTesting/ItemFixture", order = 1)]
public class BaseItemFixtureSO : ScriptableObject
{
    public int startingXP;

    public int numShotgunLevels;
    public int numLaserLevels;
    public int numDiscGunLevels;
    public int numDischargerLevels;
    public int numBeamLaserLevels;
    public int numLauncherLevels;
    public int numFlailLevels;
    public int numAcidLevels;
    public int numBlackHoleLevels;

    public int numDamageLevels;
    public int numROFLevels;
    public int numMobilityLevels;
    public int numRotateSpeedLevels;
    public int numXPIncreaseLevels;
}
