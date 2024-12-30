
using Microsoft.Win32;
using OfficeOpenXml;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.View.EmployeeView;
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
    public class ColumnMapping
    {
        public EmployeeField EmployeeField { get; set; }
        public string ExcelColumn { get; set; }
    }
    public class EmployeeField
    {
        public string EmployeeFieldName { get; set; }
        public string EmployeeFieldNameDescription { get; set; }
    }
    internal class ImportEmployeeDataViewModel:BaseViewModel
    {
        #region "Properties and Fields"

        private ObservableCollection<ColumnMapping> _ColumnMappings;
        public ObservableCollection<ColumnMapping> ColumnMappings
        {
            get => _ColumnMappings;
            set
            {
                if (_ColumnMappings != value)
                {
                    _ColumnMappings = value;
                    NotifyPropertyChanged("ColumnMappings");
                }
            }
        }

        private ObservableCollection<string> _ExcelHeaderColumns;
        public ObservableCollection<string> ExcelHeaderColumns
        {
            get => _ExcelHeaderColumns;
            set
            {
                if (_ExcelHeaderColumns != value)
                {
                    _ExcelHeaderColumns = value;
                    NotifyPropertyChanged("ExcelHeaderColumns");
                }
            }
        }
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

        private ObservableCollection<EmployeeField> _EmployeeFields;
        public ObservableCollection<EmployeeField> EmployeeFields
        {
            get => _EmployeeFields;
            set
            {
                if (_EmployeeFields != value)
                {
                    _EmployeeFields = value;
                    NotifyPropertyChanged("EmployeeFields");
                }
            }
        }
        #endregion
        #region "Command"
      
        public ICommand OpenExcelCommand { get; set; }
        public ICommand RunCommand { get; set; }
        #endregion
        internal ImportEmployeeDataViewModel()
        {

            EmployeeFields = new ObservableCollection<EmployeeField>()
            {
                new EmployeeField(){EmployeeFieldName="MSNV",EmployeeFieldNameDescription="MSNV"},
                new EmployeeField(){EmployeeFieldName="FirtName",EmployeeFieldNameDescription="Tên"},
                new EmployeeField(){EmployeeFieldName="LastName",EmployeeFieldNameDescription="Họ và tên đệm"},
                new EmployeeField(){EmployeeFieldName="DateOfBirth",EmployeeFieldNameDescription="Ngày sinh"},
                new EmployeeField(){EmployeeFieldName="Gender",EmployeeFieldNameDescription="Giới tính"},
                new EmployeeField(){EmployeeFieldName="Email",EmployeeFieldNameDescription="Email"},
                new EmployeeField(){EmployeeFieldName="PhoneNumber",EmployeeFieldNameDescription="Số điện thoại"},
                new EmployeeField(){EmployeeFieldName="Address",EmployeeFieldNameDescription="Địa chỉ"},
                new EmployeeField(){EmployeeFieldName="HireDate",EmployeeFieldNameDescription="Ngày tuyển dụng"},
                new EmployeeField(){EmployeeFieldName="IsActive",EmployeeFieldNameDescription="IsActive"},
                new EmployeeField(){EmployeeFieldName="SocialInsuranceNumber",EmployeeFieldNameDescription="Số BHXH"},
                new EmployeeField(){EmployeeFieldName="TaxIdentificationNumber",EmployeeFieldNameDescription="Mã số thuế"},
                new EmployeeField(){EmployeeFieldName="DocumentNumber",EmployeeFieldNameDescription="Số hồ sơ"},
                new EmployeeField(){EmployeeFieldName="IsSolider",EmployeeFieldNameDescription="Bộ đội"},
                new EmployeeField(){EmployeeFieldName="IsPartyMember",EmployeeFieldNameDescription="Đảng viên"},
                new EmployeeField(){EmployeeFieldName="CitizenIdentificationNumber",EmployeeFieldNameDescription="Số CCCD"},
                new EmployeeField(){EmployeeFieldName="TrainedOccupation",EmployeeFieldNameDescription="Ngành nghề"},
                new EmployeeField(){EmployeeFieldName="Status",EmployeeFieldNameDescription="Trạng thái"}
            };
            ExcelHeaderColumns = new();
            ColumnMappings = new();
            
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
            RunCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                string[] formats = { "M/dd/yyyy", "MM/dd/yyyy", "dd/MM/yyyy", "d/MM/yyyy", "d/M/yyyy", "dd/M/yyyy","yyyy-MM-dd" }; // Thêm các định dạng ngày tháng khác nếu cần
                string msnvColName = GetExcelColumnForEmployeeField("MSNV");
                string firtNameColName = GetExcelColumnForEmployeeField("FirtName");
                string lastNameColName = GetExcelColumnForEmployeeField("LastName");
                string dateOfBirthColName = GetExcelColumnForEmployeeField("DateOfBirth");
                string addressColName = GetExcelColumnForEmployeeField("Address");
                string genderColName = GetExcelColumnForEmployeeField("Gender");
                string emailColName = GetExcelColumnForEmployeeField("Email");
                string phoneNumberColName = GetExcelColumnForEmployeeField("PhoneNumber");
                string hireDateColName = GetExcelColumnForEmployeeField("HireDate");
                string isActiveColName = GetExcelColumnForEmployeeField("IsActive");
                string socialInsuranceNumberColName = GetExcelColumnForEmployeeField("SocialInsuranceNumber");
                string taxIdentificationNumberColName = GetExcelColumnForEmployeeField("TaxIdentificationNumber");
                string documentNumberColName = GetExcelColumnForEmployeeField("DocumentNumber");
                string isSoliderColName = GetExcelColumnForEmployeeField("IsSolider");
                string isPartyMemberColName = GetExcelColumnForEmployeeField("IsPartyMember");
                string citizenIdentificationNumberColName = GetExcelColumnForEmployeeField("CitizenIdentificationNumber");
                string trainedOccupationColName = GetExcelColumnForEmployeeField("TrainedOccupation");
                string statusColName = GetExcelColumnForEmployeeField("Status");

                try
                {
                    foreach (DataRowView rowView in _ExcelData)
                    {
                        DateTime dateOfBirth=new();
                        DataRow row = rowView.Row;

                        if(dateOfBirthColName != null)
                        {
                            var dateOfBirthString = row[dateOfBirthColName].ToString();
                            DateTime.TryParseExact(dateOfBirthString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirth);
                        }    
                        

                        Employee newEmployee = new Employee
                        {
                            MSNV = row[msnvColName].ToString(),
                            FirtName = firtNameColName != null? row[firtNameColName].ToString():null,
                            LastName = firtNameColName != null ? row[lastNameColName].ToString():null,
                            DateOfBirth = dateOfBirth,
                            Address = addressColName != null ? row[addressColName].ToString() : null,
                            Gender = genderColName!=null? row[genderColName].ToString():null,
                            Email = emailColName != null ? row[emailColName].ToString() : null,
                            PhoneNumber = phoneNumberColName != null ? row[phoneNumberColName].ToString() : null,
                            HireDate = hireDateColName != null ? DateTime.Parse(row[hireDateColName].ToString()) : DateTime.Now,
                            IsActive = isActiveColName != null ? bool.Parse(row[isActiveColName].ToString()) : true,                                
                            SocialInsuranceNumber = socialInsuranceNumberColName != null ? row[socialInsuranceNumberColName].ToString() : null,
                            TaxIdentificationNumber = taxIdentificationNumberColName != null ? row[taxIdentificationNumberColName].ToString() : null,
                            CitizenIdentificationNumber = citizenIdentificationNumberColName != null ? row[citizenIdentificationNumberColName].ToString() : null,
                            
                        };                        
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }              
                
            });
        }
        public string GetExcelColumnForEmployeeField(string employeeFieldName)
        {
            var mapping = ColumnMappings.FirstOrDefault(cm => cm.EmployeeField?.EmployeeFieldName == employeeFieldName);
            return mapping?.ExcelColumn;
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
            ////ExcelHeaderColumns = new ObservableCollection<string>(headers);

            foreach(var item in headers)
            {
                ColumnMappings.Add(new ColumnMapping() { ExcelColumn = item});
            }
            ExcelData = dataTable.DefaultView;
        }
    }
}
