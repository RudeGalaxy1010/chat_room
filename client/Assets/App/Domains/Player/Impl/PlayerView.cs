using App.Domains.Player.Api;
using TMPro;
using UnityEngine;

namespace App.Domains.Player.Impl
{
    public class PlayerView : View, IPlayerView
    {
        [SerializeField] private Transform _transform = null!;
        [SerializeField] private TMP_Text _nameLabel = null!;
        [SerializeField] private SpriteRenderer _spriteRenderer = null!;

        public Vector2 Position => _transform.position;

        public void SetPosition(Vector2 position) =>
            _transform.position = position;

        public void SetName(string value) =>
            _nameLabel.text = value;

        public void SetColor(Color color) =>
            _spriteRenderer.color = color;

        public void Despawn()
        {
            if (this == null)
            {
                return;
            }

            Destroy(gameObject);
        }
    }
}