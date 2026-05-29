using Gestor_de_Productos_WTF;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GestorProductosWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private GestorProductos gestor = new GestorProductos();
        public MainWindow() {
            InitializeComponent();
            CargarDatosIniciales();
            dataGridProductos.ItemsSource = gestor.ObtenerListaProductos();
            comboTipoBusqueda.Items.Add("ID");
            comboTipoBusqueda.Items.Add("Nombre");
            //comboTipoBusqueda.Items.Add("Codigo Barras");
            comboTipoBusqueda.SelectedIndex = 0;

            comboCriterioOrden.Items.Add("ID");
            comboCriterioOrden.Items.Add("Nombre");
            comboCriterioOrden.Items.Add("Precio");
            comboCriterioOrden.SelectedIndex = 0;
        }

        public void CargarDatosIniciales() {
            gestor.AgregarProductos(
            new Producto
            {
                Id = 3,
                CodigoBarras = "123456",
                Nombre = "Audifonos",
                Categoria = "Audio",
                Precio = 5.99,
                Stock = 10
            }
            );

            gestor.AgregarProductos(
                    new Producto
                    {
                        Id = 1,
                        CodigoBarras = "789456",
                        Nombre = "XBox",
                        Categoria = "Entretenimiento",
                        Precio = 1000,
                        Stock = 10
                    }
                );

            gestor.AgregarProductos(
                    new Producto
                    {
                        Id = 4,
                        CodigoBarras = "456123",
                        Nombre = "Mouse",
                        Categoria = "Accesorios",
                        Precio = 60,
                        Stock = 15
                    }
                );
            gestor.AgregarProductos(
                    new Producto
                    {
                        Id = 2,
                        CodigoBarras = "741258",
                        Nombre = "PlayStation",
                        Categoria = "Entretenimiento",
                        Precio = 1500,
                        Stock = 20
                    }
                );
        }


        public void MostrarResultadoBusqueda(Producto producto, int iteracionesP) {
            txtResultadoBusqueda.Text = producto?.ToString() ?? "No encontrado";
            iteraciones.Value = iteracionesP;
        }

     

        public void DibujarGraficoBarras(List<Producto> productos) {
            canvasGrafico.Children.Clear();
            double maxPrecio = productos.Max(p => p.Precio);
            double escala = canvasGrafico.ActualHeight / maxPrecio;

            for (int i = 0; i < productos.Count - 1; i++) {
                Rectangle barra = new Rectangle
                {
                    Width = 30,
                    Height = productos[i].Precio * escala,
                    Fill = Brushes.CornflowerBlue,
                    Margin = new Thickness(i * 70, canvasGrafico.ActualHeight
                        - (productos[i].Precio * escala), 0, 0)
                };
                canvasGrafico.Children.Add(barra);

                TextBlock etiqueta = new TextBlock
                {
                    Text = productos[i].Nombre,
                    Margin = new Thickness(i * 40, canvasGrafico.ActualHeight - 20, 0, 0)
                };
                canvasGrafico.Children.Add(etiqueta);
            }
        }

       

       
        private void btnBuscar_Click_1(object sender, RoutedEventArgs e) {
            string criterio = comboTipoBusqueda.SelectedItem.ToString();
            string valor = txtBusqueda.Text;

            switch (criterio) {
                case "ID":
                    if (int.TryParse(valor, out int id)) {
                        var (producto, iteraciones) =
                            Buscador.BusquedaBinaria(gestor.ObtenerListaProductos(), id);
                        MostrarResultadoBusqueda(producto, iteraciones);
                    }
                    break;

                case "Nombre":
                    var (productoNombre, iteracionesNombre) =
                            Buscador.BusquedaSecuencial(gestor.ObtenerListaProductos(), valor);
                    MostrarResultadoBusqueda(productoNombre, iteracionesNombre);
                    break;

            }
        }

        private void btnOrdenar_Click_1(object sender, RoutedEventArgs e) {
            List<Producto> productos = new List<Producto>(gestor.ObtenerListaProductos());

            string criterio = comboCriterioOrden.SelectedItem.ToString();

            switch (criterio) {
                case "ID":
                    Ordenador.QuickSortPorId(productos);
                    break;

                case "Nombre":
                    Ordenador.MergeSortPorNombre(productos);
                    break;
                case "Precio":
                    Ordenador.QuickSortPorPrecio(productos);
                    break;
            }

            listViewOrdenados.ItemsSource = productos;
            DibujarGraficoBarras(productos);
        }

        private void btnEliminar_Click_1(object sender, RoutedEventArgs e) {
            if (dataGridProductos.SelectedItem is Producto productoSeleccionado) {
                bool eliminado = gestor.EliminarProducto(productoSeleccionado.CodigoBarras);

                if (eliminado) {
                    dataGridProductos.ItemsSource = null;
                    dataGridProductos.ItemsSource = gestor.ObtenerListaProductos();
                    MessageBox.Show("Producto eliminado", "Exito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            } else {
                MessageBox.Show("Seleccion un producto", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e) {
            var ventanaAgregar = new Window1();
            if (ventanaAgregar.ShowDialog() == true) {
                Producto nuevoProducto = ventanaAgregar.Producto;

                try {
                    gestor.AgregarProductos(nuevoProducto);
                    dataGridProductos.ItemsSource = null;
                    dataGridProductos.ItemsSource = gestor.ObtenerListaProductos();

                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}