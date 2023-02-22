
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
public  class StatisticsWriter : MonoBehaviour{



    public int gridSampleFrequency, statSampleFrequency, autoPosSampleFreq, gamPosSampleFreq;
   public  int sampleToCSVTimer_nutrientGrid, sampleToCSVTimer_stats, autoPosTimer, gamPosTimer;
    public static int[,] sampleGrid;
    void Start(){
        fileNumber_nutrient = 1000;
        fileNumber_stats = 1000;
        
        statsHeaderCounter = 0;

        if (gridSampleFrequency < 31){
            gridSampleFrequency = 31;
        }
        if (statSampleFrequency < 31){
            statSampleFrequency = 31;
        }
        if(autoPosSampleFreq < 31){
            autoPosSampleFreq = 31;
        }
        if(gamPosSampleFreq < 31){
            gamPosSampleFreq = 31;
        }
        sampleToCSVTimer_nutrientGrid = gridSampleFrequency -4;
        sampleToCSVTimer_stats = statSampleFrequency -4;
        firstStat = false;
        
    }
void FixedUpdate(){
    sampleToCSVTimer_nutrientGrid += 1;
    sampleToCSVTimer_stats += 1;
    autoPosTimer += 1;
    gamPosTimer += 1;
if (sampleToCSVTimer_nutrientGrid >= gridSampleFrequency){
            sampleToCSVTimer_nutrientGrid = 0;
            WriteNutrientGrid(sampleGrid);
            
        }
if (sampleToCSVTimer_stats == statSampleFrequency){
            sampleToCSVTimer_stats = 0;
            WriteStats();
            
        }
if (autoPosTimer >= autoPosSampleFreq){
            autoPosTimer = 0;
            WriteAutoTrophPositions();
            
        }
if (gamPosTimer >= gamPosSampleFreq){
            gamPosTimer = 0;
            WriteGametePositions();
            
        }
    
}

public static int[] gridDims = new int[2];


int fileNumber_nutrient;
int fileNumber_stats;
string fileNumString;
List<string[]> rowData = new List<string[]>();
List<string[]> rowData_stats = new List<string[]>();
List<string[]> rowData_autoPos = new List<string[]>();
List<string[]> rowData_gametePos = new List<string[]>();
public  void WriteNutrientGrid(int[,] grid){
     rowData.Clear();   
            
            string[] rowDataTemp;
             
            
            for(int y = 0; y < gridDims[1]; y++){
                rowDataTemp = new string[gridDims[0]];
                for(int x = 0; x < gridDims[0]; x++){
                rowDataTemp[x] = grid[x,y].ToString();
                }
                rowData.Add(rowDataTemp);
            }
            
            
            

        string[][] output = new string[rowData.Count][];

        for(int i = 0; i < output.Length; i++){
            output[i] = rowData[i];
        }

        int     length         = output.GetLength(0);
        string     delimiter     = ",";

        StringBuilder sb = new StringBuilder();
        
        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));
        
        
        string filePath = getPath("gridData",fileNumber_nutrient);
        fileNumber_nutrient += 1;
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Flush();
        outStream.Close();
        outStream.Dispose();
        
    }

    int statsHeaderCounter = 0;
    bool firstStat;
    public void WriteStats(){
        
        
            
            string[] rowDataTemp = new string[7];
            
            if(firstStat == false){
                rowDataTemp[0] = "time_seconds";
                rowDataTemp[1] = "time_steps";
                rowDataTemp[2] = "autotrophs";
                rowDataTemp[3] = "gametes";
                rowDataTemp[4] = "nutrients_free";
                rowDataTemp[5] = "nutrients_locked";
                rowDataTemp[6] = "nutrients_total";
                //statsHeaderCounter = 1;
                //rowData_stats.Add(rowDataTemp);
                firstStat = true;

            }else{
                rowDataTemp[0] = Mathf.FloorToInt(StatDisplay.tSecs).ToString();
                rowDataTemp[1] = StatDisplay.tSteps.ToString();
                rowDataTemp[2] = IndividualStats.GetNAutos().ToString();
                rowDataTemp[3] = GameteStats.GetNGamete().ToString();
                rowDataTemp[4] = nutrientStats.freeNutrients.ToString();
                rowDataTemp[5] = nutrientStats.lockedNutrients.ToString();
                rowDataTemp[6] = nutrientStats.totalNutrients.ToString();
            }
            
            
                
                
                rowData_stats.Add(rowDataTemp);
            
            
            
            

        string[][] output = new string[rowData_stats.Count][];

        for(int i = 0; i < output.Length; i++){
            output[i] = rowData_stats[i];
        }

        int     length         = output.GetLength(0);
        string     delimiter     = ",";

        StringBuilder sb = new StringBuilder();
        
        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));
        
        
        string filePath = getPath("Stats",0);
        
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Flush();
        outStream.Close();
        outStream.Dispose();
        
    }

int autoPosHeader = 0;
 void WriteAutoTrophPositions(){
             
            int N = IndividualStats.GetNAutos();
            string[] rowDataTemp;
            
            rowDataTemp = new string[4];
            if(autoPosHeader == 0){
                rowDataTemp[0] = "time_seconds";
                rowDataTemp[1] = "time_steps";
                rowDataTemp[2] = "x";
                rowDataTemp[3] = "y";
                autoPosHeader += 1;
                rowData_autoPos.Add(rowDataTemp);
            }else{

            }
             for(int i = 0; i < N; i++){
                rowDataTemp[0] = Mathf.FloorToInt(StatDisplay.tSecs).ToString();
                rowDataTemp[1] = StatDisplay.tSteps.ToString();
                rowDataTemp[2] = GetScaledPosition(Autotroph_main.individuals[i].transform.position).x.ToString();
                rowDataTemp[3] = GetScaledPosition(Autotroph_main.individuals[i].transform.position).y.ToString();
                rowData_autoPos.Add(rowDataTemp);
             }
            
            
            
            
            

        string[][] output = new string[rowData_autoPos.Count][];

        for(int i = 0; i < output.Length; i++){
            output[i] = rowData_autoPos[i];
        }

        int     length         = output.GetLength(0);
        string     delimiter     = ",";

        StringBuilder sb = new StringBuilder();
        
        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));
        
        
        string filePath = getPath("AutotrophPositions",0);
        
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Flush();
        outStream.Close();
        outStream.Dispose();
}

int gametePosHeader = 0;

void WriteGametePositions(){
              
            int N = GameteStats.GetNGamete();
            string[] rowDataTemp;
            
            rowDataTemp = new string[4];
            if(gametePosHeader == 0){
                rowDataTemp[0] = "time_seconds";
                rowDataTemp[1] = "time_steps";
                rowDataTemp[2] = "x";
                rowDataTemp[3] = "y";
                gametePosHeader += 1;
                rowData_gametePos.Add(rowDataTemp);
            }else{

            }
             for(int i = 0; i < N; i++){
                rowDataTemp[0] = Mathf.FloorToInt(StatDisplay.tSecs).ToString();
                rowDataTemp[1] = StatDisplay.tSteps.ToString();
                if(GameteMain.gameteScripts[i] != null){
                    rowDataTemp[2] = GetScaledPosition(GameteMain.gameteScripts[i].transform.position).x.ToString();
                    rowDataTemp[3] = GetScaledPosition(GameteMain.gameteScripts[i].transform.position).y.ToString();
                    rowData_gametePos.Add(rowDataTemp);
                }/*else{
                    rowDataTemp[2] = "0";
                    rowDataTemp[3] = "0";
                }*/
                
                
             }
            
            
            
            
            

        string[][] output = new string[rowData_gametePos.Count][];

        for(int i = 0; i < output.Length; i++){
            output[i] = rowData_gametePos[i];
        }

        int     length         = output.GetLength(0);
        string     delimiter     = ",";

        StringBuilder sb = new StringBuilder();
        
        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));
        
        
        string filePath = getPath("GametePositions",0);
        ;
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Flush();
        outStream.Close();
        outStream.Dispose();
}


    private  string getPath(string fileName, int fileNumber){
        
        #if UNITY_EDITOR
        return Application.dataPath +"/CSV/"+fileName+ fileNumber+ ".csv";
        #elif UNITY_ANDROID
        return Application.persistentDataPath+fileName"+ fileNumber+ ".csv";
        #elif UNITY_STANDALONE_OSX
        return Application.dataPath+"/"+fileName+ fileNumber+ ".csv";
        #else
        return Application.dataPath +"/"+fileName+ fileNumber+ ".csv";
        #endif
    }


public static Vector2 GetScaledPosition(Vector2 input){
    float box0 = DiscreteGrid.boxSize.x/2f;
    float box1 =DiscreteGrid.boxSize.y/2f;
    Vector2 output = new Vector2((input.x+box0)/(2f*box1),(input.y+box1)/(2f*box1));
    return output;
}

}


