using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilder : MonoBehaviour {

    public GameObject plane;
    public GameObject cube;
    public CityObject origin;
    public ValueManager valueManager;


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
            origin.width = Subtree(origin);
            //Debug.Log(origin.form.transform.localScale.x);
            //log(origin);        
            scaleObject(origin);
            positionObjects(origin);
            setMaterials(origin);
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
            case "block": return cube;
            case "scope": return plane;
            default: return null;
        }
    }
    
    private CityObject CreateGameObjectFromProjectData(ProjectData projectData, CityObject cityObjectParent)
    {
        //projectData.setParents(projectData);

        /* * * 
         * Based on the type of the object, the object should instantiate itself with its prefab. -- atm they should print their type and their id
         */

        //Debug.Log(projectData.type + projectData.id);
        var prefab = FindPrefab(projectData.type);
        if (!prefab)
            return null;


        CityObject rootCityObject = new CityObject(prefab, cityObjectParent, projectData.width, projectData.height, projectData.value);

        scaleObject(rootCityObject);
        
        /* * *
         * If an object has one or more children, then the children should execute this method on their own.
         */

        if (projectData.children != null)
        {
            foreach (ProjectData data in projectData.children)
            {
                //Debug.Log(data.id);
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
        
    }

   
    private void logWidths (CityObject cityObject)
    {
        if (cityObject.children == null)
        {
            Debug.Log(cityObject.width);
        }
        else
        {
            foreach (CityObject child in cityObject.children)
            {
                logWidths(child);  
            }
            Debug.Log(cityObject.width);
        }
    }

    /*private int oldgetSubtreeWidth(ProjectData projectData)
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
                    totalWidth += oldgetSubtreeWidth(subData);
                }
            }
        }
        return totalWidth;
    }*/

    /* * *
     * This Method adjusts the widths of the planes because they didn´t get a width of the ProjectData before.
     * So the Planes add up all widths of their children (cubes) for theirown.
     */
    private int Subtree(CityObject cityObject)
    {
        if (cityObject.children == null)
        {
            return cityObject.width;
        }
        else
        {
            foreach(CityObject child in cityObject.children)
            {
                cityObject.width += Subtree(child);
                Debug.Log(cityObject.width);
            }
            return cityObject.width;
        }
    }


    /*private void scaleObject(CityObject node)
    {
        int maxWidth = node.width;
        if (node.form.transform.localScale.x == 2) {
            Debug.Log(node.form.name);
            //node.gameObject.transform.localScale.Set(scaleRatio, scaleRatio, scaleRatio);
            foreach (CityObject child in node.children)
            {
                //Debug.Log(maxWidth);
                //Debug.Log(child.width);
                double Ratio = (double) child.width / maxWidth;
                float scaleRatio = (float)Ratio;
                Debug.Log(scaleRatio);
                float height = 1f;
                if (!IsScope(child))
                {
                   height = (float)(2 / node.form.transform.localScale.y) * scaleRatio;
                }
                child.form.transform.localScale = new Vector3(scaleRatio, height, scaleRatio);
                if (child.children != null)
                {
                    scaleObject(child);
                }
            }
        }
        else
        {
            foreach (CityObject child in node.children)
            {
                double Ratio = (double)((child.width / maxWidth) * node.form.transform.localScale.x);
                float scaleRatio = (float)Ratio;
                float height = 1f;
                if (!IsScope(child))
                {
                    height = (float)(2 / node.form.transform.localScale.y) * scaleRatio;
                }
                child.form.transform.localScale = new Vector3(scaleRatio, height, scaleRatio);
                if (child.children != null)
                {
                    scaleObject(child);
                }
            }
        }
    }*/

    private void scaleObject(CityObject node)
    {
        int maxWidth = node.width;
        foreach (CityObject child in node.children)
        {
            //Debug.Log(maxWidth);
            //Debug.Log(child.width);
            /*if (node.form.transform.localScale.x == 2) -- old
            {
                Ratio = (double)child.width / maxWidth;
                scaleRatio = (float)Ratio;
            }
            else
            {
                Debug.Log(maxWidth);
                Debug.Log(child.width);
                Ratio = (double) child.width / maxWidth;
                Debug.Log(Ratio);
                //scaleRatio = (float)Ratio * node.form.transform.localScale.x;
                scaleRatio = (float)Ratio;
            }*/
            float scaleRatio = (float)child.width / maxWidth;

            //Debug.Log(scaleRatio); -- fehler mit float zeigen
            /*float height = 1f;
            if (!IsScope(child))
            {
                height = (float)(2 / node.form.transform.localScale.y) * scaleRatio;
            }*/
            float height = scaleHeights(child);
            child.form.transform.localScale = new Vector3(scaleRatio, height, scaleRatio);
            if (child.children != null)
            {
                scaleObject(child);
            }
        }           
    }

    private float scaleHeights(CityObject node)
    {
        float height;
        //Debug.Log(node.height);
        
        if(IsScope(node)){
            height = 1f;
        }
        else
        {
            Debug.Log(node.height);
            height = (float)40 * node.height / 100; //40 = 1 / 0.025 (= height of the first scope)
        }
        Debug.Log(height);
        return height;
    }

    private void positionObjects(CityObject node)
    {
        float usedSpace = 0;
        float middle = node.form.transform.localScale.x / 2;
        foreach(CityObject child in node.children)
        {
            float xPos = 1 - child.form.transform.localScale.x;
            //Debug.Log(xPos);
            float yPos = 1.025f + (0.5f * child.height/100) + (node.form.transform.localScale.y); //0.025 is the top of the table
            float zPos = 1 - (usedSpace * 2) - child.form.transform.localScale.z;
            usedSpace += child.form.transform.localScale.z;
            child.form.transform.position = new Vector3(xPos, yPos, zPos);
            /*float xPos = node.form.transform.localPosition.x + middle - (child.form.transform.localScale.x * 1);//* node.form.transform.localScale.x);
            //Debug.Log(xPos);
            float yPos = 1 + (0.5f * child.height / 100) + 0.025f; //0.025 is the top of the table
            float zPos = node.form.transform.localPosition.z + middle - (usedSpace * 2) - (child.form.transform.localScale.z * 1);//node.form.transform.localScale.z);
            usedSpace += child.form.transform.localScale.z;
            child.form.transform.position = new Vector3(xPos, yPos, zPos);*/
            if (IsScope(child))
            {
                child.form.transform.position += new Vector3(0,0.0125f,0);
                positionChildren(child);
            }
        }
    }

    private void positionChildren(CityObject node)
    {
        float usedSpace = 0;
        float middle = node.form.transform.localScale.x / 2;
        foreach (CityObject child in node.children)
        {
            float xPos = 1 - (child.form.transform.localScale.x * node.form.transform.localScale.x); //passt
            //Debug.Log(xPos);
            float yPos = 1.05f + (0.5f * child.height / 100) + node.form.transform.lossyScale.y ; // 1.0375 is the yPos of the first Plane + 0.0125 for the plane
            //0.025 is the top of the table + 0.0125 for the first plane
            //float zPos = 1 - node.form.transform.localScale.x - (usedSpace * 2) - child.form.transform.localScale.z;
            float zPos = (node.form.transform.position.z + node.form.transform.localScale.z) -
                ((child.form.transform.localScale.z + usedSpace) * node.form.transform.localScale.z);
            usedSpace += (child.form.transform.localScale.z * 2);
            child.form.transform.position = new Vector3(xPos, yPos, zPos);
            if (IsScope(child))
            {
                positionChildren(child);
            }
        }
    }

    private void setMaterials(CityObject root)
    {
        foreach(CityObject child in root.children)
        {
            valueManager.setValue(child);
            if (IsScope(child)){
                setMaterials(child);
            }
            
        }
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

    private bool IsScope(CityObject cityObject)
    {
        if (cityObject.children.Count != 0) return true;
        else return false;
    }


}

