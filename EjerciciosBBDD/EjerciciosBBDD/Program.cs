using System;
using System.Data.SqlClient;

namespace EjerciciosBBDD
{
    class Program
    {
        static SqlConnection connection = new SqlConnection("Data Source=LAPTOP-IE5VLG6J\\SQLEXPRESS01;Initial Catalog=Northwind;Integrated Security=True");
        static void Main(string[] args)
        {
            string country;
            bool apellidocorrecto = false; // booleano para la condicion while
            do
            {
                Console.WriteLine("Whats your lastname?");
                string lastname = Console.ReadLine().ToLower();
                string query = $"Select lastname from employees where lastname = '{lastname}'";
                 SqlCommand command = new SqlCommand(query, connection);

                connection.Open(); //abrimos la conexion

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                  
                    connection.Close();
                    apellidocorrecto = true;
                    Console.WriteLine("Tu apellido existe, a que país quieres cambiar?");
                    country = Console.ReadLine().ToUpper();
                    query = $"update employees set country = '{country}' where lastname = '{lastname}'";
                    command = new SqlCommand(query, connection);
                    connection.Open(); // la abrimos
                    Console.WriteLine(command.ExecuteNonQuery() + " rows affected");
                    connection.Close();
                    Console.WriteLine("Tu país se ha actualizado");
                    
                    query = $"Select * from Employees where LastName like '{lastname}'";
                    command = new SqlCommand(query, connection);
                    connection.Open(); // la abrimos
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["LastName"].ToString()} {reader["Country"].ToString()}");
                    }
                    connection.Close();
                }
                else
                {
                    Console.WriteLine("Tu apellido no existe en nuestra base de datos");
                    connection.Close();
                }
            } while (!apellidocorrecto);



        }
    }
}
