using DG.Tweening;
using I2.Loc;
using Library.Utils;
using TMPro;
using UnityEngine;

namespace App.Tutorial
{
    public class CloudHelpText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI message;

        public void Setup(TutorialStepType stepType)
        {
            string transl =LocalizationManager.GetTranslation(stepType.ToString());
            if (transl == null) transl = stepType.GetAttribute<TutorialAttribute>().DefaultText;
            message.text = transl.Length <=0 ? stepType.GetAttribute<TutorialAttribute>().DefaultText : transl;
            message.DOFade(1, 0.2f);
        }
    }
}