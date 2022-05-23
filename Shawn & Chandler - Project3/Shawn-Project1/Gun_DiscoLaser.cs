//Shawn Carter

using System;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;
using System.IO;
using System.Collections.Generic;
using Sce.PlayStation.Core.Audio;

namespace ShawnProject1
{
	public class DiscoLaser : LaserGun
	{
		private static Sound gunshot = new Sound(@"\Application\assets\sound effect\laser gun 2.wav");
		private static SoundPlayer player = gunshot.CreatePlayer();
		
		private Timer rechargeRate;
		
		public DiscoLaser(GraphicsContext graphics, Vector3 position) 
			: base(graphics, position)
		{
			Graphics = graphics;
			Name = "Disco Laser";
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
			BulletTexture = new Texture2D(@"\Application\assets\images\bullets\556bullet.png", false);
			CasingTexture = new Texture2D(@"\Application\assets\images\particles\556casing.png", false);
			Status = GunStatus.CanShoot;
			FireType = FireType.Automatic;
			sprite.Position = position;
		}
		
		private DiscoLaser(Texture2D texture, GraphicsContext graphics, Vector3 position, string name, int damage, 
		                 int burstCount, int ammoMax, float accuracy, int burstDelay, int rateOfFire, 
		                 int rechargeRate, FireType fireType, Texture2D bulletTexture) 
			: base(graphics, position)
		{
			sprite = new Sprite(graphics, texture);
			sprite.Position = position;
			
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
			this.rechargeRate = new Timer(rechargeRate);
			BulletTexture = bulletTexture;
			Status = GunStatus.CanShoot;
		}
		
		public override GameObject[] Shoot (float angle)
		{
			List<GameObject> items = new List<GameObject>();
			
			if (Status != GunStatus.OutOfAmmo)
			{
				Angle = angle;
				CurrentBurst++;
				Status = GunStatus.Shooting;
				
				Vector4 color = new Vector4((float)Ran.NextDouble() * .75f + .25f, 
				                            (float)Ran.NextDouble() * .75f + .25f, 
				                            (float)Ran.NextDouble() * .75f + .25f, 1);
				
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
		
		public static DiscoLaser Create(GraphicsContext graphics, string filePath)
		{
			try
			{
				using (StreamReader reader = new StreamReader(filePath))
				{
					string[] info = new string[12];
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
					Texture2D bulletTexture = new Texture2D(info[10], false), 
						groundTexture = new Texture2D(info[11], false);
					
					return new DiscoLaser(groundTexture, graphics, Vector3.Zero, info[1], int.Parse(info[2]), 
					                      int.Parse(info[3]), int.Parse(info[4]), float.Parse(info[5]), 
					                      int.Parse(info[6]), int.Parse(info[7]), int.Parse(info[9]),
					                      fireType, bulletTexture);
				}
			}
			catch
			{
				return new DiscoLaser(graphics, Vector3.Zero);
			}
		}
	}
}

