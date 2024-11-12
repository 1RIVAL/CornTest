using UnityEngine;

public class csBack : MonoBehaviour
{
    // 현재 활성화된 UI 화면
    public GameObject currentUIScreen;

    // 이전에 활성화된 UI 화면
    public GameObject previousUIScreen;

    // Start는 게임 시작 시 첫 번째 UI 화면을 활성화합니다.
    void Start()
    {
       
    }

    // UI 화면을 변경하는 함수
    public void ShowUIScreen(GameObject newUIScreen)
    {
        // 이전 화면이 있을 경우 비활성화
        if (currentUIScreen != null)
        {
            currentUIScreen.SetActive(false);
        }

        // 새로운 UI 화면 활성화
        currentUIScreen = newUIScreen;
        currentUIScreen.SetActive(true);
    }

    // Back 버튼을 눌렀을 때 호출되는 함수
    public void OnBackButtonPressed()
    {
        // 이전 UI 화면이 있을 경우, 이전 화면으로 돌아가기
        if (previousUIScreen != null)
        {
            ShowUIScreen(previousUIScreen);
        }
    }

    // 화면 전환 시 이전 화면을 기록하는 함수
    public void SetPreviousUIScreen(GameObject uiScreen)
    {
        previousUIScreen = uiScreen;
    }
}
