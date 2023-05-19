using System;
using UnityEngine;
using UnityEngine.UI;
using ZBase.Foundation.Mvvm.ViewBinding;

namespace ZBase.Foundation.Mvvm.Unity.ViewBinding.Binders
{
    [AddComponentMenu("MVVM/Binders/Image Binder")]
    public partial class ImageBinder : MonoBinder
    {
        [SerializeField]
        private Image[] _targets = new Image[0];

        protected override void OnAwake()
        {
            if (_targets.Length < 1)
            {
                if (this.gameObject.TryGetComponent<Image>(out var target))
                {
                    _targets = new Image[] { target };
                }
            }

            if (_targets.Length < 1)
            {
                Debug.LogWarning($"The target list is empty.", this);
            }
        }

        [Binding]
        [field: Label("Color")]
        [field: HideInInspector]
        private void SetColor(Color value)
        {
            var targets = _targets.AsSpan();
            var length = targets.Length;

            for (var i = 0; i < length; i++)
            {
                var target = targets[i];

                if (target)
                {
                    target.color = value;
                }
            }
        }

        [Binding]
        [field: Label("Sprite")]
        [field: HideInInspector]
        private void SetSprite(Sprite value)
        {
            var targets = _targets.AsSpan();
            var length = targets.Length;

            for (var i = 0; i < length; i++)
            {
                var target = targets[i];

                if (target)
                {
                    target.sprite = value;
                }
            }
        }

        [Binding]
        [field: Label("Fill Method")]
        [field: HideInInspector]
        private void SetFillMethod(Image.FillMethod value)
        {
            var targets = _targets.AsSpan();
            var length = targets.Length;

            for (var i = 0; i < length; i++)
            {
                var target = targets[i];

                if (target)
                {
                    target.fillMethod = value;
                }
            }
        }

        [Binding]
        [field: Label("Fill Origin")]
        [field: HideInInspector]
        private void SetFillOrigin(int value)
        {
            var targets = _targets.AsSpan();
            var length = targets.Length;

            for (var i = 0; i < length; i++)
            {
                var target = targets[i];

                if (target)
                {
                    target.fillOrigin = value;
                }
            }
        }

        [Binding]
        [field: Label("Fill Amount")]
        [field: HideInInspector]
        private void SetFillAmount(float value)
        {
            var targets = _targets.AsSpan();
            var length = targets.Length;

            for (var i = 0; i < length; i++)
            {
                var target = targets[i];

                if (target)
                {
                    target.fillAmount = value;
                }
            }
        }
    }
}
