using Lunari.Tsuki.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ItemStackView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _quantityLabel;
        [SerializeField] private Image _thumbnail;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _isClearParameter = "IsClear";
        [SerializeField] private Button _button;
        private ItemStack _itemStack;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            if (_itemStack.IsEmpty())
            {
                return;
            }

            var skin = _itemStack.Definition.Skin;
            if (skin == null)
            {
                return;
            }

            if (Player.Instance.Entity.Access(out Skin skinTrait))
            {
                skinTrait.ChangeSkin(skin);
            }
        }

        public void Replicate(ItemStack itemStack)
        {
            var isClear = itemStack.IsEmpty();
            _animator.SetBool(_isClearParameter, isClear);
            _itemStack = itemStack;
            if (isClear)
            {
                _nameLabel.text = string.Empty;
                _quantityLabel.text = string.Empty;
                _thumbnail.sprite = null;
                _thumbnail.enabled = false;
            }
            else
            {
                var def = itemStack.Definition;
                _nameLabel.text = def.name;
                _quantityLabel.text = itemStack.Quantity.ToString();
                _thumbnail.enabled = true;
                _thumbnail.sprite = def.Thumbnail;
            }
        }
    }
}