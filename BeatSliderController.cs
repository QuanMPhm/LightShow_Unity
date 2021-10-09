using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatSliderController : MonoBehaviour
{
    private BossController bc;

    public Slider slider;
    public GameObject handle;
    private Image handleImg;
    //public LevelController levelController;

    [SerializeField] int beatPosition;
    [SerializeField] int BPB;
    [SerializeField] int weakBeat;


    // Start is called before the first frame update
    void Start()
    {
        handleImg = handle.GetComponent<Image>();
        bc = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossController>();
        weakBeat = bc.WeakBeat;
        BPB = bc.BPB;
        StartCoroutine("checkBeatPos");
    }

    IEnumerator checkBeatPos()
    {
        while (true)
        {
            beatPosition = bc.BPosition;
            float sliderPosition;
            if (beatPosition == 0) sliderPosition = 1;
            else sliderPosition = (beatPosition) * (1f / BPB);

            slider.value = sliderPosition;
            if (slider.value == 1) handleImg.color = Color.green;
            else handleImg.color = Color.red;

            yield return new WaitUntil(() => beatPosition != bc.BPosition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
