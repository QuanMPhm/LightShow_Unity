using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FirstCircleController : ObstacleController
{
    // Override velocity setter
    public override void setVelocity(float speedParam, float angleParam)
    {
        float angleInRad = (angleParam / 180f) * Mathf.PI;
        float xvel = (float) Math.Cos(angleInRad);
        float yvel = (float) Math.Sin(angleInRad);
        velocity = new Vector2(xvel, yvel) * speedParam;
        speed = speedParam;
        angle = angleParam;
        m_rigidbody.velocity = velocity;

    }

    public override void setSpawnPoint( float xs, float ys)
    {
        if (spawnType == ObstacleTags.SPAWN_MANUAL)
        {
            spawnPoint = new Vector2(xs, ys);
            gameObject.transform.position = spawnPoint;
        } else
        {
            base.setSpawnPoint( xs, ys);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
