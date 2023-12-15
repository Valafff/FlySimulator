using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Linq;
//using System.Text;
using System.Threading;
using System.Web;
//using System.Threading.Tasks;
//using System.Xml.Linq;
using System.Xml.Serialization;

namespace FlySimulator
{


	internal class Source
	{
		int timeToDoExecies { get; set; }
		public int airPortLenght { get; set; }
		public Pilot pilot;
		public Dispatcher dispatcher_1;
		public Dispatcher dispatcher_2;
		public Airpalane np = new Airpalane();
		public int flyPhase = 0;
		string filename;
		public void StartExam()
		{
			Console.WriteLine("Программа \"Тренажер пилота самолета\"");
			Console.Write("Введите имя экзаменуемого пилота: ");
			string name = Console.ReadLine();
			pilot = new Pilot(name);
			Console.Write("Введите имя первого диспетчера: ");
			name = Console.ReadLine();
			dispatcher_1 = new Dispatcher(name);
			Console.Write("Введите имя второго диспетчера: ");
			name = Console.ReadLine();
			dispatcher_2 = new Dispatcher(name);
			Console.Write("Введите начальный объем топлива л (0 - знач. по умолчанию): ");
			int temp;
			temp = Convert.ToInt32(Console.ReadLine());
			if (temp > 0)
			{
				np.tank.Change_Fuel_Level = temp;
			}
			else { np.tank.Change_Fuel_Level = np.tank.Volume; }
			Console.Write("Введите длину взлетно-посадочной полосы м (0 - знач. по умолчанию): ");
			temp = Convert.ToInt32(Console.ReadLine());
			if (temp >= 500 && temp <= 4000)
			{
				airPortLenght = temp;
			}
			else { airPortLenght = 3000; }
			Console.Write("Введите время на выполнения упражнения (0 - знач. по умолчанию): ");
			timeToDoExecies = Convert.ToInt32(Console.ReadLine());
			if (timeToDoExecies <= 0 || timeToDoExecies > 180)
			{
				timeToDoExecies = 60;
			}
		}
		public void Help()
		{
			int X = 60;
			int Y = 0;
			Console.SetCursorPosition(X, Y);
			Console.WriteLine("Клавиши управления самолетом:");
			Console.SetCursorPosition(X, Y + 1);
			Console.WriteLine("ENTER - запустить/остановить двигатель");
			Console.SetCursorPosition(X, Y + 2);
			Console.WriteLine("W - прибавить тягу");
			Console.SetCursorPosition(X, Y + 3);
			Console.WriteLine("S - убавить тягу");
			Console.SetCursorPosition(X, Y + 4);
			Console.WriteLine($"{Convert.ToChar(24)} - набор высоты");
			Console.SetCursorPosition(X, Y + 5);
			Console.WriteLine($"{Convert.ToChar(25)} - снижение");
			Console.SetCursorPosition(X, Y + 6);
			Console.WriteLine("SPACE - поставитть самолет на тормоз/убрать с тормоза");
			Console.SetCursorPosition(X, Y + 7);
			Console.WriteLine("C - убрать шасси/выпустить шасси");
		}
		//Взлет
		//public void Phase_0()
		//{
		//	string step0 = "Взлет разрешен, ПОСТАВЬТЕ САМОЛЕТ НА ТОРМОЗ";
		//	Commands(step0);
		//}

		//Команды
		public void Commands(string arg, Dispatcher dis)
		{
			Console.SetCursorPosition(0, 16);
			Console.WriteLine($"Комманды диспетчера {dis.Name}");
			Console.SetCursorPosition(0, 17);
			Console.WriteLine("******************************************");
			Console.SetCursorPosition(0, 18);
			Console.WriteLine($"                                                              ");
			Console.SetCursorPosition(0, 18);
			Console.WriteLine($"{arg}");
			Console.SetCursorPosition(0, 19);
			Console.WriteLine("******************************************");
			Console.SetCursorPosition(0, 20);
			Console.WriteLine($"Штрафные баллы пилота {pilot.Name}: {pilot.PenaltyPoints}");
		}
		//public void Info(string arg = "")
		//{
		//	Console.SetCursorPosition(0, 20);
		//	Console.WriteLine($"Результат предыдущего упражнения");
		//	//Console.SetCursorPosition(0, 21);
		//	//Console.WriteLine("*********************************************************");
		//	Console.SetCursorPosition(0, 22);
		//	Console.WriteLine($"                                                              ");
		//	Console.SetCursorPosition(0, 22);
		//	Console.WriteLine($"{arg}");
		//	//Console.SetCursorPosition(0, 23);
		//	//Console.WriteLine("*********************************************************");
		//}
		//Выезд за полосу
		public bool crash_t1(int crit)
		{
			if (crit >= airPortLenght && np.CURRHEIGHT == 0)
			{
				np.airplaneCrashed = true;
				pilot.exercises.Add($"САМОЛЕТ ВЫЕХАЛ ЗА ВЗЛЕТНО-ПОСАДОЧНУЮ ПОЛОСУ!");
				return true;
			}
			return false;
		}
		public void doExam()
		{
			int count = 0;
			int timeWindow = timeToDoExecies;
			int criticAlairPortLenght = 0;
			pilot.exercises.Add($"Экзаменуемый пилот: {Convert.ToString(pilot.Name)}");
			while (!np.landing)
			{
				pilot.exercises.Add($"Диспетчер №1: {Convert.ToString(dispatcher_1.Name)}");
				//установка тормоза
				do
				{
					count++;
					np.getInfo();
					if (np.engine.isStarted && !np.gear.BrakeIsOn)
					{
						timeWindow--;
						Commands(Convert.ToString($"Поставьте самолет на ТОРМОЗ(SPACE) Временное окно:{timeWindow}"), dispatcher_1);
					}
					if (np.gear.BrakeIsOn && timeWindow > 0)
					{
						pilot.exercises.Add($"Упражнение {Convert.ToString(flyPhase + 1)} выполнено на {Convert.ToString(count)} секунде полета. Тормоз был установлен.");
						flyPhase++;
					}
					else if (np.gear.BrakeIsOn && timeWindow <= 0 || np.airPlaneOnGround == false || np.CURRSPEED != 0)
					{
						pilot.exercises.Add($"Упражнение {Convert.ToString(flyPhase + 1)} НЕ выполнено на {Convert.ToString(count)} секунде полета. Тормоз не был установлен");
						pilot.PenaltyPoints += 50;
						flyPhase++;
					}
					else if (!np.gear.BrakeIsOn && timeWindow <= 0)
					{
						pilot.exercises.Add($"Упражнение {Convert.ToString(flyPhase + 1)} НЕ выполнено на {Convert.ToString(count)} секунде полета. Тормоз не был установлен");
						pilot.PenaltyPoints += 50;
						flyPhase++;
					}
					if (np.CURRSPEED != 0 && np.CURRHEIGHT == 0)
					{
						criticAlairPortLenght += (int)(np.CURRSPEED / 3.6);
					}
					if (crash_t1(criticAlairPortLenght)) { break; }

					Thread.Sleep(1000);
				}
				while (flyPhase != 1);
				if (np.airplaneCrashed) { break; }
				//Набор тяги
				timeWindow = timeToDoExecies;
				do
				{
					count++;
					np.getInfo();
					if (np.engine.CURRENTTHRUST <= 10000)
					{
						timeWindow--;
						Commands(Convert.ToString($"Наберите тягу не менее 10000 кгс Временное окно:{timeWindow}"), dispatcher_1);
					}
					if (np.engine.CURRENTTHRUST >= 10000 && timeWindow > 0)
					{
						pilot.exercises.Add($"Упражнение {Convert.ToString(flyPhase + 1)} выполнено на {Convert.ToString(count)} секунде полета. Тяга двигателя была набрана и равнялась {Convert.ToString(np.engine.CURRENTTHRUST)} кгс");
						flyPhase++;
					}
					else if (np.engine.CURRENTTHRUST <= 10000 && timeWindow <= 0 || np.airPlaneOnGround == false)
					{
						pilot.exercises.Add($"Упражнение {Convert.ToString(flyPhase + 1)} НЕ выполнено на {Convert.ToString(count)} секунде полета. Достаточная тяга двигателя не была набрана и равнялась {Convert.ToString(np.engine.CURRENTTHRUST)} кгс");
						pilot.PenaltyPoints += 50;
						flyPhase++;
					}
					if (np.CURRSPEED != 0 && np.CURRHEIGHT == 0)
					{
						criticAlairPortLenght += (int)(np.CURRSPEED / 3.6);
					}
					if (crash_t1(criticAlairPortLenght)) { break; }
					if (np.airplaneCrashed) { pilot.exercises.Add($"САМОЛЕТ РАЗБИЛСЯ"); break; }
					Thread.Sleep(1000);
				} while (flyPhase != 2);
				if (np.airplaneCrashed) { break; }

				//Проверка тяги двигателя
				if (np.engine.CURRENTTHRUST == 0)
					do
					{
						count++;
						np.getInfo();
						Commands(Convert.ToString($"Увеличте тягу двигателей!"), dispatcher_1);
						Thread.Sleep(1000);
					} while (np.engine.CURRENTTHRUST == 0);

				//Взлет
				timeWindow = timeToDoExecies;
				do
				{
					count++;
					timeWindow--;
					np.getInfo();
					Commands(Convert.ToString($"Отпустите тормоз и взлетайте Временное окно:{timeWindow}"), dispatcher_1);
					if (!np.gear.BrakeIsOn)
					{
						criticAlairPortLenght += (int)(np.CURRSPEED / 3.6);
					}
					if (crash_t1(criticAlairPortLenght)) { break; }
					if (np.airplaneInAir && timeWindow > 0)
					{
						pilot.exercises.Add($"Упражнение {Convert.ToString(flyPhase + 1)} выполнено на {Convert.ToString(count)} секунде полета. Тормоз был отпущен.");
						flyPhase++;
					}
					else if (!np.airplaneInAir && timeWindow <= 0)
					{
						pilot.exercises.Add($"Упражнение {Convert.ToString(flyPhase + 1)} НЕ выполнено на {Convert.ToString(count)} секунде полета. Тормоз не был отпущен в заданное время.");
						pilot.PenaltyPoints += 50;
						flyPhase++;
					}
					if (np.airplaneCrashed) { pilot.exercises.Add($"САМОЛЕТ РАЗБИЛСЯ"); break; }
					Thread.Sleep(1000);
				} while (flyPhase != 3);
				if (np.airplaneCrashed) { break; }

				//Проверка высоты
				if (np.CURRHEIGHT == 0)
					do
					{
						count++;
						np.getInfo();
						Commands(Convert.ToString($"Набирайте высоту!!! Осталось {airPortLenght - criticAlairPortLenght} метров"), dispatcher_1);
						if (np.CURRSPEED != 0 && np.CURRHEIGHT == 0)
						{
							criticAlairPortLenght += (int)(np.CURRSPEED / 3.6);
						}
						if (crash_t1(criticAlairPortLenght)) { break; }
						Thread.Sleep(1000);
					} while (np.CURRHEIGHT == 0);
				if (np.airplaneCrashed) { pilot.exercises.Add($"САМОЛЕТ РАЗБИЛСЯ"); break; }

				//Полет шасси
				timeWindow = timeToDoExecies;
				do
				{
					count++;
					timeWindow--;
					np.getInfo();
					Commands(Convert.ToString($"Уберите шасси Временное окно:{timeWindow}"), dispatcher_1);
					if (np.gear.GearHidden && timeWindow > 0)
					{
						pilot.exercises.Add($"Упражнение {Convert.ToString(flyPhase + 1)} выполнено на {Convert.ToString(count)} секунде полета. Шасси были убраны.");
						flyPhase++;
					}
					else if (!np.gear.GearHidden && timeWindow <= 0)
					{
						pilot.exercises.Add($"Упражнение {Convert.ToString(flyPhase + 1)} НЕ выполнено на {Convert.ToString(count)} секунде полета. Шасси не были убраны.");
						pilot.PenaltyPoints += 50;
						flyPhase++;
					}
					if (np.airplaneCrashed) { pilot.exercises.Add($"САМОЛЕТ РАЗБИЛСЯ"); break; }
					Thread.Sleep(1000);
				} while (flyPhase != 4);
				if (np.airplaneCrashed) { break; }
				//Полет корректровка высоты диспетчер 1
				timeWindow = timeToDoExecies;
				Random rnd = new Random();
				int temp_h = 7 * np.CURRSPEED - (200 - rnd.Next(0, 400));
				do
				{
					count++;
					timeWindow--;
					Commands(Convert.ToString($"Рекомендованная высота: {temp_h} Временное окно:{timeWindow}"), dispatcher_1);
					if (np.CURRHEIGHT >= temp_h - 200 && np.CURRHEIGHT <= temp_h + 200 && timeWindow > 0)
					{
						pilot.exercises.Add($"Упражнение {Convert.ToString(flyPhase + 1)} выполнено на {Convert.ToString(count)} секунде полета. Была достигнута рекомендованная высота: {Convert.ToString(temp_h)} м Фактическая высота: {Convert.ToString(np.CURRHEIGHT)} м");
						flyPhase++;

					}
					else if (Math.Abs(np.CURRHEIGHT - temp_h) >= 300 && Math.Abs(np.CURRHEIGHT - temp_h) < 600 && timeWindow < 0)
					{
						pilot.exercises.Add($"Высота НЕ выдержана на {Convert.ToString(count)} секунде полета. Не была достигнута рекомендованная высота: {Convert.ToString(temp_h)} м Фактическая высота: {Convert.ToString(np.CURRHEIGHT)} м Отклонение не более 600 м");
						pilot.PenaltyPoints += 25;
						flyPhase++;

					}
					else if (Math.Abs(np.CURRHEIGHT - temp_h) >= 600 && Math.Abs(np.CURRHEIGHT - temp_h) < 1000 && timeWindow < 0)
					{
						pilot.exercises.Add($"Высота НЕ выдержана на {Convert.ToString(count)} секунде полета. Не была достигнута рекомендованная высота: {Convert.ToString(temp_h)} м Фактическая высота: {Convert.ToString(np.CURRHEIGHT)} м Отклонение не более 1000 м");
						pilot.PenaltyPoints += 50;
						flyPhase++;

					}
					else if (Math.Abs(np.CURRHEIGHT - temp_h) >= 1000 && timeWindow < 0)
					{
						np.airplaneCrashed = true;
						pilot.exercises.Add($"САМОЛЕТ РАЗБИЛСЯ! Не была достигнута рекомендованная высота {Convert.ToString(np.CURRHEIGHT)} отклонение более 1000 м");
						break;
					}
					if (np.airplaneCrashed) { pilot.exercises.Add($"САМОЛЕТ РАЗБИЛСЯ"); break; }
					np.getInfo();
					Thread.Sleep(1000);

				} while (flyPhase != 5);
				if (np.airplaneCrashed) { break; }
				pilot.exercises.Add($"Диспетчер №2: {Convert.ToString(dispatcher_2.Name)}");
				//Полет корректровка высоты диспетчер 2
				timeWindow = timeToDoExecies;
				temp_h = 7 * np.CURRSPEED - (200 - rnd.Next(0, 400));
				do
				{
					count++;
					timeWindow--;
					Commands(Convert.ToString($"Рекомендованная высота: {temp_h} Временное окно:{timeWindow}"), dispatcher_2);
					if (np.CURRHEIGHT >= temp_h - 200 && np.CURRHEIGHT <= temp_h + 200 && timeWindow > 0)
					{
						pilot.exercises.Add($"Упражнение {Convert.ToString(flyPhase + 1)} выполнено на {Convert.ToString(count)} секунде полета. Была достигнута рекомендованная высота: {Convert.ToString(temp_h)} м Фактическая высота: {Convert.ToString(np.CURRHEIGHT)} м");
						flyPhase++;
					}
					else if (Math.Abs(np.CURRHEIGHT - temp_h) >= 300 && Math.Abs(np.CURRHEIGHT - temp_h) < 600 && timeWindow < 0)
					{
						pilot.exercises.Add($"Высота НЕ выдержана на {Convert.ToString(count)} секунде полета. Не была достигнута рекомендованная высота: {Convert.ToString(temp_h)} м Фактическая высота: {Convert.ToString(np.CURRHEIGHT)} м Отклонение не более 600 м");
						pilot.PenaltyPoints += 25;
						flyPhase++;

					}
					else if (Math.Abs(np.CURRHEIGHT - temp_h) >= 600 && Math.Abs(np.CURRHEIGHT - temp_h) < 1000 && timeWindow < 0)
					{
						pilot.exercises.Add($"Высота НЕ выдержана на {Convert.ToString(count)} секунде полета. Не была достигнута рекомендованная высота: {Convert.ToString(temp_h)} м Фактическая высота: {Convert.ToString(np.CURRHEIGHT)} м Отклонение не более 1000 м");
						pilot.PenaltyPoints += 50;
						flyPhase++;

					}
					else if (Math.Abs(np.CURRHEIGHT - temp_h) >= 1000 && timeWindow < 0)
					{
						np.airplaneCrashed = true;
						pilot.exercises.Add($"САМОЛЕТ РАЗБИЛСЯ! Не была достигнута рекомендованная высота {Convert.ToString(np.CURRHEIGHT)} отклонение более 1000 м");
						break;
					}
					if (np.airplaneCrashed) { pilot.exercises.Add($"САМОЛЕТ РАЗБИЛСЯ"); break; }
					np.getInfo();
					Thread.Sleep(1000);

				} while (flyPhase != 6);
				if (np.airplaneCrashed) { break; }

				//Посадка
				temp_h = 800;
				timeWindow = timeToDoExecies;
				do
				{
					count++;
					timeWindow--;
					Commands(Convert.ToString($"Снижайтесь до отметки {temp_h} м Временное окно:{timeWindow}"), dispatcher_2);
					if (np.CURRHEIGHT <= temp_h + 200 && np.CURRHEIGHT >= temp_h - 200 && timeWindow > 0)
					{
						pilot.exercises.Add($"Упражнение {Convert.ToString(flyPhase + 1)} выполнено на {Convert.ToString(count)} секунде полета. Была достигнута рекомендованная высота: {Convert.ToString(temp_h)} м Фактическая высота: {Convert.ToString(np.CURRHEIGHT)} м");
						flyPhase++;
					}
					else if ((np.CURRHEIGHT >= temp_h + 200 || np.CURRHEIGHT <= temp_h - 200) && timeWindow <= 0)
					{
						pilot.exercises.Add($"Высота НЕ выдержана на {Convert.ToString(count)} секунде полета. Не была достигнута рекомендованная высота: {Convert.ToString(temp_h)} м Фактическая высота: {Convert.ToString(np.CURRHEIGHT)} м");
						pilot.PenaltyPoints += 50;
						flyPhase++;
					}
					if (np.airplaneCrashed) { pilot.exercises.Add($"САМОЛЕТ РАЗБИЛСЯ"); break; }
					np.getInfo();
					Thread.Sleep(1000);
				} while (flyPhase != 7);
				if (np.airplaneCrashed) { break; }

				//Посадка - снизить скорость и тягу
				timeWindow = timeToDoExecies;
				temp_h = 350;
				do
				{
					count++;
					timeWindow--;
					Commands(Convert.ToString($"Снизьте скорость до {temp_h} км/ч и тягу до {temp_h * 15.78} Временное окно:{timeWindow}"), dispatcher_2);
					if (np.CURRSPEED <= temp_h + 100 && np.CURRSPEED >= temp_h - 100 && timeWindow > 0 && np.engine.CURRENTTHRUST < temp_h * 15.78)
					{
						pilot.exercises.Add($"Упражнение {Convert.ToString(flyPhase + 1)} выполнено на {Convert.ToString(count)} секунде полета. Была достигнута скорость {Convert.ToString(np.CURRSPEED)} км/час и тяга {Convert.ToString(np.engine.CURRENTTHRUST)} кгс");
						flyPhase++;
					}
					else if (/*(np.CURRSPEED >= temp_h + 200 || np.CURRSPEED <= temp_h - 200) &&*/ timeWindow <= 0 && np.engine.CURRENTTHRUST > temp_h * 15.78)
					{
						pilot.exercises.Add($"Скорость НЕ выдержана на {Convert.ToString(count)} секунде полета. Фактическая скорость {Convert.ToString(np.CURRSPEED)} км/час и тяга {Convert.ToString(np.engine.CURRENTTHRUST)} кгс не соответствовали рекомендуемым 350 км/ч и 5512 кгс");
						pilot.PenaltyPoints += 100;
						flyPhase++;
					}
					if (np.airplaneCrashed) { pilot.exercises.Add($"САМОЛЕТ РАЗБИЛСЯ"); break; }
					np.getInfo();
					Thread.Sleep(1000);
				} while (flyPhase != 8);
				if (np.airplaneCrashed) { break; }
				// Посадка - выпустить шасси
				timeWindow = timeToDoExecies;
				do
				{
					count++;
					timeWindow--;
					Commands(Convert.ToString($"Выпускайте шасси Временное окно:{timeWindow}"), dispatcher_2);
					if (!np.gear.GearHidden && timeWindow > 0)
					{
						pilot.exercises.Add($"Упражнение {Convert.ToString(flyPhase + 1)} выполнено на {Convert.ToString(count)} секунде полета. Шасси были выпущены.");
						flyPhase++;
					}
					else if (np.gear.GearHidden && timeWindow <= 0)
					{
						np.airplaneCrashed = true;
						pilot.exercises.Add($"САМОЛЕТ РАЗБИЛСЯ! Шасси не были выпущены");
						break;
					}
					if (np.airplaneCrashed) { pilot.exercises.Add($"САМОЛЕТ РАЗБИЛСЯ! Шасси не были выпущены"); break; }
					np.getInfo();
					Thread.Sleep(1000);
				} while (flyPhase != 9);
				if (np.airplaneCrashed) { break; }
				// Посадка 
				timeWindow = timeToDoExecies;
				do
				{
					count++;
					timeWindow--;
					Commands(Convert.ToString($"Посадка разрешена Временное окно:{timeWindow}"), dispatcher_2);
					np.landing = true;
					if (np.CURRHEIGHT == 0 && timeWindow >= 0 && !np.airplaneCrashed)
					{
						pilot.exercises.Add($"Упражнение {Convert.ToString(flyPhase + 1)} выполнено на {Convert.ToString(count)} секунде полета. Посадка прошла успешно");
						pilot.exercises.Add($"САМОЛЕТ СЕЛ!");
						Console.WriteLine("САМОЛЕТ СЕЛ!");
						break;
					}
					else if (np.CURRHEIGHT == 0 || np.CURRSPEED == 0 && timeWindow <= 0)
					{
						np.airplaneCrashed = true;
						pilot.exercises.Add($"САМОЛЕТ РАЗБИЛСЯ ПРИ ПОСАДКЕ!");
						break;
					}
					if (np.airplaneCrashed) { pilot.exercises.Add($"САМОЛЕТ РАЗБИЛСЯ"); break; }
					np.getInfo();
					Thread.Sleep(1000);
				}
				while (true);
				if (np.airplaneCrashed) { break; }
			}
			//Разбор полетов
			if (np.airplaneCrashed) { pilot.PenaltyPoints += 1000; }
			pilot.exercises.Add($"Экзаменуемый пилот набрал {Convert.ToString(pilot.PenaltyPoints)} штрафных баллов");
			if (pilot.PenaltyPoints < 1000) { pilot.exercises.Add($"Пилот {Convert.ToString(pilot.Name)} допущен к полетам"); }
			else { pilot.exercises.Add($"Пилот {Convert.ToString(pilot.Name)} непригоден к полетам"); }

			//Запись в файл
			XmlSerializer pilotCard = new XmlSerializer(typeof(List<string>));
			filename = $"Pilot_card_{Convert.ToString(DateTime.Now.ToShortDateString())}_{Convert.ToString(DateTime.Now.Hour)}_{Convert.ToString(DateTime.Now.Minute)}.xml";
			try
			{
				using (Stream pilotCardStream = File.Create(filename))
				{
					pilotCard.Serialize(pilotCardStream, pilot.exercises);
				}
				Console.Clear();
				Console.WriteLine("Отчет сохранен");
				Console.WriteLine("Нажмите ESC для выхода или R для чтения последнего файла");
			}
			catch (Exception)
			{
				throw;
			}
		}
		public void readFile()
		{
			XmlSerializer readPilotCard = new XmlSerializer(typeof(List<string>));
			List<string> templist = null;
			using (Stream fsteread = File.OpenRead(filename))
			{
				templist = (List<string>)readPilotCard.Deserialize(fsteread);
			}
			foreach (string temp in templist)
			{
				Console.WriteLine(temp);
			}
			Console.WriteLine("\nНажмите ESC для выхода из программы");
		}
	}
}
