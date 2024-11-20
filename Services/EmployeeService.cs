﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Module05Exercise01.Services;
using Module05Exercise01.Model;
using MySql.Data.MySqlClient;

namespace Module05Exercise01.Services
{
    public class EmployeeService
    {
        private readonly string _connectionString;
        public EmployeeService()
        {
            var dbService = new DatabaseConnectionService();
            _connectionString = dbService.GetConnectionString();
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            var employeeService = new List<Employee>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Retrieve data
                var cmd = new MySqlCommand("SELECT * FROM tblemployee", conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employeeService.Add(new Employee
                        {
                            EmployeeID = reader.GetInt32("EmployeeID"),
                            Name = reader.GetString("Name"),
                            email = reader.GetString("email"),
                            Address = reader.GetString("Address"),
                            ContactNo = reader.GetString("ContactNo")
                        });
                    }
                }
            }
            return employeeService;
        }

        public async Task<bool> InsertEmployeeAsync(Employee newEmployee)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("INSERT INTO tblemployee (Name, Address, email, ContactNo) VALUES (@Name, @Address, @email, @ContactNo)", conn);
                    cmd.Parameters.AddWithValue("@Name", newEmployee.Name);
                    cmd.Parameters.AddWithValue("@Address", newEmployee.Address);
                    cmd.Parameters.AddWithValue("@email", newEmployee.email);
                    cmd.Parameters.AddWithValue("@ContactNo", newEmployee.ContactNo);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding employee record: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("DELETE FROM tblemployee WHERE EmployeeID = @EmployeeID", conn);
                    cmd.Parameters.AddWithValue("@EmployeeID", id);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting employee record: {ex.Message}");
                return false;
            }
        }

        public async Task <bool> UpdateEmployeeAsync(Employee updatedEmployee)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand(
                        "UPDATE tblemployee SET Name = @Name, Address = @Address, email = @email, ContactNo = @ContactNo WHERE EmployeeID = @EmployeeID", conn);
                    cmd.Parameters.AddWithValue("@Name", updatedEmployee.Name);
                    cmd.Parameters.AddWithValue("@Address", updatedEmployee.Address);
                    cmd.Parameters.AddWithValue("@email", updatedEmployee.email);
                    cmd.Parameters.AddWithValue("@ContactNo", updatedEmployee.ContactNo);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0; // Returns true if the update was successful
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error updating employee record: {ex.Message}");
                return false;
            }
        }
    }
}
