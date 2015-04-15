using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AirportDEMO
{
	class CounterBoard : Sprite
	{
		#region Fields
		Color color;
		Entrance masterEntrance;
		#endregion

		#region Constructors
		/// <summary>
		/// Nếu ko truyền vào tham số millisecondsPerFrame thì sử dụng 
		/// defaultMillisecondsPerFrame = 16 (mặc định)
		/// </summary>
		public CounterBoard
			(
			Entrance masterEntrance, Vector2 position, float drawLayer
			)
			: this
			(
			masterEntrance, position, drawLayer, defaultMillisecondsPerFrame
			)
		{
			// Bỏ trống vì đã gọi Constructor bên dưới.
		}

		/// <summary>
		/// Ngược lại, sử dụng millisecondsPerFrame từ Constructor.
		/// </summary>
		public CounterBoard
			(
			Entrance masterEntrance, Vector2 position, float drawLayer,
			int millisecondsPerFrame
			)
			: base
			(position, 0.0f, drawLayer, millisecondsPerFrame)
		{
			color = Color.Red;
			this.masterEntrance = masterEntrance;
		}
		#endregion

		#region Main_Methods
		public override void Update(GameTime gameTime)
		{
			if (masterEntrance.IsFinishedProcessing)
			{
				color = Color.Lime;
			}
			else color = Color.Red;
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			// Vẽ đối tượng.
			spriteBatch.Draw
				(
				textureImage, position, new Rectangle
					(
					currentFrame.X * frameSize.X,
					currentFrame.Y * frameSize.Y,
					frameSize.X, frameSize.Y
					),
				color, RotationAngle, origin, 1.0f,
				SpriteEffects.None, drawLayer
				);
		}
		#endregion
	}
}
