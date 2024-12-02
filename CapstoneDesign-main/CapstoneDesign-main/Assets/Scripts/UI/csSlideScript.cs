using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class csSlideScript : MonoBehaviour
{
    public Slider slider; // �����̴�
    public UDPTCPReceive udptcpReceive; 
    public GameObject popcorn;
    public GameObject corn;
    public Button backTurtleNeck; // playTurtleNeck�� Back ��ư
    public Button backWorkout; // playWorkout�� Back ��ư
    public GameObject playTurtleNeck; // TurtleNeck ȭ��
    public GameObject playWorkout; // Workout ȭ��
    public bool good = false; // ���� ���� ����
    public int aimCounter; // ��ǥ Ƚ�� ����

    private GameObject activeScreen; // ���� Ȱ��ȭ�� ȭ��
    private GameObject deactiveScreen; // ���� ��Ȱ��ȭ�� ȭ��

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
        // ��ǥ �޼� X
        if (!good && udptcpReceive.turtleCounter < aimCounter)
        {
            slider.value += slider.maxValue / aimCounter;
            if (slider.value >= slider.maxValue)
            {
                popcorn.SetActive(true);
                corn.SetActive(false);
            }
        }

        // ��ǥ �޼� O
        else if (!good && udptcpReceive.turtleCounter >= aimCounter)
        {

            slider.value -= slider.maxValue / aimCounter;
            if (slider.value <= slider.minValue)
            {
                good = true;
            }
        }

        // ȭ�� ��ȯ
        SwitchScreen();
    }


    public void OnBackButtonClickWorkout()
    {
        // ��ǥ �޼� X
        if (!good && udptcpReceive.armCounter < aimCounter)
        {
            slider.value += slider.maxValue / aimCounter;
            if (slider.value >= slider.maxValue)
            {
                popcorn.SetActive(true);
                corn.SetActive(false);
            }
        }
        // ��ǥ �޼� O
        else if (!good && udptcpReceive.armCounter >= aimCounter)
        {
            slider.value -= slider.maxValue / aimCounter;
            if (slider.value <= slider.minValue)
            {
                good = true;
            }
        }

        // ȭ�� ��ȯ
        SwitchScreen();
    }





    // ȭ�� ��ȯ �Լ�
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
