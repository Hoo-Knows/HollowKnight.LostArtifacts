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

		public GameObject artifactsGO;
		public Artifact[] artifacts;
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

		public override string GetVersion() => AssemblyUtils.GetAssemblyVersionHash();

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
			artifacts = new Artifact[20];
			UnityEngine.Object.DontDestroyOnLoad(artifactsGO);

			//Create icons directory if it doesn't exist
			iconPath = Path.Combine(AssemblyUtils.getCurrentDirectory(), "Icons");
			IoUtils.EnsureDirectory(iconPath);

			AddArtifact<TravelersGarment>();
			AddArtifact<PavingStone>();
			AddArtifact<Tumbleweed>();
			AddArtifact<ChargedCrystal>();

			On.HeroController.Start += HeroControllerStart;
			On.HeroController.OnDisable += HeroControllerOnDisable;
			ModHooks.LanguageGetHook += LanguageGetHook;
			ModHooks.GetPlayerBoolHook += GetPlayerBoolHook;

			Log("Initialized");
		}

		private void HeroControllerStart(On.HeroController.orig_Start orig, HeroController self)
		{
			//Activate artifacts if they are equipped
			orig(self);
			foreach(Artifact artifact in artifacts)
			{
				if(artifact == null || artifact.active) continue;

				//Ensure that an artifact is locked/unlocked
				if(Settings.unlocked[artifact.ID()]) artifact.Unlock();

				//Activate artifact
				artifact.level = 0;
				if(Settings.slotHandle == artifact.ID()) artifact.level = 1;
				if(Settings.slotBladeL == artifact.ID()) artifact.level = 2;
				if(Settings.slotBladeR == artifact.ID()) artifact.level = 2;
				if(Settings.slotHead == artifact.ID()) artifact.level = 3;
				if(artifact.level > 0)
				{
					artifact.Activate();
				}
			}
		}

		private void HeroControllerOnDisable(On.HeroController.orig_OnDisable orig, HeroController self)
		{
			//Deactivate artifacts
			foreach(Artifact artifact in artifacts)
			{
				if(artifact == null || !artifact.active) continue;

				artifact.Deactivate();
			}
			orig(self);
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
			pageFSM.AddCustomAction("Artifacts", () => artifactsUI.SetActive(true));

			//Set inventory inactive
			pageFSM.InsertCustomAction("Move Pane L", () => CloseInventory(false), 0);
			pageFSM.InsertCustomAction("Move Pane R", () => CloseInventory(false), 0);
			GameManager.instance.inventoryFSM.InsertCustomAction("Close", () => CloseInventory(true), 0);
			GameManager.instance.inventoryFSM.InsertCustomAction("Damage Close", () => CloseInventory(true), 0);
			GameManager.instance.inventoryFSM.InsertCustomAction("R Lock Close", () => CloseInventory(true), 0);
		}

		private void CloseInventory(bool full)
		{
			artifactsUI.SetActive(false);
			//Needed if closing the inventory while on the Artifacts tab
			if(full) pageFSM.SetState("Init");
		}

		public void AddArtifact<T>() where T : Artifact
		{
			Artifact artifact = artifactsGO.AddComponent<T>();
			artifact.sprite = GetArtifactSprite(artifact.Name());
			artifact.unlocked = Settings.unlocked[artifact.ID()];
			artifacts[artifact.ID()] = artifact;
		}

		private Sprite GetArtifactSprite(string name)
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

		private void ExtractSprite(string name)
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