using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DashCircleController : ObstacleController
{
    public float minSpeed = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody.velocity = new Vector2(0,0);
        m_rigidbody.drag = 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void FixedUpdate()
    {
        //Check if obstacle is a Fuse Obstacle
        if (HasFuse && !HasFuseBlown)
        {
            //Add to fuse timer
            fuseTimer += Time.deltaTime;

            //If reached fuse end
            if (fuseTimer > FuseDelay)
            {
                HasFuseBlown = true;
                if (isBox) boxCollider2D.enabled = true;
                else circleCollider2D.enabled = true;
                Color c = sp_renderer.material.color;
                c.a = 1;
                sp_renderer.material.color = c;
            }
        }

        if (HasFuseBlown && m_rigidbody.velocity == new Vector2(0, 0))
        {
            Debug.Log("DASH!");
            m_rigidbody.velocity = this.velocity;
            StartCoroutine("disableDrag");
        }

        
    }

    public override void setVelocity(float speed, float angle)
    {
        float angleInRad = (angle / 180f) * Mathf.PI;
        float xvel = (float)Math.Cos(angleInRad);
        float yvel = (float)Math.Sin(angleInRad);
        velocity = new Vector2(xvel, yvel) * speed;
        this.speed = speed;
        this.angle = angle;
    }

    public override void setSpawnPoint(float xs, float ys)
    {
        if (spawnType == ObstacleTags.SPAWN_MANUAL)
        {
            spawnPoint = new Vector2(xs, ys);
            gameObject.transform.position = spawnPoint;
        } else
        {
            base.setSpawnPoint(xs, ys);
        }
        
    }

    IEnumerator disableDrag()
    {
        float curSpeed = Mathf.Sqrt(Mathf.Pow(m_rigidbody.velocity.x, 2) + Mathf.Pow(m_rigidbody.velocity.y, 2));
        while (curSpeed > minSpeed)
        {
            curSpeed = Mathf.Sqrt(Mathf.Pow(m_rigidbody.velocity.x, 2) + Mathf.Pow(m_rigidbody.velocity.y, 2));
            yield return null;
        }
        m_rigidbody.drag = 0;
    }
}
