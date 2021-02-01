using System.Collections.Generic;
using UnityEngine;
using static worldScript;

public class vehicleControl : MonoBehaviour
{

    public enum directions { NORTH, SOUTH, EAST, WEST};

    public worldScript myWorldScript;
    int posX, posY, id, width, height, timeWhenStopped;
    int xTrailStart, yTrailStart, xTrailStop, yTrailStop;
    public directions myDirection, prevDirection;
    float tFactor, timeStopped;
    Vector3 currentPos, prevPos, nextPos;
    worldScript.tile currentTile;
    int THRESHOLD_STOP_TIME = 2;
    int TIME_AFTER_STOP = 1000;

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

    bool stopped;



    void Start()
    {

        timeStopped = 0;
        stopped = false;

        listOfPositionsAhead = new List<posAhead>();
        listOfPositionsAhead.Clear();

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
                    if (//Always look on the right (side of life)!!
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


    bool shouldIStop(int x, int y, directions d)
    {

        int timeNow = (System.DateTime.Now.Hour * 3600000) + (System.DateTime.Now.Minute * 60000) + (System.DateTime.Now.Second * 1000) + (System.DateTime.Now.Millisecond);

        if ((stopped == true) && ((timeNow - timeWhenStopped) < TIME_AFTER_STOP))
            return false;
        else
        {

            bool ret = false;
            tile t, t2;
            switch (d)
            {
                case directions.NORTH:
                    t = myWorldScript.getTileXY(x, y);
                    t2 = myWorldScript.getTileXY(x, y - 1);
                    if ((t.whichRoadType == roadTypes.STOP_TOP_RIGHT) /*|| (t2.tStates == tileStates.PERSON)*/ || (t2.tStates != tileStates.FREE))
                        ret = true;
                    break;

                case directions.SOUTH:
                    t = myWorldScript.getTileXY(x, y);
                    t2 = myWorldScript.getTileXY(x, y + 1);
                    if ((t.whichRoadType == roadTypes.STOP_BOTTOM_LEFT) /*|| (t2.tStates == tileStates.PERSON)*/ || (t2.tStates != tileStates.FREE))
                        ret = true;
                    break;

                case directions.EAST:
                    t = myWorldScript.getTileXY(x, y);
                    t2 = myWorldScript.getTileXY(x + 1, y);
                    if ((t.whichRoadType == roadTypes.STOP_BOTTOM_RIGHT) /*|| (t2.tStates == tileStates.PERSON)*/ || (t2.tStates != tileStates.FREE))
                        ret = true;
                    break;

                case directions.WEST:
                    t = myWorldScript.getTileXY(x, y);
                    t2 = myWorldScript.getTileXY(x - 1, y);
                    if ((t.whichRoadType == roadTypes.STOP_TOP_LEFT) /*|| (t2.tStates == tileStates.PERSON)*/ || (t2.tStates != tileStates.FREE))
                        ret = true;
                    break;

            }

            stopped = ret;

            return ret;
        }

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

        bool forceExit = false;
        if (timeStopped > THRESHOLD_STOP_TIME)
        {
            forceExit = true;
            timeWhenStopped = (System.DateTime.Now.Hour * 3600000) + (System.DateTime.Now.Minute * 60000) + (System.DateTime.Now.Second * 1000) + (System.DateTime.Now.Millisecond);
        }

        if ((shouldIStop(posX, posY, myDirection) == true) && (forceExit == false))
        {

        }
        else
        {
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

}