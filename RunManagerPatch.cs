using HarmonyLib;

namespace RepoWebListener;

[HarmonyPatch(typeof(RunManager))]
internal class RunManagerPatch
{
    [HarmonyPatch("ChangeLevel")]
    [HarmonyPostfix]
    private static void ChangeLevel_Postfix()
    {
        // Clear the old allowed items, valuables, and enemies
        RepoWebListener.AllowedItems.Clear();
        RepoWebListener.AllowedValuables.Clear();
        RepoWebListener.AllowedEnemies.Clear();
        // Set up the allowed items, valuables, and enemies
        RepoWebListener.AllowedItems = PencilUtils.GetAllowedItems();
        RepoWebListener.AllowedValuables = PencilUtils.GetAllowedValuables();
        RepoWebListener.AllowedEnemies = PencilUtils.GetAllowedEnemies();
    }
}