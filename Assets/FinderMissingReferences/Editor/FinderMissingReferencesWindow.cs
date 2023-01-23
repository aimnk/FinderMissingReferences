using System;
using System.Collections.Generic;
using FinderMissingReferences.Editor.Core;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace FinderMissingReferences.Editor
{
    /// <summary>
    /// Editor window for finding lost links
    /// </summary>
    public class FinderMissingReferencesWindow : EditorWindow
    {
        private const string EDITOR_PREFS_DATA = "FinderMissingData";
        
        private SearchType _searchType;
        
        private Vector2 _scrollPos;

        private List<MissingReferencesData> _missingReferencesDatas = new List<MissingReferencesData>();
        
        private readonly Dictionary<int, string> _iconDictionary = new Dictionary<int, string>()
        {
            {1, "Clipboard"},
            {2, "d_Search Icon"},
            {3, "Error"},
            {4, "TreeEditor.Trash"}
        };
        
        [MenuItem("Finder Missing References/Open window")]
        public static void LaunchUnreferencedAssetsWindow() 
            => GetWindow<FinderMissingReferencesWindow>();
        
        public void OnGUI()
        {
            DrawHeader();
            DrawTitle();
            
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            
            DrawResultMissingReferences(_missingReferencesDatas);
            
            EditorGUILayout.EndScrollView();
            
            GUILayout.FlexibleSpace();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();
            _searchType = (SearchType)EditorGUILayout.EnumPopup(
                _searchType, GUILayout.Width(200), GUILayout.Height(20));

            GUIStyle guiStyle  = new GUIStyle(GUI.skin.button);
            guiStyle.fontStyle = FontStyle.Bold;
            guiStyle.normal.textColor = Color.white;
            guiStyle.alignment = TextAnchor.UpperCenter;
            

            if (GUILayout.Button(TextWithImage("Start search", 2), 
                    guiStyle, GUILayout.Width(200), GUILayout.Height(20)))
            {
                ButtonStartSearch();
            }
            
            DrawCountErrors();
            
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Clear result", guiStyle, GUILayout.Width(100)))
            {
                EditorPrefs.DeleteKey(EDITOR_PREFS_DATA);
                _missingReferencesDatas = new List<MissingReferencesData>();
            }

            EditorGUILayout.EndHorizontal();
            DrawHorizontalSeparator();
        }

        private GUIContent TextWithImage(string text, int indexImage)
        {
            var content = EditorGUIUtility.IconContent(_iconDictionary[indexImage]);
            content.text = text;
            return content;
        }

        private void DrawCountErrors()
        {
            if (_missingReferencesDatas.Count > 0)
            {
                GUIStyle guiStyle = new GUIStyle();
                guiStyle.normal.textColor = Color.red;
                guiStyle.alignment = TextAnchor.LowerLeft;
                guiStyle.fontStyle = FontStyle.Bold;
                
                EditorGUILayout.LabelField(TextWithImage("Count errors: ", 3), 
                    GUILayout.Width(95));
                
                EditorGUILayout.LabelField(_missingReferencesDatas.Count.ToString(), guiStyle,
                    GUILayout.Width(35));
            }
        }

        private void ButtonStartSearch()
        {
            UpdateProgressBar(0, 1);

            IMissingFinder missingFinder = _searchType switch
            {
                SearchType.AllReferences => new FinderMissingReferencesInAssets(),
                SearchType.ReferencesInBuild => new FinderMissingReferencesBuildScenes(),
                SearchType.ReferencesInCurrentScene => new FinderMissingReferencesCurrentScene(),
                SearchType.AllScenes => new FInderMissingReferencesAllScenes(),
                SearchType.Everywhere => new FinderMissingReferencesEverywhere(),
                _ => throw new NullReferenceException()
            };

            _missingReferencesDatas = missingFinder.FindMissingReferences(UpdateProgressBar);

            EditorUtility.ClearProgressBar();
        }

        private void DrawTitle()
        {
            EditorGUILayout.BeginHorizontal();
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.fontStyle = FontStyle.Bold;
            guiStyle.normal.textColor = Color.white;
            guiStyle.alignment = TextAnchor.MiddleCenter;
            
            EditorGUILayout.LabelField("#", guiStyle, GUILayout.Width(35));
            EditorGUILayout.LabelField("Name", guiStyle, GUILayout.Width(250));
            EditorGUILayout.LabelField("Path", guiStyle, GUILayout.Width(200));
            EditorGUILayout.LabelField("Errors", guiStyle, GUILayout.Width(75));
            EditorGUILayout.EndHorizontal();
            DrawHorizontalSeparator();
        }
        
        private void DrawResultMissingReferences(List<MissingReferencesData> missingReferencesDatas)
        {
            if (missingReferencesDatas == null)
            {
                return;
            }
            
            for (int i = 0; i < missingReferencesDatas.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(35));
                
                GUIStyle guiStyle = new GUIStyle(GUI.skin.button);
                guiStyle.alignment = TextAnchor.MiddleLeft;
                guiStyle.fixedWidth = 250;
                
                var obj = AssetDatabase.LoadMainAssetAtPath(missingReferencesDatas[i].PathToAsset);

                if (obj != null)
                {
                    var guiContent = EditorGUIUtility.ObjectContent(null, obj.GetType());
                    guiContent.text = missingReferencesDatas[i].Name;

                    if (GUILayout.Button(guiContent, guiStyle, GUILayout.Height(20)))
                    {
                        Selection.activeObject = obj;
                    }
                }

                EditorGUILayout.LabelField(missingReferencesDatas[i].PathToAsset,
                    GUILayout.Width(200));
                
                if (GUILayout.Button(EditorGUIUtility.IconContent(_iconDictionary[1]), GUILayout.Width(25)))
                {
                    GUIUtility.systemCopyBuffer = missingReferencesDatas[i].PathToAsset;
                }

                GUIStyle labelStyle = new GUIStyle();
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.normal.textColor = Color.red;
                labelStyle.fontStyle = FontStyle.Bold;
                
                EditorGUILayout.LabelField(missingReferencesDatas[i].CountMissingGUID.ToString(),labelStyle,
                    GUILayout.Width(25));
                
                if (missingReferencesDatas[i].MissingReferencesSceneData != null)
                {
                    DrawFoldoutHeader(ref missingReferencesDatas[i].IsShow);
                }

                EditorGUILayout.EndHorizontal();
                DrawHorizontalSeparator();
                
                if (missingReferencesDatas[i].IsShow)
                {
                    DrawMissingGameObjectInScene(missingReferencesDatas[i]);
                }
            }
            Repaint();
        }

        private void DrawFoldoutHeader(ref bool isShow)
        {
            GUIStyle guiStyle = new GUIStyle(GUI.skin.button);
            guiStyle.fixedWidth = 25;

            isShow = EditorGUILayout.BeginFoldoutHeaderGroup(isShow, isShow ? "↑" : "↓", guiStyle);
            
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawMissingGameObjectInScene(MissingReferencesData missingReferencesData)
        {
            var sceneData = missingReferencesData.MissingReferencesSceneData;

            if (sceneData == null)
            {
                return;
            }

            for (int i = 0; i < sceneData.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(50);
                
                GUIStyle guiStyle = new GUIStyle(GUI.skin.button);
                guiStyle.alignment = TextAnchor.MiddleLeft;
                guiStyle.fixedWidth = 250;
                
                var guiContent = EditorGUIUtility.ObjectContent(null, typeof(GameObject));
                
                guiContent.text = sceneData[i].Name;
                
                if (GUILayout.Button(guiContent, guiStyle,GUILayout.Height(20)))
                {
                    if (!GlobalObjectId.TryParse(sceneData[i].GlobalObjectId, out GlobalObjectId globalObjectId))
                    {
                        return;
                    }
                    PingObject(missingReferencesData, globalObjectId);
                }
                
                EditorGUILayout.EndHorizontal();
            }

            DrawHorizontalSeparator();
        }

        private void PingObject(MissingReferencesData missingReferencesData, GlobalObjectId globalObjectId)
        {
            if (missingReferencesData.PathToAsset.EndsWith(".unity"))
            {
                EditorSceneManager.OpenScene(missingReferencesData.PathToAsset);
            }

            var gameObject = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(globalObjectId);

            if (missingReferencesData.PathToAsset.EndsWith(".prefab"))
            {
#if UNITY_2021_1_OR_NEWER
PrefabStageUtility.OpenPrefab(missingReferencesData.PathToAsset);
Selection.activeObject = gameObject;
#endif
            }

            EditorGUIUtility.PingObject(gameObject);
        }
        
        private void UpdateProgressBar(int currentProgress, int maxProgress)
        {
            string progressBarTitle = "Finder missing references";
            string progressBarText = String.Empty;

            progressBarText = currentProgress == 0 ? $"Preparing search in {_searchType.ToString()}" 
                : "Searching missing references..";

            EditorUtility.DisplayProgressBar(progressBarTitle,progressBarText,
                (float) currentProgress/maxProgress);
        }

        private void DrawHorizontalSeparator()
        {
            EditorGUILayout.BeginHorizontal();
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.margin = new RectOffset(0, 0, 4, 4);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 2, guiStyle), Color.grey);
            EditorGUILayout.EndHorizontal();
        }

        private void OnEnable()
        {
            var data =  EditorPrefs.GetString(EDITOR_PREFS_DATA, string.Empty);
            
            if (!string.IsNullOrWhiteSpace(data))
            {
                _missingReferencesDatas = JsonConvert.DeserializeObject<List<MissingReferencesData>>(data);
            }
        }
        
        private void OnDisable() 
            => EditorPrefs.SetString(EDITOR_PREFS_DATA, JsonConvert.SerializeObject(_missingReferencesDatas));
    }
}