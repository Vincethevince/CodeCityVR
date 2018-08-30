using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CityBuilder : MonoBehaviour {

    public GameObject plane;
    public GameObject cube;
    public CityObject origin;
    public ValueManager valueManager;

    private double distance = 0.5;

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
            ProjectView projectView = ProcessProjectData(complexityUrl.text);
            origin = CreateGameObjectFromProjectData(projectView.data[0], origin);
            Algorithm();
        }
    }

    /* * *
     * This function is doing all the steps after creating all the GameObjects. First all widths are adjusted for the scope objects.  
     * Then all Objects are scaled and positioned. In the end, all Objects get a new Material.
     */

    private void Algorithm()
    {
        origin.width = Subtree(origin);        
        ScaleObject(origin);
        SortChildren(origin);
        PositionObjectsRadial(origin);
        SetMaterials(origin);
    }
    
    private ProjectView ProcessProjectData(string jsonString)
    {
        ProjectView parsejson = JsonUtility.FromJson<ProjectView>(jsonString);
        return parsejson;
    }

    /* * *
     * This functions gets the string of the projectData.type attribute and returns the fitting prefab for the new object
     */

    private GameObject FindPrefab(string type)
    {
        switch (type.ToLower())
        {
            case "block": return cube;
            case "scope": return plane;
            default: return null;
        }
    }
    
    private CityObject CreateCityObjectFromProjectData(ProjectData projectData, CityObject cityObjectParent)
    {
        
        var prefab = FindPrefab(projectData.type);
        if (!prefab)
            return null;

        CityObject rootCityObject = new CityObject(prefab, cityObjectParent, projectData.width, projectData.height, projectData.value);

        ScaleObject(rootCityObject);
        
        /* * *
         * If an object has one or more children, then the children should execute this method on their own.
         */

        if (projectData.children.Count() != 0)
        {
            foreach (ProjectData data in projectData.children)
            {
                CityObject newChild = CreateGameObjectFromProjectData(data, rootCityObject);
                rootCityObject.AppendChild(newChild);
            }
        }
        return rootCityObject;
    }
    
    /* * *
     * This Method adjusts the widths of the planes because they didn´t get a width of the ProjectData before.
     * So the Planes add up all widths of their children (cubes) for theirown.
     */
    private double Subtree(CityObject cityObject)
    {

        foreach (CityObject child in cityObject.children)
        {
            cityObject.width += Subtree(child) + 2 * distance;
            Debug.Log(cityObject.width);
        }
        return cityObject.width;

    }

    private void ScaleObject(CityObject node)
    {
        double maxWidth = node.width;
        foreach (CityObject child in node.children)
        {
            
            float scaleRatio = (float)(child.width * 1.3 / maxWidth);

            if (scaleRatio > 1)
            {
                scaleRatio = (float) (child.width / maxWidth);
            }

            float height = ScaleHeights(child);
            child.form.transform.localScale = new Vector3(scaleRatio, height, scaleRatio);
            if (child.children.Count() != null)
            {
                ScaleObject(child);
            }
        }           
    }

    /* * *
     * Scopes and leafs need a different way to scale their heights and this function is doing it.
     * The size of the scopes should be all the same while the leaf objects get heights relativ to it´s height-value.
     */

    private float ScaleHeights(CityObject node)
    {
        float height;
        
        if(IsScope(node)){
            height = 1f;
        }
        else
        {
            height = (float) (1/plane.transform.localScale.y) * node.height / 100; //40 = 1 / 0.01 (= height of the first scope)
        }
    
        return height;
    }

    /* * *
     * This function sets all creted objects in a line on one side of the table using its whole width.
     */

    private void PositionObjectsLine(CityObject node)
    {
        float usedSpace = 0;
        float middle = node.form.transform.localScale.x / 2;
        //SortChildren(node);
        foreach(CityObject child in node.children)
        {
            float xPos = 1 - child.form.transform.localScale.x;
            //Debug.Log(xPos);
            float yPos = 1.025f + (0.5f * child.height/100) + (node.form.transform.localScale.y); //0.025 is the top of the table
            float zPos = 1 - (usedSpace * 2) - child.form.transform.localScale.z;
            usedSpace += child.form.transform.localScale.z;
            child.form.transform.position = new Vector3(xPos, yPos, zPos);

            if (IsScope(child))
            {
                float planeHeight = plane.transform.localScale.y / 2;
                child.form.transform.position += new Vector3(0,planeHeight,0);
                PositionChildrenLine(child);
            }
        }
    }

    /* * *
     * Due to the way unity is calculating with its scaling of the parents etc. - this function is needed to position all objects 
     * beyond the second levelin the tree of all Objects.
     */

    private void PositionChildrenLine(CityObject node)
    {
        float usedSpace = 0;
        float middle = node.form.transform.localScale.x / 2;
        foreach (CityObject child in node.children)
        {
            float xPos = 1 - (child.form.transform.localScale.x * node.form.transform.localScale.x); //passt

            float yPos = 1.035f + (0.5f * child.height / 100) + node.form.transform.lossyScale.y ; 
            //1.035 => 1.03 is the yPos of the first Plane + 0.005 for the plane
            //1.03 => 1.025 is the top of the table + 0.005 for the first plane

            float zPos = (node.form.transform.position.z + node.form.transform.localScale.z) -
                ((child.form.transform.localScale.z + usedSpace) * node.form.transform.localScale.z);
            usedSpace += (child.form.transform.localScale.z * 2);

            child.form.transform.position = new Vector3(xPos, yPos, zPos);
            if (IsScope(child))
            {
                PositionChildrenLine(child);
            }
        }
    }

    private void PositionObjectsRadial(CityObject node)
    {
        LayoutVar variables = new LayoutVar();
        Debug.Log("Size: " + GetArraySize(node));
        variables.size = GetArraySize(node);
        GameObject[,] layout = new GameObject[variables.size, variables.size];
        distance = distance * 1.3 / node.width;

        foreach (CityObject child in node.children)
        {
            layout[variables.z, variables.x] = child.form;
            Debug.Log(variables.x +" und "+  variables.z);
            float xPos;
            float zPos;

            if (variables.x == 0 && variables.z == 0)
            {
                xPos = 1 - child.form.transform.localScale.x - (float)distance;
                zPos = 1 - child.form.transform.localScale.z - (float)distance;
            }
            else if (variables.x == 0 && variables.z != 0)
            {
                xPos = 1 - child.form.transform.localScale.x - (float)distance;
                zPos = layout[variables.z - 1, variables.x].transform.position.z 
                     - layout[variables.z - 1, variables.x].transform.localScale.z
                     - child.form.transform.localScale.z
                     - (float)distance * 2f;
            }
            else if (variables.x != 0 && variables.z == 0)
            {
                xPos = layout[variables.z, variables.x - 1].transform.position.x 
                     - layout[variables.z, variables.x - 1].transform.localScale.x
                     - child.form.transform.localScale.x
                     - (float)distance * 2f;
                zPos = 1 - child.form.transform.localScale.z - (float)distance;
            }
            else
            {
                xPos = layout[variables.z - 1, variables.x].transform.position.x - (float)distance * 2f; 

                zPos = layout[variables.z, variables.x - 1].transform.position.z - (float)distance * 2f;
            }

            float yPos = 1.025f + (0.5f * child.height / 100) + (node.form.transform.localScale.y);

            child.form.transform.position = new Vector3(xPos, yPos, zPos);

            if (IsScope(child))
            {
                float planeHeight = plane.transform.localScale.y / 2;
                child.form.transform.position += new Vector3(0, planeHeight, 0);
                PositionChildrenRadial(child);
            }

            LayoutManager(variables); 
        }
    }
    
    private void PositionChildrenRadial(CityObject node)
    {
        LayoutVar variables = new LayoutVar();
        variables.size = GetArraySize(node);
        GameObject[,] layout = new GameObject[variables.size, variables.size];

        foreach (CityObject child in node.children)
        {
            layout[variables.z, variables.x] = child.form;
            Debug.Log(variables.x + " und " + variables.z);
            float xPos;
            float zPos;

            if (variables.size == 1)
            {
                xPos = node.form.transform.position.x;
                zPos = node.form.transform.position.z;
            }
            else if (variables.x == 0 && variables.z == 0)
            {
                xPos = (node.form.transform.position.x + node.form.transform.localScale.x)
                        - (child.form.transform.localScale.x * node.form.transform.localScale.x)
                        - (float)distance;
                zPos = (node.form.transform.position.z + node.form.transform.localScale.z)
                        - (child.form.transform.localScale.z * node.form.transform.localScale.z)
                        - (float)distance;
            }
            else if (variables.x == 0 && variables.z != 0)
            {
                xPos = (node.form.transform.position.x + node.form.transform.localScale.x)
                        - (child.form.transform.localScale.x * node.form.transform.localScale.x)
                        - (float)distance;
                zPos = layout[variables.z - 1, variables.x].transform.position.z
                        - (layout[variables.z - 1, variables.x].transform.localScale.z * node.form.transform.localScale.z)
                        - (child.form.transform.localScale.z * node.form.transform.localScale.z)
                        - (float)distance * 2f;
            }
            else if (variables.x != 0 && variables.z == 0)
            {
                xPos = layout[variables.z, variables.x - 1].transform.position.x
                        - (layout[variables.z, variables.x - 1].transform.localScale.x * node.form.transform.localScale.x)
                        - (child.form.transform.localScale.x * node.form.transform.localScale.x)
                        - (float)distance * 2f;
                zPos = (node.form.transform.position.z + node.form.transform.localScale.z)
                        - (child.form.transform.localScale.z * node.form.transform.localScale.z)
                        - (float)distance;
            }
            else
            {
                xPos = layout[variables.z - 1, variables.x].transform.position.x - (float)distance * 2f;
                zPos = layout[variables.z, variables.x - 1].transform.position.z - (float)distance * 2f;
            }

            float yPos = plane.transform.position.y + (plane.transform.localScale.y/2) + (0.5f * child.height / 100) 
                 + node.form.transform.lossyScale.y; 

            child.form.transform.position = new Vector3(xPos, yPos, zPos);

            if (IsScope(child))
            {
                float planeHeight = plane.transform.localScale.y / 2;
                child.form.transform.position += new Vector3(0, planeHeight, 0);
                PositionChildrenRadial(child);
            }

            LayoutManager(variables);
        }
    }

    private LayoutVar LayoutManager(LayoutVar layoutVar)
    {
        int x = layoutVar.x;
        int z = layoutVar.z;
        int level = layoutVar.level;
        int size = layoutVar.size;

        if (x == z && level != size)
        {
            level += 1;
            z += 1;
            x = 0;
        }

        else if (z > x)
        {
            int temp = x;
            x = z;
            z = temp;
        }

        else if (z < x)
        {
            int temp = z;
            z = x;
            x = temp + 1;
        }

        layoutVar.x = x;
        layoutVar.z = z;
        layoutVar.level = level;
        return layoutVar;
    }

    /* * *
     * This function sorts all children of a cityobject in descending order
     */

    private void SortChildren(CityObject node)
    {
        node.children = node.children.OrderByDescending(x => x.width).ToList();
        foreach(CityObject child in node.children)
        {
            if (IsScope(child))
            {
                SortChildren(child);
            }
        }
    }

    /* * *
     * This function sets the material for every created GameObject
     */

    private void SetMaterials(CityObject node)
    {
        foreach(CityObject child in node.children)
        {
            valueManager.SetValue(child);
            if (IsScope(child)){
                SetMaterials(child);
            }
            
        }
    }


    /* * *
     * This function walks trough the whole tree and deletes the objects from the bottom to the top
     */

    public void DestroyBuiltCity(CityObject node)
    {
       foreach(CityObject child in node.children)
        {
            DestroyBuiltCity(child);
            
        }
            node.DestroyCityObject();

    }

    /* * *
     * This Method checks if a CityObject has children. If it has children, the function knows that the Object is 
     * a Scope and returns true. Else it just returns false because only Leafs of the tree haven´t got children.
     */

    private bool IsScope(CityObject cityObject)
    {
        if (cityObject.children.Count != 0) return true;
        else return false;
    }


    /* * *
     * For positioning the Objects radial, a Scope needs to be split into a n*n Matrix. This Function finds the next 
     * biggest square number n^2 and returns n for knowledge about the needed matrix size.
     */

    private int GetArraySize(CityObject node)
    {
        int size = 0; ;
        while ((size * size) < node.children.Count)
        {
            size++;
        }
        return size;
    }
}
