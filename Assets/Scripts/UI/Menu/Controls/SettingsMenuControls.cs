using UnityEngine;
using Zenject;

public class SettingsMenuControls : MonoBehaviour
{
    [Inject] readonly SceneTransition m_sceneTransition;

    public void ReturnToMainMenu()
    {
        m_sceneTransition.LoadScene((int)SceneTransition.Scenes.StartScene);
    }
}