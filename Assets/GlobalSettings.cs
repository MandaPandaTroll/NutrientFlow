using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalSettings : MonoBehaviour
{
    public double meanMaximumLifeSpan;
    public double std_lifeSpan;
    public bool autoStart;
    public static bool startButtonPressed{get;set;}
   public int autoStartTimer, autoStartDelay;
   public int quitCheckPeriod, quitCheckTimer;

   public bool QuitOnGlobalExtinction;
   public bool QuitAtTime;
   public int quitTime;
   public bool QuitAtGeneration;
   public int quitGeneration;
   public int LogTimePeriod;
   int logTimeTimer;
   public Button startButton;
    void Start(){
        AccessibleGlobalSettings.meanMaximumLifeSpan = meanMaximumLifeSpan;
        AccessibleGlobalSettings.std_lifeSpan = std_lifeSpan;
        ParamLookup.QuitOnGlobalExtinction = QuitOnGlobalExtinction;
        ParamLookup.QuitAtTime = QuitAtTime;
        ParamLookup.quitTime = quitTime;
        ParamLookup.QuitAtGeneration = QuitAtGeneration;
        ParamLookup.quitGeneration = quitGeneration;
        
    }

    void Update(){
        if(autoStart && startButtonPressed == false){
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
        quitCheckTimer += 1;
        if(quitCheckTimer >= quitCheckPeriod){
            quitCheckTimer = 0;
            if(QuitOnGlobalExtinction){
            if (IndividualStats.GetNAutos() + GameteStats.GetNGamete() <= 0){
                Debug.Log("GLOBAL EXTINCTION");
                Application.Quit();
                }
            }
            if(QuitAtTime){
            if (StatDisplay.tSteps >= quitTime){
                Debug.Log("MAX TIME REACHED");
                Application.Quit();
            }
         }

         if(QuitAtGeneration){
            if (Mathf.FloorToInt(IndividualStats.GetMeanGeneration()) >= quitGeneration){
                Debug.Log("MAX GENERATION REACHED");
                Application.Quit();
            }
         }


        }

        
    }

    void WriteTimeLog(){
        Debug.Log(System.DateTime.Now + " | " + " seconds: " + StatDisplay.tSecs +  " | " + " Steps: " + StatDisplay.tSteps  +"\n" +
         "Autotrophs: " + IndividualStats.GetNAutos() +"\n" +
         "Gametes: " + GameteStats.GetNGamete() +"\n" +
         "Mean Generation Number: " + IndividualStats.GetMeanGeneration()+"\n" +
         "TotalNutrients: " + nutrientStats.totalNutrients) ;
    }



    
}

public static class AccessibleGlobalSettings{
    public static double meanMaximumLifeSpan{get;set;}
    public static double std_lifeSpan{get; set;}


}

public static class ParamLookup{
    
    public static int[] gridDims {get;set;}
    public static float initConc{get;set;}
    public static string initDistribution{get;set;}
    public static int circleRadius{get;set;}
    public static int totalNutrients{get;set;}
    public static int DiffusionPeriod{get;set;}
    public static int diffusionLimit{get;set;}
    public static int initPop{get;set;}

    public static int gridSamplePeriod{get;set;}
    public static int statSamplePeriod{get;set;}
    public static int autoPosSamplePeriod{get;set;}
    public static int gamPosSamplePeriod{get;set;}
    public static int repStatSamplePeriod{get;set;}
    public static bool QuitOnGlobalExtinction{get;set;}
   public static bool QuitAtTime{get;set;}
   public static int quitTime{get;set;}
   public static bool QuitAtGeneration{get;set;}
   public static int quitGeneration{get;set;}
    
    
    
    
    /*
    "gridSampleFrequency = "+ gridSampleFrequency + "\n" +
        "statSampleFrequency = "+ statSampleFrequency + "\n" + 
        "autoPosSampleFreq = "+ autoPosSampleFreq + "\n" +
        "gamPosSampleFreq = "+ gamPosSampleFreq + "\n" +
        "repStatFreq = "+ repStatFreq + "\n" +
    */


}
