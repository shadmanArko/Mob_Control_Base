using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class HudView : MonoBehaviour
    {
        public UIBlurAnimation blurAnimation;
        public UIPopupAnimation failedPopup;
        public UIPopupAnimation winPopup;
        public Button retryButton;
        public Button playAgainButton;
    }
}