using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Satchel;
using HutongGames.PlayMaker.Actions;

namespace LostArtifacts
{
	public class NoxiousShroom : Artifact
	{
		public override int ID() => 3;
		public override string Name() => "Noxious Shroom";
		public override string Description() => "Found only in the deepest recesses of the Fungal Wastes, these mushrooms " +
			"have highly concentrated toxins. Releasing them would be disastrous…for the enemy.";
		public override string Levels() => "1, 2, 3 chances to spread";
		public override string TraitName() => "Toxic";
		public override string TraitDescription() => "Hitting an enemy releases a spore cloud that can spread to nearby enemies";

		private GameObject cloudGO;

		public override void Activate()
		{
			base.Activate();

			foreach(var pool in ObjectPool.instance.startupPools)
			{
				if(pool.prefab.name == "Knight Spore Cloud")
				{
					cloudGO = pool.prefab;
					break;
				}
			}

			On.HealthManager.Hit += HealthManagerHit;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				StartCoroutine(CloudControl(self, new List<string>()));
			}
			orig(self, hitInstance);
		}

		private IEnumerator CloudControl(HealthManager hm, List<string> enemies)
		{
			string name = hm.gameObject.name;
			Vector3 pos = hm.transform.position;

			if(enemies.Contains(name) || enemies.Count > level) yield break;
			enemies.Add(name);

			//Spawn cloud
			GameObject cloud = Instantiate(cloudGO, pos, Quaternion.identity);

			cloud.LocateMyFSM("Control").GetAction<SetScale>("Normal", 1).x = 0.5f;
			cloud.LocateMyFSM("Control").GetAction<SetScale>("Normal", 1).y = 0.5f;
			cloud.LocateMyFSM("Control").GetAction<SetScale>("Deep", 1).x = 0.5f;
			cloud.LocateMyFSM("Control").GetAction<SetScale>("Deep", 1).y = 0.5f;

			cloud.SetActive(true);
			cloud.GetComponent<DamageEffectTicker>().damageInterval = 0.25f;

			//Wait a bit to spread
			yield return new WaitForSeconds(2f);
			HealthManager next = FindNearestHealthManager(pos, name);
			if(next != null)
			{
				StartCoroutine(CloudControl(next, enemies));
			}
			yield break;
		}

		private HealthManager FindNearestHealthManager(Vector3 pos, string targetName)
		{
			List<HealthManager> hms = new List<HealthManager>(GameObject.FindObjectsOfType<HealthManager>());

			for(int i = 0; i < hms.Count; i++)
			{
				if(hms[i].name == targetName) hms.RemoveAt(i);
			}

			if(hms.Count == 0) return null;

			HealthManager next = hms[0];
			foreach(HealthManager hm in hms)
			{
				if(Vector3.Distance(hm.transform.position, pos) < Vector3.Distance(next.transform.position, pos) &&
					Vector3.Distance(hm.transform.position, pos) < 10f)
				{
					next = hm;
				}
			}
			return next;
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
			StopAllCoroutines();
		}
	}
}
