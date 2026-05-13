using System;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Backend.System.UI.Interface
{
    public interface IModal
    {
        public GameObject GameObject { get; }

        public void Show();
        public void Hide();
        public Type GetModalType();
    }
}