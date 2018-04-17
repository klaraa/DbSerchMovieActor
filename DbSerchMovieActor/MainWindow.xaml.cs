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
            if (searchMovie.Text != null && searchMovie.Text != "")
            {
                using (var connection = new NpgsqlConnection("Host=localhost;Username=student;Password=student;Database=vorlesung"))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT title FROM movies WHERE title ILIKE @v OR title ~* @p OR metaphone(title,6) = metaphone(@p, 6) ; "; //ORDER BY levenshtein(lower(@p), lower(title))
                        cmd.Parameters.AddWithValue("p", searchMovie.Text);
                        string movieLike = searchMovie.Text + "%";
                        cmd.Parameters.AddWithValue("v", movieLike);
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
            if(searchActor.Text != null && searchActor.Text != "")
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


        //search movie direct
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<string> titles = new List<string>();
            if (searchMovie.Text != null && searchMovie.Text != "")
            {
                using (var connection = new NpgsqlConnection("Host=localhost;Username=student;Password=student;Database=vorlesung"))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT title FROM movies; ";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                titles.Add(reader.GetString(0));
                            }
                        }
                    }
                }
                titles = titles.FindAll(t => levenshtein(t, searchMovie.Text) < 6);
                result.Text = titles.Aggregate((result, t) => result += "\n" + t);
            }
        }



        //search actor
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (searchActor.Text != null && searchActor.Text != "")
            {
                using (var connection = new NpgsqlConnection("Host=localhost;Username=student;Password=student;Database=vorlesung"))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT name FROM actors WHERE name ILIKE @v OR name ~* @p OR metaphone(name,8) % metaphone(@p, 8) ORDER BY levenshtein(lower(@p), lower(name)); ";
                        cmd.Parameters.AddWithValue("p", searchActor.Text);
                        string actorLike = searchActor.Text + "%";
                        cmd.Parameters.AddWithValue("v", actorLike);
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
            if (searchMovie.Text != null && searchMovie.Text != "")
            {
                using (var connection = new NpgsqlConnection("Host=localhost;Username=student;Password=student;Database=vorlesung"))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT name FROM actors NATURAL JOIN movies_actors NATURAL JOIN movies WHERE metaphone(title,6) = metaphone(@p,6);";
                        cmd.Parameters.AddWithValue("p", searchMovie.Text);
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
            List<string> names = new List<string>();
            if (searchActor.Text != null && searchActor.Text != "")
            {
                using (var connection = new NpgsqlConnection("Host=localhost;Username=student;Password=student;Database=vorlesung"))
                {
                    connection.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandText = "SELECT name FROM actors; ";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                names.Add(reader.GetString(0));
                            }
                        }
                    }
                }
                names = names.FindAll(n => levenshtein(n, searchActor.Text) < 6);
                result.Text = names.Aggregate((result, n) => result += "\n" + n);
            }
        }


        private Int32 levenshtein(String a, String b)
        {

            if (string.IsNullOrEmpty(a))
            {
                if (!string.IsNullOrEmpty(b))
                {
                    return b.Length;
                }
                return 0;
            }

            if (string.IsNullOrEmpty(b))
            {
                if (!string.IsNullOrEmpty(a))
                {
                    return a.Length;
                }
                return 0;
            }

            Int32 cost;
            Int32[,] d = new int[a.Length + 1, b.Length + 1];
            Int32 min1;
            Int32 min2;
            Int32 min3;

            for (Int32 i = 0; i <= d.GetUpperBound(0); i += 1)
            {
                d[i, 0] = i;
            }

            for (Int32 i = 0; i <= d.GetUpperBound(1); i += 1)
            {
                d[0, i] = i;
            }

            for (Int32 i = 1; i <= d.GetUpperBound(0); i += 1)
            {
                for (Int32 j = 1; j <= d.GetUpperBound(1); j += 1)
                {
                    cost = Convert.ToInt32(!(a[i - 1] == b[j - 1]));

                    min1 = d[i - 1, j] + 1;
                    min2 = d[i, j - 1] + 1;
                    min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }

            return d[d.GetUpperBound(0), d.GetUpperBound(1)];

        }
    }
}
