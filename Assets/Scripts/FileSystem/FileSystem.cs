using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FileSystem
{
    [CreateAssetMenu(fileName = "FileSystem", menuName = "ScriptableObjects/FileSystem", order = 4)]
    public class FileSystemHolder : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private string rootName = "root";
        [SerializeField] private string[] paths;
        [SerializeField] private int[] sizes;
        private Directory _calatogue;

        private Dictionary<string, Directory> _pathToDirectory = new Dictionary<string, Directory>();

        public Directory Catalogue => _calatogue;

        public void OnBeforeSerialize()
        {
            paths = _pathToDirectory.Keys.ToArray();
            sizes = _pathToDirectory.Values.Select(d => 
                    (d.Type == DirectoryType.Folder) ? -1 : ((File) d).Size).ToArray();
        }

        public void OnAfterDeserialize()
        {
            _calatogue = new Folder("", rootName);
            _pathToDirectory = new Dictionary<string, Directory>();
            for (var j = 0; j < paths.Length; j++)
            {
                var dirs = paths[j].Split('\\');
                var parentFolder = rootName;
                _pathToDirectory.Add(parentFolder, _calatogue);
                foreach (var d in dirs)
                {
                    if (!_pathToDirectory.ContainsKey(d))
                    {
                        Directory newDir;
                        if (d.Contains('.'))
                        {
                            var fileName = d.Split('.');
                            newDir = ((Folder) _pathToDirectory[parentFolder]).AddFile(fileName[0], fileName[1], sizes[j]);
                        }
                        else
                        {
                            newDir = ((Folder)_pathToDirectory[parentFolder]).AddFolder(d);
                        }

                        
                        _pathToDirectory.Add(parentFolder + '\\' + d, newDir);
                        parentFolder += '\\' + d;
                    }
                }
            }
        }
    }
}
