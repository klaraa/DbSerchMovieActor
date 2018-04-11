using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DbSerchMovieActor
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //search movie
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (searchMovie != null)
            {

            }
            else
            {
                return;
            }
        }


        //search movie direct
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }



        //search actor
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (searchActor.Text != null)
            {
                using (var connection = new NpgsqlConnection("Host=localhost;Username=student;Password=student;Database=vorlesung"))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT title FROM movies NATURAL JOIN movies_actors NATURAL JOIN actors WHERE metaphone(name,6) = metaphone(@p,6);";
                        cmd.Parameters.AddWithValue("p", searchActor.Text);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Text += reader.GetString(0) + "\n";
                            }
                        }
                    }
                }
            }
        }



        //search actor direct
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }
    }
}
