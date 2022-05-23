//Shawn Carter

using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using System.Collections.Generic;
using System.IO;
using Sce.PlayStation.Core.Audio;

namespace ShawnProject1
{
	public class LaserGun : Gun
	{	
		private static Sound gunshot = new Sound(@"\Application\assets\sound effect\laser gun 1.wav");
		private static SoundPlayer player = gunshot.CreatePlayer();
		
		private Vector4 color;
		private Timer rechargeRate;
		
		/// <summary>
		/// Default Laser
		/// </summary>
		public LaserGun(GraphicsContext graphics, Vector3 position) 
			: base(new Texture2D(@"\Application\assets\images\guns\laser.png", false), graphics, position)
		{
			Graphics = graphics;
			Name = "Laser Rifle";
			Damage = 1;
			BurstCount = 0;
			CurrentBurst = 0;
			Ammo = 100;
			AmmoMax = Ammo;
			Angle = 0;
			Accuracy = .2f;
			BurstDelay = new Timer(1);
			RateOfFire = new Timer(1);
			rechargeRate = new Timer(60);
			color = new Vector4(1, 0, 0, 1);
			BulletTexture = new Texture2D(@"\Application\assets\images\bullets\556bullet.png", false);
			CasingTexture = new Texture2D(@"\Application\assets\images\particles\556casing.png", false);
			Status = GunStatus.CanShoot;
			FireType = FireType.Automatic;
			sprite.Position = position;
		}
		
		private LaserGun(Texture2D texture, GraphicsContext graphics, Vector3 position, string name, int damage, 
		                 int burstCount, int ammoMax, float accuracy, int burstDelay, int rateOfFire, 
		                 int rechargeRate, Vector4 color, FireType fireType, Texture2D bulletTexture) 
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
			FireType = fireType;
			this.color = color;
			this.rechargeRate = new Timer(rechargeRate);
			BulletTexture = bulletTexture;
			Status = GunStatus.CanShoot;
		}
		
		public override GameObject[] Update ()
		{
			List<GameObject> items = new List<GameObject>();
			rechargeRate.Update();
			
			if (Ammo > 0)
				Status = GunStatus.Waiting;
			
			if (rechargeRate.Status == TimerStatus.JustCompleted)
			{
				if (Ammo != AmmoMax)
					Ammo++;
				rechargeRate.Reset();
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
				Status = GunStatus.Shooting;
				
				float bulletAngle = angle + (float)Ran.NextDouble() * Accuracy - Accuracy / 2;
				Bullet bullet = new Laser(BulletTexture, Graphics, Position, bulletAngle, Damage, color);
				
				Ammo--;
				player.Play();
				if (Ammo <= 0)
					Status = GunStatus.OutOfAmmo;
				
				items.Add(bullet);
			}
			
			return items.ToArray();
		}
		
		public static Gun Create(GraphicsContext graphics, string filePath)
		{
			try
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
					
					FireType fireType = FireType.Automatic;
					switch (info[8])
					{
					case "SemiAuto":
						fireType = FireType.SemiAuto;
						break;
					case "Burst":
						fireType = FireType.Burst;
						break;
					}
					Texture2D bulletTexture = new Texture2D(info[11], false), 
						groundTexture = new Texture2D(info[12], false);
					string[] colorString = info[10].ToString().Split(',');
					Vector4 color = new Vector4(float.Parse(colorString[0]), float.Parse(colorString[1]), 
					                            float.Parse(colorString[2]), float.Parse(colorString[3]));
					
					return new LaserGun(groundTexture, graphics, Vector3.Zero, info[1], int.Parse(info[2]), 
					                    int.Parse(info[3]), int.Parse(info[4]), float.Parse(info[5]), 
					                    int.Parse(info[6]), int.Parse(info[7]), int.Parse(info[9]), color,
					                    fireType, bulletTexture);
				}
			}
			catch
			{
				return new LaserGun(graphics, Vector3.Zero);
			}
		}
	}
}

