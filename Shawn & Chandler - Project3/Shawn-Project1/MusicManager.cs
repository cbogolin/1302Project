//Shawn Carter

using System;
using Sce.PlayStation.Core.Audio;

namespace ShawnProject1
{
	public static class MusicManager
	{
		private static float volume;
		private static string nextSong, currentSong;
		private static Bgm music;
		private static BgmPlayer musicPlayer;
		
		public static void Initialize()
		{
			volume = 0;
		}
		
		public static void Update()
		{
			if (nextSong != "")
			{
				volume -= .025f;
				musicPlayer.Volume = volume;
				if (volume <= 0)
				{
					if (music != null)
						music.Dispose();
					if (musicPlayer != null)
						musicPlayer.Dispose();
					music = new Bgm(nextSong);
					musicPlayer = music.CreatePlayer();
					musicPlayer.Loop = true;
					musicPlayer.Play();
					currentSong = nextSong;
					nextSong = "";
				}
			}
			else
			{
				if (volume < 1)
				{
					volume+= .025f;
					musicPlayer.Volume = volume;
				}
			}
		}
		
		public static void PlaySong(string path)
		{
			if (currentSong != path)
				nextSong = path;
		}
		
		public static void QuickChange(string path)
		{
			if (currentSong != path)
			{
				if (music != null)
					music.Dispose();
				if (musicPlayer != null)
					musicPlayer.Dispose();
				music = new Bgm(path);
				musicPlayer = music.CreatePlayer();
				musicPlayer.Loop = true;
				musicPlayer.Play();
				currentSong = path;
				nextSong = "";
			}
		}
	}
}

