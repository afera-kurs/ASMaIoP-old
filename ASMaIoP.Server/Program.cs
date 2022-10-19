using System;
using System.Net;
using ASMaIoP.General.Client;
using System.Collections.Generic;
using System.Web;
using MySql.Data.MySqlClient;
using ASMaIoP.General;
using System.Data;
using System.Threading;
using System.Runtime.Intrinsics.X86;
using System.IO;
using System.Threading.Tasks;

namespace ASMaIoP.Server
{
	internal class Program
	{
		static List<UserData> lUsers = new List<UserData>();
		//mission complite

		static string defaultImagePath;
		static string ConnectionString;

        class ASMaIoP_Connection : ASMaIoP.General.Server.Connection
		{

			public override bool Process() //Метод обработки протокола
			{
                DatabaseInterface newInterface = new DatabaseInterface(ConnectionString);
				try
				{
				ProtocolId nProtoId = (ProtocolId)ReadInt(); //Получаем номер протокола от клиента 
				switch(nProtoId) //Проверяем подходит ли нам этот протокол
				{
					case ProtocolId.Auth:
						{
							string CardId = ReadString();
							Thread thread = null;

							Console.WriteLine($"[Debug]: card id recived:{CardId}");

							if (database.IsCardIdExits(CardId))
							{
								int somethingNumber = 0;
								bool IsFind = false;
								foreach(UserData _data in lUsers)
								{
									if(_data.sCardId == CardId)
									{
										IsFind = true;
										break;
									}
									somethingNumber++;
								}

								if (!IsFind)
								{
									UserData usr = new UserData { sCardId = CardId };
									Action act =
										delegate
										{
											database._connection.Open();

											MySqlCommand cmd = new MySqlCommand($"SELECT employee_ID, employee_role_ID, role_lvl FROM employee JOIN role ON employee_role_ID=role_ID JOIN cards ON cards_employee_ID=employee_ID WHERE cards_ID='{CardId}'", database._connection);
											MySqlDataReader reader = cmd.ExecuteReader();

											reader.Read();

											usr.sEmployeeId = reader[0].ToString();
											usr.sRoleId = reader[1].ToString();
											usr.nRoleLevel = Convert.ToInt32(reader[2]);

											reader.Close();

											database._connection.Close();

											lUsers.Add(usr);

											somethingNumber = lUsers.Count - 1;
										};

									thread = new Thread(
										() => act()
									);
									thread.Start();
								}
								
								Write(1);
								Write(somethingNumber);
								if(thread != null)
								thread.Join();
								Write(lUsers[somethingNumber].nRoleLevel);
							}
							else
							{
								Write(0);
							}
						}
						return false;
					case ProtocolId.DataTransfer_MyProfile:
						{
							newInterface = new DatabaseInterface(ConnectionString);

                            int nSessionId = ReadInt();
							UserData data = lUsers[nSessionId];
							string EmployeeId = data.sEmployeeId;

                            newInterface._connection.Open();

							MySqlCommand cmd = new MySqlCommand($"SELECT people_name,people_surname,people_patronymic,role_title, image_path FROM employee JOIN people ON people_ID=employee_people_ID JOIN role ON role.role_ID=employee_role_ID JOIN image ON employee_image_ID=image_ID WHERE employee_ID={EmployeeId}", newInterface._connection);

							MySqlDataReader reader = cmd.ExecuteReader();
							reader.Read();

							string Name = reader[0].ToString();
							string Surname = reader[1].ToString();
							string Patronimyc = reader[2].ToString();
							string RoleTitle = reader[3].ToString();
							string ImagePath = reader[4].ToString();

							reader.Close();

							string ProfileInfo = $"Name={Name};Surname={Surname};Patronimyc={Patronimyc};role={RoleTitle};";
                            newInterface._connection.Close();
							Write(ProfileInfo);
							if(File.Exists(ImagePath))
							{
								Write(File.ReadAllBytes(ImagePath));
							}
							else
							{
								Write(File.ReadAllBytes(defaultImagePath));
							}
							Console.WriteLine(ProfileInfo);
						}
						return false;
					case ProtocolId.DataTransfer_UpdateImage_CreateProfile:
						{
                            newInterface = new DatabaseInterface(ConnectionString);
                            int nSessionId = ReadInt();
							UserData data = lUsers[nSessionId];
							byte[] img = ReadBytes();

							string GeneratedPath = $"res/{data.sEmployeeId}.png";

							File.WriteAllBytes(GeneratedPath, img);

                            newInterface._connection.Open();

								//MySqlCommand cmd = new MySqlCommand($"UPDATE image INNER JOIN employee ON employee_image_ID=image_ID SET image_path={GeneratedPath} WHERE employee_ID={data.sEmployeeId}");
							MySqlCommand cmd = new MySqlCommand($"INSERT INTO image(image_path) VALUES('{GeneratedPath}')", newInterface._connection);

							cmd.ExecuteNonQuery();
							cmd = new MySqlCommand($"UPDATE employee SET employee_image_ID=LAST_INSERT_ID() WHERE employee_ID={data.sEmployeeId}", newInterface._connection);
							cmd.ExecuteNonQuery();
								
							newInterface._connection.Close();
						}
							return false;
					 case ProtocolId.DataTransfer_CreateProfile:
						{
							int nSessionId = ReadInt();
							UserData data = lUsers[nSessionId];
							if (data.nRoleLevel != 3 && data.nRoleLevel != 999) return false;

							database._connection.Open();

							MySqlCommand cmd = new MySqlCommand("SELECT employee_ID, people_name, people_surname FROM employee JOIN people ON employee_people_ID=people_ID", database._connection);

							DataTable table = new DataTable();
							MySqlDataAdapter adapter = new MySqlDataAdapter();
							adapter.SelectCommand = cmd;
							adapter.Fill(table);

							Write(table.Rows.Count);

							for (int i = 0; i < table.Rows.Count; i++)
							{
								Write($"{table.Rows[i].ItemArray[0]} - {table.Rows[i].ItemArray[1]} {table.Rows[i].ItemArray[2]}");
							}

							cmd = new MySqlCommand("SELECT role_ID, role_title FROM role", database._connection);

							table = new DataTable();
							adapter = new MySqlDataAdapter();
							adapter.SelectCommand = cmd;
							adapter.Fill(table);

							Write(table.Rows.Count);

							for (int i = 0; i < table.Rows.Count; i++)
							{
								Write($"{table.Rows[i].ItemArray[0]} - {table.Rows[i].ItemArray[1]}");
							}

							database._connection.Close();
						}
						return false;
					case ProtocolId.DataTransfer_Inventory:
						{
							int nSessionId = ReadInt();
							UserData data = lUsers[nSessionId];

							database._connection.Open();

							MySqlCommand cmd = new MySqlCommand($"SELECT inventory_title, inventory_description " +
								$"FROM inventory JOIN employee ON employee.employee_ID=inventory.inventory_employee_ID " +
								$"WHERE employee.employee_ID={data.sEmployeeId}", database._connection);

							DataTable table = new DataTable();
							MySqlDataAdapter adapter = new MySqlDataAdapter();
							adapter.SelectCommand = cmd;
							adapter.Fill(table);

							Write(table.Rows.Count);

							for(int i = 0; i < table.Rows.Count; i++)
							{
								Write($"{table.Rows[i].ItemArray[0]};{table.Rows[i].ItemArray[1]}");
							}

							database._connection.Close();

						}
						return false;
					case ProtocolId.DataSearchEmpl_CreateProfile:
						{
							int nSessionId = ReadInt();
							UserData data = lUsers[nSessionId];
							if (data.nRoleLevel != 3 && data.nRoleLevel != 999) return false;

							string sTargetEmployeeID = ReadString();

							database._connection.Open();

							MySqlCommand cmd = new MySqlCommand($"SELECT people_name,people_surname,people_patronymic,role_title,image_path FROM employee JOIN people ON people_ID=employee_people_ID JOIN image ON image_ID=employee_image_ID JOIN role ON role.role_ID=employee_role_ID WHERE employee_ID={sTargetEmployeeID}", database._connection);
							MySqlDataReader reader = cmd.ExecuteReader();
							reader.Read();

							string Name = reader[0].ToString();
							string Surname = reader[1].ToString();
							string Patronymic = reader[2].ToString();
							string role = reader[3].ToString();
							string ImagePath = reader[4].ToString();

							reader.Close();

							string ProfileInfo = $"Name={Name};Surname={Surname};Patronymic={Patronymic};role={role};";
							

							database._connection.Close();
							Write(ProfileInfo);
							if (File.Exists(ImagePath))
							{
								Write(File.ReadAllBytes(ImagePath));
							}
							else
							{
								Write(File.ReadAllBytes(defaultImagePath));
							}

							}
						return false;
					case ProtocolId.DataUpdateEmpl_CreateProfile:
						{
							int nSessionId = ReadInt();
							UserData data = lUsers[nSessionId];
							if (data.nRoleLevel != 3 && data.nRoleLevel != 999) return false;

							string[] _data = ReadString().Split(';');

							string sTargetEmployeeID = _data[0];
							string sTargetEmployeeRoleId = _data[1];
							string sTargetPeopleName= _data[2];
							string sTargetPeopleSurname = _data[3];
							string sTargetPeoplePatronymic = _data[4];

							database._connection.Open();
							MySqlCommand cmd = new MySqlCommand($"UPDATE employee INNER JOIN role ON employee_role_ID=role_ID INNER JOIN people ON employee_people_ID=people_ID SET people_name={sTargetPeopleName}, people_surname={sTargetPeopleSurname}, people_patronymic = {sTargetPeoplePatronymic}, employee_role_ID={sTargetEmployeeRoleId} WHERE employee_ID = {sTargetEmployeeID}", database._connection);

							int nCount = cmd.ExecuteNonQuery();
							Write(nCount > 0 ? 1 : 0);

							database._connection.Close();

						}

						return false;
					case ProtocolId.DataWriteEmpl_CreateProfile:
						{
							int nSessionId = ReadInt();
							UserData data = lUsers[nSessionId];
							if (data.nRoleLevel != 3 && data.nRoleLevel != 999) return false;

							string[] sData = ReadString().Split(';');

							string Name = sData[0];
							string SurName = sData[1];
							string Patronymic = sData[2];
							string role_ID = sData[3];
							string cardID = sData[4];

							if(database.IsCardIdExits(cardID))
							{
								Write(0);
								return false;
							}

							database._connection.Open();

							MySqlCommand cmd = new MySqlCommand($"INSERT INTO people(people_name, people_surName, people_patronymic) VALUES ('{Name}', '{SurName}', '{Patronymic}'); INSERT INTO employee(employee_role_ID,employee_people_ID) VALUES({role_ID},LAST_INSERT_ID()); INSERT INTO cards(cards_ID, cards_employee_ID) VALUES ('{cardID}', LAST_INSERT_ID())", database._connection);
							int nCount = cmd.ExecuteNonQuery();
							database._connection.Close();
							
							Write(nCount > 0 ? 1:0);
						}
						return false;
					case ProtocolId.DataDeleteEmpl_CreateProfile:
						{
							int nSessionId = ReadInt();
							UserData data = lUsers[nSessionId];
							if (data.nRoleLevel != 3 && data.nRoleLevel != 999) return false;

							string sTargetEmployeeID = ReadString();

							database._connection.Open();
							MySqlCommand cmd = new MySqlCommand($"DELETE employee, people, cards FROM cards" +
								$" JOIN employee ON employee_ID=cards_employee_ID " +
								$"JOIN people ON employee_people_ID=people_ID WHERE employee_ID ={sTargetEmployeeID}", database._connection);
							int nCount = cmd.ExecuteNonQuery();
							database._connection.Close();
							Write(nCount > 0 ? 1:0);
						}
						return false;
					case ProtocolId.DataWrite_AppItems:
						{
							int nSessionId = ReadInt();
							UserData data = lUsers[nSessionId];
							if (data.nRoleLevel != 3 && data.nRoleLevel != 999) return false;

							database._connection.Open();

							string[] _data = ReadString().Split(';');

							MySqlCommand cmd = new MySqlCommand($"INSERT INTO inventory(inventory_employee_ID, inventory_title, inventory_description) VALUES ({_data[0]},'{_data[1]}','{_data[2]}')", database._connection);
							int nCount = cmd.ExecuteNonQuery();
							database._connection.Close();
							Write(nCount > 0 ? 1 : 0);
						}
						return false;
					case ProtocolId.DataTransfer_AppItems:
						{
							int nSessionId = ReadInt();
							UserData data = lUsers[nSessionId];
							if (data.nRoleLevel != 3 && data.nRoleLevel != 999) return false;

							database._connection.Open();

							MySqlCommand cmd = new MySqlCommand("SELECT employee_ID, people_name, people_surname FROM employee JOIN people ON employee_people_ID=people_ID", database._connection);

							DataTable table = new DataTable();
							MySqlDataAdapter adapter = new MySqlDataAdapter();
							adapter.SelectCommand = cmd;
							adapter.Fill(table);

							Write(table.Rows.Count);

							for (int i = 0; i < table.Rows.Count; i++)
							{
								Write($"{table.Rows[i].ItemArray[0]} - {table.Rows[i].ItemArray[1]} {table.Rows[i].ItemArray[2]}");
							}

							database._connection.Close();
						}
						return false;
					case ProtocolId.DataLoadFromEmployeeID_AppItems:
						{
							int nSessionId = ReadInt();
							UserData data = lUsers[nSessionId];
							if (data.nRoleLevel != 3 && data.nRoleLevel != 999) return false;

							string sTaragetEmployeeID = ReadString();
							MySqlCommand cmd = new MySqlCommand($"SELECT inventory_ID, inventory_title, inventory_description FROM inventory JOIN employee ON employee_ID=inventory_employee_ID WHERE employee_ID={sTaragetEmployeeID}", database._connection);
							MySqlDataAdapter adapter = new MySqlDataAdapter();
							DataTable table = new DataTable();
							adapter.SelectCommand = cmd;
							adapter.Fill(table);

							Write(table.Rows.Count);

							for(int i = 0; i < table.Rows.Count; i++)
							{
								Write($"{table.Rows[i].ItemArray[0]};{table.Rows[i].ItemArray[1]};{table.Rows[i].ItemArray[2]}");
							}

							database._connection.Close();
						}
						return false;
					case ProtocolId.DataDelete_AppItems:
						{
							int nSessionId = ReadInt();
							UserData data = lUsers[nSessionId];
							if (data.nRoleLevel != 3 && data.nRoleLevel != 999) return false;

							string sTargetInventoryID = ReadString();

							database._connection.Open();
							MySqlCommand cmd = new MySqlCommand($"DELETE FROM inventory WHERE inventory_ID={sTargetInventoryID}", database._connection);
							int nCount = cmd.ExecuteNonQuery();
							database._connection.Close();
							Write(nCount > 0 ? 1 : 0);
						}
						return false;
					case ProtocolId.DataUpdate_AppItems:
						{
							int nSessionId = ReadInt();
							UserData data = lUsers[nSessionId];
							if (data.nRoleLevel != 3 && data.nRoleLevel != 999) return false;

							string[] _data = ReadString().Split(';');
							
							database._connection.Open();

							MySqlCommand cmd = new MySqlCommand($"UPDATE inventory SET inventory_title='{_data[1]}', inventory_description='{_data[2]}' WHERE inventory_ID={_data[0]}", database._connection);
							int nCount = cmd.ExecuteNonQuery();

							database._connection.Close();
							Write(nCount > 0 ? 1 : 0);
						}
						return false;
					case ProtocolId.DataTransfer_Tasks:
						{
							int nSessionId = ReadInt();
							UserData data = lUsers[nSessionId];

							database._connection.Open();

							string sStatesPacket = "";

                            MySqlCommand cmd = new MySqlCommand("SELECT * FROM tasks_state", database._connection);
                            MySqlDataReader reader = cmd.ExecuteReader();

							if (reader.Read())
							{
								sStatesPacket = $"{reader[0]}.{reader[1]}";

                                    while (reader.Read())
                                    {
                                        sStatesPacket += $";{reader[0]}.{reader[1]}";
                                    }
                            }

							reader.Close();

                            Write(sStatesPacket);

                            cmd = new MySqlCommand("SELECT people_name, people_surname, tasks_description, task_state_title, tasks.tasks_ID, task_state_ID FROM tasks JOIN employee ON tasks_owner_employee_ID=employee_ID JOIN people ON employee_people_ID=people_ID JOIN tasks_state ON tasks_st_ID=task_state_ID", database._connection);
							DataTable dataTable = new DataTable();
							MySqlDataAdapter Krivo_adapter = new MySqlDataAdapter();
							Krivo_adapter.SelectCommand = cmd;
							Krivo_adapter.Fill(dataTable);

							Write(dataTable.Rows.Count);

							for(int i = 0; i < dataTable.Rows.Count; i++)
							{

								MySqlCommand cmd2 = new MySqlCommand($"SELECT COUNT(*) FROM task_executant_group JOIN tasks ON tasks.tasks_ID=task_executant_group.tasks_ID WHERE tasks.tasks_ID={dataTable.Rows[i].ItemArray[4]}", database._connection);

								string Row = $"{dataTable.Rows[i].ItemArray[0]};{dataTable.Rows[i].ItemArray[1]};{dataTable.Rows[i].ItemArray[2]};{dataTable.Rows[i].ItemArray[3]};{cmd2.ExecuteScalar().ToString()};{dataTable.Rows[i].ItemArray[4]};{dataTable.Rows[i].ItemArray[5]}";

								Console.WriteLine(Row);

									//SELECT people_name, people_surname FROM people JOIN employee ON people_ID=employee_people_ID JOIN task_executant_group ON executant_emloyee_ID=employee_ID JOIN tasks ON tasks_ID=task_ID WHERE tasks_ID=1
								 Write(Row);
									//MySqlCommand cmd2 = new MySqlCommand($"SELECT people_name, people_surname FROM people JOIN employee ON people_ID=employee_people_ID JOIN task_executant_group ON executant_emloyee_ID=employee_ID JOIN tasks ON tasks_ID=task_ID WHERE tasks_ID={dataTable.Rows[i].ItemArray[4]}", database._connection);
									//DataTable dataTable2 = new DataTable();
									//MySqlDataAdapter Krivo_adapter2 = new MySqlDataAdapter();
									//Krivo_adapter2.SelectCommand = cmd2;
									//Krivo_adapter2.Fill(dataTable);

									//Write(dataTable2.Rows.Count);

									//for (int j = 0; j < dataTable2.Rows.Count; j++)
									//{
									//    Write($"{dataTable2.Rows[j].ItemArray[0]};{dataTable2.Rows[j].ItemArray[1]}");
									//}

							}

							database._connection.Close();
						}
						return false;
					case ProtocolId.DataLoadImage:
						{
							byte[] img = ReadBytes();
							File.WriteAllBytes("234", img);
						}
						return false;
					case ProtocolId.DataGetImage:
						{
							byte[] img = File.ReadAllBytes(ReadString());
							Write(img);
						}
						return false;
					case ProtocolId.GetTaskInfo:
						{
                                int nSessionId = ReadInt();
                                UserData data = lUsers[nSessionId];

                                string sTaskId = ReadString();

                                database._connection.Open();

                                MySqlCommand cmd = new MySqlCommand($"SELECT people_name, people_surname, tasks_description, task_state_title FROM tasks JOIN employee ON tasks_owner_employee_ID=employee_ID JOIN people ON employee_people_ID=people_ID JOIN tasks_state ON tasks_st_ID=task_state_ID WHERE tasks.tasks_ID={sTaskId}", database._connection);
                                MySqlDataReader reader = cmd.ExecuteReader();
                                if (reader.Read())
                                {
                                    reader[0].ToString();
                                }

                                string CreatorName = reader[0].ToString();
                                string CreatorSurname = reader[1].ToString();
                                string TaskDescription = reader[2].ToString();
                                string TaskStateTitle = reader[3].ToString();

                                reader.Close();

                                string sPacket = $"{CreatorName};{CreatorSurname};{TaskDescription};{TaskStateTitle}";
                                MySqlCommand cmd3 = new MySqlCommand($"SELECT people_name, people_surname, executant_employee_ID FROM people JOIN employee ON people_ID = employee_people_ID JOIN task_executant_group ON task_executant_group.executant_employee_ID = employee_ID JOIN tasks ON tasks.tasks_ID = task_executant_group.tasks_ID WHERE tasks.tasks_ID={sTaskId}", database._connection);
                                reader = cmd3.ExecuteReader();

                                while (reader.Read())
                                {
                                    sPacket += $";{reader[0].ToString()} {reader[1].ToString()};{reader[2].ToString()}";
                                }

                                reader.Close();
                                Write(sPacket);

                                database._connection.Close();
                            }
						return false;
					case ProtocolId.DataDelete_Tasks:
						{
							int nSessionId = ReadInt();
							UserData data = lUsers[nSessionId];
							if (data.nRoleLevel != 3 && data.nRoleLevel != 999) return false;
							string sTaskId = ReadString();
							
							database._connection.Open();
								/*
								 DELETE employee, people, cards FROM cards
								 JOIN employee ON employee_ID=cards_employee_ID
								 JOIN people ON employee_people_ID=people_ID WHERE employee_ID = 1
								 */

							MySqlCommand cmd = new MySqlCommand($"DELETE task_executant_group FROM task_executant_group WHERE tasks_ID={sTaskId};DELETE tasks FROM tasks WHERE tasks_ID={sTaskId}");
                                Write(cmd.ExecuteNonQuery() > 0 ? 1 : 0);

                                database._connection.Close();
						}
						return false;
					case ProtocolId.TaskExecutantJoin:
						{
							int nSessionId = ReadInt();
							UserData data = lUsers[nSessionId];
							database._connection.Open();

							string sTaskID = ReadString();

							MySqlCommand cmd = new MySqlCommand($"INSERT INTO task_executant_group(executant_employee_ID, tasks_ID) VALUES({data.sEmployeeId},{sTaskID})", database._connection);

							database._connection.Close();
						}
						return false;
						case General.Client.ProtocolId.DataSave_Tasks:
						{
							int nSessionId = ReadInt();
							UserData data = lUsers[nSessionId];
							if (data.nRoleLevel != 3 && data.nRoleLevel != 999) return false;

                            database._connection.Open();

                                string[] Data = ReadString().Split(';');

							MySqlCommand cmd = new MySqlCommand($"UPDATE tasks SET tasks_description='{Data[1]}', tasks_st_ID={Data[2]} WHERE tasks_ID={Data[0]}",database._connection);
                            Write(cmd.ExecuteNonQuery() > 0 ? 1 : 0);

                            database._connection.Close();
						}
						return false;
						case General.Client.ProtocolId.TaskDeleteMembers:
						{
                                int nSessionId = ReadInt();
                                UserData data = lUsers[nSessionId];
                                if (data.nRoleLevel != 3 && data.nRoleLevel != 999) return false;

                                newInterface._connection.Open();

								string sEmployeeID = ReadString();

								MySqlCommand cmd = new MySqlCommand($"DELETE task_executant_group FROM task_executant_group WHERE executant_employee_ID={sEmployeeID}", newInterface._connection);
                                Write(cmd.ExecuteNonQuery() > 0 ? 1 : 0);

								newInterface._connection.Close();
                        }
						return false;
						case General.Client.ProtocolId.DataWrite_Tasks:
							{
                                int nSessionId = ReadInt();
                                UserData data = lUsers[nSessionId];
                                if (data.nRoleLevel != 3 && data.nRoleLevel != 999) return false;

                                newInterface._connection.Open();
								string sTaskID = ReadString();
                                MySqlCommand cmd = new MySqlCommand($"INSERT INTO tasks() task_executant_group(executant_employee_ID, tasks_ID) VALUES({data.sEmployeeId},{sTaskID})", newInterface._connection);
                                Write(cmd.ExecuteNonQuery() > 0 ? 1 : 0);

                                string sTaskDescription = ReadString();

								newInterface._connection.Close();
                            }
							return false;
						default:
							return false;
					}
				}
				catch(Exception e)

                {
					if (newInterface._connection.State == ConnectionState.Open) newInterface._connection.Close();
					if (database._connection.State == ConnectionState.Open) database._connection.Close();
					Console.WriteLine($"иди фикси ошибку:{e.Message}");
					return false;
				}
				return false;
			}
		}
		static General.Config Configuration = new General.Config();//созд экземпляр класса config
		static General.Server.Server<ASMaIoP_Connection> m_Server;//объявляем переменную m_Server типа Server
		static DatabaseInterface database;

		static void Main(string[] args)
		{
			System.IO.Directory.CreateDirectory("res");
			if (Configuration.ParseFromFile("server.cfg") != General.ErrorCode.SUCCESS) //Загружаем файл server.cfg
			{
				Console.WriteLine("failed to find configuration file!");
				return;
			}
			if(!Configuration.ContaintsVariable("port")) //Ищем в cfg переменную port 
			{
				Console.WriteLine("failed to find variable \'port\'!");
				return;
			}
			if (!Configuration.ContaintsVariable("MySQL_Address")) //Ищем в cfg переменную MySQL_Address 
			{
				Console.WriteLine("failed to find variable \'MySQL_Address\'!");
				return;
			}
			if (!Configuration.ContaintsVariable("MySQL_Port")) //Ищем в cfg переменную MySQL_Port 
			{
				Console.WriteLine("failed to find variable \'MySQL_Port\'!");
				return;
			}
			if (!Configuration.ContaintsVariable("MySQL_Login")) //Ищем в cfg переменную MySQL_Login 
			{
				Console.WriteLine("failed to find variable \'MySQL_Login\'!");
				return;
			}
			if (!Configuration.ContaintsVariable("MySQL_Password")) //Ищем в cfg переменную MySQL_Password 
			{
				Console.WriteLine("failed to find variable \'MySQL_Password\'!");
				return;
			}
			if (!Configuration.ContaintsVariable("MySQL_DataBase")) //Ищем в cfg переменную MySQL_DataBase 
			{
				Console.WriteLine("failed to find variable \'MySQL_DataBase\'!");
				return;
			}

			if (!Configuration.ContaintsVariable("defaultImagePath")) //Ищем в cfg переменную MySQL_DataBase 
			{
				Console.WriteLine("failed to find variable \'defaultImagePath\'!");
				return;
			}

			defaultImagePath = Configuration["defaultImagePath"];


			string sPort = Configuration["port"];//Присвайваем переменной знанчение из cfg 

			//"server=caseum.ru;port=33333;user=test_user;" +
			// "database=db_test;password=test_pass;";
			//
			string sConnectionString = $"" +
			$"server={Configuration["MySQL_Address"]};" +
			$"port={Configuration["MySQL_Port"]};" +
			$"user={Configuration["MySQL_Login"]};" +
			$"database={Configuration["MySQL_DataBase"]};" +
			$"password={Configuration["MySQL_Password"]};";


			Console.WriteLine($"[Debug]: {sConnectionString}");

			try
			{
				ConnectionString = sConnectionString;

				database = new DatabaseInterface(sConnectionString);
				m_Server = new General.Server.Server<ASMaIoP_Connection>(short.Parse(sPort)); //Создаём экземпляр 
			}
			catch(FormatException e)
			{
				Console.WriteLine(e.Message); 
				return;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Invalid error: {e.Message}!");
				return;
			}
			catch
			{
				Console.WriteLine("Invalid error!"); 
				return;
			}

			m_Server.Start();//Запускаем сервер
			while(true)
			{
				string Cmd = Console.ReadLine(); //Получаем команду от пользвателя
				if (Cmd.Length > 0)//Проверяем чтобы строка не была пустой
				{
					string[] sWords = Cmd.Split(' '); //Делим строку по пробелам

					if (sWords.Length < 0) continue; //Проверям количество слов

					if (sWords[0] == "CmdStop") //Если 1 слово = CmdStop, выключаем сервер
					{
						m_Server.Stop();
					}
				}
			}
		}
	}
}
