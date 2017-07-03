using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnchorLoader : MonoBehaviour {

    public AnchorManager AnchorManager;

    public void OnAnchorStoreReady()
    {
        AnchorManager.Load();
    }
}
