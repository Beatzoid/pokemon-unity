using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The SceneDetails class manages all data and details about a scene
/// </summary>
public class SceneDetails : MonoBehaviour
{
    [SerializeField] private List<SceneDetails> connectedScenes;

    public bool IsLoaded { get; private set; }

    private List<SavableEntity> savableEntities;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"Entered {gameObject.name}");

            LoadScene();
            GameController.Instance.SetCurrentScene(this);

            foreach (SceneDetails scene in connectedScenes)
            {
                scene.LoadScene();
            }

            SceneDetails prevScene = GameController.Instance.PrevScene;

            if (prevScene != null)
            {
                List<SceneDetails> prevLoadedScenes = prevScene.connectedScenes;
                foreach (SceneDetails scene in prevLoadedScenes)
                {
                    if (!connectedScenes.Contains(scene) && scene != this)
                        scene.UnloadScene();
                }

                if (!connectedScenes.Contains(prevScene))
                    prevScene.UnloadScene();
            }
        }
    }

    public void LoadScene()
    {
        if (!IsLoaded)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;

            operation.completed += (_) =>
            {
                savableEntities = GetSavableEntitiesInScene();
                SavingSystem.i.RestoreEntityStates(savableEntities);
            };
        }
    }

    public void UnloadScene()
    {
        if (IsLoaded)
        {
            SavingSystem.i.CaptureEntityStates(savableEntities);

            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }

    private List<SavableEntity> GetSavableEntitiesInScene()
    {
        Scene currentScene = SceneManager.GetSceneByName(gameObject.name);
        List<SavableEntity> savableEntities = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currentScene).ToList();
        return savableEntities;
    }
}
