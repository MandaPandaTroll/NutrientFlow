using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameteMain : MonoBehaviour
{
    public int[] coordNute = new int[3];
    public static float cellSize{get;set;}
    public int parentLifeSpan;
    public float maxTurnAngle;
    public int zygoteLifeSpan;
    public float movementSpeed;
    public  Vector2 mapBounds;
    public static Vector3 originPosition{get;set;}
 
    public double meanMaximumLifeSpan;
    public double std_lifeSpan;
    
    public struct gamete{
    public enum gameteType{A, B, C}
    
    public bool perceptive;
    
    }

    public int parentNumber;
    public static int gameteCount;
    public int gameteNumber;
    public bool motile;
    public static int zygoteNutrients;
    public static List<GameteMain> gameteScripts = new List<GameteMain>();
    
                                                        //   A  ,  B ,  C
    public bool[,] compatibilityTable = new bool[3,3]{ /*A*/{true,true,true},
                                                       /*B*/{true,true,true},
                                                       /*C*/{true,true,true}
                                                        };
    
    public bool autoTrophSpawned;
    public bool chemotaxisEnabled;
    public float perceptionRadius;
    public static int staticNutrientLevel{get;set;}
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
    bool isActing;
    Rigidbody2D rb;
    Collider2D thisCollider;

   Collider2D[] otherGametes;
   LayerMask gameteMask;
   int masterTimer;
   public int masterFrequency;
    // Start is called before the first frame update
    void Awake(){
        masterTimer = 0;
        coordNute = new int[3]{-1,-1,0};
        coordNute = GameteStats.GetCoordNute(this);
        if(gameteScripts.Contains(this) == false){
            gameteScripts.Add(this);
        }
        
        
        interactingGameteIDs = new int[2];
        
        actionTimer = Random.Range(0,actionFrequency+1);
        isActing = false;
        autoTrophSpawned = false;
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        thisCollider = this.gameObject.GetComponent<Collider2D>();
        gameteCount += 1;
        gameteNumber = gameteCount;
        interactingGameteIDs[0] = gameteNumber;
        maximumLifeSpan = Mathf.FloorToInt (ExtraMath.GetNormal(meanMaximumLifeSpan, std_lifeSpan));
    }
    Vector2 tempVector = new Vector2(0,0);
    void Start()
    {
        //GameteStats.GetCoordNute(this);
        /*rb.MovePosition(new Vector2(Mathf.Round(rb.position.x),Mathf.Round(rb.position.y)));
        tempVector = rb.position;
        if(rb.position.x > mapBounds.x){
            tempVector.x = mapBounds.x;
        }else if(rb.position.x < -mapBounds.x){
            tempVector.x = -mapBounds.x;
        }
        if(rb.position.y > mapBounds.y){
            tempVector.y = mapBounds.y;
        }else if(rb.position.y < -mapBounds.y){
            tempVector.y = -mapBounds.y;
        }*/
        //rb.MovePosition(tempVector);
        interactionRadius = cellSize/8f;
        interactingGameteIDs = new int[2];
         nutrientgrid = GameObject.Find("NutrientGrid").GetComponent<DiscreteGrid>().nutrientGrid;
        
        gameteMask = LayerMask.GetMask("Gametes");
        staticNutrientLevel = Autotroph_main.staticGameteCost;
        nutrientLevel = staticNutrientLevel;
        interactingGameteIDs[0] = gameteNumber;
        //rb.MoveRotation(Random.Range(-180f,180f));
        rb.MoveRotation(0);
        mapBounds =boxDims.mapBounds;

        if(rb.position.x >= mapBounds.x){
            
            rb.MovePosition(new Vector2(mapBounds.x,rb.position.y));
        }else if(rb.position.x <= -mapBounds.x){
            
            rb.MovePosition(new Vector2(-mapBounds.x,rb.position.y));
        }
        if(rb.position.y >= mapBounds.y){
            
            rb.MovePosition(new Vector2(rb.position.x,mapBounds.y));
        }else if(rb.position.x <= -mapBounds.y){
            
            rb.MovePosition(new Vector2(-rb.position.x,mapBounds.y));
        }
    }
    
    // Update is called once per frame
    int sumNutrients;
    int cellValue;

    Vector2 pos = new Vector2(0,0);
    public int[] interactingGameteIDs;
    List <GameteMain> gameteScriptList = new List<GameteMain>();
    void FixedUpdate()
    {
        masterTimer +=1;
        if(masterTimer >= masterFrequency){
            masterTimer = 0;
            coordNute[2] = nutrientLevel;
        if(rb.position.x >= mapBounds.x){
            pos = new Vector2(mapBounds.x-(cellSize/2f),rb.position.y);
            rb.MovePosition(pos);
        }else if(rb.position.x <= -mapBounds.x){
            pos = new Vector2(-mapBounds.x+(cellSize/2f),rb.position.y);
            rb.MovePosition(pos);
        }
        if(rb.position.y >= mapBounds.y){
            pos = new Vector2(rb.position.x,mapBounds.y-(cellSize/2f));
            rb.MovePosition(pos);
        }else if(rb.position.y <= -mapBounds.y){
            pos = new Vector2(-rb.position.x,-mapBounds.y+(cellSize/2f));
            rb.MovePosition(pos);
        }
        coordNute = GameteStats.GetCoordNute(this);
        numSteps += 1;
        actionTimer += 1;
        if(numSteps >= maximumLifeSpan && isActing == false ){
            cellValue = nutrientgrid.GetValue(transform.position);
            nutrientgrid.SetValue(transform.position, cellValue+nutrientLevel);
            nutrientLevel = 0;
            StatisticsWriter.deaths_gamete +=1;
            gameteScripts.Remove(this);
            Destroy(gameObject, 0.5f);
        }if(autoTrophSpawned == true){
            nutrientLevel = 0;
            gameteScripts.Remove(this);
            Destroy(gameObject);
        }
        if(motile == true){
            
                if(Mathf.Abs(rb.position.x) < mapBounds.x-Autotroph_main.margin && Mathf.Abs(rb.position.y) < mapBounds.y-Autotroph_main.margin){
                    rb.MovePosition(transform.position + transform.up*movementSpeed );
                    rb.MoveRotation(rb.rotation + Random.Range(-maxTurnAngle,maxTurnAngle));
                }else{
                    if(rb.position.x <= -mapBounds.x+Autotroph_main.margin){
                    Vector2 newPos = rb.position;
                    newPos = new Vector2(-mapBounds.x+Autotroph_main.margin,rb.position.y);
                    rb.MoveRotation(Random.Range(-180f,180f));
                    rb.MovePosition(newPos);
                }
                else if(rb.position.x >= mapBounds.x-Autotroph_main.margin){
                    Vector2 newPos = rb.position;
                    newPos = new Vector2(mapBounds.x-Autotroph_main.margin,rb.position.y);
                    rb.MoveRotation(Random.Range(-180f,180f));
                    rb.MovePosition(newPos);
                }
                if(rb.position.y <= -mapBounds.y+Autotroph_main.margin){
                    Vector2 newPos = rb.position;
                    newPos = new Vector2(rb.position.x,-mapBounds.y+Autotroph_main.margin);
                    rb.MoveRotation(Random.Range(-180f,180f));
                    rb.MovePosition(newPos);
                }
                else if(rb.position.y >= mapBounds.y-Autotroph_main.margin){
                    Vector2 newPos = rb.position;
                    newPos = new Vector2(rb.position.x,mapBounds.y-Autotroph_main.margin);
                    rb.MoveRotation(Random.Range(-180f,180f));
                    rb.MovePosition(newPos);
                }
                rb.MovePosition(transform.position + transform.up*movementSpeed );
                }
                
                
            }

        

        //Actions are taken each time the actionTimer reaches a pre-defined value
        if(actionTimer >= actionFrequency && numSteps < maximumLifeSpan){
            
            
            isActing = true;
            
            if(chemotaxisEnabled == true){
                otherGametes = Physics2D.OverlapCircleAll(transform.position,perceptionRadius, gameteMask);
            }else if(chemotaxisEnabled == false){
                otherGametes = Physics2D.OverlapCircleAll(transform.position,interactionRadius, gameteMask);
            }
            
            
            
                GameteMain otherGamete_script;
                
            if(otherGametes.Length > 1){
                gameteScriptList.Clear();
                for(int i = 0; i < otherGametes.Length-1; i++){
                    if(otherGametes[i] != thisCollider){
                        gameteScriptList.Add(otherGametes[i].gameObject.GetComponent<GameteMain>());
                    }
                }
                for(int i = 0; i < gameteScriptList.Count; i++){
                    if(gameteScriptList[i].interactingGameteIDs[0] != gameteNumber && numSteps < maximumLifeSpan && gameteScriptList[i].numSteps < gameteScriptList[i].maximumLifeSpan){
                        
                        if(incompatibilityEnabled == false){
                        interactingGameteIDs[1] = gameteScriptList[i].gameteNumber;
                        otherGamete_script = gameteScriptList[i];
                        if(otherGamete_script.autoTrophSpawned == false && autoTrophSpawned == false){
                                    CreateZygote(otherGamete_script);
                                }
                        
                        break;
                        }else if( incompatibilityEnabled == true){
                            if(gameteScriptList[i].parentNumber != this.parentNumber){
                                interactingGameteIDs[1] = gameteScriptList[i].gameteNumber;
                                otherGamete_script = gameteScriptList[i];
                                if(otherGamete_script.autoTrophSpawned == false && autoTrophSpawned == false){
                                    CreateZygote(otherGamete_script);
                                    Destroy(otherGamete_script.gameObject, 0.5f);
                                    Destroy(gameObject,0.5f);

                                }
                                
                                break;
                            }
                        }
                        
                    }
                }
                


            }
/*
                if(otherGametes.Length > 0 && otherGametes[0] != this.GetComponent<Collider2D>()){
                    interactingGameteIDs[0] = gameObject.GetInstanceID();
                    if(otherGametes.Length <= 2 && otherGametes[1] != this){
                        otherGamete_script = otherGametes[1].gameObject.GetComponent<GameteMain>();
                    }else{
                        
                        for(int i = 1; i < otherGametes.Length-1; i++){
                             tempDistance = Vector2.Distance(otherGametes[i].transform.position,transform.position);
                            if(tempDistance < minDistance && tempDistance > 0){
                                minDistance = tempDistance;
                                minIndex = i;
                            }

                        }
                        otherGamete_script = otherGametes[minIndex].gameObject.GetComponent<GameteMain>();
                    }
                    
                    if(otherGamete_script == this){
                        return;
                    }

                    interactingGameteIDs[1] = otherGamete_script.gameObject.GetInstanceID();
                    if(otherGamete_script.interactingGameteIDs[1] == interactingGameteIDs[0] && interactingGameteIDs[0] != interactingGameteIDs[1]){
                       /* if(otherGamete_script.autoTrophSpawned == false ){
                        autoTrophSpawned = true;
                        Vector2 zygotePosition = (transform.position + otherGamete_script.transform.position)/2f;
                            sumNutrients = staticNutrientLevel*2;
                            nutrientLevel = 0;
                            otherGamete_script.nutrientLevel = 0;
                            
                            GameObject thisAutotroph = Instantiate (autotroph_prefab, zygotePosition, transform.rotation);
                            Autotroph_main thisAutotroph_script = thisAutotroph.GetComponent<Autotroph_main>();
                            thisAutotroph_script.generation = (generation+otherGamete_script.generation)/2;
                            thisAutotroph_script.parentGametes.Add(gameObject.GetInstanceID());
                            thisAutotroph_script.parentGametes.Add(otherGamete_script.gameObject.GetInstanceID());
                            Destroy(otherGamete_script.gameObject);
                            gameteScripts.Remove(otherGamete_script);
                            if(thisAutotroph_script.nutrientLevel != sumNutrients){
                                thisAutotroph_script.nutrientLevel = sumNutrients;
                            }
                            
                            
                            gameteScripts.Remove(this);
                            Destroy(gameObject);
                            
                    }
                    
                    }
                    
                            
                            
                            
                            
                }
                  */      
                            
                        
                
            

            

            actionTimer = 0;
            isActing = false;
        }
        }
        
    }

    void CreateZygote( GameteMain otherGamete_script){
        if(numSteps > maximumLifeSpan){
            return;
        }
        if(autoTrophSpawned){
            gameteScripts.Remove(this);
                nutrientLevel = 0;

                Destroy(gameObject);

        }
            if(otherGamete_script.autoTrophSpawned == true){
                gameteScripts.Remove(this);
                nutrientLevel = 0;

                Destroy(gameObject);
                return;
            }
                        autoTrophSpawned = true;
                        otherGamete_script.autoTrophSpawned = true;
                        Vector2 zygotePosition = (transform.position + otherGamete_script.transform.position)/2f;
                            sumNutrients = staticNutrientLevel;
                            nutrientLevel = 0;
                            otherGamete_script.nutrientLevel = 0;
                            
                            GameObject thisAutotroph = Instantiate (autotroph_prefab, new Vector3(transform.position.x,transform.position.y,transform.position.z), this.transform.rotation);
                            StatisticsWriter.zygotesFormed += 1;
                            Autotroph_main thisAutotroph_script = thisAutotroph.GetComponent<Autotroph_main>();
                            thisAutotroph_script.generation = (generation+otherGamete_script.generation)/2;
                            thisAutotroph_script.parentGametes = new int[2]{gameteNumber,otherGamete_script.gameteNumber};
                            if(Autotroph_main.InheritedLifeSpan == true){
                                /*double[] msd = ExtraMath.GetMeanAndStDev(parentLifeSpan,otherGamete_script.parentLifeSpan);
                                msd[1] = (double)Mathf.Sqrt((float)msd[1]);
                                zygoteLifeSpan = Mathf.FloorToInt(ExtraMath.GetNormal(msd[0],msd[1]));*/

                                zygoteLifeSpan = Random.Range(((parentLifeSpan+otherGamete_script.parentLifeSpan)/2)-1,((parentLifeSpan+otherGamete_script.parentLifeSpan)/2)+2);
                                thisAutotroph_script.maximumLifeSpan = zygoteLifeSpan;
                            }
                            gameteScripts.Remove(otherGamete_script);

                            Destroy(otherGamete_script.gameObject);
                            nutrientgrid.SetValue(transform.position,nutrientgrid.GetValue(transform.position)+sumNutrients);
                            if(thisAutotroph_script.nutrientLevel != sumNutrients){
                                thisAutotroph_script.nutrientLevel = sumNutrients;
                                
                            }
                            
                            
                            gameteScripts.Remove(this);
                            Destroy(gameObject);

    }

    void OnDestroy(){
        
        gameteScripts.Remove(this);
        nutrientLevel = 0;
            
    }

    static bool isNotThisCollider(Collider2D n, Collider2D thisCollider)
{
    return n != thisCollider;
}
}


public static class GameteStats{
    public static int GetNGamete(){
        int popSize = GameteMain.gameteScripts.Count;
        
        return popSize;
    }
    public static int GetSumNutrients(){
        int output = 0;
        GameObject[] gams = GameObject.FindGameObjectsWithTag("smallGamete");
        foreach (GameObject gam in gams){
            output+= gam.GetComponent<GameteMain>().nutrientLevel;
        }
        /*foreach(GameteMain gameteScript in GameteMain.gameteScripts){
            output += gameteScript.nutrientLevel;
        }*/
        return output;
    }

    public static int[] GetCoordNute( GameteMain gam){
            
            int[] output = new int[3];
            output[0] = Mathf.FloorToInt((gam.transform.position - Autotroph_main.originPosition).x / Autotroph_main.cellSize);
            output[1] = Mathf.FloorToInt((gam.transform.position - Autotroph_main.originPosition).y / Autotroph_main.cellSize);
            output[2] = gam.nutrientLevel;
            if(output[0] < 0  || output[1] < 0 || output[0] > ParamLookup.gridDims[0]-1 || output[1] > ParamLookup.gridDims[1]-1){
                output[0] = -1;
                output[1] = -1;
                output[2] = 0;
            }
            return output;
        }

}