using TMPro;
using UnityEngine;

namespace ZooWorld.Gameplay.FX
{
    public class FloatingTextView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _riseSpeed = 1.5f;
        [SerializeField] private float _lifetime = 1.2f;

        public TMP_Text Text => _text;
        public float Lifetime => _lifetime;
        public float RiseSpeed => _riseSpeed;

        public void SetText(string value)
        {
            if (_text != null) _text.text = value;
        }

        public void SetColor(Color color)
        {
            if (_text != null) _text.color = color;
        }
    }
}
