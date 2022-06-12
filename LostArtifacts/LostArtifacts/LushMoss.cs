﻿using System;

namespace LostArtifacts
{
	public class LushMoss : Artifact
	{
		public override int ID() => 2;
		public override string Name() => "Lush Moss";
		public override string Description() => "This piece of moss came from the Massive Moss Charger. It has a mysterious " +
			"healing power that allows the moss chargers to recover their moss no matter how many times it is destroyed.";
		public override string Levels() => "40, 30, 20 hits";
		public override string TraitName() => "Nature’s Aura";
		public override string TraitDescription() => "Heal a mask after a certain amount of hits";

		private int counter;
		private int hitsNeeded;

		public override void Activate()
		{
			base.Activate();

			counter = 0;
			hitsNeeded = 50 - level * 10;

			On.HealthManager.Hit += HealthManagerHit;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				counter++;
				if(counter >= hitsNeeded)
				{
					HeroController.instance.AddHealth(1);
					counter = 0;
				}
			}
			orig(self, hitInstance);
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
		}
	}
}