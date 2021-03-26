using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SistemaVentas
{
    public class SQL
    {
        public SQL()
        {

        }

        //-----------------------------------------PRODUCTOS--------------------------------------------------------------------------

        public void insertarProducto(SqlConnection connection, Producto prod)
        {
            try
            {
                SqlCommand command;
                command = new SqlCommand("insert into Productos(ID,Nombre,Descripcion,PrecioCompra,PrecioVenta,Stock)" +
                    " values(@ID,@Nombre,@Descripcion,@PrecioCompra,@PrecioVenta,@Stock)", connection);

                command.Parameters.AddWithValue("@ID", prod.ID);
                command.Parameters.AddWithValue("@Nombre", prod.Nombre);
                command.Parameters.AddWithValue("@Descripcion", prod.Descripcion);
                command.Parameters.AddWithValue("@PrecioCompra", prod.PrecioCompra);
                command.Parameters.AddWithValue("@PrecioVenta", prod.PrecioVenta);
                command.Parameters.AddWithValue("@Stock", prod.Stock);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "31");
            }
           

        }
        
        public Producto obtenerProducto(SqlConnection connection,long id)
        {
            Producto producto = new Producto()
            {
                ID = id,
                Nombre = "",
                Descripcion = "",
                PrecioCompra = "0",
                PrecioVenta = "0",
                Stock = 0
            };

            try
            {
               
                SqlDataReader dataReader;
                SqlCommand command;
                command = new SqlCommand("Select * from Productos", connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    if ((long)dataReader.GetDecimal(0) == id)
                        producto = new Producto()
                        {
                            ID = (long)dataReader.GetDecimal(0),
                            Nombre = dataReader.GetString(1),
                            Descripcion = dataReader.GetString(2),
                            PrecioCompra = dataReader.GetString(3),
                            PrecioVenta = dataReader.GetString(4),
                            Stock = dataReader.GetInt32(5)
                        };
                  
                }
                dataReader.Close();
                command.Dispose();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message+"41");
            }
            return producto;

        }

        public void actualizarProducto(SqlConnection connection, Producto producto)
        {
            try
            {
                SqlCommand command;
                Console.WriteLine(producto.PrecioCompra);

                command = new SqlCommand("Update Productos set Nombre='" + producto.Nombre + "', Descripcion='" + producto.Descripcion + "',PrecioCompra='" + producto.PrecioCompra +
                    "',PrecioVenta='" + producto.PrecioVenta + "',Stock='+" + producto.Stock + "' where ID =" + producto.ID + "", connection);
                command.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message + "51");
            }
        }

        public void ponerStock(SqlConnection connection, Producto producto)
        {
            SqlCommand command;
            command = new SqlCommand("Update Productos set Stock='" + producto.Stock + "' where ID =" + producto.ID + "", connection);
            command.ExecuteNonQuery();
        }
        public void disminuirStock(SqlConnection connection, Producto producto)
        {
            SqlCommand command = new SqlCommand("Update Productos set Stock='" + (producto.Stock-1) + "' where ID =" + producto.ID + "", connection);
            command.ExecuteNonQuery();
        }
        public void aumentarStock(SqlConnection connection, Producto producto)
        {
            SqlCommand command = new SqlCommand("Update Productos set Stock='" + (producto.Stock + 1) + "' where ID =" + producto.ID + "", connection);
            command.ExecuteNonQuery();
        }

        public List<Producto> productosStockBajo(SqlConnection connection)
        {
            List<Producto> productos = new List<Producto>();
            try
            {
                SqlDataReader dataReader;
                SqlCommand command;
                command = new SqlCommand("Select * from Productos", connection);

                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    Producto producto = new Producto()
                    {
                        Nombre = dataReader.GetString(1),
                        Descripcion = dataReader.GetString(2),
                        Stock = dataReader.GetInt32(5)
                    };
                    if (producto.Stock <= 10)
                    {
                        productos.Add(producto);
                    }
                }
                dataReader.Close();
                command.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message+"45");
            }
            return productos;
        }

        //-----------------------------------------VENTAS--------------------------------------------------------------------------
        public void insertarVenta(SqlConnection connection, Producto prod)
        {
            string FyH = DateTime.Now.ToString();
            //--------------------------------insertado venta del mes-------------------------------------------------------
            SqlCommand command;
            command = new SqlCommand("insert into VentaMes(FechaHora,CodigoBarra,Nombre,Descripcion,PrecioCompra,PrecioVenta)" +
                " values(@FechaHora,@CodigoBarra,@Nombre,@Descripcion,@PrecioCompra,@PrecioVenta)", connection);

            command.Parameters.AddWithValue("@FechaHora", FyH);
            command.Parameters.AddWithValue("@CodigoBarra", prod.ID);
            command.Parameters.AddWithValue("@Nombre", prod.Nombre);
            command.Parameters.AddWithValue("@Descripcion", prod.Descripcion);
            command.Parameters.AddWithValue("@PrecioCompra", prod.PrecioCompra);
            command.Parameters.AddWithValue("@PrecioVenta", prod.PrecioVenta);
            command.ExecuteNonQuery();
            //--------------------------------insertado venta de la semana---------------------------------------------------
        
            command = new SqlCommand("insert into VentaSemana(FechaHora,CodigoBarra,Nombre,Descripcion,PrecioCompra,PrecioVenta)" +
                " values(@FechaHora,@CodigoBarra,@Nombre,@Descripcion,@PrecioCompra,@PrecioVenta)", connection);

            command.Parameters.AddWithValue("@FechaHora", FyH);
            command.Parameters.AddWithValue("@CodigoBarra", prod.ID);
            command.Parameters.AddWithValue("@Nombre", prod.Nombre);
            command.Parameters.AddWithValue("@Descripcion", prod.Descripcion);
            command.Parameters.AddWithValue("@PrecioCompra", prod.PrecioCompra);
            command.Parameters.AddWithValue("@PrecioVenta", prod.PrecioVenta);
            command.ExecuteNonQuery();
            //--------------------------------insertado venta del dia--------------------------------------------------------
            
            command = new SqlCommand("insert into VentaDia(FechaHora,CodigoBarra,Nombre,Descripcion,PrecioCompra,PrecioVenta)" +
                " values(@FechaHora,@CodigoBarra,@Nombre,@Descripcion,@PrecioCompra,@PrecioVenta)", connection);

            command.Parameters.AddWithValue("@FechaHora", FyH);
            command.Parameters.AddWithValue("@CodigoBarra", prod.ID);
            command.Parameters.AddWithValue("@Nombre", prod.Nombre);
            command.Parameters.AddWithValue("@Descripcion", prod.Descripcion);
            command.Parameters.AddWithValue("@PrecioCompra", prod.PrecioCompra);
            command.Parameters.AddWithValue("@PrecioVenta", prod.PrecioVenta);
            command.ExecuteNonQuery();
        }


        public List<Producto> obtenerVentas(SqlConnection connection,string tipo)
        {
            List<Producto> productos = new List<Producto>();
            try
            {
                SqlDataReader dataReader;
                SqlCommand command;
                command = new SqlCommand("Select * from "+tipo, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    Producto producto = new Producto()
                    {
                            FechaHora = dataReader.GetString(1),
                            ID = (long)dataReader.GetDecimal(2),
                            Nombre = dataReader.GetString(3),
                            Descripcion = dataReader.GetString(4),
                            PrecioCompra = dataReader.GetString(5),
                            PrecioVenta = dataReader.GetString(6),
                        };
                    productos.Add(producto);
                }
                dataReader.Close();
                command.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "60");
            }
            return productos;
        }

        public void limpiarTabla(SqlConnection connection, string tipo)
        {
            SqlCommand command;
            command = new SqlCommand("DELETE FROM " + tipo, connection);
            command.ExecuteNonQuery();
        }

        //-----------------------------------------TOTAL Y GANANCIAS MES--------------------------------------------------------------------------

        public void insertarInfoMes(SqlConnection connection, Producto producto)
        {
            int numMes = 0;
            string nombreMes = DateTime.Now.ToString("MMMM");
           
            switch (nombreMes)
            {
                case "enero":
                    numMes = 1;
                    break;
                case "febrero":
                    numMes = 2;
                    break;
                case "marzo":
                    numMes = 3;
                    break;
                case "abril":
                    numMes = 4;
                    break;
                case "mayo":
                    numMes = 5;
                    break;
                case "junio":
                    numMes = 6;
                    break;
                case "julio":
                    numMes = 7;
                    break;
                case "agosto":
                    numMes = 8;
                    break;
                case "septiembre":
                    numMes = 9;
                    break;
                case "octubre":
                    numMes = 10;
                    break;
                case "noviembre":
                    numMes = 11;
                    break;
                case "diciembre":
                    numMes = 12;
                    break;
                default:
                  
                    break;
            }
            try
            {
                Double Total=0.0;
                Double Ganancias=0.0;
                SqlCommand command = new SqlCommand("Select * from DataMes", connection);
                SqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    if (dataReader.GetInt32(0) == numMes)
                    {
                        Total = Convert.ToDouble(dataReader.GetString(1));
                        Ganancias = Convert.ToDouble(dataReader.GetString(2));
                      
                    }
                }
                Double Pcom= Convert.ToDouble(producto.PrecioCompra);
                Double Pven= Convert.ToDouble(producto.PrecioVenta);
                Double ganancias= Pven - Pcom;

                dataReader.Close();
                command.Dispose();

                command = new SqlCommand("Update DataMes set Total='" + (Total+ Pven) + "', Ganancias='" + (Ganancias+ ganancias) + "' where ID =" + numMes+"", connection);
                command.ExecuteNonQuery();
            }
            catch  (Exception e)
            {
                Console.WriteLine(e.Message);
            }
          

        }
        public Mes ObtenerInfoMes(SqlConnection connection, int numMes)
        {
            Mes mes=null;
            SqlCommand command = new SqlCommand("Select * from DataMes", connection);
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                if (dataReader.GetInt32(0) == numMes)
                {
                    mes = new Mes()
                    {
                        Total = Convert.ToDouble(dataReader.GetString(1)),
                        Ganancias = Convert.ToDouble(dataReader.GetString(2))
                    };
                }
            }
            dataReader.Close();
            command.Dispose();
            return mes;
        }
        public void limpiarTablaInfoMes(SqlConnection connection)
        {
            for (int i = 0; i < 13; i++)
            {
                SqlCommand command = new SqlCommand("Update DataMes set Total='" + 0 + "', Ganancias='" + 0
                + "' where ID =" + i, connection);
                command.ExecuteNonQuery();
            }
           
        }


    }
}
