using System;
using UnityEngine;
using UnityEditor;

namespace TotBaseEditor
{
    public class PCMScriptableObject : PCMMenuElement
    {
        private Type type;

        public PCMScriptableObject(Type type) {
            this.type = type;
            CreateAssetMenuAttribute attr = type.GetCustomAttributes(typeof(CreateAssetMenuAttribute), false)[0] as CreateAssetMenuAttribute;
            string[] menuPath = attr.menuName.Split('/');
            name = $"{menuPath[menuPath.Length - 2]}/{menuPath[menuPath.Length - 1]}";
        }

        public override void Execute()
        {
            ScriptableObject so = ScriptableObject.CreateInstance(type);
            string folder = EditorUtils.getActiveFolderPath();
            AssetDatabase.CreateAsset (so, AssetDatabase.GenerateUniqueAssetPath (folder + "/New MyAsset.asset"));
            Selection.SetActiveObjectWithContext(so, null);
        }
    }
}