using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;
using System.Text;



public class UDPTCPReceive : MonoBehaviour
{
    private Thread receiveThread; // 데이터 수신 스레드

    private TcpClient tcpClient; // TCP 클라이언트
    private UdpClient udpClient; // UDP 클라이언트 

    private NetworkStream stream; // 서버로부터 데이터 읽기 위한 네트워크 스트림

    private Texture2D texture; // 수신 이미지 표시 텍스쳐
    private byte[] receivedImageBytes; // 수신 이미지 저장 버퍼
    private bool newDataReceived = false; // 새로운 데이터 수신 유무 플래그

    public string serverIP = "127.0.0.1"; // 서버 IP 주소
    public int tcpServerPort = 8080; // TCP 포트 번호
    public int udpServerPort = 5052; // UDP 포트 번호

    private bool startReceiving = true; // 데이터 수신 여부 제어 플래그

    private object lockObject = new object(); // 스레드 동기화 위한 잠금 객체

    // UI 오브젝트
    public GameObject playWorkout; 
    public GameObject playTurtleNeck;

    public string data; // 데이터 저장 변수
    public int armCounter; // 횟수 저장 변수_arm
    public int turtleCounter; // 횟수 저장 변수_turtle
    public bool printToConsole = false; // 데이터 콘솔 출력 여부

    void Start()
    {
        // 텍스처 초기화
        texture = new Texture2D(2, 2);

        // 수신 스레드 생성
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        try
        {
            // 서버 연결
            tcpClient = new TcpClient(serverIP, tcpServerPort);
            stream = tcpClient.GetStream();
            byte[] lengthBuffer = new byte[8]; // 들어오는 데이터 길이 저장 버퍼

            udpClient = new UdpClient(udpServerPort);

            while (startReceiving)
            {
                try
                {
                    // TCP 데이터 수신
                    int bytesRead = stream.Read(lengthBuffer, 0, lengthBuffer.Length);
                    if (bytesRead == 0) break; // 데이터 읽히지 않으면 루프 종료

                    long dataLength = BitConverter.ToInt64(lengthBuffer, 0);  // 길이 버퍼를 long으로 변환
                    byte[] dataBuffer = new byte[dataLength]; // 들어오는 데이터 저장 버퍼 생성
                    int totalRead = 0;

                    // 버퍼에 데이터 읽어오기
                    while (totalRead < dataLength)
                    {
                        bytesRead = stream.Read(dataBuffer, totalRead, dataBuffer.Length - totalRead);
                        if (bytesRead == 0) break; // 데이터 읽히지 않으면 루프 종료
                        totalRead += bytesRead; // 총 읽은 바이트 수 업데이트
                    }

                    // 객체 잠그고 수신된 이미지 바이트 업데이트
                    lock (lockObject)
                    {
                        receivedImageBytes = dataBuffer;
                        newDataReceived = true; // 새로운 데이터 수신되었음을 나타내는 플래그 설정
                    }

                    // UDP 데이터 수신
                    IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] dataByte = udpClient.Receive(ref anyIP);
                    //data = Encoding.UTF8.GetString(dataByte);

                    // 데이터 값 파싱: "armCounter,turtleNeckCount"
                    string receivedData = Encoding.UTF8.GetString(dataByte);
                    string[] splitData = receivedData.Split(',');

                    if (splitData.Length == 2)
                    {
                        // 각 값을 분리하여 각각의 카운터 값에 할당
                        lock (lockObject)
                        {
                            armCounter = int.Parse(splitData[0]);  // armCounter
                            turtleCounter = int.Parse(splitData[1]);  // turtleNeckCount
                        }
                    }

                    if (printToConsole) 
                    {
                        print("Arm Counter: " + armCounter);
                        print("Turtle Neck Counter: " + turtleCounter);
                    }

                }
                catch (Exception e)
                {
                    Debug.LogError("ReceiveData에서 오류 발생: " + e.ToString());
                    startReceiving = false; // 오류 발생 시 데이터 수신 중지
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ReceiveData 초기화에서 오류 발생: " + e.ToString());
            startReceiving = false; // 오류 발생 시 데이터 수신 중지
        }

        finally
        {
            // 리소스 정리
            if (stream != null) stream.Close();
            if (tcpClient != null) tcpClient.Close();
            if (udpClient != null) udpClient.Close();
        }
    }


    void Update()
    {
        // 새로운 데이터 수신되면 텍스처를 새로운 이미지로 업데이트
        if (newDataReceived)
        {
            lock (lockObject)
            {
                texture.LoadImage(receivedImageBytes); // 수신된 이미지 바이트를 텍스처에 로드
                texture.Apply();
                newDataReceived = false; // 플래그 초기화
            }
            UpdateUIImage();
        }
    }


    private void UpdateUIImage()
    {
        // playWorkout 오브젝트가 유효한지 확인
        if (playWorkout.activeSelf)
        {
            // UI 오브젝트에서 Image 컴포넌트 가져오기
            Image imageComponent = playWorkout.GetComponent<Image>();
            if (imageComponent != null)
            {
                // 텍스처를 적용하기 전에 Apply() 호출
                texture.Apply();

                // 텍스처를 스프라이트로 변환하여 UI 이미지에 적용
                Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                imageComponent.sprite = newSprite;
            }
            else
            {
                Debug.LogError("playWorkout 오브젝트에 Image 컴포넌트가 없습니다.");
            }
        }


        // playTurtleNeck 오브젝트가 유효한지 확인
        else if (playTurtleNeck.activeSelf)
        {
            // UI 오브젝트에서 Image 컴포넌트 가져오기
            Image imageComponent = playTurtleNeck.GetComponent<Image>();
            if (imageComponent != null)
            {
                // 텍스처를 적용하기 전에 Apply() 호출
                texture.Apply();

                // 텍스처를 스프라이트로 변환하여 UI 이미지에 적용
                Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                imageComponent.sprite = newSprite;
            }
            else
            {
                Debug.LogError("playTurtleNeck 오브젝트에 Image 컴포넌트가 없습니다.");
            }
        }


    }

    void OnApplicationQuit()
    {
        // 애플리케이션 종료 시 데이터 수신 중지하고 리소스 정리
        startReceiving = false;
        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Join(); // 수신 스레드 종료될 때까지 대기
        if (stream != null) stream.Close(); // 네트워크 스트림 닫기
        if (tcpClient != null) tcpClient.Close(); // TCP 클라이언트 닫기
        if (udpClient != null) udpClient.Close(); // UDP 클라이언트 닫기
    }
}