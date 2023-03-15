//Based on code by CodeMonkey

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
public class DiscreteGrid : MonoBehaviour
{
    public bool autoRemoveNutrients;

   
    //public int initDiffusionRate;
 
    public static int diffusionRate{get;set;}
    public static float fDiffusionRate{get;set;}
    public int displayDiffusionRate;
   
    public static bool DiffusionEnabled{get;set;}
    public bool DoSpawnNutrients{get;set;}
    public bool initDiffusionEnabled;
    public IntGrid nutrientGrid;
    public Transform boxTransform;
    public int gridWidth, gridHeight;
    public float cellSize;
    Vector3 originPosition;

    public float initialConcentration;
   
    public float nutrientSampleRate;
    float sampleTimer;
    public float removePeriod;
    public float removeTimer;
    public bool spawnInMiddle;
    public bool veryRandomSpawn;
    public bool defaultSpawn;
    public int nutesToSpawn;
    public bool diffusionEnabled;
    public int diffusionLimit;
    int dirCounter = 0;
    int dirs;
    public int numCells;
   int tempFree, tempLocked, tempTotal;
   public int maxPerVeryRandomSpawnDivisor;
   public StatisticsWriter statisticsWriter;
   public Slider diffRateSlider;

   public static Vector2 boxSize{get;set;}
   public bool autoRespawnNutrientsEnabled;
   bool nutesSpawned;
  
   void Awake(){
        gridWidth = Mathf.FloorToInt(boxTransform.localScale.x/cellSize);
        gridHeight = Mathf.FloorToInt(boxTransform.localScale.y/cellSize);
        StatisticsWriter.gridDims[0] = gridWidth;
        StatisticsWriter.gridDims[1] = gridHeight;
        boxSize = new Vector2(boxTransform.localScale.x, boxTransform.localScale.y);
        boxDims.mapBounds = boxSize/2f;
        nutesSpawned = false;
        
        
   }
    void Start()
    {

        
        fDiffusionRate = diffRateSlider.value;
        diffusionRate = Mathf.FloorToInt(fDiffusionRate);
        if(diffusionRate <= 0){
            diffusionRate = 1;
        }
        displayDiffusionRate = diffusionRate;
        
        diffusionEnabled = initDiffusionEnabled;
        originPosition = new Vector3(-boxTransform.localScale.x/2f,-boxTransform.localScale.y/2f,0f);
        

        nutrientGrid = new IntGrid(gridWidth,gridHeight,cellSize,originPosition);
        
        numCells = gridWidth*gridHeight;
        
        //CalculateDiffusionRate();
        
        
       
       System.Array.Clear(nutrientGrid.gridArray,0,nutrientGrid.gridArray.Length);
    }


    void Update(){
        if(DoSpawnNutrients == true){
            DoSpawnNutrients = false;
            initialConcentration = InputFieldToFloat.value;
            if(!nutesSpawned){
                nutesToSpawn = Mathf.FloorToInt(initialConcentration*numCells);
                SpawnNutrients(nutesToSpawn);
            }
            
            
         }
        
        /*if(autoRespawnNutrientsEnabled == true){
            if(nutrientStats.totalNutrients <= nutesToSpawn-32){
                for(int i = nutrientStats.totalNutrients; i < nutesToSpawn;i++ ){
                    int x = Random.Range(0,gridWidth);
                    int y = Random.Range(0,gridHeight);
                    int tempVal = nutrientGrid.GetValue(x,y);
                    nutrientGrid.SetValue(x,y,tempVal+1);
                }
            }
        }*/
        
    }
 public int nutesLeft;
    public void SpawnNutrients(int nutesToSpawn){
        
        if(nutesSpawned == false){
            nutesLeft = nutesToSpawn;
         if(spawnInMiddle == true && !veryRandomSpawn && !defaultSpawn){
            nutrientGrid.SetValue(gridWidth/2,gridHeight/2,nutesToSpawn);

        }
        
        if(veryRandomSpawn == true && !spawnInMiddle && !defaultSpawn){
            if (maxPerVeryRandomSpawnDivisor <= 0){
                maxPerVeryRandomSpawnDivisor = 1;
            }

            int randX = 0, randY = 0;
            int thisVal = 0;
            int totNutes = nutrientGrid.GetSum();
            int randVal = 0;
            
                while(nutesLeft > 0){
                    totNutes = nutrientGrid.GetSum();
                    if(nutesLeft <= 0){
                        break;
                }else{
                    randX = Random.Range(0,gridWidth);
                    randY = Random.Range(0,gridHeight);
                    
                    if(nutesLeft >= nutesToSpawn/maxPerVeryRandomSpawnDivisor ){
                        randVal = Random.Range(0, nutesToSpawn/maxPerVeryRandomSpawnDivisor+1);
                        
                    }else if(nutesLeft >= maxPerVeryRandomSpawnDivisor){
                        randVal = Random.Range(0, maxPerVeryRandomSpawnDivisor+1); 
                    }else{
                        randVal =Random.Range(0,nutesLeft+1);

                    }
                    
                    if(totNutes < nutesToSpawn && randVal <= nutesLeft){
                        thisVal = nutrientGrid.GetValue(randX,randY);
                        nutrientGrid.SetValue(randX,randY,thisVal+randVal);
                        nutesLeft -= randVal;
                    }
                }
                    
                }
                
                    
                    
                
            
            
        }
        
         if(defaultSpawn == true && !spawnInMiddle && !veryRandomSpawn){
            
            int randX, randY;
            int thisVal;
            for(int nutesLeft1 = nutesToSpawn; nutesLeft1 > 0; nutesLeft1--){
                randX = Random.Range(0,gridWidth);
                randY = Random.Range(0,gridHeight);
                thisVal = nutrientGrid.GetValue(randX,randY);
                nutrientGrid.SetValue(randX,randY,thisVal+1);
               
            }
                
                
           

        }
        nutesSpawned = true;
        }
        

        StatisticsWriter.sampleGrid = nutrientGrid.gridArray;
        statisticsWriter.WriteNutrientGrid(nutrientGrid.gridArray);
    }

    int diffusionTimer;

    void FixedUpdate(){
        
        
        sampleTimer += Time.fixedDeltaTime;
        diffusionTimer += 1;
        StatisticsWriter.sampleGrid = nutrientGrid.gridArray;
        if(autoRemoveNutrients == true){
            removeTimer += Time.fixedDeltaTime;
        }
        
        if (sampleTimer >= nutrientSampleRate){
            
            SampleNutrients();
            
        }
        if(diffusionTimer >= diffusionRate && diffusionEnabled == true){
            
             //diffusionRate = 1f/diffusionDivisor;
            DefaultDiffusion();
            
        }
        
        if(autoRemoveNutrients == true && removeTimer >= removePeriod && nutesSpawned == true){
            removeTimer = 0;
            if(nutrientStats.totalNutrients > nutesToSpawn){
                    int excessNutes = nutrientStats.totalNutrients-nutesToSpawn;
                    
                    while(excessNutes > 0){
                        int randX = Random.Range(0,gridWidth);
                    int randY = Random.Range(0,gridHeight);
                    int thisVal = nutrientGrid.GetValue(randX,randY);
                    if(thisVal > 0){
                        nutrientGrid.SetValue(randX,randY,thisVal-1);
                        excessNutes -= 1;
                    }
                    }
                    

            }
        }
        

        
    }


int[,] kernel = new int[3,3];

void DefaultDiffusion(){
            displayDiffusionRate = diffusionRate;
            //statisticsWriter.WriteNutrientGrid(nutrientGrid.gridArray);
            int hereval;
           
            

            int dirs = UnityEngine.Random.Range(1,5);
            if(dirCounter == 0){
                for(int x = 0; x < gridWidth; x++){
                    for(int y = 0; y < gridHeight; y++){
                    
                    kernel = nutrientGrid.GetKernel(x,y);
                    //if(kernel.Cast<int>().Sum() == 0){
                     //   break;
                    //}
                    hereval = kernel[1,1];

                  
                    int maxValue = -1;
                    
                    int maxFirstIndex = -1;
                    int maxSecondIndex = -1;



                    
                    int value = -1;
                    dirs = Random.Range(1,5);
                 switch(dirs){
                case 1:
                for (int i = 0; i  < 2; i++){
                    for (int j = 0; j < 2; j++) {
                         value = kernel[i, j];

                        if (value > maxValue ) {
                            maxFirstIndex = i;
                        maxSecondIndex = j;

                        maxValue = value;
                            }
                    }
                 }
                break;
                case 2:
                for (int i = 2; i > 0; i--){
                        for (int j = 0; j < 2; j++) {
                     value = kernel[i, j];

                    if (value > maxValue) {
                        maxFirstIndex = i;
                        maxSecondIndex = j;

                        maxValue = value;
                            }
                    }
                 }
                 break;

                 case 3:
                for (int i = 0; i  < 2; i++){
                        for (int j = 2; j > 0; j--) {
                     value = kernel[i, j];

                    if (value > maxValue) {
                        maxFirstIndex = i;
                        maxSecondIndex = j;

                        maxValue = value;
                            }
                    }
                 }
                break;

                case 4:
                for (int i = 2; i > 0; i--){
                        for (int j = 2; j > 0 ; j--) {
                     value = kernel[i, j];

                    if (value > maxValue) {
                        maxFirstIndex = i;
                        maxSecondIndex = j;

                        maxValue = value;
                            }
                    }
                 }
                break;
                
            }
                 
                 
                    int deltaval = maxValue - hereval;
                    int moveAmount = 0;
                //gridWidth = nutrientGrid.gridArray.GetLength(0);
                //gridHeight = nutrientGrid.gridArray.GetLength(1);
                    if(hereval < maxValue && maxValue > diffusionLimit /*&& x < gridWidth && y < gridHeight && x > 0 && y > 0*/){
                        if(deltaval > 0 ){
                            moveAmount = GetFancyDiffusion(deltaval);
                        }
                        
                        if(moveAmount > 0){
                        //moveAmount = Random.Range(1,moveAmount);
                        nutrientGrid.SetValue(maxFirstIndex+x-1,maxSecondIndex+y-1,maxValue-moveAmount);
                            nutrientGrid.SetValue(x,y,hereval+moveAmount);
                        }
                        
                        
                            
                            
                            
                    }

                }
                }
                dirCounter = 1;
            }else if(dirCounter == 1){
                for(int x = gridWidth; x > 0; x--){
                for(int y = gridHeight; y > 0; y--){
                    kernel = nutrientGrid.GetKernel(x,y);
                    //if(kernel.Cast<int>().Sum() == 0){
                     //   break;
                    //}

                    hereval = kernel[1,1];

                  
                    int maxValue = -1;
                    
                    int maxFirstIndex = -1;
                    int maxSecondIndex = -1;



                    
 
                    dirs = Random.Range(1,5);
                 switch(dirs){
                case 1:
                for (int i = 0; i  < 2; i++){
                        for (int j = 0; j < 2; j++) {
                    int value = kernel[i, j];

                    if (value > maxValue) {
                        maxFirstIndex = i;
                        maxSecondIndex = j;

                        maxValue = value;
                            }
                    }
                 }
                break;
                case 2:
                for (int i = 2; i > 0; i--){
                        for (int j = 0; j < 2; j++) {
                    int value = kernel[i, j];

                    if (value > maxValue) {
                        maxFirstIndex = i;
                        maxSecondIndex = j;

                        maxValue = value;
                            }
                    }
                 }
                 break;

                 case 3:
                for (int i = 0; i  < 2; i++){
                        for (int j = 2; j > 0; j--) {
                    int value = kernel[i, j];

                    if (value > maxValue) {
                        maxFirstIndex = i;
                        maxSecondIndex = j;

                        maxValue = value;
                            }
                    }
                 }
                break;

                case 4:
                for (int i = 2; i > 0; i--){
                        for (int j = 2; j > 0 ; j--) {
                    int value = kernel[i, j];

                    if (value > maxValue) {
                        maxFirstIndex = i;
                        maxSecondIndex = j;

                        maxValue = value;
                            }
                    }
                 }
                break;
                
            }
                 
                 
                    int deltaval = maxValue - hereval;
                    int moveAmount = 0;
                //gridWidth = nutrientGrid.gridArray.GetLength(0);
                //gridHeight = nutrientGrid.gridArray.GetLength(1);
                    if(hereval < maxValue && maxValue > diffusionLimit /*&& x < gridWidth && y < gridHeight && x > 0 && y > 0*/){
                        if(deltaval > 0 ){
                            moveAmount = GetFancyDiffusion(deltaval);
                        }
                        
                        if(moveAmount > 0){
                        //moveAmount = Random.Range(1,moveAmount);
                        nutrientGrid.SetValue(maxFirstIndex+x-1,maxSecondIndex+y-1,maxValue-moveAmount);
                            nutrientGrid.SetValue(x,y,hereval+moveAmount);
                        }
                        
                        
                            
                            
                            
                    }
                    
                }
            }
                dirCounter = 2;
            }else if(dirCounter == 2){
                for(int x = gridWidth; x > 0; x--){
                for(int y = 0; y < gridHeight; y++){
                    kernel = nutrientGrid.GetKernel(x,y);
                    //if(kernel.Cast<int>().Sum() == 0){
                     //   break;
                    //}

                    hereval = kernel[1,1];

                  
                    int maxValue = -1;
                    
                    int maxFirstIndex = -1;
                    int maxSecondIndex = -1;



                    
 
                    dirs = Random.Range(1,5);
                 switch(dirs){
                case 1:
                for (int i = 0; i  < 2; i++){
                        for (int j = 0; j < 2; j++) {
                    int value = kernel[i, j];

                    if (value > maxValue) {
                        maxFirstIndex = i;
                        maxSecondIndex = j;

                        maxValue = value;
                            }
                    }
                 }
                break;
                case 2:
                for (int i = 2; i > 0; i--){
                        for (int j = 0; j < 2; j++) {
                    int value = kernel[i, j];

                    if (value > maxValue) {
                        maxFirstIndex = i;
                        maxSecondIndex = j;

                        maxValue = value;
                            }
                    }
                 }
                 break;

                 case 3:
                for (int i = 0; i  < 2; i++){
                        for (int j = 2; j > 0; j--) {
                    int value = kernel[i, j];

                    if (value > maxValue) {
                        maxFirstIndex = i;
                        maxSecondIndex = j;

                        maxValue = value;
                            }
                    }
                 }
                break;

                case 4:
                for (int i = 2; i > 0; i--){
                        for (int j = 2; j > 0 ; j--) {
                    int value = kernel[i, j];

                    if (value > maxValue) {
                        maxFirstIndex = i;
                        maxSecondIndex = j;

                        maxValue = value;
                            }
                    }
                 }
                break;
                
            }
                 
                 
                    int deltaval = maxValue - hereval;
                    int moveAmount = 0;
                //gridWidth = nutrientGrid.gridArray.GetLength(0);
                //gridHeight = nutrientGrid.gridArray.GetLength(1);
                    if(hereval < maxValue && maxValue > diffusionLimit /*&& x < gridWidth && y < gridHeight && x > 0 && y > 0*/){
                        if(deltaval > 0 ){
                            moveAmount = GetFancyDiffusion(deltaval);
                        }
                        if(moveAmount > 0){
                        //moveAmount = Random.Range(1,moveAmount);
                        nutrientGrid.SetValue(maxFirstIndex+x-1,maxSecondIndex+y-1,maxValue-moveAmount);
                            nutrientGrid.SetValue(x,y,hereval+moveAmount);
                        }
                        
                        
                            
                            
                            
                    }

                }
            }
                dirCounter = 3;
            }else if(dirCounter == 3){
                for(int x = 0; x < gridWidth; x++){
                for(int y = gridHeight; y > 0; y--){
                    kernel = nutrientGrid.GetKernel(x,y);
                    //if(kernel.Cast<int>().Sum() == 0){
                     //   break;
                    //}

                    hereval = kernel[1,1];

                  
                    int maxValue = -1;
                    
                    int maxFirstIndex = -1;
                    int maxSecondIndex = -1;



                    
 
                    dirs = Random.Range(1,5);
                 switch(dirs){
                case 1:
                for (int i = 0; i  < 2; i++){
                        for (int j = 0; j < 2; j++) {
                    int value = kernel[i, j];

                    if (value > maxValue) {
                        maxFirstIndex = i;
                        maxSecondIndex = j;

                        maxValue = value;
                            }
                    }
                 }
                break;
                case 2:
                for (int i = 2; i > 0; i--){
                        for (int j = 0; j < 2; j++) {
                    int value = kernel[i, j];

                    if (value > maxValue) {
                        maxFirstIndex = i;
                        maxSecondIndex = j;

                        maxValue = value;
                            }
                    }
                 }
                 break;

                 case 3:
                for (int i = 0; i  < 2; i++){
                        for (int j = 2; j > 0; j--) {
                    int value = kernel[i, j];

                    if (value > maxValue) {
                        maxFirstIndex = i;
                        maxSecondIndex = j;

                        maxValue = value;
                            }
                    }
                 }
                break;

                case 4:
                for (int i = 2; i > 0; i--){
                        for (int j = 2; j > 0 ; j--) {
                    int value = kernel[i, j];

                    if (value > maxValue) {
                        maxFirstIndex = i;
                        maxSecondIndex = j;

                        maxValue = value;
                            }
                    }
                 }
                break;
                
            }
                 
                 
                    int deltaval = maxValue - hereval;
                    int moveAmount = 0;
                //gridWidth = nutrientGrid.gridArray.GetLength(0);
                //gridHeight = nutrientGrid.gridArray.GetLength(1);
                    if(hereval < maxValue && maxValue > diffusionLimit /*&& x < gridWidth && y < gridHeight && x > 0 && y > 0*/){
                        if(deltaval > 0 ){
                            moveAmount = GetFancyDiffusion(deltaval);
                        }
                       
                        if(moveAmount > 0){
                        //moveAmount = Random.Range(1,moveAmount);
                        nutrientGrid.SetValue(maxFirstIndex+x-1,maxSecondIndex+y-1,maxValue-moveAmount);
                            nutrientGrid.SetValue(x,y,hereval+moveAmount);
                        }
                        
                        
                            
                            
                            
                    }

                }
            }
            System.Array.Clear(kernel,0,kernel.Length);
                dirCounter = 0;
            }
            
            
            
            
            
         

        diffusionTimer = 0;
        
        return;
}
int lastNutes, lastFree, lastLocked;

    
    void SampleNutrients(){
            
            tempFree = 0;
            tempLocked = 0;
            tempTotal = 0;

            tempFree = nutrientGrid.GetSum();
            tempLocked = IndividualStats.GetSumNutrients() + GameteStats.GetSumNutrients();
            tempTotal = tempFree + tempLocked;
            if(lastNutes != tempTotal){
                Debug.Log("free = " + tempFree +" lastFree = " + lastFree + " locked = " + tempLocked + " lastLocked = " + lastLocked);
            }
            lastNutes = tempTotal;
            lastFree = tempFree;
            lastLocked = tempLocked;
            nutrientStats.totalNutrients = tempTotal;
            nutrientStats.freeNutrients = tempFree;
            nutrientStats.lockedNutrients = tempLocked;
            
            sampleTimer = 0;
    }

    void RemoveNutrients(){

    }
    
    public int[,] GetKernelFull(int x, int y){
    int[,] internalKernel = new int[3,3];

    internalKernel[0, 2] = nutrientGrid.GetValue(x - 1, y + 1);  // Top left
    internalKernel[1, 2] = nutrientGrid.GetValue(x + 0, y + 1);  // Top center
    internalKernel[2, 2] = nutrientGrid.GetValue(x + 1, y + 1);  // Top right
    internalKernel[0, 1] = nutrientGrid.GetValue(x - 1, y + 0);  // Mid left
    internalKernel[1, 1] = nutrientGrid.GetValue(x + 0, y + 0);  // Current pixel
    internalKernel[2, 1] = nutrientGrid.GetValue(x + 1, y + 0);  // Mid right
    internalKernel[0, 0] = nutrientGrid.GetValue(x - 1, y - 1);  // Low left
    internalKernel[1, 0] = nutrientGrid.GetValue(x + 0, y - 1);  // Low center
    internalKernel[2, 0] = nutrientGrid.GetValue(x + 1, y - 1);  // Low right
    
    return internalKernel;
}

/* public float GetdiffusionRate(InputField input){
    float output;
    output = float.Parse(input.text);
    return output;
 }*/


public static void CalculateDiffusionRate(){
    DiscreteGrid.diffusionRate = Mathf.FloorToInt(fDiffusionRate);
    
    
}

int GetFancyDiffusion(int input){
    int maxout;
    
    if(input/4 > 1){
        maxout = (Mathf.FloorToInt(Mathf.Sqrt((float) input)));
        
    }else{maxout = 1;}
    

    return Random.Range(0,maxout+1);
}


}



public static class nutrientStats{
    public static int lockedNutrients{get;set;}
    public static int freeNutrients{get;set;}
    public static int totalNutrients{get;set;}

    
}

public static class boxDims{
    public static Vector2 mapBounds{get;set;}

}

