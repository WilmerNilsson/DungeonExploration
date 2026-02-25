using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    
    [SerializeField] private AnimationClip Anim_in;
    [SerializeField] private AnimationClip Anim_out;
    [SerializeField] private Animator _animator;

    [SerializeField] private float transitionTime = 1f;
    [SerializeField] private float reloadTransitionTime = 1f;
    
    private bool isReloadingNow = false;
    
    private static SceneTransition instance;
    

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (Anim_in)
        {
            _animator.Play(Anim_in.name);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public static SceneTransition GetInstance() 
    {
        return instance;
    }
    
    public void ChangeScene(string sceneToLoad)
    {
        if (isReloadingNow)
        {
            return;
        }

        isReloadingNow = true;
        // Load the scene named "NewScene"
        StartCoroutine(LoadLevel(sceneToLoad, transitionTime));
    }

    public void ReloadScene()
    {
        if (!isReloadingNow)
        {
            isReloadingNow = true;

            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().name, reloadTransitionTime));
        }
    }

    IEnumerator LoadLevel(string level, float loadTime)
    {
        if (Anim_out)
        {
            _animator.Play(Anim_out.name);

            yield return new WaitForSeconds(loadTime);
        }

        SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Anim_in)
        {
            _animator.Play(Anim_in.name);
        }
        isReloadingNow = false;
    }

    public void SetTransitionIn(AnimationClip  clip)
    {
        Anim_in  = clip;
    }
    
    public void SetTransitionOut(AnimationClip  clip)
    {
        Anim_out  = clip;
    }
}
