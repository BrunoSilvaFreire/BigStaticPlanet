using Lunari.Tsuki.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI
{
    public class ItemStackView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _quantityLabel;
        [SerializeField] private TMP_Text _priceLabel;
        [SerializeField] private Image _thumbnail;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _isClearParameter = "IsClear";
        [SerializeField] private Button _button;
        private ItemStack _itemStack;

        public UnityAction<ItemStack> ClickCallback { get; set; }

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

            ClickCallback?.Invoke(_itemStack);
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
                _priceLabel.text = string.Empty;
            }
            else
            {
                var def = itemStack.Definition;
                _nameLabel.text = def.name;
                _quantityLabel.text = itemStack.Quantity.ToString();
                _thumbnail.enabled = true;
                _thumbnail.sprite = def.Thumbnail;
                _priceLabel.text = $"${def.Price}";
            }
        }
    }
}