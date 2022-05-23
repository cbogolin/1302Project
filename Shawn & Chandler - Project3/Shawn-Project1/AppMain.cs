//Shawn Carter

using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.HighLevel.UI;
using System.IO;

namespace ShawnProject1
{
	public class AppMain	
	{
		private static Player player;
		private static List<Gun> guns;
		private static List<Particle> particles;
		private static List<Bullet> playerBullets;
		private static List<Bullet> enemyBullets;
		private static List<Enemy> enemies;
		private static List<Vector3> spawns;
		
		private static Sprite introScreen, creditsScreen, highScoreScreen, newScoreScreen, controlsScreen, 
			gameoverScreen, playingScreen, exitScreen;
		
		private const string GUNDIRECTORY = "/Application/assets/guns/";
		
		private static int totalKilled, enemiesKilled, shieldsKilled, trackersKilled, rangersKilled, 
						   timeSurvived, bulletsFired;
		
		private static GameState gameState;
		private static Menu mainMenu, exitMenu, info, gameOver, title, highScore, newHighScore;
		
		private static Texture2D enemyTexture;
		
		private static float endingAlpha;
		private static Sprite endingSprite;
		
		private static List<Score> highScores;
		private static int stringIndex;
		private static Score newScore;
		
		private static Scene scene;
		private static Label[] labels;
		private static List<Label> gunInfo;
		private static Scene gameoverScene;
		private static List<Label> gameOverInfo;
		
		private static GraphicsContext graphics;
		private static Random ran;
		private static bool running;
		
		
		
		private static Score test;
		
		public static void Main (string[] args)
		{
			Initialize ();

			while (running) {
				SystemEvents.CheckEvents ();
				Update ();
				Render ();
			}
		}

		public static void Initialize ()
		{
			#region starting stuff
			running = true;
			gameState = GameState.MainMenu;
			ValidateFiles();
			
			totalKilled = 0;
			trackersKilled = 0;
			shieldsKilled = 0;
			rangersKilled = 0;
			enemiesKilled = 0;
			timeSurvived = 0;
			bulletsFired = 0;
			
			graphics = new GraphicsContext ();
			UISystem.Initialize(graphics);
			
			ran = new Random();
			#endregion
			
			Texture2D texture = new Texture2D(@"\Application\assets\images\Antibody.png", false);
			enemyTexture = new Texture2D(@"\Application\assets\images\Antibody_02.png", false);
			
			player = new Player(texture, graphics, new Vector3(graphics.Screen.Width / 2, graphics.Screen.Height / 2, 0));
			guns = new List<Gun>();
			particles = new List<Particle>();
			playerBullets = new List<Bullet>();
			enemyBullets = new List<Bullet>();
			enemies = new List<Enemy>();
			
			CreateMenus();
			CreateBackgrounds();
			
			endingAlpha = 0;
			endingSprite = new Sprite(graphics, new Texture2D(@"\Application\assets\images\information\blank.png", false));
			endingSprite.Scale = new Vector2(graphics.Screen.Width / endingSprite.Width, 
			                                 graphics.Screen.Height / endingSprite.Height);
			endingSprite.SetColor(0, 0, 0, 0);
			
			spawns = new List<Vector3>();
			spawns.Add(new Vector3(25, 25, 0));
			spawns.Add(new Vector3(graphics.Screen.Width - 25, 25, 0));
			spawns.Add(new Vector3(25, graphics.Screen.Height - 25, 0));
			spawns.Add(new Vector3(graphics.Screen.Width - 25, graphics.Screen.Height - 25, 0));
			
			texture = new Texture2D(@"Application\assets\images\information\alphabet.png", false);
			Score.Initialize(graphics, texture);
			
			highScores = new List<Score>();
			ReadHighScores();
			MusicManager.Initialize();
			MusicManager.QuickChange(@"\Application\assets\music\Ultralounge.mp3");
			
			test = new Score("test", 200);
			
			foreach(string s in Directory.GetFiles(GUNDIRECTORY + @"\enemy\"))
			{
				if (s.Contains(".gun"))
					Ranger.AddGun(Gun.FromFile(graphics, s));
			}
			
			//player.AddGun(new HandGun(graphics, Vector3.Zero));
			//player.AddGun(new Melee(graphics, Vector3.Zero));
			
			
			Enemy.SetPlayer(ref player);
			
			#region labels
			scene = new Scene();
			gameoverScene = new Scene();
			gameOverInfo = new List<Label>();
			labels = new Label[5];
			for (int i = 0; i < labels.Length; i++)
			{
				labels[i] = new Label();
				labels[i].SetPosition(0, i * 25);
				scene.RootWidget.AddChildLast(labels[i]);
			}
			//labels[0].Text = player.SelectedGun.Status.ToString();
			//labels[0].TextColor = new UIColor(1, 1, 1, 1);
			
			gunInfo = new List<Label>();
			foreach(Gun g in player.Guns)
			{
				Label label = new Label();
				label.Text = g.Name;
				gunInfo.Add(label);
				
				for (int i = 0; i < gunInfo.Count; i++)
					gunInfo[i].SetPosition(0, graphics.Screen.Height - 25 * (gunInfo.Count - i));
				scene.RootWidget.AddChildLast(gunInfo[gunInfo.Count - 1]);
			}
			
			UISystem.SetScene(scene, null);
			#endregion
			
			#region guns
			List<string> gunList = new List<string>();
			gunList.AddRange(GetAvailableGuns(@"\Documents\saveinfo\defaultguns.txt"));
			gunList.AddRange(GetAvailableGuns(@"\Documents\saveinfo\unlockedguns.txt"));
			
			for (int i = 0; i < 5; i++)
			{
				int index = ran.Next(gunList.Count);
				guns.Add(Gun.FromFile(graphics, GUNDIRECTORY + gunList[index] + ".gun"));
				do
				{
					guns[guns.Count - 1].Position = new Vector3(ran.Next(25, 900), ran.Next(25, 500), 0);
				} while (player.Position.Distance(guns[i].Position) < player.Radius + guns[i].Radius);
				gunList.RemoveAt(index);
			}
			guns.Add(Gun.FromFile(graphics, GUNDIRECTORY + "handgun.gun"));
			guns[guns.Count - 1].Position = new Vector3(graphics.Screen.Width / 2, 25, 0);
			//guns.Add(Gun.FromFile(graphics, @"\Application\assets\guns\autopistol.gun"));
			#endregion
		}
		
		public static void CreateMenus()
		{
			Sprite[] menuOptions = new Sprite[5];
			Texture2D texture = new Texture2D(@"\Application\assets\images\information\Play Button.png", false);
			menuOptions[0] = new Sprite(graphics, texture);
			menuOptions[0].Position = new Vector3(33, 31, 0);
			texture = new Texture2D(@"\Application\assets\images\information\Controls Button.png", false);
			menuOptions[1] = new Sprite(graphics, texture);
			menuOptions[1].SetColor(.5f, .5f, .5f, 1);
			menuOptions[1].Position = new Vector3(129, 123, 0);
			texture = new Texture2D(@"\Application\assets\images\information\High Scores Button.png", false);
			menuOptions[2] = new Sprite(graphics, texture);
			menuOptions[2].SetColor(.5f, .5f, .5f, 1);
			menuOptions[2].Position = new Vector3(228, 210, 0);
			texture = new Texture2D(@"\Application\assets\images\information\Credits Button.png", false);
			menuOptions[3] = new Sprite(graphics, texture);
			menuOptions[3].SetColor(.5f, .5f, .5f, 1);
			menuOptions[3].Position = new Vector3(309, 304, 0);
			texture = new Texture2D(@"\Application\assets\images\information\Exit Button.png", false);
			menuOptions[4] = new Sprite(graphics, texture);
			menuOptions[4].SetColor(.5f, .5f, .5f, 1);
			menuOptions[4].Position = new Vector3(413, 404, 0);
			mainMenu = new Menu(menuOptions, MenuOrientation.Vertical);
			
			menuOptions = new Sprite[2];
			texture = new Texture2D(@"\Application\assets\images\information\No Button.png", false);
			menuOptions[0] = new Sprite(graphics, texture);
			menuOptions[0].Position = new Vector3(graphics.Screen.Width / 2 - 155, 
			                                      graphics.Screen.Height / 2, 0);
			texture = new Texture2D(@"\Application\assets\images\information\Yes Button.png", false);
			menuOptions[1] = new Sprite(graphics, texture);
			menuOptions[1].Position = new Vector3(graphics.Screen.Width / 2 + 5, 
			                                      graphics.Screen.Height / 2, 0);
			menuOptions[1].SetColor(.5f, .5f, .5f, 1);
			exitMenu = new Menu(menuOptions, MenuOrientation.Horizontal);
			Sprite sprite;
			
			texture = new Texture2D(@"\Application\assets\images\information\Controls Screen.png", false);
			sprite = new Sprite(graphics, texture);
			sprite.Scale = new Vector2(graphics.Screen.Width / sprite.Width, 
			                           graphics.Screen.Height / sprite.Height);
			info = new Menu();
			info.AddExtra(sprite);
			
			gameOver = new Menu();
		}
		
		public static void CreateBackgrounds()
		{
			Texture2D texture = new Texture2D(@"\Application\assets\images\information\Title Screen.png", false);
			introScreen = new Sprite(graphics, texture);
			introScreen.Scale = new Vector2(graphics.Screen.Width / introScreen.Width, 
			                                graphics.Screen.Height / introScreen.Height);
			
			texture = new Texture2D(@"\Application\assets\images\information\Controls Screen.png", false);
			controlsScreen = new Sprite(graphics, texture);
			controlsScreen.Scale = new Vector2(graphics.Screen.Width / controlsScreen.Width, 
			                                  graphics.Screen.Height / controlsScreen.Height);
			
			texture = new Texture2D(@"\Application\assets\images\information\Credits Screen.png", false);
			creditsScreen = new Sprite(graphics, texture);
			creditsScreen.Scale = new Vector2(graphics.Screen.Width / creditsScreen.Width, 
			                                  graphics.Screen.Height / creditsScreen.Height);
			
			texture = new Texture2D(@"\Application\assets\images\information\High Scores Screen.png", false);
			highScoreScreen = new Sprite(graphics, texture);
			highScoreScreen.Scale = new Vector2(graphics.Screen.Width / highScoreScreen.Width, 
			                                  graphics.Screen.Height / highScoreScreen.Height);
			
			texture = new Texture2D(@"\Application\assets\images\information\New High Score Screen.png", false);
			newScoreScreen = new Sprite(graphics, texture);
			newScoreScreen.Scale = new Vector2(graphics.Screen.Width / newScoreScreen.Width, 
			                                  graphics.Screen.Height / newScoreScreen.Height);
			
			texture = new Texture2D(@"\Application\assets\images\information\Game Over Screen.png", false);
			gameoverScreen = new Sprite(graphics, texture);
			gameoverScreen.Scale = new Vector2(graphics.Screen.Width / gameoverScreen.Width, 
			                                  graphics.Screen.Height / gameoverScreen.Height);
			
			texture = new Texture2D(@"\Application\assets\images\information\Play Screen.png", false);
			playingScreen = new Sprite(graphics, texture);
			playingScreen.Scale = new Vector2(graphics.Screen.Width / playingScreen.Width, 
			                                  graphics.Screen.Height / playingScreen.Height);
			
			texture = new Texture2D(@"\Application\assets\images\information\Exit Screen.png", false);
			exitScreen = new Sprite(graphics, texture);
			exitScreen.Scale = new Vector2(graphics.Screen.Width / exitScreen.Width, 
			                                  graphics.Screen.Height / exitScreen.Height);
		}

		public static void Update ()
		{
			var gamePadData = GamePad.GetData(0);
			int selected = 0;
			
			MusicManager.Update();
			
			switch (gameState)
			{
				#region main menu
				case GameState.MainMenu:
				selected = mainMenu.Update(gamePadData);
				switch (selected)
				{
				case 0:
					gameState = GameState.Playing;
					MusicManager.PlaySong(@"\Application\assets\music\Your Call.mp3");
					mainMenu.Reset();
					selected = 0;
					break;
				case 1:
					gameState = GameState.Controls;
					mainMenu.Reset();
					selected = 0;
					break;
				case 2:
					gameState = GameState.HighScore;
					mainMenu.Reset();
					selected = 0;
					break;
				case 3:
					gameState = GameState.Credits;
					mainMenu.Reset();
					selected = 0;
					break;
				case 4:
					gameState = GameState.ExitMenu;
					mainMenu.Reset();
					selected = 0;
					break;
				}
				break;
				#endregion
				
				#region playing
			case GameState.Playing:
				if (player.Health <= 0)
				{
					gameState = GameState.Ending;
					MusicManager.PlaySong(@"Application\assets\music\Severe Tire Damage.mp3");
				}
				timeSurvived++;
				
				SortPlayerEntities(player.Update(gamePadData));
				for (int i = particles.Count - 1; i >= 0; i--)
				{
					SortPlayerEntities(particles[i].Update());
					if (particles[i].State == EntityState.Dead)
						particles.RemoveAt(i);
				}
				
				for (int i = enemies.Count - 1; i >= 0; i--)
				{
					SortEnemyEntities(enemies[i].Update());
					
					if (enemies[i].Position.Distance(player.Position) < (enemies[i].Radius + player.Radius) / 2)
						player.Hit();
					
					if (enemies[i].State == EntityState.Dead)
					{
						TrackKill(enemies[i]);
						enemies.RemoveAt(i);
					}
				}
				
				#region bullets
				#region player bullets
				for (int i = playerBullets.Count - 1; i >= 0; i--)
				{
					SortPlayerEntities(playerBullets[i].Update());
					Vector3 position = playerBullets[i].Position;
					if (position.X < 0 || position.X > graphics.Screen.Width || position.Y < 0 || position.Y > 
					    graphics.Screen.Height)
						playerBullets[i].Hit();
					if (playerBullets[i] is Laser)
					{
						foreach (Enemy e in enemies)
						{
							Laser laser = (Laser) playerBullets[i];
							if (laser.Collides(e.Position, e.Radius))
							{
								e.Hit(playerBullets[i].Damage);
							}
						}
					}
					else if (playerBullets[i] is Swing)
					{
						foreach (Enemy e in enemies)
						{
							Swing swing = (Swing) playerBullets[i];
							if (swing.Collides(e.Position, e.Radius))
							{
								e.Hit(playerBullets[i].Damage);
							}
						}
					}
					else
					{
						foreach (Enemy e in enemies)
						{
							if (e.State == EntityState.Alive && e.Position.DistanceSquared(position) < e.Radius * e.Radius)
							{
								playerBullets[i].Hit();
								e.Hit(playerBullets[i].Damage);
							}
						}
					}
					if (playerBullets[i].State == EntityState.Dead)
							playerBullets.RemoveAt(i);
				}	
				#endregion
				
				#region enemy bullets
				for (int i = enemyBullets.Count - 1; i >= 0; i--)
				{
					SortEnemyEntities(enemyBullets[i].Update());
					Vector3 position = enemyBullets[i].Position;
					if (position.X < 0 || position.X > graphics.Screen.Width || position.Y < 0 || position.Y > graphics.Screen.Height)
						enemyBullets[i].Hit();
					if (enemyBullets[i] is Laser)
					{
						Laser laser = (Laser) enemyBullets[i];
						if (laser.Collides(player.Position, player.Radius / 2))
							player.Hit();
					}
					else
					{
						if (player.Position.DistanceSquared(position) < player.Radius * player.Radius / 2)
						{
							enemyBullets[i].Hit();
							player.Hit();
						}
					}
					if (enemyBullets[i].State == EntityState.Dead)
							enemyBullets.RemoveAt(i);
				}
				#endregion
				#endregion
				
				for (int i = guns.Count - 1; i >= 0; i--)
				{
					if (player.Position.Distance(guns[i].Position) < player.Radius + guns[i].Radius)
					{
						player.AddGun(guns[i]);
						guns.RemoveAt(i);
					}
				}
				
				#region adding more enemies
				if (enemies.Count < 3)
				{
					float minDistance = 2000;
					int closestDistance = 0;
					Vector3 spawn = new Vector3();
					for (int i = 0; i < spawns.Count; i++)
					{
						float distance = player.Position.DistanceSquared(spawns[i]);
						if (distance < minDistance)
						{
							minDistance = distance;
							closestDistance = i;
						}
					}
					
					int index = 0;
					do
					{
						index = ran.Next(spawns.Count);
					}while(index == closestDistance);
					spawn = spawns[index];
					
					int enemyTypes = 4;
					foreach(Enemy e in enemies)
						if (e is Shielder)
							enemyTypes--;
					
					switch(ran.Next(enemyTypes))
					{
					case 0:
						enemies.Add(new Enemy(enemyTexture, graphics, spawn));
						break;
					case 1:
						enemies.Add(new Tracker(enemyTexture, graphics, spawn, 3));
						break;	
					case 2:
						enemies.Add(new Ranger(enemyTexture, graphics, spawn));
						break;	
					case 3:
						enemies.Add(new Shielder(enemyTexture, graphics, spawn, 5, 100));
						break;
					}
				}
				#endregion
				
				#region updating labels
				labels[0].Text = "Health: " + player.Health;
				if (player.Guns.Length > 0)
				{
					labels[2].Text = player.SelectedGun.Name;
					labels[3].Text = player.SelectedGun.Ammo + "/" + player.SelectedGun.AmmoMax;
					labels[4].Text = player.SelectedGun.FireType.ToString();
				}
				
				while(player.Guns.Length != gunInfo.Count)
				{
					Label label = new Label();
					label.Text = player.Guns[gunInfo.Count].Name;
					gunInfo.Add(label);
					
					for (int i = 0; i < gunInfo.Count; i++)
						gunInfo[i].SetPosition(0, graphics.Screen.Height - 25 * (gunInfo.Count - i));
					scene.RootWidget.AddChildLast(gunInfo[gunInfo.Count - 1]);
				}
				
				for (int i = 0; i < player.Guns.Length; i++)
				{
					Gun gun = player.Guns[i];
					UIColor color = new UIColor(0, 0, 0, 1);
					if (player.SelectedGun == gun)
						color.G = 1;
					
					if (gun.Status == GunStatus.OutOfAmmo)
						color.R = 1;
					
					if (color.R == 0 && color.G == 0)
						color = new UIColor(1, 1, 1, 1);
					
					gunInfo[i].TextColor = color;
					gunInfo[i].Text = gun.Name;
				}
				#endregion
				break;
				#endregion
				
				#region ending
			case GameState.Ending:
				endingAlpha += .005f;
				endingSprite.SetColor(0, 0, 0, endingAlpha);
				if (endingAlpha > 1)
				{
					labels[2].Text = "";
					labels[3].Text = "";
					labels[4].Text = "";
					
					foreach (Label l in gameOverInfo)
						gameoverScene.RootWidget.RemoveChild(l);
					gameOverInfo = new List<Label>();
					UISystem.SetScene(gameoverScene);
					
					string[] achievements = CheckAchievements();
					if (achievements.Length > 0)
					{
						int index = 0;
						foreach(string s in achievements)
						{
							Label label = new Label();
							label.SetPosition(graphics.Screen.Width / 2 - s.Length * 5, index * 25);
							if (s.Contains("【"))
								label.SetPosition(graphics.Screen.Width / 2 - s.Length * 7, index * 25);
							label.Text = s;
							label.Width = 1000;
							gameOverInfo.Add(label);
							gameoverScene.RootWidget.AddChildLast(gameOverInfo[gameOverInfo.Count - 1]);
							index++;
						}
					}
					else 
					{
						Label label = new Label();
						label.Text = "No achievements earned";
						label.SetPosition(graphics.Screen.Width / 2 - label.Text.Length * 5, 0);
						label.Width = 1000;
						gameOverInfo.Add(label);
						gameoverScene.RootWidget.AddChildLast(gameOverInfo[gameOverInfo.Count - 1]);
					}
					gameState = GameState.Gameover;	
				}
				break;
				#endregion
				
				#region gameover
			case GameState.Gameover:
				endingAlpha -= .01f;
				endingSprite.SetColor(0, 0, 0, endingAlpha);
				if (gameOver.Update(gamePadData) == 0)
				{
					UISystem.SetScene(gameoverScene);
					gameState = GameState.Achievements;
				}
				break;
				#endregion
				
				#region achievements
			case GameState.Achievements:
				if ((gamePadData.ButtonsDown & GamePadButtons.Cross) != 0)
				{
					Score tempScore = new Score("", enemiesKilled);
					if (CompareHighScore(tempScore) >= 0)
					{
						gameState = GameState.EnteringHighScore;
						newScore = new Score("", totalKilled);
						newScore.SetText("A");
						newScore.X = graphics.Screen.Width / 2 - 48;
						newScore.Y = graphics.Screen.Height / 2;
						MusicManager.PlaySong(@"\Application\assets\music\Discovery Hit.mp3");
					}
					else 
					{
						gameState = GameState.HighScore;
						Reset();
					}
				}
				break;
				#endregion
				
				#region scores
			case GameState.EnteringHighScore:
				if ((gamePadData.ButtonsDown & GamePadButtons.Up) != 0)
				{
					stringIndex++;
					if (stringIndex >= Score.CHARACTERS.Length)
						stringIndex = 0;
					newScore.EditCharacter(newScore.DisplayText.Length - 1, Score.CHARACTERS[stringIndex]);
				}
				if ((gamePadData.ButtonsDown & GamePadButtons.Down) != 0)
				{
					stringIndex--;
					if (stringIndex < 0)
						stringIndex = Score.CHARACTERS.Length - 1;
					newScore.EditCharacter(newScore.DisplayText.Length - 1, Score.CHARACTERS[stringIndex]);
				}
				if ((gamePadData.ButtonsDown & GamePadButtons.Cross) != 0)
				{
					if (newScore.DisplayText.Length == 3)
					{
						gameState = GameState.HighScore;
						newScore = new Score(newScore.DisplayText, totalKilled);
						AddScore(newScore);
						SaveHighScores();
						Reset();
					}
					else
						newScore.SetText(newScore.DisplayText + Score.CHARACTERS[stringIndex]);
				}
				break;
				
			case GameState.HighScore:
				if ((gamePadData.ButtonsDown & GamePadButtons.Cross) != 0)
				{
					gameState = GameState.MainMenu;
					MusicManager.PlaySong(@"\Application\assets\music\UltraLounge.mp3");
				}
				break;
				#endregion
				
				#region credits
			case GameState.Credits:
				if((gamePadData.ButtonsDown & GamePadButtons.Cross) != 0)
					gameState = GameState.MainMenu;
				break;
				#endregion
				
				#region info
			case GameState.Controls:
				selected = info.Update(gamePadData);
				if (selected == 0)
				{
					gameState = GameState.MainMenu;
					selected = 0;
				}
				break;
				#endregion
				
				#region exit
			case GameState.ExitMenu:
				selected = exitMenu.Update(gamePadData);
				switch (selected)
				{
				case 0:
					gameState = GameState.MainMenu;
					break;
				case 1:
					running = false;
					break;
				}
				break;
				#endregion
			}
		}
		
		public static void SortPlayerEntities(GameObject[] items)
		{
			for (int i = 0; i < items.Length; i++)
			{
				if (items[i] is Bullet)
				{
					playerBullets.Add((Bullet)items[i]);
					bulletsFired++;
				}
				else if (items[i] is Particle)
				{
					particles.Add((Particle)items[i]);
				}
			}
		}
		
		public static void SortEnemyEntities(GameObject[] items)
		{
			foreach (GameObject go in items)
			{
				if (go is Bullet)
					enemyBullets.Add(go as Bullet);
				else if (go is Particle)
					particles.Add(go as Particle);
				else if (go is Enemy)
					enemies.Add(go as Enemy);
			}
		}
		
		public static string[] GetAvailableGuns(string filePath)
		{
			using (StreamReader reader = new StreamReader(filePath))
			{
				List<string> guns = new List<string>();
				while (reader.Peek() >= 0)
				{
					if (reader.ReadLine().Contains("- true"))
						guns.Add(reader.ReadLine());
					else
						reader.ReadLine();
				}
				return guns.ToArray();
			}
		}
		
		public static void ValidateFiles()
		{
			if (!Directory.Exists(@"\Documents\saveinfo"))
				Directory.CreateDirectory(@"\Documents\saveinfo");
			if (!File.Exists(@"\Documents\saveinfo\defaultguns.txt"))
				using (StreamReader reader = new StreamReader(@"\Application\assets\misc\defaultguns.txt"))
				{
					using (StreamWriter writer = new StreamWriter(@"\Documents\saveinfo\defaultguns.txt"))
					{
						while (reader.Peek() >= 0)
							writer.WriteLine(reader.ReadLine());
					}
				}
			if (!File.Exists(@"\Documents\saveinfo\unlockedguns.txt"))
				using (StreamReader reader = new StreamReader(@"\Application\assets\misc\unlockedguns.txt"))
				{
					using (StreamWriter writer = new StreamWriter(@"\Documents\saveinfo\unlockedguns.txt"))
					{
						while (reader.Peek() >= 0)
							writer.WriteLine(reader.ReadLine());
					}
				}
			if (!File.Exists(@"\Documents\saveinfo\scores.txt"))
			{
				using (StreamWriter writer = new StreamWriter(@"\Documents\saveinfo\scores.txt"))
				{
					writer.WriteLine("SJC: 50");
				}
			}
		}
		
		public static void ReadHighScores()
		{
			highScores.Clear();
			char[] split = {':', ' '};
			try
			{
				using (StreamReader reader = new StreamReader(@"\Documents\saveinfo\scores.txt"))
				{
					while (reader.Peek() >= 0)
					{
						string line = reader.ReadLine();
						string[] splitLine = line.Split(split, StringSplitOptions.RemoveEmptyEntries);
						
						highScores.Add(new Score(splitLine[0], int.Parse(splitLine[1])));
					}
				}
			}
			catch 
			{
				ValidateFiles();
				ReadHighScores();
			}
			for (int i = 0; i < highScores.Count; i++)
			{
				highScores[i].X = graphics.Screen.Width / 2 - 144;
				highScores[i].Y = graphics.Screen.Height / 2 - 100 + 32 * i;
			}
		}
		
		public static void SaveHighScores()
		{
			using (StreamWriter writer = new StreamWriter(@"\Documents\saveinfo\scores.txt"))
			{
				foreach(Score s in highScores)
					writer.WriteLine(s.ToString());
			}
		}
		
		public static int CompareHighScore(Score score)
		{
			for(int i = 0; i < highScores.Count; i++)
				if (score.Amount > highScores[i].Amount)
					return i;
			if (highScores.Count < 5)
				return highScores.Count + 1;
			return -1;
		}
		
		public static void AddScore(Score score)
		{
			bool placedScore = false;
			
			highScores.Add(score);
			
			if (highScores.Count == 2)
			{
				if (score.Amount > highScores[0].Amount)
				{
					highScores[1] = highScores[0];
					highScores[0] = score;
				}
			}
			else
			{
				for (int i = highScores.Count - 1; i > 0; i--)
				{
					highScores[i] = highScores[i - 1];
					if (score.Amount < highScores[i].Amount)
					{
						highScores[i] = score;
						break;
					}
				}
			}
				
			if (highScores.Count > 5)
				highScores.RemoveRange(5, highScores.Count - 5);
			
			for (int i = 0; i < highScores.Count; i++)
			{
				highScores[i].X = graphics.Screen.Width / 2 - 144;
				highScores[i].Y = graphics.Screen.Height / 2 - 100 + 32 * i;
			}
		}
		
		public static string[] CheckAchievements()
		{
			List<string> lines = new List<string>();
			List<string> result = new List<string>();
			string path = @"\Documents\saveinfo\unlockedguns.txt";
			using (StreamReader reader = new StreamReader(path))
			{
				lines.Add(reader.ReadLine());
				if (lines[lines.Count - 1].Contains("- false"))
				{
					if (timeSurvived >= 3600)
					{
						lines[lines.Count - 1] = lines[lines.Count - 1].Split('-')[0] + "- true";
						result.Add("Survived for one minute: April Showers (Spiral Gun) unlocked");
					}
				}
				lines.Add(reader.ReadLine());
				
				lines.Add(reader.ReadLine());
				if (lines[lines.Count - 1].Contains("- false"))
				{
					if (timeSurvived >= 10800)
					{
						lines[lines.Count - 1] = lines[lines.Count - 1].Split('-')[0] + "- true";
						result.Add("Survived for three minutes: Sprinkler (Spiral Gun) unlocked");
					}
				}
				lines.Add(reader.ReadLine());
				
				lines.Add(reader.ReadLine());
				if (lines[lines.Count - 1].Contains("- false"))
				{
					if (timeSurvived >= 18000)
					{
						lines[lines.Count - 1] = lines[lines.Count - 1].Split('-')[0] + "- true";
						result.Add("Survived for five minutes: Vulcan (Minigun) unlocked");
					}
				}
				lines.Add(reader.ReadLine());
				
				lines.Add(reader.ReadLine());
				if (lines[lines.Count - 1].Contains("- false"))
				{
					if (totalKilled >= 30)
					{
						lines[lines.Count - 1] = lines[lines.Count - 1].Split('-')[0] + "- true";
						result.Add("Killed 30 enemies: Auto-Pistol (Handgun) unlocked");
					}
				}
				lines.Add(reader.ReadLine());
				
				lines.Add(reader.ReadLine());
				if (lines[lines.Count - 1].Contains("- false"))
				{
					if (totalKilled >= 50)
					{
						lines[lines.Count - 1] = lines[lines.Count - 1].Split('-')[0] + "- true";
						result.Add("Killed 50 enemies: Heavy Rifle (Assault Rifle) unlocked");
					}
				}
				lines.Add(reader.ReadLine());
				
				lines.Add(reader.ReadLine());
				if (lines[lines.Count - 1].Contains("- false"))
				{
					if (totalKilled >= 100)
					{
						lines[lines.Count - 1] = lines[lines.Count - 1].Split('-')[0] + "- true";
						result.Add("Killed 100 enemies: Striker (Shotgun) unlocked");
					}
				}
				lines.Add(reader.ReadLine());
				
				lines.Add(reader.ReadLine());
				if (lines[lines.Count - 1].Contains("- false"))
				{
					if (shieldsKilled >= 20)
					{
						lines[lines.Count - 1] = lines[lines.Count - 1].Split('-')[0] + "- true";
						result.Add("Killed 20 Shielders: Auto-Shotgun (Shotgun) unlocked");
					}
				}
				lines.Add(reader.ReadLine());
				
				lines.Add(reader.ReadLine());
				if (lines[lines.Count - 1].Contains("- false"))
				{
					if (rangersKilled >= 20)
					{
						lines[lines.Count - 1] = lines[lines.Count - 1].Split('-')[0] + "- true";
						result.Add("Killed 20 Rangers: Decimator (Laser Gun) unlocked");
					}
				}
				lines.Add(reader.ReadLine());
				
				lines.Add(reader.ReadLine());
				if (lines[lines.Count - 1].Contains("- false"))
				{
					if (trackersKilled >= 20)
					{
						lines[lines.Count - 1] = lines[lines.Count - 1].Split('-')[0] + "- true";
						result.Add("Killed 20 Trackers: Hand Cannon (Handgun) unlocked");
					}
				}
				lines.Add(reader.ReadLine());
				
				lines.Add(reader.ReadLine());
				if (lines[lines.Count - 1].Contains("- false"))
				{
					if (trackersKilled >= 10 && shieldsKilled >= 10 && rangersKilled >= 10 && enemiesKilled >= 10)
					{
						lines[lines.Count - 1] = lines[lines.Count - 1].Split('-')[0] + "- true";
						result.Add("Killed 10 of each enemy: ディスコレーザー【レーザーガン】unlocked");
					}
				}
				lines.Add(reader.ReadLine());
				
				lines.Add(reader.ReadLine());
				if (lines[lines.Count - 1].Contains("- false"))
				{
					if (bulletsFired >= 2000)
					{
						lines[lines.Count - 1] = lines[lines.Count - 1].Split('-')[0] + "- true";
						result.Add("Fired 2000 bullets: Bullethose (SMG) unlocked");
					}
				}
				lines.Add(reader.ReadLine());
			}
			
			if (result.Count > 0)
			{
				File.Delete(path);
				
				using (StreamWriter writer = new StreamWriter(path))
				{
					foreach (string s in lines)
						writer.WriteLine(s);
				}
			}
			
			return result.ToArray();
		}
		
		public static void TrackKill(Enemy enemy)
		{
			totalKilled++;
			if (enemy is Tracker)
				trackersKilled++;
			else if (enemy is Shielder)
				shieldsKilled++;
			else if (enemy is Ranger)
				rangersKilled++;
			else
				enemiesKilled++;
		}
		
		public static void Reset()
		{
			totalKilled = 0;
			trackersKilled = 0;
			shieldsKilled = 0;
			rangersKilled = 0;
			enemiesKilled = 0;
			timeSurvived = 0;
			bulletsFired = 0;
			
			Texture2D texture = new Texture2D(@"\Application\assets\images\Antibody.png", false);
			
			player = new Player(texture, graphics, new Vector3(550, 250, 0));
			guns.Clear();
			particles.Clear();
			playerBullets.Clear();
			enemyBullets.Clear();
			enemies.Clear();
			endingAlpha = 0;
			endingSprite.SetColor(0, 0, 0, 0);
			
			stringIndex = 0;
			
			Enemy.SetPlayer(ref player);
			
			#region labels
			foreach(Label l in gunInfo)
				scene.RootWidget.RemoveChild(l);
			gunInfo.Clear();
			foreach(Gun g in player.Guns)
			{
				Label label = new Label();
				label.Text = g.Name;
				gunInfo.Add(label);
				
				for (int i = 0; i < gunInfo.Count; i++)
					gunInfo[i].SetPosition(0, graphics.Screen.Height - 25 * (gunInfo.Count - i));
				scene.RootWidget.AddChildLast(gunInfo[gunInfo.Count - 1]);
			}
			
			UISystem.SetScene(scene, null);
			#endregion
			
			#region guns
			List<string> gunList = new List<string>();
			gunList.AddRange(GetAvailableGuns(@"\Documents\saveinfo\defaultguns.txt"));
			gunList.AddRange(GetAvailableGuns(@"\Documents\saveinfo\unlockedguns.txt"));
			
			for (int i = 0; i < 5; i++)
			{
				int index = ran.Next(gunList.Count);
				guns.Add(Gun.FromFile(graphics, GUNDIRECTORY + gunList[index] + ".gun"));
				do
				{
					guns[guns.Count - 1].Position = new Vector3(ran.Next(25, 900), ran.Next(25, 500), 0);
				} while (player.Position.Distance(guns[i].Position) < player.Radius + guns[i].Radius);
				gunList.RemoveAt(index);
			}
			guns.Add(Gun.FromFile(graphics, GUNDIRECTORY + "handgun.gun"));
			guns[guns.Count - 1].Position = new Vector3(graphics.Screen.Width / 2, 25, 0);
			//guns.Add(Gun.FromFile(graphics, @"\Application\assets\guns\autopistol.gun"));
			#endregion
		}

		public static void Render()
		{
			// Clear the screen
			graphics.SetClearColor (0.4f, 0.4f, 0.4f, 0.0f);
			graphics.Clear ();
			
			switch (gameState)
			{
			case GameState.MainMenu:
				introScreen.Render();
				mainMenu.Render();
				break;
				
				#region playing
			case GameState.Playing:
				playingScreen.Render();
				RenderEntities();
				
				UISystem.Render();
				break;
				#endregion
			case GameState.Ending:
				playingScreen.Render();
				RenderEntities();
				UISystem.Render();
				endingSprite.Render();
				break;
			case GameState.EnteringHighScore:
				newScoreScreen.Render();
				newScore.Render();
				break;
			case GameState.HighScore:
				highScoreScreen.Render();
				foreach (Score s in highScores)
					s.Render();
				break;
			case GameState.Achievements:
				playingScreen.Render();
				UISystem.Render();
				break;
			case GameState.Gameover:
				gameoverScreen.Render();
				endingSprite.Render();
				break;
			case GameState.Controls:
				info.Render();
				break;
			case GameState.Credits:
				creditsScreen.Render();
				break;
			case GameState.ExitMenu:
				exitScreen.Render();
				exitMenu.Render();
				break;
			}

			// Present the screen
			graphics.SwapBuffers ();
		}
		
		public static void RenderEntities()
		{
			foreach(Particle p in particles)
				p.Render();
			foreach(Gun g in guns)
				g.Render();
			foreach(Enemy e in enemies)
				e.Render();
			
			player.Render();
			
			foreach(Bullet b in playerBullets)
				b.Render();
			foreach(Bullet b in enemyBullets)
				b.Render();
		}
	}
}
