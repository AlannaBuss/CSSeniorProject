using UnityEngine;
using System.Collections;

//The abstract keyword enables you to create classes and class members that are incomplete and must be implemented in a derived class.
public abstract class MovingObject : Object
{
    public MapManager map;                  // reference to the map
    public float moveTime = 0.1f;           //Frames it will take object to move
    public LayerMask blockingLayer;         //Layer on which collision will be checked.

    protected BoxCollider2D boxCollider;    //The BoxCollider2D component attached to this object.
    protected Rigidbody2D rb2D;             //The Rigidbody2D component attached to this object.
    private float inverseMoveTime;          //Used to make movement more efficient.


    // Sets up the initial physics stuff
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
    }

    //Move returns true if it is able to move and false if not. 
    protected virtual bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        //Calculate end position based on the direction parameters passed in when calling Move.
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        //Disable the boxCollider so that linecast doesn't hit this object's own collider.
        boxCollider.enabled = false;

        //Cast a line from start point to end point checking collision on blockingLayer.
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        //Check if anything was hit
        if (hit.transform == null)
        {
            if (MoveToTile(xDir, yDir))
            {
                transform.position = new Vector3(tileX, tileY, 0);
                rb2D.MovePosition(new Vector3(tileX, tileY, 0));
            }

            //Return true to say that Move was successful
            return true;
        }
        return false;
    }

    // Attempts to move to the tile in the given direction
    protected virtual bool MoveToTile(int xDir, int yDir)
    {
        // Move to the map to the left
        if (tileX == 0 && xDir == -1)
        {
            // There's an object on the other side blocking movement
            if (map.map[mapX - 1][mapY].ObjectAt(9, tileY))
                return false;
            // Move onto the tile next to us
            else
            {
                tileX = 9;
                mapX -= 1;
                return true;
            }
        }
        // Move to the map to the right
        else if (tileX == 9 && xDir == 1)
        {
            // There's an object on the other side blocking movement
            if (map.map[mapX + 1][mapY].ObjectAt(0, tileY))
                return false;
            // Move onto the tile next to us
            else
            {
                tileX = 0;
                mapX += 1;
                return true;
            }
        }
        // Move to the map below
        else if (tileY == 0 && yDir == -1)
        {
            // There's an object on the other side blocking movement
            if (map.map[mapX][mapY - 1].ObjectAt(tileX, 9))
                return false;
            // Move onto the tile next to us
            else
            {
                tileY = 9;
                mapY -= 1;
                return true;
            }
        }
        // Move to the map above
        else if (tileY == 9 && yDir == 1)
        {
            // There's an object on the other side blocking movement
            if (map.map[mapX][mapY + 1].ObjectAt(tileX, 0))
                return false;
            // Move onto the tile next to us
            else
            {
                tileY = 0;
                mapY += 1;
                return true;
            }
        }
        // Nothing was hit
        else
        {
            tileX += xDir;
            tileY += yDir;
            return true;
        }
    }

    //AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player).
    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        //Hit will store whatever our linecast hits when Move is called.
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        //Check if nothing was hit by linecast
        if (hit.transform == null)
            return;
        //Get a component reference to the component of type T attached to the object that was hit
        T hitComponent = hit.transform.GetComponent<T>();

        //If canMove is false and hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);
    }


    //OnCantMove will be overriden by functions in the inheriting classes.
    protected abstract void OnCantMove<T>(T component)
        where T : Component;

    //Because you can't call your parent's parent's methods
    public override void initQuest()
    {
        base.initQuest();
    }

    //Interactions for moving objects change the title
    public override void Interact()
    {
        print("You interacted with a moving object");
    }
}