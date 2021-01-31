using System.Collections.Generic;
using UnityEngine;
using static worldScript;

public class vehicleControl : MonoBehaviour
{

    public enum directions { NORTH, SOUTH, EAST, WEST};

    public worldScript myWorldScript;

    int posX, posY, id, width, height;
    int xTrailStart, yTrailStart, xTrailStop, yTrailStop;

    public directions myDirection, prevDirection;

    float tFactor, timeStopped;

    Vector3 currentPos, prevPos, nextPos;
    
    worldScript.tile currentTile;

    
    public struct posAhead
    {
        public posAhead(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
        public int x, y;
    }

    List<posAhead> listOfPositionsAhead;

    //List<GameObject> sphereTests;



    void Start()
    {

        timeStopped = 0;

        listOfPositionsAhead = new List<posAhead>();
        listOfPositionsAhead.Clear();

        //sphereTests = new List<GameObject>();
        //sphereTests.Clear();

    }

    directions getInitialDirection(tile currentTile, directions currentDirection)
    {

        directions myDirection = currentDirection;

        if ((currentTile.neighborgs[0] == true) && (currentTile.neighborgs[1] == true))//vertical
        {
            if (Random.Range(0, 2) == 0)
                myDirection = directions.NORTH;
            else
                myDirection = directions.SOUTH;
        }
        else if ((currentTile.neighborgs[2] == true) && (currentTile.neighborgs[3] == true))//horizontal
        {
            if (Random.Range(0, 2) == 0)
                myDirection = directions.EAST;
            else
                myDirection = directions.WEST;
        }
        else if ((currentTile.neighborgs[0] == true) && (currentTile.neighborgs[1] == false))//north
        {
            myDirection = directions.NORTH;
        }
        else if ((currentTile.neighborgs[0] == false) && (currentTile.neighborgs[1] == true))//south
        {
            myDirection = directions.SOUTH;
        }
        else if ((currentTile.neighborgs[2] == true) && (currentTile.neighborgs[3] == false))//WEST
        {
            myDirection = directions.WEST;
        }
        else if ((currentTile.neighborgs[2] == false) && (currentTile.neighborgs[3] == true))//EAST
        {
            myDirection = directions.EAST;
        }

        return myDirection;

    }

    void setNewDirection(directions currentDirection)
    {

        prevDirection = currentDirection;

        currentTile = myWorldScript.getTileXY(posX, posY);

        if (currentTile.whichType != tileTypes.NULL)
        {
            switch (currentDirection)
            {

                case directions.NORTH:
                case directions.SOUTH:
                    if (Random.Range(0, 10) > 3) { }//70% chance of continuing the same direction
                    else//turn left or right, or inverting the direction
                    {
                        if ((currentTile.neighborgs[2] == true) && (currentTile.neighborgs[3] == true))//horizontal
                        {
                            if (Random.Range(0, 2) == 0)
                                myDirection = directions.EAST;
                            else
                                myDirection = directions.WEST;
                        }
                        else if ((currentTile.neighborgs[2] == true) && (currentTile.neighborgs[3] == false))//WEST
                        {
                            myDirection = directions.WEST;
                        }
                        else if ((currentTile.neighborgs[2] == false) && (currentTile.neighborgs[3] == true))//EAST
                        {
                            myDirection = directions.EAST;
                        }
                        else if ((currentDirection == directions.SOUTH) && (currentTile.neighborgs[0] == true) && (currentTile.neighborgs[1] == false))
                            myDirection = directions.NORTH;
                        else if ((currentDirection == directions.NORTH) && (currentTile.neighborgs[0] == false) && (currentTile.neighborgs[1] == true))
                            myDirection = directions.SOUTH;
                    }
                    break;

                case directions.WEST:
                case directions.EAST:
                    if (Random.Range(0, 10) > 3) { }//70% chance of continuing the same direction
                    else//turn up or down, or inverting the direction
                    {
                        if ((currentTile.neighborgs[0] == true) && (currentTile.neighborgs[1] == true))//vertical
                        {
                            if (Random.Range(0, 2) == 0)
                                myDirection = directions.NORTH;
                            else
                                myDirection = directions.SOUTH;
                        }
                        else if ((currentTile.neighborgs[0] == true) && (currentTile.neighborgs[1] == false))//north
                        {
                            myDirection = directions.NORTH;
                        }
                        else if ((currentTile.neighborgs[0] == false) && (currentTile.neighborgs[1] == true))//south
                        {
                            myDirection = directions.SOUTH;
                        }
                    }
                break;
            }

            switch (myDirection)
            {
                case directions.NORTH:
                    currentTile = myWorldScript.getTileXY(posX, posY - 1);
                    if (prevDirection!=directions.NORTH)
                    {
                        if (prevDirection == directions.WEST)
                            transform.Rotate(new Vector3(1, 0, 0), 90f * 3f);
                        else
                            transform.Rotate(new Vector3(1, 0, 0), 90f);
                    }
                    
                    if ((currentTile.whichType != tileTypes.NULL) && (currentTile.neighborgs[0] == true))
                    {
                        posY--;
                        nextPos = new Vector3((posX - (width / 2)) * 2 + 0.5f, 1.5f, ((posY - (height / 2)) * -1) * 2);
                    }
                    else if ((currentTile.whichType != tileTypes.NULL) && (currentTile.neighborgs[0] == false))//////////////////////
                    {
                        transform.Rotate(new Vector3(1, 0, 0), 180f);
                        myDirection = directions.SOUTH;
                    }
                    break;

                case directions.SOUTH:
                    currentTile = myWorldScript.getTileXY(posX, posY + 1);
                    if (prevDirection != directions.SOUTH)
                    {
                        if (prevDirection == directions.WEST)
                            transform.Rotate(new Vector3(1, 0, 0), 90f);
                        else
                            transform.Rotate(new Vector3(1, 0, 0), 90f * 3f);
                    }

                    if ((currentTile.whichType != tileTypes.NULL) && (currentTile.neighborgs[1] == true))
                    {
                        posY++;
                        nextPos = new Vector3((posX - (width / 2)) * 2 - 0.5f, 1.5f, ((posY - (height / 2)) * -1) * 2);
                    }
                    else if ((currentTile.whichType != tileTypes.NULL) && (currentTile.neighborgs[1] == false))//////////////////////
                    {
                        transform.Rotate(new Vector3(1, 0, 0), 180f);
                        myDirection = directions.NORTH;
                    }
                break;

                case directions.WEST:
                    currentTile = myWorldScript.getTileXY(posX - 1, posY);
                    if (prevDirection != directions.WEST)
                    {
                        if (prevDirection == directions.NORTH)
                            transform.Rotate(new Vector3(1, 0, 0), 90f * 3f);
                        else
                            transform.Rotate(new Vector3(1, 0, 0), 90f);
                    }

                    if ((currentTile.whichType != tileTypes.NULL) && (currentTile.neighborgs[3] == true))
                    {
                        posX--;
                        nextPos = new Vector3((posX - (width / 2)) * 2, 1.5f, ((posY - (height / 2)) * -1) * 2 + 0.5f);
                    }
                    else if (((currentTile.whichType != tileTypes.NULL) && (currentTile.neighborgs[2] == false)) || (currentTile.whichType != tileTypes.BUILDING))
                    {
                        transform.Rotate(new Vector3(1, 0, 0), 180f);
                        myDirection = directions.EAST;
                    }
                break;

                case directions.EAST:
                    currentTile = myWorldScript.getTileXY(posX + 1, posY);
                    if (prevDirection != directions.EAST)
                    {
                        if (prevDirection == directions.NORTH)
                            transform.Rotate(new Vector3(1, 0, 0), 90f);
                        else
                            transform.Rotate(new Vector3(1, 0, 0), 90f * 3f);
                    }

                    if ((currentTile.whichType != tileTypes.NULL) && (currentTile.neighborgs[2] == true))
                    {
                        posX++;
                        nextPos = new Vector3((posX - (width / 2)) * 2, 1.5f, ((posY - (height / 2)) * -1) * 2 - 0.5f);
                    }
                    else if (((currentTile.whichType != tileTypes.NULL) && (currentTile.neighborgs[3] == false)) || (currentTile.whichType != tileTypes.BUILDING))
                    {
                        transform.Rotate(new Vector3(1, 0, 0), 180f);
                        myDirection = directions.WEST;
                    }
                break;

            }
        }

        currentPos = transform.localPosition;

    }


    public void init(worldScript _myWorldScript, int x, int y, int w, int h, int _i)
    {

        myWorldScript = _myWorldScript;
        posX = x;
        posY = y;
        width = w;
        height = h;
        tFactor = 0f;
        id = _i;

        currentTile = myWorldScript.getTileXY(posX, posY);

        //myWorldScript.setStateTile(x, y, false);

        myDirection = getInitialDirection(currentTile, myDirection);
        prevDirection = myDirection;

        switch (myDirection)
        {
            case directions.NORTH:
                transform.Rotate(new Vector3(1, 0, 0), 90f);
                transform.localPosition = new Vector3(transform.localPosition.x + 0.5f, transform.localPosition.y, transform.localPosition.z);
                nextPos = new Vector3((posX - (width / 2)) * 2 +0.5f, 1.5f, (((posY-1) - (height / 2)) * -1) * 2);
            break;

            case directions.SOUTH:
                transform.Rotate(new Vector3(1, 0, 0), 270f);
                transform.localPosition = new Vector3(transform.localPosition.x - 0.5f, transform.localPosition.y, transform.localPosition.z);
                nextPos = new Vector3((posX - (width / 2)) * 2-0.5f, 1.5f, (((posY + 1) - (height / 2)) * -1) * 2);
            break;

            case directions.WEST:
                transform.Rotate(new Vector3(1, 0, 0), 0f);
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 0.5f);
                nextPos = new Vector3(((posX - 1) - (width / 2)) * 2, 1.5f, ((posY - (height / 2)) * -1) * 2 +0.5f);
            break;

            case directions.EAST:
                transform.Rotate(new Vector3(1, 0, 0), 180f);
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - 0.5f);
                nextPos = new Vector3(((posX + 1) - (width / 2)) * 2, 1.5f, ((posY - (height / 2)) * -1) * 2-0.5f);
            break;

        }

        currentPos = transform.localPosition;
        prevPos = currentPos;

    }

    void deleteTrail()
    {

        for (int i = 0; i < listOfPositionsAhead.Count; i++)
        {
            posAhead pa = listOfPositionsAhead[i];
            myWorldScript.worldTable[pa.x, pa.y].tStates = tileStates.FREE;
            myWorldScript.worldTable[pa.x, pa.y].whoIsInThisTile = -1;
        }

        listOfPositionsAhead.Clear();

        //for (int i=0;i<sphereTests.Count;i++)
        //    GameObject.Destroy(sphereTests[i].gameObject);


    }

    void markTrail()
    {

        for (int i = xTrailStart; i <= xTrailStop; i++)
        {
            for (int j = yTrailStart; j <= yTrailStop; j++)
            {
                tile t = myWorldScript.getTileXY(i, j);
                if (t.whichType == tileTypes.ROAD)
                {
                    t.tStates = (tileStates)myDirection;
                    t.whoIsInThisTile = id;
                    myWorldScript.worldTable[i, j] = t;

                    listOfPositionsAhead.Add(new posAhead(i, j));
                    ////////////////////////////////////////////////
                    //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //sphere.transform.localPosition = new Vector3((i - (myWorldScript.WIDTH / 2)) * 2, 1.5f, ((j - (myWorldScript.HEIGHT / 2)) * -1) * 2);
                    //sphere.GetComponent<MeshRenderer>().material.color = Color.white;
                    //sphere.transform.localScale = new Vector3(1, 1, 1);
                    //sphere.transform.SetParent(this.gameObject.transform);
                    //sphereTests.Add(sphere);
                    ////////////////////////////////////////////////

                }
            }
        }
    }



    public float increasePosition(int x, int y, directions d, int id)
    {

        xTrailStart = 0;
        xTrailStop = 0;
        yTrailStart = 0;
        yTrailStop = 0;
        int square = 3;

        bool isOkToMove = true;

        switch (d)
        {
            case directions.NORTH:
                xTrailStart = x;
                xTrailStop = x;
                yTrailStart = y - square;
                yTrailStop = y - 1;
            break;

            case directions.SOUTH:
                xTrailStart = x;
                xTrailStop = x;
                yTrailStart = y + 1;
                yTrailStop = y + square;
            break;

            case directions.EAST:
                xTrailStart = x + 1;
                xTrailStop = x + square;
                yTrailStart = y;
                yTrailStop = y;
            break;

            case directions.WEST:
                xTrailStart = x - square;
                xTrailStop = x - 1;
                yTrailStart = y;
                yTrailStop = y;
            break;

        }

        for (int i = xTrailStart; i <= xTrailStop; i++)
        {
            for (int j = yTrailStart; j <= yTrailStop; j++)
            {
                tile t = myWorldScript.getTileXY(i, j);
                if ((t.whichType == tileTypes.ROAD) && (t.whoIsInThisTile != id))
                {
                    if (//Always look on the right (side of life! )!!
                        ((d == directions.NORTH) && ((t.tStates == tileStates.WEST) || (t.tStates == tileStates.NORTH))) ||
                        ((d == directions.SOUTH) && ((t.tStates == tileStates.EAST) || (t.tStates == tileStates.SOUTH))) ||
                        ((d == directions.EAST) && ((t.tStates == tileStates.NORTH) || (t.tStates == tileStates.EAST))) ||
                        ((d == directions.WEST) && ((t.tStates == tileStates.SOUTH) || (t.tStates == tileStates.WEST)))
                        )
                    {
                        isOkToMove = false;
                        break;
                    }

                }
            }
            if (isOkToMove == false)
                break;
        }

        if (isOkToMove == true)
            return 0.1f;
        else
            return 0f;

    }


    void Update()
    {
        
        transform.localPosition = Vector3.Lerp(currentPos, nextPos, tFactor);

        if (transform.localPosition != prevPos)
        {
            prevPos = transform.localPosition;
            timeStopped = 0f;
        }
        else
            timeStopped += Time.deltaTime;

        //GetComponentInChildren<TextMesh>().text = timeStopped.ToString();

        tFactor += increasePosition(posX, posY, myDirection, id);// 0.1f;

        if (tFactor >= 1f)//arrives to the next tile
        {

            deleteTrail();

            tFactor = 0f;

            currentPos = nextPos;

            setNewDirection(myDirection);

            markTrail();

        }

    }

}