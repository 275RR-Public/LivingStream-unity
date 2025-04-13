using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainUIHandler : MonoBehaviour
{
    // Existing public fields (unchanged)
    public GameObject ocean;
    public Light directionalLight;
    public GameObject quadLogo;
    public GameObject planeBlack;
    public GameObject sphereRed;
    public GameObject sphereBlue;
    public float oceanLowerAmount = 1f;
    public float lightIntensityChange = 0.5f;
    public float quadMoveAmount = 2f;
    private const string calibrationText1 = "Arham Ali";
    private const string calibrationText2 = "Will Forbes";
    private const string calibrationText3 = "Bryan Ukeje";
    private const string calibrationText4 = "James Hofer";

    // Existing private fields (unchanged)
    private Vector3 originalOceanPosition;
    private float originalLightIntensity;
    private Vector3 originalQuadPosition;
    private Vector3 originalPlaneBlackPosition;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private Vector3 originalSphereRedPosition;
    private Vector3 originalSphereBluePosition;
    private bool isTideLowered = false;
    private Coroutine currentTransition;
    private AudioSource backgroundMusic;
    private float originalVolume;
    private GameObject mainCamera;

    // Private fields for text (removed target font sizes)
    private Label text1;
    private Label text2;
    private Label text3;
    private Label text4;
    private string originalText1;
    private string originalText2;
    private string originalText3;
    private string originalText4;
    private float originalText1FontSize;
    private FontStyle originalText1FontStyle;
    private float originalText2FontSize;
    private FontStyle originalText2FontStyle;
    private float originalText3FontSize;
    private FontStyle originalText3FontStyle;
    private float originalText4FontSize;
    private FontStyle originalText4FontStyle;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Store original values for existing elements (unchanged)
        if (ocean != null) originalOceanPosition = ocean.transform.position;
        if (directionalLight != null) originalLightIntensity = directionalLight.intensity;
        if (quadLogo != null) originalQuadPosition = quadLogo.transform.position;
        if (planeBlack != null) originalPlaneBlackPosition = planeBlack.transform.position;
        if (sphereRed != null) originalSphereRedPosition = sphereRed.transform.position;
        if (sphereBlue != null) originalSphereBluePosition = sphereBlue.transform.position;
        mainCamera = GameObject.Find("Main Camera");
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
            originalCameraRotation = mainCamera.transform.rotation;
            backgroundMusic = mainCamera.GetComponent<AudioSource>();
            if (backgroundMusic != null) originalVolume = backgroundMusic.volume;
        }

        // Query UI elements (unchanged)
        var toggleCalibrate = root.Q<Button>("CalibrateButton");
        var playButton = root.Q<Button>("PlayButton");
        var quitButton = root.Q<Button>("QuitButton");
        text1 = root.Q<Label>("text1");
        text2 = root.Q<Label>("text2");
        text3 = root.Q<Label>("text3");
        text4 = root.Q<Label>("text4");

        // Store original text content and styles, ensure initial visibility
        if (text1 != null)
        {
            originalText1 = text1.text;
            originalText1FontSize = text1.resolvedStyle.fontSize;
            originalText1FontStyle = text1.resolvedStyle.unityFontStyleAndWeight;
            text1.style.opacity = new StyleFloat(1f); // Ensure visible
        }
        if (text2 != null)
        {
            originalText2 = text2.text;
            originalText2FontSize = text2.resolvedStyle.fontSize;
            originalText2FontStyle = text2.resolvedStyle.unityFontStyleAndWeight;
            text2.style.opacity = new StyleFloat(1f);
        }
        if (text3 != null)
        {
            originalText3 = text3.text;
            originalText3FontSize = text3.resolvedStyle.fontSize;
            originalText3FontStyle = text3.resolvedStyle.unityFontStyleAndWeight;
            text3.style.opacity = new StyleFloat(1f);
        }
        if (text4 != null)
        {
            originalText4 = text4.text;
            originalText4FontSize = text4.resolvedStyle.fontSize;
            originalText4FontStyle = text4.resolvedStyle.unityFontStyleAndWeight;
            text4.style.opacity = new StyleFloat(1f);
        }

        // Set up Play Button (unchanged)
        if (playButton != null)
        {
            playButton.clicked += () => SceneManager.LoadScene("LivingStream");
        }

        // Set up Calibrate Button with fading
        if (toggleCalibrate != null)
        {
            toggleCalibrate.clicked += () =>
            {
                if (currentTransition != null)
                {
                    StopCoroutine(currentTransition);
                }

                if (!isTideLowered)
                {
                    // Entering calibration mode
                    if (playButton != null) playButton.style.display = DisplayStyle.None;
                    if (quitButton != null) quitButton.style.display = DisplayStyle.None;

                    // Start text fade and change
                    StartCoroutine(TextTransitionCoroutine(true));

                    // Set target values and start transition for other elements
                    float targetOceanY = originalOceanPosition.y - oceanLowerAmount;
                    float targetLightIntensity = originalLightIntensity - lightIntensityChange;
                    float targetVolume = 0f;
                    Vector3 targetQuadPosition = originalQuadPosition - new Vector3(0, quadMoveAmount, 0);
                    float targetPlaneBlackY = 0f;
                    Vector3 targetCameraPosition = new Vector3(0, 10.7f, 0);
                    Quaternion targetCameraRotation = Quaternion.Euler(90, 0, 0);
                    Vector3 targetSphereRedPosition = new Vector3(5, 0, 0);
                    Vector3 targetSphereBluePosition = new Vector3(0, 0, 5);

                    if (ocean != null && directionalLight != null && backgroundMusic != null &&
                        quadLogo != null && planeBlack != null && mainCamera != null &&
                        sphereRed != null && sphereBlue != null)
                    {
                        currentTransition = StartCoroutine(TransitionCoroutine(
                            ocean.transform.position.y, targetOceanY,
                            directionalLight.intensity, targetLightIntensity,
                            backgroundMusic.volume, targetVolume,
                            quadLogo.transform.position, targetQuadPosition,
                            planeBlack.transform.position.y, targetPlaneBlackY,
                            mainCamera.transform.position, targetCameraPosition,
                            mainCamera.transform.rotation, targetCameraRotation,
                            sphereRed.transform.position, targetSphereRedPosition,
                            sphereBlue.transform.position, targetSphereBluePosition
                        ));
                    }

                    isTideLowered = true;
                    toggleCalibrate.text = "Back";
                }
                else
                {
                    // Exiting calibration mode
                    if (playButton != null) playButton.style.display = DisplayStyle.Flex;
                    if (quitButton != null) quitButton.style.display = DisplayStyle.Flex;

                    // Start text fade and revert
                    StartCoroutine(TextTransitionCoroutine(false));

                    // Set original values and start transition for other elements
                    float targetOceanY = originalOceanPosition.y;
                    float targetLightIntensity = originalLightIntensity;
                    float targetVolume = originalVolume;
                    Vector3 targetQuadPosition = originalQuadPosition;
                    float targetPlaneBlackY = originalPlaneBlackPosition.y;
                    Vector3 targetCameraPosition = originalCameraPosition;
                    Quaternion targetCameraRotation = originalCameraRotation;
                    Vector3 targetSphereRedPosition = originalSphereRedPosition;
                    Vector3 targetSphereBluePosition = originalSphereBluePosition;

                    if (ocean != null && directionalLight != null && backgroundMusic != null &&
                        quadLogo != null && planeBlack != null && mainCamera != null &&
                        sphereRed != null && sphereBlue != null)
                    {
                        currentTransition = StartCoroutine(TransitionCoroutine(
                            ocean.transform.position.y, targetOceanY,
                            directionalLight.intensity, targetLightIntensity,
                            backgroundMusic.volume, targetVolume,
                            quadLogo.transform.position, targetQuadPosition,
                            planeBlack.transform.position.y, targetPlaneBlackY,
                            mainCamera.transform.position, targetCameraPosition,
                            mainCamera.transform.rotation, targetCameraRotation,
                            sphereRed.transform.position, targetSphereRedPosition,
                            sphereBlue.transform.position, targetSphereBluePosition
                        ));
                    }

                    isTideLowered = false;
                    toggleCalibrate.text = "Calibrate";
                }
            };
        }

        // Set up Quit Button (unchanged)
        if (quitButton != null)
        {
            quitButton.clicked += () =>
            {
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            };
        }
    }

    // New coroutine for text fading and changing
    private IEnumerator TextTransitionCoroutine(bool isCalibrating)
    {
        // Fade out text
        yield return StartCoroutine(FadeTextCoroutine(1f, 0f, 0.5f));

        // Apply text changes instantly when faded out
        if (isCalibrating)
        {
            if (text1 != null)
            {
                text1.text = calibrationText1;
                text1.style.fontSize = new StyleLength(24f);
                text1.style.unityFontStyleAndWeight = FontStyle.Normal;
            }
            if (text2 != null)
            {
                text2.text = calibrationText2;
                text2.style.fontSize = new StyleLength(24f);
                // Font style unchanged
            }
            if (text3 != null)
            {
                text3.text = calibrationText3;
                text3.style.fontSize = new StyleLength(24f);
                text3.style.unityFontStyleAndWeight = FontStyle.Normal;
            }
            if (text4 != null)
            {
                text4.text = calibrationText4;
                text4.style.fontSize = new StyleLength(24f);
                // Font style unchanged
            }
        }
        else
        {
            if (text1 != null)
            {
                text1.text = originalText1;
                text1.style.fontSize = new StyleLength(originalText1FontSize);
                text1.style.unityFontStyleAndWeight = originalText1FontStyle;
            }
            if (text2 != null)
            {
                text2.text = originalText2;
                text2.style.fontSize = new StyleLength(originalText2FontSize);
                text2.style.unityFontStyleAndWeight = originalText2FontStyle;
            }
            if (text3 != null)
            {
                text3.text = originalText3;
                text3.style.fontSize = new StyleLength(originalText3FontSize);
                text3.style.unityFontStyleAndWeight = originalText3FontStyle;
            }
            if (text4 != null)
            {
                text4.text = originalText4;
                text4.style.fontSize = new StyleLength(originalText4FontSize);
                text4.style.unityFontStyleAndWeight = originalText4FontStyle;
            }
        }

        // Fade in text
        yield return StartCoroutine(FadeTextCoroutine(0f, 1f, 0.5f));
    }

    // New coroutine for fading text opacity
    private IEnumerator FadeTextCoroutine(float startOpacity, float endOpacity, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            float opacity = Mathf.Lerp(startOpacity, endOpacity, t);

            if (text1 != null) text1.style.opacity = new StyleFloat(opacity);
            if (text2 != null) text2.style.opacity = new StyleFloat(opacity);
            if (text3 != null) text3.style.opacity = new StyleFloat(opacity);
            if (text4 != null) text4.style.opacity = new StyleFloat(opacity);

            yield return null;
        }

        // Ensure final opacity is set
        if (text1 != null) text1.style.opacity = new StyleFloat(endOpacity);
        if (text2 != null) text2.style.opacity = new StyleFloat(endOpacity);
        if (text3 != null) text3.style.opacity = new StyleFloat(endOpacity);
        if (text4 != null) text4.style.opacity = new StyleFloat(endOpacity);
    }

    // Updated TransitionCoroutine without font size lerping
    private IEnumerator TransitionCoroutine(
        float startOceanY, float targetOceanY,
        float startLightIntensity, float targetLightIntensity,
        float startVolume, float targetVolume,
        Vector3 startQuadPosition, Vector3 targetQuadPosition,
        float startPlaneBlackY, float targetPlaneBlackY,
        Vector3 startCameraPosition, Vector3 targetCameraPosition,
        Quaternion startCameraRotation, Quaternion targetCameraRotation,
        Vector3 startSphereRedPosition, Vector3 targetSphereRedPosition,
        Vector3 startSphereBluePosition, Vector3 targetSphereBluePosition
    )
    {
        float timer = 0f;
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / 2f);

            // Lerp other elements
            if (ocean != null)
            {
                Vector3 pos = ocean.transform.position;
                pos.y = Mathf.Lerp(startOceanY, targetOceanY, t);
                ocean.transform.position = pos;
            }
            if (directionalLight != null)
            {
                directionalLight.intensity = Mathf.Lerp(startLightIntensity, targetLightIntensity, t);
            }
            if (backgroundMusic != null)
            {
                backgroundMusic.volume = Mathf.Lerp(startVolume, targetVolume, t);
            }
            if (quadLogo != null)
            {
                quadLogo.transform.position = Vector3.Lerp(startQuadPosition, targetQuadPosition, t);
            }
            if (planeBlack != null)
            {
                Vector3 pos = planeBlack.transform.position;
                pos.y = Mathf.Lerp(startPlaneBlackY, targetPlaneBlackY, t);
                planeBlack.transform.position = pos;
            }
            if (mainCamera != null)
            {
                mainCamera.transform.position = Vector3.Lerp(startCameraPosition, targetCameraPosition, t);
                mainCamera.transform.rotation = Quaternion.Lerp(startCameraRotation, targetCameraRotation, t);
            }
            if (sphereRed != null)
            {
                sphereRed.transform.position = Vector3.Lerp(startSphereRedPosition, targetSphereRedPosition, t);
            }
            if (sphereBlue != null)
            {
                sphereBlue.transform.position = Vector3.Lerp(startSphereBluePosition, targetSphereBluePosition, t);
            }

            yield return null;
        }

        // Set final values
        if (ocean != null)
        {
            Vector3 pos = ocean.transform.position;
            pos.y = targetOceanY;
            ocean.transform.position = pos;
        }
        if (directionalLight != null)
        {
            directionalLight.intensity = targetLightIntensity;
        }
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = targetVolume;
        }
        if (quadLogo != null)
        {
            quadLogo.transform.position = targetQuadPosition;
        }
        if (planeBlack != null)
        {
            Vector3 pos = planeBlack.transform.position;
            pos.y = targetPlaneBlackY;
            planeBlack.transform.position = pos;
        }
        if (mainCamera != null)
        {
            mainCamera.transform.position = targetCameraPosition;
            mainCamera.transform.rotation = targetCameraRotation;
        }
        if (sphereRed != null)
        {
            sphereRed.transform.position = targetSphereRedPosition;
        }
        if (sphereBlue != null)
        {
            sphereBlue.transform.position = targetSphereBluePosition;
        }
    }
}