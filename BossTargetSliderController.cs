using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossTargetSliderController : MonoBehaviour
{
    private BossController bc;

    // Start is called before the first frame update
    public LevelController levelController;
    public Slider bossTargetSlider;
    public GameObject markerPrefab;
    public BossPhaseJSON[] bossPlan;
    public int hitCount = 2;
    public float[] targets;
    public float maxTarget;
    void Start()
    {
        //Obtain boss controller for hit count
        bc = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossController>();

        //Fill up target array
        bossPlan = levelController.bossPlan;
        for (int i = 0; i < bossPlan.Length; i++)
        {
            if (bossPlan[i].untilHits == -1) continue; //if -1, skip it
            targets[i] = bossPlan[i].untilHits;
        }


        RectTransform slidedRT = gameObject.GetComponent<RectTransform>();
        float maxLen = slidedRT.rect.width;
        maxTarget = targets[targets.Length - 1];  //We assume last value in the JSON is the largest value. BAD IMPLEMENTATION
        bossTargetSlider.maxValue = maxTarget;  //max Value is last value of target
        //For each target, place target at i
        foreach (float i in targets)
        {
            float distanceInBar = ((i / maxTarget) * maxLen) - maxLen/2f;
            RectTransform markerRT = Instantiate(markerPrefab).GetComponent<RectTransform>();
            if (i == maxTarget)
            {
                Image markerI = markerRT.GetComponent<Image>();
                markerI.color = Color.yellow;
            }
            markerRT.SetParent(bossTargetSlider.transform);
            markerRT.localPosition = new Vector2(distanceInBar, -15f);
            markerRT.localScale = new Vector2(0.15f, 0.15f);
        }

        StartCoroutine("checkFinalTarget");
        StartCoroutine("checkHitCount");
    }

    // Update is called once per frame
    void Update()
    {
        //bossTargetSlider.value = hitCount;
    }

    IEnumerator checkHitCount()
    {
        while (hitCount < maxTarget)
        {
            hitCount = bc.HitCount;
            bossTargetSlider.value = hitCount;
            yield return new WaitUntil(() => bc.HitCount != hitCount);
        }
    }

    IEnumerator checkFinalTarget()
    {
        yield return new WaitUntil(() => bossTargetSlider.value == maxTarget);
        ColorBlock cb = bossTargetSlider.colors;
        cb.normalColor = Color.yellow;
        bossTargetSlider.colors = cb;
    }
}
