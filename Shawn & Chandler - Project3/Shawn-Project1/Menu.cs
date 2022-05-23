//Shawn Carter

using System;
using Sce.PlayStation.Core.Input;
using System.Collections.Generic;

namespace ShawnProject1
{
	public class Menu
	{
		private Sprite[] options;
		private List<Sprite> extras;
		private int selected;
		private MenuOrientation orientation;
		
		public Menu()
		{
			options = new Sprite[0];
			orientation = MenuOrientation.Horizontal;
			extras = new List<Sprite>();
			selected = 0;
		}
		
		public Menu (Sprite[] options, MenuOrientation orientation)
		{
			this.options = options;
			this.orientation = orientation;
			extras = new List<Sprite>();
			selected = 0;
		}
		
		public int Update(GamePadData gamePadData)
		{
			
			if (options.Length > 0)
				switch (orientation)
				{
				case MenuOrientation.Vertical:
					if ((gamePadData.ButtonsDown & GamePadButtons.Down) != 0)
					{
						options[selected].SetColor(.5f, .5f, .5f, 1);
						if (options.Length - 1 > selected)
							selected++;
						else
							selected = 0;
						options[selected].SetColor(1, 1, 1, 1);
					}
					if ((gamePadData.ButtonsDown & GamePadButtons.Up) != 0)
					{
						options[selected].SetColor(.5f, .5f, .5f, 1);
						if (selected > 0)
							selected--;
						else
							selected = options.Length - 1;
						options[selected].SetColor(1, 1, 1, 1);
					}
					break;
				case MenuOrientation.Horizontal:
					if ((gamePadData.ButtonsDown & GamePadButtons.Right) != 0)
					{
						options[selected].SetColor(.5f, .5f, .5f, 1);
						if (options.Length - 1 > selected)
							selected++;
						else
							selected = 0;
						options[selected].SetColor(1, 1, 1, 1);
					}
					if ((gamePadData.ButtonsDown & GamePadButtons.Left) != 0)
					{
						options[selected].SetColor(.5f, .5f, .5f, 1);
						if (selected > 0)
							selected--;
						else
							selected = options.Length - 1;
						options[selected].SetColor(1, 1, 1, 1);
					}
					break;
				case MenuOrientation.Grid:
					break;
				}
			
			if ((gamePadData.ButtonsDown & GamePadButtons.Cross) != 0)
				return selected;
			return -1;
		}
		
		public void AddExtra(Sprite sprite)
		{
			extras.Add(sprite);
		}
		
		public void Reset()
		{
			options[selected].SetColor(.5f, .5f, .5f, 1);
			selected = 0;
			options[selected].SetColor(1, 1, 1, 1);
		}
		
		public void Render()
		{
			foreach(Sprite s in options)
				s.Render();
			foreach(Sprite s in extras)
				s.Render();
		}
	}
}

