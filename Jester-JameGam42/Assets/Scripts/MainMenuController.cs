using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    [Header("UI Components")]
    public VisualElement ui;

    public Button startButton;
    public Button optionsButton;
    public Button exitButton;
    public Label title;

    [Header("Sound FX")]
    [SerializeField] private AudioClip clickButtonNoise;

    // Start is called before the first frame update
    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    // Update is called once per frame
    private void OnEnable()
    {
        title = ui.Q<Label>("Title");
        startButton = ui.Q<Button>("Start");
        startButton.clicked += StartButtonClicked;

        optionsButton = ui.Q<Button>("Options");
        optionsButton.clicked += OptionsButtonClicked;

        exitButton = ui.Q<Button>("Exit");
        exitButton.clicked += ExitButtonClicked;

        StartCoroutine(FadeInTitle(1f, 0.5f));
        StartCoroutine(FadeInButtons(2f, 0.5f));
    }

    private IEnumerator FadeInTitle(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            float opacity = Mathf.Lerp(0f, 1f, t);
            title.style.opacity = opacity;

            time += Time.deltaTime;
            yield return null;
        }

        title.style.opacity = 1f;
    }

    private IEnumerator FadeInButtons(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            float opacity = Mathf.Lerp(0f, 1f, t);
            startButton.style.opacity = opacity;
            optionsButton.style.opacity = opacity;
            exitButton.style.opacity = opacity;

            time += Time.deltaTime;
            yield return null;
        }

        startButton.style.opacity = 1f;
        optionsButton.style.opacity = 1f;
        exitButton.style.opacity = 1f;
    }

    private void StartButtonClicked()
    {
        SoundManager.instance.PlaySoundFXClip(clickButtonNoise, transform, 50f);
        StartCoroutine(WaitForSoundThenLoad());
    }

    private IEnumerator WaitForSoundThenLoad()
    {
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(1);
    }

    private void OptionsButtonClicked()
    {
        SoundManager.instance.PlaySoundFXClip(clickButtonNoise, transform, 50f);
        Debug.Log("Options");
    }

    private void ExitButtonClicked()
    {
        SoundManager.instance.PlaySoundFXClip(clickButtonNoise, transform, 50f);
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying=false;
#endif
    }
}
