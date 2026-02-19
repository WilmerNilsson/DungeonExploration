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
    
    private static SceneTransition instance;


    private bool isReloadingNow = false;

    private void Awake()
    {
        _animator.Play(Anim_in.name);
        instance = this;
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

        _animator.Play(Anim_out.name);

        yield return new WaitForSeconds(loadTime);

        SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
    }
}
