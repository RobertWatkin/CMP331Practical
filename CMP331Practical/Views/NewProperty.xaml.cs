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
    /// Interaction logic for NewProperty.xaml
    /// </summary>
    public partial class NewProperty : Window
    {
        // TODO adjust monthly rent selection to allow decimals - property management and this

        IRepository<Property> propertyContext;
        IRepository<User> userContext;
        IRepository<Role> roleContext;

        User loggedInUser;
        string[] maintainanceList = { "Electrical", "Heating", "Gas", "Plumbing", "None" };


        public NewProperty(User loggedInUser)
        {
            this.loggedInUser = loggedInUser;
            InitializeComponent();

            this.propertyContext = ContainerHelper.Container.Resolve<IRepository<Property>>();
            this.userContext = ContainerHelper.Container.Resolve<IRepository<User>>();
            this.roleContext = ContainerHelper.Container.Resolve<IRepository<Role>>();

            ComboBoxSetup();
        }

        private void ComboBoxSetup()
        {
            List<User> userList = userContext.Collection().ToList();
            List<Role> roleList = roleContext.Collection().ToList();
            List<string> maintainanceRoles = new List<string>();
            string agentRole = "";

            // find out which role id's represents each role
            foreach (Role r in roleList)
            {
                string[] split = r.Name.Split(' ');
                Console.WriteLine(split[0]);
                if (split[0].Equals("Maintainance"))
                {
                    maintainanceRoles.Add(r.Id);
                }
                else if (r.Name.Equals("Letting Agent"))
                {
                    agentRole = r.Id;
                }
            }

            // sort users into maintainance staff and letting agents
            List<User> maintainanceStaff = new List<User>();
            List<User> lettingAgents = new List<User>();

            foreach (User user in userList)
            {
                if (maintainanceRoles.Contains(user.RoleId))
                {
                    maintainanceStaff.Add(user);
                }
                else if (user.RoleId.Equals(agentRole))
                {
                    lettingAgents.Add(user);
                }
            }

            // adds null option to cmbbox incase not required
            User nullUser = new User(null, null, null, null, null);
            maintainanceStaff.Add(nullUser);
            lettingAgents.Add(nullUser);

            // set combo box values for maintainance selection
            cmbMaintainanceStaff.ItemsSource = maintainanceStaff;
            cmbMaintainanceStaff.SelectedValuePath = "Id";

            // set combo box values for letting agent selection
            cmbLettingAgent.ItemsSource = lettingAgents;
            cmbLettingAgent.SelectedValuePath = "Id";

            cmbRequiredMaintainance.ItemsSource = maintainanceList;

        }

        public async void SaveRecord(object sender, RoutedEventArgs e)
        {
            // TODO save record
            if (txtAddressLine1.Text.Equals("") || txtPostCode.Text.Equals("") || cmbLettingAgent.SelectedItem == null || cmbMaintainanceStaff.SelectedItem == null || cmbRequiredMaintainance == null)
            {
                MessageBox.Show("Please enter Address Line 1, Post Code and Select Letting Agent, Maintainance Staff and Required Maintainance");
            }
            else
            {
                Property property = new Property((bool) chkAvailable.IsChecked, txtAddressLine1.Text, txtAddressLine2.Text, txtPostCode.Text, float.Parse(monthlyRent.Text), cmbRequiredMaintainance.SelectedValue.ToString(), (DateTime) dtpQuarterly.Value, (DateTime) dtpAnnualGasInspection.Value, (DateTime) dtpFiveYearElectricalInspection.Value, cmbMaintainanceStaff.SelectedValue.ToString(), cmbLettingAgent.SelectedValue.ToString());
                propertyContext.Insert(property);
                await propertyContext.Commit();
                MessageBox.Show("Record Created!", "Creation Successful!");
                PropertyManagement pm = new PropertyManagement(loggedInUser);
                this.Hide();
                pm.Show();
            }
        }

        private void Dashboard(object sender, RoutedEventArgs e)
        {
            // return to dashboard
            Dashboard d = new Dashboard(loggedInUser);
            d.Show();
            this.Close();

        }
    }
}
