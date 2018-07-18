using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilder : MonoBehaviour {

    public GameObject plane;
    public GameObject cube;
    public CityObject origin;
    private bool newPlane = true;

    private int maxW = 0;


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

    private GameObject FindPrefab(string type)
    {
        switch (type.ToLower())
        {
            case "block": newPlane = false; return cube;
            case "scope": newPlane = true; return plane;
            default: return null;
        }
    }
    
    private CityObject CreateGameObjectFromProjectData(ProjectData projectData, CityObject cityObjectParent)
    {
        //projectData.setParents(projectData);

        /* * * 
         * Based on the type of the object, the object should instantiate itself with its prefab. -- atm they should print their type and their id
         */

        Debug.Log(projectData.type + projectData.id);
        var prefab = FindPrefab(projectData.type);
        if (!prefab)
            return null;


        CityObject rootCityObject = new CityObject(prefab, cityObjectParent, getSubtreeWidth(projectData));

        Debug.Log(rootCityObject.width);
        if(newPlane)
        {
            maxW = rootCityObject.width;
        }
        //rootCityObject.scalethis();
        scaleObject(rootCityObject);
        
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
                rootCityObject.appendChild(newChild);
            }
        }
        return rootCityObject;
    }

    private void calculatePosition(CityObject root)
    {
        int maxWidth = root.width;

        foreach(CityObject child in root.children)
        {
            Debug.Log("Root: "+ root.width);
            float scaleRatio = (child.width / maxWidth);
            Debug.Log("Scale Ratio: " + scaleRatio);
            child.gameObject.transform.localScale.Set(scaleRatio, scaleRatio, scaleRatio);
            if (child.children != null)
            {
                calculatePosition(child);
            }
        }
    }

    private void scaleObject(CityObject node)
    {
        //float scaleRatio = (float)(node.width/maxW);
        Debug.Log(node.form.name);
        //node.gameObject.transform.localScale.Set(scaleRatio, scaleRatio, scaleRatio);
    }

    private int getSubtreeWidth(ProjectData projectData)
    {
        int totalWidth = 0;

        totalWidth += projectData.width;
        if (projectData.children != null)
        {
            foreach (ProjectData subData in projectData.children)
            {
                totalWidth += subData.width;

                if (subData.children != null)
                {
                    totalWidth += getSubtreeWidth(subData);
                }
            }
        }
        return totalWidth;
    }

    /* * *
     * This Method walks trough the whole tree and deletes the objects from the bottom to the top
     */

    public void destroyBuiltCity(CityObject node)
    {
       foreach(CityObject child in node.children)
        {
            destroyBuiltCity(child);
            
        }
            node.destroyCityObject();

    }


}
