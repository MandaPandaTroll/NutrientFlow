
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour
{

   
    public static List<int> causeOfDeathRunningList = new List<int>();
    public float ageToStarvationRatio;
    public  static int tSteps;
    public bool doReset{get;set;}
    float meanGen;
   public static float tSecs;

    public float statUpdatePeriod;
    float statUpdateTimer;

    public Text tSecText, tStepText, meanGenText, totNuteText, freeNuteText,lockedNuteText, autoNuteText,gamNuteText, nAutoText, nGameteText, diffusionRateText, generationStDevText;
    
    // Start is called before the first frame update
    void Start()
    {
        tSteps = -1;
        tSecs = 0;
        //Text[] allBoxes = GetComponentsInChildren<Text>();
        /*foreach(Text text in allBoxes){
            if (text.gameObject.tag == "Time"){
                tSecText = text;
            }else if(text.gameObject.tag == "Steps"){
                tStepText = text;
            }else if(text.gameObject.tag == "MeanGen"){
                meanGenText = text;
            }
        }*/

    }
    void Update(){
        if(doReset == true){
            tSteps = -1;
            tSecs = 0;
            doReset = false;
        }
    }


    void FixedUpdate()
    {
        tSteps +=1;
        tSecs += Time.fixedDeltaTime;
        statUpdateTimer += Time.deltaTime;
        if(statUpdateTimer >= statUpdatePeriod){
            tSecText.text = "Seconds" + "\n" + Mathf.FloorToInt(tSecs).ToString();
            tStepText.text = "Steps" + "\n" + tSteps.ToString();
            totNuteText.text = "Total" + "\n" + "Nutrients" + "\n" +nutrientStats.totalNutrients.ToString();
            freeNuteText.text = "Free" + "\n" + "Nutrients" + "\n" + nutrientStats.freeNutrients.ToString();
            lockedNuteText.text = "Locked"+ "\n" + "Nutrients" + "\n" +nutrientStats.lockedNutrients.ToString();
            autoNuteText.text = "Autotroph"+ "\n" + "Nutrients" + "\n" +IndividualStats.GetSumNutrients().ToString();
            gamNuteText.text = "Gamete"+ "\n" + "Nutrients" + "\n" +GameteStats.GetSumNutrients().ToString();
            nAutoText.text = "Autotrophs" + "\n" + IndividualStats.GetNAutos().ToString();
            nGameteText.text = "Gametes" + "\n" + GameteStats.GetNGamete().ToString();
            meanGenText.text = "Mean" + "\n" + "generation" +  "\n" +IndividualStats.GetMeanGeneration().ToString();
            generationStDevText.text = "SD" + "\n" + "generation" +  "\n" +ExtraMath.GetGenerationStDev(IndividualStats.GetGenerationVals()).ToString();
            diffusionRateText.text = "Diffusion"+ "\n" + "Period(step)" + "\n" + DiscreteGrid.diffusionRate.ToString();
            /*if(causeOfDeathRunningList.Count > 100){
                ageToStarvationRatio = causeOfDeathRunningList.
               causeOfDeathRunningList.RemoveRange(0,causeOfDeathRunningList.Count-100);
            }*/
            statUpdateTimer = 0;
        }
    }

    
    
}
