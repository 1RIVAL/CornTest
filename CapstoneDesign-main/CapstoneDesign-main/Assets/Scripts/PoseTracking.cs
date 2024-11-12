using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseTracking : MonoBehaviour
{
    public UDPTCPReceive udptcpReceive;
    public GameObject[] posePoints;
  
    void Start()
    {

    }

    void Update()
    {
        // UDP �������ݷ� ���۵� �����͸� �޾ƿ´�.
        string data = udptcpReceive.data;

        // ������ �� �����Ϳ��� ���ȣ([ ])�� ����.
        data = data.Remove(0, 1);
        data = data.Remove(data.Length - 1, 1);

        // ��ǥ�� �������� �����͸� ��Ȱ
        string[] points = data.Split(',');

        for (int i = 0; i < 21; i++)
        {
            float x = 5 - float.Parse(points[i * 3]) / 100;
            float y = float.Parse(points[i * 3 + 1]) / 100;
            float z = float.Parse(points[i * 3 + 2]) / 100;

            posePoints[i].transform.localPosition = new Vector3(x, y, z);

        }
    }

}
