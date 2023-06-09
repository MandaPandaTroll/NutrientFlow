
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.IO.Compression;

public  class StatisticsWriter : MonoBehaviour{

string EditorStatsDir;

    public int gridSampleFrequency, statSampleFrequency, autoPosSampleFreq, gamPosSampleFreq, repStatFreq, compGridDataFreq, repEventSampleFreq, deathEventSampleFreq;
   public  int sampleToCSVTimer_nutrientGrid, sampleToCSVTimer_stats, autoPosTimer, gamPosTimer, repStatTimer, compGridDataTimer, repEventTimer,deathEventTimer;

    public static bool doSampleRepEvents{get;set;}
   public static int deaths_individual{get;set;}
   public static int deaths_gamete{get;set;}
   public static int zygotesFormed{get;set;}
   string gridPath = "NutrientGridData";
   string popPath = "PopData";
   string posPath = "PositionData";
   string repPath = "RepData";
   string lockedNutePath = "LockedNutrientGridData";
   string gridPopPath = "gridPopData";
   string selfingGridPath = "selfingGridData";


    public static int[,] sampleGrid;


    void Start(){
        firstAutoPos = false;
        firstGamPos = false;
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            EditorStatsDir = new string(System.Environment.GetEnvironmentVariable("USERPROFILE")+"/UnityEditorStatsOutput/");
           EditorStatsDir = EditorStatsDir.Replace("\\", "/"); //  you need "\\" because "\" is escape symbol.
            Debug.Log(EditorStatsDir);

            if(Directory.Exists(EditorStatsDir)){
            Directory.Delete(EditorStatsDir,true);
            }
            Directory.CreateDirectory(EditorStatsDir);
        }
    

        ParamLookup.gridDims = gridDims;
        
        string writePath =  getZipPath()+"ZippedGridData/";
        string readPath = getZipPath() + gridPath + "/";
        
        
        
        
        

        if(Directory.Exists(getZipPath()+gridPath)){
            Directory.Delete(getZipPath()+gridPath,true);
        }

        if(Directory.Exists(getZipPath()+gridPopPath)){
            Directory.Delete(getZipPath()+gridPopPath,true);
        }
        if(Directory.Exists(getZipPath()+selfingGridPath)){
            Directory.Delete(getZipPath()+selfingGridPath,true);
        }
        if(Directory.Exists(getZipPath()+lockedNutePath)){
            Directory.Delete(getZipPath()+lockedNutePath,true);
        }
        if(Directory.Exists(getZipPath()+popPath)){
            Directory.Delete(getZipPath()+popPath,true);
        }
        if(Directory.Exists(getZipPath()+posPath)){
            Directory.Delete(getZipPath()+posPath,true);
        }
        if(Directory.Exists(getZipPath()+repPath)){
            Directory.Delete(getZipPath()+repPath,true);
        }
        if(Directory.Exists(getZipPath()+writePath)){
            Directory.Delete(getZipPath()+writePath,true);
        }
        if(File.Exists(getZipPath()+"params.txt")){
            File.Delete(getZipPath()+"params.txt");
        }


        Directory.CreateDirectory(getZipPath()+gridPath);
        Directory.CreateDirectory(getZipPath()+lockedNutePath);
        Directory.CreateDirectory(getZipPath()+gridPopPath);
        Directory.CreateDirectory(getZipPath()+selfingGridPath);
        Directory.CreateDirectory(getZipPath()+popPath);
        Directory.CreateDirectory(getZipPath()+posPath);
        Directory.CreateDirectory(getZipPath()+repPath);
        
        Directory.CreateDirectory(writePath);
        if(File.Exists(writePath+"GridDatas.zip")){
            File.Delete(writePath+"GridDatas.zip");}
            //File.Create(writePath+"GridDatas.zip");
            //Directory.CreateDirectory(writePath+"GridDatas");
            //ZipFile.CreateFromDirectory(writePath,writePath+"GridDatas.zip");
            //Directory.Delete(writePath+"GridDatas");
            using (FileStream fs = File.Create(writePath+"GridDatas.zip"))
            {
                fs.Close();
                fs.Dispose();
            }
        
        
        deaths_individual = 0;
        deaths_gamete = 0;
        zygotesFormed = 0;
        fileNumber_nutrient = 1000;
        fileNumber_lockedNutrient = 1000;
        fileNumber_gridPop = 1000;
        fileNumber_repEvent = 1000;
        fileNumber_deathEvent = 1000;
        fileNumber_selfingGrid = 1000;
        
        
        

        if (gridSampleFrequency < 1){
            gridSampleFrequency = 1;
        }
        if (statSampleFrequency < 1){
            statSampleFrequency = 1;
        }
        if(autoPosSampleFreq < 1){
            autoPosSampleFreq = 1;
        }
        if(gamPosSampleFreq < 1){
            gamPosSampleFreq = 1;
        }
        if(repStatFreq < 1){
            repStatFreq = 1;
        }
        sampleToCSVTimer_nutrientGrid = gridSampleFrequency -4;
        sampleToCSVTimer_stats = statSampleFrequency -4;
        autoPosTimer = autoPosSampleFreq -4;
        gamPosTimer = gamPosSampleFreq -4;
        firstStat = false;
        
        rowData = new List<string[]>();
        rowData_autoPos = new List<string[]>();
        rowData_gametePos = new List<string[]>();
        rowData_repStat = new List<string[]>();
        rowData_stats = new List<string[]>();

        ParamLookup.gridSamplePeriod = gridSampleFrequency;
        ParamLookup.statSamplePeriod = statSampleFrequency;
        ParamLookup.autoPosSamplePeriod = autoPosSampleFreq;
        ParamLookup.gamPosSamplePeriod = gamPosSampleFreq;
        ParamLookup.repStatSamplePeriod = repStatFreq;
        ParamLookup.repEventSamplePeriod = repEventSampleFreq;
        ParamLookup.deathEventSamplePeriod = deathEventSampleFreq;
        /*
        List<string> normoTestList_x = new List<string>();
        List<string> normoTestList_y = new List<string>();
        List<string> uniTestList_float_x = new List<string>();
        List<string> uniTestList_float_y = new List<string>();
        List<string> uniTestList_int_x = new List<string>();
        List<string> uniTestList_int_y = new List<string>();
        for(int i = 0; i < 1e6; i++){
            normoTestList_x.Add(ExtraMath.GetNormal(0,0.25).ToString("0.0000000"));
            normoTestList_y.Add(ExtraMath.GetNormal(0,0.25).ToString("0.0000000"));
            uniTestList_float_x.Add(Random.Range(-1f,1f).ToString("0.0000000"));
            uniTestList_float_y.Add(Random.Range(-1f,1f).ToString("0.0000000"));
            uniTestList_int_x.Add(Random.Range(0,System.Int32.MaxValue).ToString());
            uniTestList_int_y.Add(Random.Range(0,System.Int32.MaxValue).ToString());
        }*/
        
        //WriteRandomTest(normoTestList_x,normoTestList_y,uniTestList_float_x,uniTestList_float_y,uniTestList_int_x,uniTestList_int_y);
    }
    
void FixedUpdate(){

    sampleToCSVTimer_nutrientGrid += 1;
    sampleToCSVTimer_stats += 1;
    autoPosTimer += 1;
    gamPosTimer += 1;
    repStatTimer +=1;
    compGridDataTimer += 1;
    if(doSampleRepEvents){
        repEventTimer +=1;
    }
    deathEventTimer +=1;
if (sampleToCSVTimer_nutrientGrid >= gridSampleFrequency){
            sampleToCSVTimer_nutrientGrid = 0;
            WriteNutrientGrid(sampleGrid);
            //if(IndividualStats.GetNAutos() > 0 || GameteStats.GetNGamete() > 0){
            WriteLockedGridData(DiscreteGrid.lockedNutrientCoords);
            WriteGridPopData(IndividualStats.GetGridPop(Autotroph_main.individuals,gridDims));
            WriteSelfingGridData(IndividualStats.GetSelfingGrid(Autotroph_main.individuals,gridDims));
        //}
            
        }
if (sampleToCSVTimer_stats >= statSampleFrequency){
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
    if(compGridDataTimer >= compGridDataFreq && CompressionEnabled){
        compGridDataTimer = 0;
        CompressGridData();
    }
    if(doSampleRepEvents){
        if(repEventTimer >= repEventSampleFreq){
            repEventTimer = 0;
            WriteRepEvents(IndividualStats.repEvents);
        }
    }
    if(deathEventTimer >= deathEventSampleFreq){
        deathEventTimer = 0;
        WriteDeathEvents(IndividualStats.deathEvents);
    }
    

    
}
public bool CompressionEnabled;
public static int[] gridDims = new int[2];


int fileNumber_nutrient, zipNumber_nutrient, fileNumber_lockedNutrient, fileNumber_repEvent, fileNumber_deathEvent;


List<string[]> rowData = new List<string[]>();
List<string[]> rowData_stats = new List<string[]>();
List<string[]> rowData_autoPos = new List<string[]>();
List<string[]> rowData_gametePos = new List<string[]>();
List<string[]> rowData_repStat = new List<string[]>();
List<string[]> rowData_repEvent = new List<string[]>();
List<string[]> rowData_deathEvent = new List<string[]>();
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
        
        return;
    }

    public void WriteLockedGridData(int[,] grid){
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
        
        
        string filePath = getPath(lockedNutePath,"gridData_locked",fileNumber_lockedNutrient);
        string compPath = filePath;
        fileNumber_lockedNutrient += 1;
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Flush();
        outStream.Close();
        outStream.Dispose();
        return;
    }

    int fileNumber_gridPop;
    public void WriteGridPopData(int[,] grid){
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
        
        
        string filePath = getPath(gridPopPath,"gridPopData",fileNumber_gridPop);
        string compPath = filePath;
        fileNumber_gridPop += 1;
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Flush();
        outStream.Close();
        outStream.Dispose();
        
        return;
    }
    
    int fileNumber_selfingGrid;
    public void WriteSelfingGridData(int[,] grid){
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
        
        
        string filePath = getPath(selfingGridPath,"selfingGridData",fileNumber_selfingGrid);
        string compPath = filePath;
        fileNumber_selfingGrid += 1;
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Flush();
        outStream.Close();
        outStream.Dispose();
        
        return;
    }

   
    bool firstStat;
    bool firstAutoPos;
    bool firstGamPos;
    public void WriteStats(){
        
        
            
            string[] rowDataTemp = new string[15];
            
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
                rowDataTemp[13] = "meanGeneration";
                rowDataTemp[14] = "globalSelfingRatio";
                //statsHeaderCounter = 1;
                //rowData_stats.Add(rowDataTemp);
                firstStat = true;

            }else{
                rowDataTemp[0] = Mathf.FloorToInt(StatDisplay.tSecs).ToString();
                rowDataTemp[1] = GlobalTimeControls.globalSteps.ToString();
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
                rowDataTemp[13] = IndividualStats.GetMeanGeneration().ToString();
                IndividualStats.globalSelfingRatio = IndividualStats.GetGlobalSelfingRatio(Autotroph_main.individuals);
                rowDataTemp[14] = IndividualStats.globalSelfingRatio.ToString();
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
        return;
    }


 void WriteAutoTrophPositions(){
             
            int N = IndividualStats.GetNAutos();
            string[] rowDataTemp;
            
            
            if(firstAutoPos == false){
                rowDataTemp = new string[7];
                rowDataTemp[0] = "time_seconds";
                rowDataTemp[1] = "time_steps";
                rowDataTemp[2] = "individualNumber";
                rowDataTemp[3] = "x";
                rowDataTemp[4] = "y";
                rowDataTemp[5] = "nutrientLevel";
                rowDataTemp[6] = "spentNutrients";

                firstAutoPos = true;
                rowData_autoPos.Add(rowDataTemp);
            }
             for(int i = 0; i < N; i++){
                rowDataTemp = new string[7];
                rowDataTemp[0] = Mathf.FloorToInt(StatDisplay.tSecs).ToString();
                rowDataTemp[1] = GlobalTimeControls.globalSteps.ToString();
                rowDataTemp[2] = Autotroph_main.individuals[i].individualNumber.ToString();
                rowDataTemp[3] = Autotroph_main.individuals[i].coordNute[0].ToString();
                rowDataTemp[4] = Autotroph_main.individuals[i].coordNute[1].ToString();
                rowDataTemp[5] = Autotroph_main.individuals[i].nutrientLevel.ToString();
                rowDataTemp[6] = Autotroph_main.individuals[i].totalSpentNutrients.ToString();
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
        return;
}



int repit;
void WriteReproductiveData(List<Autotroph_main> inds){
    repit += 1;
    int N = IndividualStats.GetNAutos();
    
   
            string secs = Mathf.FloorToInt(StatDisplay.tSecs).ToString();
            string steps = GlobalTimeControls.globalSteps.ToString();
            string[] rowDataTemp;
            
           
            
            if(repit == 1){
                rowDataTemp = new string[12];
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
                rowDataTemp[10] = "energyLevel";
                rowDataTemp[11] = "nutrientLevel";

                rowData_repStat.Add(rowDataTemp);
                
            }
                for(int i = 0; i < N; i++){
                rowDataTemp = new string[12];
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
                rowDataTemp[10] = inds[i].energyLevel.ToString();
                rowDataTemp[11] = inds[i].nutrientLevel.ToString();
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
        return;

}

//time_steps, indnum, preNute, postNute, preEnergy, postEnergy, repType
//int repEventIt;
void WriteRepEvents(List<string[]> repevents){
    //repEventIt += 1;
    rowData_repEvent.Clear();
    int N = repevents.Count;
   
            
            
            string[] rowDataTemp;
            
           
            
            //if(repit == 1){
                rowDataTemp = new string[20];
                rowDataTemp[0] = "time_steps";
                rowDataTemp[1] = "individualNumber";
                rowDataTemp[2] = "repEventNumber";
                rowDataTemp[3] = "preNute";
                rowDataTemp[4] = "postNute";
                rowDataTemp[5] = "preEnergy";
                rowDataTemp[6] = "postEnergy";
                rowDataTemp[7] = "repType";
                rowDataTemp[8] = "age";
                rowDataTemp[9] = "migrations_left";
                rowDataTemp[10] = "migrations_right";
                rowDataTemp[11] = "migrations_up";
                rowDataTemp[12] = "migrations_down";
                rowDataTemp[13] = "migrations_upLeft";
                rowDataTemp[14] = "migrations_upRight";
                rowDataTemp[15] = "migrations_downLeft";
                rowDataTemp[16] = "migrations_downRight";
                rowDataTemp[17] = "pos_x";
                rowDataTemp[18] = "pos_y";
                rowDataTemp[19] = "maxlifespan";

                rowData_repEvent.Add(rowDataTemp);
                
            //}
            //time_steps, indnum, preNute, postNute, preEnergy, postEnergy, repType
                for(int i = 0; i < N; i++){
                rowDataTemp = new string[20];
                rowDataTemp[0] = repevents[i][0];
                rowDataTemp[1] = repevents[i][1];
                rowDataTemp[2] = repevents[i][2];
                rowDataTemp[3] = repevents[i][3];
                rowDataTemp[4] = repevents[i][4];
                rowDataTemp[5] = repevents[i][5];
                rowDataTemp[6] = repevents[i][6];
                rowDataTemp[7] = repevents[i][7];
                rowDataTemp[8] = repevents[i][8];
                rowDataTemp[9] = repevents[i][9];
                rowDataTemp[10] = repevents[i][10];
                rowDataTemp[11] = repevents[i][11];
                rowDataTemp[12] = repevents[i][12];
                rowDataTemp[13] = repevents[i][13];
                rowDataTemp[14] = repevents[i][14];
                rowDataTemp[15] = repevents[i][15];
                rowDataTemp[16] = repevents[i][16];
                rowDataTemp[17] = repevents[i][17];
                rowDataTemp[18] = repevents[i][18];
                rowDataTemp[19] = repevents[i][19];

                rowData_repEvent.Add(rowDataTemp);
                
                
             }
             
            
            //rowData_repStat.Add(rowDataTemp);
             
            
            
            
            
            

        string[][] output = new string[rowData_repEvent.Count][];

        for(int i = 0; i < output.Length; i++){
            output[i] = rowData_repEvent[i];
        }

        int     length         = output.GetLength(0);
        string     delimiter     = ",";

        StringBuilder sb = new StringBuilder();
        
        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));
        
        
        string filePath = getPath(repPath,"repEvents",fileNumber_repEvent);
        
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Flush();
        outStream.Close();
        outStream.Dispose();
        fileNumber_repEvent +=1;
        IndividualStats.repEvents.Clear();
        return;

}


void WriteDeathEvents(List<string[]> deathevents){
    //repEventIt += 1;
    rowData_deathEvent.Clear();
    int N = deathevents.Count;
   
            
            
            string[] rowDataTemp;
            
           
            
            
                rowDataTemp = new string[22];
                rowDataTemp[0] = "time_steps";
                rowDataTemp[1] = "individualNumber";
                rowDataTemp[2] = "causeOfDeath";
                rowDataTemp[3] = "age";
                rowDataTemp[4] = "maximumLifeSpan";
                rowDataTemp[5] = "generation";
                rowDataTemp[6] = "nutrientLevel";
                rowDataTemp[7] = "spentNutrients";
                rowDataTemp[8] = "energyLevel";
                rowDataTemp[9] = "reproductiveEvents";
                rowDataTemp[10] = "migrations_left";
                rowDataTemp[11] = "migrations_right";
                rowDataTemp[12] = "migrations_up";
                rowDataTemp[13] = "migrations_down";
                rowDataTemp[14] = "migrations_upLeft";
                rowDataTemp[15] = "migrations_upRight";
                rowDataTemp[16] = "migrations_downLeft";
                rowDataTemp[17] = "migrations_downRight";
                rowDataTemp[18] = "pos_x";
                rowDataTemp[19] = "pos_y";
                rowDataTemp[20] = "reachedMaturityAge";
                rowDataTemp[21] = "maturity";




                rowData_deathEvent.Add(rowDataTemp);
                
            
            
                for(int i = 0; i < N; i++){
                rowDataTemp = new string[22];
                rowDataTemp[0] = deathevents[i][0];
                rowDataTemp[1] = deathevents[i][1];
                rowDataTemp[2] = deathevents[i][2];
                rowDataTemp[3] = deathevents[i][3];
                rowDataTemp[4] = deathevents[i][4];
                rowDataTemp[5] = deathevents[i][5];
                rowDataTemp[6] = deathevents[i][6];
                rowDataTemp[7] = deathevents[i][7];
                rowDataTemp[8] = deathevents[i][8];
                rowDataTemp[9] = deathevents[i][9];
                rowDataTemp[10] = deathevents[i][10];
                rowDataTemp[11] = deathevents[i][11];
                rowDataTemp[12] = deathevents[i][12];
                rowDataTemp[13] = deathevents[i][13];
                rowDataTemp[14] = deathevents[i][14];
                rowDataTemp[15] = deathevents[i][15];
                rowDataTemp[16] = deathevents[i][16];
                rowDataTemp[17] = deathevents[i][17];
                rowDataTemp[18] = deathevents[i][18];
                rowDataTemp[19] = deathevents[i][19];
                rowDataTemp[20] = deathevents[i][20];
                rowDataTemp[21] = deathevents[i][21];

                rowData_deathEvent.Add(rowDataTemp);
                
                
             }
             
            
            //rowData_repStat.Add(rowDataTemp);
             
            
            
            
            
            

        string[][] output = new string[rowData_deathEvent.Count][];

        for(int i = 0; i < output.Length; i++){
            output[i] = rowData_deathEvent[i];
        }

        int     length         = output.GetLength(0);
        string     delimiter     = ",";

        StringBuilder sb = new StringBuilder();
        
        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));
        
        
        string filePath = getPath(popPath,"deathEvents",fileNumber_deathEvent);
        
        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Flush();
        outStream.Close();
        outStream.Dispose();
        fileNumber_deathEvent += 1;
        IndividualStats.deathEvents.Clear();
        return;

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
                rowDataTemp[1] = GlobalTimeControls.globalSteps.ToString();
                if(GameteMain.gameteScripts[i] != null){
                    rowDataTemp[2] = GameteMain.gameteScripts[i].gameteNumber.ToString();
                    rowDataTemp[3] = GameteMain.gameteScripts[i].coordNute[0].ToString();
                    rowDataTemp[4] = GameteMain.gameteScripts[i].coordNute[1].ToString();
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
        return;
}


    private  string getPath(string subDir, string fileName, int fileNumber){
        
        #if UNITY_EDITOR
        //return Application.dataPath +"/CSV/"+subDir+"/"+fileName+ fileNumber+ ".csv";
        return "C:/Users/gushanamc/UnityEditorStatsOutput/"+subDir+"/"+fileName+fileNumber+".csv";
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
        //return Application.dataPath +"/CSV/";
        return "C:/Users/gushanamc/UnityEditorStatsOutput/";
        #elif UNITY_ANDROID
        return Application.persistentDataPath+"/";
        #elif UNITY_STANDALONE_OSX
        return Application.dataPath+"/";
        #elif UNITY_SERVER
        return Application.dataPath+"/";
        #else
        return Application.dataPath +"/";
        #endif
    }

    static string staticGetPath(){
        #if UNITY_EDITOR
        //return Application.dataPath +"/CSV/";
        return "C:/Users/gushanamc/UnityEditorStatsOutput/";
        #elif UNITY_ANDROID
        return Application.persistentDataPath+"/";
        #elif UNITY_STANDALONE_OSX
        return Application.dataPath+"/";
        #elif UNITY_SERVER
        return Application.dataPath+"/";
        #else
        return Application.dataPath +"/";
        #endif
    }
    
    void CompressGridData(){
        
         compGridDataTimer = 0;
        string writePath =  getZipPath()+"ZippedGridData/";
        string readPath = getZipPath() + gridPath + "/";

        string sourceDirectory = readPath;
            string archiveDirectory = writePath;
        //var files = Directory.EnumerateFiles(sourceDirectory, "*.csv", SearchOption.AllDirectories);

        

       var files = Directory.GetFiles(readPath);
        
        
        
        using (FileStream zipToOpen = new FileStream(writePath+"GridDatas.zip", FileMode.Open)){
            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
            
                {
                    
                    foreach(string file in files){
                        
                        ZipArchiveEntry entry = archive.CreateEntryFromFile(file,Path.GetFileName(file));
                        //entry = File.Copy(file,)
                    }
                    
                    archive.Dispose();
                    
                    zipToOpen.Close();
                    zipToOpen.Flush();
                    zipToOpen.Dispose();
                
                }



                
        }
        
       
       
        Directory.Delete(readPath,true);
        Directory.CreateDirectory(readPath);
        
        
        

        compGridDataTimer = 0;
        zipNumber_nutrient+=1;
    }


public static Vector2 GetScaledPosition(Vector2 input){
    float box0 = DiscreteGrid.boxSize.x/2f;
    float box1 =DiscreteGrid.boxSize.y/2f;
    Vector2 output = new Vector2((input.x+box0)/(2f*box1),(input.y+box1)/(2f*box1));
    return output;
}

public static void WriteRandomTest(List<string> norm_x, List<string> norm_y,List<string> uniFloat_x,List<string> uniFloat_y,List<string> uniInt_x,List<string> uniInt_y){
string path = StatisticsWriter.staticGetPath()+"randos.txt";
if(File.Exists(path)){
            File.Delete(path);
        }
        
        using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("norm_x,norm_y,unifloat_x,unifloat_y,uniint_x,uniint_y");
                for(int i = 0; i < norm_x.Count; i++){
                    sw.WriteLine(norm_x[i]+","+norm_y[i]+","+uniFloat_x[i]+","+uniFloat_y[i]+","+uniInt_x[i]+","+uniInt_y[i]);
                }
                
                sw.Close();
                
                sw.Dispose();
                
                
            }

}



public static void WriteParams(){
string path = StatisticsWriter.staticGetPath()+"params.txt";
        if(File.Exists(path)){
            File.Delete(path);
        }
        
            
            using (StreamWriter sw = File.CreateText(path))
            {
                
                sw.WriteLine("Parameter"+","+"Value");
                sw.WriteLine("Start.time" +","+System.DateTime.Now+ "\n");
                sw.WriteLine("Grid.width" +","+ParamLookup.gridDims[0]+ "\n");
                sw.WriteLine("Grid.height" +","+ParamLookup.gridDims[1]+ "\n");
                sw.WriteLine("Mean.global.nutrient.concentration" +","+ ParamLookup.initConc + "\n");
                sw.WriteLine("Nutrients.total"+","+ParamLookup.totalNutrients + "\n");
                sw.WriteLine("Nutrient.initial.distribution"+","+ParamLookup.initDistribution + "\n");
                sw.WriteLine("Diffusion.period"+","+ParamLookup.DiffusionPeriod + "\n");
                sw.WriteLine("Diffusion.limit"+","+ParamLookup.diffusionLimit + "\n");
                sw.WriteLine("Population.initial.size: "+","+ParamLookup.initPop + "\n");
                sw.WriteLine("Sample.period.grid"+","+ParamLookup.gridSamplePeriod + "\n");
                sw.WriteLine("Sample.period.PopStat"+","+ParamLookup.statSamplePeriod + "\n");
                sw.WriteLine("Sample.period.Autotroph.position"+","+ParamLookup.autoPosSamplePeriod + "\n");
                sw.WriteLine("Sample.period.Gamete.position"+","+ParamLookup.gamPosSamplePeriod + "\n");
                sw.WriteLine("Sample.period.RepStat"+","+ParamLookup.repStatSamplePeriod);
                sw.WriteLine("Quit.on.global.extinction"+","+ParamLookup.QuitOnGlobalExtinction);
                sw.WriteLine("Quit.at.time"+","+ParamLookup.QuitAtTime);
                sw.WriteLine("Quit.time: "+","+ParamLookup.quitTime);
                sw.WriteLine("Quit.at.generation"+","+ParamLookup.QuitAtGeneration);
                sw.WriteLine("Quit.generation"+","+ParamLookup.quitGeneration);
                sw.WriteLine("Reproduction.mode"+","+ParamLookup.ModeOfReproduction);
                sw.WriteLine("Juvenile.migration"+","+ParamLookup.juvenileMigration);
                sw.WriteLine("P.migration"+","+ParamLookup.pMigration);
                sw.WriteLine("Simultaneous.rep.mig"+","+ParamLookup.simultRepMig);
                sw.WriteLine("Mean.maximum.lifespan"+","+AccessibleGlobalSettings.meanMaximumLifeSpan);
                sw.WriteLine("SD.maximum.lifespan"+","+AccessibleGlobalSettings.std_lifeSpan);
                sw.WriteLine("Nutrient.requirement.gamete"+","+ParamLookup.gameteCost_nutrient);
                sw.WriteLine("Energy.requirement.gamete"+","+ParamLookup.gameteCost_energy);
                sw.WriteLine("Nutrient.requirement.growth"+","+ParamLookup.growthCost_nutrient);
                sw.WriteLine("Energy.requirement.growth"+","+ParamLookup.growthCost_energy);
                sw.WriteLine("Nutrient.requirement.growth.total"+","+ (ParamLookup.growthCost_nutrient*(1f/ParamLookup.growthRate)));
                sw.WriteLine("Energy.requirement.growth.total"+","+(ParamLookup.growthCost_energy*(1f/ParamLookup.growthRate)));
                sw.WriteLine("Maturity.coefficient"+","+ParamLookup.maturityCoef);


                sw.WriteLine("Circle.radius"+","+ParamLookup.circleRadius);
                
                
                sw.Close();
                
                sw.Dispose();

                Debug.Log(
                "–––––––––––––––––––––––––"+ "\n"+
                "Grid dimensions: ["+ParamLookup.gridDims[0]+","+ParamLookup.gridDims[1]+"]" + "\n"+
                "Mean global nutrient concentration: " + ParamLookup.initConc + "\n"+
                "Total nutrients: "+ParamLookup.totalNutrients + "\n"+
                "Initial nutrient distribution: "+ParamLookup.initDistribution + "\n"+
                "Diffusion period: "+ParamLookup.DiffusionPeriod + "\n"+
                "Diffusion limit: "+ParamLookup.diffusionLimit + "\n"+
                "Initial population size: "+ParamLookup.initPop + "\n"+
                "Grid sample period: "+ParamLookup.gridSamplePeriod + "\n"+
                "PopStat sample period: "+ParamLookup.statSamplePeriod + "\n"+
                "Autotroph position sample period: "+ParamLookup.autoPosSamplePeriod + "\n"+
                "Gamete position sample period: "+ParamLookup.gamPosSamplePeriod + "\n"+
                "RepStat sample period: "+ParamLookup.repStatSamplePeriod+ "\n"+
                "Quit on global extinction: "+ParamLookup.QuitOnGlobalExtinction+ "\n"+
                "Quit at time: "+ParamLookup.QuitAtTime+ "\n"+
                "Quit time: "+ParamLookup.quitTime+ "\n"+
                "Quit at generation: "+ParamLookup.QuitAtGeneration+ "\n"+
                "Quit generation: "+ParamLookup.quitGeneration+ "\n"+
                "Circle radius: "+ParamLookup.circleRadius+ "\n"+
                "Mode of reproduction: "+ParamLookup.ModeOfReproduction+ "\n"+
                "Juvenile.migration: "+ParamLookup.juvenileMigration+"\n"+
                "P.migration: "+ParamLookup.pMigration+"\n"+
                "Simultaneous.rep.mig: "+ParamLookup.simultRepMig+"\n"+

                
                "–––––––––––––––––––––––––"
                );
                
                
            }	


}

}





