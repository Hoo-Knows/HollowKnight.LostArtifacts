using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFCore;
using Satchel;
using System.IO;
using System.Text.RegularExpressions;

namespace LostArtifacts
{
	public class LostArtifacts : Mod, ILocalSettings<Settings>
	{
		public static LostArtifacts Instance;

		private GameObject prefabUI;
		private string iconPath;
		private UI.ArtifactManager artifactManager;

		public GameObject artifactsGO;
		public GameObject artifactsUI;
		public PlayMakerFSM pageFSM;

		public static Settings Settings { get; set; } = new Settings();
		public void OnLoadLocal(Settings s)
		{
			Log("Loading Lost Artifacts settings");
			Settings = s;
		}
		public Settings OnSaveLocal()
		{
			Log("Saving Lost Artifacts settings");
			return Settings;
		}

		public override string GetVersion() => "1.1.1b";

		public LostArtifacts() : base("LostArtifacts")
		{
			Instance = this;

			//Load UI prefab
			AssetBundle assetBundle = AssetBundle.LoadFromStream(
				typeof(LostArtifacts).Assembly.GetManifestResourceStream("LostArtifacts.Resources.artifactsui"));
			if(assetBundle == null)
			{
				Log("Failed to load AssetBundle!");
				return;
			}
			prefabUI = assetBundle.LoadAsset<GameObject>("Artifacts");

			artifactsUI = UnityEngine.Object.Instantiate(prefabUI);
			GameObject.DontDestroyOnLoad(artifactsUI);
			artifactsUI.SetActive(false);

			//Add inventory page with SFCore
			InventoryHelper.AddInventoryPage(
				InventoryPageType.Empty,
				"LostArtifacts.Artifacts",
				"LostArtifacts.PageConvKey",
				"Artifacts",
				"artifactsUnlocked",
				EditInventory);
		}

		public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
		{
			Log("Initializing");

			//Add artifacts
			artifactsGO = new GameObject("Artifacts GO");
			UnityEngine.Object.DontDestroyOnLoad(artifactsGO);

			//Create icons directory if it doesn't exist
			iconPath = Path.Combine(AssemblyUtils.getCurrentDirectory(), "Icons");
			IoUtils.EnsureDirectory(iconPath);

			artifactManager = artifactsUI.GetComponentInChildren<UI.ArtifactManager>();
			artifactManager.AddArtifact<TravelersGarment>();
			artifactManager.AddArtifact<Tumbleweed>();
			artifactManager.AddArtifact<ChargedCrystal>();

			On.HeroController.Start += HeroControllerStart;
			On.QuitToMenu.Start += QuitToMenuStart;
			ModHooks.LanguageGetHook += LanguageGetHook;
			ModHooks.GetPlayerBoolHook += GetPlayerBoolHook;

			Log("Initialized");
		}

		private void HeroControllerStart(On.HeroController.orig_Start orig, HeroController self)
		{
			orig(self);
			//Activate artifacts if they are equipped
			foreach(Artifact artifact in artifactManager.artifacts)
			{
				if(artifact == null || artifact.active) continue;

				artifact.level = 0;
				if(Settings.slotHandle == artifact.id) artifact.level = 1;
				if(Settings.slotBladeL == artifact.id) artifact.level = 2;
				if(Settings.slotBladeR == artifact.id) artifact.level = 2;
				if(Settings.slotHead == artifact.id) artifact.level = 3;
				if(artifact.level > 0)
				{
					artifact.active = true;
					artifact.Activate();
				}
			}
		}

		private IEnumerator QuitToMenuStart(On.QuitToMenu.orig_Start orig, QuitToMenu self)
		{
			orig(self);
			foreach(Artifact artifact in artifactManager.artifacts)
			{
				if(artifact == null || !artifact.active) continue;

				artifact.Deactivate();
				artifact.level = 0;
				artifact.active = false;
			}
			yield break;
		}

		private void EditInventory(GameObject page)
		{
			//Cache FSM
			pageFSM = page.LocateMyFSM("Empty UI");

			//Create state
			pageFSM.AddState("Artifacts");

			//Change to state after init
			pageFSM.ChangeTransition("Init Heart Piece", "FINISHED", "Artifacts");

			//Add left and right arrow transitions out of the inventory
			pageFSM.AddTransition("Artifacts", "LEFT", "Move Pane L");
			pageFSM.AddTransition("Artifacts", "RIGHT", "Move Pane R");

			//Set inventory active
			pageFSM.AddCustomAction("Artifacts", () =>
			{
				artifactsUI.SetActive(true);
			});

			//Set inventory inactive
			pageFSM.InsertCustomAction("Move Pane L", () => artifactsUI.SetActive(false), 0);
			pageFSM.InsertCustomAction("Move Pane R", () => artifactsUI.SetActive(false), 0);
			GameManager.instance.inventoryFSM.InsertCustomAction("Close", () =>
			{
				artifactsUI.SetActive(false);
				//Needed or else the UI will break when closing the inventory while on the artifacts tab
				pageFSM.SetState("Init");
			}, 0);
		}

		public Sprite GetArtifactSprite(string name)
		{
			Texture2D texture = TextureUtils.createTextureOfColor(64, 64, Color.clear);

			name = Regex.Replace(name, @"[^0-9a-zA-Z\._]", "");
			string path = Path.Combine(iconPath, name + ".png");
			//Extract sprite from Resources if it doesn't exist
			if(!File.Exists(path))
			{
				try
				{
					ExtractSprite(name);
				}
				catch
				{
					Log("Failed to extract sprite for " + name);
				}
			}
			if(File.Exists(path)) texture = TextureUtils.LoadTextureFromFile(path);

			return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 64f);
		}

		public void ExtractSprite(string name)
		{
			string path = Path.Combine(iconPath, name + ".png");

			Stream stream = typeof(LostArtifacts).Assembly.GetManifestResourceStream("LostArtifacts.Resources." + name + ".png");
			{
				var buffer = new byte[stream.Length];
				stream.Read(buffer, 0, buffer.Length);
				File.WriteAllBytes(path, buffer);
				stream.Dispose();
			}
		}

		private string LanguageGetHook(string key, string sheetTitle, string orig)
		{
			if(key == "LostArtifacts.PageConvKey") return "Artifacts";
			return orig;
		}

		private bool GetPlayerBoolHook(string name, bool orig)
		{
			if(name == "artifactsUnlocked") return PlayerData.instance.GetInt("nailSmithUpgrades") > 0;
			return orig;
		}
	}
}