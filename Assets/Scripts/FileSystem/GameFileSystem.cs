using System;
using System.Collections.Generic;
using System.Linq;
using FileSystem;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = System.Diagnostics.Debug;

public class GameFileSystem : MonoBehaviour, ISmartphoneService, IPointerClickHandler
{
    public RectTransform foldersGuiPanel;
    public GameObject entryGameObject;
    public GameObject toolPanel;
    public TextAsset[] jFileSystemAsset;
    public HardDrive[] fileSystems;
    public TextMeshProUGUI currentDirectoryTmp;
    
    [Header("Appearance")]
    public Sprite folderIcon;
    public Sprite pictureIcon;
    public Sprite musicIcon;
    public Sprite tableIcon;
    public Sprite archiveIcon;
    public Sprite textIcon;
    public Sprite otherIcon;
    
    private Dictionary<DirectoryType, Sprite> _directoryTypeIcon;
    private Folder _fileSystemHolder;
    private JObject _jFileSystem;
    
    private Dictionary<string, Directory> _pathToDirectory;
    private Dictionary<string, Directory> _rootFolders;
    private Directory _currentDirectory;
    public string CurrentDirectoryPath => _currentDirectory.Path;
    private Directory _buffer;
    private string _directoryThatShallBeRemoved;
    private (string name, int size)[] _fileSearchQueries;
    private Dictionary<(string name, int size), bool> _foundFiles;
    
    private GameObject _explorerTools;
    public bool IsShowingRoot => !_currentDirectory.Path.Contains('\\');
    public bool IsOnTopOfFileSystem => _currentDirectory == _fileSystemHolder;
    // Start is called before the first frame update
    public void Initiate()
    {
        _directoryTypeIcon = new Dictionary<DirectoryType, Sprite>();
        _directoryTypeIcon.Add(DirectoryType.Folder, folderIcon);
        _directoryTypeIcon.Add(DirectoryType.Other, otherIcon);
        _directoryTypeIcon.Add(DirectoryType.MusicFile, musicIcon);
        _directoryTypeIcon.Add(DirectoryType.TableFile, tableIcon);
        _directoryTypeIcon.Add(DirectoryType.TextFile, textIcon);
        _directoryTypeIcon.Add(DirectoryType.ArchiveFile, archiveIcon);
        _directoryTypeIcon.Add(DirectoryType.PictureFile, pictureIcon);
        
        _pathToDirectory = new Dictionary<string, Directory>();
        _rootFolders = new Dictionary<string, Directory>();
        ConstructFileListTree();
        _currentDirectory = _fileSystemHolder;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            var tools = Instantiate(toolPanel, eventData.position, transform.rotation, transform);
            ConnectExplorerTools(tools);
            tools.GetComponent<ExplorerTools>().EstablishConnection(this);
        }
        else
        {
            DisconnectExplorerTools();
        }
    }
    private void CheckFilePresent(string fileName, int size)
    {
        foreach (var directoryEntry in _pathToDirectory.Values)
        {
            if (directoryEntry.Name == fileName && directoryEntry.Type != DirectoryType.Folder)
            {
                var fileWithMatchingName = (File) directoryEntry;
                if (fileWithMatchingName.Size == size)
                {
                    _foundFiles[(fileName, size)] = true;
                    return;
                }
            } 
        }
        _foundFiles[(fileName, size)] = false;
    }
    
    private GameObject InstantiateDirectoryEntryButton(Directory subDir)
    {
        var entryButton = Instantiate(entryGameObject, foldersGuiPanel);
        //entryButton.transform.SetParent(foldersGuiPanel, false);
        var directoryEntryComponent = entryButton.GetComponent<DirectoryEntry>();
        directoryEntryComponent.SetPath(subDir.Path, _directoryTypeIcon[subDir.Type], this);
        return entryButton;
    }

    public void ReceiveFileSearchQueries(string[] names, int[] sizes)
    {
        _foundFiles = new Dictionary<(string name, int size), bool>();
        Debug.Assert(names.Length == sizes.Length);
        _fileSearchQueries = new (string name, int size)[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            _fileSearchQueries[i] = (names[i], sizes[i]);
            _foundFiles.Add(_fileSearchQueries[i], false);
            CheckFilePresent(names[i], sizes[i]);
        }
    }

    public bool IsFilePresent(string fileName, int size)
    {
        return _foundFiles[(fileName, size)];
    }

    #region Interfaces

    public void Back()
    {
        if (IsOnTopOfFileSystem)
        {
            gameObject.SetActive(false);
        }
        else
        {
            GetToParentDirectory();
        }
        DisconnectExplorerTools();
    }

    public void Home()
    {
        gameObject.SetActive(false);
        DisconnectExplorerTools();
    }

    public void Open()
    {
        gameObject.SetActive(true);
        OpenDirectory(_currentDirectory);
    }

    public bool IsOpened()
    {
        return gameObject.activeInHierarchy;
    }

    #endregion

    #region ExplorerTools
    public void ConnectExplorerTools(GameObject et)
    {
        if (_explorerTools != null)
        {
            Destroy(_explorerTools);
        }
        _explorerTools = et;
    }

    public void DisconnectExplorerTools()
    {
        if (_explorerTools != null)
        {
            Destroy(_explorerTools);
        }
    }
    #endregion

    #region CopyPaste
    
    private void StoreInBuffer(Directory dir, bool markForDestruction)
    {
        switch (dir)
        {
            case Folder dirFolder:
                _buffer = new Folder(dirFolder);
                _buffer.ReassignPath("");
                break;
            case File dirFile:
                _buffer = new File(dirFile);
                _buffer.ReassignPath("");
                break;
        }

        if (markForDestruction)
        {
            dir.Mark();
        }

        _directoryThatShallBeRemoved = (markForDestruction) ? dir.Path : "";
    }

    public void UnMarkAll()
    {
        _pathToDirectory[""].Unmark();
    }

    public void Copy(string path)
    {
        UnMarkAll();
        StoreInBuffer(_pathToDirectory[path], false);
    }
    
    public void Cut(string path)
    {
        UnMarkAll();
        StoreInBuffer(_pathToDirectory[path], true);
        _directoryThatShallBeRemoved = path;
    }

    public void Paste(string path)
    {
        var dir = (Folder)_pathToDirectory[path];
        Directory copiedDirectory;
        switch (_buffer)
        {
            case Folder f:
                copiedDirectory = new Folder(f);
                
                if (_pathToDirectory.ContainsKey(dir.Path + '\\' + copiedDirectory.Path))
                {
                    ((Folder) copiedDirectory).Rename(dir, copiedDirectory.Name);
                }
                copiedDirectory.ReassignPath(dir.Path);
                dir.SubDirectories.Add(copiedDirectory);
                AddDirectoryToDictionary(copiedDirectory);
                break;
            case File f:
                
                copiedDirectory = new File(f);
                if (_pathToDirectory.ContainsKey(dir.Path + '\\' + copiedDirectory.Path))
                {
                    ((File) copiedDirectory).Rename(dir, copiedDirectory.Name);
                }
                
                copiedDirectory.ReassignPath(dir.Path);
                dir.SubDirectories.Add(copiedDirectory);
                AddDirectoryToDictionary(copiedDirectory);
                break;
        }
        if (_directoryThatShallBeRemoved != "")
        {
            Delete(_directoryThatShallBeRemoved);
            _directoryThatShallBeRemoved = "";
        }
        RecheckFiles();
        OpenDirectory(_currentDirectory);
    }

    public void DeleteDirectory(string path)
    {
        Delete(path);
        RecheckFiles();
        OpenDirectory(_currentDirectory);
    }

    private void Delete(string pathToDelete)
    {
        var parentPath = pathToDelete.Substring(0, pathToDelete.LastIndexOf('\\'));
        var parentDirectory = (Folder)_pathToDirectory[parentPath];
        var directoryToDelete = _pathToDirectory[pathToDelete];
        switch (directoryToDelete)
        {
            case Folder f:
                foreach (var dir in f.SubDirectories.ToArray())
                {
                    Delete(dir.Path);
                    _pathToDirectory.Remove(dir.Path);

                }
                break;
            case File f:
                _pathToDirectory.Remove(f.Path);
                break;       
        }
        parentDirectory.SubDirectories.Remove(directoryToDelete);
        
    }

    private void RecheckFiles()
    {
        for (int i = 0; i < _fileSearchQueries.Length; i++)
        {
            CheckFilePresent(_fileSearchQueries[i].name, _fileSearchQueries[i].size);
        }
    }
    
    public bool CanPaste(string path)
    {
        return !_pathToDirectory[path].IsMarked && !(_pathToDirectory[path] is File);
    }
    
    #endregion

    #region FileSystemConstruction

    public static Directory ConsturctDirectoriesFromJson(TextAsset jsonFile)
    {
        JObject jFileSystem = JObject.Parse(jsonFile.text);
        Folder rootFolder = new Folder(String.Empty,jFileSystem["rootName"].Value<string>());
        var subDirs = jFileSystem["entries"];
        if (subDirs.Any())
        {
            rootFolder.FillFolder(subDirs);
        }

        return rootFolder;
    }

    private void ConstructFileListTree()
    {
        _fileSystemHolder = new Folder(String.Empty, String.Empty);
        _pathToDirectory.Add(_fileSystemHolder.Path, _fileSystemHolder);
        foreach (var asset in jFileSystemAsset)
        {
            var newRootFolder = ConsturctDirectoriesFromJson(asset);
            newRootFolder.ReassignPath(_fileSystemHolder.Path);
            AddDirectoryToDictionary(newRootFolder);
            AddRootDirectory(newRootFolder);
            InstantiateDirectoryEntryButton(newRootFolder);
        }

        foreach (var fileSystem in fileSystems)
        {
            var rootFolder = new Folder((Folder)fileSystem.Catalogue); 
            AddDirectoryToDictionary(rootFolder);
            AddRootDirectory(rootFolder);
            InstantiateDirectoryEntryButton(rootFolder);
        }
        
    }

    #endregion

    #region DirectoryOperations

        public void GetToParentDirectory()
        {
            if (!IsShowingRoot)
            {
                string parentPath = _currentDirectory.Path.Substring(0, _currentDirectory.Path.LastIndexOf('\\'));
                _currentDirectory = _pathToDirectory[parentPath];
                OpenDirectory(parentPath);
            }
            else
            {
                _currentDirectory = _fileSystemHolder;
                OpenDirectory(_fileSystemHolder);
                currentDirectoryTmp.text = "Проводник";
            }
        }

    

        private void AddRootDirectory(Directory dir)
        {
            _rootFolders.Add(dir.Path, dir);
            _fileSystemHolder.SubDirectories.Add(dir);
        }

        public void AddDirectoryToDictionary(Directory dir)
        {
            _pathToDirectory.Add(dir.Path, dir);
            if (dir is Folder)
            {
                foreach (var subDir in ((Folder)dir).SubDirectories)
                {
                    AddDirectoryToDictionary(subDir);
                }
            }
        }
        
        public void RemoveDirectoryToDictionary(Directory dir)
        {
            _pathToDirectory.Remove(dir.Path);
            if (dir is Folder)
            {
                foreach (var subDir in ((Folder)dir).SubDirectories)
                {
                    RemoveDirectoryToDictionary(subDir);
                }
            }
        }
        
        public void OpenDirectory(string path)
        {
            var dir = _pathToDirectory[path];
            if (dir.Type == DirectoryType.Folder)
            {
                for (int i = 0; i < foldersGuiPanel.childCount; i++)
                {
                    Destroy(foldersGuiPanel.GetChild(i).gameObject);
                }
                var directoryFolder = (Folder) dir;
                foreach (var subdir in directoryFolder.SubDirectories)
                {
                    InstantiateDirectoryEntryButton(subdir);
                }
    
                _currentDirectory = dir;
                currentDirectoryTmp.text = _currentDirectory.Name;
            }
        }
        
        public void OpenDirectory(Directory dir)
        {
            if (dir.Type == DirectoryType.Folder)
            {
                for (int i = 0; i < foldersGuiPanel.childCount; i++)
                {
                    Destroy(foldersGuiPanel.GetChild(i).gameObject);
                }
                var directoryFolder = (Folder) dir;
                foreach (var subdir in directoryFolder.SubDirectories)
                {
                    InstantiateDirectoryEntryButton(subdir);
                }
    
                _currentDirectory = dir;
                currentDirectoryTmp.text = _currentDirectory.Name;
            }
        }
        public void AddCatalogue(Directory dir)
        {
            AddRootDirectory(dir);
            AddDirectoryToDictionary(dir);
        }
        
        public void AddCatalogueToDirectory(Directory dir, string path)
        {
            var destination = _pathToDirectory[path];
            Debug.Assert(destination is Folder);
            var folder = (Folder) destination;
            folder.SubDirectories.Add(dir);
            dir.ReassignPath(folder.Path);
            AddDirectoryToDictionary(dir);
        }
    
        public void RemoveCatalogue(string path)
        {
            var dirToRemove = _pathToDirectory[path];
            _fileSystemHolder.SubDirectories.Remove(dirToRemove);
            RemoveDirectoryToDictionary(dirToRemove);
            _rootFolders.Remove(path);
        }

        #endregion
}

