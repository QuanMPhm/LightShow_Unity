using System.Collections;
using System;

//The top JSON class
[Serializable]
public class LevelJSONGen
{
    //A class which describes a JSON describing a level. This class should contain all information 
    //Relating to a level, and will be the object created by other class upon importing any 
    //level JSON files
    //Before we implement a full-fletched level editor

    // Start is called before the first frame update
    public string songName;
    public string artistName;
    public BossDescriptorJSON bossDescriptor;  //Describes basic properties of boss
    public TimeStampJSON[] timeStamps;  //Contains time stamps for attacks
    public BossPhaseJSON[] bossPhases;  //Contains boss phases
}

[Serializable]
public class BossDescriptorJSON
{
    public string movementAI;
    public float xspawn;
    public float yspawn;
}

[Serializable]
public class BossPhaseJSON
{
    public int untilHits;  //Number of hits before boss changes to next phase
    public TriggerDescriptorJSON[] triggerList;
}


[Serializable]
public class TimeStampJSON  //
{
    public float time; 
    public TriggerDescriptorJSON[] triggerList;
}

[Serializable]
public class TriggerDescriptorJSON  //Class to describe a single trigger, like A, C:Amin
{
    public string musicTrigger;  //A, A#, C:A, C:Amin, B:, etc
    public AttackDescriptorJSON[] attackList;
}

[Serializable]
public class AttackDescriptorJSON  //Class to describe a single attack.
{
    public string triggerBy;  //What is the trigger of this attacK?
    public string obstacleName;
    public string spawnType;  //Variables for determining spawn and movement. 
    public string movementType;  //For now, implement linear movement. And Normal or Random Spawn
    public float xspawn;
    public float yspawn;
    public float speed;
    public float angle;
    public float fuseTime;  //Duration of fuse in seconds
    public int quantity;  //For obstacles that spawn variable # of obstacles

}


