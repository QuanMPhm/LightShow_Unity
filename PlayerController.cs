using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject dashLine;
    public Rigidbody2D m_rigidbody;
    public float playerSpeed = 2;
    public float dashDelay = 2;
    public float dashTimer = 0f;
    public float dashDistance = 3;
    public float xinput;
    public float yinput;
    public int lives = 3;
    public float invulnTime = 3;
    public float invulnFlashSpeed = 0.5f;  //Time between flashes in seconds

    [SerializeField] bool isInvuln;
    [SerializeField] bool isGodMode; //Immortality
    [SerializeField] float xUpperBound;
    [SerializeField] float yUpperBound;
    [SerializeField] float xLowerBound;
    [SerializeField] float yLowerBound;
    [SerializeField] bool isMoving;

    Vector2 pos;
    Vector2 movement;

    private void Start()
    {
        Camera cam = Camera.main;
        yUpperBound = cam.orthographicSize;
        yLowerBound = -yUpperBound;
        xUpperBound = yUpperBound * cam.aspect;
        xLowerBound = -xUpperBound;
    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.position;
        xinput = Input.GetAxisRaw("Horizontal");
        yinput = Input.GetAxisRaw("Vertical");
        if (xinput != 0 || yinput != 0) isMoving = true;
        else isMoving = false;
        if (Input.GetKeyDown("space") && dashTimer <= 0 && isMoving) Dash();
        movement = new Vector2(xinput, yinput);
    }

    private void FixedUpdate()
    {
        if (dashTimer > 0) dashTimer -= Time.fixedDeltaTime;
        if (pos.x > xUpperBound || pos.x < xLowerBound || pos.y > yUpperBound || pos.y < yLowerBound) m_rigidbody.velocity = new Vector2(0, 0);
        else
        {
            m_rigidbody.velocity = movement * playerSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Obstacle" && !isInvuln && !isGodMode)
        {
            Debug.Log("Hit by: " + collision.gameObject);
            lives--;
            StartCoroutine("TempInvuln");

        }
    }

    // To play fading animation when player is damageds
    IEnumerator TempInvuln()
    {
        isInvuln = true;

        int flashCount = (int) (invulnTime / invulnFlashSpeed);
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        for (int i = 0; i < flashCount; i++)
        {
            Color c = renderer.color;
            if (c.a == 1) c.a = 0;
            else c.a = 1;
            renderer.color = c;
            yield return new WaitForSeconds(invulnFlashSpeed);
        }

        //Always return to full alpha
        Color c2 = renderer.color;
        c2.a = 1;
        renderer.color = c2;
        isInvuln = false;
    }

    private void Dash()
    {

        //Base on direction of travel, dash in that direction...
        double normFact = Math.Pow((Math.Pow(xinput, 2) + Math.Pow(yinput, 2)), 0.5);
        Vector3 tempVec = new Vector3(xinput/(float) normFact, yinput/(float) normFact, 0);
        tempVec *= dashDistance;

        Vector2 oldPos = transform.position;
        transform.position += tempVec;
        Vector2 newPos = transform.position;
        Vector2 dashPos = (oldPos + newPos) / 2;

        //Place Dash Line in front of player in direction of movement
        GameObject dashLineCopy = Instantiate(dashLine);
        dashLineCopy.transform.position = dashPos;
        float zRotate = 0;
        if ((xinput == 1 && yinput == 1) || (xinput == -1 && yinput == -1)) zRotate = -45;
        else if ((xinput == -1 && yinput == 1) || (xinput == 1 && yinput == -1)) zRotate = 45;
        else if (xinput != 0 && yinput == 0) zRotate = 90;
        dashLineCopy.transform.Rotate(0, 0, zRotate);

        dashTimer = dashDelay;
    }
}
