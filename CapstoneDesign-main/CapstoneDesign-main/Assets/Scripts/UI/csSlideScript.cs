using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class csSlideScript : MonoBehaviour
{
    public Slider slider; // �����̴�
    public UDPTCPReceive udptcpReceive; 
    public Button back; 
    public GameObject popcorn;
    public GameObject corn;
    public bool good = false; // ���� ���� ����
    public int aimCounter; // ��ǥ Ƚ�� ����

    //int temperature;
    //int temperatureMax;


    void Awake()
    {
        //temperature = 5;
        //temperatureMax = 10;

        aimCounter = 5;
    }

    // Start is called before the first frame update
    void Start()
    {
        back.onClick.AddListener(OnButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        

        //slider.value = (float)temperature / temperatureMax;

    }

    void OnButtonClick()
    {
        Debug.Log("go back");
        // ��ǥ �޼� X
        if (!good && udptcpReceive.counter < aimCounter)
        {
            slider.value += slider.maxValue / aimCounter;
            if (slider.value >= slider.maxValue)
            {
                popcorn.SetActive(true);
                corn.SetActive(false);
            }
            
        }

        // ��ǥ �޼� O
        else if (!good && udptcpReceive.counter >= aimCounter)
        {
           
            slider.value -= slider.maxValue / aimCounter;
            if (slider.value <= slider.minValue)
            {
                good = true;
            }

        }
        
    }
}
