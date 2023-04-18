using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;

namespace ConsoleApp52
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Getting Connection ...");
            MySqlConnection conn = DBUtils.GetDBConnection();
            String date_p;
            Console.WriteLine("Введите дату");
            date_p = Console.ReadLine();

            try
            {
                Console.WriteLine("Openning Connection ...");

                conn.Open();

                Console.WriteLine("Connection successful!");
                QueryEmployee(conn,date_p);
                updatematerial(conn, "Велмар");
                //PDV(conn);
                Not_using_material(conn);
                SUM_ORDER_DATE(conn);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            Console.Read();
        }
        private static void QueryEmployee(MySqlConnection conn, string param)
        {
            String o_date;
            String o_amount;
            String o_unit;
            String o_date_supply;
            String m_product_name;
            String sql = "select o.o_date, o.o_amount, o.o_unit,o.o_date_supply, m.m_product_name from orders o inner join material m on o.material_m_id=m.m_id " +
                " where o_date_supply=?";



            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            MySqlParameter myInParam = new MySqlParameter();
            myInParam.Value = param;
            cmd.Parameters.Add(myInParam);



            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    Console.WriteLine("|Дата   |  Сумма      | ед изм |  Дата заказа|  Название продукта|");
                    Console.WriteLine("-----------------------------------------------------------------");

                    while (reader.Read())
                    {
                        o_date = reader["o_date"].ToString().Substring(0, 10);
                        o_amount = reader["o_amount"].ToString();
                        o_unit = reader["o_unit"].ToString();
                        o_date_supply = reader["o_date_supply"].ToString();
                        m_product_name = reader["m_product_name"].ToString();


                        Console.WriteLine("|{0}|{1}|{2}|{3}|{4:40}|", o_date, o_amount, o_unit, o_date_supply, m_product_name);
                        Console.WriteLine("_____________________________________________________");


                    }
                }
            }



        }
        private static void PDV(MySqlConnection conn)
        {
            String o_code;
            String o_amount;
            String o_PDV;
            String o_date_supply;
            String m_product_name;
            String sql = "select o_code, round (sum(m.m_price*o.o_amount* case when o_unit in ('г','л') then 0.001 " +
            "when o_unit='т' then 1000 else 1 end), 2) as amount, round (sum(m.m_price*o.o_amount* case when o_unit in ('г','л') then 0.001 " +
            "when o_unit='т' then 1000 else 1 " +
            "end)*1.2, 2) as PDV " +
            "from orders o " +
            "inner join material m on o.material_m_id=m.m_id " +
            "group by o_code ";


            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
           



            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    Console.WriteLine("|Код   |Вартість замовлення |    ПДВ              | ");
                    Console.WriteLine("---------------------------------------------------");
                    while (reader.Read())
                    {
                        o_code = reader["o_code"].ToString();
                        o_amount = reader["amount"].ToString();
                        o_PDV = reader["PDV"].ToString();
                    
                     

                        Console.WriteLine("|{0,-6}|{1,-20}|{2,-20} |", o_code, o_amount, o_PDV );
                        Console.WriteLine("__________________________________________________");


                    }
                }
            }

        

        }
        private static void SUM_ORDER_DATE(MySqlConnection conn)
        {
            String o_name;
            String o_date;
            String o_order_amount;
            String sql = "select s_name, o_date, " +
                         "round (sum(m.m_price*o.o_amount* " +
                         "case when o_unit in ('г','л') then 0.001 " +
                         "when o_unit='т'               then 1000  " +
                         "else 1 " +
                         "end), 2) as order_amount " +
                         "from orders o " +
                         "inner join material m on o.material_m_id=m.m_id " +
                         "inner join supply s on s.s_id=m.Supply_s_id " +
                         "group by s_name, o_date ";

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;




            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    Console.WriteLine("|Постачальник   |Дата замовлення     |Вартість замовлення  | ");
                    Console.WriteLine("------------------------------------------------------------");
                    while (reader.Read())
                    {
                        o_name = reader["s_name"].ToString();
                        o_date = reader["o_date"].ToString();
                        o_order_amount = reader["order_amount"].ToString();



                        Console.WriteLine("|{0,-15}|{1,-20}|{2,-20} |", o_name, o_date.Substring(1,10), o_order_amount);
                        Console.WriteLine("-----------------------------------------------------------");


                    }
                }
            }
           
        

        }
        private static void Not_using_material(MySqlConnection conn)
        {
            String o_id;
            String o_product_name;
            String o_price;
            String o_product_code;
            String o_manufactor;
            String sql = "select * from material m " +
                         "where not exists(select material_m_id " +
                         "from orders " +
                         "where material_m_id=m.m_id )";


            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;




            using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    Console.WriteLine("|Код   |Назва матеріалу     |Ціна                |Код товару  | Виробник                       | ");
                    Console.WriteLine("------------------------------------------------------------------------------------------------");
                    while (reader.Read())
                    {
                        o_id = reader["m_id"].ToString();
                        o_product_name = reader["m_product_name"].ToString();
                        o_price = reader["m_price"].ToString();
                        o_product_code= reader["m_product_code"].ToString();
                        o_manufactor= reader["m_manufactor"].ToString();  



                        Console.WriteLine("|{0,-6}|{1,-20}|{2,-20}|{3, -10}|{4,-10}|", o_id, o_product_name, o_price, o_product_code, o_manufactor);
                        Console.WriteLine("------------------------------------------------------------------------------------------------");


                    }
                }
            }



        }
        private static void updatematerial(MySqlConnection conn, string param)
        {
            String o_amount;
            String o_unit;
            String o_date_supply;
            String m_product_name;
            
            String sql=" Update material set m_price= m_price*1.15 where m_id in (select s_id from supply s where s.s_name=?)";

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            MySqlParameter myInParam = new MySqlParameter();
            myInParam.Value = param;
            cmd.Parameters.Add(myInParam);



            using (DbDataReader reader = cmd.ExecuteReader()) ;


            
               
            



        }
    }
}