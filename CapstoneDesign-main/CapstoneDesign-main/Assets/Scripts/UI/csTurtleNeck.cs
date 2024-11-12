using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csTurtleNeck : MonoBehaviour
{
    public GameObject playTurtleNeck; // playWorkout, playTurtleNeck �� �������� ��ġ�� // back ��ư ������ ȭ�� ��������� �ϱ�
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
            Debug.LogError("TCPReceive �ν��Ͻ��� ���� X");
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
