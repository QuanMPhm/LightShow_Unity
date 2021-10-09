using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThickBeamController : ObstacleController
{

    //First beam implementation of setSpawnPoint
    public override void setSpawnPoint(float xs, float ys)
    {
        if (spawnType == ObstacleTags.SPAWN_RANDOM)
        {
            Debug.Log("In setspawn with: " + xLowerBound + " " + yLowerBound);

            //Get random int for either vertical or horizontal
            float rotateRandom = Random.Range(-1, 1);
            float xspwn, yspwn;
            if (rotateRandom >= 0) //Vertical case
            {
                xspwn = Random.Range(xLowerBound + boundPadding, xUpperBound - boundPadding);
                yspwn = 0;
            }
            else  //Horizontal case
            {
                yspwn = Random.Range(yLowerBound + boundPadding, yUpperBound - boundPadding);
                xspwn = 0;
                gameObject.transform.Rotate(0, 0, 90);
            }

            spawnPoint = new Vector2(xspwn, yspwn);
            gameObject.transform.position = spawnPoint;
        }
        else if (spawnType == ObstacleTags.SPAWN_MANUAL)
        {
            spawnPoint = new Vector2(xs, ys);
            gameObject.transform.position = spawnPoint;
        }
        else  //If setting string is invalid, default to randomSpawn
        {
            Debug.Log("Invalid Spawn Setting");
            float xspwn = Random.Range(xLowerBound + boundPadding, xUpperBound - boundPadding);
            float yspwn = Random.Range(yLowerBound + boundPadding, yUpperBound - boundPadding);

            spawnPoint = new Vector2(xspwn, yspwn);
            gameObject.transform.position = spawnPoint;
        }
        return;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
