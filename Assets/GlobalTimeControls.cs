using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

 using UnityEngine.SceneManagement;



public class GlobalTimeControls : MonoBehaviour
{
    public Button startButton;
    public Button resetButton;
    float ogTimeScale;
    public  bool begin{get;set;}
    public  bool reset{get;set;}
    // Start is called before the first frame update
    void Awake()
    {
        ogTimeScale = Time.timeScale;
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(begin == true && startButton.enabled == true){
            Time.timeScale = ogTimeScale;
            startButton.gameObject.SetActive(false);
            resetButton.gameObject.SetActive(true);
        }
        if(resetButton.enabled == true && reset == true){
            reset = false;
            resetButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(true);
             SceneManager.LoadScene("SampleScene");
        }
    }
}
