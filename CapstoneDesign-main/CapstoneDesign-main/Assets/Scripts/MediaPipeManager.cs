using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.IO;

public class MediaPipeManager : MonoBehaviour
{

    private static Process PythonProcess;

    // Unity 애플리케이션 작동 시, 아래 RunOnStart() 메서드 자동 호출하는 Attribute
    [RuntimeInitializeOnLoadMethod]
    private static void RunOnStart()
    {
        // 미디어파이프 실행 파이썬 경로 정의
        string pythonExePath;

        // 전치리기 지시문_조건부 컴파일 (플랫폼에 따라 적용)
#if UNITY_STANDALONE_WIN
        pythonExePath = Application.streamingAssetsPath + "/PythonApp/dist/main/main.exe";
#elif UNITY_ANDROID
            // Android용 실행 파일 경로 설정
#else
            // 다른 플랫폼용 설정
#endif

        UnityEngine.Debug.Log("Python 실행 파일 경로: " + pythonExePath);

        if (!File.Exists(pythonExePath))
        {
            UnityEngine.Debug.LogError("Python 실행 파일을 찾을 수 없습니다: " + pythonExePath);
            return;
        }

        // 외부 프로그램인 MediaPipe 적용 파이썬 실행을 위한 ProcessStartInfo 클래스: 프로세스 시작할 때 사용되는 값 집합 지정 클래스
        ProcessStartInfo PythonInfo = new ProcessStartInfo
        {
            FileName = pythonExePath,
            WindowStyle = ProcessWindowStyle.Normal,
            CreateNoWindow = false,
            //WindowStyle = ProcessWindowStyle.Hidden, // 파이썬 실행 윈도우 창 숨기기
            //CreateNoWindow = true,
            UseShellExecute = false,
            //RedirectStandardOutput = true,
            //RedirectStandardError = true
        };

        try
        {
            PythonProcess = Process.Start(PythonInfo);
            if (PythonProcess == null)
            {
                UnityEngine.Debug.LogError("Python 프로세스를 시작할 수 없습니다.");
            }
            else
            {
                UnityEngine.Debug.Log("Python 프로세스가 성공적으로 시작되었습니다.");

                //PythonProcess.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log("Python Output: " + args.Data);
                //PythonProcess.BeginOutputReadLine();

                //PythonProcess.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError("Python Error: " + args.Data);
                //PythonProcess.BeginErrorReadLine();
            }
        }

        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Python 프로세스 시작 에러: " + e.Message);
        }


        Application.quitting += () => // 이벤트 핸들러: 람다(익명) 함수 적용
        {
            if (PythonProcess != null && !PythonProcess.HasExited)
            {
                try
                {
                    PythonProcess.Kill();
                    UnityEngine.Debug.Log("Python 프로세스가 종료되었습니다.");
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogError("Python 프로세스 종료 에러: " + e.Message);
                }
            }
        };
    }
}
