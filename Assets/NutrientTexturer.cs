using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NutrientTexturer : MonoBehaviour
{
  public float initSensitivity;
public float sensitivity{get; set;}
//public float sensitivity;
Gradient gradient;
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;
    
    DiscreteGrid gridContainer;
    public RawImage img;
    IntGrid m_nutgrid;
    public CustomRenderTexture rendTex;
   int [] dims = new int[2];
   
   Color homoGen = new Color(0.1f,0.1f,0.1f,1.0f);
   
    // Start is called before the first frame update
     void Start()
     {  
      sensitivity = initSensitivity;
      gradient = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[2];
        colorKey[0].color = Color.black;
        colorKey[0].time = 0.0f;
        colorKey[1].color = Color.white;//new Color(0.4f,0.4f,0.4f,1.0f);
        colorKey[1].time = 1.0f;

        

        alphaKey = new GradientAlphaKey[2];

        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;
        
        

        gradient.SetKeys(colorKey,alphaKey);

      gridContainer = GameObject.Find("NutrientGrid").GetComponent<DiscreteGrid>();
      
      m_nutgrid = gridContainer.nutrientGrid;
      dims[0] = gridContainer.gridWidth;
      dims[1] = gridContainer.gridHeight;
      float widthQuot = (float)Screen.width/dims[0];
       float heightQout = (float)Screen.height/dims[1];

      float newDim = newDim = 1024f/dims[0];
       
        
       
      wtoh = (float)(dims[0])/(float)(dims[1]);
        img.GetComponent<RectTransform>().sizeDelta = new Vector2( dims[0]*gridContainer.cellSize, dims[1]*gridContainer.cellSize);
         // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
         //for(int i = 0; i <63; i ++){
            //previousX[i] = 0;
            //previousY[i] = 0;
                 texture =  new Texture2D(dims[0], dims[1], TextureFormat.RGBA32, false);
                 //rendTex.width = dims[0];
                 //rendTex.height = dims[1];
                  for(int i= 0; i < dims[0]; i++){
                    for(int j= 0; j < dims[1]; j++){
                    texture.SetPixel(i, j, new Color(0f,0f,0f,0.5f));
                    }
                }
         
         
     }
   




   Texture2D texture; 
   
  public float textureRefreshRate = 1f;
  float timePassed = 0;
  float scaledVal = 0;
    float wtoh;
    // Update is called once per frame

  Color tempCol;
    void FixedUpdate()
    {   
      timePassed += Time.deltaTime;
        float maxVal = 0;
       float tempval = 0;
       float minVal = 256;
       if(timePassed >= textureRefreshRate){
        
        
          for(int i = 0;i < dims[0];i++){
          for(int j = 0;j < dims[1];j++){
            tempval = (float)m_nutgrid.GetValue(i,j);
            if(tempval > maxVal){
              maxVal = tempval;
            }
            if(tempval < minVal){
              minVal = tempval;
            }
          }

        }
       //maxVal = maxVal*10f;
        if( maxVal > 0){
          for(int i = 0;i < dims[0];i++){
            for(int j = 0;j < dims[1];j++){

              tempval = (float)m_nutgrid.GetValue(i,j);
              if (tempval > 0 && tempval > minVal){
                  if(tempval > minVal){
                    scaledVal =  Mathf.Clamp01((1f/sensitivity)*(Mathf.Log10((float)tempval/(float)maxVal))+1f);
                     //scaledVal = Mathf.Pow(scaledVal,4f);
                    // if(tempval > 0){
                      //scaledVal = 1.0f;
                      // }else{scaledVal = 0f;}
                     tempCol = gradient.Evaluate(scaledVal);
                    texture.SetPixel(i,j,tempCol);
                  }else if(tempval == minVal && minVal > 0){
                    texture.SetPixel(i,j,homoGen);
                  }
                  
              }else if (tempval == 0){texture.SetPixel(i,j,Color.black);}
              
              
            }
          }
        }
        
        
         
         // set the pixel values
        
         
         // Apply all SetPixel calls
         texture.Apply();
         
         // connect texture to material of GameObject t$$anonymous$$s script is attached to
         gameObject.GetComponent<RawImage>().texture = texture;
         timePassed = 0;
       }
        
        
    }
    
}
