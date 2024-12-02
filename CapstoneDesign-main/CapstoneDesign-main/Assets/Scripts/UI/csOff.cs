using UnityEngine;
using UnityEngine.UI;

public class csOff : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip audioClip;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = audioClip;
    }

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
        audioSource.Play();
        Application.Quit();
        // �����Ϳ��� �׽�Ʈ�� �� ������ ���� �ڵ�
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
