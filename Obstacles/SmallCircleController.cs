using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCircleController : ObstacleController
{
    public override void setVelocity(float speedParam, float angleParam)
    {
        float angleInRad = (angleParam / 180f) * Mathf.PI;
        float xvel = (float)Mathf.Cos(angleInRad);
        float yvel = (float)Mathf.Sin(angleInRad);
        velocity = new Vector2(xvel, yvel) * speedParam;
        speed = speedParam;
        angle = angleParam;
        m_rigidbody.velocity = velocity;
    }

    public override void setSpawnPoint(float xs, float ys)
    {
        if (spawnType == ObstacleTags.SPAWN_MANUAL)
        {
            spawnPoint = new Vector2(xs, ys);
            gameObject.transform.position = spawnPoint;
        }
        else base.setSpawnPoint(xs, ys);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
