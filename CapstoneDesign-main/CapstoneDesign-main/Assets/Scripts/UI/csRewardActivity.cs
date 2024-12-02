using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csRewardActivity : MonoBehaviour
{
    public GameObject selectRewardActivity;
    public AudioSource audioSource;
    public AudioClip audioClip;

    // Start is called before the first frame update
    void Start()
    {
        Button buttonRewardActivity = GetComponent<Button>();
        buttonRewardActivity.onClick.AddListener(OnButtonClick);

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = audioClip;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnButtonClick()
    {
        //Debug.Log("clicked");
        selectRewardActivity.SetActive(true);
        audioSource.Play();
    }
}
