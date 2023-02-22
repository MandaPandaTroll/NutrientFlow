
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour
{

   

    public  static int tSteps;
    public bool doReset{get;set;}
    float meanGen;
   public static float tSecs;

    public float statUpdatePeriod;
    float statUpdateTimer;

    public Text tSecText, tStepText, meanGenText, totNuteText, freeNuteText,lockedNuteText, nAutoText, nGameteText;
    
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
            tSecText.text = Mathf.FloorToInt(tSecs).ToString();
            tStepText.text = tSteps.ToString();
            totNuteText.text = nutrientStats.totalNutrients.ToString();
            freeNuteText.text = nutrientStats.freeNutrients.ToString();
            lockedNuteText.text = nutrientStats.lockedNutrients.ToString();
            nAutoText.text = IndividualStats.GetNAutos().ToString();
            nGameteText.text = GameteStats.GetNGamete().ToString();
            meanGenText.text = IndividualStats.GetMeanGeneration().ToString();
            statUpdateTimer = 0;
        }
    }

    
    
}
