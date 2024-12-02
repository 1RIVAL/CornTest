import cv2
import mediapipe as mp
import numpy as np
import math
import socket
import struct

# Mediapipe 설정
mp_drawing = mp.solutions.drawing_utils
mp_holistic = mp.solutions.holistic

# 네트워크 설정 (TCP/UDP)
tcp_server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
tcp_server_socket.bind(("0.0.0.0", 8080))
tcp_server_socket.listen(0)
print("TCP port 8080...")

udp_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
udp_serverAddressPort = ("127.0.0.1", 5052)
print("UDP port 5052...")

# TCP 클라이언트 연결 수락
tcp_client_socket, tcp_client_address = tcp_server_socket.accept()
print(f"TCP Connection from: {tcp_client_address}")

# 계산 변수
arm_counter, turtle_counter = 0, 0  # 각 카운터 초기화
stage = None  # 팔 운동 단계 (DOWN 또는 UP)
turtle_neck_count = 0  # 거북목 카운트 초기화

# 팔 운동 각도 계산 함수
def calculate_angle(a, b, c):
    a = np.array(a)  # 첫 번째 점
    b = np.array(b)  # 중간 점
    c = np.array(c)  # 끝 점

    radians = np.arctan2(c[1] - b[1], c[0] - b[0]) - np.arctan2(a[1] - b[1], a[0] - b[0])
    angle = np.abs(radians * 180.0 / np.pi)

    if angle > 180.0:
        angle = 360 - angle
    return angle


# 패킷을 나누어 보내는 함수
def send_packet_in_chunks(socket, packet, chunk_size=1024):
    total_len = len(packet)
    for i in range(0, total_len, chunk_size):
        chunk = packet[i:i + chunk_size]
        socket.sendall(chunk)


# Mediapipe Holistic 모델 인스턴스 생성
with mp_holistic.Holistic(min_detection_confidence=0.5, min_tracking_confidence=0.5) as holistic:
    cap = cv2.VideoCapture(0)
    if not cap.isOpened():
        print("카메라 열기 실패")
        exit()

    cap.set(3, 1280)
    cap.set(4, 720)

    try:
        while cap.isOpened():

            ret, frame = cap.read()
            if not ret:
                break

            # 이미지를 RGB로 변환
            image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            image.flags.writeable = False

            # Holistic 모델로 처리
            results = holistic.process(image)

            # 다시 BGR로 변환
            image.flags.writeable = True
            image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

            # 랜드마크 추출 및 팔 운동, 거북목 인식 처리
            try:

                # 팔 운동 감지
                if results.pose_landmarks is not None:
                    landmarks = results.pose_landmarks.landmark
                    shoulder = [landmarks[mp_holistic.PoseLandmark.LEFT_SHOULDER.value].x,
                                landmarks[mp_holistic.PoseLandmark.LEFT_SHOULDER.value].y]
                    elbow = [landmarks[mp_holistic.PoseLandmark.LEFT_ELBOW.value].x,
                             landmarks[mp_holistic.PoseLandmark.LEFT_ELBOW.value].y]
                    wrist = [landmarks[mp_holistic.PoseLandmark.LEFT_WRIST.value].x,
                             landmarks[mp_holistic.PoseLandmark.LEFT_WRIST.value].y]

                    angle = calculate_angle(shoulder, elbow, wrist)

                    if angle > 160:
                        stage = "down"
                    if angle < 30 and stage == "down":
                        stage = "up"
                        arm_counter += 1
                        print(f"Arm counter: {arm_counter}")

                    udp_socket.sendto(f"{arm_counter},{turtle_neck_count}".encode(), udp_serverAddressPort)


                    cv2.rectangle(image, (0, 0), (280, 73), (245, 117, 16), -1)

                    # Rep 데이터 표시
                    cv2.putText(image, 'count', (15, 12), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 0, 0), 1, cv2.LINE_AA)
                    cv2.putText(image, str(arm_counter), (10, 60), cv2.FONT_HERSHEY_SIMPLEX, 2, (255, 255, 255), 2, cv2.LINE_AA)

                    # Stage 데이터 표시
                    cv2.putText(image, 'stage', (115, 12), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 0, 0), 1, cv2.LINE_AA)
                    cv2.putText(image, stage, (110, 60), cv2.FONT_HERSHEY_SIMPLEX, 2, (255, 255, 255), 2, cv2.LINE_AA)

                    # 랜드마크 연결 그리기
                    mp_drawing.draw_landmarks(image, results.pose_landmarks, mp_holistic.POSE_CONNECTIONS,
                                              mp_drawing.DrawingSpec(color=(245, 117, 66), thickness=2, circle_radius=2),
                                              mp_drawing.DrawingSpec(color=(245, 66, 230), thickness=2, circle_radius=2))

                # 거북목 감지
                if results.pose_landmarks is not None and results.face_landmarks is not None:
                    pose_landmarks = results.pose_landmarks.landmark
                    face_landmarks = results.face_landmarks.landmark

                    # 어깨 중심 좌표 계산
                    left_shoulder = pose_landmarks[mp_holistic.PoseLandmark.LEFT_SHOULDER.value]
                    right_shoulder = pose_landmarks[mp_holistic.PoseLandmark.RIGHT_SHOULDER.value]
                    center_shoulder_x = (left_shoulder.x + right_shoulder.x) / 2
                    center_shoulder_y = (left_shoulder.y + right_shoulder.y) / 2
                    center_shoulder_z = (left_shoulder.z + right_shoulder.z) / 2

                    cv2.circle(image,
                               (int(center_shoulder_x * image.shape[1]), int(center_shoulder_y * image.shape[0])), 15,
                               (255, 0, 255), cv2.FILLED)


                    # 턱 좌표 계산
                    chin = face_landmarks[152]

                    cv2.circle(image, (int(chin.x * image.shape[1]), int(chin.y * image.shape[0])), 15, (255, 0, 255),
                               cv2.FILLED)


                    # 목 길이 계산
                    length = math.hypot(int(chin.x * image.shape[1]) - int(center_shoulder_x * image.shape[1]),
                                        int(chin.y * image.shape[0]) - int(center_shoulder_y * image.shape[0]))

                    cv2.line(image, (int(chin.x * image.shape[1]), int(chin.y * image.shape[0])),
                             (int(center_shoulder_x * image.shape[1]), int(center_shoulder_y * image.shape[0])),
                             (255, 0, 255), 3)


                    # 카메라와의 거리 측정
                    pose_depth = abs(500 - (center_shoulder_z * 1000))

                    # 거북목 감지 임계치
                    turtleneck_detect_threshold = max(0, math.log2(pose_depth + 1)) * 10

                    # 목길이, 임계치, 노트북과의 거리
                    # print("Length : {:.3f},   Threshold : {:.3f},   Pose_depth : {:.3f}".format(length,
                    #                                                                             turtleneck_detect_threshold,
                    #                                                                             pose_depth))

                    # 거북목 인식
                    if length < turtleneck_detect_threshold:
                        turtle_neck_count += 1
                    if length < turtleneck_detect_threshold and turtle_neck_count > 100:
                        print("WARNING: Turtle neck detected!")
                        cv2.putText(image, 'WARNING', (640, 100), cv2.FONT_HERSHEY_SIMPLEX, 3, (0, 0, 255), 3, cv2.LINE_AA)

                        # 카운트 초기화
                        turtle_neck_count = 0
                        turtle_counter += 1
                        print(f"Turtle neck counter: {turtle_counter}")

                    udp_socket.sendto(f"{arm_counter},{turtle_neck_count}".encode(), udp_serverAddressPort)


            except Exception as e:
                print("Error detecting landmarks:", e)
                pass

            # 이미지를 JPEG로 인코딩
            _, buffer = cv2.imencode('.jpg', image)
            frame_data = buffer.tobytes()

            # 데이터 패킷 생성
            packet = struct.pack("Q", len(frame_data)) + frame_data

            send_packet_in_chunks(tcp_client_socket, packet)

            # 'q' 키가 눌리면 종료
            if cv2.waitKey(10) & 0xFF == ord('q'):
                break

    except KeyboardInterrupt:
        print("프로그램 종료")

    finally:
        # 리소스 해제
        cap.release()
        tcp_client_socket.close()
        tcp_server_socket.close()
        udp_socket.close()
        cv2.destroyAllWindows()