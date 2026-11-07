using UnityEngine;
using UnityEngine.UI;

namespace Library.View
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class SpriteSwapToggleButton : ToggleButton
    {
        [SerializeField] private Sprite _checkedSprite;
        [SerializeField] private Sprite _uncheckedSprite;

        protected override void UpdateState()
        {
            GetComponent<Image>().sprite = IsChecked ? _checkedSprite : _uncheckedSprite;
        }
    }
}