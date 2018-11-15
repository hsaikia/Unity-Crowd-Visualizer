using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Agent {
    public float r;
    public float g;
    public float b;
    public float radius;
    public List<Vector3> pos;
    public List<Vector3> tar;

    Vector3 forw_;

    public Agent(){
        pos = new List<Vector3>();
        tar = new List<Vector3>();
    }
    public Vector3 getForward(int t){
        Vector3 newforw;
        if(t == 0){
           newforw = tar[0] - pos[0];
        } else {
            newforw = pos[t] - pos[t - 1];
        }
        if(newforw.magnitude < 0.00001){
            return forw_;
        }
        forw_ = newforw;
        return forw_;
    }
}

public class Wall
{
    public Vector3 p1;
    public Vector3 p2;
}

public class SWCrowd : MonoBehaviour
{
    public bool showInfo;
    public GameObject walkerPrefab_;
    public GameObject targetPrefab_;
    public TextAsset csvFileAgents_;
    public TextAsset csvFileWalls_;
    private int numAgents_;
    private List<Agent> agents_;
    private List<GameObject> walkerAgents_;
    private List<GameObject> targetAgents_;
    private List<Wall> walls_;
    public Text info_;
    private int columns_;
    private int timestep;
    private int maxTimeStep;

    private int totalCollisions;
    public bool animate;
    public bool showTargets;

    [Range(1, 3)]
    public int colors_;

    [Range (0, 100)]
    public int trailLength;

    void Start()
    {
        agents_ = new List<Agent>();
        walls_ = new List<Wall>();
        walkerAgents_ = new List<GameObject>();
        targetAgents_ = new List<GameObject>();
        readCSV();

        if(animate){
            InvokeRepeating("moveAgents", 0.0f, 0.1f);
        }
        //InvokeRepeating("moveAgents", 0.0f, 0.1f);
    }

    void Update()
    {
        
    }

    void OnDrawGizmos(){

    }

    void readCSV(){
        char lineSeparator = '\n'; // It defines line seperate character
        char fieldSeparator = ','; // It defines field seperate chracter

        List<List<float> > dataitems = new List<List<float> >(); 

        string[] records = csvFileAgents_.text.Split(lineSeparator);

        bool ignoreFirst = true;

        foreach (string record in records)
        {
            string[] fields = record.Split(fieldSeparator);
            //Debug.Log(record);
            if(ignoreFirst){
                columns_ = fields.GetLength(0);
                ignoreFirst = false;
                continue;
            }

            //Debug.Log("Columns extracted " + fields.GetLength(0));

            if(fields.GetLength(0) != columns_){
                continue;
            }

            List<float> row = new List<float>();

            int c = 0;
            foreach(string field in fields)
            {
                if(c >= columns_){
                    break;
                }
                //Debug.Log("Field " + field);
                row.Add(float.Parse(field));       
                c++;
            }

            dataitems.Add(row);
        }
        
        for(int i = 0; i < dataitems.Count; i++){
            // TIME ,ID ,POS_X ,POS_Y, TAR_X, TAR_Y, AGENT_RADIUS, COLOR_R, COLOR_G, COLOR_B 
            int id = Convert.ToInt32(dataitems[i][1]);
            int time = Convert.ToInt32(dataitems[i][0]);

            maxTimeStep = Mathf.Max(maxTimeStep, time);

            while(id + 1 > agents_.Count){
                agents_.Add(new Agent());
            }
            
            agents_[id].pos.Add(new Vector3(dataitems[i][2], 0, dataitems[i][3]));
            agents_[id].tar.Add(new Vector3(dataitems[i][4], 0, dataitems[i][5]));
            agents_[id].radius = dataitems[i][6];
            agents_[id].r = dataitems[i][7];
            agents_[id].g = dataitems[i][8];
            agents_[id].b = dataitems[i][9];
        }

        numAgents_ = agents_.Count;

        // get walls

        if (csvFileWalls_.ToString() != "")
        {
            // wall file present
            records = csvFileWalls_.text.Split(lineSeparator);

            ignoreFirst = true;
            List<List<float>> wallItems = new List<List<float>>();

            foreach (string record in records)
            {
                string[] fields = record.Split(fieldSeparator);
                //Debug.Log(record);
                if (ignoreFirst)
                {
                    columns_ = fields.GetLength(0);
                    ignoreFirst = false;
                    continue;
                }

                //Debug.Log("Columns extracted " + fields.GetLength(0));

                if (fields.GetLength(0) != columns_)
                {
                    continue;
                }

                List<float> row = new List<float>();

                int c = 0;
                foreach (string field in fields)
                {
                    if (c >= columns_)
                    {
                        break;
                    }
                    //Debug.Log("Field " + field);
                    row.Add(float.Parse(field));
                    c++;
                }

                wallItems.Add(row);
            }

            for (int i = 0; i < wallItems.Count; i++)
            {
                Wall w = new Wall();
                w.p1.x = wallItems[i][1];
                w.p1.z = wallItems[i][2];
                w.p2.x = wallItems[i][3];
                w.p2.z = wallItems[i][4];
                walls_.Add(w);
            }
        }
        SetGameObjects();
    }

    void UpdateInfo(){
        info_.text = "B.o.B Do something!";
    }

    void ClearScene(){

        foreach(var wa in walkerAgents_){
            Destroy(wa);
        }

        walkerAgents_.Clear();

        foreach(var tar in targetAgents_){
            Destroy(tar);
        }

        targetAgents_.Clear();
    }

    void SetGameObjects()
    {
        ClearScene();
        totalCollisions = 0;
        for(int i = 0; i < numAgents_; i++)
        {
            GameObject walkerClone = Instantiate(walkerPrefab_, agents_[i].pos[0] , Quaternion.identity);
            if (colors_ == 3)
            {
                walkerClone.GetComponent<MeshRenderer>().material.color = new Color(agents_[i].r, agents_[i].g, agents_[i].b);
            } else if (colors_ == 2)
            {
                if(i < numAgents_ / 2)
                {
                    walkerClone.GetComponent<MeshRenderer>().material.color = Color.red;
                } else
                {
                    walkerClone.GetComponent<MeshRenderer>().material.color = Color.blue;
                }
            } else
            {
                walkerClone.GetComponent<MeshRenderer>().material.color = Color.red;
            }

            walkerClone.transform.localScale = new Vector3(2 * agents_[i].radius, 1, 2 * agents_[i].radius);
            walkerClone.transform.forward = agents_[i].getForward(0);
            walkerClone.SetActive(true);
            //line.startColor = new Color(agents_[i].r, agents_[i].g, agents_[i].b);
            //line.endColor = new Color(agents_[i].r, agents_[i].g, agents_[i].b);
            walkerClone.AddComponent(typeof(LineRenderer));
            walkerClone.GetComponent<LineRenderer>().material = new Material(Shader.Find("Sprites/Default"));

            if (colors_ == 3)
            {
                walkerClone.GetComponent<LineRenderer>().startColor = new Color(agents_[i].r, agents_[i].g, agents_[i].b);
            }
            else if (colors_ == 2)
            {
                if (i < numAgents_ / 2)
                {
                    walkerClone.GetComponent<LineRenderer>().startColor = new Color(1.0f, 0.5f, 0.0f);
                }
                else
                {
                    walkerClone.GetComponent<LineRenderer>().startColor = new Color(0.0f, 0.5f, 1.0f);
                }
            } else
            {
                walkerClone.GetComponent<LineRenderer>().startColor = new Color(1.0f, 0.5f, 0.0f);
            }

            walkerClone.GetComponent<LineRenderer>().widthMultiplier = 0.2f;

            GameObject targetClone = Instantiate(targetPrefab_, agents_[i].tar[0], Quaternion.identity);
            targetClone.GetComponent<MeshRenderer>().material.color = new Color(agents_[i].r, agents_[i].g, agents_[i].b);
            targetClone.transform.localScale = new Vector3(2 * agents_[i].radius, 1, 2 * agents_[i].radius);
            targetClone.SetActive(true);

            walkerAgents_.Add(walkerClone);
            targetAgents_.Add(targetClone);
        }
        for(int i = 0; i < walls_.Count; i++)
        {
            var directionVector = Quaternion.AngleAxis(90, Vector3.up) * (walls_[i].p2 - walls_[i].p1);

            GameObject cube;
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(directionVector.magnitude, 2, 0.1f);
            //quad.transform.rotation = Quaternion.Euler(90, 0, 0);
            //

            cube.transform.position = (walls_[i].p1 + walls_[i].p2) / 2;
            cube.transform.rotation = Quaternion.LookRotation(directionVector.normalized);
            cube.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 0);
        }

        totalCollisions = 0;
    }

    void moveAgents(){
       
        for(int i = 0; i < numAgents_; i++)
        {
            if (!showTargets)
            {
                targetAgents_[i].SetActive(false);
            }

            walkerAgents_[i].transform.SetPositionAndRotation(agents_[i].pos[timestep], Quaternion.identity);
            walkerAgents_[i].transform.forward = agents_[i].getForward(timestep);
            targetAgents_[i].transform.SetPositionAndRotation(agents_[i].tar[timestep], Quaternion.identity);

            List<Vector3> poss = new List<Vector3>();

            for(int t = timestep; t >= 0; t--){
                if(poss.Count >= trailLength){
                    break;
                }
                poss.Add(agents_[i].pos[t]);
            }

            walkerAgents_[i].GetComponent<LineRenderer>().positionCount = poss.Count;
            walkerAgents_[i].GetComponent<LineRenderer>().SetPositions(poss.ToArray());
        }

        // find number of collisions
        int collisions = 0;
        for(int i = 0; i < numAgents_; i++)
        {
            var di = walkerAgents_[i].transform.position - targetAgents_[i].transform.position;

            if(di.magnitude < agents_[i].radius)
            {
                continue;
            }

            for(int j = i + 1; j < numAgents_; j++)
            {
                var dj = walkerAgents_[j].transform.position - targetAgents_[j].transform.position;

                if (dj.magnitude < agents_[j].radius)
                {
                    continue;
                }

                if ((walkerAgents_[i].transform.position - walkerAgents_[j].transform.position).magnitude < 2 * agents_[i].radius){
                    collisions++;
                    totalCollisions++;
                }
            }
        }

        timestep = (timestep + 1) % maxTimeStep;

        if(timestep == 0)
        {
            totalCollisions = 0;
        }


        ScreenCapture.CaptureScreenshot("Screenshots/shot_" + timestep + ".png");

        if (!showInfo)
        {
            info_.text = "";
        } else
        {
            info_.text = "Timestep : " + timestep + "/" + maxTimeStep +
            "\nCollisions : " + collisions + "\nTotal Collisions : " + totalCollisions;
        }
    }

}