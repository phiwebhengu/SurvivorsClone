using UnityEngine;

namespace CloneGame.Player
{
    public class MeleeSwingVisual : MonoBehaviour
    {
        [SerializeField] private float duration = 0.12f;
        [SerializeField] private Color color = new Color(1f, 1f, 1f, 0.95f);
        [SerializeField] private int segments = 14;

        private LineRenderer line;
        private float timer;

        public void Init(Vector2 size)
        {
            line = gameObject.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.loop = false;
            line.positionCount = segments;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = color;
            line.endColor = color;
            line.sortingOrder = 10;
            line.numCapVertices = 6;
            line.textureMode = LineTextureMode.Stretch;

            // Thin at both ends, thick in the middle — reads as a single clean swipe instead of a blocky outline.
            var widthCurve = new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(0.5f, 0.22f),
                new Keyframe(1f, 0f)
            );
            line.widthCurve = widthCurve;
            line.widthMultiplier = 1f;

            // Draw a shallow arc sweeping across the hit area, rather than tracing its literal rectangular bounds.
            float arcHeight = size.y * 0.7f;
            float halfLength = size.x * 0.5f;

            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)(segments - 1);
                float x = Mathf.Lerp(-halfLength * 0.2f, halfLength, t);
                float y = Mathf.Sin(t * Mathf.PI) * arcHeight - arcHeight * 0.4f;
                line.SetPosition(i, new Vector3(x, y, 0f));
            }

            Destroy(gameObject, duration);
        }

        private void Update()
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);

            Color c = color;
            c.a = Mathf.Lerp(color.a, 0f, t);
            line.startColor = c;
            line.endColor = c;
        }
    }
}
