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
using CMP331Practical.Contracts;
using CMP331Practical.Views;
using CMP331Practical.Models;
using Unity;

namespace CMP331Practical.Views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Dashboard : Window
    {

        IRepository<Role> roleContext;
        IRepository<Property> propertyContext;
        private User loggedInUser;
        Role currentRole = null;

        public List<Property> assignedProperties;

        public Dashboard(User loggedInUser)
        {

            this.roleContext = ContainerHelper.Container.Resolve<IRepository<Role>>();
            this.propertyContext = ContainerHelper.Container.Resolve<IRepository<Property>>();

            InitializeComponent();
            LoadNotifications();
            LoadAssignedProperties();
            this.loggedInUser = loggedInUser;
            

            // set name and role on dashboard
            txtUserName.Content = this.loggedInUser.Firstname + " " + this.loggedInUser.Lastname;

            // get the users current role name
            List<Role> roleList = roleContext.Collection().ToList();
            foreach (Role r in roleList)
            {
                if (r.Id == loggedInUser.RoleId)
                {
                    currentRole = r;
                }
            }
            txtUserRole.Content = currentRole.Name;
        }

        private void LoadAssignedProperties()
        {
            assignedProperties = new List<Property>();
            List<Property> allProperties = propertyContext.Collection().ToList();

            if (currentRole.Name == "System Admin") // all properties displayed for system admin
            {
                assignedProperties = allProperties;
            }
            else if (currentRole.Name == "Letting Agent") // only properties assigned to letting agents displayed to the letting agent
            {
                foreach (Property p in allProperties)
                {
                    if (p.LettingAgentId == loggedInUser.Id)
                    {
                        assignedProperties.Add(p);
                    }
                }
            }
            else // only properties assigned to maintainance staff displayed to maintainance staff
            {
                foreach (Property p in allProperties)
                {
                    if (p.MaintainanceStaffId == loggedInUser.Id)
                    {
                        assignedProperties.Add(p);
                    }
                }
            }


            assignedProperties = allProperties;
            AssignedProperties.ItemsSource = assignedProperties;



        }

        private void SignOut(object sender, RoutedEventArgs e)
        {
            MainWindow l = new MainWindow();
            l.Show();
            this.Close();
        }

        private void LoadNotifications()
        {
            // TODO outstanding payments - X maintainance staff
            // TODO outstanding inspections
        }

        private void PropertyManagement(object sender, RoutedEventArgs e)
        {
            // open property managmeent window
            PropertyManagement pm = new PropertyManagement(loggedInUser);
            pm.Show();
            this.Close();
        }

        private void AvailableProperties(object sender, RoutedEventArgs e)
        {
            // TODO available properties
        }

        private void UserManagement(object sender, RoutedEventArgs e)
        {
            // open user management window
            UserManagement um = new UserManagement(loggedInUser);
            um.Show();
            this.Close();
        }
    }
}
