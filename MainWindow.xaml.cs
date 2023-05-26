﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

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

			string connectionString = ConfigurationManager.ConnectionStrings["ZooApp.Properties.Settings.PanjutorialsDBConnectionString"].ConnectionString;
			sqlConnection = new SqlConnection(connectionString);
			ShowZoos();
			ShowAllAnimals();
		}


		private void ShowAllAnimals()
		{
			try
			{
				string query = "select * from Animal";
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

				using (sqlDataAdapter)
				{
					DataTable animalTable = new DataTable();
					sqlDataAdapter.Fill(animalTable);

					listAllAnimals.DisplayMemberPath = "Name";
					listAllAnimals.SelectedValuePath = "Id";
					listAllAnimals.ItemsSource = animalTable.DefaultView;

				}
			}
			catch (Exception e)
			{

				MessageBox.Show(e.ToString());
			}

		}

		private void ShowZoos()
		{
			try
			{
				string query = "select * from Zoo";
				// the SqlDataAdapter can be imagined like an Interface to make Tables usable by C#-Objects
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

				using (sqlDataAdapter)
				{
					DataTable zooTable = new DataTable();

					sqlDataAdapter.Fill(zooTable);

					//Which Information of the Table in DataTable should be shown in our ListBox?
					listZoos.DisplayMemberPath = "Location";
					//Which Value should be delivered, when an Item from our ListBox is selected?
					listZoos.SelectedValuePath = "Id";
					//The Reference to the Data the ListBox should populate
					listZoos.ItemsSource = zooTable.DefaultView;
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}

		}

		private void ShowAssociatedAnimals()
		{
			try
			{
				string query = "select * from Animal a inner join ZooAnimal " +
					"za on a.Id = za.AnimalId where za.ZooId = @ZooId";

				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				// the SqlDataAdapter can be imagined like an Interface to make Tables usable by C#-Objects
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

				using (sqlDataAdapter)
				{

					sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);

					DataTable animalTable = new DataTable();

					sqlDataAdapter.Fill(animalTable);

					//Which Information of the Table in DataTable should be shown in our ListBox?
					listAssociatedAnimals.DisplayMemberPath = "Name";
					//Which Value should be delivered, when an Item from our ListBox is selected?
					listAssociatedAnimals.SelectedValuePath = "Id";
					//The Reference to the Data the ListBox should populate
					listAssociatedAnimals.ItemsSource = animalTable.DefaultView;
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}

		}

		private void listZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ShowAssociatedAnimals();
			ShowSelectedZooInTextBox();
		}

		private void DeleteZoo_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string query = "delete from Zoo where id = @ZooId";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				sqlConnection.Open();
				sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
				sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());

			}
			finally
			{
				sqlConnection.Close();
				ShowZoos();
			}

		}

		private void AddZoo_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string query = "insert into Zoo values (@Location)";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				sqlConnection.Open();
				sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);
				sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());

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
				string query = "insert into Animal values (@Name)";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				sqlConnection.Open();
				sqlCommand.Parameters.AddWithValue("@Name", myTextBox.Text);
				sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());

			}
			finally
			{
				sqlConnection.Close();
				ShowAllAnimals();
			}
		}

		private void addAnimalToZoo_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string query = "insert into ZooAnimal values (@ZooId, @AnimalId)";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				sqlConnection.Open();
				sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
				sqlCommand.Parameters.AddWithValue("@AnimalId", listAllAnimals.SelectedValue);
				sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());

			}
			finally
			{
				sqlConnection.Close();
				ShowAssociatedAnimals();
			}

		}

		private void DeleteAnimal_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string query = "delete from Animal where id = @AnimalId";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				sqlConnection.Open();
				sqlCommand.Parameters.AddWithValue("@AnimalId", listAllAnimals.SelectedValue);
				sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());

			}
			finally
			{
				sqlConnection.Close();
				ShowAllAnimals();
			}
		}

		private void ShowSelectedZooInTextBox()
		{
			try
			{
				string query = "select location from zoo where Id = @ZooId";

				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

				// the SqlDataAdapter can be imagined like an Interface to make Tables usable by C#-Objects
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

				using (sqlDataAdapter)
				{

					sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);

					DataTable zooDataTable = new DataTable();

					sqlDataAdapter.Fill(zooDataTable);

					myTextBox.Text = zooDataTable.Rows[0]["Location"].ToString();
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		private void ShowSelectedAnimalInTextBox()
		{
			try
			{
				string query = "select Name from Animal where Id = @AnimalId";

				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

				// the SqlDataAdapter can be imagined like an Interface to make Tables usable by C#-Objects
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

				using (sqlDataAdapter)
				{

					sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);

					DataTable animalDataTable = new DataTable();

					sqlDataAdapter.Fill(animalDataTable);

					myTextBox.Text = animalDataTable.Rows[0]["Name"].ToString();
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		private void listAllAnimals_SelectinChanged(object sender, EventArgs e)
		{
			ShowSelectedAnimalInTextBox();
		}

		private void UpdateZoo_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string query = "UPDATE Zoo SET  Location = @Location WHERE Id = @ZooId ";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				sqlConnection.Open();
				sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
				sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);
				sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());

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
				string query = "UPDATE Animal SET  Name = @Name WHERE Id = @AnimalId ";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				sqlConnection.Open();
				sqlCommand.Parameters.AddWithValue("@AnimalId", listAllAnimals.SelectedValue);
				sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);
				sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());

			}
			finally
			{
				sqlConnection.Close();
				ShowAllAnimals();
			}

		}

	}
}
