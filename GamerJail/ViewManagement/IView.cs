using System;

namespace GamerJail.ViewManagement
{
    interface IView
    {
        event EventHandler CloseRequest;
        void Close();
    }
}
