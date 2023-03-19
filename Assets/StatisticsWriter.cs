
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

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

        ParamLookup.gridDims = gridDims;
        
        string writePath =  getZipPath()+"ZippedGridData/";
        string readPath = getZipPath() + gridPath + "/";
        
        
        
        
        

        if(Directory.Exists(getZipPath()+gridPath)){
            Directory.Delete(getZipPath()+gridPath,true);
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

        ParamLookup.gridSamplePeriod = gridSampleFrequency;
        ParamLookup.statSamplePeriod = statSampleFrequency;
        ParamLookup.autoPosSamplePeriod = autoPosSampleFreq;
        ParamLookup.gamPosSamplePeriod = gamPosSampleFreq;
        ParamLookup.repStatSamplePeriod = repStatFreq;
        
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
    if(compGridDataTimer >= compGridDataFreq && CompressionEnabled){
        compGridDataTimer = 0;
        CompressGridData();
    }
    

    
}
public bool CompressionEnabled;
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
        
        
            
            string[] rowDataTemp = new string[14];
            
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
                rowDataTemp[13] = IndividualStats.GetMeanGeneration().ToString();
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
        #elif UNITY_SERVER
        return Application.dataPath+"/";
        #else
        return Application.dataPath +"/";
        #endif
    }

    static string staticGetPath(){
        #if UNITY_EDITOR
        return Application.dataPath +"/CSV/";
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


public static void WriteParams(){
string path = StatisticsWriter.staticGetPath()+"params.txt";
        if(File.Exists(path)){
            File.Delete(path);
        }
        
            
            using (StreamWriter sw = File.CreateText(path))
            {

                sw.WriteLine("Parameter"+","+"Value");
                sw.WriteLine("Grid width: " +","+ParamLookup.gridDims[0]+ "\n");
                sw.WriteLine("Grid height: " +","+ParamLookup.gridDims[1]+ "\n");
                sw.WriteLine("Mean global nutrient concentration: " +","+ ParamLookup.initConc + "\n");
                sw.WriteLine("Total nutrients: "+","+ParamLookup.totalNutrients + "\n");
                sw.WriteLine("Initial nutrient distribution: "+","+ParamLookup.initDistribution + "\n");
                sw.WriteLine("Diffusion period: "+","+ParamLookup.DiffusionPeriod + "\n");
                sw.WriteLine("Diffusion limit: "+","+ParamLookup.diffusionLimit + "\n");
                sw.WriteLine("Initial population size: "+","+ParamLookup.initPop + "\n");
                sw.WriteLine("Grid sample period: "+","+ParamLookup.gridSamplePeriod + "\n");
                sw.WriteLine("PopStat sample period: "+","+ParamLookup.statSamplePeriod + "\n");
                sw.WriteLine("Autotroph position sample period: "+","+ParamLookup.autoPosSamplePeriod + "\n");
                sw.WriteLine("Gamete position sample period: "+","+ParamLookup.gamPosSamplePeriod + "\n");
                sw.WriteLine("RepStat sample period: "+","+ParamLookup.repStatSamplePeriod);
                sw.WriteLine("Quit on global extinction: "+","+ParamLookup.QuitOnGlobalExtinction);
                sw.WriteLine("Quit at time: "+","+ParamLookup.QuitAtTime);
                sw.WriteLine("Quit time: "+","+ParamLookup.quitTime);
                sw.WriteLine("Quit at generation: "+","+ParamLookup.QuitAtGeneration);
                sw.WriteLine("Quit generation: "+","+ParamLookup.quitGeneration);
                if(ParamLookup.initDistribution == "Circle"){
                    sw.WriteLine("Circle radius: "+","+ParamLookup.circleRadius);
                }
                
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
                
                "–––––––––––––––––––––––––"
                );
                if(ParamLookup.initDistribution == "Circle"){
                Debug.Log("Circle radius: "+","+ParamLookup.circleRadius);
                }
            }	


}

}





