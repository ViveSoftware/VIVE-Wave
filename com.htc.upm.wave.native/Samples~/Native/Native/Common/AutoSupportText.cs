using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Wave.Native;

// Auto create text and owner
public class AutoSupportText : MonoBehaviour {

    [Tooltip("Enter #[.#[.#]]\nEmpty to be ignored"), SerializeField]
    public string SDKVersion = "";

    [Tooltip("Enter #\nZero to be ignored"), Range(0, 10), SerializeField]
    private int APILevel = 0;

    [Tooltip("Enter ####[.#[.#]]\nEmpty to be ignored"), SerializeField]
    private string UnityVersion = "";

    [Tooltip("Enter Owner's team\nEmpty to be ignored"), SerializeField]
    private string Owner = "EngineTeam";

    [Tooltip("Color of font"), SerializeField]
    private Color color = Color.black;

    private readonly string TitleSDK = "SDKVersion";
    private readonly string TitleAPI = "APILevel";
    private readonly string TitleUnity = "UnityVersion";
    private readonly string TitleOwner = "UnityVersion";
    private readonly string TitleCanDoTest = "CanDoTest";

    private readonly string StrSupportedSDK = "Supported SDK version: ";
    private readonly string StrSupportedAPI = "Supported API level: ";
    private readonly string StrSupportedUnity = "Supported Unity version: ";
    private readonly string StrOwner = "Function Owner: ";
    private readonly string StrCanDoTest = "CanDoTest: ";

    private bool canDoTest = true;
    private string reason = "";

    private Text TextSDKVersion;
    private Text TextAPILevel;
    private Text TextUnityVersion;
    private Text TextOwner;

    // This function can help compare version in xxx.xxx.xxx format. Return true if expected version is less or equal.
    static bool VersionCompare(string currentVersion, string expectedVersion)
    {
        char[] pattern = { '.', 'f', 'p' };
        // compare year
        string[] currentVersions = currentVersion.Split(pattern);
        int currentVersionsCount = currentVersions.Length;

        string[] expectedVersions = expectedVersion.Split(pattern);
        int expectedVersionsCount = expectedVersions.Length;

        bool equal = false;
        int compareLevels = Mathf.Min(currentVersionsCount, expectedVersionsCount);
        for (int i = 0; i < compareLevels; i++)
        {
            int uV, inV;
            if (!int.TryParse(currentVersions[i], out uV))
                return false;  // Not a number
            if (!int.TryParse(expectedVersions[i], out inV))
                return false;  // Not a number

            if (inV < uV)
                return true;
            if (inV == uV)
            {
                equal = true;
                continue;
            }
            if (inV > uV)
                return false;
        }
        return equal;
    }

    Text CreateText(string name, Transform parent, string content)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        var text = obj.AddComponent<Text>();
        text.text = content;
        text.font = Font.CreateDynamicFontFromOSFont("Arial", 8);
        text.color = color;
        text.fontSize = 8;
        text.resizeTextMinSize = 0;
        text.resizeTextMaxSize = 40;
        text.resizeTextForBestFit = true;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        return text;
    }

    public class LayoutObject
    {
        public float proportion = 1.0f;
        public object target;

        public T GetTarget<T>()
        {
            return (T)target;
        }
    }

    public class GraphicLayoutObject : LayoutObject
    {
        public GraphicLayoutObject(Graphic target, float proportion = 1.0f)
        {
            this.target = target;
            this.proportion = proportion;
        }
    }


    // Assume all element will fill all width and share height equally.
    public class VerticalLayout
    {
        private List<LayoutObject> list = new List<LayoutObject>();

        public VerticalLayout()
        {
        }

        public void Add(Graphic element, float proportion = 1.0f)
        {
            if (element != null)
                list.Add(new GraphicLayoutObject(element, proportion));
        }

        public void Apply()
        {
            float heightTotal = 0;
            for (int i = 0; i < list.Count; i++)
            {
                heightTotal += list[i].proportion;
            }

            float heightScale = 1.0f / heightTotal;
            float heightAcc = 1; // top down
            for (int i = 0; i < list.Count; i++)
            {
                var graphic = list[i].GetTarget<Graphic>();
                if (graphic != null)
                {
                    var rect = graphic.rectTransform;
                    rect.pivot = new Vector2(0.5f, 0.5f);
                    rect.offsetMax = Vector2.zero;
                    rect.offsetMin = Vector2.zero;
                    rect.anchorMax = new Vector2(1, heightAcc);
                    var height = heightScale * list[i].proportion;
                    rect.anchorMin = new Vector2(0, heightAcc - height);
                    heightAcc -= height;
                    rect.ForceUpdateRectTransforms();
                    graphic.SetLayoutDirty();
                }
            }
        }
    }

    VerticalLayout verticalLayout = new VerticalLayout();

    void CreateTexts()
    {
        if (!string.IsNullOrEmpty(SDKVersion))
            TextSDKVersion = CreateText(TitleSDK, transform, StrSupportedSDK + SDKVersion + " or higher");
        if (APILevel > 0)
            TextAPILevel = CreateText(TitleAPI, transform, StrSupportedAPI + APILevel + " or higher");
        if (!string.IsNullOrEmpty(UnityVersion))
            TextUnityVersion = CreateText(TitleUnity, transform, StrSupportedUnity + UnityVersion + " or higher");
        if (!string.IsNullOrEmpty(Owner))
            TextOwner = CreateText(TitleOwner, transform, StrOwner + Owner);

        verticalLayout.Add(TextSDKVersion);
        verticalLayout.Add(TextAPILevel);
        verticalLayout.Add(TextUnityVersion);
        verticalLayout.Add(TextOwner);
        verticalLayout.Add(CreateText(TitleCanDoTest, transform, StrCanDoTest + (canDoTest ? "Yes" : "No")));
        if (!canDoTest)
            verticalLayout.Add(CreateText("Reason", transform, "Reason: " + reason), 3);

        verticalLayout.Apply();
    }

    void Start() {
#if UNITY_EDITOR
        if (!Application.isEditor)
            return;
#endif

        var sb = new StringBuilder();

        // TODO  No way to get SDK Version in #.#.# format
        //if (!string.IsNullOrEmpty(SDKVersion) && VersionCompare("3.1.4", SDKVersion))
        //{
        //    sb.Append("SDK Version(" + "3.1.4" + ") is lower than expect. ");
        //    canDoTest = false;
        //}

        uint apiLevelRuntime = Interop.WVR_GetWaveRuntimeVersion();
        uint apiLevelClient = Interop.WVR_GetWaveSDKVersion();
        if (APILevel != 0 && APILevel > apiLevelClient)
        {
            sb.Append("Client API level(" + apiLevelClient + ") is lower than expect. ");
            canDoTest = false;
        }

        if (apiLevelRuntime != 0 && APILevel > apiLevelRuntime)
        {
            sb.Append("Runtime API level(" + apiLevelRuntime + ") is lower than expect. ");
            canDoTest = false;
        }

        if (!string.IsNullOrEmpty(UnityVersion) && !VersionCompare(Application.unityVersion, UnityVersion))
        {
            sb.Append("Unity player version(" + UnityVersion + ") is lower than expect. ");
            canDoTest = false;
        }

        reason = sb.ToString();
        CreateTexts();
    }
}
