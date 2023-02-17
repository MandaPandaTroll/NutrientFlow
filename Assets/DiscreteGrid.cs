//Based on code by CodeMonkey

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
public class DiscreteGrid : MonoBehaviour
{



 

   
    public float initDiffusionRate;
    public  float diffusionRate;
    public static float diffusionDivisor{get;set;}
    public static bool DiffusionEnabled{get;set;}
    public bool DoSpawnNutrients{get;set;}
    public bool initDiffusionEnabled;
    public IntGrid nutrientGrid;
    public Transform boxTransform;
    public int gridWidth, gridHeight;
    public float cellSize;
    Vector3 originPosition;

    public float initialConcentration;
    public bool discreteAdd;
    public float nutrientSampleRate;
    float sampleTimer;
    public bool spawnInMiddle;
    public bool veryRandomSpawn;
    public bool defaultSpawn;
    public int nutesToSpawn;
    public bool diffusionEnabled;
    public int diffusionLimit;
    int dirCounter = 0;
    int dirs;
    public int numCells;
    int arrayDim0, arrayDim1;
   int tempFree, tempLocked, tempTotal;
   public int maxPerVeryRandomSpawnDivisor;
    void Start()
    {

        diffusionRate = initDiffusionRate;
        diffusionDivisor = 1f;
        diffusionEnabled = initDiffusionEnabled;
        originPosition = new Vector3(-boxTransform.localScale.x/2f,-boxTransform.localScale.y/2f,0f);
        gridWidth = Mathf.FloorToInt(boxTransform.localScale.x/cellSize);
        gridHeight = Mathf.FloorToInt(boxTransform.localScale.y/cellSize);

        nutrientGrid = new IntGrid(gridWidth,gridHeight,cellSize,originPosition);
        arrayDim0 = nutrientGrid.gridArray.GetLength(0);
        arrayDim1 = nutrientGrid.gridArray.GetLength(1);
        numCells = gridWidth*gridHeight;
        
        
       
    }

    void Update(){
        if(DoSpawnNutrients == true){
            DoSpawnNutrients = false;
            initialConcentration = InputFieldToFloat.value;
            SpawnNutrients();
        }
    }

    public void SpawnNutrients(){
        nutesToSpawn = Mathf.FloorToInt(initialConcentration*numCells);
        if(discreteAdd == true){
            for(int i = 0; i < gridWidth;i++){
            for(int j = 0; j < gridHeight; j++){
                nutrientGrid.SetValue(i,j,(Random.Range( 0,2)));
                //Debug.Log(nutrientGrid.GetValue(i,j));
             }
            }
        }else if(spawnInMiddle == true){
            nutrientGrid.SetValue(gridWidth/2,gridHeight/2,nutesToSpawn);
        }else if(veryRandomSpawn == true){
            if (maxPerVeryRandomSpawnDivisor <= 0){
                maxPerVeryRandomSpawnDivisor = 1;
            }
            int panicCounter = 0;
            int nutesLeft = nutesToSpawn;
            int x;
            int y;
            int tempVal;
            while(nutesLeft > 0){
            panicCounter++;
             x = Random.Range(0,gridWidth);
             y = Random.Range(0,gridHeight);
             tempVal = nutrientGrid.GetValue(x,y) + Random.Range(0,nutesLeft/maxPerVeryRandomSpawnDivisor);
            nutesLeft = nutesLeft - tempVal;
            nutrientGrid.SetValue(x,y,tempVal);
            };
            
        }
        else if(defaultSpawn == true){
            
            int randX, randY;
            int thisVal;
            for(int nutesLeft = nutesToSpawn; nutesLeft > 0; nutesLeft--){
                randX = Random.Range(0,gridWidth);
                randY = Random.Range(0,gridHeight);
                thisVal = nutrientGrid.GetValue(randX,randY);
                nutrientGrid.SetValue(randX,randY,thisVal+1);
               
            }
                
                
           

        }
    }

    float diffusionTimer;

    void FixedUpdate(){

        
        sampleTimer += Time.fixedDeltaTime;
        diffusionTimer += Time.fixedDeltaTime;

        if (sampleTimer >= nutrientSampleRate){
            
            SampleNutrients();
        }
        if(diffusionTimer >= diffusionRate && diffusionEnabled == true){
             diffusionRate = 1f/diffusionDivisor;
            DefaultDiffusion();
        }

        
    }


int[,] kernel = new int[3,3];

void DefaultDiffusion(){
   
            int hereval;
           
            gridWidth = arrayDim0;
            gridHeight = arrayDim1;

           // int dirs = UnityEngine.Random.Range(-2,3);
            if(dirCounter == 0){
                for(int x = 0; x < gridWidth-1; x++){
                    for(int y = 0; y < gridHeight-1; y++){
                    
                    kernel = nutrientGrid.GetKernel(x,y);
                    if(kernel.Cast<int>().Sum() == 0){
                        break;
                    }
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
                        if(deltaval > 0 && deltaval <= 1){
                            moveAmount = 1;
                        }
                        else if(deltaval > 1 && deltaval <= 2){
                            moveAmount = 1;
                        }else if(deltaval > 2 && deltaval <= 4){
                            moveAmount = 2;
                        }else if(deltaval > 4 && deltaval <= 8){
                            moveAmount = 4;
                        }else if(deltaval > 8 && deltaval <= 16){
                            moveAmount = 8;
                        }else if(deltaval > 16 && deltaval <= 32){
                            moveAmount = 16;
                        }else if(deltaval > 32 ){
                            moveAmount = 32;
                        }/*else if(deltaval > 64){
                            moveAmount= Mathf.ClosestPowerOfTwo(deltaval)/4;
                        }*/
                        
                        if(moveAmount > 0){
                        moveAmount = Random.Range(1,moveAmount);
                        nutrientGrid.SetValue(maxFirstIndex+x-1,maxSecondIndex+y-1,maxValue-moveAmount);
                            nutrientGrid.SetValue(x,y,hereval+moveAmount);
                        }
                        
                        
                            
                            
                            
                    }

                }
                }
                dirCounter = 1;
            }else if(dirCounter == 1){
                for(int x = gridWidth-1; x > 0; x--){
                for(int y = gridHeight-1; y > 0; y--){
                    kernel = nutrientGrid.GetKernel(x,y);
                    

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
                        if(deltaval > 0 && deltaval <= 1){
                            moveAmount = 1;
                        }
                        else if(deltaval > 1 && deltaval <= 2){
                            moveAmount = 1;
                        }else if(deltaval > 2 && deltaval <= 4){
                            moveAmount = 2;
                        }else if(deltaval > 4 && deltaval <= 8){
                            moveAmount = 4;
                        }else if(deltaval > 8 && deltaval <= 16){
                            moveAmount = 8;
                        }else if(deltaval > 16 && deltaval <= 32){
                            moveAmount = 16;
                        }else if(deltaval > 32 ){
                            moveAmount = 32;
                        }/*else if(deltaval > 64){
                            moveAmount= Mathf.ClosestPowerOfTwo(deltaval)/4;
                        }*/
                        
                        if(moveAmount > 0){
                        moveAmount = Random.Range(1,moveAmount);
                        nutrientGrid.SetValue(maxFirstIndex+x-1,maxSecondIndex+y-1,maxValue-moveAmount);
                            nutrientGrid.SetValue(x,y,hereval+moveAmount);
                        }
                        
                        
                            
                            
                            
                    }
                    
                }
            }
                dirCounter = 2;
            }else if(dirCounter == 2){
                for(int x = gridWidth-1; x > 0; x--){
                for(int y = 0; y < gridHeight-1; y++){
                    kernel = nutrientGrid.GetKernel(x,y);
                    

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
                        if(deltaval > 0 && deltaval <= 1){
                            moveAmount = 1;
                        }
                        else if(deltaval > 1 && deltaval <= 2){
                            moveAmount = 1;
                        }else if(deltaval > 2 && deltaval <= 4){
                            moveAmount = 2;
                        }else if(deltaval > 4 && deltaval <= 8){
                            moveAmount = 4;
                        }else if(deltaval > 8 && deltaval <= 16){
                            moveAmount = 8;
                        }else if(deltaval > 16 && deltaval <= 32){
                            moveAmount = 16;
                        }else if(deltaval > 32){
                            moveAmount = 32;
                        }/*else if(deltaval > 64){
                            moveAmount= Mathf.ClosestPowerOfTwo(deltaval)/4;
                        }*/
                        if(moveAmount > 0){
                        moveAmount = Random.Range(1,moveAmount);
                        nutrientGrid.SetValue(maxFirstIndex+x-1,maxSecondIndex+y-1,maxValue-moveAmount);
                            nutrientGrid.SetValue(x,y,hereval+moveAmount);
                        }
                        
                        
                            
                            
                            
                    }

                }
            }
                dirCounter = 3;
            }else if(dirCounter == 3){
                for(int x = 0; x < gridWidth-1; x++){
                for(int y = gridHeight-1; y > 0; y--){
                    kernel = nutrientGrid.GetKernel(x,y);
                    

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
                        if(deltaval > 0 && deltaval <= 1){
                            moveAmount = 1;
                        }
                        else if(deltaval > 1 && deltaval <= 2){
                            moveAmount = 1;
                        }else if(deltaval > 2 && deltaval <= 4){
                            moveAmount = 2;
                        }else if(deltaval > 4 && deltaval <= 8){
                            moveAmount = 4;
                        }else if(deltaval > 8 && deltaval <= 16){
                            moveAmount = 8;
                        }else if(deltaval > 16 && deltaval <= 32){
                            moveAmount = 16;
                        }else if(deltaval > 32 ){
                            moveAmount = 32;
                        }/*else if(deltaval > 64){
                            moveAmount= Mathf.ClosestPowerOfTwo(deltaval)/4;
                        }*/
                        if(moveAmount > 0){
                        moveAmount = Random.Range(1,moveAmount);
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
        //return;
}
    
    void SampleNutrients(){
            tempFree = 0;
            tempLocked = 0;
            tempTotal = 0;

            tempFree = nutrientGrid.GetSum();
            tempLocked = IndividualStats.GetSumNutrients() + GameteStats.GetSumNutrients();
            tempTotal = tempFree + tempLocked;

            nutrientStats.totalNutrients = tempTotal;
            nutrientStats.freeNutrients = tempFree;
            nutrientStats.lockedNutrients = tempLocked;
            
            sampleTimer = 0;
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

 public float GetdiffusionRate(InputField input){
    float output;
    output = float.Parse(input.text);
    return output;
 }
}

public static class nutrientStats{
    public static int lockedNutrients{get;set;}
    public static int freeNutrients{get;set;}
    public static int totalNutrients{get;set;}

    
}