﻿using System;
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
using System.Windows.Shapes;
using CMP331Practical.Models;
using CMP331Practical.Contracts;
using Unity;

namespace CMP331Practical.Views
{
    /// <summary>
    /// Interaction logic for NewUser.xaml
    /// </summary>
    public partial class NewUser : Window
    {

        IRepository<User> userContext;
        IRepository<Role> roleContext;
        private User loggedInUser;


        public NewUser(User loggedInUser)
        {
            this.loggedInUser = loggedInUser;
            this.userContext = ContainerHelper.Container.Resolve<IRepository<User>>();
            this.roleContext = ContainerHelper.Container.Resolve<IRepository<Role>>();

            // initialise WPF components
            InitializeComponent();

            List<Role> roleList = roleContext.Collection().ToList();

            cmbRole.ItemsSource = roleList;
            cmbRole.DisplayMemberPath = "Name";
            cmbRole.SelectedValuePath = "Id";
            cmbRole.SelectedItem = null;

        }

        private async void SaveRecord(object sender, RoutedEventArgs e)
        {
            // TODO save record
            if (txtFirstName.Text.Equals("") || txtLastName.Text.Equals("") || txtEmail.Text.Equals("") || txtPassword.Password.Equals("") || cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Please enter First Name, Last Name, Email, Password and Select Role");
            }
            else
            {
                User user = new User(txtFirstName.Text, txtLastName.Text, txtEmail.Text, txtPassword.Password, cmbRole.SelectedValue.ToString());
                userContext.Insert(user);
                await userContext.Commit();
                MessageBox.Show("Record Created!", "Creation Successful!");
                UserManagement um = new UserManagement(loggedInUser);
                this.Hide();
                um.Show();
            }
        }


        private void Dashboard(object sender, RoutedEventArgs e)
        {
            Dashboard d = new Dashboard(loggedInUser);
            d.Show();
            this.Close();
        }
    }
}
