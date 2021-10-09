using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrapCode : MonoBehaviour
{
    public Slider slider;
    public GameObject handle;
    private Image handleImg;

    // Start is called before the first frame update
    void Start()
    {
        handleImg = handle.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
 /*       beatPosition = bc.BPosition;
        float sliderPosition;
        if (beatPosition == 0) sliderPosition = 1;
        else sliderPosition = (beatPosition) * (1f / BPB);*/

        //slider.value = sliderPosition;
        if (slider.value == 1)
        {
            handleImg.color = Color.green;
/*            ColorBlock cb = slider.colors;
            cb.normalColor = Color.green;
            slider.colors = cb;*/
        }
        else
        {
            handleImg.color = Color.red;
          /*  ColorBlock cb = slider.colors;
            cb.normalColor = Color.red;
            slider.colors = cb;*/
        }
    }
}
