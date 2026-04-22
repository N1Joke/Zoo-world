using TMPro;
using UnityEngine;

namespace ZooWorld.UI.DeathCounter
{
    public class DeathCounterView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _preyDeadText;
        [SerializeField] private TextMeshProUGUI _predatorDeadText;

        private const string _preyFormat = "Prey dead: {0}";
        private const string _predatorFormat = "Predators dead: {0}";

        public void SetPreyDead(int count)
        {
            if (_preyDeadText != null)
                _preyDeadText.text = string.Format(_preyFormat, count);
        }

        public void SetPredatorDead(int count)
        {
            if (_predatorDeadText != null)
                _predatorDeadText.text = string.Format(_predatorFormat, count);
        }
    }
}
