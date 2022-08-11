using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] private List<SceneDetails> connectedScenes;

    public bool IsLoaded { get; private set; }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"Entered {gameObject.name}");

            LoadScene();
            GameController.instance.SetCurrentScene(this);

            foreach (SceneDetails scene in connectedScenes)
            {
                scene.LoadScene();
            }

            if (GameController.instance.PrevScene != null)
            {
                List<SceneDetails> prevLoadedScenes = GameController.instance.PrevScene.connectedScenes;
                foreach (SceneDetails scene in prevLoadedScenes)
                {
                    if (!connectedScenes.Contains(scene) && scene != this)
                        scene.UnloadScene();
                }
            }
        }
    }

    public void LoadScene()
    {
        if (!IsLoaded)
        {
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;
        }
    }

    public void UnloadScene()
    {
        if (IsLoaded)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }
}
