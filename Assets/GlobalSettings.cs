using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    public double meanMaximumLifeSpan;
    public double std_lifeSpan;
    void Start(){
        AccessibleGlobalSettings.meanMaximumLifeSpan = meanMaximumLifeSpan;
        AccessibleGlobalSettings.std_lifeSpan = std_lifeSpan;

        
    }

    
}

public static class AccessibleGlobalSettings{
    public static double meanMaximumLifeSpan{get;set;}
    public static double std_lifeSpan{get; set;}
}
