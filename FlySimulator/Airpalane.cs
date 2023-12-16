using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlySimulator
{

	internal class Airpalane
	{

		public Tank tank = new Tank(50000, 1000);
		public JetEngine engine = new JetEngine(15000, 0, 1);
		public LandingGear gear = new LandingGear(false);
		//ЧЕРНЫЙ ЯЩИК КУДА ПИШУТСЯ СОБЫТИЯ. ЧЕРНЫЙ ЯЩИК ПОДПИСАН НА РАЗНЫЕ СОБЫТИЯ: создал делегат, создал событие, подписал метод record на событие(файл source), указал условие вызова события
		public flightRecorder recorder = new flightRecorder();
		public delegate void ToRecord(bool arg_airPlaneOnGround, bool arg_airplaneInAir, int arg_speed, int arg_height, double arg_fuel, double arg_consumption);
		public event ToRecord toRecordEvent;
		
		readonly double max_speed;
		readonly int max_height;

		int currHeight;
		public bool time_to_thrust = false;
		public bool time_to_up = false;
		public bool landing = false;
		public bool getout = false;

		//Фазы полета
		public bool airplaneCrashed { get; set; }
		public bool airplaneInAir { get; set; }
		public bool airPlaneOnGround { get; set; }

		public double MAX_SPEED { get { return max_speed; } }
		public int MAX_HEIGHT { get { return max_height; } }
		public int CURRSPEED { get; set; }
		public int POSSIBLEHEIGHT { get; set; }
		//public int CURRHEIGHT { get; set; }
		public int CURRHEIGHT
		{
			get { return currHeight; }
			set
			{
				if (value < 0 && CURRHEIGHT <= 0) { currHeight = 0; }
				else if (value > 0 && CURRHEIGHT >= MAX_HEIGHT) { currHeight = MAX_HEIGHT; }
				else { currHeight = value; }
			}
		}
		public double CURRAIRFRICTION { get; set; }

		public Airpalane( /*Tank arg_tank, JetEngine arg_engine, LandingGear arg_gear,*/ int max_speed = 950, int max_h = 12000)
		{
			this.max_speed = max_speed;
			max_height = max_h;
			airPlaneOnGround = true;
			airplaneCrashed = false;
			airplaneInAir = false;
		}
		public void myClear(int startX, int startY, int row = 15, int col = 60)
		{
			char symbol = ' ';
			Console.SetCursorPosition(startX, startY);
			for (int i = 0; i < row; i++)
			{
				for (int j = 0; j < col; j++)
				{
					Console.Write(symbol);
				}
				Console.WriteLine();
			}
		}
		public void speedTable()
		{
			double percent = engine.CURRENTTHRUST / engine.ENGINEMAXTHRUST;
			if (!gear.BrakeIsOn /*&& CURRHEIGHT != 0*/) { CURRSPEED = (int)(percent * MAX_SPEED); }
		}

		//Метод который вызывает событие - записать данные в черный ящик
		public void mainEventChanges()
		{
			if (recorder.temp.controlSpeed != CURRSPEED ||
				recorder.temp.controlHeight != CURRHEIGHT ||
				recorder.temp.controlFuelLevel != tank.Change_Fuel_Level ||
				recorder.temp.controlEngineThrust != engine.CURRENTTHRUST ||
				recorder.temp.inAir != airplaneInAir ||
				recorder.temp.onGround != airPlaneOnGround)
			{
				toRecordEvent(airPlaneOnGround, airplaneInAir, CURRSPEED, CURRHEIGHT, tank.Change_Fuel_Level, engine.CURRENTTHRUST);
			}
		}


		public void heightTable(int add_h)
		{
			double percent = engine.CURRENTTHRUST / engine.ENGINEMAXTHRUST;
			POSSIBLEHEIGHT = (int)(percent * MAX_HEIGHT);

			if (add_h > 0 && CURRHEIGHT < POSSIBLEHEIGHT && CURRSPEED != 0)
			{
				CURRHEIGHT += add_h;
			}
			else if (add_h < 0 && CURRHEIGHT > 0)
			{
				CURRHEIGHT += add_h;
			}
			if (CURRHEIGHT < 0) { CURRHEIGHT = 0; }

			if (CURRHEIGHT > 0) { airPlaneOnGround = false; airplaneInAir = true; }
			else { airPlaneOnGround = true; }
		}



		public void getInfo(int x = 0, int y = 0)
		{
			engine.consumptionTable();
			speedTable();
			myClear(x, y);
			Console.CursorVisible = false;
			Console.SetCursorPosition(20, 0);
			Console.WriteLine("Телеметрия:");
			Console.WriteLine();
			if (engine.isStarted) { Console.ForegroundColor = ConsoleColor.Green; }
			Console.WriteLine(engine);
			Console.ResetColor();
			Console.WriteLine("********************************");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Скорость самолета {CURRSPEED} км/ч");
			Console.WriteLine($"Высота полета {CURRHEIGHT} м");
			Console.ResetColor();
			Console.WriteLine("********************************");
			if (tank.Change_Fuel_Level <= 2000) { Console.ForegroundColor = ConsoleColor.Red; }
			else { Console.ForegroundColor = ConsoleColor.Green; }
			Console.WriteLine(tank);
			Console.ResetColor();
			Console.WriteLine("********************************");
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine(gear);
			Console.ResetColor();
			//ПРОВЕРКА НА ИЗМЕНЕНИЕ СВОЙСТВ САМОЛЕТА И ЕГО УЗЛОВ В СЛУЧАЕ ИЗМЕНЕНИЯ ВЫЗЫВАЕТСЯ СОБЫТИЕ СДЕЛАТЬ ЗАПИСЬ В ЧЕРНЫЙ ЯЩИК
			mainEventChanges();
		}


		public void MAINSTATUS()
		{
			Random rnd = new Random();
			bool wasStarted = false;
			do
			{
				//Если двигатель работает и потребляет топливо
				while (engine.isStarted && tank.giveFuel(engine.CONSUMPTIONPERSECOND) > 0 && !airplaneCrashed)
				{
					//getInfo();
					time_to_thrust = true;
					time_to_up = true;
					wasStarted = true;
					if (engine.CURRENTTHRUST == 0)
					{
						CURRHEIGHT -= rnd.Next(300, 400);
						if (CURRHEIGHT <= 0 && airplaneInAir == true && !landing) { airplaneCrashed = true; break; }
					}
					if (airplaneInAir && CURRHEIGHT <= 0 && !landing) { airplaneCrashed = true; break; }
					Thread.Sleep(1000);
				}
				//Console.WriteLine("OK");
				if (tank.Change_Fuel_Level == 0)
				{
					//Console.WriteLine("Топливо закончилось");
					engine.isStarted = false;
					//getout = true;
				}
				if (wasStarted && engine.isStarted == false)
				{
					//Console.WriteLine("Двигатель отключился");
					//getout = true;
					//getInfo();
				}
				if (!engine.isStarted || engine.CURRENTTHRUST == 0)
				{
					CURRHEIGHT -= rnd.Next(300, 400);
					if (CURRHEIGHT <= 0 && airplaneInAir == true && !landing) { airplaneCrashed = true; }
				}
				if (airplaneCrashed && !landing)
				{
					Console.WriteLine("САМОЛЕТ ПОТЕРПЕЛ КРУШЕНИЕ");
					break;
				}
				Thread.Sleep(1000);
			} while (!getout);
		}
	}
}
