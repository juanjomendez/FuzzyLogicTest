using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static worldScript;

public class personControl : MonoBehaviour
{


    public enum directions { NORTH, SOUTH, EAST, WEST };

    public worldScript myWorldScript;
    int posX, posY, id, width, height;
    float tFactor, randomSpeed;
    worldScript.tile currentTile;
    public directions myDirection, prevDirection;
    Vector3 currentPos, prevPos, nextPos;


    void Start()
    {
        
    }


    directions getInitialDirection(tile currentTile, int x, int y)
    {

        if (
            (Random.Range(0, 2) > 0) &&
            ((myWorldScript.getTileXY(x, y - 1).whichType == tileTypes.BUILDING) ||
            ((myWorldScript.getTileXY(x, y - 1).whichType == tileTypes.ROAD) && (myWorldScript.getTileXY(x, y - 1).whichRoadType == roadTypes.CROSS_VERTICAL)))
            )
            myDirection = directions.NORTH;

        if (
            (Random.Range(0, 2) > 0) &&
            ((myWorldScript.getTileXY(x, y + 1).whichType == tileTypes.BUILDING) ||
            ((myWorldScript.getTileXY(x, y + 1).whichType == tileTypes.ROAD) && (myWorldScript.getTileXY(x, y + 1).whichRoadType == roadTypes.CROSS_VERTICAL)))
            )
            myDirection = directions.SOUTH;

        if (
            (Random.Range(0, 2) > 0) &&
            ((myWorldScript.getTileXY(x - 1, y).whichType == tileTypes.BUILDING) ||
            ((myWorldScript.getTileXY(x - 1, y).whichType == tileTypes.ROAD) && (myWorldScript.getTileXY(x - 1, y).whichRoadType == roadTypes.CROSS_HORIZONTAL)))
            )
            myDirection = directions.WEST;

        if (
            (Random.Range(0, 2) > 0) &&
            ((myWorldScript.getTileXY(x + 1, y).whichType == tileTypes.BUILDING) ||
            ((myWorldScript.getTileXY(x + 1, y).whichType == tileTypes.ROAD) && (myWorldScript.getTileXY(x + 1, y).whichRoadType == roadTypes.CROSS_HORIZONTAL)))
            )
            myDirection = directions.EAST;

        return myDirection;

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

        myDirection = getInitialDirection(currentTile, posX, posY);
        prevDirection = myDirection;

        switch (myDirection)
        {
            case directions.NORTH:
                transform.localPosition = new Vector3(transform.localPosition.x + 0.5f, transform.localPosition.y, transform.localPosition.z);
                nextPos = new Vector3((posX - (width / 2)) * 2, 1.5f, (((posY - 1) - (height / 2)) * -1) * 2);
                break;

            case directions.SOUTH:
                transform.localPosition = new Vector3(transform.localPosition.x - 0.5f, transform.localPosition.y, transform.localPosition.z);
                nextPos = new Vector3((posX - (width / 2)) * 2, 1.5f, (((posY + 1) - (height / 2)) * -1) * 2);
                break;

            case directions.WEST:
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 0.5f);
                nextPos = new Vector3(((posX - 1) - (width / 2)) * 2, 1.5f, ((posY - (height / 2)) * -1) * 2);
                break;

            case directions.EAST:
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - 0.5f);
                nextPos = new Vector3(((posX + 1) - (width / 2)) * 2, 1.5f, ((posY - (height / 2)) * -1) * 2);
                break;

        }

        currentPos = transform.localPosition;
        prevPos = currentPos;
    }


    float increasePosition(int x, int y, directions d, int id)
    {

        return 0.025f + randomSpeed;

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
                        if ((myWorldScript.getTileXY(posX - 1, posY).whichType == tileTypes.BUILDING) && (myWorldScript.getTileXY(posX + 1, posY).whichType == tileTypes.BUILDING))//horizontal
                        {
                            if (Random.Range(0, 2) == 0)
                                myDirection = directions.EAST;
                            else
                                myDirection = directions.WEST;
                        }
                        else if ((myWorldScript.getTileXY(posX - 1, posY).whichType == tileTypes.BUILDING) && (myWorldScript.getTileXY(posX + 1, posY).whichType != tileTypes.BUILDING))//WEST
                        {
                            myDirection = directions.WEST;
                        }
                        else if ((myWorldScript.getTileXY(posX + 1, posY).whichType == tileTypes.BUILDING) && (myWorldScript.getTileXY(posX - 1, posY).whichType != tileTypes.BUILDING))//EAST
                        {
                            myDirection = directions.EAST;
                        }
                        else if ((currentDirection == directions.SOUTH) && (myWorldScript.getTileXY(posX, posY - 1).whichType == tileTypes.BUILDING) && (myWorldScript.getTileXY(posX, posY + 1).whichType != tileTypes.BUILDING))
                            myDirection = directions.NORTH;
                        else if ((currentDirection == directions.NORTH) && (myWorldScript.getTileXY(posX, posY + 1).whichType == tileTypes.BUILDING) && (myWorldScript.getTileXY(posX, posY - 1).whichType != tileTypes.BUILDING))
                            myDirection = directions.SOUTH;
                    }
                    break;

                case directions.WEST:
                case directions.EAST:
                    if (Random.Range(0, 10) > 3) { }//70% chance of continuing the same direction
                    else//turn up or down, or inverting the direction
                    {
                        if ((myWorldScript.getTileXY(posX, posY - 1).whichType == tileTypes.BUILDING) && (myWorldScript.getTileXY(posX, posY + 1).whichType == tileTypes.BUILDING))//horizontal
                        {
                            if (Random.Range(0, 2) == 0)
                                myDirection = directions.NORTH;
                            else
                                myDirection = directions.SOUTH;
                        }
                        else if ((myWorldScript.getTileXY(posX, posY - 1).whichType == tileTypes.BUILDING) && (myWorldScript.getTileXY(posX, posY + 1).whichType != tileTypes.BUILDING))
                        {
                            myDirection = directions.NORTH;
                        }
                        else if ((myWorldScript.getTileXY(posX, posY + 1).whichType == tileTypes.BUILDING) && (myWorldScript.getTileXY(posX, posY - 1).whichType != tileTypes.BUILDING))
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

                    if ((currentTile.whichType == tileTypes.BUILDING) || ((currentTile.whichType == tileTypes.ROAD) && (currentTile.whichRoadType == roadTypes.CROSS_HORIZONTAL)) )
                    {
                        posY--;
                        nextPos = new Vector3((posX - (width / 2)) * 2 + 0.5f, 1.5f, ((posY - (height / 2)) * -1) * 2);
                    }
                    else
                    {
                        myDirection = directions.SOUTH;
                    }
                    break;

                case directions.SOUTH:
                    currentTile = myWorldScript.getTileXY(posX, posY + 1);

                    if ((currentTile.whichType == tileTypes.BUILDING) || ((currentTile.whichType == tileTypes.ROAD) && (currentTile.whichRoadType == roadTypes.CROSS_HORIZONTAL)))
                    {
                        posY++;
                        nextPos = new Vector3((posX - (width / 2)) * 2 - 0.5f, 1.5f, ((posY - (height / 2)) * -1) * 2);
                    }
                    else
                    {
                        myDirection = directions.NORTH;
                    }
                    break;

                case directions.WEST:
                    currentTile = myWorldScript.getTileXY(posX - 1, posY);

                    if ((currentTile.whichType == tileTypes.BUILDING) || ((currentTile.whichType == tileTypes.ROAD) && (currentTile.whichRoadType == roadTypes.CROSS_VERTICAL)))
                    {
                        posX--;
                        nextPos = new Vector3((posX - (width / 2)) * 2, 1.5f, ((posY - (height / 2)) * -1) * 2 + 0.5f);
                    }
                    else
                    {
                        myDirection = directions.EAST;
                    }
                    break;

                case directions.EAST:
                    currentTile = myWorldScript.getTileXY(posX + 1, posY);

                    if ((currentTile.whichType == tileTypes.BUILDING) || ((currentTile.whichType == tileTypes.ROAD) && (currentTile.whichRoadType == roadTypes.CROSS_VERTICAL)))
                    {
                        posX++;
                        nextPos = new Vector3((posX - (width / 2)) * 2, 1.5f, ((posY - (height / 2)) * -1) * 2 - 0.5f);
                    }
                    else
                    {
                        myDirection = directions.WEST;
                    }
                    break;

            }
        }

        currentPos = transform.localPosition;

        randomSpeed = Random.Range(-0.015f, +0.015f);

    }


    void Update()
    {

        transform.localPosition = Vector3.Lerp(currentPos, nextPos, tFactor);

        tFactor += increasePosition(posX, posY, myDirection, id);// 0.1f;

        if (tFactor >= 1f)//arrives to the next tile
        {

            tFactor = 0f;

            currentPos = nextPos;

            setNewDirection(myDirection);

        }

    }

}
