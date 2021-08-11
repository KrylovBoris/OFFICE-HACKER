// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using UnityEngine;

namespace GlobalMechanics.UI
{
    public class Approval : MonoBehaviour
    {
    
        public delegate void TaskToApprove();

        private TaskToApprove _task;

        public void Set(TaskToApprove t)
        {
            _task = t;
        }
    
        public void Yes()
        {
            _task.Invoke();
            Destroy(this.gameObject);
        }

        public void No()
        {
            Destroy(this.gameObject);
        }
    }
}
