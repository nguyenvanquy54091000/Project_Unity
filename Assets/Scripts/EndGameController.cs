using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameController : MonoBehaviour
{
    public void Load()
    {
        SceneManager.LoadScene("PlayGame");
    }

    public void Quit()
    {
        Debug.Log("Quit button clicked!");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Dừng Play Mode
#else
        Application.Quit(); // Thoát ứng dụng
#endif
    }
}
