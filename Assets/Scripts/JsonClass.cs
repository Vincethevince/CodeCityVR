using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* * * Root-Class
 * The URLs of all project-index-files should be written in a Root-Object
 */
[System.Serializable]
public class Root
{
    public Views[] projects;
}

/* * * JsonProject-Class
 * The name and the URLs of all complexity-files should be written in a JsonProject-Object
 */
[System.Serializable]
public class JsonProject
{
    public string name;
    public Views[] views;
}

/* * * Views-Class
 * Class for being able to deserialize Arrays from Json Files into C#-Objects 
 */
[System.Serializable]
public class Views
{
    public string href;
}

/* * * ProjectView-Class
 * Root of a Complexity File
 */
[System.Serializable]
public class ProjectView
{
    public string visualization;
    public string name;
    public ProjectData[] data;
}

/* * * ProjectData-Class
 * Actual Data from the Objects in a project
 */
[System.Serializable]
public class ProjectData
{
    public int id;
    public string type;
    public double width;
    public int height;
    public int value;
    public ProjectData[] children;

    public void setParents(ProjectData parent)
    {
        foreach (ProjectData c in children)
        {
            c.setParents(this);
        }
    }
}

public class LayoutVar
{
    public int x;
    public int z;
    public int level;
    public int size;
    
    public LayoutVar()
    {
        x = 0;
        z = 0;
        level = 0;
    }
}
