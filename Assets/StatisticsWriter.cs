
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.IO.Compression;
public  class StatisticsWriter : MonoBehaviour{



    public int gridSampleFrequency, statSampleFrequency, autoPosSampleFreq, gamPosSampleFreq, repStatFreq, compGridDataFreq;
   public  int sampleToCSVTimer_nutrientGrid, sampleToCSVTimer_stats, autoPosTimer, gamPosTimer, repStatTimer, compGridDataTimer;

   public static int deaths_individual{get;set;}
   public static int deaths_gamete{get;set;}
   public static int zygotesFormed{get;set;}
   string gridPath = "NutrientGridData";
   string popPath = "PopData";
   string posPath = "PositionData";
   string repPath = "RepData";


    public static int[,] sampleGrid;
    void Start(){
        deaths_individual = 0;
        deaths_gamete = 0;
        zygotesFormed = 0;
        fileNumber_nutrient = 1000;
        
        
        

        if (gridSampleFrequency < 2){
            gridSampleFrequency = 2;
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
        if(repStatFreq < 31){
            repStatFreq = 31;
        }
        sampleToCSVTimer_nutrientGrid = gridSampleFrequency -2;
        sampleToCSVTimer_stats = statSampleFrequency -2;
        firstStat = false;
        
        rowData = new List<string[]>();
        rowData_autoPos = new List<string[]>();
        rowData_gametePos = new List<string[]>();
        rowData_repStat = new List<string[]>();
        rowData_stats = new List<string[]>();
    }
void FixedUpdate(){

    sampleToCSVTimer_nutrientGrid += 1;
    sampleToCSVTimer_stats += 1;
    autoPosTimer += 1;
    gamPosTimer += 1;
    repStatTimer +=1;
    compGridDataTimer += 1;
if (sampleToCSVTimer_nutrientGrid >= gridSampleFrequency){
            sampleToCSVTimer_nutrientGrid = 0;
            WriteNutrientGrid(sampleGrid);
            
        }
if (sampleToCSVTimer_stats == statSampleFrequency){
            sampleToCSVTimer_stats = 0;
            //Debug.Log("indDiff = " + IndividualStats.CheckPopulationSize());
            //Debug.Log("gamDiff = " + IndividualStats.CheckGameteSize());
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
if (repStatTimer >= repStatFreq){
            repStatTimer = 0;
            
            WriteReproductiveData(Autotroph_main.individuals);
            
        }
    if(compGridDataTimer >= compGridDataFreq){
        CompressGridData();
    }

    
}

public static int[] gridDims = new int[2];


int fileNumber_nutrient, zipNumber_nutrient;


List<string[]> rowData = new List<string[]>();
List<string[]> rowData_stats = new List<string[]>();
List<string[]> rowData_autoPos = new List<string[]>();
List<string[]> rowData_gametePos = new List<string[]>();
List<string[]> rowData_repStat = new List<string[]>();
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
        
        
        string filePath = getPath(gridPath,"gridData",fileNumber_nutrient);
        string compPath = filePath;
        fileNumber_nutrient += 1;
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Flush();
        outStream.Close();
        outStream.Dispose();
        
    }

   
    bool firstStat;
    public void WriteStats(){
        
        
            
            string[] rowDataTemp = new string[13];
            
            if(firstStat == false){
                rowDataTemp[0] = "time_seconds";
                rowDataTemp[1] = "time_steps";
                rowDataTemp[2] = "autotrophs";
                rowDataTemp[3] = "gametes";
                rowDataTemp[4] = "nutrients_free";
                rowDataTemp[5] = "nutrients_locked";
                rowDataTemp[6] = "nutrients_autotroph";
                rowDataTemp[7] = "nutrients_gamete";
                rowDataTemp[8] = "nutrients_total";
                rowDataTemp[9] = "deaths_individual";
                rowDataTemp[10] = "deaths_gamete";
                rowDataTemp[11] = "zygotesFormed";
                rowDataTemp[12] = "gametes_total";
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
                rowDataTemp[6] = IndividualStats.GetSumNutrients().ToString();
                rowDataTemp[7] = GameteStats.GetSumNutrients().ToString();
                rowDataTemp[8] = nutrientStats.totalNutrients.ToString();
                rowDataTemp[9] = deaths_individual.ToString();
                rowDataTemp[10] = deaths_gamete.ToString();
                rowDataTemp[11] = zygotesFormed.ToString();
                rowDataTemp[12] = GameteMain.gameteCount.ToString();
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
        
        
        string filePath = getPath(popPath,"Stats",0);
        
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
            
            
            if(autoPosHeader == 0){
                rowDataTemp = new string[7];
                rowDataTemp[0] = "time_seconds";
                rowDataTemp[1] = "time_steps";
                rowDataTemp[2] = "individualNumber";
                rowDataTemp[3] = "x";
                rowDataTemp[4] = "y";
                rowDataTemp[5] = "nutrientLevel";
                rowDataTemp[6] = "spentNutrients";

                autoPosHeader += 1;
                rowData_autoPos.Add(rowDataTemp);
            }else{

            }
             for(int i = 0; i < N; i++){
                rowDataTemp = new string[7];
                rowDataTemp[0] = Mathf.FloorToInt(StatDisplay.tSecs).ToString();
                rowDataTemp[1] = StatDisplay.tSteps.ToString();
                rowDataTemp[2] = Autotroph_main.individuals[i].individualNumber.ToString();
                rowDataTemp[3] = GetScaledPosition(Autotroph_main.individuals[i].transform.position).x.ToString();
                rowDataTemp[4] = GetScaledPosition(Autotroph_main.individuals[i].transform.position).y.ToString();
                rowDataTemp[5] = Autotroph_main.individuals[i].nutrientLevel.ToString();
                rowDataTemp[6] = Autotroph_main.individuals[i].spentNutrients.ToString();
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
        
        
        string filePath = getPath(posPath,"AutotrophPositions",0);
        
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Flush();
        outStream.Close();
        outStream.Dispose();
}



int repit;
void WriteReproductiveData(List<Autotroph_main> inds){
    repit += 1;
    int N = IndividualStats.GetNAutos();
    
   
            string secs = Mathf.FloorToInt(StatDisplay.tSecs).ToString();
            string steps = StatDisplay.tSteps.ToString();
            string[] rowDataTemp;
            
           
            
            if(repit == 1){
                rowDataTemp = new string[10];
                rowDataTemp[0] = "time_seconds";
                rowDataTemp[1] = "time_steps";
                rowDataTemp[2] = "individualNumber";
                rowDataTemp[3] = "parentGameteA";
                rowDataTemp[4] = "parentGameteB";
                rowDataTemp[5] = "instanceID";
                rowDataTemp[6] = "maximumLifeSpan";
                rowDataTemp[7] = "gametesProduced";
                rowDataTemp[8] = "age_steps";
                rowDataTemp[9] = "generation";
                rowData_repStat.Add(rowDataTemp);
                
            }
                for(int i = 0; i < N; i++){
                rowDataTemp = new string[10];
                rowDataTemp[0] = secs;
                rowDataTemp[1] = steps;
                rowDataTemp[2] = inds[i].individualNumber.ToString();
                rowDataTemp[3] = inds[i].parentGametes[0].ToString();
                rowDataTemp[4] = inds[i].parentGametes[1].ToString();
                rowDataTemp[5] = inds[i].GetInstanceID().ToString();
                rowDataTemp[6] = inds[i].maximumLifeSpan.ToString();
                rowDataTemp[7] = inds[i].gametesProduced.ToString();
                rowDataTemp[8] = inds[i].age.ToString();
                rowDataTemp[9] = inds[i].generation.ToString();
                rowData_repStat.Add(rowDataTemp);
                
                
             }
             
            
            //rowData_repStat.Add(rowDataTemp);
             
            
            
            
            
            

        string[][] output = new string[rowData_repStat.Count][];

        for(int i = 0; i < output.Length; i++){
            output[i] = rowData_repStat[i];
        }

        int     length         = output.GetLength(0);
        string     delimiter     = ",";

        StringBuilder sb = new StringBuilder();
        
        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));
        
        
        string filePath = getPath(repPath,"repStat",0);
        
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
            
            
            if(gametePosHeader == 0){
                rowDataTemp = new string[5];
                rowDataTemp[0] = "time_seconds";
                rowDataTemp[1] = "time_steps";
                rowDataTemp[2] = "gameteNumber";
                rowDataTemp[3] = "x";
                rowDataTemp[4] = "y";
                gametePosHeader += 1;
                rowData_gametePos.Add(rowDataTemp);
            }else{

            }
             for(int i = 0; i < N; i++){
                rowDataTemp = new string[5];
                rowDataTemp[0] = Mathf.FloorToInt(StatDisplay.tSecs).ToString();
                rowDataTemp[1] = StatDisplay.tSteps.ToString();
                if(GameteMain.gameteScripts[i] != null){
                    rowDataTemp[2] = GameteMain.gameteScripts[i].gameteNumber.ToString();
                    rowDataTemp[3] = GetScaledPosition(GameteMain.gameteScripts[i].transform.position).x.ToString();
                    rowDataTemp[4] = GetScaledPosition(GameteMain.gameteScripts[i].transform.position).y.ToString();
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
        
        
        string filePath = getPath(posPath, "GametePositions",0);
        ;
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Flush();
        outStream.Close();
        outStream.Dispose();
}


    private  string getPath(string subDir, string fileName, int fileNumber){
        
        #if UNITY_EDITOR
        return Application.dataPath +"/CSV/"+subDir+"/"+fileName+ fileNumber+ ".csv";
        #elif UNITY_ANDROID
        return Application.persistentDataPath"/"+subDir+"/"+fileName+ fileNumber+ ".csv";
        #elif UNITY_STANDALONE_OSX
        return Application.dataPath+"/"+subDir+"/"+fileName+ fileNumber+ ".csv";
        #else
        return Application.dataPath +"/"+subDir+"/"+fileName+ fileNumber+ ".csv";
        #endif
    }
    
    private string getZipPath(){
        #if UNITY_EDITOR
        return Application.dataPath +"/CSV/";
        #elif UNITY_ANDROID
        return Application.persistentDataPath+"/";
        #elif UNITY_STANDALONE_OSX
        return Application.dataPath+"/";
        #else
        return Application.dataPath +"/";
        #endif
    }
    void CompressGridData(){
        
        string writePath = new string(getZipPath()+"ZippedGridData/");
        string readPath = new string(getZipPath() + gridPath + "/");
        

        if(System.IO.File.Exists(writePath+"GridDatas"+zipNumber_nutrient+".zip")){
            System.IO.File.Delete(writePath+"GridDatas"+zipNumber_nutrient+".zip");
        }
        ZipFile.CreateFromDirectory(readPath,writePath+"GridDatas"+zipNumber_nutrient+".zip");
        
        foreach(string file in System.IO.Directory.GetFiles(readPath, "*.csv"))
        {
        System.IO.File.Delete(file);
        }
        foreach(string file in System.IO.Directory.GetFiles(readPath, " *.csv.meta"))
        {
        System.IO.File.Delete(file);
        }
        

        compGridDataTimer = 0;
        zipNumber_nutrient+=1;
    }


public static Vector2 GetScaledPosition(Vector2 input){
    float box0 = DiscreteGrid.boxSize.x/2f;
    float box1 =DiscreteGrid.boxSize.y/2f;
    Vector2 output = new Vector2((input.x+box0)/(2f*box1),(input.y+box1)/(2f*box1));
    return output;
}

}


