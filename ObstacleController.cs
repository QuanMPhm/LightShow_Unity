using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    //Base class for projectiles

    private BossController bc;

    public bool IsDeadly; //If projectile deals damage
    public bool HasFuse; //If Projectile have fuse delay
    protected bool HasFuseBlown = false;  //Did we past fuse?
    public bool HasOOBCheck;  //Does Obstacle care about OOB?
    public bool HasRigidBody;  //Does Obstacle have RigidBody?
    public float FuseDelay;  //Fuse delay time, before dealing damage
    public float FuseAlpha = 0.3f;  //Transparency of fuse objects

    public string spawnType;
    public string movementType;

    [SerializeField] protected int quantity;
    [SerializeField] protected float speed;  //Speed of projectile, for velocity calculation
    [SerializeField] protected float angle;  //Angle of projectile, for velocity calculation
    [SerializeField] protected float fuseTimer = 0;
    [SerializeField] protected float fadeSpeed = 0.01f;
    [SerializeField] protected float xUpperBound;
    [SerializeField] protected float yUpperBound;
    [SerializeField] protected float xLowerBound;
    [SerializeField] protected float yLowerBound;
    [SerializeField] protected float boundPadding = 1;
    [SerializeField] protected string pathing;  //If pathing is linear, random, etc
    [SerializeField] protected Vector2 velocity; //Velocity of projectile
    [SerializeField] protected Vector2 spawnPoint;  //Initial spawn point

    protected Rigidbody2D m_rigidbody;  //Rigidbody of projectile
    protected SpriteRenderer sp_renderer;

    protected bool isBox;  //Is it box, or circle collider???
    protected BoxCollider2D boxCollider2D;
    protected CircleCollider2D circleCollider2D;

    public void Awake()
    {
        //Obtain bounds always
        Camera cam = Camera.main;
        yUpperBound = cam.orthographicSize + boundPadding;
        yLowerBound = -yUpperBound;
        xUpperBound = cam.orthographicSize * cam.aspect + boundPadding;
        xLowerBound = -xUpperBound;
        sp_renderer = gameObject.GetComponent<SpriteRenderer>();

        //Get boss controller,
        bc = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossController>();


        if (HasRigidBody) m_rigidbody = gameObject.GetComponent<Rigidbody2D>();

        //If object has OOB Check, DO IT
        if (HasOOBCheck) StartCoroutine("OutOfBoundCheck");

        //If object has fuse, disable collider at spawn time
        //We're ASSUMING obstacle either have boxCollider or CircleCollider
        if (HasFuse)
        {
            boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
            circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
            if (boxCollider2D == null && circleCollider2D == null) Debug.Log("Object no collider???");
            else if (boxCollider2D == null)
            {
                circleCollider2D.enabled = false;
                isBox = false;
            }
            else
            {
                boxCollider2D.enabled = false;
                isBox = true;
            }

            //Make fused object faded
            Color c = sp_renderer.material.color;
            c.a = FuseAlpha;
            sp_renderer.material.color = c;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void FixedUpdate()
    {
        //Check if obstacle is a Fuse Obstacle
        if (HasFuse)
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
                StartCoroutine("FadeAfterFuse");
            }
        }
    }

    //Default implementation for fading after obstacle WITH FUSE blows
    public virtual IEnumerator FadeAfterFuse()
    { 
        for (float ft = 1f; ft >= 0; ft -= fadeSpeed)
        {
            Color c = sp_renderer.material.color;
            c.a = ft;
            sp_renderer.material.color = c;

            //If alpha goes below certain point, disable hitbox
            if (ft <= 0.9f)
            {
                gameObject.tag = "Disabled";
                /*if (isBox) boxCollider2D.enabled = false;
                else circleCollider2D.enabled = false;*/
            }
            yield return null;
        }
        Destroy(gameObject);
    }

    //Default implementation of OOB
    public virtual IEnumerator OutOfBoundCheck()
    {
        for (; ;)
        {
            Vector2 pos = gameObject.transform.position;
            if (pos.x > xUpperBound || pos.x < xLowerBound ||
                pos.y > yUpperBound || pos.y < yLowerBound)
            {
                Destroy(gameObject);
            }
            //Wait 0.5 seconds before next OOB check
            yield return new WaitForSeconds(0.5f);
        }
    }


    //Method(s) for setting various properties of obstacles, i.e velocity, spawn point, etc
    //By default, absolutely ignore, the subclasses will override if nessecary
    public virtual void setSpawnPoint(float xs, float ys)
    {
        if (spawnType == ObstacleTags.SPAWN_ON_BOSS)
        {
            gameObject.transform.position = bc.transform.position;
        } else
        {
            Debug.Log("Invalid Spawn Setting " + spawnType + ", default to 0,0");
            spawnPoint = new Vector2(0, 0);
            gameObject.transform.position = spawnPoint;
        }
        return;
    }

    public virtual void setVelocity(float speed, float angle)
    {
        return;
    }

    //Overload of default setVelocity that's called by LevelController
    //For internal use by Spawner-type obstacles
    public virtual void setVelocity(Vector2 speedvec)
    {
        m_rigidbody.velocity = speedvec;
    }

    public virtual void setQuantity(int quant)
    {
        return;
    }
}
