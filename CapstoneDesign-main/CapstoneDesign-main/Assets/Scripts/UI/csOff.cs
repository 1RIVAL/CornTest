using UnityEngine;
using UnityEngine.UI;

public class csOff : MonoBehaviour
{
    void Update()
    {
        // ESC ��ư�� ������ ��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitApplication();
        }
    }

    // ���� �޼���
    public void QuitApplication()
    {
        Application.Quit();
        // �����Ϳ��� �׽�Ʈ�� �� ������ ���� �ڵ�
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
