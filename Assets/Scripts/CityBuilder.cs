using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilder : MonoBehaviour {

    public GameObject plane;
    public GameObject cube;
    public GameObject origin;
    private int cubeCounter;
    private int planeCounter;

    public IEnumerator BuildCodeCity(JsonProject JsonProjectToBuild)
    {
        Debug.Log(JsonProjectToBuild.views[0].href);
        WWW complexityUrl = new WWW(JsonProjectToBuild.views[0].href);
        yield return complexityUrl;

        if (complexityUrl.error != null)
        {
            Debug.Log("ERROR: " + complexityUrl.error);
        }
        else
        {
            Debug.Log("No Error");
            Debug.Log(complexityUrl.text);

            ProjectView projectView = ProcessProjectData(complexityUrl.text);
            /*foreach (ProjectData allProjects in projectView.data)
            {
                CreateGameObjectFromProjectData(allProjects);
            }*/
            CreateGameObjectFromProjectData(projectView.data[0]);
        }
    }

    private ProjectView ProcessProjectData(string jsonString)
    {
        ProjectView parsejson = JsonUtility.FromJson<ProjectView>(jsonString);
        return parsejson;
    }

    private void CreateGameObjectFromProjectData(ProjectData projectData)
    {
        //projectData.setParents(projectData);

        /* * * 
         * Based on the type of the object, the object should instantiate itself with its prefab. -- atm they should print their type and their id
         */

        switch (projectData.type)
        {
            case "block": GameObject cubes = Instantiate(cube); cubes.transform.SetParent(origin.transform,true); 
               /* cubes.transform.localScale = new Vector3((float)0.8, (float)0.8, (float)0.8);*/ Debug.Log("cube" + projectData.id); break;

            case "scope": //GameObject planes = new GameObject() ; planes.transform.SetParent(origin.transform); planes = Instantiate(plane);
                GameObject planes = Instantiate(plane); planes.transform.SetParent(origin.transform,true); Debug.Log(getSubtreeWidth(projectData));
                /*planes.transform.localScale = new Vector3((float)0.8, (float)0.8, (float)0.8);*/ Debug.Log("plane" + projectData.id); break;

            default: break;
        }
        
        /* * *
         * If an object has one or more children, then the children should execute this method on their own.
         */

        if (projectData.children != null)
        {
            foreach (ProjectData data in projectData.children)
            {
                Debug.Log(data.id);
            }
            foreach (ProjectData data in projectData.children)
            {
                CreateGameObjectFromProjectData(data);
            }
        }
    }

    private void calculatePosition(GameObject gameObject)
    {

    }

    private double getSubtreeWidth(ProjectData projectData)
    {
        double totalWidth = 0;

        foreach(ProjectData subData in projectData.children)
        {
            totalWidth += subData.width;

            if (subData.children != null)
            {
                totalWidth += getSubtreeWidth(subData);
            }
        }
        return totalWidth;
    }


}
