using UnityEngine;

public sealed class StartMenuAmbience : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image knight;
    [SerializeField] private Sprite[] idleFrames;
    [SerializeField] private RectTransform[] leaves;
    [SerializeField] private UnityEngine.UI.Image[] lights;
    [SerializeField] private UnityEngine.UI.Image[] beams;
    [SerializeField] private RectTransform background;
    private Vector2[] lightPositions;

    public void Configure(UnityEngine.UI.Image knightImage, Sprite[] frames,
        RectTransform[] leafTransforms, UnityEngine.UI.Image[] lightImages,
        UnityEngine.UI.Image[] beamImages, RectTransform forestBackground)
    {
        knight = knightImage;
        idleFrames = frames;
        leaves = leafTransforms;
        lights = lightImages;
        beams = beamImages;
        background = forestBackground;
    }

    private void Awake()
    {
        lightPositions = new Vector2[lights.Length];
        for (int i = 0; i < lights.Length; i++)
            lightPositions[i] = lights[i].rectTransform.anchoredPosition;
    }

    private void Update()
    {
        float time = Time.unscaledTime;
        knight.sprite = idleFrames[(int)(time * 12f) % idleFrames.Length];
        for (int i = 0; i < leaves.Length; i++)
        {
            float phase = time * (50f + i * 7f % 31f) / 1240f + i / 14f;
            float progress = Mathf.Repeat(phase, 1f);
            leaves[i].anchoredPosition = new Vector2(-1100f + i * 160f + progress * 280f + Mathf.Sin(time * 0.9f + i) * 45f, 620f - progress * 1240f);
            leaves[i].localRotation = Quaternion.Euler(0f, 0f, time * (22f + i % 4 * 8f) + Mathf.Sin(time + i) * 30f + i * 37f);
        }
        for (int i = 0; i < lights.Length; i++)
        {
            float pulse = Mathf.Sin(time * (0.8f + i * 0.15f) + i);
            lights[i].rectTransform.anchoredPosition = lightPositions[i] + new Vector2(Mathf.Sin(time * 0.36f + i) * 40f, Mathf.Cos(time * 0.3f + i) * 24f);
            lights[i].rectTransform.localScale = Vector3.one * (1f + pulse * 0.05f);
            lights[i].color = new Color(1f, 0.91f, 0.55f, 0.135f + pulse * 0.035f);
        }
        for (int i = 0; i < beams.Length; i++)
            beams[i].color = new Color(1f, 0.91f, 0.62f, 0.14f + Mathf.Sin(time * (0.75f + i * 0.1f) + i * 1.8f) * 0.035f);
        background.anchoredPosition = new Vector2(Mathf.Sin(time * 0.15f) * 4f, Mathf.Cos(time * 0.12f) * 4f);
    }
}
