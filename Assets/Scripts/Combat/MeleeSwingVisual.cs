using UnityEngine;

namespace CloneGame.Player
{
    /// <summary>
    /// Temporary visual-only effect: draws a fading box outline for the melee swing.
    /// Pure code, no sprite/art asset.
    /// </summary>
    public class MeleeSwingVisual : MonoBehaviour
    {
        [SerializeField] private float duration = 0.15f;
        [SerializeField] private Color color = new Color(1f, 1f, 1f, 0.9f);

        private LineRenderer line;
        private float timer;

        public void Init(Vector2 size)
        {
            line = gameObject.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.loop = true;
            line.positionCount = 4;
            line.widthMultiplier = 0.06f;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = color;
            line.endColor = color;
            line.sortingOrder = 10;

            float hx = size.x / 2f;
            float hy = size.y / 2f;
            line.SetPosition(0, new Vector3(-hx, -hy, 0f));
            line.SetPosition(1, new Vector3(hx, -hy, 0f));
            line.SetPosition(2, new Vector3(hx, hy, 0f));
            line.SetPosition(3, new Vector3(-hx, hy, 0f));

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
