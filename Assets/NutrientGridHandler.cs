//Based on code by CodeMonkey

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutrientGridHandler : MonoBehaviour
{

    public FloatGrid nutrientGrid;
    public Transform boxTransform;
    public int gridWidth, gridHeight;
    public float cellSize;
    Vector3 originPosition;

    public float initialConcentration;
    public bool discreteAdd;
    public float nutrientSampleRate;
    float sampleTimer;

    // Start is called before the first frame update
    void Start()
    {
        originPosition = new Vector3(-boxTransform.localScale.x/2f,-boxTransform.localScale.y/2f,0f);
        gridWidth = Mathf.FloorToInt(boxTransform.localScale.x);
        gridHeight = Mathf.FloorToInt(boxTransform.localScale.y);

        nutrientGrid = new FloatGrid(gridWidth,gridHeight,cellSize,originPosition);

        if(discreteAdd == true){
            for(int i = 0; i < gridWidth;i++){
            for(int j = 0; j < gridHeight; j++){
                nutrientGrid.SetValue(i,j,(float)(Random.Range( 0,2)));
                //Debug.Log(nutrientGrid.GetValue(i,j));
             }
            }
        }else{
                for(int i = 0; i < gridWidth;i++){
            for(int j = 0; j < gridHeight; j++){
                nutrientGrid.SetValue(i,j,Random.Range( 0f,initialConcentration));
                //Debug.Log(nutrientGrid.GetValue(i,j));
            }
        }
        }

        
    }

    float tempFree, tempLocked, tempTotal;
    // Update is called once per frame
    void Update()
    {
        sampleTimer += Time.fixedDeltaTime;
        if (sampleTimer >= nutrientSampleRate){
            tempFree = 0;
            tempLocked = 0;
            tempTotal = 0;

            tempFree = nutrientGrid.GetSum();
            tempLocked = IndividualStats.GetSumNutrients();
            tempTotal = tempFree + tempLocked;

            nutrientStats.totalNutrients = tempTotal;
            nutrientStats.freeNutrients = tempFree;
            nutrientStats.lockedNutrients = tempLocked;
            sampleTimer = 0;
        }
        
        
        
    }

    
}

public static class nutrientStats{
    public static float lockedNutrients{get;set;}
    public static float freeNutrients{get;set;}
    public static float totalNutrients{get;set;}

    
}
