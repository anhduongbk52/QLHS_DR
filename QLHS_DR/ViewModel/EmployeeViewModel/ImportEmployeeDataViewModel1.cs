

using Microsoft.Win32;
using OfficeOpenXml;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.EmployeeViewModel
{
    
    internal class ImportEmployeeDataViewModel1 : BaseViewModel
    {
        #region "Properties and Fields"               
        ServiceFactory _ServiceFactory = new ServiceFactory();
        private DataView _ExcelData;
        public DataView ExcelData
        {
            get => _ExcelData;
            set
            {
                if (_ExcelData != value)
                {
                    _ExcelData = value;
                    NotifyPropertyChanged("ExcelData");
                }
            }
        }
        private int _ProgressBarValue;
        public int ProgressBarValue
        {
            get => _ProgressBarValue;
            set
            {
                if (_ProgressBarValue != value)
                {
                    _ProgressBarValue = value;
                    NotifyPropertyChanged("ProgressBarValue");
                }
            }
        }

        #endregion
        #region "Command"

        public ICommand OpenExcelCommand { get; set; }
        public ICommand RunCommand { get; set; }
        #endregion
        internal ImportEmployeeDataViewModel1()
        {
            ProgressBarValue = 0;
            OpenExcelCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    Multiselect = false
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;
                    LoadDataFromExcel(filePath);
                }
            });
            RunCommand = new RelayCommand<object>((p) => { if (_ExcelData!=null) return true; else return false; }, (p) =>
            {
                string[] formats = { "M/dd/yyyy", "MM/dd/yyyy", "dd/MM/yyyy", "d/MM/yyyy", "d/M/yyyy", "dd/M/yyyy", "yyyy-MM-dd" }; // Thêm các định dạng ngày tháng khác nếu cần
                try
                {
                    ProgressBarValue = 0;
                    int i = 0;
                    byte[] avatar = null; 
                    foreach (DataRowView rowView in _ExcelData)
                    {
                        
                        DataRow row = rowView.Row;
                        DateTime dateOfBirth = new();
                        Employee newEmployee = new Employee();                        

                        newEmployee.MSNV = row.Table.Columns.Contains("MSNV") ? row["MSNV"].ToString():null;
                        newEmployee.FirtName = row.Table.Columns.Contains("FirtName") ? row["FirtName"].ToString():null;
                        newEmployee.LastName = row.Table.Columns.Contains("LastName") ? row["LastName"].ToString() : null;
                        if (row.Table.Columns.Contains("MSNV"))
                        {
                            var dateOfBirthString = row["DateOfBirth"].ToString();
                            DateTime.TryParseExact(dateOfBirthString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth);
                            newEmployee.DateOfBirth = dateOfBirth;
                        }                        
                        newEmployee.Address = row.Table.Columns.Contains("Address") ? row["Address"].ToString() : null;
                        newEmployee.Gender = row.Table.Columns.Contains("Gender") ? row["Gender"].ToString() : null;
                        newEmployee.Email = row.Table.Columns.Contains("Email") ? row["Email"].ToString() : null;
                        newEmployee.PhoneNumber = row.Table.Columns.Contains("PhoneNumber") ? row["PhoneNumber"].ToString() : null;
                        newEmployee.HireDate = row.Table.Columns.Contains("HireDate") ? DateTime.Parse(row["HireDate"].ToString()) : DateTime.Now;
                        newEmployee.IsActive = row.Table.Columns.Contains("IsActive") ? row["IsActive"].ToString() == "1" : (bool?)null;                      
 
                        newEmployee.IsPartyMember = row.Table.Columns.Contains("IsPartyMember") ? row["IsPartyMember"].ToString() == "1" : (bool?)null;
                        newEmployee.IsSolider = row.Table.Columns.Contains("IsSolider") ? row["IsSolider"].ToString() == "1" : (bool?)null;
                        
                        newEmployee.SocialInsuranceNumber = row.Table.Columns.Contains("SocialInsuranceNumber") ? row["SocialInsuranceNumber"].ToString() : null;
                        newEmployee.TaxIdentificationNumber = row.Table.Columns.Contains("TaxIdentificationNumber") ? row["TaxIdentificationNumber"].ToString() : null;
                        newEmployee.CitizenIdentificationNumber = row.Table.Columns.Contains("CitizenIdentificationNumber") ? row["CitizenIdentificationNumber"].ToString() : null;
                        _ServiceFactory.NewEmployee(newEmployee, avatar);
                        i = i + 1;
                        ProgressBarValue = (i * 100) / _ExcelData.Count;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            });
        }
        
        private void LoadDataFromExcel(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Add this line
            var dataTable = new DataTable();
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                // Get the dimensions of the worksheet
                var start = worksheet.Dimension.Start;
                var end = worksheet.Dimension.End;

                // Add columns to DataTable
                for (int col = start.Column; col <= end.Column; col++)
                {
                    string columnHeader = worksheet.Cells[start.Row, col].Text;
                    dataTable.Columns.Add(columnHeader);
                }

                // Add rows to DataTable
                for (int row = start.Row + 1; row <= end.Row; row++)
                {
                    var dataRow = dataTable.NewRow();
                    bool hasData = false;

                    for (int col = start.Column; col <= end.Column; col++)
                    {
                        var cellValue = worksheet.Cells[row, col].Text;
                        if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            hasData = true;
                        }
                        dataRow[col - start.Column] = cellValue;
                    }

                    if (hasData)
                    {
                        dataTable.Rows.Add(dataRow);
                    }
                }
            }
            var headers = dataTable.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToList();         
            ExcelData = dataTable.DefaultView;
        }
    }
}
