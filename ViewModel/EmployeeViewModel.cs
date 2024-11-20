using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Module05Exercise01.Model;
using Module05Exercise01.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace Module05Exercise01.ViewModel
{
    public class EmployeeViewModel : INotifyPropertyChanged
    {
        private readonly EmployeeService _employeeService;
        public ObservableCollection<Employee> EmployeeList { get; set; }
        public ObservableCollection<Employee> FilteredEmployeeList { get; set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterEmployee();
            }
        }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                if (_selectedEmployee != null)
                {
                    NewEmployeeName = _selectedEmployee.Name;
                    NewEmployeeAddress = _selectedEmployee.Address;
                    NewEmployeeEmail = _selectedEmployee.email;
                    NewEmployeeContactNo = _selectedEmployee.ContactNo;
                    IsEmployeeSelected = true;
                    IsEmployeeSelectedAdd = false;

                }
                else
                {
                    IsEmployeeSelected = false;
                    IsEmployeeSelectedAdd = true;
                }

                OnPropertyChanged();
            }
        }

        private bool _isEmployeeSelected;
        public bool IsEmployeeSelected
        {
            get => _isEmployeeSelected;
            set
            {
                _isEmployeeSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _isEmployeeSelectedAdd;
        public bool IsEmployeeSelectedAdd
        {
            get => _isEmployeeSelectedAdd;
            set
            {
                _isEmployeeSelectedAdd = value;
                OnPropertyChanged();
            }
        }

        // New Employee entry for name, address, email, contact no
        private string _newEmployeeName;
        public string NewEmployeeName
        {
            get => _newEmployeeName;
            set
            {
                _newEmployeeName = value;
                OnPropertyChanged();
            }
        }

        private string _newEmployeeAddress;
        public string NewEmployeeAddress
        {
            get => _newEmployeeAddress;
            set
            {
                _newEmployeeAddress = value;
                OnPropertyChanged();
            }
        }


        private string _newEmployeeEmail;
        public string NewEmployeeEmail
        {
            get => _newEmployeeEmail;
            set
            {
                _newEmployeeEmail = value;
                OnPropertyChanged();
            }
        }

        private string _newEmployeeContactNo;
        public string NewEmployeeContactNo
        {
            get => _newEmployeeContactNo;
            set
            {
                _newEmployeeContactNo = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadDataCommand { get; }
        public ICommand AddEmployeeCommand { get; }
        public ICommand SelectedEmployeeCommand { get; }
        public ICommand UpdateEmployeeCommand { get; }
        public ICommand DeleteEmployeeCommand { get; }

        //EmployeeViewModel Constructor

        public EmployeeViewModel()
        {
            _employeeService = new EmployeeService();
            EmployeeList = new ObservableCollection<Employee>();

            // For filtering
            FilteredEmployeeList = new ObservableCollection<Employee>();

            // Initialize the command
            LoadDataCommand = new Command(async () => await LoadData());

            AddEmployeeCommand = new Command(async () => await InsertEmployee());
            UpdateEmployeeCommand = new Command(async () => await UpdateEmployee());
            SelectedEmployeeCommand = new Command<Employee>(employee => SelectedEmployee = employee);
            DeleteEmployeeCommand = new Command(async () => await DeleteEmployee(), () => SelectedEmployee != null);

            // Optionally load data on initialization
            LoadData(); // uncomment if you want data to load automatically
        }

        public async Task LoadData()
        {
            if (IsBusy) return;
            IsBusy = true;
            StatusMessage = "Loading employee data...";
            try
            {
                var employees = await _employeeService.GetAllEmployeesAsync();
                EmployeeList.Clear();
                foreach (var employee in employees)
                {
                    EmployeeList.Add(employee);
                }
                StatusMessage = "Data loaded successfully!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load data: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task InsertEmployee()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(NewEmployeeName) || string.IsNullOrWhiteSpace(NewEmployeeAddress) || string.IsNullOrWhiteSpace(NewEmployeeEmail) || string.IsNullOrWhiteSpace(NewEmployeeContactNo))
            {
                StatusMessage = "Please fill in all fields before adding";
                return;
            }
            IsBusy = true;
            StatusMessage = "Adding new employee...";

            try
            {
                var newEmployee = new Employee
                {
                    Name = NewEmployeeName,
                    Address = NewEmployeeAddress,
                    email = NewEmployeeEmail,
                    ContactNo = NewEmployeeContactNo
                };
                var isSuccess = await _employeeService.InsertEmployeeAsync(newEmployee);
                if (isSuccess)
                {
                    NewEmployeeName = string.Empty;
                    NewEmployeeAddress = string.Empty;
                    NewEmployeeEmail = string.Empty;
                    NewEmployeeContactNo = string.Empty;

                    StatusMessage = "New employee added successfully!";
                }
                else
                {
                    StatusMessage = "Failed to add new employee";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed adding employee: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
                await LoadData();
            }
        }
        //private async Task UpdateEmployee()
        //{
        //    if (IsBusy || SelectedEmployee == null)
        //    {
        //        StatusMessage = "Select an employee to update.";
        //        return;
        //    }

        //    IsBusy = true;
        //    StatusMessage = "Updating employee...";

        //    try
        //    {
        //        SelectedEmployee.Name = NewEmployeeName;
        //        SelectedEmployee.Address = NewEmployeeAddress;
        //        SelectedEmployee.email = NewEmployeeEmail;
        //        SelectedEmployee.ContactNo = NewEmployeeContactNo;

        //        var success = await _employeeService.UpdateEmployeeAsync(SelectedEmployee);
        //        StatusMessage = success ? "Employee updated successfully!" : "Failed to update employee";

        //        await LoadData();
        //    }
        //    catch (Exception ex)
        //    {
        //        StatusMessage = $"Error: {ex.Message}";
        //    }
        //    finally
        //    {
        //        IsBusy = false;
        //    }
        //}

        private async Task UpdateEmployee()
        {
            if (SelectedEmployee == null) return;
            var answer = await Application.Current.MainPage.DisplayAlert("Confirm Update", $"Are you sure you want to update?", "Yes", "No");


            if (IsBusy || string.IsNullOrWhiteSpace(NewEmployeeName) || string.IsNullOrWhiteSpace(NewEmployeeAddress) || string.IsNullOrWhiteSpace(NewEmployeeEmail) || string.IsNullOrWhiteSpace(NewEmployeeContactNo))
            {
                StatusMessage = "Please fill in all fields before updating";
                return;
            }

            IsBusy = true;
            StatusMessage = "Updating employee...";
            try
            {
                SelectedEmployee.Name = NewEmployeeName;
                SelectedEmployee.Address = NewEmployeeAddress;
                SelectedEmployee.email = NewEmployeeEmail;
                SelectedEmployee.ContactNo = NewEmployeeContactNo;

                var success = await _employeeService.UpdateEmployeeAsync(SelectedEmployee);
                StatusMessage = success ? "Employee updated successfully" : "Failed to update employee";
                
                await LoadData();

            }
            catch (Exception ex)
            {
                StatusMessage = $"Error updating employee: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeleteEmployee()
        {
            if (SelectedEmployee == null) return;
            var answer = await Application.Current.MainPage.DisplayAlert("Confirm Delete", $"Are you sure you want to delete {SelectedEmployee.Name}?", "Yes", "No");

            if (!answer) return;

            IsBusy = true;
            StatusMessage = "Deleting employee..";

            try
            {
                var success = await _employeeService.DeleteEmployeeAsync(SelectedEmployee.EmployeeID);
                StatusMessage = success ? "Employee deleted successfully!" : "Failed to delete employee";

                if (success)
                {
                    EmployeeList.Remove(SelectedEmployee);
                    SelectedEmployee = null;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting employee: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
                await LoadData();
            }
        }
        private void FilterEmployee()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredEmployeeList.Clear();
                foreach (var employee in EmployeeList)
                {
                    FilteredEmployeeList.Add(employee);
                }
            }
            else
            {
                var filtered = EmployeeList.Where(e => e.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                                                    || e.Address.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                                                    || e.email.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                                                    || e.ContactNo.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                FilteredEmployeeList.Clear();
                foreach (var employee in filtered)
                {
                    FilteredEmployeeList.Add(employee);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}