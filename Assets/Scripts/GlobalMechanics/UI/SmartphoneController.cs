// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using FileSystem;
using UnityEngine;

namespace GlobalMechanics.UI
{
    public class SmartphoneController : MonoBehaviour
    {

        public GameFileSystem explorer;
        public Bounties bounties;
        public Store.Store store;
        public MailSmartphoneUI mail;
        public CodexController codex;
        private bool _explorerInitialized = false;
        public enum Program
        {
            None,
            Explorer,
            Store,
            Bounties,
            SocialGraph,
            Mail,
            Codex
        }

        private Program _current;

        private ISmartphoneService _currentSmartphoneProgram;

        public void OpenExplorer()
        {
            OpenProgram(Program.Explorer);
        }
    
        public void OpenStore()
        {
            OpenProgram(Program.Store);
        }
    
        public void OpenBounties()
        {
            OpenProgram(Program.Bounties);
        }
    
        public void OpenSocialGraph()
        {
            OpenProgram(Program.SocialGraph);
        }
    
        public void OpenMail()
        {
            OpenProgram(Program.Mail);
        }
    
        public void OpenCodex()
        {
            OpenProgram(Program.Codex);
        }
    
        private void OpenProgram(Program program)
        {
            switch (program)
            {
                case Program.Explorer:
                    _currentSmartphoneProgram = explorer;
                    break;
                case Program.Mail:
                    _currentSmartphoneProgram = mail;
                    break;
                case Program.Codex:
                    _currentSmartphoneProgram = codex;
                    break;
                case Program.Bounties:
                    _currentSmartphoneProgram = bounties;
                    break;
                case Program.SocialGraph:
                case Program.Store:
                    _currentSmartphoneProgram = store;
                    break;
                default:
                    break;
            }
            _current = program;
            _currentSmartphoneProgram.Open();
        }

        public void SmartBack()
        {
            _currentSmartphoneProgram.Back();
            if (_currentSmartphoneProgram.IsOpened())
            {
                _current = Program.None;
            }
        }

        public void SmartHome()
        {
            _currentSmartphoneProgram.Home();
            _current = Program.None;
        }

        // Start is called before the first frame update
        void Start()
        {
            _current = Program.None;
            explorer.Initiate();
            bounties.Initiate();
            //bounties.gameObject.SetActive(false);
            explorer.gameObject.SetActive(true);       
        }

        // Update is called once per frame
        void Update()
        {
            if (!_explorerInitialized)
            {
                explorer.gameObject.SetActive(false);
                _explorerInitialized = true;
            }
        }

        public GameFileSystem GetExplorer()
        {
            return explorer;
        }
    
        public Bounties GetBounties()
        {
            return bounties;
        }
    }
}
