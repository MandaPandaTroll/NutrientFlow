using System;


public  class ExtraMath{
static Random rand = new Random(); //reuse this if you are generating many
public static System.Random RandGen = new System.Random();
public static float GetNormal(double mean, double stdDev){
    //By yoyoyoyosef, stackoverflow  Jan 2017
Random rand = new Random(); //reuse this if you are generating many
  double u1 = 1.0-rand.NextDouble(); //uniform(0,1] random doubles
  double u2 = 1.0-rand.NextDouble();
 double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
             Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
  double randNormal =
             mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
    
     float output = (float) randNormal;
    return output;

}

public static double GetNormalDouble(double mean, double stdDev){
    //By yoyoyoyosef, stackoverflow  Jan 2017

  double u1 = 1.0-rand.NextDouble(); //uniform(0,1] random doubles
  double u2 = 1.0-rand.NextDouble();
 double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
             Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
  double randNormal =
             mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
    
     double output =  randNormal;
    return output;

}

public static double[] GetMeanAndStDev(int val1, int val2){
    float temp_val1 = (float)val1;
    float temp_val2 = (float)val2;
    float mean = (temp_val1+temp_val2)/2f;
    float variance = (MathF.Pow((mean-temp_val1),2f)+MathF.Pow((mean-temp_val2),2f))/2f;
    float sd = MathF.Sqrt(variance);
    double[] output = new double[2]{(double)mean,(double)sd};
    return output;

}

public static float GetGenerationStDev( int[] generationVals){
    float sum = 0;
    float mean;
    float variance = 0;
    float sd;
    int n = generationVals.Length;
    for(int i = 0; i < n; i++){
        sum += (float)generationVals[i];
    }
    mean = sum/(float)n;
    for(int i = 0; i < n; i++){
        variance += MathF.Pow((float)generationVals[i]-mean,2f);
    }
    variance = variance/n;
    sd = MathF.Sqrt(variance);
    return sd;

}





 
    
}