using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Author: JohannesMP (2018-08-12)
//
// A wrapper that provides the means to safely serialize Scene Asset References.
//
// Internally we serialize an Object to the SceneAsset which only exists at editor time.
// Any time the object is serialized, we store the path provided by this Asset (assuming it was valid).
//
// This means that, come build time, the string path of the scene asset is always already stored, which if 
// the scene was added to the build settings means it can be loaded.
//
// It is up to the user to ensure the scene exists in the build settings so it is loadable at runtime.
// To help with this, a custom PropertyDrawer displays the scene build settings state.
//
//  Known issues:
// - When reverting back to a prefab which has the asset stored as null, Unity will show the property 
// as modified despite having just reverted. This only happens on the fist time, and reverting again fix it. 
// Under the hood the state is still always valid and serialized correctly regardless.


/// <summary>
/// A wrapper that provides the means to safely serialize Scene Asset References.
/// </summary>

namespace TotBase
{
    [Serializable]
    public class SceneReference
    {
        // This should only ever be set during serialization/deserialization!
        [SerializeField]
        private string scenePath = string.Empty;

        // Use this when you want to actually have the scene path
        public string ScenePath => scenePath;

        public static implicit operator string(SceneReference sceneReference)
        {
            return sceneReference.ScenePath;
        }

        public void LoadScene(bool additive = false)
        {
            SceneManager.LoadScene(ScenePath, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
        }
        public Scene LoadScene(LoadSceneParameters parameters)
        {
            return SceneManager.LoadScene(ScenePath, parameters);
        }
        public void LoadScene()
        {
            SceneManager.LoadScene(ScenePath);
        }
        public AsyncOperation LoadSceneAsync(bool additive = false)
        {
            return SceneManager.LoadSceneAsync(ScenePath, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
        }
        public AsyncOperation LoadSceneAsync(LoadSceneParameters parameters)
        {
            return SceneManager.LoadSceneAsync(ScenePath, parameters);
        }
        public Scene GetScene()
        {
            return SceneManager.GetSceneByPath(ScenePath);
        }
    }
}

