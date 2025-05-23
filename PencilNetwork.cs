using System.IO;
using ExitGames.Client.Photon;
using REPOLib.Modules;
using UnityEngine;
namespace RepoWebListener;

class PencilNetwork
{
    public class MissionOptions
    {
        public Color color1 { get; set; }
        public Color color2 { get; set; }
        public string msg { get; set; }
        public float time { get; set; }

        public static byte[] Serialize(object customObject)
        {
            MissionOptions options = (MissionOptions)customObject;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(options.msg);
                    writer.Write(options.color1.r);
                    writer.Write(options.color1.g);
                    writer.Write(options.color1.b);
                    writer.Write(options.color2.r);
                    writer.Write(options.color2.g);
                    writer.Write(options.color2.b);
                    writer.Write(options.time);
                }
                return stream.ToArray();
            }
        }
        public static object Deserialize(byte[] serializedCustomObject)
        {
            using (MemoryStream stream = new MemoryStream(serializedCustomObject))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                MissionOptions options = new MissionOptions();
                options.msg = reader.ReadString();
                options.color1 = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                options.color2 = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                options.time = reader.ReadSingle();
                return options;
            }
        }

    }
    public static NetworkedEvent NewChatterEvent;

    public static void InitNetworking()
    {
        NewChatterEvent = new NetworkedEvent("NewChatterEvent", HandleChatterEvent);
        // Register the event
        PhotonPeer.RegisterType(typeof(MissionOptions), 100, MissionOptions.Serialize, MissionOptions.Deserialize);
    }
    private static void HandleChatterEvent(EventData eventData)
    {
        MissionOptions options = (MissionOptions)eventData.CustomData;
        PencilUI instance = PencilUI.instance;
        if (instance != null)
        {
            instance.ShowEventText(
                options.msg,
                options.color1,
                options.color2,
                options.time
            );
        }
        else
        {
            RepoWebListener.Logger.LogError("PencilUI instance is null");
        }
    }
}