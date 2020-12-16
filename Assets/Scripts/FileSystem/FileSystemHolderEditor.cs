using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace FileSystem
{
    [CustomEditor(typeof(FileSystemHolder))]
    public class FileSystemHolderEditor : Editor
    {

        private DirectoryTreeView directoryDisplay;
        private TreeViewState _state;

        private Vector2 _scrollPos;

        public void OnEnable()
        {
            if (_state == null)
                _state = new TreeViewState();

            directoryDisplay =
                new DirectoryTreeView((Folder)((FileSystemHolder)serializedObject.targetObject).Catalogue, _state);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rootName"));
            var rect = GUILayoutUtility.GetLastRect();
            using (var scope = new EditorGUILayout.ScrollViewScope(_scrollPos))
            {

            }
            rect = new Rect(0, rect.yMax, rect.width, rect.height * 5);
            GUILayout.BeginArea(rect);
            directoryDisplay.OnGUI(rect);
            GUILayout.EndArea();

            if (GUILayout.Button("Add Folder"))
            {
                directoryDisplay.StartCreatingFolder();
            }
            if (GUILayout.Button("Add File"))
            {
                directoryDisplay.StartCreatingFolder();
            }


            serializedObject.ApplyModifiedProperties();
        }

        private class DirectoryTreeView : TreeView
        {
            private int idGiver = -1;
            private Folder _rootFolder;

            private Dictionary<int, DirectoryData> _myTreeData = new Dictionary<int, DirectoryData>();

            private readonly Texture2D _folderIcon = (Texture2D)EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_Folder Icon" : "Folder Icon").image;
            private Texture2D _textFileIcon = (Texture2D)EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_TextAsset Icon" : "TextAsset Icon").image;
            private Texture2D _tableFileIcon = (Texture2D)EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_UnityEditor.SceneView@2x" : "UnityEditor.SceneView@2x").image;
            private Texture2D _pictureFileIcon = (Texture2D)EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_Texture Icon" : "Texture Icon").image;
            private Texture2D _musicFileIcon = (Texture2D)EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_AudioImporter Icon" : "AudioImporter Icon").image;
            private Texture2D _archiveFileIcon = (Texture2D)EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_ModelImporter Icon" : "ModelImporter Icon").image;
            private Texture2D _otherFileIcon = (Texture2D)EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_Settings@2x" : "Settings@2x").image;

            private Directory editDirectory;
            private string editName;
            private int chosenEditExtension;
            private string editExtension;
            private int editSize;

            private int _creationNode;
            private Folder _createDirectoryAt;
            private bool _isCreatingFile;

            private GUIContent[] _extensions = new[]
            {
                new GUIContent(".txt"),
                new GUIContent(".doc"),
                new GUIContent(".html"),
                new GUIContent(".htm"),
                new GUIContent(".xml"),
                new GUIContent(".json"),
                new GUIContent(".bytes"),
                new GUIContent(".yaml"),
                new GUIContent(".json"),
                new GUIContent(".fnt"),
                new GUIContent(".xls"),
                new GUIContent(".csv"),
                new GUIContent(".dif"),
                new GUIContent(".dbf"),
                new GUIContent(".ods"),
                new GUIContent(".bmp"),
                new GUIContent(".psd"),
                new GUIContent(".jpg"),
                new GUIContent(".gif"),
                new GUIContent(".jpeg"),
                new GUIContent(".tiff"),
                new GUIContent(".png"),
                new GUIContent(".mp3"),
                new GUIContent(".wav"),
                new GUIContent(".flac"),
                new GUIContent(".wma"),
                new GUIContent(".aiff"),
                new GUIContent(".7z"),
                new GUIContent(".zip"),
                new GUIContent(".rar"),
                new GUIContent(".iso"),
                new GUIContent(".tar"),
                new GUIContent(".jar")
            };
            
            public DirectoryTreeView(Folder root, TreeViewState state) : base(state)
            {
                _rootFolder = root;
                Reload();
            }

            protected override TreeViewItem BuildRoot()
            {
                var root = new TreeViewItem(idGiver++, -1);
                root.AddChild(BuildFolder(_rootFolder));
                return root;
            }

            private TreeViewItem BuildFolder(Folder directory)
            {
                var result = new TreeViewItem(idGiver++, directory.Path.Split('\\').Length, directory.Name);
                _myTreeData.Add(result.id, new DirectoryData(directory));
                result.icon = _folderIcon;
                foreach (var dir in directory.SubDirectories)
                {
                    if (dir is Folder folder)
                    {
                        var newFolder = BuildFolder(folder);
                        result.AddChild(newFolder);
                        _myTreeData.Add(newFolder.id, new DirectoryData(folder));
                    }
                    else
                    {
                        if (dir is File file)
                        {
                            var newFileItem = new TreeViewItem(idGiver++, file.Path.Split('\\').Length, file.Name);
                            result.AddChild(newFileItem);
                            
                        }
                    }
                }

                if (directory == _createDirectoryAt)
                {
                    _creationNode = idGiver++;
                    result.AddChild(new TreeViewItem(_creationNode));
                }
                    
                return result;
            }

            protected override void RowGUI(RowGUIArgs args)
            {
                var item = (TreeViewItem)args.item;
                var data = _myTreeData[item.id];
                Rect rect = args.rowRect;
                rect.x += GetContentIndent(item);
                
                if (_creationNode == item.id)
                {
                    if (_isCreatingFile)
                    {
                        editName = EditorGUI.TextField(rect, "Name");
                        editExtension = _extensions[EditorGUI.Popup(rect, chosenEditExtension, _extensions)].text;
                        editSize = EditorGUI.IntField(rect, editSize);
                    }
                    else
                    {
                        editName = EditorGUI.TextField(rect, "Name");
                    }
                }
                
                if (_myTreeData.ContainsKey(item.id))
                {
                    var d = _myTreeData[item.id];
                    if (d.isFolder)
                    {
                        if (!d.isBeingEdited)
                        {
                            base.RowGUI(args);
                            return;
                        }
                        else
                        {
                            editName = EditorGUI.TextField(rect, "Name");
                        }
                    }
                    else
                    {
                        if (!d.isBeingEdited)
                        {
                            base.RowGUI(args);
                            return;
                        }
                        else
                        {
                            editName = EditorGUI.TextField(rect, "Name");
                            editExtension = _extensions[EditorGUI.Popup(rect, chosenEditExtension, _extensions)].text;
                            editSize = EditorGUI.IntField(rect, editSize);
                        }
                    }
                }
            }
            
            private class DirectoryData
            {
                public Directory directory;
                public bool isBeingEdited;
                public bool isFolder;
                public string extension;
                public int size;

                public DirectoryData(Directory directory)
                {
                    this.directory = directory;
                    this.isBeingEdited = false;
                    this.isFolder = directory is Folder;
                    if (directory is File file)
                    {
                        this.extension = file.Name.Split('.').Last();
                        this.size = file.Size;
                    }

                }
            }

            public void ApplyEdit(int id)
            {
                if (!_myTreeData[id].isBeingEdited) return;
                if (editDirectory is File file)
                {
                    file.Rename(editName + editExtension);
                    file.ChangeSize(editSize);
                }
                else
                {
                    if (editDirectory is Folder folder)
                    {
                        folder.Rename(editName);
                    }
                }
                
                _myTreeData[id].isBeingEdited = false;
                Reload();
            }

            public void ApplyDirectoryCreation()
            {
                if (_createDirectoryAt == null) return;
                if (_isCreatingFile)
                {
                    _createDirectoryAt.AddFile(editName, editExtension, editSize);
                }
                else
                {
                    _createDirectoryAt.AddFolder(editName);
                }

                _createDirectoryAt = null;
                Reload();
            }

            protected override void SelectionChanged(IList<int> selectedIds)
            {
                ApplyDirectoryCreation();
                foreach (var k in _myTreeData.Keys)
                {
                    ApplyEdit(k);
                }
                
                base.SelectionChanged(selectedIds);
            }

            protected override void ContextClickedItem(int id)
            {
                _myTreeData[id].isBeingEdited = true;
            }

            public void StartCreatingFolder()
            {
                var selection = GetSelection();
                foreach (var item in selection)
                {
                    if (_myTreeData[item].directory is Folder)
                    {
                        _creationNode = item;
                        _createDirectoryAt = (Folder)_myTreeData[item].directory;
                        _isCreatingFile = false;
                    }
                }
                Reload();
            }
            
            public void StartCreatingFile()
            {
                var selection = GetSelection();
                foreach (var item in selection)
                {
                    if (_myTreeData[item].directory is Folder)
                    {
                        _creationNode = item;
                        _createDirectoryAt = (Folder)_myTreeData[item].directory;
                        _isCreatingFile = true;
                    }
                }
                Reload();
            }
        }
    }
}