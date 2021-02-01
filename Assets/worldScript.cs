using System.IO;
using UnityEngine;
using static vehicleControl;

public class worldScript : MonoBehaviour
{

    public enum tileTypes { BUILDING, ROAD, NULL};

    public enum roadTypes { NORMAL, CROSS_VERTICAL, CROSS_HORIZONTAL, STOP_TOP_RIGHT, STOP_BOTTOM_RIGHT, STOP_BOTTOM_LEFT, STOP_TOP_LEFT }

    public enum tileStates { NORTH, SOUTH, EAST, WEST, FREE, PERSON };

    public struct tile
    {
        public tile (tileTypes _t)
        {
            whichType = _t;
            whichRoadType = roadTypes.NORMAL;
            neighborgs = new bool[4];
            tStates = tileStates.FREE;
            whoIsInThisTile = -1;
        }

        public tileTypes whichType;
        public roadTypes whichRoadType;
        public tileStates tStates;
        public bool [] neighborgs;
        public int whoIsInThisTile;
    };


    public GameObject roadTile, buildingTile, crossTile, stopTile, vehiclePrefab, personPrefab;
    public tile[,] worldTable;
    public int WIDTH, HEIGHT, nVehicles, nPeople;
    int minW, maxW, minH, maxH;

    Color[] vehicleColors = { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan, Color.magenta };
    Color[] peopleColors = { Color.black, Color.white, Color.grey };



    void Start()
    {

        worldTable = new tile[WIDTH, HEIGHT];

        minW = WIDTH / 8;
        maxW = WIDTH / 4;
        minH = HEIGHT / 8;
        maxH = HEIGHT / 4;

        resetWorld();

        createRoads();

        createTiles();

        intantiateWorld();

        instantiateVehicles();

        instantiatePeople();

        //showWorldText();

    }


    bool noOneCloseEnough(int x, int y)
    {

        bool isOK = true;

        for (int i = x - 2;i <= x + 2;i++)
        {
            for (int j = y - 2;j <= y + 2; j++)
            {

                if ((x >= 0) && (x < WIDTH) && (y >= 0) && (y < HEIGHT))
                {
                    tile t = getTileXY(i, j);
                    if ((t.whichType == tileTypes.ROAD) && (t.tStates != tileStates.FREE))
                    {
                        isOK = false;
                        break;
                    }
                }
            }
            if (isOK == false)
                break;

        }

        return isOK;

    }


    //Method to instantiate the people
    void instantiatePeople()
    {

        for (int i = 0; i < nPeople; i++)
        {
            bool busy = true;
            int x, y;
            x = 0;
            y = 0;
            while (busy == true)
            {
                x = Random.Range(0, WIDTH);
                y = Random.Range(0, HEIGHT);
                if (worldTable[x, y].whichType == tileTypes.BUILDING)
                    busy = false;
                else
                    busy = true;
            }

            GameObject newPerson = (GameObject)Instantiate(personPrefab, new Vector3((x - (WIDTH / 2)) * 2, 1.5f, ((y - (HEIGHT / 2)) * -1) * 2), Quaternion.identity);

            newPerson.GetComponent<MeshRenderer>().material.color = peopleColors[Random.Range(0, peopleColors.Length)];

            newPerson.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            newPerson.transform.SetParent(this.gameObject.transform);

            newPerson.GetComponent<personControl>().init(this, x, y, WIDTH, HEIGHT, i);

        }

    }


    //Method to instantiate the vehicles
    void instantiateVehicles()
    {

        for (int i = 0;i < nVehicles;i++)
        {
            bool busy = true;
            int x, y;
            x = 0;
            y = 0;
            while (busy == true)
            {
                x = Random.Range(0, WIDTH);
                y = Random.Range(0, HEIGHT);
                if ((worldTable[x, y].whichType == tileTypes.ROAD) && (worldTable[x, y].tStates == tileStates.FREE) && (noOneCloseEnough(x, y) == true))
                    busy = false;
                else
                    busy = true;
            }

            GameObject newVehicle = (GameObject)Instantiate(vehiclePrefab, new Vector3((x - (WIDTH / 2)) * 2, 1.5f, ((y - (HEIGHT / 2)) * -1) * 2), Quaternion.identity);

            newVehicle.GetComponent<MeshRenderer>().material.color = vehicleColors[Random.Range(0, vehicleColors.Length)];

            newVehicle.transform.localScale = new Vector3(1, 1, 1);
            newVehicle.transform.Rotate(new Vector3(0,0,1), 90);
            newVehicle.transform.SetParent(this.gameObject.transform);

            newVehicle.GetComponent<vehicleControl>().init(this, x, y, WIDTH, HEIGHT, i);

        }

    }


    roadTypes shouldIPutCrossOrStop(int x, int y)
    {

        roadTypes ret = roadTypes.NORMAL;

        bool veticalLineFound = true;

        for (int i = y - 2;i < y + 2;i++)
        {
            if ((i >= 0) && (i < HEIGHT) && (x > 0) && (x < WIDTH - 1))
            {
                if ((worldTable[x-1,i].whichType!=tileTypes.BUILDING) || (worldTable[x, i].whichType != tileTypes.ROAD) || (worldTable[x, i].whichRoadType != roadTypes.NORMAL) || (worldTable[x + 1, i].whichType != tileTypes.BUILDING))
                {
                    veticalLineFound = false;
                    break;
                }
            }
            else
            {
                veticalLineFound = false;
                break;
            }
        }
        if (veticalLineFound == true)
            ret = roadTypes.CROSS_VERTICAL;
        else
        {

            bool horizontalLineFound = true;

            for (int i = x - 2; i < x + 2; i++)
            {
                if ((i >= 0) && (i < WIDTH) && (y > 0) && (y < HEIGHT - 1))
                {
                    if ((worldTable[i, y-1].whichType != tileTypes.BUILDING) || (worldTable[i, y].whichType != tileTypes.ROAD) || (worldTable[i, y].whichRoadType != roadTypes.NORMAL) || (worldTable[i, y+1].whichType != tileTypes.BUILDING))
                    {
                        horizontalLineFound = false;
                        break;
                    }
                }
                else
                {
                    horizontalLineFound = false;
                    break;
                }
            }
            if (horizontalLineFound == true)
                ret = roadTypes.CROSS_HORIZONTAL;
            else//check the possibility of placing a "stop"
            {
                if ((y > 0) && (x < WIDTH - 1) && (worldTable[x, y].whichType == tileTypes.ROAD) && (worldTable[x+1, y].whichType == tileTypes.BUILDING) && 
                        (worldTable[x, y-1].whichType == tileTypes.ROAD) && (worldTable[x+1, y-1].whichType == tileTypes.ROAD))
                            ret = roadTypes.STOP_TOP_RIGHT;
                else if ((x < WIDTH -1) && (y < HEIGHT - 1) && (worldTable[x, y].whichType == tileTypes.ROAD) && (worldTable[x + 1, y].whichType == tileTypes.ROAD) &&
                    (worldTable[x, y + 1].whichType == tileTypes.BUILDING) && (worldTable[x + 1, y + 1].whichType == tileTypes.ROAD))
                        ret = roadTypes.STOP_BOTTOM_RIGHT;
                else if ((x > 0) && (y < HEIGHT - 1) && (worldTable[x, y].whichType == tileTypes.ROAD) && (worldTable[x - 1, y].whichType == tileTypes.BUILDING) &&
                    (worldTable[x, y + 1].whichType == tileTypes.ROAD) && (worldTable[x - 1, y + 1].whichType == tileTypes.ROAD))
                        ret = roadTypes.STOP_BOTTOM_LEFT;
                else if ((x > 0) && (y > 0) && (worldTable[x, y].whichType == tileTypes.ROAD) && (worldTable[x, y-1].whichType == tileTypes.BUILDING) &&
                    (worldTable[x-1, y].whichType == tileTypes.ROAD) && (worldTable[x - 1, y - 1].whichType == tileTypes.ROAD))
                        ret = roadTypes.STOP_TOP_LEFT;

            }

        }

        if ((ret != roadTypes.NORMAL) && (Random.Range(0, 2) > 0))
            return roadTypes.NORMAL;

        return ret;

    }


    //Once the tile habe been generated, creates the world, by instantiating gameObjects (LAND, ROAD, STOPS or CROSS)
    void intantiateWorld()
    {

        GameObject tile=null;

        for (int i = 0; i < WIDTH; i++)
        {

            for (int j = 0; j < HEIGHT; j++)
            {
                switch (worldTable[i, j].whichType)
                {
                    case tileTypes.BUILDING:
                        tile = (GameObject)Instantiate(buildingTile, new Vector3((i - (WIDTH / 2))*2, 0, ((j - (HEIGHT / 2)) * -1)*2), Quaternion.identity);
                        tile.transform.localScale = new Vector3(2,2,2);
                    break;

                    case tileTypes.ROAD:
                        roadTypes typeRoad = shouldIPutCrossOrStop(i, j);
                        GameObject instantiatedObject = null;
                        int rotation = 0;
                        switch (typeRoad)
                        {
                            case roadTypes.NORMAL:
                                instantiatedObject = roadTile;
                                worldTable[i, j].whichRoadType = roadTypes.NORMAL;
                            break;

                            case roadTypes.CROSS_HORIZONTAL:
                                instantiatedObject = crossTile;
                                worldTable[i, j].whichRoadType = roadTypes.CROSS_HORIZONTAL;
                                rotation = 0;
                            break;

                            case roadTypes.CROSS_VERTICAL:
                                instantiatedObject = crossTile;
                                worldTable[i, j].whichRoadType = roadTypes.CROSS_VERTICAL;
                                rotation = 90;
                            break;

                            case roadTypes.STOP_TOP_RIGHT:
                                instantiatedObject = stopTile;
                                worldTable[i, j].whichRoadType = roadTypes.STOP_TOP_RIGHT;
                                rotation = 180;
                            break;

                            case roadTypes.STOP_BOTTOM_RIGHT:
                                instantiatedObject = stopTile;
                                worldTable[i, j].whichRoadType = roadTypes.STOP_BOTTOM_RIGHT;
                                rotation = 270;
                            break;

                            case roadTypes.STOP_BOTTOM_LEFT:
                                instantiatedObject = stopTile;
                                worldTable[i, j].whichRoadType = roadTypes.STOP_BOTTOM_LEFT;
                                rotation = 360;
                            break;

                            case roadTypes.STOP_TOP_LEFT:
                                instantiatedObject = stopTile;
                                worldTable[i, j].whichRoadType = roadTypes.STOP_TOP_LEFT;
                                rotation = 90;
                            break;

                        }

                        tile = (GameObject)Instantiate(instantiatedObject, new Vector3((i - (WIDTH / 2))*2, 0, ((j - (HEIGHT / 2)) * -1)*2), Quaternion.identity);
                        tile.transform.localScale = new Vector3(2, 2, 2);
                        tile.transform.Rotate(new Vector3(0,1,0),rotation);

                    break;

                }
                tile.transform.SetParent(this.gameObject.transform);

            }

        }

    }


    //After creating the roads, fill up each tile with its neighbours (whether a cell has exit from North, South, East or West)
    void createTiles()
    {

        for (int i = 0;i < WIDTH;i++)
        {

            for (int j = 0; j < HEIGHT; j++)
            {

                if (worldTable[i,j].whichType == tileTypes.ROAD)
                {

                    if (j >= 1)
                    {
                        if (worldTable[i, j - 1].whichType == tileTypes.ROAD)
                            worldTable[i, j].neighborgs[0] = true;
                        else
                            worldTable[i, j].neighborgs[0] = false;
                    }
                    else
                        worldTable[i, j].neighborgs[0] = false;


                    if (j < HEIGHT - 1)
                    {
                        if ((worldTable[i, j + 1].whichType == tileTypes.ROAD))
                            worldTable[i, j].neighborgs[1] = true;
                        else
                            worldTable[i, j].neighborgs[1] = false;
                    }
                    else
                        worldTable[i, j].neighborgs[1] = false;

                    if (i >= 1)
                    {
                        if ((worldTable[i - 1, j].whichType == tileTypes.ROAD))
                            worldTable[i, j].neighborgs[2] = true;
                        else
                            worldTable[i, j].neighborgs[2] = false;
                    }
                    else
                        worldTable[i, j].neighborgs[2] = false;

                    if (i < WIDTH - 1)
                    {
                        if ((worldTable[i + 1, j].whichType == tileTypes.ROAD))
                            worldTable[i, j].neighborgs[3] = true;
                        else
                            worldTable[i, j].neighborgs[3] = false;
                    }
                    else
                        worldTable[i, j].neighborgs[3] = false;

                }

            }

        }

    }


    //Create some random roads...
    //First draw some vertical ones with a random distance between them, 
    //and then some horizontal ones...
    void createRoads()
    {

        int startX = Random.Range(1, minW);
        int startY, finalY, finalX;

        while (startX < WIDTH)
        {
            startY = Random.Range(1, minH-1);
            finalY = Random.Range(HEIGHT - minH, HEIGHT + minH);

            for (int j = startY; j < finalY; j++)
            {
                if ((startX >= 1) && (startX < WIDTH - 1) && (j >= 1) && (j < HEIGHT - 1))
                {
                    worldTable[startX, j].whichType = tileTypes.ROAD;
                    worldTable[startX, j].tStates = tileStates.FREE;
                }
            }
            startX += 2 + Random.Range(minW - 2, minW + 2);

        }

        startY = Random.Range(1, minH);

        while (startY < HEIGHT)
        {
            startX = Random.Range(1, minW);
            finalX = Random.Range(WIDTH - minW, WIDTH + minW);

            for (int j = startX; j < finalX; j++)
            {
                if ((j >= 1) && (j < WIDTH - 1) && (startY >= 1) && (startY < HEIGHT - 1))
                {
                    worldTable[j, startY].whichType = tileTypes.ROAD;
                    worldTable[j, startY].tStates = tileStates.FREE;
                }
            }

            startY += 2 + Random.Range(minH - 2, minH + 2);

        }

    }


    //Returns a cell, giving its x and y coordinates
    public tile getTileXY(int x, int y)
    {

        tile t = new tile();
        t.whichType = tileTypes.NULL;

        if ((x >= 0) && (x < WIDTH) && (y >= 0) && (y < HEIGHT))
            return worldTable[x, y];
        else
            return t;

    }


    //Make everything a building! NO roads yet...
    void resetWorld()
    {

        for (int j = 0; j < HEIGHT; j++)
        {
            for (int i = 0; i < WIDTH; i++)
            {
                tile t = new tile(tileTypes.BUILDING);
                t.tStates = tileStates.FREE;
                worldTable[i, j] = t;
            }
        }

    }

    
    //For testing purposes... 
    //This funtion writes a file text to show each tile with its "neighbours" 0->North, 1->South, 2->West, 3->East
    void showWorldText()
    {

        var file = File.Open(Application.persistentDataPath + "/test.txt", FileMode.Create, FileAccess.Write);
        var writer = new StreamWriter(file);

        for (int j = 0; j < HEIGHT; j++)
        {
            for (int i = 0; i < WIDTH; i++)
            {
                if (worldTable[i, j].whichType == tileTypes.ROAD)
                {
                    int kk = 0;

                    if (worldTable[i, j].neighborgs[0] == true)
                        kk += 1;
                    if (worldTable[i, j].neighborgs[1] == true)
                        kk += 2;
                    if (worldTable[i, j].neighborgs[2] == true)
                        kk += 4;
                    if (worldTable[i, j].neighborgs[3] == true)
                        kk += 8;

                    writer.Write(kk.ToString("X1"));
                }
                else
                    writer.Write("0");// writer.Write(((int)worldTable[i,j].whichType).ToString());
            }
            writer.Write(System.Environment.NewLine);
        }

        writer.Close();

    }


    void Update()
    {

    }

}