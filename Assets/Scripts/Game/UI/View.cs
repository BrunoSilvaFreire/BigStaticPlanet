using UnityEditor;
using UnityEngine;

namespace Game.UI
{
    public abstract class View : MonoBehaviour
    {
        [SerializeField] [HideInInspector] private bool shown;

        [SerializeField] protected CanvasGroup _group;

        public bool Shown
        {
            get => shown;
            set
            {
                if (value == shown)
                {
                    return;
                }

                if (_group != null)
                {
                    _group.interactable = value;
                }
#if UNITY_EDITOR
                if (!EditorApplication.isPlaying)
                {
                    if (value)
                    {
                        Show(true);
                    }
                    else
                    {
                        Hide(true);
                    }

                    return;
                }
#endif
                if (value)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
        }

        public void Show(bool immediate = false)
        {
            shown = true;
            if (immediate)
            {
                ImmediateReveal();
            }
            else
            {
                Reveal();
            }
        }

        public void Hide(bool immediate = false)
        {
            shown = false;
            if (immediate)
            {
                ImmediateConceal();
            }
            else
            {
                Conceal();
            }
        }

        public void SetShown(bool shown, bool immediate)
        {
            if (shown)
            {
                Show(immediate);
            }
            else
            {
                Hide(immediate);
            }
        }

        protected abstract void Conceal();
        protected abstract void Reveal();
        protected abstract void ImmediateConceal();
        protected abstract void ImmediateReveal();
        public abstract bool IsFullyShown();
    }
}