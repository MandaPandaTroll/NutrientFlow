using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameteMain : MonoBehaviour
{

    public struct gamete{
    public enum gameteType{A, B, C}
    public bool motile;
    public bool perceptive;
    
    }
    public static List<GameteMain> gameteScripts = new List<GameteMain>();
    
                                                        //   A  ,  B ,  C
    public bool[,] compatibilityTable = new bool[3,3]{ /*A*/{true,true,true},
                                                       /*B*/{true,true,true},
                                                       /*C*/{true,true,true}
                                                        };
    
    public bool autoTrophSpawned;
    public bool chemotaxisEnabled;
    public float perceptionRadius;
    
    public bool incompatibilityEnabled;
    public float interactionRadius;
    public int actionFrequency;
   public int actionTimer;
   public int nutrientLevel;
   public int numSteps;
   public int maximumLifeSpan;
   public int generation;
   public GameObject autotroph_prefab;
    public  IntGrid nutrientgrid;

   Collider2D[] otherGametes;
   LayerMask gameteMask;
    // Start is called before the first frame update
    void Awake(){
        if(gameteScripts.Contains(this) == false){
            gameteScripts.Add(this);
        }
    }
    void Start()
    {
         nutrientgrid = GameObject.Find("NutrientGrid").GetComponent<DiscreteGrid>().nutrientGrid;
        autoTrophSpawned = false;
        gameteMask = LayerMask.GetMask("Gametes");
    }
    
    // Update is called once per frame
    int sumNutrients;
    int cellValue;
    void FixedUpdate()
    {
        numSteps += 1;
        actionTimer += 1;
        if(numSteps >= maximumLifeSpan){
            cellValue = nutrientgrid.GetValue(transform.position);
            nutrientgrid.SetValue(transform.position, cellValue+nutrientLevel);
            nutrientLevel = 0;
            gameteScripts.Remove(this);
            Destroy(gameObject, 0.5f);
        }

        

        //Actions are taken each time the actionTimer reaches a pre-defined value
        if(actionTimer >= actionFrequency){
            actionTimer = 0;
            if(chemotaxisEnabled == true){
                otherGametes = Physics2D.OverlapCircleAll(transform.position,perceptionRadius, gameteMask);
            }else if(chemotaxisEnabled == false){
                otherGametes = Physics2D.OverlapCircleAll(transform.position,interactionRadius, gameteMask);
            }
            //problematic
            if(incompatibilityEnabled == false){
                GameteMain otherGamete_script;
                
                if(otherGametes.Length > 1 && otherGametes[1] != this.GetComponent<Collider2D>()){
                    otherGamete_script = otherGametes[1].gameObject.GetComponent<GameteMain>();
                    if(otherGamete_script.autoTrophSpawned == false){
                        autoTrophSpawned = true;
                        Vector2 zygotePosition = (transform.position + otherGamete_script.transform.position)/2f;
                            sumNutrients = nutrientLevel*2;
                            nutrientLevel = 0;
                            otherGamete_script.nutrientLevel = 0;
                            GameObject thisAutotroph = Instantiate (autotroph_prefab, zygotePosition, transform.rotation);
                            thisAutotroph.GetComponent<Autotroph_main>().nutrientLevel = sumNutrients;
                            thisAutotroph.GetComponent<Autotroph_main>().generation = generation;
                            
                    }
                            gameteScripts.Remove(this);
                            gameteScripts.Remove(otherGamete_script);
                            Destroy(otherGamete_script.gameObject,0.5f);
                            Destroy(gameObject, 0.5f);
                }
                        
                            
                        
                
            }

            


            
        }
    }

    void OnDestroy(){
        
             
            
    }
}


public static class GameteStats{
    public static int GetNGamete(){
        int popSize = GameteMain.gameteScripts.Count;
        return popSize;
    }
    public static int GetSumNutrients(){
        int output = 0;
        foreach(GameteMain gameteScript in GameteMain.gameteScripts){
            output += gameteScript.nutrientLevel;
        }
        return output;
    }

}