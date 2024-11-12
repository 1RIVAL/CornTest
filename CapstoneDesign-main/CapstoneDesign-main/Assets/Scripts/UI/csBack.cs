using UnityEngine;

public class csBack : MonoBehaviour
{
    // ���� Ȱ��ȭ�� UI ȭ��
    public GameObject currentUIScreen;

    // ������ Ȱ��ȭ�� UI ȭ��
    public GameObject previousUIScreen;

    // Start�� ���� ���� �� ù ��° UI ȭ���� Ȱ��ȭ�մϴ�.
    void Start()
    {
       
    }

    // UI ȭ���� �����ϴ� �Լ�
    public void ShowUIScreen(GameObject newUIScreen)
    {
        // ���� ȭ���� ���� ��� ��Ȱ��ȭ
        if (currentUIScreen != null)
        {
            currentUIScreen.SetActive(false);
        }

        // ���ο� UI ȭ�� Ȱ��ȭ
        currentUIScreen = newUIScreen;
        currentUIScreen.SetActive(true);
    }

    // Back ��ư�� ������ �� ȣ��Ǵ� �Լ�
    public void OnBackButtonPressed()
    {
        // ���� UI ȭ���� ���� ���, ���� ȭ������ ���ư���
        if (previousUIScreen != null)
        {
            ShowUIScreen(previousUIScreen);
        }
    }

    // ȭ�� ��ȯ �� ���� ȭ���� ����ϴ� �Լ�
    public void SetPreviousUIScreen(GameObject uiScreen)
    {
        previousUIScreen = uiScreen;
    }
}
