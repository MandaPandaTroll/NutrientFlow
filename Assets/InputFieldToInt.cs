using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
public class InputFieldToInt : MonoBehaviour
{

    public InputField inputField;
    public static string fieldText{get;set;}

    public int value{get;set;}
    public int defaultValue = 0;
    // Start is called before the first frame update
    void Start()
    {
        value = defaultValue;
        inputField.text = defaultValue.ToString();

        if(fieldText != null){
            value = Int16.Parse(fieldText);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        fieldText = inputField.text;
        if(fieldText != null){
            value = Int16.Parse(fieldText);
        }
    }
}
