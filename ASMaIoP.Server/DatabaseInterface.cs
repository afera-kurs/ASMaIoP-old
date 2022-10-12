using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;


namespace ASMaIoP.Server
{
    internal class DatabaseInterface
    {
        public readonly MySqlConnection _connection;

        public DatabaseInterface(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
        }

        public bool IsCardIdExits(string CardId)
        {
            try
            {
                _connection.Open();

                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM cards WHERE cards_ID = '{CardId}'", _connection);
                DataTable table = new DataTable();
                
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.Fill(table);

                int nCount = table.Rows.Count;

                _connection.Close();
                return nCount > 0;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public int GetRoleLevel(string EmployeeId)
        {
            try
            {
                _connection.Open();

                MySqlCommand cmd = new MySqlCommand($"SELECT role_lvl FROM role JOIN employee ON employee_role_ID=role_ID WHERE employee_ID = '{EmployeeId}'", _connection);

                string sId = cmd.ExecuteScalar().ToString();
                _connection.Close();

                return Convert.ToInt32(sId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }

        }

        public string GetEmployeeId(string CardId)
        {
            try
            {
                _connection.Open();

                MySqlCommand cmd = new MySqlCommand($"SELECT cards_employee_ID FROM cards WHERE cards_ID = '{CardId}'", _connection);

                string sId = cmd.ExecuteScalar().ToString();
                _connection.Close();

                return sId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "";
            }
        }
    }
}
