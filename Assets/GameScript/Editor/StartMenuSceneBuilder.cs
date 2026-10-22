using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public static class StartMenuSceneBuilder
{
    private const string Art = "Assets/GameRes/UIPanelArt/StartMenu/";
    private const string ScenePath = "Assets/GameRes/Scene/StartMenu.unity";
    private static TMP_FontAsset font;
    private static Sprite buttonSprite;
    private static Material buttonFontMaterial;

    [MenuItem("Tools/ARPG/Build Start Menu Scene")]
    public static void Build()
    {
        if (SceneManager.GetSceneByPath(ScenePath).isLoaded)
            throw new System.InvalidOperationException("StartMenu is already open. Close it before rebuilding to preserve scene edits.");

        Sprite background = ImportSprite(Art + "StartMenu_Background.png");
        ImportSprite(Art + "StartMenu_ButtonWood.png", true);
        TextureImporter buttonImporter = (TextureImporter)AssetImporter.GetAtPath(Art + "StartMenu_ButtonWood.png");
        buttonImporter.spriteImportMode = SpriteImportMode.Multiple;
        buttonImporter.spritePixelsPerUnit = 100f;
        SpriteDataProviderFactories factories = new SpriteDataProviderFactories();
        factories.Init();
        ISpriteEditorDataProvider provider = factories.GetSpriteEditorDataProviderFromObject(buttonImporter);
        provider.InitSpriteEditorDataProvider();
        SpriteRect existing = provider.GetSpriteRects().FirstOrDefault(rect => rect.name == "StartMenu_ButtonWood");
        GUID spriteId = existing != null ? existing.spriteID : GUID.Generate();
        provider.SetSpriteRects(new[]
        {
            new SpriteRect
            {
                name = "StartMenu_ButtonWood",
                rect = new Rect(47f, 141f, 1985f, 490f),
                border = new Vector4(110f, 120f, 110f, 130f),
                alignment = SpriteAlignment.Center,
                pivot = new Vector2(0.5f, 0.5f),
                spriteID = spriteId
            }
        });
        provider.GetDataProvider<ISpriteNameFileIdDataProvider>().SetNameFileIdPairs(new[]
        {
            new SpriteNameFileIdPair("StartMenu_ButtonWood", spriteId)
        });
        provider.Apply();
        buttonImporter.SaveAndReimport();
        buttonSprite = AssetDatabase.LoadAllAssetsAtPath(Art + "StartMenu_ButtonWood.png").OfType<Sprite>().Single();
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/GameRes/UIFont/FusionPixel/FusionPixel_12px_Proportional_zh-Hans SDF.asset");
        buttonFontMaterial = AssetDatabase.LoadAssetAtPath<Material>(Art + "StartMenu_Font.mat");
        if (buttonFontMaterial == null)
        {
            buttonFontMaterial = new Material(font.material) { name = "StartMenu_Font" };
            AssetDatabase.CreateAsset(buttonFontMaterial, Art + "StartMenu_Font.mat");
        }
        buttonFontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 0.035f);
        EditorUtility.SetDirty(buttonFontMaterial);
        Sprite[] frames = AssetDatabase.LoadAllAssetsAtPath("Assets/GameRes/EntityArt/Player/Warrior/Warrior_Idle.png")
            .OfType<Sprite>().OrderBy(sprite => sprite.name).ToArray();
        Sprite leafSprite = MakeEffectSprite("StartMenu_Leaf.png", 9, 7, false);
        Sprite glowSprite = MakeEffectSprite("StartMenu_Glow.png", 32, 32, true);
        Sprite beamSprite = MakeBeamSprite();
        Sprite arrowSprite = MakeArrowSprite();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        SceneManager.SetActiveScene(scene);
        GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.GetComponent<Camera>();
        camera.orthographic = true;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.025f, 0.04f, 0.045f);
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);
        GameObject lightObject = new GameObject("Menu Light", typeof(Light));
        lightObject.GetComponent<Light>().type = LightType.Directional;
        lightObject.GetComponent<Light>().intensity = 0.5f;

        GameObject canvasObject = new GameObject("StartMenuCanvas", typeof(RectTransform), typeof(Canvas),
            typeof(UnityEngine.UI.CanvasScaler), typeof(UnityEngine.UI.GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;
        UnityEngine.UI.CanvasScaler scaler = canvasObject.GetComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        RectTransform design = Rect("DesignRoot", canvasObject.transform, Vector2.zero, new Vector2(1920f, 1080f));
        design.gameObject.AddComponent<StartMenuDesignFit>();
        design.gameObject.AddComponent<UnityEngine.UI.RectMask2D>();
        RectTransform forest = Image("ForestBackground", design, Vector2.zero, new Vector2(1920f, 1080f), background, Color.white).rectTransform;
        forest.localScale = Vector3.one * 1.015f;

        UnityEngine.UI.Image[] beams = new UnityEngine.UI.Image[3];
        for (int i = 0; i < beams.Length; i++)
        {
            beams[i] = Image("Sunbeam" + i, design, new Vector2(330f + i * 170f, 250f - i * 65f),
                new Vector2(140f + i * 40f, 800f), beamSprite, new Color(1f, 0.91f, 0.62f, 0.14f));
            beams[i].rectTransform.localRotation = Quaternion.Euler(0f, 0f, -25f);
        }

        UnityEngine.UI.Image[] lights = new UnityEngine.UI.Image[3];
        for (int i = 0; i < lights.Length; i++)
            lights[i] = Image("LightMote" + i, design, new Vector2(-550f + i * 490f, 130f - i * 90f),
                new Vector2(240f + i * 80f, 240f + i * 80f), glowSprite, new Color(1f, 0.91f, 0.55f, 0.135f));

        UnityEngine.UI.Image knight = Image("KnightIdle", design, FromTop(1283f, 600f),
            new Vector2(288f, 288f), frames[0], Color.white);
        RectTransform[] leaves = new RectTransform[14];
        for (int i = 0; i < leaves.Length; i++)
        {
            float width = 20f + i % 4 * 4f;
            Color color = i % 4 == 0 ? new Color(0.93f, 0.65f, 0.25f, 0.88f)
                : new Color(0.72f + i % 3 * 0.08f, 0.83f, 0.37f, 0.8f + i % 4 * 0.05f);
            leaves[i] = Image("FallingLeaf" + i, design, new Vector2(-950f + i * 140f, 560f - i * 80f),
                new Vector2(width, width * 7f / 9f), leafSprite, color).rectTransform;
        }
        design.gameObject.AddComponent<StartMenuAmbience>().Configure(knight, frames, leaves, lights, beams, forest);

        RectTransform menu = Rect("Menu", design, Vector2.zero, new Vector2(1920f, 1080f));
        UnityEngine.UI.Button start = Button("StartGameButton", menu, "开始游戏", FromTop(800f, 608f), new Vector2(380f, 100f));
        GameObject arrow = Image("SelectionArrow", start.transform, new Vector2(-145f, 0f), new Vector2(12f, 18f),
            arrowSprite, new Color(0.91f, 0.96f, 0.72f)).gameObject;
        arrow.SetActive(false);
        start.GetComponent<StartMenuButtonFeedback>().Configure(arrow);
        UnityEngine.UI.Button settingsButton = Button("SettingsButton", menu, "游戏设置", FromTop(800f, 732f), new Vector2(340f, 88f));
        UnityEngine.UI.Button quit = Button("QuitButton", menu, "退出游戏", FromTop(800f, 840f), new Vector2(340f, 88f));

        RectTransform settings = Rect("Settings", design, Vector2.zero, new Vector2(1920f, 1080f));
        Image("Dimmer", settings, Vector2.zero, new Vector2(1920f, 1080f), null, new Color(0f, 0.035f, 0.05f, 0.75f)).raycastTarget = true;
        RectTransform panel = Image("Panel", settings, Vector2.zero, new Vector2(680f, 530f), null, new Color(0.055f, 0.11f, 0.14f, 0.98f)).rectTransform;
        Text("SettingsTitle", panel, "游戏设置", new Vector2(0f, 190f), new Vector2(500f, 70f), 48f);
        Text("VolumeLabel", panel, "音量", new Vector2(-210f, 75f), new Vector2(120f, 60f), 36f);
        UnityEngine.UI.Slider volume = VolumeSlider(panel);
        UnityEngine.UI.Toggle fullscreen = FullscreenToggle(panel);
        UnityEngine.UI.Button back = Button("BackButton", panel, "返回", new Vector2(0f, -175f), new Vector2(310f, 84f));
        settings.gameObject.SetActive(false);

        StartMenuController controller = canvasObject.AddComponent<StartMenuController>();
        controller.Configure(menu.gameObject, settings.gameObject, start, volume, fullscreen);
        UnityEventTools.AddPersistentListener(start.onClick, controller.StartGame);
        UnityEventTools.AddPersistentListener(settingsButton.onClick, controller.OpenSettings);
        UnityEventTools.AddPersistentListener(quit.onClick, controller.QuitGame);
        UnityEventTools.AddPersistentListener(back.onClick, controller.CloseSettings);
        UnityEventTools.AddPersistentListener(volume.onValueChanged, controller.SetVolume);
        UnityEventTools.AddPersistentListener(fullscreen.onValueChanged, controller.SetFullscreen);
        GameObject events = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
        events.GetComponent<EventSystem>().firstSelectedGameObject = start.gameObject;

        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) }
            .Concat(EditorBuildSettings.scenes.Where(entry => entry.path != ScenePath)).ToArray();
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = canvasObject;
        Debug.Log("StartMenu saved: " + ScenePath + ". Other loaded scenes were preserved; open StartMenu alone for Play Mode.");
    }

    private static Vector2 FromTop(float x, float y) => new Vector2(x - 960f, 540f - y);

    private static RectTransform Rect(string name, Transform parent, Vector2 position, Vector2 size)
    {
        RectTransform rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return rect;
    }

    private static UnityEngine.UI.Image Image(string name, Transform parent, Vector2 position, Vector2 size, Sprite sprite, Color color)
    {
        UnityEngine.UI.Image image = Rect(name, parent, position, size).gameObject.AddComponent<UnityEngine.UI.Image>();
        image.sprite = sprite;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static TMP_Text Text(string name, Transform parent, string value, Vector2 position, Vector2 size, float fontSize)
    {
        TextMeshProUGUI text = Rect(name, parent, position, size).gameObject.AddComponent<TextMeshProUGUI>();
        text.font = font;
        text.text = value;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(1f, 0.93f, 0.77f);
        text.raycastTarget = false;
        return text;
    }

    private static UnityEngine.UI.Button Button(string name, Transform parent, string value, Vector2 position, Vector2 size)
    {
        UnityEngine.UI.Image image = Image(name, parent, position, size, buttonSprite, new Color(0.86f, 0.9f, 0.83f));
        image.type = UnityEngine.UI.Image.Type.Sliced;
        image.pixelsPerUnitMultiplier = 20f;
        image.raycastTarget = true;
        UnityEngine.UI.Button button = image.gameObject.AddComponent<UnityEngine.UI.Button>();
        button.targetGraphic = image;
        button.transition = UnityEngine.UI.Selectable.Transition.None;
        TMP_Text label = Text("Label", image.transform, value, Vector2.zero, size - new Vector2(36f, 12f), size.y >= 100f ? 56f : 48f);
        label.fontSharedMaterial = buttonFontMaterial;
        label.fontStyle = FontStyles.Bold;
        image.gameObject.AddComponent<StartMenuButtonFeedback>();
        return button;
    }

    private static UnityEngine.UI.Slider VolumeSlider(Transform panel)
    {
        RectTransform rect = Rect("VolumeSlider", panel, new Vector2(70f, 75f), new Vector2(330f, 48f));
        UnityEngine.UI.Slider slider = rect.gameObject.AddComponent<UnityEngine.UI.Slider>();
        UnityEngine.UI.Image track = Image("Track", rect, Vector2.zero, new Vector2(330f, 12f), null, new Color(0.2f, 0.3f, 0.32f));
        track.raycastTarget = true;
        RectTransform fillArea = Rect("FillArea", rect, Vector2.zero, new Vector2(310f, 12f));
        UnityEngine.UI.Image fill = Image("Fill", fillArea, Vector2.zero, new Vector2(310f, 12f), null, new Color(0.82f, 0.68f, 0.34f));
        RectTransform handleArea = Rect("HandleArea", rect, Vector2.zero, new Vector2(310f, 48f));
        UnityEngine.UI.Image handle = Image("Handle", handleArea, Vector2.zero, new Vector2(24f, 38f), null, new Color(1f, 0.88f, 0.55f));
        handle.raycastTarget = true;
        slider.fillRect = fill.rectTransform;
        slider.handleRect = handle.rectTransform;
        slider.targetGraphic = handle;
        slider.value = 1f;
        return slider;
    }

    private static UnityEngine.UI.Toggle FullscreenToggle(Transform panel)
    {
        RectTransform rect = Rect("FullscreenToggle", panel, new Vector2(0f, -30f), new Vector2(440f, 64f));
        UnityEngine.UI.Toggle toggle = rect.gameObject.AddComponent<UnityEngine.UI.Toggle>();
        UnityEngine.UI.Image box = Image("Box", rect, new Vector2(-160f, 0f), new Vector2(42f, 42f), null, new Color(0.26f, 0.35f, 0.36f));
        box.raycastTarget = true;
        UnityEngine.UI.Image check = Image("Check", box.transform, Vector2.zero, new Vector2(24f, 24f), null, new Color(1f, 0.88f, 0.55f));
        Text("Label", rect, "全屏显示", new Vector2(40f, 0f), new Vector2(280f, 64f), 36f);
        toggle.targetGraphic = box;
        toggle.graphic = check;
        return toggle;
    }

    private static Sprite ImportSprite(string path, bool multiple = false)
    {
        AssetDatabase.ImportAsset(path);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = multiple ? SpriteImportMode.Multiple : SpriteImportMode.Single;
        importer.filterMode = FilterMode.Point;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.alphaIsTransparency = true;
        importer.maxTextureSize = 4096;
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static Sprite MakeEffectSprite(string name, int width, int height, bool glow)
    {
        string path = Art + name;
        if (!File.Exists(path))
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                float alpha = glow
                    ? Mathf.Pow(Mathf.Clamp01(1f - Vector2.Distance(new Vector2(x, y), new Vector2((width - 1) * 0.5f, (height - 1) * 0.5f)) / (width * 0.5f)), 2f)
                    : (Mathf.Abs(x - 4) + Mathf.Abs(y - 3) <= 4 ? 1f : 0f);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
            texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
        }
        return ImportSprite(path);
    }

    private static Sprite MakeBeamSprite()
    {
        string path = Art + "StartMenu_Sunbeam.png";
        if (!File.Exists(path))
        {
            Texture2D texture = new Texture2D(24, 96, TextureFormat.RGBA32, false);
            for (int y = 0; y < 96; y++)
            for (int x = 0; x < 24; x++)
            {
                float horizontal = Mathf.Clamp01(1f - Mathf.Abs(x - 11.5f) / 12f);
                float vertical = Mathf.Clamp01(y / 20f) * Mathf.Clamp01((95f - y) / 10f);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, horizontal * vertical));
            }
            texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
        }
        return ImportSprite(path);
    }

    private static Sprite MakeArrowSprite()
    {
        string path = Art + "StartMenu_SelectionArrow.png";
        if (!File.Exists(path))
        {
            Texture2D texture = new Texture2D(7, 9, TextureFormat.RGBA32, false);
            for (int y = 0; y < 9; y++)
            for (int x = 0; x < 7; x++)
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, x <= 6 - Mathf.Abs(y - 4) * 1.5f ? 1f : 0f));
            texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
        }
        return ImportSprite(path);
    }
}
