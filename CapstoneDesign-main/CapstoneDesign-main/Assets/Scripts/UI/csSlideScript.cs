using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class csSlideScript : MonoBehaviour
{
    public Slider slider; // 슬라이더
    public UDPTCPReceive udptcpReceive; 
    public GameObject popcorn;
    public GameObject corn;
    public Button backTurtleNeck; // playTurtleNeck의 Back 버튼
    public Button backWorkout; // playWorkout의 Back 버튼
    public GameObject playTurtleNeck; // TurtleNeck 화면
    public GameObject playWorkout; // Workout 화면
    public bool good = false; // 성공 유무 변수
    public int aimCounter; // 목표 횟수 변수

    private GameObject activeScreen; // 현재 활성화된 화면
    private GameObject deactiveScreen; // 현재 비활성화된 화면

    void Awake()
    {
        aimCounter = 5;
    }

    // Start is called before the first frame update
    void Start()
    {
        backTurtleNeck.onClick.AddListener(OnBackButtonClickTurtleNeck);
        backWorkout.onClick.AddListener(OnBackButtonClickWorkout);
    }

    // Update is called once per frame
    void Update()
    {
        if (playTurtleNeck.activeSelf)
        {
            activeScreen = playTurtleNeck;
            deactiveScreen = playWorkout;

        }
        else if (playWorkout.activeSelf)
        {
            activeScreen = playWorkout;
            deactiveScreen = playTurtleNeck;
        }
    }

    public void OnBackButtonClickTurtleNeck()
    {
        // 목표 달성 X
        if (!good && udptcpReceive.turtleCounter < aimCounter)
        {
            slider.value += slider.maxValue / aimCounter;
            if (slider.value >= slider.maxValue)
            {
                popcorn.SetActive(true);
                corn.SetActive(false);
            }
        }

        // 목표 달성 O
        else if (!good && udptcpReceive.turtleCounter >= aimCounter)
        {

            slider.value -= slider.maxValue / aimCounter;
            if (slider.value <= slider.minValue)
            {
                good = true;
            }
        }

        // 화면 전환
        SwitchScreen();
    }


    public void OnBackButtonClickWorkout()
    {
        // 목표 달성 X
        if (!good && udptcpReceive.armCounter < aimCounter)
        {
            slider.value += slider.maxValue / aimCounter;
            if (slider.value >= slider.maxValue)
            {
                popcorn.SetActive(true);
                corn.SetActive(false);
            }
        }
        // 목표 달성 O
        else if (!good && udptcpReceive.armCounter >= aimCounter)
        {
            slider.value -= slider.maxValue / aimCounter;
            if (slider.value <= slider.minValue)
            {
                good = true;
            }
        }

        // 화면 전환
        SwitchScreen();
    }





    // 화면 전환 함수
    private void SwitchScreen()
    {
        if (activeScreen != null)
        {
            activeScreen.SetActive(false);

            Transform parent = activeScreen.transform.parent;
            if (parent != null)
            {
                foreach (Transform child in parent)
                {
                    if (child.gameObject != activeScreen)
                    {
                        child.gameObject.SetActive(true);
                        deactiveScreen.SetActive(false);
                    }
                }
                parent.gameObject.SetActive(true);
            }

            activeScreen = null;
        }
    }
}
