using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace FileSystem
{
    public enum DirectoryType
    {
        None,
        Folder,
        MusicFile,
        PictureFile,
        TableFile,
        TextFile,
        ArchiveFile,
        Other
    }
    
    public abstract class Directory
    {
        protected string _path;
        //protected GameFileSystem host;
        protected bool _markedForDestruction;
        public bool IsMarked => _markedForDestruction;

        public virtual void Mark()
        {
            _markedForDestruction = true;
        }

        public virtual void Unmark()
        {
            _markedForDestruction = false;
        }

        public string Path => _path;

        public virtual string Name
        {
            get
            {
                if (_path.Contains('\\'))
                {
                    return _path.Substring(_path.LastIndexOf('\\') + 1);
                }
                return _path;
            }
        }

        public virtual DirectoryType Type => DirectoryType.None;
        
        protected Directory(string parentDirectoryPath, string name)
        {
            _path = parentDirectoryPath + (parentDirectoryPath.Length != 0 ? "\\" : "") + name;
        }

        protected Directory(Directory dir)
        {
            _path = dir.Path;
        }

        public virtual void ReassignPath(string newPath)
        {
            string oldName = Name;
            _path = newPath + (newPath.Length != 0 ? "\\" : "") + oldName;
        }
    }
    
    public class File : Directory
    {
        private string _extension;
        private int _size;
        public int Size => _size;
        public override DirectoryType Type => FromExtensionToFileType(_extension);

        //public string Name => _path.Substring(_path.LastIndexOf('\\'));

        private static DirectoryType FromExtensionToFileType(string extension)
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
                    return DirectoryType.TextFile;
                case ".xls":
                case ".csv":
                case ".dif":
                case ".dbf":
                case ".ods":
                    return DirectoryType.TableFile;
                case ".bmp":
                case ".psd":
                case ".jpg":
                case ".gif":
                case ".jpeg":
                case ".tiff":
                case ".png":
                    return DirectoryType.PictureFile;
                case ".mp3":
                case ".wav":
                case ".flac":
                case ".wma":
                case ".aiff":
                    return DirectoryType.MusicFile;
                case ".7z":
                case ".zip":
                case ".rar":
                case ".iso":
                case ".tar":
                case ".jar":
                    return DirectoryType.ArchiveFile;
                case "":
                    return DirectoryType.None;
                default:
                    return DirectoryType.Other;
            }
        }
        
        public File(string parentDirectoryPath, string name, string extension, int size):base(parentDirectoryPath, name)
        {
            _extension = extension;
            _path += _extension;
            _size = size;
        }

        public File(File file) : base(file)
        {
            _extension = file._extension;
            //_path += _extension;
            _size = file._size;
        }

        public void Rename(string newName)
        {
            string newExtension = newName.Substring(newName.IndexOf('.'));
            string fileName = newName.Substring(0, newName.IndexOf('.'));
            _extension = newExtension;
            _path = _path.Substring(0, _path.LastIndexOf('\\')) + fileName + _extension;
        }

        public void ChangeSize(int editSize)
        {
            _size = editSize;
        }
    }

    public class Folder: Directory
    {
        private List<Directory> _subDirectories;
        public override DirectoryType Type => DirectoryType.Folder;
        
        public Folder(string parentDirectoryPath, string name):base(parentDirectoryPath, name)
        {
            _subDirectories = new List<Directory>();
        }

        public Folder(Folder folder):base(folder)
        {
            _subDirectories = new List<Directory>();
            foreach (var dir in folder._subDirectories)
            {
                switch (dir)
                {
                    case Folder subFolder:
                        _subDirectories.Add(new Folder(subFolder));
                        break;
                    
                    case File file:
                        _subDirectories.Add(new File(file));
                        break;
                }
            }
        }

        public void Rename(string newName)
        {
            if (_path.Contains('\\'))
            {
                string parentPath = Path.Substring(0, Path.LastIndexOf('\\'));
                _path = parentPath + '\\' + newName;
            }
            else
            {
                _path = newName;
            }
            foreach (var dir in SubDirectories)
            {
                dir.ReassignPath(Path);
            }
        }

        public override void ReassignPath(string newPath)
        {
            base.ReassignPath(newPath);
            foreach (var dir in _subDirectories)
            {
                dir.ReassignPath(this.Path);
            }
        }

        public override void Mark()
        {
            base.Mark();
            foreach (var dir in _subDirectories)
            {
                dir.Mark();
            }
        }

        public override void Unmark()
        {
            base.Unmark();
            foreach (var dir in _subDirectories)
            {
                dir.Unmark();
            }
        }

        public List<Directory> SubDirectories => _subDirectories;

        public Folder AddFolder(string name)
        {
            var dir = new Folder(this.Path, name);
            _subDirectories.Add(dir);
            return dir;
        }

        public File AddFile(string name, string extension, int fileSize)
        {
            var dir = new File(this.Path, name, extension, fileSize);
            _subDirectories.Add(dir);
            return dir;
        }

        public void FillFolder(JToken branchesHolder)
        {
            foreach (var directory in branchesHolder)
            {
                if (directory["type"].Value<string>() == "folder")
                {
                    var newFolder = this.AddFolder(directory["entryName"].Value<string>());
                    //host.AddDirectoryToDictionary(newFolder.Path, newFolder);
                    if (directory["entries"].Any())
                    {
                        newFolder.FillFolder(directory["entries"]);
                    }
                }
                else
                {
                    var newFile = this.AddFile(directory["entryName"].Value<string>(), directory["type"].Value<string>(), directory["size"].Value<int>());
                    //host.AddDirectoryToDictionary(newFile.Path, newFile);
                }
            }
        }
    }
}

