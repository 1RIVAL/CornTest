import cv2
import mediapipe as mp
import math
import socket
import struct

mp_drawing = mp.solutions.drawing_utils
mp_holistic = mp.solutions.holistic

cap = cv2.VideoCapture(0)
cap.set(3, 1280)
cap.set(4, 720)


# 네트워크 설정 (TCP)
tcp_server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
tcp_server_socket.bind(("0.0.0.0", 8080))
tcp_server_socket.listen(0)
print("TCP port 8080...")

# UDP 클라이언트 설정
udp_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
udp_serverAddressPort = ("127.0.0.1", 5052)
print("UDP port 5052...")

# 클라이언트 연결 대기
tcp_client_socket, tcp_client_address = tcp_server_socket.accept()
print(f"TCP Connection from: {tcp_client_address}")


# 계산 변수
turtle_counter = 0 # 거북목 측정 횟수

###################################################
sensitivity = 10
###################################################

# turtle_neck_count 변수 초기 세팅
turtle_neck_count = 0



# Setup mediapipe instance
with mp_holistic.Holistic(min_detection_confidence=0.5,min_tracking_confidence=0.5) as holistic:

    while cap.isOpened():
        ret, frame = cap.read()
        if not ret:
            break

        # Recolor image to RGB
        image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        image.flags.writeable = False

        # Make detection
        results = holistic.process(image)

        # Recolor back to BGR
        image.flags.writeable = True
        image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

        # Extract Landmarks
        try:
            if results.pose_landmarks and results.face_landmarks:
                pose_landmarks = results.pose_landmarks.landmark
                face_landmarks = results.face_landmarks.landmark

                # 양 어깨 좌표(11번, 12번)의 중심 좌표 구하기
                left_shoulder = pose_landmarks[mp_holistic.PoseLandmark.LEFT_SHOULDER.value]
                right_shoulder = pose_landmarks[mp_holistic.PoseLandmark.RIGHT_SHOULDER.value]

                center_shoulder_x = (left_shoulder.x + right_shoulder.x) / 2
                center_shoulder_y = (left_shoulder.y + right_shoulder.y) / 2
                center_shoulder_z = (left_shoulder.z + right_shoulder.z) / 2

                cv2.circle(image, (int(center_shoulder_x * image.shape[1]), int(center_shoulder_y * image.shape[0])), 15, (255, 0, 255), cv2.FILLED)

                # 얼굴의 턱 좌표(152번) 구하기
                chin = face_landmarks[152]

                cv2.circle(image, (int(chin.x * image.shape[1]), int(chin.y * image.shape[0])), 15, (255, 0, 255), cv2.FILLED)

                # 목 길이 구하기 - 유클리드 거리 공식(피타고라스 정리 활용, 이미지 픽셀 기반 거리 좌표 계산)
                # 웹캠에서 수집한 이미지는 픽셀 단위로 구성되기에, Nomalized 좌표 계산보다 픽셀 기반 거리 계산을 진행
                length = math.hypot(int(chin.x * image.shape[1]) - int(center_shoulder_x * image.shape[1]),
                                    int(chin.y * image.shape[0]) - int(center_shoulder_y * image.shape[0]))

                # length = math.hypot(chin.x - center_shoulder_x, chin.y - center_shoulder_y)


                cv2.line(image, (int(chin.x * image.shape[1]), int(chin.y * image.shape[0])),
                         (int(center_shoulder_x * image.shape[1]), int(center_shoulder_y * image.shape[0])),
                         (255, 0, 255), 3)

                # 카메라부터의 거리 측정 (스케일링 적용)
                pose_depth = abs(500 - (center_shoulder_z * 1000))

                # 거북목 감지 임계치 설정 (pose_depth 로그 값을 통한, 카메라 거리에 따른 비선형성 유지)
                turtleneck_detect_threshold = max(0, math.log2(pose_depth + 1)) * sensitivity

                # 목길이, 임계치, 노트북과의 거리
                print("Length : {:.3f},   Threshold : {:.3f},   Pose_depth : {:.3f}".format(length,
                                                                                        turtleneck_detect_threshold,
                                                                                        pose_depth))

                # 목 길이가 임계치보다 작을 때, 거북목으로 판단
                if length < turtleneck_detect_threshold:
                    turtle_neck_count += 1

                # 100번 거북목으로 인식되면 경고 표시
                if length < turtleneck_detect_threshold and turtle_neck_count > 100:
                    print("WARNING")
                    cv2.putText(image, 'WARNING', (50, 100), cv2.FONT_HERSHEY_SIMPLEX, 3, (0, 0, 255), 3, cv2.LINE_AA)

                    # 카운트를 다시 0으로 되돌리기
                    turtle_neck_count = 0

                    # 거북목 판단 횟수 증가
                    turtle_counter += 1
                    print(turtle_counter)


                # UDP를 통해 counter 값 전송
                udp_socket.sendto(str(turtle_counter).encode(), udp_serverAddressPort)

            else:
                print("No pose or face landmarks detected")


        except Exception as e:
            print("Error detecting landmarks:", e)
            pass

        # 이미지를 JPEG로 인코딩
        _, buffer = cv2.imencode('.jpg', image)
        frame_data = buffer.tobytes()

        # 데이터 패킷 생성
        packet = struct.pack("Q", len(frame_data)) + frame_data

        # 클라이언트로 데이터 전송
        tcp_client_socket.sendall(packet)

        # image를 우리에게 보여주는 부분
        #cv2.imshow("TurtleNeck Detection", image)

        if cv2.waitKey(10) & 0xFF == ord('q'):
            break


cap.release()
tcp_client_socket.close()
tcp_server_socket.close()
udp_socket.close()
cv2.destroyAllWindows()
