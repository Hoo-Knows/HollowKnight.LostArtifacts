namespace LostArtifacts
{
	public class Settings
	{
		public int[] slots { get; set; } = { -1, -1, -1, -1 };
		public int overchargedSlot { get; set; } = -1;
		public bool unlockedSlots { get; set; } = false;
		public bool[] unlocked { get; set; } = new bool[21];
	}
}
