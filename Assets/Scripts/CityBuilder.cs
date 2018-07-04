using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilder : MonoBehaviour {

    public GameObject plane;
    public GameObject cube;
    public CityObject origin;


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
            origin = CreateGameObjectFromProjectData(projectView.data[0], origin);
        }
    }

    private ProjectView ProcessProjectData(string jsonString)
    {
        ProjectView parsejson = JsonUtility.FromJson<ProjectView>(jsonString);
        return parsejson;
    }

    private CityObject CreateGameObjectFromProjectData(ProjectData projectData, CityObject cityObjectParent)
    {
        //projectData.setParents(projectData);

        /* * * 
         * Based on the type of the object, the object should instantiate itself with its prefab. -- atm they should print their type and their id
         */
        CityObject rootCityObject;
        switch (projectData.type)
        {
            case "block": rootCityObject = new CityObject(cube, cityObjectParent);
                /* cubes.transform.localScale = new Vector3((float)0.8, (float)0.8, (float)0.8);*/
                Debug.Log("cube" + projectData.id); break;

            case "scope":   rootCityObject = new CityObject(plane, cityObjectParent);
                Debug.Log(getSubtreeWidth(projectData));
                /*planes.transform.localScale = new Vector3((float)0.8, (float)0.8, (float)0.8);*/
                Debug.Log("plane" + projectData.id); break;

            default: rootCityObject = null; break;
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
                CityObject newChild = CreateGameObjectFromProjectData(data, rootCityObject);
                rootCityObject.appendChildren(newChild);
            }
        }
        return rootCityObject;
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

    /* * *
     * This Method walks trough the whole tree and deletes the objects from the bottom to the top
     */

    public void destroyBuiltCity(CityObject parent)
    {
       foreach(CityObject child in parent.children)
        {
            destroyBuiltCity(child);
            
        }
            parent.destroyCityObject();

    }


}
