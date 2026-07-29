using UnityEngine;

namespace CloneGame.Player
{
    /// <summary>
    /// Temporary visual-only effect: draws an expanding, fading ring using a LineRenderer.
    /// Spawned by AoEAttack each time it pulses, so the player can actually see the attack happening. Pure code, no sprite or particle assets required.
    /// </summary>
    public class AoEPulseVisual : MonoBehaviour
    {
        [SerializeField] private float duration = 0.35f;
        [SerializeField] private int segments = 32;
        [SerializeField] private Color color = new Color(1f, 0.55f, 0.1f, 0.85f);

        private LineRenderer line;
        private float targetRadius;
        private float timer;

        public void Init(float radius)
        {
            targetRadius = radius;

            line = gameObject.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.loop = true;
            line.positionCount = segments;
            line.widthMultiplier = 0.08f;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = color;
            line.endColor = color;
            line.sortingOrder = 10;

            Destroy(gameObject, duration);
        }

        private void Update()
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);

            DrawCircle(Mathf.Lerp(0f, targetRadius, t));

            Color c = color;
            c.a = Mathf.Lerp(color.a, 0f, t);
            line.startColor = c;
            line.endColor = c;
        }

        private void DrawCircle(float radius)
        {
            for (int i = 0; i < segments; i++)
            {
                float angle = (i / (float)segments) * Mathf.PI * 2f;
                line.SetPosition(i, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius);
            }
        }
    }
}
