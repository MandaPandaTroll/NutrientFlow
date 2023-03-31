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
   public int SetInitPopDefault;
    public static int initPopDefault{get;set;}
    public int SetInitDiffusionPeriod;
    public static int initDiffusionPeriod{get;set;}
    public float SetInitConcentration;
    public static float initConc_master{get;set;}
   public bool QuitOnGlobalExtinction;
   public bool QuitAtTime;
   public int quitTime;
   public bool QuitAtGeneration;
   public int quitGeneration;
   public int LogTimePeriod;
   int logTimeTimer;

   public bool doSampleRepEvents;
   public Button startButton;

   public Autotroph_main autotrophPrefabScript;
   public GameteMain gametePrefabScript;
   public InputFieldToInt initPopField;
   public InputFieldToFloat initConcField;
   
    void Awake(){
        ParamLookup.isServer = GetIsServer();
        initPopDefault = SetInitPopDefault;
        initDiffusionPeriod = SetInitDiffusionPeriod;
        initConc_master = SetInitConcentration;
        initConcField.defaultValue = initConc_master;
        initPopField.defaultValue = initPopDefault;
        if(doSampleRepEvents){
            ParamLookup.doSampleRepEvents = true;
        }
        AccessibleGlobalSettings.meanMaximumLifeSpan = meanMaximumLifeSpan;
        AccessibleGlobalSettings.std_lifeSpan = std_lifeSpan;
        ParamLookup.QuitOnGlobalExtinction = QuitOnGlobalExtinction;
        ParamLookup.QuitAtTime = QuitAtTime;
        ParamLookup.quitTime = quitTime;
        ParamLookup.QuitAtGeneration = QuitAtGeneration;
        ParamLookup.quitGeneration = quitGeneration;
        if(autotrophPrefabScript.AsexualReproductionEnabled){
            ParamLookup.ModeOfReproduction = "Asexual";
        }
        else if(!autotrophPrefabScript.AsexualReproductionEnabled){
            if(gametePrefabScript.incompatibilityEnabled){
                ParamLookup.ModeOfReproduction = "Obligate outcrossing";
            }else if(!gametePrefabScript.incompatibilityEnabled){
                ParamLookup.ModeOfReproduction = "Mixed mating";
            }
            
        }
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
            if (GlobalTimeControls.globalSteps >= quitTime){
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
        Debug.Log(System.DateTime.Now + " | " + " seconds: " + StatDisplay.tSecs +  " | " + " Steps: " + GlobalTimeControls.globalSteps  +"\n" +
         "Autotrophs: " + IndividualStats.GetNAutos() +"\n" +
         "Gametes: " + GameteStats.GetNGamete() +"\n" +
         "Mean Generation Number: " + IndividualStats.GetMeanGeneration()+"\n" +
         "TotalNutrients: " + nutrientStats.totalNutrients) ;
    }


    bool GetIsServer(){
        #if UNITY_SERVER
        return true;
        #else
        return false;
        #endif
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
    public static int repEventSamplePeriod{get;set;}
    public static int deathEventSamplePeriod{get;set;}
    public static bool QuitOnGlobalExtinction{get;set;}
   public static bool QuitAtTime{get;set;}
   public static int quitTime{get;set;}
   public static bool QuitAtGeneration{get;set;}
   public static int quitGeneration{get;set;}
   public static string ModeOfReproduction{get;set;}
   public static bool doSampleRepEvents{get;set;}
   public static bool isServer{get;set;}
    
    
    
    
    /*
    "gridSampleFrequency = "+ gridSampleFrequency + "\n" +
        "statSampleFrequency = "+ statSampleFrequency + "\n" + 
        "autoPosSampleFreq = "+ autoPosSampleFreq + "\n" +
        "gamPosSampleFreq = "+ gamPosSampleFreq + "\n" +
        "repStatFreq = "+ repStatFreq + "\n" +
    */


}
