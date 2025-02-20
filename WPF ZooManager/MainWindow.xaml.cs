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
using System.Configuration; // This is needed to access the ConfigurationManager class.
using System.Data.SqlClient;
using System.Data; // This is needed to access the SqlConnection class.

namespace WPF_ZooManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;

        public MainWindow()
        {
            InitializeComponent();

            // Setting our connection by catching the connection string from the WPF_ZooManager.Properties.Settings.CSharpDBConnectionString.
            string connectionString = ConfigurationManager.ConnectionStrings["WPF_ZooManager.Properties.Settings.CSharpDBConnectionString"].ConnectionString;

            sqlConnection = new SqlConnection(connectionString); // Passing the connection string to the SqlConnection object.
            ShowZoos();
            ShowAnimals();
        }

        private void ShowZoos()
        {
            try
            {
                string query = "SELECT * FROM Zoo";

                // The SqlDataAdapter can be imagened like an Interface to make the Tables usable by C#-Objects.
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection); // SqlDataAdapter is used to execute the query on our database passed.

                // the using statement is used to ensure that an object is disposed of properly when it is no longer needed
                using (sqlDataAdapter) // the using statement is used to ensure that an object is disposed of properly when it is no longer needed
                {
                    DataTable zooTable = new DataTable(); // Creating a new DataTable object to store the data from the query.
                    sqlDataAdapter.Fill(zooTable); // Filling the DataTable with the data from the query.

                    listZoos.DisplayMemberPath = "Location";
                    listZoos.SelectedValuePath = "Id";
                    // Bind the DataTable's DefaultView to a listZoos in WPF.
                    listZoos.ItemsSource = zooTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowAssociatedAnimals()
        {
            try
            {
                string query = "SELECT * FROM Animal a INNER JOIN ZooAnimal za ON a.Id = za.AnimalFK WHERE za.ZooFK = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection); // Using SqlCommand to use an sql parameter.

                // The SqlDataAdapter can be imagened like an Interface to make the Tables usable by C#-Objects.
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand); // SqlDataAdapter is used to execute the query on our database passed.

                // the using statement is used to ensure that an object is disposed of properly when it is no longer needed
                using (sqlDataAdapter) // the using statement is used to ensure that an object is disposed of properly when it is no longer needed
                {
                    sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue); // Defining our Parameter.

                    DataTable associetedAnimalTable = new DataTable(); // Creating a new DataTable object to store the data from the query.
                    sqlDataAdapter.Fill(associetedAnimalTable); // Filling the DataTable with the data from the query.

                    listAssociatedAnimals.DisplayMemberPath = "Name";
                    listAssociatedAnimals.SelectedValuePath = "Id";
                    // Bind the DataTable's DefaultView to a listZoos in WPF.
                    listAssociatedAnimals.ItemsSource = associetedAnimalTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void listZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedZooInTextBox();
            ShowAssociatedAnimals();
        }

        private void listAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedAnimalInTextBox();
        }

        private void ShowAnimals()
        {
            try
            {
                string query = "SELECT * FROM Animal";

                // The SqlDataAdapter can be imagened like an Interface to make the Tables usable by C#-Objects.
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection); // SqlDataAdapter is used to execute the query on our database passed.

                // the using statement is used to ensure that an object is disposed of properly when it is no longer needed
                using (sqlDataAdapter) // the using statement is used to ensure that an object is disposed of properly when it is no longer needed
                {
                    DataTable animalTable = new DataTable(); // Creating a new DataTable object to store the data from the query.
                    sqlDataAdapter.Fill(animalTable); // Filling the DataTable with the data from the query.

                    listAnimals.DisplayMemberPath = "Name";
                    listAnimals.SelectedValuePath = "Id";
                    // Bind the DataTable's DefaultView to a listZoos in WPF.
                    listAnimals.ItemsSource = animalTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Other way to execute a sql query:
                string query = "DELETE FROM Zoo WHERE id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open(); // Opens the connection.
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar(); // Execute the query.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }
        }

        private void RemoveAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Other way to execute a sql query:
                string query = "DELETE FROM ZooAnimal WHERE AnimalFK = @AnimalAssociatedId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open(); // Opens the connection.
                sqlCommand.Parameters.AddWithValue("@AnimalAssociatedId", listAssociatedAnimals.SelectedValue);
                sqlCommand.ExecuteScalar(); // Execute the query.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();
            }
        }

        private void AddZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Other way to execute a sql query:
                string query = "INSERT INTO Zoo (Location) VALUES (@Location)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open(); // Opens the connection.
                sqlCommand.Parameters.AddWithValue("@Location", AppTextBox.Text);
                sqlCommand.ExecuteScalar(); // Execute the query.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }
        }

        private void AddAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Other way to execute a sql query:
                string query = "INSERT INTO Animal (Name) VALUES (@Name)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open(); // Opens the connection.
                sqlCommand.Parameters.AddWithValue("@Name", AppTextBox.Text);
                sqlCommand.ExecuteScalar(); // Execute the query.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlConnection.Close();
                ShowAnimals();
            }
        }

        private void AddAnimalToZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Other way to execute a sql query:
                string query = "INSERT INTO ZooAnimal (AnimalFK, ZooFK) VALUES (@AnimalId, @ZooId)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open(); // Opens the connection.
                sqlCommand.Parameters.AddWithValue("@AnimalId", listAnimals.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
                sqlCommand.ExecuteScalar(); // Execute the query.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();
            }
        }

        private void UpdateZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Other way to execute a sql query:
                string query = "UPDATE Zoo SET Location = @Location WHERE Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open(); // Opens the connection.
                sqlCommand.Parameters.AddWithValue("@Location", AppTextBox.Text);
                sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue );
                sqlCommand.ExecuteScalar(); // Execute the query.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlConnection.Close();
                ShowZoos();
            }
        }

        private void UpdateAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Other way to execute a sql query:
                string query = "UPDATE Animal SET Name = @Name WHERE Id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open(); // Opens the connection.
                sqlCommand.Parameters.AddWithValue("@Name", AppTextBox.Text);
                sqlCommand.Parameters.AddWithValue("@AnimalId", listAnimals.SelectedValue);
                sqlCommand.ExecuteScalar(); // Execute the query.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlConnection.Close();
                ShowAnimals();
            }
        }

        private void DeleteAnimalToZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Other way to execute a sql query:
                string query = "DELETE FROM Animal WHERE Id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open(); // Opens the connection.
                sqlCommand.Parameters.AddWithValue("@AnimalId", listAnimals.SelectedValue);
                sqlCommand.ExecuteScalar(); // Execute the query.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                sqlConnection.Close();
                ShowAssociatedAnimals();
                ShowAnimals();
            }
        }

        private void ShowSelectedZooInTextBox()
        {
            try
            {
                string query = "SELECT Location FROM Zoo WHERE Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection); // Using SqlCommand to use an sql parameter.

                // The SqlDataAdapter can be imagened like an Interface to make the Tables usable by C#-Objects.
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand); // SqlDataAdapter is used to execute the query on our database passed.

                // the using statement is used to ensure that an object is disposed of properly when it is no longer needed
                using (sqlDataAdapter) // the using statement is used to ensure that an object is disposed of properly when it is no longer needed
                {
                    sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue); // Defining our Parameter.

                    DataTable zooDataTable = new DataTable(); // Creating a new DataTable object to store the data from the query.
                    sqlDataAdapter.Fill(zooDataTable); // Filling the DataTable with the data from the query.

                    AppTextBox.Text = zooDataTable.Rows[0]["Location"].ToString();
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowSelectedAnimalInTextBox()
        {
            try
            {
                string query = "SELECT Name FROM Animal WHERE Id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection); // Using SqlCommand to use an sql parameter.

                // The SqlDataAdapter can be imagened like an Interface to make the Tables usable by C#-Objects.
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand); // SqlDataAdapter is used to execute the query on our database passed.

                // the using statement is used to ensure that an object is disposed of properly when it is no longer needed
                using (sqlDataAdapter) // the using statement is used to ensure that an object is disposed of properly when it is no longer needed
                {
                    sqlCommand.Parameters.AddWithValue("@AnimalId", listAnimals.SelectedValue); // Defining our Parameter.

                    DataTable animalDataTable = new DataTable(); // Creating a new DataTable object to store the data from the query.
                    sqlDataAdapter.Fill(animalDataTable); // Filling the DataTable with the data from the query.

                    AppTextBox.Text = animalDataTable.Rows[0]["Name"].ToString();
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
