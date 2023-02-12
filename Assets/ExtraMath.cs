using System;


public  class ExtraMath{


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



 
    
}