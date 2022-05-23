//ShawnC Carter

using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core;

namespace ShawnProject1
{
	public class Score
	{
		public const string CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ .!?0123456789";
		private static GraphicsContext graphics;
		private static Texture2D characterSprites;
		
		private string name, displayText;
		private int amount, x, y;
		private List<Sprite> sprites;
		
		public int Amount
		{
			get { return amount; }
		}
		public string Name
		{
			get { return name; }
		}
		public string DisplayText
		{
			get { return displayText; }
		}
		public int Y
		{
			get 
			{
				return y;
			}
			set 
			{
				y = value;
				for(int i = 0; i < sprites.Count; i++)
					sprites[i].Position = new Vector3(sprites[i].Position.X, value,0);
			}
		}
		public int X
		{
			get { return x; }
			set 
			{
				x = value;
				for(int i = 0; i < sprites.Count; i++)
					sprites[i].Position = new Vector3(32 * i + value, sprites[i].Position.Y, 0);
			}
		}
		
		public Score() : this("def", 30)
		{
		}
		
		public Score(string name, int amount)
		{
			this.name = name.ToUpper();
			this.amount = amount;
			displayText = "";
			sprites = new List<Sprite>();
			
			SetText(name + "..." + amount);
		}
		
		public void SetText(string line)
		{
			line = line.ToUpper();
			this.displayText = line;
			sprites.Clear();
			for(int i = 0; i < line.Length; i++)
			{
				int index = CHARACTERS.IndexOf(line[i]);
				if (index == -1)
					index = 26;
				Sprite sprite = new Sprite(graphics, characterSprites);
				Vector2 point1 = new Vector2(index * 32, 0);
				Vector2 point2 = new Vector2((index + 1) * 32, 32);
				sprite.Scale = new Vector2(32 / sprite.Width, 32 / sprite.Height);
				sprite.SetTextureCoord(point1,point2);
				sprite.Position = new Vector3(i * 32 + x, y, 0);
				sprite.SetColor(0, 0, 0, 1);
				sprites.Add(sprite);
			}
		}
		
		public void EditCharacter(int index, char character)
		{
			if (index < sprites.Count)
			{
				character = ("" + character).ToUpper()[0];
				char[] array = displayText.ToCharArray();
				array[index] = character;
				int charIndex = CHARACTERS.IndexOf(character);
				if (charIndex == -1)
				{
					charIndex = 26;
					array[index] = ' ';
				}
				displayText = "";
				foreach (char c in array)
					displayText += c;
				Vector2 point1 = new Vector2(charIndex * 32, 0);
				Vector2 point2 = new Vector2((charIndex + 1) * 32, 32);
				sprites[index].SetTextureCoord(point1,point2);
			}
		}
		
		public void Render()
		{
			foreach(Sprite s in sprites)
				s.Render();
		}
		
		public override string ToString ()
		{
			return name + ": " + amount;
		}
		
		public static void Initialize(GraphicsContext graphicsContext, Texture2D characterTexture)
		{
			graphics = graphicsContext;
			characterSprites = characterTexture;
		}
	}
}

