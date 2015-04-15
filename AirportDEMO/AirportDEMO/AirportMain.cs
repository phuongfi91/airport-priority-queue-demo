using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AirportDEMO
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class AirportMain : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		static public List<Passenger> QueueVIP { get; set; }
		static public List<Passenger> QueueE1 { get; set; }
		static public List<Passenger> QueueE2 { get; set; }

		static public Entrance EntranceVIP { get; set; }
		static public Entrance EntranceE1 { get; set; }
		static public Entrance EntranceE2 { get; set; }

		Sprite[] receptionist;
		CounterBoard[] counterBoards;
		Texture2D counterBoard;
		Texture2D legendBoard;

		SpriteFont HUDFont;

		static public List<Passenger> PassengersList { get; set; }
		Texture2D background;
		Texture2D background_front;
		Texture2D[] passengerTexturesVIP1;
		Texture2D[] passengerTexturesVIP2;
		Texture2D[] passengerTexturesN1;
		Texture2D[] passengerTexturesN2a;
		Texture2D[] passengerTexturesN2b;

		TimeSpan randomCooldown = TimeSpan.FromSeconds(2);
		TimeSpan randomTimer;
		bool IsReadyToSpawn
		{
			get { return randomTimer > randomCooldown; }
		}

		public AirportMain()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			this.IsMouseVisible = true;
			this.Window.Title = "Airport DEMO - By Phuong D. Nguyen & Chau D. Nguyen";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			receptionist = new Sprite[3];
			PassengersList = new List<Passenger>();
			passengerTexturesVIP1 = new Texture2D[8];
			passengerTexturesVIP2 = new Texture2D[8];
			passengerTexturesN1 = new Texture2D[8];
			passengerTexturesN2a = new Texture2D[8];
			passengerTexturesN2b = new Texture2D[8];
			QueueVIP = new List<Passenger>();
			QueueE1 = new List<Passenger>();
			QueueE2 = new List<Passenger>();
			EntranceVIP = new Entrance();
			EntranceE1 = new Entrance();
			EntranceE2 = new Entrance();
			counterBoards = new CounterBoard[3];
			for (int i = 0; i < 3; ++i)
			{
				receptionist[i] = new Sprite(new Vector2(180 + i * 165, 150), 0, 0.1f, 50);
			}

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			HUDFont = Content.Load<SpriteFont>(@"Fonts\HUDFont");
			legendBoard = Content.Load<Texture2D>(@"Textures\LegendBoard");
			background = Content.Load<Texture2D>(@"Textures\Airport_Terminal");
			background_front = Content.Load<Texture2D>(@"Textures\Airport_Terminal_Boards");
			counterBoard = Content.Load<Texture2D>(@"Textures\CounterBoard");
			counterBoards[0] = new CounterBoard
				(EntranceVIP, new Vector2(135 + 0 * 175, 235), 0.5f);
			counterBoards[1] = new CounterBoard
				(EntranceE1, new Vector2(135 + 1 * 175, 235), 0.5f);
			counterBoards[2] = new CounterBoard
				(EntranceE2, new Vector2(135 + 2 * 175, 235), 0.5f);
			for (int i = 0; i < 3; ++i)
			{
				counterBoards[i].LoadTexture(counterBoard, new Point(60, 60), new Point(1, 1));
				receptionist[i].LoadTexture(Content.Load<Texture2D>(@"Textures\R\" + i),
					new Point(64, 64), new Point(9, 8));
			}
			for (int i = 0; i < 8; ++i)
			{
				passengerTexturesVIP1[i] = Content.Load<Texture2D>(@"Textures\VIP1\" + i);
				passengerTexturesVIP2[i] = Content.Load<Texture2D>(@"Textures\VIP2\" + i);
				passengerTexturesN1[i] = Content.Load<Texture2D>(@"Textures\N1\" + i);
				passengerTexturesN2a[i] = Content.Load<Texture2D>(@"Textures\N2a\" + i);
				passengerTexturesN2b[i] = Content.Load<Texture2D>(@"Textures\N2b\" + i);
			}

		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			// TODO: Add your update logic here
			RandomPassenger(gameTime);

			for (int i = 0; i < PassengersList.Count; ++i)
			{
				Passenger passenger = PassengersList[i];
				passenger.Update(gameTime);
				if (!PassengersList.Contains(passenger)) --i;
			}

			for (int i = 0; i < 3; ++i)
			{
				counterBoards[i].Update(gameTime);
				receptionist[i].Update(gameTime);
			}

			EntranceVIP.Update(gameTime);
			EntranceE1.Update(gameTime);
			EntranceE2.Update(gameTime);

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
			foreach (Passenger passenger in PassengersList)
			{
				passenger.Draw(gameTime, spriteBatch);
			}
			for (int i = 0; i < 3; ++i)
			{
				counterBoards[i].Draw(gameTime, spriteBatch);
				receptionist[i].Draw(gameTime, spriteBatch);
			}
			spriteBatch.DrawString(HUDFont, EntranceVIP.Timer.Seconds.ToString(),
				new Vector2(124 + 0 * 175, 206), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
			spriteBatch.DrawString(HUDFont, EntranceE1.Timer.Seconds.ToString(),
				new Vector2(124 + 1 * 175, 206), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
			spriteBatch.DrawString(HUDFont, EntranceE2.Timer.Seconds.ToString(),
				new Vector2(124 + 2 * 175, 206), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);

			spriteBatch.Draw(legendBoard, new Vector2(650, 40), new Rectangle(0, 0, 140, 400),
				Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1.0f);
			spriteBatch.Draw(background, Vector2.Zero, new Rectangle(0, 0, 800, 480),
				Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
			spriteBatch.Draw(background_front, Vector2.Zero, new Rectangle(0, 0, 800, 480),
				Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.5f);
			/*spriteBatch.DrawString(HUDFont, 
				Mouse.GetState().X.ToString() + "," + Mouse.GetState().Y.ToString(),
				Vector2.Zero, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);*/
			spriteBatch.End();

			base.Draw(gameTime);
		}

		private void RandomPassenger(GameTime gameTime)
		{
			randomTimer += gameTime.ElapsedGameTime;
			Random rand = new Random();
			if (IsReadyToSpawn)
			{
				randomTimer = TimeSpan.Zero;
				const int queueCapacity = 10;
				Texture2D[] passengerTextures = null;
				PassengerType passengerType;
				double type = rand.NextDouble();
				if (type < 0.1)
				{
					if (QueueVIP.Count >= queueCapacity) return;
					passengerTextures = passengerTexturesVIP1;
					passengerType = PassengerType.VIPWOLuggage;
				}
				else if(type < 0.2)
				{
					if (QueueVIP.Count >= queueCapacity) return;
					passengerTextures = passengerTexturesVIP2;
					passengerType = PassengerType.VIPWLuggage;
				}
				else if (type < 0.4)
				{
					if (QueueVIP.Count >= queueCapacity) return;
					passengerTextures = passengerTexturesN1;
					passengerType = PassengerType.NormalWOLuggage;
				}
				else
				{
					if (QueueE1.Count >= queueCapacity
						&&
						QueueE2.Count >= queueCapacity)
					{
						return;
					}
					passengerType = PassengerType.NormalWLuggage;
					int color = rand.Next(0, 2);
					switch (color)
					{
						case 0:
							passengerTextures = passengerTexturesN2a;
							break;
						case 1:
							passengerTextures = passengerTexturesN2b;
							break;
						default:
							break;
					}
				}
				PassengersList.Add(new Passenger
					((PassengerType)passengerType, new Vector2(400, 500), 
					Direction.Up, 0.5f, 0.5f));
				PassengersList[PassengersList.Count - 1].LoadTexture(passengerTextures,
					new Point(64, 64), new Point(10, 7));
			}
		}
	}
}
