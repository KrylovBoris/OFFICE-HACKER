// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

namespace GlobalMechanics.UI
{
    public interface IInteractable
    {
        void Interact();
        string InteractionDescription();
    }

    public interface ISmartphoneService
    {
        void Back();
        void Home();

        void Open();
        bool IsOpened();
    }

    public interface IMail
    {
        void OpenLetter(string key);
    }

    public interface INotification
    {
        bool HasToShowNotification();
        string NotificationAnimationBool();
    }
}