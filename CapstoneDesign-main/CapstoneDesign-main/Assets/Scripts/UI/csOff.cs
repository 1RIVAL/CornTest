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
        // ESC 버튼이 눌렸을 때
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitApplication();
        }
    }

    // 종료 메서드
    public void QuitApplication()
    {
        audioSource.Play();
        Application.Quit();
        // 에디터에서 테스트할 때 에디터 종료 코드
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
