using System;
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

        public void Replicate(ItemStack itemStack)
        {
            var isClear = itemStack.IsEmpty();
            _animator.SetBool(_isClearParameter, isClear);
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