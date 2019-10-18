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
using System.Collections.ObjectModel;

namespace KR
{
    /// <summary>
    /// Логика взаимодействия для PageEmployee.xaml
    /// </summary>
    public partial class PageEmployee : Page
    {
        private bool isDirty = true;

        public static TitleEmployeeEntities DataEntitiesEmployee { get; set; }

        ObservableCollection<Employee> ListEmployee = new ObservableCollection<Employee>();
        public PageEmployee()
        {
            DataEntitiesEmployee = new TitleEmployeeEntities();
            InitializeComponent();
        }

        private void GetEmployees()
        {
            var employees = DataEntitiesEmployee.Employee;
            var queryEmployee = from employee in employees
                                orderby employee.Surname
                                select employee;
            foreach (Employee emp in queryEmployee)
            {
                ListEmployee.Add(emp);
            }
            DataGridEmployee.ItemsSource = ListEmployee;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)

        {

            GetEmployees();
            DataGridEmployee.SelectedIndex = 0;

        }
        private void RewriteEmployee()
        {
            DataEntitiesEmployee = new TitleEmployeeEntities();
            ListEmployee.Clear();
            GetEmployees();
        }

        private void UndoCommandBinding_Executed(object sender,ExecutedRoutedEventArgs e)
        {
            RewriteEmployee();
            DataGridEmployee.IsReadOnly = true;
            isDirty = true;
        }


        //private void EditCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = isDirty;
        //}

        //private void SaveCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = !isDirty;
        //}
        private void EditCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DataGridEmployee.IsReadOnly = false;
            DataGridEmployee.BeginEdit();
            isDirty = false;
        }
        private void SaveCommandBinding_Executed(object sender,ExecutedRoutedEventArgs e)
        {
            DataEntitiesEmployee.SaveChanges();
            isDirty = true;
            DataGridEmployee.IsReadOnly = true;
        }
        private void NewCommandBinding_Executed(object sender,ExecutedRoutedEventArgs e)
        {
            Employee employee = new Employee()
            {
                ID = -1,
                Surname = "не задано",
                Name = "не задано",
                Patronymic = "не задано",
                Telephone = "не задано",
                Email = "не задано",
                TitleID = 0
            };
            try
            {
                DataEntitiesEmployee.Employee.Add(employee);
                ListEmployee.Add(employee);
                DataGridEmployee.ScrollIntoView(employee);
                DataGridEmployee.SelectedIndex = DataGridEmployee.Items.Count - 1;
                DataGridEmployee.Focus();
                DataGridEmployee.IsReadOnly = false;
                isDirty = false;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    "Ошибка добавления нового сотрудника в контекст данных" +
                    ex.ToString());
            }
        }

        private void DeleteCommandBinding_Executed(object sender,
                                    ExecutedRoutedEventArgs e)
        {
            Employee emp = DataGridEmployee.SelectedItem as Employee;
            if (emp != null)
            {
                MessageBoxResult result =
                    MessageBox.Show("Удалить сотрудника: " + emp.Surname +
                        " " + emp.Name + " " + emp.Patronymic, "Предупреждение",
                        MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    DataEntitiesEmployee.Employee.Remove(emp);
                    DataGridEmployee.SelectedIndex =
                        DataGridEmployee.SelectedIndex == 0 ? 1 :
                        DataGridEmployee.SelectedIndex - 1;
                    ListEmployee.Remove(emp);
                    DataEntitiesEmployee.SaveChanges();
                }
            }
            else
                MessageBox.Show("Выберите строку для удаления!");
        }

        private void FindCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            BorderFind.Visibility = System.Windows.Visibility.Visible;
        }
        private void TextBoxSurname_TextChanged(object sender, TextChangedEventArgs e)
        {
            ButtonFindSurname.IsEnabled = true;
            ButtonFindTitle.IsEnabled = false;
            ComboBoxTitle.SelectedIndex = -1;
        }
        private void ButtonFindSurname_Click(object sender, RoutedEventArgs e)
        {
            string surname = TextBoxSurname.Text;
            DataEntitiesEmployee = new TitleEmployeeEntities();
            ListEmployee.Clear();
            var employees = DataEntitiesEmployee.Employee;
            var queryEmployee = from employee in employees
                                where employee.Surname == surname
                                select employee;
            foreach (Employee emp in queryEmployee)
            {
                ListEmployee.Add(emp);
            }
            if (ListEmployee.Count > 0)
            {
                DataGridEmployee.ItemsSource = ListEmployee;
                ButtonFindSurname.IsEnabled = true;
                ButtonFindTitle.IsEnabled = false;
            }
            else
                MessageBox.Show("Сотрудник с фамилией \n" + surname + "\n не найден", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private void ComboBoxTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonFindTitle.IsEnabled = true;
            ButtonFindSurname.IsEnabled = false;
            TextBoxSurname.Text = "";
        }
        private void ButtonFindTitle_Click(object sender, RoutedEventArgs e)
        {
            DataEntitiesEmployee = new TitleEmployeeEntities();
            ListEmployee.Clear();

            Title title = ComboBoxTitle.SelectedItem as Title;
            var employees = DataEntitiesEmployee.Employee;
            var queryEmployee = from employee in employees
                                where employee.TitleID == title.ID
                                orderby employee.Surname
                                select employee;
            foreach (Employee emp in queryEmployee)
            {
                ListEmployee.Add(emp);
            }
            DataGridEmployee.ItemsSource = ListEmployee;
        }
        private void RefreshCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RewriteEmployee();
            DataGridEmployee.IsReadOnly = false;
        }

    }
}
