using UnityEngine;

[AddComponentMenu("redd096/Singletons/Game Manager")]
[DefaultExecutionOrder(-100)]
public class GameManager : redd096.Singleton<GameManager>
{
    //public UIManager uiManager { get; private set; }

    protected override void SetDefaults()
    {
        //get references
        //uiManager = FindObjectOfType<UIManager>();
    }
}