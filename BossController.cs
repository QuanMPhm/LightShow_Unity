using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private LevelController lvlcontroller;
    private TriggerDescriptorJSON[] currentAttackPlan;  //The current attack plan being used to generate attackData
    private List<KeyValuePair<float, AttackDescriptorJSON>> attackData;
    private int bossPlanIndex = 0;

    //Controller to controll the Boss, its invincibility, attacks, timing, movement for now
    public BossPhaseJSON[] bossPlan;
    public List<KeyValuePair<float, string>> levelData; //Time-array of music events
    public bool IsInvincible;  //Is Boss Invuln
    public bool IsInvinChanged;  //Did Boss change Invuln, for animation timing
    public bool IsSongEnd;  //Did song end
    public int HitCount = 0;  //How many times did boss get hit?
    public int TargetHitCount;  //How many hits before move to next phase
    public int BPB;  //What is beat time of music, for invuln timing
    public float invulnFlashSpeed = 0.1f;
    public float songTime;

    public int BPosition = 0;  // Beat position is bar. For BPB = 4, it would go 1 2 3 0 1 2 3 0
    public int WeakBeat = 0;  //Beat on which boss is vulnerable, 0 means 4th beat
    [SerializeField] float nextAttackTime; //Time of next attack
    [SerializeField] int nextAttackIndex = 0; //Index of next attack in attackData


    string MovementAI;  //MovementAI
    string AttackAI;  //AttackAI, determined by JSON format, should be a unique datatype

    private string debugDir = @"C:\Users\Quan Minh Pham\Documents\Projects\LightShow 2021\TestLevelData";


    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //Obtain reference to LevelController
        lvlcontroller = GameObject.Find("LevelController").GetComponent<LevelController>();

        //Given boss plan, now generate the first boss attack array
        TargetHitCount = bossPlan[bossPlanIndex].untilHits;
        currentAttackPlan = bossPlan[bossPlanIndex].triggerList;
        attackData = trigger2attack(levelData, currentAttackPlan);

        StreamWriter fs = File.AppendText(debugDir + @"\boss_test_" + bossPlanIndex + ".txt");
        foreach (KeyValuePair<float, AttackDescriptorJSON> pair in attackData)
        {
            fs.WriteLine(pair.Key + ", " + pair.Value.obstacleName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "DashStrike" && !IsInvincible)
        {
            HitCount++;
            StartCoroutine("TempInvuln");
        }
    }

    // Update is called once per frame
    void Update()
    {
        songTime = lvlcontroller.songTime;

        // Priorities list
        // Check if num of time hit is greater than TargetHitCount, if so, re-generate attack plan
        // A special hitcount number is -1, indicating the last hitcount. If so, no longer check
        if (HitCount > TargetHitCount && TargetHitCount != -1)
        {
            bossPlanIndex++;
            TargetHitCount = bossPlan[bossPlanIndex].untilHits;
            currentAttackPlan = bossPlan[bossPlanIndex].triggerList;
            attackData = trigger2attack(levelData, currentAttackPlan);

            //Everytime we generate attaack plan, we can reset this to 0. I'm geinus
            nextAttackIndex = 0;
        }

        if (songTime > nextAttackTime)
        {
            AttackDescriptorJSON AttackJSON = attackData[nextAttackIndex].Value;
            performAttack(AttackJSON);

            //Increment time
            nextAttackIndex++;
            nextAttackTime = attackData[nextAttackIndex].Key;
        }
    }

    IEnumerator TempInvuln()
    {
        IsInvincible = true;
        int CurrentBPosition = BPosition;
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();

        while (CurrentBPosition == BPosition)
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
    }

    //Functino for incrementing beat
    //If we're now in the first beat of the bar, change boss sprite to green, and disable invuln
    //This is obviously not implemented correctly somehow, check back later
    public void enterNextBeat()
    {
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        BPosition = (BPosition + 1) % BPB;
        if (BPosition == WeakBeat)
        {
            
            Color c1 = Color.green;
            renderer.color = c1;
            IsInvincible = false;
            return;
        }

        //If boss does not have red color,and revert color
        if (!renderer.color.Equals(Color.red))
        {
            Color c1 = Color.red;
            renderer.color = c1;
            IsInvincible = true;
        }
    }

    public void setMovement(string movementStr)
    {
        MovementAI = movementStr;
        if (MovementAI == ObstacleTags.BOSS_MOV_STATIC)
        {
            //Boss doesn't move, noice
        } else
        {
            Debug.Log("Invalid movement AI string, default to static");
        }
    }

    public void setSpawn(float xs, float ys)
    {
        gameObject.transform.position = new Vector2(xs, ys);
    }

    void performAttack(AttackDescriptorJSON AttackStr)  //Perform attack, given attackstr
    {
        //now identify asset, and all given fields
        //For now, assume our 5head level designer gave all the fields nessecary
        string obstacleName = AttackStr.obstacleName;
        GameObject attackObj = Resources.Load<GameObject>(obstacleName);
        GameObject go = Instantiate<GameObject>(attackObj);
        ObstacleController oc = (ObstacleController)go.GetComponent("ObstacleController");

        //Get spawn setting
        string spawnType = AttackStr.spawnType;
        string movementType = AttackStr.movementType;

        float xspawn = AttackStr.xspawn;
        float yspawn = AttackStr.yspawn;
        oc.setSpawnPoint(xspawn, yspawn);

        //Get fuse time
        float fuseTime = AttackStr.fuseTime;
        oc.FuseDelay = fuseTime;

        //Get velocity
        float speed = AttackStr.speed;
        float angle = AttackStr.angle;
        oc.setVelocity(speed, angle);
        return;
    }

    //Function to convert time-trigger array to time-attack array
    //Modified so that we're assuming only one attackPlan
    List<KeyValuePair<float, AttackDescriptorJSON>> trigger2attack(List<KeyValuePair<float, string>> triggerDat
        , TriggerDescriptorJSON[] attackPlan)
    {
        List<KeyValuePair<float, AttackDescriptorJSON>> attackDat = new List<KeyValuePair<float, AttackDescriptorJSON>>();

        foreach (KeyValuePair<float, string> trigger in triggerDat)
        {
            float triggerTime = trigger.Key;
            string triggerStr = trigger.Value;

            foreach (TriggerDescriptorJSON triggerDes in attackPlan)
            {
                if (triggerDes.musicTrigger == triggerStr)
                {
                    foreach (AttackDescriptorJSON attackDes in triggerDes.attackList)
                    {

                        //If fuse time is less than CURRENT TIME OF MUSIC, ignore the attack 
                        float fuse = attackDes.fuseTime;
                        float adjustedTime = triggerTime - fuse;
                        if (adjustedTime < songTime) continue;

                        attackDes.triggerBy = triggerStr;
                        attackDat.Add(new KeyValuePair<float, AttackDescriptorJSON>(adjustedTime, attackDes));
                    }
                    break;
                }
            }

        }

        attackDat = attackDat.OrderBy(entry => entry.Key).ToList();
        return attackDat;
    }
}
