using FileSystem;
using UnityEngine;
using UnityEngine.UI;

namespace GlobalMechanics.UI
{
    public class ExplorerTools : MonoBehaviour
    {
        public Button pasteButton;
        public GameObject approvalWindow;
        private string _pathToDirectory;
        private GameFileSystem _fileSystem;
        private DirectoryEntry _button;

    
        public void EstablishConnection(GameFileSystem fs)
        {
            _fileSystem = fs;
            _pathToDirectory = fs.CurrentDirectoryPath;
            foreach (var button in transform.GetComponentsInChildren<Button>())
            {
                pasteButton.interactable = false;
            }
        }
        public void EstablishConnection(GameFileSystem fs, DirectoryEntry entry)
        {
            _fileSystem = fs;
            _button = entry;
            _pathToDirectory = _button.Path;
            pasteButton.interactable = _fileSystem.CanPaste(_pathToDirectory);
        }
    
        public void Open()
        {
            _fileSystem.OpenDirectory(_pathToDirectory);
            Destroy(gameObject);
        }

        public void Copy()
        {
            _fileSystem.Copy(_pathToDirectory);
            Destroy(gameObject);
        }

        public void Cut()
        {
            _fileSystem.Cut(_pathToDirectory);
            Destroy(gameObject);
        }

        public void Paste()
        {
            _fileSystem.Paste(_pathToDirectory);
            Destroy(gameObject);
        }

        public void Rename()
        {
            _button.StartRenaming();
            Destroy(gameObject);
        }

        public void Delete()
        {
            Instantiate(approvalWindow, Input.mousePosition, _button.transform.rotation, _fileSystem.transform).GetComponent<Approval>().Set(() => _fileSystem.DeleteDirectory(_pathToDirectory));
            Destroy(gameObject);
        }
    }
}
