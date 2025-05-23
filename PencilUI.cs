
using HarmonyLib;
using TMPro;
using UnityEngine;
namespace RepoWebListener;
// to maybe be used at some point, idk 
// not sure if I can find an alternative 
class PencilUI : SemiUI
{

    internal TextMeshProUGUI Text;

    public static PencilUI instance;

    private string messagePrev = "prev";

    private Color bigMessageColor = Color.white;

    private Color bigMessageFlashColor = Color.white;

    private float messageTimer;
    private void Awake()
    {
        RepoWebListener.Logger.LogInfo("PencilUI Awake");
    }
    public override void Start()
	{
		base.Start();
		Text = GetComponent<TextMeshProUGUI>();
		instance = this;
		Text.text = "";
	}
    [HarmonyPatch(typeof(MapToolController))]
    internal class MapToolControllerPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void Update_Postfix(MapToolController __instance)
        {
           	if (__instance.Active)
			{
				if (instance.Text.text != "")
				{
					instance.Show();
				}
			}
        }
    }
    [HarmonyPatch(typeof(SpectateCamera))]
    internal class SpectateCameraPatch
    {
        [HarmonyPatch("LateUpdate")]
        [HarmonyPostfix]
        private static void LateUpdate_Postfix()
        {
            instance.Show();
        }
    }
    public void ShowEventText(string message, Color colorMain, Color colorFlash, float time = 3f)
    {
        RepoWebListener.Logger.LogInfo($"Message: {message} (0)");
        if (messageTimer <= 0f)
        {
            bigMessageColor = colorMain;
            bigMessageFlashColor = colorFlash;
            messageTimer = time;
            RepoWebListener.Logger.LogInfo($"Message: {message} (1)");
            if (message != messagePrev)
            {
                RepoWebListener.Logger.LogInfo($"Message: {message} (2)");
                Text.fontMaterial.SetColor(ShaderUtilities.ID_FaceColor, bigMessageColor);
                Text.fontMaterial.SetColor(ShaderUtilities.ID_GlowColor, bigMessageColor);
                Text.color = bigMessageColor;
                Text.text = message;
                SemiUISpringShakeY(20f, 10f, 0.3f);
                SemiUITextFlashColor(bigMessageFlashColor, 0.2f);
                SemiUISpringScale(0.4f, 5f, 0.2f);
                messagePrev = message;
            }
        }
    }

    public override void Update()
    {
        base.Update();
        if (messageTimer > 0f)
        {
            messageTimer -= Time.deltaTime;
            return;
        }
        messagePrev = "prev";
        Hide();
    }
}