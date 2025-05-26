using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using static RepoWebListener.PencilUtils;
namespace RepoWebListener;

[BepInPlugin("PencilFoxStudios.RepoWebListener", "RepoWebListener", "1.0.7")]
[BepInDependency(REPOLib.MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
public class RepoWebListener : BaseUnityPlugin
{
    internal static RepoWebListener Instance { get; private set; } = null!;
    public new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }
    static HttpListener listener = new HttpListener();
    string url = "http://localhost";
    private CancellationTokenSource cts = new CancellationTokenSource();
    public static Dictionary<string, string> AllowedItems = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public static Dictionary<string, string> AllowedValuables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public static Dictionary<string, EnemySetup> AllowedEnemies = new Dictionary<string, EnemySetup>(StringComparer.OrdinalIgnoreCase);
    private static Dictionary<string, EnemySetup> enemySetups = new Dictionary<string, EnemySetup>();

    private void Awake()
    {
        Instance = this;
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;
        Patch();
        PencilUtils.Initialize(new RepoWebListenerConfigActivator(Config));
        url = $"http://{PencilConfig.WebServerListenIP}:{PencilConfig.WebServerListenPort}/";
        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
        if (PencilConfig.WebServerEnabled)
        {
            Logger.LogInfo("Starting web server...");
            listener.Prefixes.Add(url);
            listener.Start();
            Logger.LogInfo("Listening on " + url);
        }
        else
        {
            Logger.LogInfo("Web server is disabled. Check the config file to enable it.");
        }


        // Set up the listener to handle requests
        Task.Run(() => ListenLoop(cts.Token));
        Task.Run(() => GoThroughChatters(cts.Token));
    }
    private void Start()
    {
        Logger.LogInfo("RepoWebListener is ready!");

        Events.Init();
        
    }

    private async Task ListenLoop(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HandleRequest(context);
                // apparently threading in unity is illegal
                // unity sucks
                // _ = Task.Run(() => HandleRequest(context)); // handle each request separately

            }
        }
        catch (HttpListenerException ex)
        {
            Logger.LogWarning($"Listener stopped: {ex.Message}");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error in ListenLoop: {ex}");
        }
    }




    private async Task GoThroughChatters(CancellationToken token)
    {

        while (!token.IsCancellationRequested)
        {
            if (Events.EventQueue.Count == 0 ||
            !SemiFunc.IsMultiplayer() ||
             IsBlacklistedLevel() // Check if the current level is blacklisted
            || (LevelGenerator.Instance.Generated == false) // Check if the level is generated
            || (RoundDirector.instance.extractionPointsCompleted == 0 &&
            (!RoundDirector.instance.extractionPointActive)) // Check if the players haven't left truck yet

            )
            {

                await Task.Delay(1000, token); // Wait for 1 second if there are no chatters or
                // Logger.LogInfo("Waiting for the time to be right...");
                                               // not in multiplayer
                continue;
            }
            Events.RunNextEvent();



            // Delay for at least 3 seconds before processing the next chatter
            await Task.Delay(Math.Max(PencilConfig.MinimumTimeBetweenEvents, 3)*1000, token);
        }


    }



    internal void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();
    }

    internal void Unpatch()
    {
        Harmony?.UnpatchSelf();
    }

    private void OnDestroy()
    {
        cts.Cancel();

        if (listener.IsListening)
        {
            listener.Stop();
            listener.Close();
        }

        Unpatch();
    }

    private void HandleRequest(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;
        Logger.LogInfo($"Received {request.HttpMethod} request");


        // GET Request
        if (request.HttpMethod == "GET")
        {
            // get ?username=chatter parameter
            string requestBody = request.QueryString["username"];
            // Respond back
            HttpListenerResponse response = context.Response;
            string responseString = Events.AddEventToQueueFrom(requestBody);
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
            Logger.LogInfo($"Chatters in queue: {Events.EventQueue.Count}");
        }
        else
        {
            context.Response.StatusCode = 405;
            context.Response.Close();
        }
    }
    private void Update()
    {
        // Code that runs every frame goes here

    }
}