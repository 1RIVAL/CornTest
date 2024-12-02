using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csOffTitle : MonoBehaviour
{
    
    public GameObject title;
    public GameObject startMenu;
    public AudioSource audioSource;
    public AudioClip audioClip;

    // Start is called before the first frame update
    void Start()
    {
        if (title != null)
        {
            title.SetActive(true);
        }

        if (startMenu != null)
        {
            startMenu.SetActive(false);
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = audioClip;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsScreenTouched())
        {
            if (title.activeSelf && !startMenu.activeSelf)
            {
                title.SetActive(false);
                startMenu.SetActive(true);
                audioSource.Play();
            }
        }
    }

    bool IsScreenTouched()
    {
        // ������ ���콺 Ŭ�� �Է�
        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }

        // ����� ��ġ �Է�
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            return true;
        }

        return false;
    }
}
