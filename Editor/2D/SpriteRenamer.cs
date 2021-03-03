using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text.RegularExpressions;
using TotBaseEditor;

public class SpriteRenamer : EditorWindow
{
    private Animator anim;
    private SpriteRenderer rend;

    [SerializeField]
    private Texture2D texture;
    [SerializeField]
    private int count;
    [SerializeField]
    private List<SpriteSerie> series = new List<SpriteSerie>();
    [SerializeField]
    private Vector2 scroll;
    [SerializeField]
    private Vector2 listScroll;

    private Regex spriteIndex = new Regex(@"_([0-9]+)$");

    [MenuItem("Window/TotBase/Sprite Renamer")]
    public static void OpenWindow()
    {
        GetWindow(typeof(SpriteRenamer));
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUI.BeginChangeCheck();
        texture = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false);
        bool changed = EditorGUI.EndChangeCheck();
        if (texture != null && changed)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            count = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().Count();
        }
        else if (texture == null && changed)
            count = 0;

        if (texture != null && count != 0)
            Renamer();
        EditorGUILayout.EndVertical();
    }

    private void Renamer()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll, false, false, GUILayout.Height(position.height));
        EditorGUILayout.PrefixLabel("Sprite Count : "); 
        EditorGUILayout.LabelField(count.ToString());
        listScroll = EditorGUILayout.BeginScrollView(listScroll, false, false, GUILayout.Height(300));
        series.ToList().ForEach(x => SerieEditor(x));
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("Add", GUILayout.Width(100)))
        {
            series.Add(new SpriteSerie());
            Repaint();
        }
        GUILayout.Space(50);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Rename Sprites", GUILayout.Width(100)))
            RenameProcess();
        if (GUILayout.Button("Try load series", GUILayout.Width(100)))
            TryReloadSeries();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(50);
        EditorGUILayout.LabelField("Animation generator", EditorStyles.boldLabel);
        EditorGUILayout.PrefixLabel("Animator...");
        anim = (Animator)EditorGUILayout.ObjectField(anim, typeof(Animator), true);
        EditorGUILayout.PrefixLabel("Sprite Renderer");
        rend = (SpriteRenderer)EditorGUILayout.ObjectField(rend, typeof(SpriteRenderer), true);
        GUILayout.Space(50);
        if (rend != null && anim != null && GUILayout.Button("Create Animations", GUILayout.Width(150)))
        {
            CreateAnimations();
        }
        EditorGUILayout.EndScrollView();
    }

    private void SerieEditor(SpriteSerie serie)
    {
        EditorGUILayout.BeginHorizontal();
        serie.name = EditorGUILayout.TextField(serie.name);
        serie.start = EditorGUILayout.IntSlider(serie.start, 0, count - 1);
        serie.end = EditorGUILayout.IntSlider(serie.end, 0, count - 1);
        if(GUILayout.Button("Delete"))
        {
            series.Remove(serie);
            Repaint();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void RenameProcess()
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        SpriteMetaData[] spritesheed = importer.spritesheet;
        series.ForEach(s =>
        {
            for(int i = s.start; i <= s.end; i++)
            {
                spritesheed[i].name = $"{texture.name}_{s.name}_{i - s.start}";
            }
        });
        importer.spritesheet = spritesheed;
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }

    private void TryReloadSeries()
    {
        string path = AssetDatabase.GetAssetPath(texture);
        string textureName = texture.name;
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        SpriteMetaData[] spritesheed = importer.spritesheet;
        Dictionary<string, SpriteSerie> tserie = new Dictionary<string, SpriteSerie>();
        for(int i = 0; i < spritesheed.Length; i++)
        {
            string name = spritesheed[i].name.Replace($"{textureName}_", "");
            name = spriteIndex.Replace(name, "");
            if (tserie.ContainsKey(name))
                tserie[name].ScaleSerie(i);
            else
            {
                tserie[name] = new SpriteSerie()
                {
                    name = name,
                    start = i,
                    end = i
                };
            }
        }

        series = tserie.Values.ToList();
    }

    private void CreateAnimations()
    {
        string path = GetFolder();
        foreach(SpriteSerie s in series)
            CreateAnimation(s, texture, anim.transform, rend.transform, path);
        AssetDatabase.SaveAssets();
    }

    private string GetFolder()
    {
        return EditorUtility.OpenFolderPanel("Animation destination folder", "Assets/", "");
    }

    private void CreateAnimation(SpriteSerie serie, Texture2D tex, Transform anim, Transform rend, string targetFolder)
    {
        if (targetFolder.StartsWith(Application.dataPath))
            targetFolder = "Assets" + targetFolder.Substring(Application.dataPath.Length);

        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tex)).OfType<Sprite>().ToArray();
        Dictionary<string, Sprite> lookupTable = new Dictionary<string, Sprite>();
        foreach (Sprite s in sprites)
            lookupTable[s.name] = s;
        TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(tex)) as TextureImporter;
        SpriteMetaData[] spritesheed = importer.spritesheet;

        string transformPath = AnimationUtility.CalculateTransformPath(rend, anim);
        int framecount = serie.end - serie.start + 1;
        
        AnimationClip clip = new AnimationClip();
        clip.name = $"{tex.name}_{serie.name}";
        clip.frameRate = framecount;
        clip.wrapMode = WrapMode.Loop;

        // now some voodoo stuff to set sprite ref on those keyframes
        EditorCurveBinding binding = EditorCurveBinding.PPtrCurve(transformPath, typeof(SpriteRenderer), "m_Sprite");
        ObjectReferenceKeyframe[] objectRefCurve = new ObjectReferenceKeyframe[framecount];
        for (int i = 0; i < objectRefCurve.Length; i++)
            objectRefCurve[i] = new ObjectReferenceKeyframe()
            {
                    time = i/(float)framecount,
                    value = lookupTable[spritesheed[serie.start + i].name]
            };

        AnimationUtility.SetObjectReferenceCurve(clip, binding, objectRefCurve);
        AssetDatabase.CreateAsset(clip, $"{targetFolder}\\{clip.name}.anim");
    }

}

public class SpriteSerie
{
    public string name;
    public int start=0;
    public int end=0;
    public void ScaleSerie(int index)
    {
        if (start > index)
            start = index;
        if (end < index)
            end = index;
    }

}