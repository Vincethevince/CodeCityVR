using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class ButtonCreator : MonoBehaviour {

    public GameObject menu;                             //The menu to create the Button on
    public Button buttonPrefab;                               //The Button prefab to instantiate
    public CityBuilder cityBuilder;                     //Script to build the city on the table


    /* * * CreateButtons-Function
     * Create a Button from a Json Project - setting the Text of the Button to the name of the project.
     * Fix the transform of that new Button.
     * Add a function to build up the CodeCity out of that project when the Button is pressed.
     * */ 


    public void createButtons(JsonProject projectToCreateButtonOf)
    {
        Button mybutton = Instantiate(buttonPrefab);
        mybutton.transform.SetParent(menu.transform);
        mybutton.GetComponentInChildren<Text>().text = projectToCreateButtonOf.name;
        mybutton.transform.localScale = new Vector3((float)0.5, (float)0.5, (float)0.5);
        mybutton.transform.localPosition = new Vector3(-5, 16, (float)-0.01);
        mybutton.transform.localRotation = new Quaternion(0,0,0,0);
        mybutton.interactable = true;
        mybutton.onClick.AddListener(delegate { StartCoroutine(cityBuilder.BuildCodeCity(projectToCreateButtonOf));
                                                mybutton.interactable = false;}) ; 
    }


 
}
