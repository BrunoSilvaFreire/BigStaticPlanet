using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    public abstract class View : MonoBehaviour
    {
        [SerializeField] [HideInInspector] private bool shown;
        [SerializeField] protected CanvasGroup _group;
        [SerializeField] private UnityEvent _onRevealed;
        [SerializeField] private UnityEvent _onConcealed;

        public UnityEvent OnRevealed => _onRevealed;

        public UnityEvent OnConcealed => _onConcealed;

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
            _onRevealed.Invoke();
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

            _onConcealed.Invoke();
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