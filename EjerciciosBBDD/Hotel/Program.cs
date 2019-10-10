using System;
using System.Data.SqlClient;

namespace Hotel
{
    class Program
    {
        static SqlConnection connection = new SqlConnection("Data Source=LAPTOP-IE5VLG6J\\SQLEXPRESS01;Initial Catalog=Hotel;Integrated Security=True");
        static void Main(string[] args)
        {
            Menu();
        }
        public static void Menu()
        {
            bool opcionCorrecta = true;
            int opcion = 0;
            do
            {
                opcionCorrecta = true;
                Console.WriteLine("Seleccione una opción del menú:\n" +
                    "1.Registrar Cliente\n" +
                    "2.Editar Cliente\n" +
                    "3.Check In\n" +
                    "4.Check out\n" +
                    "5.Ver Habitaciones\n" +
                    "6.Salir\n" +
                    "*********************************");
                if (Int32.TryParse(Console.ReadLine(), out opcion))
                {

                    switch (opcion)
                    {
                        case 1:
                            RegistrarCliente();
                            break;
                        case 2:
                            EditarCliente();
                            break;
                        case 3:
                            CheckIn();
                            break;
                        case 4:
                            CheckOut();
                            break;
                        case 5:
                            break;
                        case 6:
                            Console.WriteLine("Agur!");
                            break;
                        default:
                            Console.WriteLine("Introduce un número entre el 1-6.");
                            opcionCorrecta = false;
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Introduce un número entre el 1-6.");
                    opcionCorrecta = false;
                }

            } while (!opcionCorrecta);
        }
        public static void RegistrarCliente()
        {
            Console.WriteLine("Introduce tu DNI: o pulsa 0 para salir:");
            string dni = Console.ReadLine();
            if (dni == "0")
            {
                Menu();
            }
            else
            {
                if (!DNIExists(dni))
                {
                    Console.WriteLine("Introduce tu nombre:");
                    string nombre = Console.ReadLine();
                    Console.WriteLine("Introduce tu apellido:");
                    string apellido = Console.ReadLine();
                    string query = $"INSERT INTO CLIENTES VALUES('{nombre}', '{apellido}','{dni}')";
                    if (NonQuery(query))
                    {
                        Console.Clear();
                        Console.WriteLine("Usuario registrado.");
                        Menu();
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Error.");
                        Menu();
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("El DNI introducido ya esta registrado.");
                    Menu();
                }
            }
        }
        public static bool DNIExists(string dni)
        {
            string query = $"SELECT * FROM CLIENTES WHERE DNI LIKE '{dni}'";
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                connection.Close();
                return true;
            }
            connection.Close();
            return false;

        }
        public static bool NonQuery(string query)
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            if (command.ExecuteNonQuery() > 0)
            {
                connection.Close();
                return true;
            }
            connection.Close();
            return false;

        }
        public static void EditarCliente()
        {
            Console.WriteLine("Introduce tu DNI: o pulsa 0 para salir:");
            string dni = Console.ReadLine();
            if (dni == "0")
            {
                Menu();
            }
            else
            {
                if (DNIExists(dni))
                {
                    Console.WriteLine("Introduce tu nombre nuevo:");
                    string nombre = Console.ReadLine();
                    Console.WriteLine("Introduce tu apellido nuevo:");
                    string apellido = Console.ReadLine();
                    string query = $"UPDATE CLIENTES SET NOMBRE= '{nombre}',APELLIDO = '{apellido}'" +
                        $"WHERE DNI  LIKE '{dni}'";
                    if (NonQuery(query))
                    {
                        Console.Clear();
                        Console.WriteLine("Datos actualizados con éxito");
                        Menu();
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Error");
                        Menu();
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Debes estar registrado para modificar tus datos");
                    RegistrarCliente();
                }
            }
        }
        public static void CheckIn()
        {
            Console.WriteLine("Introduce tu DNI: o pulsa 0 para salir:");
            string dni = Console.ReadLine();
            if (dni == "0")
            {
                Menu();
            }
            else
            {
                if (DNIExists(dni))
                {
                    int idCliente = 0;
                    int idHabitacion = 0;
                    connection.Open();
                    string query = $"Select ID from Clientes where DNI like '{dni}'";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        idCliente = Convert.ToInt32(reader[0].ToString());
                    }
                    connection.Close();
                    connection.Open();
                    query = $"Select * from habitaciones where estado like 'l'";
                    command = new SqlCommand(query, connection);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader[0].ToString()} {reader[1].ToString()}");
                    }
                    connection.Close();
                    Console.WriteLine("*********************************");
                    Console.Write("Introduzca la habitación que desea reservar:    ");

                    if (Int32.TryParse(Console.ReadLine(), out idHabitacion))
                    {
                        connection.Open();
                        query = $"SELECT * FROM HABITACIONES WHERE ID={idHabitacion} AND ESTADO LIKE 'L'";
                        command = new SqlCommand(query, connection);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            connection.Close();
                            query = $"INSERT INTO RESERVAS(IDHABITACION,IDCLIENTE,FECHACHECKIN)" +
                                $" VALUES({idHabitacion},{idCliente},'{DateTime.Now}')";
                            if (NonQuery(query))
                            {
                                query = $"UPDATE HABITACIONES SET ESTADO = 'O' WHERE ID = '{idHabitacion}'";
                                if (NonQuery(query))
                                {
                                    Console.Clear();
                                    Console.WriteLine("Habitación reservada con éxito.");
                                    Menu();
                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine("Error");
                                    Menu();
                                    connection.Close();
                                }
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Error");
                                connection.Close();
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Introduzca un número de habitación correcto");
                            connection.Close();
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Introduzca un número de habitación correcto");
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Debes estar registrado para reservar habitaciones");
                    Menu();
                }
            }

        }
        public static void CheckOut()
        {
            Console.WriteLine("Introduce tu DNI: o pulsa 0 para salir:");
            string dni = Console.ReadLine();
            if (dni == "0")
            {
                Menu();
            }
            else
            {
                if (DNIExists(dni))
                {
                    string query = $"SELECT ID FROM CLIENTES WHERE DNI LIKE '{dni}'";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    int idCliente = 0;
                    if (reader.Read())
                    {
                        idCliente = Convert.ToInt32(reader[0].ToString());
                    }
                    connection.Close();

                    query = $"SELECT * FROM RESERVAS WHERE IDCLIENTE = {idCliente} AND FECHACHECKOUT IS NULL";
                    command = new SqlCommand(query, connection);
                    connection.Open();
                    reader = command.ExecuteReader();
                    Console.WriteLine("Número reserva | Habitación  | Fecha CheckIn");
                    Console.WriteLine("***************************************************");

                    while (reader.Read())
                    {
                        Console.WriteLine($"\t{reader[0].ToString()}  \t {reader[1].ToString()}\t{reader[3].ToString()}");
                    }
                    connection.Close();
                    Console.WriteLine("********************************");
                    Console.Write("Introduce el número de la reserva en la que hacer el check out:   ");
                    int idReserva = Convert.ToInt32(Console.ReadLine());

                    query = $"SELECT IDHABITACION FROM RESERVAS WHERE ID = {idReserva} AND FECHACHECKOUT IS NULL";
                    command = new SqlCommand(query, connection);
                    connection.Open();
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        int idHabitacion = Convert.ToInt32(reader[0].ToString());
                        connection.Close();
                        query = $"UPDATE RESERVAS SET FECHACHECKOUT = '{DateTime.Now}' WHERE ID= {idReserva}";
                        if (NonQuery(query))
                        {
                            query = $"UPDATE HABITACIONES SET ESTADO = 'L' WHERE ID = {idHabitacion}";
                            if (NonQuery(query))
                            {
                                Console.Clear();
                                Console.WriteLine("Check OUT realizado con éxito.");
                                Menu();

                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Error");
                                Menu();
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Error");
                            Menu();
                        }

                    }
                    else
                    {
                        connection.Close();
                        Console.Clear();
                        Console.WriteLine("Introduce un número de reserva correcto.");
                        CheckOut();
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Debes estar registrado para hacer el check out.");
                    Menu();
                }
            }
        }
    }
}
