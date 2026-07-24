using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private GameObject gameobject;
    void Start()
    {
        //AudioManager.instance.PlaySound("music", "Scene", 0, gameobject);
    }
}