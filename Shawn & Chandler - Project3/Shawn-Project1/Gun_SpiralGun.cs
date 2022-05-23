//Shawn Carter

using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using System.Collections.Generic;
using System.IO;
using Sce.PlayStation.Core.Audio;

namespace ShawnProject1
{
	public class SpiralGun : Gun
	{
		private static Sound gunshot = new Sound(@"\Application\assets\sound effect\SUPER GUN.wav");
		private static SoundPlayer player = gunshot.CreatePlayer();
		
		private int splitDelay, splitDamage, splitDuration;
		private float splitAngle;
		
		/// <summary>
		/// Default Spiral Gun
		/// </summary>
		public SpiralGun(GraphicsContext graphics, Vector3 position) 
			: base(new Texture2D(@"\Application\assets\images\guns\spiralgun.png", false), graphics, position)
		{
			Graphics = graphics;
			Name = "Spiral Gun";
			Damage = 5;
			BurstCount = 0;
			CurrentBurst = 0;
			Ammo = 40;
			AmmoMax = 40;
			Angle = 0;
			Accuracy = .1f;
			splitAngle = .5f;
			splitDamage = 3;
			splitDelay = 3;
			BurstDelay = new Timer(60);
			RateOfFire = new Timer(60);
			BulletTexture = new Texture2D(@"\Application\assets\images\bullets\556bullet.png", false);
			CasingTexture = new Texture2D(@"\Application\assets\images\particles\556casing.png", false);
			Status = GunStatus.CanShoot;
			FireType = FireType.BoltAction;
		}
		
		
		private SpiralGun(Texture2D texture, GraphicsContext graphics, Vector3 position, string name, int damage, 
		                  int burstCount, int ammoMax, float accuracy, int burstDelay, int rateOfFire, FireType fireType, 
		                  int splitDelay, int splitDamage, int splitDuration, float splitAngle, 
		                  Texture2D bulletTexture, Texture2D casingTexture) 
			: base(texture, graphics, position)
		{
			Graphics = graphics;
			Name = name;
			Damage = damage;
			BurstCount = burstCount;
			CurrentBurst = 0;
			Ammo = ammoMax;
			AmmoMax = ammoMax;
			Angle = 0;
			Accuracy = accuracy;
			BurstDelay = new Timer(burstDelay);
			RateOfFire = new Timer(rateOfFire);
			FireType = fireType;
			this.splitAngle = splitAngle;
			this.splitDamage = splitDamage;
			this.splitDuration = splitDuration;
			this.splitDelay = splitDelay;
			BulletTexture = bulletTexture;
			CasingTexture = casingTexture;
			Status = GunStatus.CanShoot;
		}
		
		public override GameObject[] Update ()
		{
			List<GameObject> items = new List<GameObject>();
			
			switch (Status)
			{
			case GunStatus.Waiting:
				BurstDelay.Update();
				if (BurstDelay.Status == TimerStatus.JustCompleted)
				{
					if (FireType == FireType.BoltAction)
					{
						Status = GunStatus.CanShoot;
						float casingAngle = (float)(Angle + FMath.PI / 2 - FMath.PI / 6 + (float)Ran.NextDouble() * Math.PI / 3);
						Particle casing = new Particle(CasingTexture, Graphics, Position, 120, Angle + FMath.PI / 2, casingAngle, 
				                               Ran.Next(3, 7), (float)Ran.NextDouble() / 4 + .05f, (float)Ran.NextDouble() / 3, 
				                               (float)Ran.NextDouble() / 50);
						items.Add(casing);
					}
					BurstDelay.Reset();
				}
				break;
			case GunStatus.Shooting:
				if (CurrentBurst < BurstCount)
				{
					Status = GunStatus.Bursting;
					RateOfFire.Reset();
				}
				else
				{
					CurrentBurst = 0;
					Status = GunStatus.Waiting;
				}
				break;
			case GunStatus.Bursting:
				RateOfFire.Update();
				if (RateOfFire.Status == TimerStatus.JustCompleted)
				{
					Status = GunStatus.Shooting;
					items.AddRange(Shoot(Angle));
				}
				break;
			}
			
			return items.ToArray();
		}
		
		public override GameObject[] Update (Vector3 position, float angle)
		{
			this.Position = position;
			this.Angle = angle;
			
			return Update();
		}
		
		public override GameObject[] Shoot (float angle)
		{
			List<GameObject> items = new List<GameObject>();
			
			if (Status != GunStatus.OutOfAmmo)
			{
				Angle = angle;
				CurrentBurst++;
				Status = GunStatus.Shooting;
				
				float bulletAngle = Angle + (float)Ran.NextDouble() * Accuracy - Accuracy / 2;
				Bullet bullet = new SpiralBullet(BulletTexture, Graphics, Position, bulletAngle, 7.5f, Damage, 1800, 
				                                 splitDelay, splitDamage, splitDuration, splitAngle);
				
				items.Add(bullet);
				
				if (FireType != FireType.BoltAction)
				{
					Status = GunStatus.CanShoot;
					float casingAngle = (float)(Angle + FMath.PI / 2 - FMath.PI / 6 + (float)Ran.NextDouble() * Math.PI / 3);
					Particle casing = new Particle(CasingTexture, Graphics, Position, 120, Angle + FMath.PI / 2, casingAngle, 
			                               Ran.Next(3, 7), (float)Ran.NextDouble() / 4 + .05f, (float)Ran.NextDouble() / 3, 
			                               (float)Ran.NextDouble() / 50);
					items.Add(casing);
				}
				
				Ammo--;
				player.Play();
				if (Ammo <= 0)
					Status = GunStatus.OutOfAmmo;
			}
			
			return items.ToArray();
		}
		
		public static Gun Create(GraphicsContext graphics, string filePath)
		{
			using (StreamReader reader = new StreamReader(filePath))
			{
				string[] info = new string[16];
				for (int i = 0; i < info.Length; i++)
				{
					string line;
					do
					{
						line = reader.ReadLine();
					}while (line.Contains("###"));
					info[i] = line;
				}
				
				FireType fireType = FireType.BoltAction;
				switch (info[8])
				{
				case "SemiAuto":
					fireType = FireType.SemiAuto;
					break;
				case "Burst":
					fireType = FireType.Burst;
					break;
				}
				Texture2D bulletTexture = new Texture2D(info[13], false), 
					casingTexture = new Texture2D(info[14], false), 
					groundTexture = new Texture2D(info[15], false);
				
				return new SpiralGun(groundTexture, graphics, Vector3.Zero, info[1], int.Parse(info[2]), 
				                     int.Parse(info[3]), int.Parse(info[4]), float.Parse(info[5]), 
				                     int.Parse(info[6]), int.Parse(info[7]), fireType, int.Parse(info[9]), 
				                     int.Parse(info[10]), int.Parse(info[11]), float.Parse(info[12]), 
				                     bulletTexture, casingTexture);
			}
			
			return new SpiralGun(graphics, Vector3.Zero);
		}
	}
}

