using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AirportDEMO
{
	public enum PassengerType
		{ VIPWOLuggage, VIPWLuggage, NormalWOLuggage, NormalWLuggage}
	public enum Direction 
		{ UpLeft, Up, UpRight, Right, DownRight, Down, DownLeft, Left }

	public class Passenger : Sprite
	{
		#region Fields
		private List<Passenger> masterQueue;
		private Entrance masterEntrance;
		private float speed;
		private bool isMoving;
		private int currentCheckPoint;
		private Direction currentDirection;
		private bool IsOutOfTerminal
		{
			get { return position.Y < -50; }
		}
		#endregion

		#region Properties
		#region Public
		public bool IsPlaying { get; set; }
		public List<Vector2> CheckPoints { get; set; }
		public Vector2 NextPosition { get; set; }
		public PassengerType PassengerType { get; set; }
		#endregion

		#region Private
		// Độ dời tọa độ sau mỗi lần di chuyển.
		private Vector2 MovingOffset(Vector2 nextPosition)
		{
			Vector2 offset;
			offset.X = (float)
				(
				speed * Math.Cos(GetAngle(this.position, nextPosition))
				);

			offset.Y = (float)
				(
				speed * Math.Sin(GetAngle(this.position, nextPosition))
				);
			return offset;
		}
		#endregion
		#endregion

		#region Constructors
		/// <summary>
		/// Nếu ko truyền vào tham số millisecondsPerFrame thì sử dụng 
		/// defaultMillisecondsPerFrame = 16 (mặc định)
		/// </summary>
		public Passenger
			(
			PassengerType passengerType, Vector2 position, Direction direction,
			float drawLayer, float speed
			)
			: this
			(
			passengerType, position, direction, drawLayer, speed, defaultMillisecondsPerFrame
			)
		{
			// Bỏ trống vì đã gọi Constructor bên dưới.
		}

		/// <summary>
		/// Ngược lại, sử dụng millisecondsPerFrame từ Constructor.
		/// </summary>
		public Passenger
			(
			PassengerType passengerType, Vector2 position, Direction direction,
			float drawLayer, float speed, int millisecondsPerFrame
			)
			: base
			(position, 0.0f, drawLayer, millisecondsPerFrame)
			
		{
			this.PassengerType = passengerType;
			this.CheckPoints = new List<Vector2>();
			switch (PassengerType)
			{
				case PassengerType.VIPWLuggage:
				case PassengerType.VIPWOLuggage:
				case PassengerType.NormalWOLuggage:
					AirportMain.QueueVIP.Add(this);
					masterQueue = AirportMain.QueueVIP;
					masterEntrance = AirportMain.EntranceVIP;
					CheckPoints.Add(new Vector2(200, 330));
					CheckPoints.Add(new Vector2(235, 170));
					CheckPoints.Add(new Vector2(275, -100));
					break;
				case PassengerType.NormalWLuggage:
					if (AirportMain.QueueE1.Count <= AirportMain.QueueE2.Count)
					{
						AirportMain.QueueE1.Add(this);
						masterQueue = AirportMain.QueueE1;
						masterEntrance = AirportMain.EntranceE1;
						CheckPoints.Add(new Vector2(400, 330));
						CheckPoints.Add(new Vector2(400, 170));
						CheckPoints.Add(new Vector2(400, -100));
					}
					else
					{
						AirportMain.QueueE2.Add(this);
						masterQueue = AirportMain.QueueE2;
						masterEntrance = AirportMain.EntranceE2;
						CheckPoints.Add(new Vector2(600, 330));
						CheckPoints.Add(new Vector2(565, 170));
						CheckPoints.Add(new Vector2(525, -100));
					}
					break;
				default:
					break;
			}
			this.IsPlaying = false;
			this.NextPosition = Vector2.Zero;
			this.speed = speed;
			this.isMoving = false;
			this.currentDirection = direction;
		}
		#endregion

		#region Main_Methods
		public override void Update(GameTime gameTime)
		{
			NextPosition = CheckPoints[currentCheckPoint];
			if (currentCheckPoint < 2
				&& 
				position == CheckPoints[currentCheckPoint])
			{
				currentCheckPoint++;
			}
			double angle = GetAngle(this.position, this.NextPosition);
			this.currentDirection = (Direction)GetDirection(angle);
			this.textureImage = textureArray[(int)currentDirection];

			// Di chuyển đến địa điểm NextPosition.
			if (masterQueue == null || currentCheckPoint < 1)
			{
				MoveTo(NextPosition);
			}
			else MoveTo(GetPositionInQueue());

			if (masterQueue != null
				&&
				masterQueue.IndexOf(this) == 0
				&&
				masterEntrance != null
				&&
				masterEntrance.CurrentPassenger == null
				&&
				this.position == GetPositionInQueue())
			{
				AskForProcessing();
			}

			// Nếu đang di chuyển thì update Animation.
			if (this.isMoving)
			{
				timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
				if (timeSinceLastFrame > millisecondsPerFrame)
				{
					timeSinceLastFrame = 0;
					++currentFrame.X;
					if (currentFrame.X >= sheetSize.X)
					{
						currentFrame.X = 0;
						++currentFrame.Y;
						if (currentFrame.Y >= sheetSize.Y)
							if (isLooped)
							{
								currentFrame.Y = 0;
							}
							else
							{
								isFinishedAnimating = true;
							}
					}
				}
			}
			// Ngược lại, cho ở tư thế tĩnh.
			else
			{
				currentFrame.X = 9;
				currentFrame.Y = 2;
			}

			if (this.IsOutOfTerminal)
			{
				AirportMain.PassengersList.Remove(this);
			}
			this.scale = 1.0f - (480 - this.position.Y) / 5000.0f;
			this.drawLayer = 
				(AirportMain.PassengersList.IndexOf(this) + 1) / 1000.0f;
		}

		/// <summary>
		/// Bé bắt đầu chơi.
		/// </summary>
		public void Play()
		{
			currentCheckPoint = 0;
			this.IsPlaying = true;
		}

		public void AskForProcessing()
		{
			masterEntrance.CurrentPassenger = this;
		}

		public void GetOutOfTheQueue()
		{
			masterQueue.Remove(this);
			masterQueue = null;
			masterEntrance = null;
		}

		/// <summary>
		/// Di chuyển bé đến tọa độ xác định.
		/// </summary>
		/// <param name="destination">Tọa độ Vector2.</param>
		public void MoveTo(Vector2 destination)
		{
			if (destination != position)
			{
				this.isMoving = true;
				position = Vector2.Clamp(position + MovingOffset(destination),
					Vector2.Min(position, destination),
					Vector2.Max(position, destination));
			}
			else this.isMoving = false;
		}
		#endregion

		#region Auxiliary_Methods
		private Vector2 GetPositionInQueue()
		{
			float dis_x = Math.Abs(CheckPoints[1].X - CheckPoints[0].X) / 10;
			float dis_y = Math.Abs(CheckPoints[1].Y - CheckPoints[0].Y) / 10;
			// Vector2 a = (CheckPoints[1] - CheckPoints[0]) / 10;
			int sign = 1;
			if (CheckPoints[1].X > CheckPoints[0].X) sign = -1;
			float x = sign * dis_x * masterQueue.IndexOf(this) + CheckPoints[1].X;
			float y = dis_y * masterQueue.IndexOf(this) + CheckPoints[1].Y;
			return new Vector2(x, y);
		}

		private int GetDirection(double angle)
		{
			angle = MathHelper.WrapAngle((float)angle);
			if (double.IsNaN(angle))
			{
				return (int)currentDirection;
			}

			double lower_bound = -7.0 / 8.0 * MathHelper.Pi;
			double upper_bound = -5.0 / 8.0 * MathHelper.Pi;
			for (int i = 0; i < 7; ++i)
			{
				if (lower_bound < angle && angle <= upper_bound)
				{
					return i;
				}
				lower_bound = upper_bound;
				upper_bound += MathHelper.Pi / 4;
			}
			return 7;
		}

		/// <summary>
		/// Lấy góc quay giữa 2 điểm.
		/// </summary>
		/// <param name="originCoordinate">Điểm đầu.</param>
		/// <param name="targetCoordinate">Điểm đích.</param>
		/// <returns></returns>
		protected double GetAngle
			(Vector2 originCoordinate, Vector2 targetCoordinate)
		{
			return GetAngle
				(originCoordinate.X, originCoordinate.Y,
				targetCoordinate.X, targetCoordinate.Y);
		}

		/// <summary>
		/// Lấy góc quay giữa 2 sprites.
		/// </summary>
		/// <param name="originSprite">Sprite đầu.</param>
		/// <param name="targetSprite">Sprite đích.</param>
		/// <returns></returns>
		protected double GetAngle(Sprite originSprite, Sprite targetSprite)
		{
			return GetAngle
				(originSprite.Position, targetSprite.Position);
		}

		/// <summary>
		/// Lấy góc quay giữa 2 điểm A và B với hoành độ, tung độ xác định.
		/// </summary>
		/// <param name="originX">Hoành độ điểm A.</param>
		/// <param name="originY">Tung độ điểm A.</param>
		/// <param name="targetX">Hoành độ điểm B.</param>
		/// <param name="targetY">Tung độ điểm B.</param>
		/// <returns></returns>
		protected double GetAngle
			(double originX, double originY, double targetX, double targetY)
		{
			double temp = Math.Sqrt
				((targetX - originX) * (targetX - originX)
				+
				(targetY - originY) * (targetY - originY));
			temp = MathHelper.Clamp
				((float)((targetX - originX) / temp), -1.0f, 1.0f);
			if (targetY > originY)
				return Math.Acos(temp);
			else
				return -Math.Acos(temp);
		}
		#endregion
	}
}
