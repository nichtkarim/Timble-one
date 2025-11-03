using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
void Start()
    {
        Debug.Log("Lade CupGame_additive Szene...");
        SceneManager.LoadScene("CupGame_additive", LoadSceneMode.Additive);
    }
}
