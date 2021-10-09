using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawnCircleController : ObstacleController
{
    public GameObject projPrefab;


    public override void setQuantity(int quant)
    {
        Debug.Log("In setQuantity of: " + gameObject.name + " " + quant);
        Vector2 speedvec;
        Vector2 spawnvec;
        float boundReference;
        if (spawnType == ObstacleTags.SPAWN_DOWN_WALL) 
        {
            speedvec = new Vector2(0, 1);
            spawnvec = new Vector2(0, yLowerBound + 0.1f);
            boundReference = xUpperBound - boundPadding;
        } 
        else if (spawnType == ObstacleTags.SPAWN_UP_WALL)
        {
            speedvec = new Vector2(0, -1);
            spawnvec = new Vector2(0, yUpperBound - 0.1f);
            boundReference = xUpperBound - boundPadding;
        } 
        else if (spawnType == ObstacleTags.SPAWN_LEFT_WALL)
        {
            speedvec = new Vector2(1, 0);
            spawnvec = new Vector2(xLowerBound + 0.1f, 0);
            boundReference = yUpperBound - boundPadding;
        }
        else if (spawnType == ObstacleTags.SPAWN_RIGHT_WALL)
        {
            speedvec = new Vector2(-1, 0);
            spawnvec = new Vector2(xUpperBound - 0.1f, 0);
            boundReference = yUpperBound - boundPadding;
        }
        else
        {
            Debug.Log("Invalid spawn setting for: " + gameObject.name);
            Debug.Log("Default to upper wall");
            speedvec = new Vector2(0, -1);
            spawnvec = new Vector2(0, yUpperBound - 0.1f);
            boundReference = xUpperBound - boundPadding;
        }

        for (int i = 0; i < quant; i++)
        {

            Vector2 tempspawnvec = spawnvec;
            ObstacleController oc = Instantiate(projPrefab).GetComponent<ObstacleController>();
            float ran_speed = Random.Range(1.2f, 1.8f);
            float ran_coor = Random.Range(-boundReference, boundReference);
            if (tempspawnvec.x == 0) tempspawnvec.x = ran_coor;
            else tempspawnvec.y = ran_coor;

            //Set projectile's position and speed directly through RigidBody. Is this okay??
            oc.transform.position = tempspawnvec;
            oc.setVelocity(speedvec * ran_speed);
        }

        //Destroy(gameObject);
    }

    public override void setSpawnPoint(float xs, float ys)
    {
        return;
    }

    // Update is called once per frame
    void Update()
    {
    }


}
