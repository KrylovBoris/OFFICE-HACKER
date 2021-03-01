using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FileSystem
{
    [CreateAssetMenu(fileName = "FileSystem", menuName = "ScriptableObjects/FileSystem", order = 4)]
    public class HardDrive : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private string[] paths = new []{"root"};
        [SerializeField] private int[] sizes;

        private Directory _catalogue;

        private Dictionary<string, Directory> _pathToDirectory = new Dictionary<string, Directory>();

        public Directory Catalogue
        {
            get
            {
                if (_catalogue == null)
                {
                    if (!paths.Any()) paths = new[] {"root"};
                    _catalogue = new Folder("", paths[0]);
                    _pathToDirectory.Add(_catalogue.Path, _catalogue);
                }
                return _catalogue;
            }
        }

        public void OnBeforeSerialize()
        {
            SerializeTree();
        }

        public void SerializeTree()
        {
            var sortedPaths = _pathToDirectory.Keys.ToList();
            sortedPaths.Sort(((s, s1) => String.Compare(s, s1, StringComparison.Ordinal)));
            paths = sortedPaths.ToArray();
            sizes = _pathToDirectory.Values.Select(d => 
                (d.Type == DirectoryType.Folder) ? -1 : ((File) d).Size).ToArray();
        }

        public void RenameRoot(string s)
        {
            ((Folder) _catalogue).Rename(null, s);
        }
        
        public void OnAfterDeserialize()
        {
            _pathToDirectory = new Dictionary<string, Directory>();
            var parentFolder = Catalogue.Path;
            for (var j = 1; j < paths.Length; j++)
            {
                var dirs = paths[j].Split('\\').ToList();
                parentFolder = dirs.First();
                dirs.RemoveAt(0);
                foreach (var d in dirs)
                {
                    if (!_pathToDirectory.ContainsKey(parentFolder + '\\' + d))
                    {
                        Directory newDir;
                        if (d.Contains('.'))
                        {
                            var fileName = d.Split('.');
                            newDir = ((Folder) _pathToDirectory[parentFolder]).AddFile(fileName[0], '.' + fileName[1], sizes[j]);
                        }
                        else
                        {
                            newDir = ((Folder)_pathToDirectory[parentFolder]).AddFolder(d);
                        }
                        
                        _pathToDirectory.Add(parentFolder + '\\' + d, newDir);
                    }
                    parentFolder += '\\' + d;
                }
            }
        }

        public void Rename(Folder folder, string s)
        {
            _pathToDirectory.Remove(folder.Path);
            RemovePathsFromDictionary(folder);
            if (folder.Path.Contains('\\'))
                folder.Rename((Folder)_pathToDirectory[folder.Path.Substring(0, folder.Path.LastIndexOf('\\'))], s);
            else
                folder.Rename(null, s);
            AddPathsToDictionary(folder);
        }

        private void AddPathsToDictionary(Folder f)
        {
            _pathToDirectory.Add(f.Path, f);
            foreach (var dir in f.SubDirectories)
            {
                switch (dir)
                {
                    case Folder folder:
                        AddPathsToDictionary(folder);
                        break;
                    case File file:
                        _pathToDirectory.Add(file.Path, file);
                        break;
                }
            }
        }
        private void RemovePathsFromDictionary(Folder f)
        {
            _pathToDirectory.Remove(f.Path);
            foreach (var dir in f.SubDirectories)
            {
                switch (dir)
                {
                    case Folder folder:
                        RemovePathsFromDictionary(folder);
                        break;
                    case File file:
                        _pathToDirectory.Remove(file.Path);
                        break;
                }
            }
        }
        public void Rename(File file, string s)
        {
            _pathToDirectory.Remove(file.Path);
            file.Rename((Folder)_pathToDirectory[file.Path.Substring(0, file.Path.LastIndexOf('\\'))], s);
            _pathToDirectory.Add(file.Path, file);
        }
        public void AddFolder(Folder directory, string s)
        {
            var folder = directory.AddFolder(s);
            _pathToDirectory.Add(folder.Path, folder);
        }
        public void AddFile(Folder directory, string s, string extension, int size)
        {
            var file = directory.AddFile(s, extension, size);
            _pathToDirectory.Add(file.Path, file);
        }
        public void RemoveDirectory(Directory directory)
        {
            var parentPath = directory.Path.Substring(0, directory.Path.LastIndexOf('\\'));
            var parentDirectory = (Folder)_pathToDirectory[parentPath];
            var directoryToDelete = _pathToDirectory[directory.Path];
            switch (directoryToDelete)
            {
                case Folder f:
                    foreach (var dir in f.SubDirectories.ToArray())
                    {
                        RemoveDirectory(dir);
                        _pathToDirectory.Remove(dir.Path);
                    }
                    break;
                case File f:
                    _pathToDirectory.Remove(f.Path);
                    break;       
            }
            _pathToDirectory.Remove(directory.Path);
            parentDirectory.SubDirectories.Remove(directoryToDelete);
        }
        public void RemoveMarked()
        {
            RemoveMarkedInFolder((Folder)Catalogue);
        }
        private void RemoveMarkedInFolder(Folder dir)
        {
            foreach (var subDirectory in dir.SubDirectories)
            {
                if (subDirectory is Folder subFolder)
                    RemoveMarkedInFolder(subFolder);
            }
            dir.SubDirectories.RemoveAll(d => d.IsMarked);
        }

        public void FixDictionary()
        {
            _pathToDirectory.Clear();
            AddPathsToDictionary((Folder)Catalogue);
        }
    }
}
