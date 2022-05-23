//Shawn Carter

using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using System.Collections.Generic;
using System.IO;
using Sce.PlayStation.Core.Audio;

namespace ShawnProject1
{
	[Serializable]
	public class HandGun : Gun
	{	
		private static Sound gunshot = new Sound(@"\Application\assets\sound effect\machine gun 1.wav");
		private static SoundPlayer player = gunshot.CreatePlayer();
		
		Timer reload;
		
		public Timer Reload
		{
			get { return reload; }
		}
		
		/// <summary>
		/// Default Handgun
		/// </summary>
		public HandGun(GraphicsContext graphics, Vector3 position) 
			: base(new Texture2D(@"\Application\assets\images\guns\assaultrifle.png", false), graphics, position)
		{
			Graphics = graphics;
			Name = "Handgun";
			Damage = 4;
			BurstCount = 0;
			CurrentBurst = 0;
			Ammo = 20;
			AmmoMax = Ammo;
			Angle = 0;
			Accuracy = .35f;
			BurstDelay = new Timer(1);
			RateOfFire = new Timer(1);
			reload = new Timer(60);
			BulletTexture = new Texture2D(@"\Application\assets\images\bullets\9mmbullet.png", false);
			CasingTexture = new Texture2D(@"\Application\assets\images\particles\9mmcasing.png", false);
			Status = GunStatus.CanShoot;
			FireType = FireType.SemiAuto;
			sprite.Position = position;
		}
		
		public HandGun(Texture2D texture, GraphicsContext graphics, Vector3 position, string name, int damage, 
		                int burstCount, int ammoMax, float accuracy, int burstDelay, int rateOfFire, int reload,
		                FireType fireType, Texture2D bulletTexture, Texture2D casingTexture) 
			: base(texture, graphics, position)
		{
			Graphics = graphics;
			Name = name;
			Damage = damage;
			BurstCount = burstCount;
			Ammo = ammoMax;
			AmmoMax = ammoMax;
			Accuracy = accuracy;
			BurstDelay = new Timer(burstDelay);
			RateOfFire = new Timer(rateOfFire);
			this.reload = new Timer(reload);
			FireType = fireType;
			BulletTexture = bulletTexture;
			CasingTexture = casingTexture;
			Status = GunStatus.CanShoot;
		}
		
		public override GameObject[] Update ()
		{
			
			List<GameObject> items = new List<GameObject>();
			reload.Update();
			if (reload.Status == TimerStatus.JustCompleted)
			{
				reload.Reset();
				Ammo = AmmoMax;
				Status = GunStatus.CanShoot;
			}
			
			
			switch (Status)
			{
			case GunStatus.Waiting:
				BurstDelay.Update();
				if (BurstDelay.Status == TimerStatus.JustCompleted)
				{
					Status = GunStatus.CanShoot;
					BurstDelay.Reset();
				}
				break;
			case GunStatus.Shooting:
				if (FireType == FireType.Burst)
					{
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
				}
				else 
					Status = GunStatus.Waiting;
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
			Position = position;
			Angle = angle;
			
			return Update();
		}
		
		public override GameObject[] Shoot (float angle)
		{
			List<GameObject> items = new List<GameObject>();
			
			if (Status != GunStatus.OutOfAmmo)
			{
				Angle = angle;
				CurrentBurst++;
				reload.Reset();
				Status = GunStatus.Shooting;
				
				float casingAngle = (float)(angle + FMath.PI / 2 - FMath.PI / 6 + (float)Ran.NextDouble() * Math.PI / 3);
				Particle casing = new Particle(CasingTexture, Graphics, Position, 120, Angle + FMath.PI / 2, casingAngle, 
				                               Ran.Next(3, 7), (float)Ran.NextDouble() / 4 + .05f, (float)Ran.NextDouble() / 3, 
				                               (float)Ran.NextDouble() / 50);
				
				float bulletAngle = angle + (float)Ran.NextDouble() * Accuracy - Accuracy / 2;
				Bullet bullet = new Bullet(BulletTexture, Graphics, Position, bulletAngle, 7.5f, Damage, 1800);
				
				Ammo--;
				player.Play();
				if (Ammo <= 0)
				{
					Status = GunStatus.OutOfAmmo;
					CurrentBurst = 0;
				}
				
				items.Add(casing);
				items.Add(bullet);
			}
			
			return items.ToArray();
		}
		
		public static Gun Create(GraphicsContext graphics, string filePath)
		{
			
			using (StreamReader reader = new StreamReader(filePath))
			{
				string[] info = new string[13];
				for (int i = 0; i < info.Length; i++)
				{
					string line;
					do
					{
						line = reader.ReadLine();
					}while (line.Contains("###"));
					info[i] = line;
				}
				
				FireType fireType = FireType.SemiAuto;
				switch (info[9])
				{
				case "Automatic":
					fireType = FireType.Automatic;
					break;
				case "Burst":
					fireType = FireType.Burst;
					break;
				}
				Texture2D bulletTexture = new Texture2D(info[10], false), 
					casingTexture = new Texture2D(info[11], false), 
					groundTexture = new Texture2D(info[12], false);
				
				return new HandGun(groundTexture, graphics, Vector3.Zero, info[1], int.Parse(info[2]), int.Parse(info[3]), 
				                        int.Parse(info[4]), float.Parse(info[5]), int.Parse(info[6]), int.Parse(info[7]),
				                        int.Parse(info[8]), fireType, bulletTexture, casingTexture);
			}
			
			return new HandGun(graphics, Vector3.Zero);
		}
	}
}

