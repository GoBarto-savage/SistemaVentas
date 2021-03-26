using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaVentas
{
    public partial class Form1 : Form
    {
        private int cantidad = 0;
        private int cantidadPAdd = 0;

        private SQL sql = new SQL();
        //SqlConnection connection = new SqlConnection("Data Source=localHost;Initial Catalog=SistemasVentas;User ID=nhussein11;Password=123");
        SqlConnection connection = new SqlConnection("Data Source=localHost;Initial Catalog=SistemasVentas;User ID=gonzalo;Password=123");
        Boolean bandera = false;

        public Form1()
        {
            InitializeComponent();
            this.CenterToScreen();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                connection.Open();
                textCodigo.Focus();
            }
            catch (Exception e78)
            {
                errorProvider1.SetError(buttonAgregar, "Conexion NO Establecida");
            }
        }


        //-----------------------------------------------ELIMINACION DE PRODUCTO EN EL GRID------------------------------------------------------

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Producto producto = sql.obtenerProducto(connection, long.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()));

                sql.aumentarStock(connection, producto);
                dataGridView1.Rows.RemoveAt(e.RowIndex);
                cantidad--;
                setear_Total();
            }
            catch(Exception e4)
            {
                Console.WriteLine(e4.Message + "11");
            }

        }
        //-----------------------------------------------AGREGADO DE PRODUCTO CON LECTOR CB------------------------------------------------------
        private void buttonAgregar_Click(object sender, EventArgs e)
        {
            AgregarProducto();
        }

        private void textCodigo_keyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                AgregarProducto();
            }
        }
        //-----------------------------------------------AGREGADO DE PRODUCTO BASE DE DATOS-----------------------------------------------------
        private void buttonNuevoProd_Click(object sender, EventArgs e)
        {
            AgregarProductoBD();
        }
        //-----------------------------------------------AGREGADO DE VENTA BASE DE DATOS-----------------------------------------------------

        private void buttonAceptar_Click(object sender, EventArgs e)
        {
            AgregarVentaBD();
          
        }

        //-----------------------------------------------FUNCIONES DE AGREGADO------------------------------------------------------

        private void AgregarProducto()
        {
            try
            {
                Producto producto = sql.obtenerProducto(connection, long.Parse(textCodigo.Text));

                if (producto.Nombre.Equals(""))
                {
                    MessageBox.Show("No existe el Producto", "Advertencia");
                    textCodigo.Text = "";
                }
                else if (producto.Stock == 0)
                {
                    MessageBox.Show("Sin Stock", "Advertencia");
                    textCodigo.Text = "";
                }
                else
                {

                    DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[cantidad].Clone();
                    row.Cells[0].Value = producto.ID;
                    row.Cells[1].Value = producto.Nombre;
                    row.Cells[2].Value = producto.Descripcion;
                    row.Cells[3].Value = producto.PrecioCompra;
                    row.Cells[4].Value = producto.PrecioVenta;
                    row.Cells[5].Value = producto.Stock-1;
          
                    dataGridView1.Rows.Add(row);

                    sql.disminuirStock(connection, producto);
                    cantidad++;
                    textCodigo.Text="";
                    setear_Total();
                }
                textCodigo.Focus();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "12");
            }
         
        }
        private void AgregarProductoBD()
        {
            try
            {
                textPrecioCompra.Text = textPrecioCompra.Text.Replace(".", ",");
                textPrecioVenta.Text = textPrecioVenta.Text.Replace(".", ",");
                Producto producto = new Producto()
                {
                    ID = long.Parse(textID.Text),
                    Nombre = textNombre.Text,
                    Descripcion = textDescripcion.Text,
                    PrecioCompra = textPrecioCompra.Text,
                    PrecioVenta = textPrecioVenta.Text,
                    Stock = Int32.Parse(textStock.Text)
                };
                if (bandera == false)  //si es falso significa que si existe por lo tanto debemos actualizar
                {
                    sql.actualizarProducto(connection, producto);
                    textID.Text = "";
                    textNombre.Text = "";
                    textDescripcion.Text = "";
                    textPrecioCompra.Text = "";
                    textPrecioVenta.Text = "";
                    textStock.Text = "";
                    textID.Focus();

                    //el listadito del costado se va rellenando
                    DataGridViewRow row = (DataGridViewRow)dataGridView2.Rows[cantidadPAdd].Clone();
                    row.Cells[0].Value = producto.Nombre;
                    row.Cells[1].Value = producto.Descripcion;
                    row.Cells[2].Value = producto.PrecioCompra;
                    row.Cells[3].Value = producto.PrecioVenta;
                    row.Cells[4].Value = producto.Stock;

                    dataGridView2.Rows.Add(row);
                    cantidadPAdd++;
                }
                else//caso contrario insertamos
                {
                    try
                    {
                        sql.insertarProducto(connection, producto);
                        textID.Text = "";
                        textNombre.Text = "";
                        textDescripcion.Text = "";
                        textPrecioCompra.Text = "";
                        textPrecioVenta.Text = "";
                        textStock.Text = "";
                        textID.Focus();

                        //el listadito del costado se va rellenando
                        DataGridViewRow row = (DataGridViewRow)dataGridView2.Rows[cantidadPAdd].Clone();
                        row.Cells[0].Value = producto.Nombre;
                        row.Cells[1].Value = producto.Descripcion;
                        row.Cells[2].Value = producto.PrecioCompra;
                        row.Cells[3].Value = producto.PrecioVenta;
                        row.Cells[4].Value = producto.Stock;

                        dataGridView2.Rows.Add(row);
                        cantidadPAdd++;
                        bandera = false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message+"1");
                    }
                }
            }
            catch (Exception e4)
            {
                Console.WriteLine(e4.Message+"7");

                MessageBox.Show("Revise campos", "Advertencia");
            }



        }

        private void AgregarVentaBD()
        {
            try
            {
                int counter = 0;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if(counter == cantidad)
                    {
                        break;
                    }
                    Producto producto = sql.obtenerProducto(connection, long.Parse(row.Cells[0].Value.ToString()));

                    //sql.disminuirStock(connection, producto);
        
                    sql.insertarVenta(connection, producto);
                    sql.insertarInfoMes(connection, producto);
                    counter++;
                }
                dataGridView1.Rows.Clear();
                textTotal.Text = "$0";
                cantidad = 0;
            }
            catch (Exception e4)
            {
                Console.WriteLine(e4.Message);
            }
           
        }

      






        //-----------------------------------------------OTROS-----------------------------------------------------------------------

        private void textID_keyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    Producto producto = sql.obtenerProducto(connection, long.Parse(textID.Text));

                    if (producto.ID.ToString().Equals(textID.Text))
                    {
                        textNombre.Text = producto.Nombre;
                        textDescripcion.Text = producto.Descripcion;
                        textPrecioCompra.Text = producto.PrecioCompra;
                        textPrecioVenta.Text = producto.PrecioVenta;
                        textStock.Text = producto.Stock.ToString();
                        if (producto.Stock == 0)
                        {
                            bandera = true;
                        }
                    }

                    textNombre.Focus();
                }
                catch(Exception ee)
                {
                    Console.WriteLine(ee.Message + "10");
                }
            }
        }

        private void setear_Total()
        {
            double sum = 0.0;
            int counter = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (counter == cantidad)
                {
                    break;
                }
                try
                {
                    string value = row.Cells[4].Value.ToString();
                    sum += Convert.ToDouble(value);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "14");
                }
                counter++;
            }
            textTotal.Text = "$" + sum.ToString("N2");    
        }

        //---------------------------------------------------CARGA DE TABS VENTAS---------------------------------------------------------------------------------
      
        private void tabControlVentas_SelectedIndexCh(object sender, EventArgs e)
        {

            if (tabControlVentas.SelectedIndex == 0)
            {
                //se carga segun dia
                List<Producto> productos = sql.obtenerVentas(connection, "VentaDia");
                dataGridView4.DataSource = productos;
                dataGridView4.Columns.Remove("Stock");
                dataGridView4.Columns.Remove("ID");
                dataGridView4.Columns[4].Width = 160;
                SetearTotalYGanancias("dia");
            }
            else if (tabControlVentas.SelectedIndex == 1)
            {
                //se carga segun semana
                List<Producto> productos = sql.obtenerVentas(connection, "VentaSemana");
                dataGridView4.DataSource = productos;
                dataGridView4.Columns.Remove("Stock");
                dataGridView4.Columns.Remove("ID");
                dataGridView4.Columns[4].Width = 160;
                SetearTotalYGanancias("semana");

            }
            else
            {
                List<Producto> productos = sql.obtenerVentas(connection,"VentaMes");
                dataGridView4.DataSource = productos;
                dataGridView4.Columns.Remove("Stock");
                dataGridView4.Columns.Remove("ID");
                dataGridView4.Columns[4].Width = 160;
                SetearTotalYGanancias("mes");

            }
        }

        private void SetearTotalYGanancias(string tipo)
        {
            double TotalVenta = 0.0;
            double TotalCompra = 0.0;
            foreach (DataGridViewRow row in dataGridView4.Rows)
            {
                try
                {
                    string value = row.Cells[3].Value.ToString();
                    TotalVenta += Convert.ToDouble(value);

                    string value2 = row.Cells[2].Value.ToString();
                    TotalCompra += Convert.ToDouble(value2);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "15");
                }
                
            }
            if ("dia".Equals(tipo))
            {
                textTotalVDia.Text = "$" + TotalVenta.ToString("N2");
                textGananciasDia.Text = "$" + (TotalVenta-TotalCompra).ToString("N2");
            }else if ("semana".Equals(tipo))
            {
                textTotalVSemana.Text = "$" + TotalVenta.ToString();
                textGananciasSemana.Text = "$" + (TotalVenta - TotalCompra).ToString("N2");
            }
            else
            {
                textTotalVMes.Text = "$" + TotalVenta.ToString();
                textGananciasMes.Text = "$" + (TotalVenta - TotalCompra).ToString("N2");
            }
            
        }
        private void button1_Click(object sender, EventArgs e)//buton para limpiar
        {
           
            
            if (tabControlVentas.SelectedIndex == 0)
            {
                DialogResult dialogResult = MessageBox.Show("Desea Eliminar el Registro del día?", "Advertencia", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    sql.limpiarTabla(connection, "VentaDia");
                    textTotalVDia.Text = "$0";
                    textGananciasDia.Text = "$0";
                }
                
            }
            else if(tabControlVentas.SelectedIndex == 1)
            {
                DialogResult dialogResult = MessageBox.Show("Desea Eliminar el Registro de la semana?", "Advertencia", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    sql.limpiarTabla(connection, "VentaSemana");
                    textTotalVSemana.Text = "$0";
                    textGananciasSemana.Text = "$0";
                }
               
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Desea Eliminar el Registro del mes?", "Advertencia", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    sql.limpiarTabla(connection, "VentaMes");
                    textTotalVMes.Text = "$0";
                    textGananciasMes.Text = "$0";
                }
               
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Desea Eliminar los Registros de Todos los meses?", "Advertencia", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                sql.limpiarTablaInfoMes(connection);
                textTotalMesBD.Text = "$0";
                textTotalGananciasBD.Text = "$0";
            }
            else if (dialogResult == DialogResult.No)
            {
             
            }
           
        }

        private void comboBoxMes_SelectedIndexChanged(object sender, EventArgs e)
        {
            string mesSelec = comboBoxMes.SelectedItem.ToString().ToLower();
            Mes mes = null;
            switch (mesSelec)
            {
                case "enero":
                    mes = sql.ObtenerInfoMes(connection, 1);
                    textTotalMesBD.Text = "$"+mes.Total.ToString("N2");
                    textTotalGananciasBD.Text = "$"+mes.Ganancias.ToString("N2");
                    break;
                case "febrero":
                    mes = sql.ObtenerInfoMes(connection, 2);
                    textTotalMesBD.Text = "$" + mes.Total.ToString("N2");
                    textTotalGananciasBD.Text = "$" + mes.Ganancias.ToString("N2");
                    break;
                case "marzo":
                    mes = sql.ObtenerInfoMes(connection, 3);
                    textTotalMesBD.Text = "$" + mes.Total.ToString("N2");
                    textTotalGananciasBD.Text = "$" + mes.Ganancias.ToString("N2");
                    break;
                case "abril":
                    mes = sql.ObtenerInfoMes(connection, 4);
                    textTotalMesBD.Text = "$" + mes.Total.ToString("N2");
                    textTotalGananciasBD.Text = "$" + mes.Ganancias.ToString("N2");
                    break;
                case "mayo":
                    mes = sql.ObtenerInfoMes(connection, 5);
                    textTotalMesBD.Text = "$" + mes.Total.ToString("N2");
                    textTotalGananciasBD.Text = "$" + mes.Ganancias.ToString("N2");
                    break;
                case "junio":
                    mes = sql.ObtenerInfoMes(connection, 6);
                    textTotalMesBD.Text = "$" + mes.Total.ToString("N2");
                    textTotalGananciasBD.Text = "$" + mes.Ganancias.ToString("N2");
                    break;
                case "julio":
                    mes = sql.ObtenerInfoMes(connection, 7);
                    textTotalMesBD.Text = "$" + mes.Total.ToString("N2");
                    textTotalGananciasBD.Text = "$" + mes.Ganancias.ToString("N2");
                    break;
                case "agosto":
                    mes = sql.ObtenerInfoMes(connection, 8);
                    textTotalMesBD.Text = "$" + mes.Total.ToString("N2");
                    textTotalGananciasBD.Text = "$" + mes.Ganancias.ToString("N2");
                    break;
                case "septiembre":
                    mes = sql.ObtenerInfoMes(connection, 9);
                    textTotalMesBD.Text = "$" + mes.Total.ToString("N2");
                    textTotalGananciasBD.Text = "$" + mes.Ganancias.ToString("N2");
                    break;
                case "octubre":
                    mes = sql.ObtenerInfoMes(connection, 10);
                    textTotalMesBD.Text = "$" + mes.Total.ToString("N2");
                    textTotalGananciasBD.Text = "$" + mes.Ganancias.ToString("N2");
                    break;
                case "noviembre":
                    mes = sql.ObtenerInfoMes(connection, 11);
                    textTotalMesBD.Text = "$" + mes.Total.ToString("N2");
                    textTotalGananciasBD.Text = "$" + mes.Ganancias.ToString("N2");
                    break;
                case "diciembre":
                    mes = sql.ObtenerInfoMes(connection, 12);
                    textTotalMesBD.Text = "$" + mes.Total.ToString("N2");
                    textTotalGananciasBD.Text = "$" + mes.Ganancias.ToString("N2");
                    break;
                default:
                    MessageBox.Show("Seleccione mes", "Advertencia");
                    break;
            }
        }
        //-----------------------------------------------------------STOCK-------------------------------------------------------------

        private void buttonAgregarStock_Click(object sender, EventArgs e)
        {
            try
            {
                Producto producto = new Producto()
                {
                    ID = long.Parse(textCodigoAgregar.Text),
                    Stock = Int32.Parse(textStockAdd.Text)
                };
                sql.ponerStock(connection, producto);
                textCodigoAgregar.Text = "";
                textStockAdd.Text = "";
                textCodigoAgregar.Focus();
            }
            catch(Exception e8)
            {
                Console.WriteLine(e8.Message + "18");
            }
          

        }

        private void textCodigoAgregar_keyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    Producto producto = sql.obtenerProducto(connection, long.Parse(textCodigoAgregar.Text));

                    if (producto.Nombre.Equals(""))
                    {
                        MessageBox.Show("No existe el Producto", "Advertencia");
                        textCodigo.Text = "";
                    }
                    else
                    {
                        textStockAdd.Text = producto.Stock.ToString();
                    }
                    textStockAdd.Focus();
                }
                catch (Exception e2)
                {
                    Console.WriteLine(e2.Message + "19");
                }
            }
             
        }

        private void buttonCargar_Click(object sender, EventArgs e)
        {
            try
            {
                List<Producto> productos = sql.productosStockBajo(connection);
                dataGridView3.DataSource = productos;
                dataGridView3.Columns.Remove("ID");
                dataGridView3.Columns.Remove("PrecioCompra");
                dataGridView3.Columns.Remove("PrecioVenta");
                dataGridView3.Columns.Remove("FechaHora");
            }
            catch (Exception e6)
            {
                Console.WriteLine(e6.Message + "19");
            }
           
        }
    }
}
