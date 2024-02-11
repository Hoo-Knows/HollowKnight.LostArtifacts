namespace LostArtifacts.Rando
{
	public class RandoSettings
	{
		public bool EnableArtifacts { get; set; } = true;
		public bool RandomizeArtifacts { get; set; } = true;
		public bool UseCustomLocations { get; set; } = true;

		[MenuChanger.Attributes.MenuRange(-1, 99)]
		public int ArtifactGroup { get; set; } = -1;
	}
}
