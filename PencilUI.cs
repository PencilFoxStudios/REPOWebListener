
namespace RepoWebListener;
// to maybe be used at some point, idk 
// not sure if I can find an alternative 
class PencilUI
{
    public static SemiUI eventLabel;

    public static int eventLabelTimer = PencilUtils.PencilConfig.MinimumTimeBetweenEvents / 2;
    public static void ResetTimer()
    {
        eventLabelTimer = PencilUtils.PencilConfig.MinimumTimeBetweenEvents / 2;
    }

    public static void Init()
    {

    }
    public static void UpdateEventLabel(string text)
    {

    }
}