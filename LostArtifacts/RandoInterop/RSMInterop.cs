using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;

namespace LostArtifacts.Rando
{
	internal static class RSMInterop
	{
		public static void Hook()
		{
			RandoSettingsManagerMod.Instance.RegisterConnection(new ArtifactRandoSettings());
		}
	}

	internal class ArtifactRandoSettings : RandoSettingsProxy<RandoSettings, string>
	{
		public override string ModKey => LostArtifacts.Instance.GetName();

		public override VersioningPolicy<string> VersioningPolicy { get; }
			= new EqualityVersioningPolicy<string>(LostArtifacts.Instance.GetVersion());

		public override void ReceiveSettings(RandoSettings settings)
		{
			if(settings != null)
			{
				RandoMenu.Apply(settings);
			}
			else
			{
				RandoMenu.Disable();
			}
		}

		public override bool TryProvideSettings(out RandoSettings settings)
		{
			settings = LostArtifacts.RandoSettings;
			return settings.EnableArtifacts;
		}
	}
}