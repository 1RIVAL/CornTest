using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.IO;

public class MediaPipeManager : MonoBehaviour
{

    private static Process PythonProcess;

    // Unity ���ø����̼� �۵� ��, �Ʒ� RunOnStart() �޼��� �ڵ� ȣ���ϴ� Attribute
    [RuntimeInitializeOnLoadMethod]
    private static void RunOnStart()
    {
        // �̵�������� ���� ���̽� ��� ����
        string pythonExePath;

        // ��ġ���� ���ù�_���Ǻ� ������ (�÷����� ���� ����)
#if UNITY_STANDALONE_WIN
        pythonExePath = Application.streamingAssetsPath + "/PythonApp/dist/main/main.exe";
#elif UNITY_ANDROID
            // Android�� ���� ���� ��� ����
#else
            // �ٸ� �÷����� ����
#endif

        UnityEngine.Debug.Log("Python ���� ���� ���: " + pythonExePath);

        if (!File.Exists(pythonExePath))
        {
            UnityEngine.Debug.LogError("Python ���� ������ ã�� �� �����ϴ�: " + pythonExePath);
            return;
        }

        // �ܺ� ���α׷��� MediaPipe ���� ���̽� ������ ���� ProcessStartInfo Ŭ����: ���μ��� ������ �� ���Ǵ� �� ���� ���� Ŭ����
        ProcessStartInfo PythonInfo = new ProcessStartInfo
        {
            FileName = pythonExePath,
            WindowStyle = ProcessWindowStyle.Normal,
            CreateNoWindow = false,
            //WindowStyle = ProcessWindowStyle.Hidden, // ���̽� ���� ������ â �����
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
                UnityEngine.Debug.LogError("Python ���μ����� ������ �� �����ϴ�.");
            }
            else
            {
                UnityEngine.Debug.Log("Python ���μ����� ���������� ���۵Ǿ����ϴ�.");

                //PythonProcess.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log("Python Output: " + args.Data);
                //PythonProcess.BeginOutputReadLine();

                //PythonProcess.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError("Python Error: " + args.Data);
                //PythonProcess.BeginErrorReadLine();
            }
        }

        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Python ���μ��� ���� ����: " + e.Message);
        }


        Application.quitting += () => // �̺�Ʈ �ڵ鷯: ����(�͸�) �Լ� ����
        {
            if (PythonProcess != null && !PythonProcess.HasExited)
            {
                try
                {
                    PythonProcess.Kill();
                    UnityEngine.Debug.Log("Python ���μ����� ����Ǿ����ϴ�.");
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogError("Python ���μ��� ���� ����: " + e.Message);
                }
            }
        };
    }
}
