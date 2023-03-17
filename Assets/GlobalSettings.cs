using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalSettings : MonoBehaviour
{
    public double meanMaximumLifeSpan;
    public double std_lifeSpan;
    public bool autoStart;
   public int autoStartTimer, autoStartDelay;
   public int LogTimePeriod;
   int logTimeTimer;
   public Button startButton;
    void Start(){
        AccessibleGlobalSettings.meanMaximumLifeSpan = meanMaximumLifeSpan;
        AccessibleGlobalSettings.std_lifeSpan = std_lifeSpan;

        
    }

    void Update(){
        if(autoStart){
            if (autoStartTimer >= autoStartDelay){
                startButton.onClick.Invoke();
                autoStart = false;
                WriteTimeLog();
            }
            autoStartTimer += 1;
            
        }
    }

    void FixedUpdate(){
        if(logTimeTimer >= LogTimePeriod){
            WriteTimeLog();
            logTimeTimer = 0;
        }
        logTimeTimer += 1;
    }

    void WriteTimeLog(){
        Debug.Log(System.DateTime.Now + " | " + " seconds: " + StatDisplay.tSecs +  " | " + " Steps: " + StatDisplay.tSteps  +"\n" +
         "Autotrophs: " + IndividualStats.GetNAutos() +"\n" +
         "Gametes: " + GameteStats.GetNGamete());
    }

    
}

public static class AccessibleGlobalSettings{
    public static double meanMaximumLifeSpan{get;set;}
    public static double std_lifeSpan{get; set;}
}
