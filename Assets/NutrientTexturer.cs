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
    
    NutrientGridHandler gridContainer;
    public RawImage img;
    FloatGrid m_nutgrid;
    public CustomRenderTexture rendTex;
   int [] dims = new int[2];
   
   
    // Start is called before the first frame update
     void Start()
     {  
      sensitivity = initSensitivity;
      gradient = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[2];
        colorKey[0].color = Color.black;
        colorKey[0].time = 0.0f;
        colorKey[1].color = new Color(1.0f,1.0f,1.0f,1.0f);
        colorKey[1].time = 1.0f;

        

        alphaKey = new GradientAlphaKey[2];

        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;
        
        

        gradient.SetKeys(colorKey,alphaKey);

      gridContainer = GameObject.Find("NutrientGrid").GetComponent<NutrientGridHandler>();
      
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
                 texture =  new Texture2D(dims[0], dims[1], TextureFormat.RGB24, false);
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
    void Update()
    {   
      timePassed += Time.deltaTime;
        float maxVal = 0;
       float tempval = 0;
       if(timePassed >= textureRefreshRate){
        
        
          for(int i = 0;i < dims[0];i++){
          for(int j = 0;j < dims[1];j++){
            tempval = m_nutgrid.GetValue(i,j);
            if(tempval > maxVal){
              maxVal = tempval;
            }
          }

        }
        
        if( maxVal > 0){
          for(int i = 0;i < dims[0];i++){
            for(int j = 0;j < dims[1];j++){
              tempval = m_nutgrid.GetValue(i,j);
              scaledVal =  tempval/maxVal;
             // if(tempval > 0){
                //scaledVal = 1.0f;
             // }else{scaledVal = 0f;}
              tempCol = gradient.Evaluate(tempval);
              texture.SetPixel(i,j,tempCol);
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
