using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Collections.Viewable;
using JetBrains.Lifetimes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace FileSystem
{
    public class FileSystemHolderEditor : EditorWindow
    {
        [SerializeField]
        private HardDrive _holder;
        private DirectoryTreeView _directoryDisplay;
        private TreeViewState _state;

        private Vector2 _scrollPos;

        public void OnEnable()
        {
            _state ??= new TreeViewState();
            if (_holder != null)
                Init();
        }

        private void Init()
        {
            _directoryDisplay ??= new DirectoryTreeView(_holder, _state);
        }

        [MenuItem("Window/Edit file system")]
        public static void OpenFileSystemEditor()
        {
            GetWindow<FileSystemHolderEditor>().Show();
        }
        
        public void OnGUI()
        {
            _holder = (HardDrive) EditorGUILayout.ObjectField(_holder, typeof(HardDrive), false);

            if (!_holder) return;
            Init();
            
            var rect = new Rect(0, 0, this.position.width, this.position.height);

            EditorGUILayout.BeginVertical();
            using (var scope = new EditorGUILayout.ScrollViewScope(_scrollPos, true, true))
            {
                _scrollPos = scope.scrollPosition;
                _directoryDisplay.OnGUI(new Rect(0, 0, rect.width - 20, rect.height));
            }
            
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add Folder"))
                {
                    _directoryDisplay.StartCreatingFolder(_holder);
                }

                if (GUILayout.Button("Add File"))
                {
                    _directoryDisplay.StartCreatingFile(_holder);
                }
            }
            EditorGUILayout.EndVertical();
            if (GUILayout.Button("Apply changes"))
            {
                _holder.SerializeTree();
                EditorUtility.SetDirty(_holder);
                this.Close();
            }
        }

        private class DirectoryTreeView : TreeView
        {
            private int _idGiver;
            private HardDrive _drive;
            private readonly Folder _rootFolder;

            private readonly Dictionary<int, Directory> _myTreeData = new Dictionary<int, Directory>();

            private static readonly Texture2D FolderIcon = (Texture2D)EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_Folder Icon" : "Folder Icon").image;
            private static readonly Texture2D TextFileIcon = (Texture2D)EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_TextAsset Icon" : "TextAsset Icon").image;
            private static readonly Texture2D TableFileIcon = (Texture2D)EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_UnityEditor.SceneView@2x" : "UnityEditor.SceneView@2x").image;
            private static readonly Texture2D PictureFileIcon = (Texture2D)EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_Texture Icon" : "Texture Icon").image;
            private static readonly Texture2D MusicFileIcon = (Texture2D)EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_AudioImporter Icon" : "AudioImporter Icon").image;
            private static readonly Texture2D ArchiveFileIcon = (Texture2D)EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_ModelImporter Icon" : "ModelImporter Icon").image;
            private static readonly Texture2D OtherFileIcon = (Texture2D)EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_Settings@2x" : "Settings@2x").image;

            private IEnumerable<Directory> _buffer = Enumerable.Empty<Directory>();

            public DirectoryTreeView(HardDrive drive, TreeViewState state) : base(state)
            {
                _drive = drive;
                _rootFolder = (Folder) drive.Catalogue;
                useScrollView = false;
                Reload();
            }

            protected override TreeViewItem BuildRoot()
            {
                _idGiver = -1;
                _myTreeData.Clear();
                var root = new TreeViewItem(_idGiver++, -1);
                root.AddChild(BuildFolder(_rootFolder));
                return root;
            }

            private TreeViewItem BuildFolder(Folder directory)
            {
                var result = new TreeViewItem(_idGiver++, directory.Path.Split('\\').Length - 1, directory.Name);
                _myTreeData.Add(result.id, directory);
                result.icon = FolderIcon;
                foreach (var dir in directory.SubDirectories)
                {
                    if (dir is Folder folder)
                    {
                        var newFolder = BuildFolder(folder);
                        result.AddChild(newFolder);
                    }
                    else
                    {
                        if (dir is File file)
                        {
                            var newFileItem = new TreeViewItem(_idGiver++, file.Path.Split('\\').Length - 1, file.Name)
                            {
                                icon = FromExtensionToIcon(file.Extension)
                            };
                            result.AddChild(newFileItem);
                            _myTreeData.Add(newFileItem.id, file);
                        }
                    }
                }

                return result;
            }
            
            private static Texture2D FromExtensionToIcon(string extension)
            {
                switch (extension)
                {
                    case ".txt":
                    case ".doc":
                    case ".odf":
                    case ".html":
                    case ".htm":
                    case ".xml":
                    case ".json":
                    case ".bytes":
                    case ".yaml":
                    case ".fnt":
                        return TextFileIcon;
                    case ".xls":
                    case ".csv":
                    case ".dif":
                    case ".dbf":
                    case ".ods":
                        return TableFileIcon;
                    case ".bmp":
                    case ".psd":
                    case ".jpg":
                    case ".gif":
                    case ".jpeg":
                    case ".tiff":
                    case ".png":
                        return PictureFileIcon;
                    case ".mp3":
                    case ".wav":
                    case ".flac":
                    case ".wma":
                    case ".aiff":
                        return MusicFileIcon;
                    case ".7z":
                    case ".zip":
                    case ".rar":
                    case ".iso":
                    case ".tar":
                    case ".jar":
                        return ArchiveFileIcon;
                    default:
                        return OtherFileIcon;
                }
            }

            protected override void RowGUI(RowGUIArgs args)
            {
                var item = args.item;
                Rect rect = args.rowRect;
                rect.x += GetContentIndent(item);
                
                if (_myTreeData.ContainsKey(item.id))
                {
                    base.RowGUI(args);
                }
            }
            
            protected override void ContextClickedItem(int id)
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Edit Node"), false, () => StartEditingNode(_drive, _myTreeData[id]));
                menu.AddItem(new GUIContent("Create File"), false, () => StartCreatingFile(_drive));
                menu.AddItem(new GUIContent("Create Folder"), false, () => StartCreatingFolder(_drive));
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Copy"), false, () => StoreCopy(ChosenDirectories()));
                menu.AddItem(new GUIContent("Cut"), false, () => MarkForCut(ChosenDirectories()));
                if (_buffer.Any() && CanPaste())
                {
                    menu.AddItem(new GUIContent("Paste"), false, () => Paste((Folder)_myTreeData[id]));
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Paste"));
                }
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, () => Delete(_myTreeData[id]));
                menu.ShowAsContext();
            }

            private bool CanPaste()
            {
                var location = _myTreeData[GetSelection().Last()];
                return !location.IsMarked && !(location is File);
            }

            private void Delete(Directory directory)
            {
                _drive.RemoveDirectory(directory);
                Reload();
            }

            private IEnumerable<Directory> ChosenDirectories()
            {
                var selected = GetSelection().Select(i => _myTreeData[i]).ToList();
                selected.Sort((directory, directory1) =>
                {
                    if (directory.Path.Contains(directory1.Path)) return 1;
                    if (directory1.Path.Contains(directory.Path)) return -1;
                    return 0;
                });

                var result = new List<Directory>();
                
                foreach (var dir in selected.Where(dir => !result.Exists(d => dir.Path.Contains(d.Path + '\\'))))
                {
                    result.Add(dir);
                }

                return result;
            }

            public void Paste(Folder dir)
            {
                foreach (var entry in _buffer)
                {
                    Directory copiedDirectory;
                    switch (entry)
                    {
                        case Folder f:
                            copiedDirectory = f;
                            if (dir.SubDirectories.Exists(d => d.Name == f.Name))
                            {
                                ((Folder) copiedDirectory).Rename(dir, copiedDirectory.Name);
                            }
                            copiedDirectory.ReassignPath(dir.Path);
                            dir.SubDirectories.Add(copiedDirectory);
                            break;
                        case File f:
                            copiedDirectory = f;
                            if (dir.SubDirectories.Exists(d => d.Name == f.Name))
                            {
                                ((File) copiedDirectory).Rename(dir, copiedDirectory.Name);
                            }
                            copiedDirectory.ReassignPath(dir.Path);
                            dir.SubDirectories.Add(copiedDirectory);
                            break;
                    }
                    _drive.RemoveMarked();
                    _drive.FixDictionary();
                }
                Reload();
            }

            private void MarkForCut(IEnumerable<Directory> directory)
            {
                StoreCopy(directory);
                foreach (var dir in directory)
                {
                    dir.Mark();
                }
            }

            private void UnmarkAll()
            {
                _rootFolder.Unmark();
            }

            private void StoreCopy(IEnumerable<Directory> directory)
            {
                UnmarkAll();
                var res = new List<Directory>();
                foreach (var dir in directory)
                {
                    switch (dir)
                    {
                        case Folder f:
                            var copiedFolder = new Folder(f);
                            copiedFolder.ReassignPath("");
                            res.Add(copiedFolder);
                            break;
                        case File f:
                            var copiedFile = new File(f);
                            copiedFile.ReassignPath("");
                            res.Add(copiedFile);
                            break;
                    }
                }

                _buffer = res;
            }

            private void StartEditingNode(HardDrive holder, Directory dir)
            {
                var item = dir;
                var signal = NodeEditor.EditNode(holder, dir);
                signal.AdviseOnce(Lifetime.Eternal, (b =>
                {
                    if (b) Reload();
                }));
            }

            public void StartCreatingFolder(HardDrive holder)
            {
                var selection = GetSelection();
                if (!selection.Any()) return;
                var item = _myTreeData[selection.Last()];
                if (item is Folder folder)
                {
                    var signal = NodeEditor.CreateNode(holder, folder, true);
                    signal.AdviseOnce(Lifetime.Eternal, (b =>
                    {
                        if (b) Reload();
                    }));
                }
            }
            public void StartCreatingFile(HardDrive holder)
            {
                var selection = GetSelection();
                var item = _myTreeData[selection.Last()];
                if (item is Folder folder)
                {
                    var signal = NodeEditor.CreateNode(holder, folder, false);
                    signal.AdviseOnce(Lifetime.Eternal, (b =>
                    {
                        if (b) Reload();
                    }));
                }

            }
        }
        
        public class NodeEditor : EditorWindow
        {
            private HardDrive _holder;
            private Directory _directory;
            private string _name;
            private int _size;
            private string _extension;
            private bool _isEditing;
            private bool _isCreatingFolder;

            private Signal<bool> _finishedEditing;

            private static readonly Lazy<Texture> TextFileIcon = new Lazy<Texture>(() => EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_TextAsset Icon" : "TextAsset Icon").image);
            private static readonly Lazy<Texture> TableFileIcon = new Lazy<Texture>(() => EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_UnityEditor.SceneView@2x" : "UnityEditor.SceneView@2x").image);
            private static readonly Lazy<Texture> PictureFileIcon = new Lazy<Texture>(() => EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_Texture Icon" : "Texture Icon").image);
            private static readonly Lazy<Texture> MusicFileIcon = new Lazy<Texture>(() => EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_AudioImporter Icon" : "AudioImporter Icon").image);
            private static readonly Lazy<Texture> ArchiveFileIcon = new Lazy<Texture>(() => EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_ModelImporter Icon" : "ModelImporter Icon").image);
            
            private int _extensionIndex;
            private readonly Lazy<GUIContent[]> _extensions = new Lazy<GUIContent[]>( ()=> new []
            {
                new GUIContent(".txt", TextFileIcon.Value),
                new GUIContent(".doc", TextFileIcon.Value),
                new GUIContent(".html", TextFileIcon.Value),
                new GUIContent(".htm", TextFileIcon.Value),
                new GUIContent(".xml", TextFileIcon.Value),
                new GUIContent(".json", TextFileIcon.Value),
                new GUIContent(".bytes", TextFileIcon.Value),
                new GUIContent(".yaml", TextFileIcon.Value),
                new GUIContent(".json", TextFileIcon.Value),
                new GUIContent(".fnt", TextFileIcon.Value),
                new GUIContent(".xls", TableFileIcon.Value),
                new GUIContent(".csv", TableFileIcon.Value),
                new GUIContent(".dif", TableFileIcon.Value),
                new GUIContent(".dbf", TableFileIcon.Value),
                new GUIContent(".ods", TableFileIcon.Value),
                new GUIContent(".bmp", PictureFileIcon.Value),
                new GUIContent(".psd", PictureFileIcon.Value),
                new GUIContent(".jpg", PictureFileIcon.Value),
                new GUIContent(".gif", PictureFileIcon.Value),
                new GUIContent(".jpeg", PictureFileIcon.Value),
                new GUIContent(".tiff", PictureFileIcon.Value),
                new GUIContent(".png", PictureFileIcon.Value),
                new GUIContent(".mp3", MusicFileIcon.Value),
                new GUIContent(".wav", MusicFileIcon.Value),
                new GUIContent(".flac", MusicFileIcon.Value),
                new GUIContent(".wma", MusicFileIcon.Value),
                new GUIContent(".aiff", MusicFileIcon.Value),
                new GUIContent(".7z", ArchiveFileIcon.Value),
                new GUIContent(".zip", ArchiveFileIcon.Value),
                new GUIContent(".rar", ArchiveFileIcon.Value),
                new GUIContent(".iso", ArchiveFileIcon.Value),
                new GUIContent(".tar", ArchiveFileIcon.Value),
                new GUIContent(".jar", ArchiveFileIcon.Value)
            });

            public void OnDisable()
            {
                Debug.Log("Close");
            }
            
            public static Signal<bool> CreateNode(HardDrive holder, Folder parent, bool isCreatingFolder)
            {
                var editor = EditorWindow.GetWindow<NodeEditor>();
                editor._holder = holder;
                editor._directory = parent;
                editor._isEditing = false;
                editor._isCreatingFolder = isCreatingFolder;
                editor.ShowPopup();
                editor._finishedEditing = new Signal<bool>();
                return editor._finishedEditing;
            }

            public static Signal<bool> EditNode(HardDrive holder, Directory entryToEdit)
            {
                var editor = EditorWindow.GetWindow<NodeEditor>();
                editor._holder = holder;
                editor._isEditing = true;
                editor._name = entryToEdit.Name.Split('.').First();
                editor._directory = entryToEdit;
                if (entryToEdit is File fileEntry)
                {
                    editor._size = fileEntry.Size;
                    editor._extension = fileEntry.Extension;
                }
                editor.ShowPopup();
                editor._finishedEditing = new Signal<bool>();
                return editor._finishedEditing;
            }
            public void Apply() 
            {
                if (_isEditing)
                {
                        if (_directory is Folder folder)
                        {
                            _holder.Rename(folder, _name);
                        }

                        if (_directory is File file)
                        {
                            _holder.Rename(file, _name + _extension);
                            file.ChangeSize(_size);
                        }
                }
                else
                {
                    if (_isCreatingFolder)
                    {
                        _holder.AddFolder((Folder) _directory, _name);
                    }
                    else
                    {
                        _holder.AddFile((Folder) _directory, _name, _extension, _size);
                    }
                }
                _finishedEditing.Fire(true);
                this.Close();
            }

            public void Cancel()
            {
                _finishedEditing.Fire(false);
                this.Close();
            }

            public void OnGUI()
            {
                _name = EditorGUILayout.TextField(_name);
                if (_isEditing)
                {
                    switch (_directory)
                    {
                        case File _:
                            _extensionIndex = EditorGUILayout.Popup(new GUIContent("File extension"), _extensionIndex, _extensions.Value);
                            _extension = _extensions.Value[_extensionIndex].text;
                            _size = EditorGUILayout.IntField("File size", _size);
                            break;
                        case Folder _:
                            
                            break;
                    }
                }
                else
                {
                    if (!_isCreatingFolder)
                    {
                        _extensionIndex = EditorGUILayout.Popup(new GUIContent("File extension"), _extensionIndex, _extensions.Value);
                        _extension = _extensions.Value[_extensionIndex].text;
                        _size = EditorGUILayout.IntField("File size", _size);
                    }
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Cancel"))
                {
                    Cancel();
                }
                if (GUILayout.Button("Apply"))
                {
                    Apply();
                }
                EditorGUILayout.EndHorizontal();
            }
        }


    }
}