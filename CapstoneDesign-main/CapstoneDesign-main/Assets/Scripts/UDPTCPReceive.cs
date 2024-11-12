using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;
using System.Text;


public class UDPTCPReceive : MonoBehaviour
{
    private Thread receiveThread; // ������ ���� ������

    private TcpClient tcpClient; // TCP Ŭ���̾�Ʈ
    private UdpClient udpClient; // UDP Ŭ���̾�Ʈ 

    private NetworkStream stream; // �����κ��� ������ �б� ���� ��Ʈ��ũ ��Ʈ��

    private Texture2D texture; // ���� �̹��� ǥ�� �ؽ���
    private byte[] receivedImageBytes; // ���� �̹��� ���� ����
    private bool newDataReceived = false; // ���ο� ������ ���� ���� �÷���

    public string serverIP = "127.0.0.1"; // ���� IP �ּ�
    public int tcpServerPort = 8080; // TCP ��Ʈ ��ȣ
    public int udpServerPort = 5052; // UDP ��Ʈ ��ȣ

    private bool startReceiving = true; // ������ ���� ���� ���� �÷���

    private object lockObject = new object(); // ������ ����ȭ ���� ��� ��ü

    // UI ������Ʈ
    public GameObject playWorkout; 
    public GameObject playTurtleNeck;

    public string data; // ������ ���� ����
    public int counter; // Ƚ�� ���� ����
    public bool printToConsole = false; // ������ �ܼ� ��� ����

    void Start()
    {
        // �ؽ�ó �ʱ�ȭ
        texture = new Texture2D(2, 2);

        // ���� ������ ����
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        try
        {
            // ���� ����
            tcpClient = new TcpClient(serverIP, tcpServerPort);
            stream = tcpClient.GetStream();
            byte[] lengthBuffer = new byte[8]; // ������ ������ ���� ���� ����

            udpClient = new UdpClient(udpServerPort);

            while (startReceiving)
            {
                try
                {
                    // TCP ������ ����
                    int bytesRead = stream.Read(lengthBuffer, 0, lengthBuffer.Length);
                    if (bytesRead == 0) break; // ������ ������ ������ ���� ����

                    long dataLength = BitConverter.ToInt64(lengthBuffer, 0);  // ���� ���۸� long���� ��ȯ
                    byte[] dataBuffer = new byte[dataLength]; // ������ ������ ���� ���� ����
                    int totalRead = 0;

                    // ���ۿ� ������ �о����
                    while (totalRead < dataLength)
                    {
                        bytesRead = stream.Read(dataBuffer, totalRead, dataBuffer.Length - totalRead);
                        if (bytesRead == 0) break; // ������ ������ ������ ���� ����
                        totalRead += bytesRead; // �� ���� ����Ʈ �� ������Ʈ
                    }

                    // ��ü ��װ� ���ŵ� �̹��� ����Ʈ ������Ʈ
                    lock (lockObject)
                    {
                        receivedImageBytes = dataBuffer;
                        newDataReceived = true; // ���ο� ������ ���ŵǾ����� ��Ÿ���� �÷��� ����
                    }

                    // UDP ������ ����
                    IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] dataByte = udpClient.Receive(ref anyIP);
                    //data = Encoding.UTF8.GetString(dataByte);

                    lock (lockObject)
                    {
                        counter = int.Parse(Encoding.UTF8.GetString(dataByte)); // UDP�� ���۵� counter �� ������Ʈ
                    }

                    if (printToConsole) { print(counter); }

                }
                catch (Exception e)
                {
                    Debug.LogError("ReceiveData���� ���� �߻�: " + e.ToString());
                    startReceiving = false; // ���� �߻� �� ������ ���� ����
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ReceiveData �ʱ�ȭ���� ���� �߻�: " + e.ToString());
            startReceiving = false; // ���� �߻� �� ������ ���� ����
        }

        finally
        {
            // ���ҽ� ����
            if (stream != null) stream.Close();
            if (tcpClient != null) tcpClient.Close();
            if (udpClient != null) udpClient.Close();
        }
    }


    void Update()
    {
        // ���ο� ������ ���ŵǸ� �ؽ�ó�� ���ο� �̹����� ������Ʈ
        if (newDataReceived)
        {
            lock (lockObject)
            {
                texture.LoadImage(receivedImageBytes); // ���ŵ� �̹��� ����Ʈ�� �ؽ�ó�� �ε�
                texture.Apply();
                newDataReceived = false; // �÷��� �ʱ�ȭ
            }
            UpdateUIImage();
        }
    }


    private void UpdateUIImage()
    {
        // playWorkout ������Ʈ�� ��ȿ���� Ȯ��
        if (playWorkout.activeSelf)
        {
            // UI ������Ʈ���� Image ������Ʈ ��������
            Image imageComponent = playWorkout.GetComponent<Image>();
            if (imageComponent != null)
            {
                // �ؽ�ó�� �����ϱ� ���� Apply() ȣ��
                texture.Apply();

                // �ؽ�ó�� ��������Ʈ�� ��ȯ�Ͽ� UI �̹����� ����
                Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                imageComponent.sprite = newSprite;
            }
            else
            {
                Debug.LogError("playWorkout ������Ʈ�� Image ������Ʈ�� �����ϴ�.");
            }
        }


        // playTurtleNeck ������Ʈ�� ��ȿ���� Ȯ��
        else if (playTurtleNeck.activeSelf)
        {
            // UI ������Ʈ���� Image ������Ʈ ��������
            Image imageComponent = playTurtleNeck.GetComponent<Image>();
            if (imageComponent != null)
            {
                // �ؽ�ó�� �����ϱ� ���� Apply() ȣ��
                texture.Apply();

                // �ؽ�ó�� ��������Ʈ�� ��ȯ�Ͽ� UI �̹����� ����
                Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                imageComponent.sprite = newSprite;
            }
            else
            {
                Debug.LogError("playTurtleNeck ������Ʈ�� Image ������Ʈ�� �����ϴ�.");
            }
        }


    }

    void OnApplicationQuit()
    {
        // ���ø����̼� ���� �� ������ ���� �����ϰ� ���ҽ� ����
        startReceiving = false;
        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Join(); // ���� ������ ����� ������ ���
        if (stream != null) stream.Close(); // ��Ʈ��ũ ��Ʈ�� �ݱ�
        if (tcpClient != null) tcpClient.Close(); // TCP Ŭ���̾�Ʈ �ݱ�
        if (udpClient != null) udpClient.Close(); // UDP Ŭ���̾�Ʈ �ݱ�
    }
}