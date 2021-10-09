using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

//LevelController will NEVER BE RESPONSIBLE FOR SHARING INFO TO PLAYER AND BOSS
//ALL OTHER OBJECTS MUST FIND LEVELCONTROLLER, AND GET INFO FROM ITS PUBLIC FIELDS

//Class started at beginning of level
//Spawns boss
//Controlls UI
//Recieve LevelDesign JSON and LevelData and allocates boss portion to boss
//FIGHT


public class LevelController : MonoBehaviour
{
    private string[] melData;
    private string[] chordData;
    private string[] noteData;
    private float nextBeatTime; //Time of next beat, for making the levelData, and for getting beat during game
    private BossController bc;  //For communicating to our spawned Boss
    private PlayerController pc; //For communicating to player
    private GameObject bossObj;
    private GameObject playerObj;
    private float introTime = 4.5f; //Time for artist name and song title

    BossDescriptorJSON bossDescriptor;
    TimeStampJSON[] levelPlan;
    int levelPlanIndex = 0;  //For index into levelPlan
    TriggerDescriptorJSON[] currentTriggerList;  //The current level attack list in use
    public BossPhaseJSON[] bossPlan;

    public Canvas PauseScree;
    public Canvas WinScreen;
    public Canvas LoseScreen;

    public Text songTitle;
    public Text artistTitle;
    public Text songTimer;
    public int beatPosition = 0;  //We start song at first beat of bar

    public bool enableAttack;  //For debuggin purposes
    public AudioSource musicSource;
    public GameObject playerFab;
    public GameObject bossFab;
    public Vector2 playerSpawn = new Vector2(-3, 0); //Player spawn point
    public float songTime;
    public float songDuration;
    public static List<KeyValuePair<float, string>> levelData = new List<KeyValuePair<float, string>>();
    private static List<KeyValuePair<float, AttackDescriptorJSON>> attackData;

    [SerializeField] int nextAttackIndex = 0; //Index of next attack in levelData
    [SerializeField] float nextAttackTime; //Time of next attack
    [SerializeField] float firstBeatTime;
    [SerializeField] int BPM;
    [SerializeField] int BPB;
    [SerializeField] float timePerBeat;
    [SerializeField] bool isPause = false;
    [SerializeField] bool isWin = false;
    [SerializeField] bool isLose = false;
    [SerializeField] bool isGameStop = false;
    private bool isIntroFinished = false; //Self0explain
    private bool songStarted = false; //Self0explain

    public static string chordCode = "C:"; //Append code to chords to recognize it in levelData
    public static string beatCode = "B:"; //Append code to chords to recognize it in levelData




    float errorWidth = 0.04f;
    List<KeyValuePair<float, string>> f2nTable = new List<KeyValuePair<float, string>>();



    // Start is called before the first frame update
    void Start()
    {
        //Fill f2n table
        f2nTable.Add(new KeyValuePair<float, string>(110f, "A"));
        f2nTable.Add(new KeyValuePair<float, string>(116.54f, "A#"));
        f2nTable.Add(new KeyValuePair<float, string>(123.47f, "B"));
        f2nTable.Add(new KeyValuePair<float, string>(130.81f, "C"));
        f2nTable.Add(new KeyValuePair<float, string>(138.59f, "C#"));
        f2nTable.Add(new KeyValuePair<float, string>(146.83f, "D"));
        f2nTable.Add(new KeyValuePair<float, string>(155.56f, "D#"));
        f2nTable.Add(new KeyValuePair<float, string>(164.81f, "E"));
        f2nTable.Add(new KeyValuePair<float, string>(174.61f, "F"));
        f2nTable.Add(new KeyValuePair<float, string>(185f, "F#"));
        f2nTable.Add(new KeyValuePair<float, string>(196f, "G"));
        f2nTable.Add(new KeyValuePair<float, string>(207.65f, "G#"));
        f2nTable.Add(new KeyValuePair<float, string>(220f, "A"));

        //First, get all the paths and info we'll need
        string songPath = "file://" + ButtonHandler.songPath;
        string songExt = Path.GetExtension(songPath);
        string melPath = ButtonHandler.melPath;
        string chordPath = ButtonHandler.chordPath;
        string JSONPath = ButtonHandler.JSONPath;
        BPM = ButtonHandler.BPM;
        BPB = ButtonHandler.BPB;
        timePerBeat = 60f / BPM;

        //Get AudioSource, get audio file format, and load audio clip
        //Currently only support mp3, oog, and wav
        AudioType audioType;
        if (songExt == ".mp3") audioType = AudioType.MPEG;
        else if (songExt == ".ogg") audioType = AudioType.OGGVORBIS;
        else audioType = AudioType.WAV;
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(songPath, audioType);
        www.SendWebRequest();
        while (!www.isDone) { }
        musicSource.clip = DownloadHandlerAudioClip.GetContent(www);
        songDuration = musicSource.clip.length;

        //First, read in mel and chord files. Together with BPM and BPB, fill up levelData!
        melData = File.ReadAllText(melPath).Split('\n');
        chordData = File.ReadAllText(chordPath).Split('\n');
        noteData = f2n(melData);
        //Get first beat by first chord recognized
        nextBeatTime = 0;
        for (int i = 0; i < chordData.Length; i++)
        {
            float chordTime = float.Parse(chordData[i].Split(',')[0]);
            string chordStr = chordCode + chordData[i].Split(',')[1];
            if (!chordStr.Contains("N"))
            {
                firstBeatTime = chordTime;
                nextBeatTime = chordTime;
                break;
            }
        }

        //Block to generate levelData array
        int noteIndex = 0, chIndex = 0;
        float smallest;
        int tempBeatPosition = 1;  //To determine which beat position in bar
        while (noteIndex < noteData.Length && chIndex < chordData.Length)
        {
            float melTime = float.Parse(noteData[noteIndex].Split(',')[0]);
            float chordTime = float.Parse(chordData[chIndex].Split(',')[0]);
            string melStr = noteData[noteIndex].Split(',')[1];
            string chordStr = chordCode + chordData[chIndex].Split(',')[1];

            smallest = Math.Min(nextBeatTime, Math.Min(melTime, chordTime));
            if (smallest == nextBeatTime)
            {
                levelData.Add(new KeyValuePair<float, string>(nextBeatTime, beatCode + tempBeatPosition));
                nextBeatTime += timePerBeat;
                tempBeatPosition = (tempBeatPosition + 1) % BPB;
            } else if (smallest == melTime) {
                levelData.Add(new KeyValuePair<float, string>(melTime, melStr));
                noteIndex++;
            } else
            {
                levelData.Add(new KeyValuePair<float, string>(chordTime, chordStr));
                chIndex++;
            }
        }

        //Then, read in JSON, extract levelPlan and bossPlan
        string levelJSON = File.ReadAllText(JSONPath);
        LevelJSONGen masterJSON = JsonUtility.FromJson<LevelJSONGen>(levelJSON);
        bossPlan = masterJSON.bossPhases;
        levelPlan = masterJSON.timeStamps;
        bossDescriptor = masterJSON.bossDescriptor;
        currentTriggerList = levelPlan[levelPlanIndex].triggerList;
        songTitle.text = masterJSON.songName;
        artistTitle.text = "By " + masterJSON.artistName;
        StartCoroutine("fadeSongArtistText");

        //THEN, filter for the fuse of each attack, and bring the time of attacks back based on fuse time
        attackData = trigger2attack(levelData, levelPlan);

        //Write out our array for debugging
        string testPath = @"C:\Users\Quan Minh Pham\Documents\Projects\LightShow 2021\TestLevelData\Test.txt";
        StreamWriter sw = File.AppendText(testPath);
        foreach (KeyValuePair<float, string> pair in levelData)
        {
            sw.WriteLine(pair);
        }
        sw.Close();

        //Write out our array for debugging
        string testPathFilter = @"C:\Users\Quan Minh Pham\Documents\Projects\LightShow 2021\TestLevelData\Test_Filter.txt";
        StreamWriter sw2 = File.AppendText(testPathFilter);
        foreach (KeyValuePair<float, AttackDescriptorJSON> pair in attackData)
        {
            sw2.WriteLine(pair.Key + ", " + pair.Value.obstacleName + " " + pair.Value.fuseTime);
        }

        sw2.Close();

        nextAttackTime = attackData[nextAttackIndex].Key; //Get first attack time

        //Finally, spawn player and boss, display basic UI stuff
        playerObj = Instantiate(playerFab);
        pc = (PlayerController) playerObj.GetComponent("PlayerController");

        bossObj = Instantiate(bossFab);
        bc = (BossController) bossObj.GetComponent("BossController");
        bc.setSpawn(bossDescriptor.xspawn, bossDescriptor.yspawn);
        bc.setMovement(bossDescriptor.movementAI);
        bc.bossPlan = bossPlan;
        bc.levelData = levelData;
        bc.BPB = BPB;

        //Get beat time
        nextBeatTime = firstBeatTime;
    }

    // Update is called once per frame
    void Update()
    {
        //Check Win Condition, which is 1 second before song ends
        if ((songDuration - musicSource.time) <= 1 && songStarted) isWin = true;

        //Perform Win or Lose actions
        if (isWin) onWin();
        if (isLose) onLose();

        //Check Pause condition
        if (Input.GetKeyDown("p")) onPause();
        //Check if game enter irreversible state
        if (isWin || isLose) isGameStop = true;
        //Check if player wants to quit
        if ((isWin || isLose || isPause) && Input.GetKeyDown("q")) SceneManager.LoadScene("SampleScene");
        
        //Check intro time
        if (Time.timeSinceLevelLoad > introTime) isIntroFinished = true;

        //Check game not paused
        if (isIntroFinished && !isPause && !isGameStop)
        {
            //Play song after intro finished
            if (!musicSource.isPlaying)
            {
                StartCoroutine("checkPlayerLives");
                musicSource.Play();
                songStarted = true;
            }

            //If in stop conditions
            songTime = musicSource.time;
            songTimer.text = ((int) songTime).ToString() + @"/" + (int) songDuration;

            if (songTime >= nextBeatTime)
            {
                bc.enterNextBeat();
                nextBeatTime += timePerBeat;
            }

            /*//Check if we've passed the first levelPlan
            if (songTime > levelPlan[levelPlanIndex].time)
            {
                levelPlanIndex++;
                currentTriggerList = levelPlan[levelPlanIndex].triggerList;
            }*/

            //Check for time to ATTACK!
            if ((songTime > nextAttackTime) && enableAttack && nextAttackIndex < attackData.Count - 1)
            {
                AttackDescriptorJSON AttackJSON = attackData[nextAttackIndex].Value;
                performAttack(AttackJSON);

                //Increment time
                nextAttackIndex++;
                nextAttackTime = attackData[nextAttackIndex].Key;

            }
            
        }
    }

    void performAttack(AttackDescriptorJSON AttackStr)  //Perform attack, given attackstr
    {

        
        //now identify asset, and all given fields
        //For now, assume our 5head level designer gave all the fields nessecary
        string obstacleName = AttackStr.obstacleName;
        GameObject attackObj = Resources.Load<GameObject>(obstacleName);
        GameObject go = Instantiate<GameObject>(attackObj);
        ObstacleController oc = (ObstacleController)go.GetComponent("ObstacleController");
        Debug.Log("Performing attack: " + obstacleName);
        //Get spawn setting
        oc.spawnType = AttackStr.spawnType;
        oc.movementType = AttackStr.movementType;

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

        //Get quantity
        int quant = AttackStr.quantity;
        oc.setQuantity(quant);
        return;
    }

    string[] f2n(string[] meldat)
    {
        ArrayList noteArrL = new ArrayList();
        string lastNote = "N"; //See what last note was, for note quanta purposes. N for no note
        foreach (string csvline in meldat)
        {
            string note = "I"; //I by default for ignore, if note was out of tune, but not nessecarily no melody playing
            string noteTime = csvline.Split(',')[0];
            string noteFreqStr = csvline.Split(',')[1];
            float noteFreq = float.Parse(noteFreqStr);

            //First normalize noteFreq to range between 110-220
            while (noteFreq > 220) noteFreq /= 2;

            //Check against f2n table to find note
            if (noteFreq < 0) note = "N";
            else
            {
                foreach (KeyValuePair<float, string> notePair in f2nTable)
                {
                    if (Math.Abs(noteFreq - notePair.Key) < notePair.Key * errorWidth)
                    {
                        note = notePair.Value;
                        break;
                    }
                }
            }

            //Check if note identified is same as last note and not (I)gnored 
            if (note != lastNote && note != "I")
            {
                noteArrL.Add(noteTime + "," + note);
                lastNote = note;
            }
        }

        //Second round of filtering
        //If note does not last for a threshold duration, ignore it
        ArrayList noteArrL2 = new ArrayList();
        float noteLengthMin = 0.04f;
        for (int i = 0; i < noteArrL.Count - 1; i++)
        {
            float noteTimeNow = float.Parse(((string) noteArrL[i]).Split(',')[0]); 
            float noteTimeNext = float.Parse(((string)noteArrL[i+1]).Split(',')[0]);
            if (noteTimeNext - noteTimeNow > noteLengthMin) noteArrL2.Add(noteArrL[i]);
        }
        return (string[])noteArrL2.ToArray(typeof(string));
    }

    //Function to convert time-trigger array to time-attack array
    List<KeyValuePair<float, AttackDescriptorJSON>> trigger2attack(List<KeyValuePair<float, string>> triggerDat
        , TimeStampJSON[] levelDat)
    {
        int planIndex = 0;
        float planTime = levelDat[planIndex].time;
        TriggerDescriptorJSON[] theCurrentPlan = levelDat[planIndex].triggerList;
        List<KeyValuePair<float, AttackDescriptorJSON>> attackDat = new List<KeyValuePair<float, AttackDescriptorJSON>>();
        //For each trigger in triggerDat, first, check if the time has passed our current trigger plan
        //THen given the trigger, see in currentPlan if we have attacks for that trigger
        //If We do have attacks, iterate through attackPlan of that trigger, and add attacks into the attackDat arr, 
        //Finally, sort our attackDat
        foreach (KeyValuePair<float, string> trigger in triggerDat)
        {
            float triggerTime = trigger.Key;
            string triggerStr = trigger.Value;

            //Check if we've passed the time of our currentPlan,
            if (triggerTime > planTime)
            {
                planIndex++;
                planTime = levelDat[planIndex].time;
                theCurrentPlan = levelDat[planIndex].triggerList;
            }

            foreach(TriggerDescriptorJSON triggerDes in theCurrentPlan)
            {
                if (triggerDes.musicTrigger == triggerStr)
                {
                    foreach (AttackDescriptorJSON attackDes in triggerDes.attackList)
                    {

                        //If fuse time is less than 0, ignore the attack 
                        float fuse = attackDes.fuseTime;
                        float adjustedTime = triggerTime - fuse;
                        if (adjustedTime < 0) continue;

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

    void onPause()
    {
        isPause = !isPause;
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            musicSource.Pause();
            PauseScree.enabled = true;
        }
        else
        {
            Time.timeScale = 1;
            musicSource.UnPause();
            PauseScree.enabled = false;
        }
    }

    void onWin()
    {
        Time.timeScale = 0;
        WinScreen.enabled = true;
    }
    void onLose()
    {
        Time.timeScale = 0;
        LoseScreen.enabled = true;
    }

    IEnumerator checkPlayerLives()
    {

        int lives = ((PlayerController) playerObj.GetComponent("PlayerController")).lives;
        while (lives > 0)
        {
            lives = ((PlayerController)playerObj.GetComponent("PlayerController")).lives;
            yield return new WaitForSeconds(0.20f);
        }
        isLose = true;
    }

    IEnumerator fadeSongArtistText()
    {
        yield return new WaitForSeconds(0.5f);

        for (float ft = 0f; ft < 1; ft += 0.05f)
        {
            Color ca1 = songTitle.color;
            ca1.a = ft;
            songTitle.color = ca1;

            Color ca2 = artistTitle.color;
            ca2.a = ft;
            artistTitle.color = ca2;

            yield return null;
        }

        yield return new WaitForSeconds(introTime - 1.5f);

        for (float ft = 1f; ft > 0; ft -= 0.05f)
        {
            Color ca1 = songTitle.color;
            ca1.a = ft;
            songTitle.color = ca1;

            Color ca2 = artistTitle.color;
            ca2.a = ft;
            artistTitle.color = ca2;

            yield return null;
        }

        artistTitle.enabled = false;
        songTitle.enabled = false;

    }
}
