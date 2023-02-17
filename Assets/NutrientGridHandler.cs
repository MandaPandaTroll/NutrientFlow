//Based on code by CodeMonkey

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
    public bool spawnInMiddle;
    

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
        }else if(spawnInMiddle == true){
            nutrientGrid.SetValue(gridWidth/2,gridHeight/2,(gridWidth*gridHeight*initialConcentration));
        }
        else{
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

            //nutrientStats.totalNutrients = tempTotal;
            //nutrientStats.freeNutrients = tempFree;
            //nutrientStats.lockedNutrients = tempLocked;
            //sampleTimer = 0;

            LeDiffusor();
        }
        
        
        
    }

    
    public float[,] kernel = new float[3,3];
    public void GaussianDiffusion(){
        for(int i = 0; i < gridWidth-1;i++){
            for(int j = 0; j < gridHeight-1;j++){
            float sumKernel = nutrientGrid.GetValue(i-1,j+1) + nutrientGrid.GetValue(i,j+1) + nutrientGrid.GetValue(i+1,j+1) +
                              nutrientGrid.GetValue(i-1,j)   + nutrientGrid.GetValue(i,j)   + nutrientGrid.GetValue(i+1,j)   +
                              nutrientGrid.GetValue(i-1,j-1) + nutrientGrid.GetValue(i,j-1) + nutrientGrid.GetValue(i+1,j-1) ;

                //kernel[0,2] = nutrientGrid.GetValue(i-1,j+1); kernel[1,2] = nutrientGrid.GetValue(i,j+1); kernel[2,2] = nutrientGrid.GetValue(i+1,j+1);
                //kernel[0,1] = nutrientGrid.GetValue(i-1,j);   kernel[1,1] = nutrientGrid.GetValue(i,j);   kernel[2,1] = nutrientGrid.GetValue(i+1,j);
                //kernel[0,0] = nutrientGrid.GetValue(i-1,j-1); kernel[1,0] = nutrientGrid.GetValue(i,j-1); kernel[2,0] = nutrientGrid.GetValue(i+1,j-1);
                //float meanKernel = (kernel.Cast<float>().Sum())/9f;
                float meanKernel = sumKernel/9.0f;
                nutrientGrid.SetValue(i,j,meanKernel);
            }
        }

    }
    // 0,2 1,2 2,2
    // 0,2     2,1
    // 0,0 1,0 2,0
    List<int[]> otherIndices = new List<int[]>();
    int[,] testIndices = new int[8,2]{{0,2},{1,2},  {2,2},{0,2},{2,1},{0,0},{1,0},{2,0}};
    void LeDiffusor(){
        float thisVal = 0;
        float thatVal = 0;
        float deltaVal = 0;
        int randX = -1;
        int randY = -1;
        for(int i = 0; i < gridWidth;i++){
            for(int j = 0; j < gridHeight;j++){
                thisVal = nutrientGrid.GetValue(i,j);
                if(thisVal > 0){
                    
                    randX = testIndices[Random.Range(0,8),0];
                    randY = testIndices[Random.Range(0,8),1];;
                    
                        thatVal = nutrientGrid.GetValue(i-1+randX,j-1+randY);
                        deltaVal = thisVal - thatVal;
                        if(deltaVal > 0){
                            nutrientGrid.SetValue(i,j,thisVal - (deltaVal/64f));
                            nutrientGrid.SetValue(i-1+randX,j-1+randY,thatVal + (deltaVal/64f));
                        }

                    
                }
            }
        }
    }

    public void OtherDiffusion(){
        float deltaVal = 0;
        float thisVal = 0;
        float thatVal = 0;
        for(int i = 0; i < gridWidth-1;i++){
            for(int j = 0; j < gridHeight-1;j++){
                    thisVal = nutrientGrid.GetValue(i,j);
                    thatVal = nutrientGrid.GetValue(i+1,j+1);
                    deltaVal = thatVal - thisVal;
                    if(deltaVal > 1f){
                        nutrientGrid.SetValue(i,j,thisVal+1f);
                        nutrientGrid.SetValue(i+1,j+1,thatVal-1f);
                    }
                }
            }
        for(int i = gridWidth; i > 1;i--){
            for(int j = 0; j < gridHeight-1;j++){
                    thisVal = nutrientGrid.GetValue(i,j);
                    thatVal = nutrientGrid.GetValue(i-1,j+1);
                    deltaVal = thatVal - thisVal;
                    if(deltaVal > 1f){
                        nutrientGrid.SetValue(i,j,thisVal+1f);
                        nutrientGrid.SetValue(i-1,j+1,thatVal-1f);
                    }
                }
            }
        for(int i = 0; i < gridWidth-1;i++){
            for(int j = gridHeight; j > 1;j--){
                    thisVal = nutrientGrid.GetValue(i,j);
                    thatVal = nutrientGrid.GetValue(i+1,j-1);
                    deltaVal = thatVal - thisVal;
                    if(deltaVal > 1f){
                        nutrientGrid.SetValue(i,j,thisVal+1f);
                        nutrientGrid.SetValue(i+1,j-1,thatVal-1f);
                    }
                }
            }
        for(int i = gridWidth; i > 1;i--){
            for(int j = gridHeight; j > 1;j--){
                    thisVal = nutrientGrid.GetValue(i,j);
                    thatVal = nutrientGrid.GetValue(i-1,j-1);
                    deltaVal = thatVal - thisVal;
                    if(deltaVal > 1f){
                        nutrientGrid.SetValue(i,j,thisVal+1f);
                        nutrientGrid.SetValue(i-1,j-1,thatVal-1f);
                    }
                }
            }
    }

    public void DefaultDiffusion(){

        
        
        for(int i = 0; i < gridWidth;i++){
            for(int j = 0; j < gridHeight;j++){
            
                float maxVal = 0;
                float tempVal = 0;
                int maxIndex_x = -1;
                int maxIndex_y = -1;
                float deltaVal = 0;
                float thisVal;
                float minVal = 1e4f;
                //List <int[]> minIndices = new List<int[]>();
                int[] tempMinIndex = new int[2]; 
                kernel[0,2] = nutrientGrid.GetValue(i-1,j+1); kernel[1,2] = nutrientGrid.GetValue(i,j+1); kernel[2,2] = nutrientGrid.GetValue(i+1,j+1);
                kernel[0,1] = nutrientGrid.GetValue(i-1,j);   kernel[1,1] = nutrientGrid.GetValue(i,j);   kernel[2,1] = nutrientGrid.GetValue(i+1,j);
                kernel[0,0] = nutrientGrid.GetValue(i-1,j-1); kernel[1,0] = nutrientGrid.GetValue(i,j-1); kernel[2,0] = nutrientGrid.GetValue(i+1,j-1);
                thisVal = kernel[1,1];

                int kernDir = Random.Range(0,4);

                if(kernDir == 0){
                    for(int x = 0; x < 2; x++){
                        for (int y = 0; y < 2; y++){
                            tempVal = kernel[x,y];
                        
                            if(tempVal > maxVal){
                                maxVal = tempVal;
                                maxIndex_x = x;
                                maxIndex_y = y;
                            }
                        
                            if(tempVal < minVal){
                            minVal = tempVal;
                            
                            }
                        }
                    }
                }
                if(kernDir == 1){
                    for(int x = 2; x > 0; x--){
                        for (int y = 0; y < 2; y++){
                            tempVal = kernel[x,y];
                        
                            if(tempVal > maxVal){
                                maxVal = tempVal;
                                maxIndex_x = x;
                                maxIndex_y = y;
                            }
                        
                            if(tempVal < minVal){
                            minVal = tempVal;
                            
                            }
                        }
                    }
                }
                if(kernDir == 2){
                    for(int x = 0; x < 2; x++){
                        for (int y = 2; y > 0; y--){
                            tempVal = kernel[x,y];
                        
                            if(tempVal > maxVal){
                                maxVal = tempVal;
                                maxIndex_x = x;
                                maxIndex_y = y;
                            }
                        
                            if(tempVal < minVal){
                            minVal = tempVal;
                            
                            }
                        }
                    }
                }
                if(kernDir == 3){
                    for(int x = 2; x > 0; x--){
                        for (int y = 2; y > 0; y--){
                            tempVal = kernel[x,y];
                        
                            if(tempVal > maxVal){
                                maxVal = tempVal;
                                maxIndex_x = x;
                                maxIndex_y = y;
                            }
                        
                            if(tempVal < minVal){
                            minVal = tempVal;
                            
                            }
                        }
                    }
                }
                
                
                deltaVal = maxVal - thisVal;

                if( deltaVal > 0){
                    
                    //tempMinIndex[0] = minIndices[Random.Range(0,minIndices.Count)][0];
                    //tempMinIndex[1] = minIndices[Random.Range(0,minIndices.Count)][1];
                    nutrientGrid.SetValue( i,j,thisVal+(deltaVal/256f) );
                    nutrientGrid.SetValue(i-1+maxIndex_x,j-1+maxIndex_y,maxVal-(deltaVal/256f));
                    
                }
                
                
            }
        }
        

    }

}


