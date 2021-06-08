using System.Collections.Generic;
using FileSystem;
using UnityEngine;

namespace GlobalMechanics.UI
{
    public class Bounties : MonoBehaviour, ISmartphoneService, INotification
    {
    
        public GameFileSystem fileSystem;
        private Objective[] _objectives;
        private string[] _wantedFiles;
        private int[] _wantedFilesSizes;

        private Dictionary<(string name, int size), Objective> _filesToObjectives;
        // Start is called before the first frame update
        public void Initiate()
        {
            _filesToObjectives = new Dictionary<(string name, int size), Objective>();
            _objectives = GetComponentsInChildren<Objective>();
            _wantedFiles = new string[_objectives.Length];
            _wantedFilesSizes = new int[_objectives.Length];;
            for(int i = 0; i < _objectives.Length; i++)
            {
                _filesToObjectives.Add((_objectives[i].file, _objectives[i].size), _objectives[i]);
                _wantedFiles[i] = _objectives[i].file;
                _wantedFilesSizes[i] = _objectives[i].size;
            }

            fileSystem.ReceiveFileSearchQueries(_wantedFiles, _wantedFilesSizes);
        }

        public void ObjectiveFound(string fileName, int size)    
        {
            _filesToObjectives[(name: fileName, size)].UnBlockButton();
        }

        public bool HasToShowNotification()
        {
            foreach (var obj in _objectives)
            {
                if (obj.Finished) return true;
            }

            return false;
        }

        public string NotificationAnimationBool()
        {
            return "UncheckedBounties";
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < _wantedFiles.Length; i++)
            {
                if (fileSystem.IsFilePresent(_wantedFiles[i], _wantedFilesSizes[i]))
                {
                    _objectives[i].UnBlockButton();
                }
                else
                {
                    _objectives[i].BlockButton();
                }
            }
        }
    
        public void Home()
        {
            this.gameObject.SetActive(false);
        }

        public void Open()
        {
            this.gameObject.SetActive(true);
        }

        public bool IsOpened()
        {
            return this.gameObject.activeInHierarchy;
        }
    
        public void Back()
        {
            this.gameObject.SetActive(false);
        }
    }
}
