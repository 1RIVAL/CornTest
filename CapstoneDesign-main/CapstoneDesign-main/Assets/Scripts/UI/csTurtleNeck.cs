using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csTurtleNeck : MonoBehaviour
{
    public GameObject playTurtleNeck; // playWorkout, playTurtleNeck 한 공간으로 합치기 // back 버튼 누르면 화면 사라지도록 하기
    public UDPTCPReceive udptcpReceive;

    // Start is called before the first frame update
    void Start()
    {
        playTurtleNeck.SetActive(false);

        Button buttonTurtleNeck= GetComponent<Button>();
        buttonTurtleNeck.onClick.AddListener(OnButtonClick);

        if (udptcpReceive != null)
        {
            udptcpReceive.playTurtleNeck = playTurtleNeck;
        }

        else
        {
            Debug.LogError("TCPReceive 인스턴스가 설정 X");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnButtonClick()
    {
        Debug.Log("clicked");

        playTurtleNeck.SetActive(true);

        Transform parent = transform.parent;
        if (parent != null)
        {
            foreach (Transform child in parent)
            {
                if (child.gameObject != playTurtleNeck)
                {
                    child.gameObject.SetActive(false);
                }
            }
            parent.gameObject.SetActive(true);
        }

    }
}
