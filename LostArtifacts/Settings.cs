namespace LostArtifacts
{
	public class Settings
	{
		public int slotHandle { get; set; } = -1;
		public int slotBladeL { get; set; } = -1;
		public int slotBladeR { get; set; } = -1;
		public int slotHead { get; set; } = -1;
		public bool[] unlocked { get; set; } = new bool[21];
	}
}
