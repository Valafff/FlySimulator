using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FlySimulator
{
	internal class Program
	{
		delegate bool test(int a);
		static Thread airPlaneThread(ref Airpalane plane)
		{
			Thread engineWork = new Thread(plane.MAINSTATUS);
			engineWork.Start();
			return engineWork;
		}
		static Thread DispatcherTread(ref Source exam)
		{
			Thread dispWork = new Thread(exam.doExam);
			dispWork.Start();
			return dispWork;
		}


		static void Main(string[] args)
		{
			Random rnd = new Random();
			Source Examenation = new Source();
			Examenation.StartExam();
			Console.Clear();
			Console.Write("Нажмите ENTER для начала экзамена или ESC для выхода: ");
			Examenation.Help();

			Examenation.np.engine.isStarted = false;
			Thread np_thr = airPlaneThread(ref Examenation.np);
			Thread examenation_thread = DispatcherTread(ref Examenation);


			ConsoleKey key;
			do
			{
				//Подключение агрегатов самолета
				key = Console.ReadKey(true).Key;
				if (key == ConsoleKey.Enter && Examenation.np.engine.isStarted == false)
				{
					Examenation.np.engine.isStarted = true;
					//Console.WriteLine("key I pressed");
				}
				else if (key == ConsoleKey.Enter && Examenation.np.engine.isStarted == true)
				{
					Examenation.np.engine.isStarted = false;
					Examenation.np.engine.CURRENTTHRUST = 0;
					//Console.WriteLine("key I pressed");
				}
				if (key == ConsoleKey.Spacebar && Examenation.np.gear.BrakeIsOn == false)
				{
					Examenation.np.gear.BrakeIsOn = true;
					//Console.WriteLine("key space pressed");
				}
				else if (key == ConsoleKey.Spacebar && Examenation.np.gear.BrakeIsOn == true)
				{
					Examenation.np.gear.BrakeIsOn = false;
				}
				if (key == ConsoleKey.C && Examenation.np.gear.GearHidden == false)
				{
					Examenation.np.gear.GearHidden = true;
				}
				else if (key == ConsoleKey.C && Examenation.np.gear.GearHidden == true)
				{
					Examenation.np.gear.GearHidden = false;
				}
				//Тяга двигателя
				if (key == ConsoleKey.W && Examenation.np.engine.isStarted && Examenation.np.tank.Change_Fuel_Level != 0 && Examenation.np.time_to_thrust)
				{
					Examenation.np.engine.CURRENTTHRUST += rnd.Next(500,1500);
					Examenation.np.time_to_thrust = false;

				}
				if (key == ConsoleKey.S && Examenation.np.engine.isStarted && Examenation.np.tank.Change_Fuel_Level != 0 && Examenation.np.time_to_thrust)
				{
					Examenation.np.engine.CURRENTTHRUST -= rnd.Next(500, 1500);
					Examenation.np.time_to_thrust = false;
				}
				if (key == ConsoleKey.UpArrow && Examenation.np.time_to_up)
				{
					Examenation.np.heightTable(rnd.Next(100, 300));
					Examenation.np.time_to_up = false;
				}
				if (key == ConsoleKey.DownArrow && Examenation.np.time_to_up)
				{
					Examenation.np.heightTable(-1*rnd.Next(100, 300));
					Examenation.np.time_to_up = false;
				}
				if (key == ConsoleKey.Escape)
				{
					np_thr.Abort();
					examenation_thread.Abort();
				}
				if (key == ConsoleKey.R)
				{
					Examenation.readFile();
				}




				//if (!Examenation.np.engine.isStarted)
				//{
				//	Examenation.np.engine.CURRENTTHRUST = 0;
				//}
			} while (key != ConsoleKey.Escape);



		}
	}
}
