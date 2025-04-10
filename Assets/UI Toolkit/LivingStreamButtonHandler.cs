using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LivingStreamButtonHandler : MonoBehaviour
{
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var LivingStreamButton = root.Q<Button>("LivingStreamButton");
        if (LivingStreamButton != null)
        {
            LivingStreamButton.clicked += () => SceneManager.LoadScene("Calibrate");
        }
    }
}