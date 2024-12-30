using QLHS_DR.Core;
using System;
using System.Collections.Generic;
using QLHS_DR.ChatAppServiceReference;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Forms;
using EofficeClient.Core;
using System.IdentityModel.Metadata;
using DevExpress.Mvvm.Native;
using DevExpress.XtraRichEdit.Import.Html;
using QLHS_DR.CustomControl;
using QLHS_DR.View.EmployeeView;
using QLHS_DR.View.PdfView;
using System.IO;

namespace QLHS_DR.ViewModel.EmployeeViewModel
{
    internal class EmployeePanelViewModel:BaseViewModel
    {
        #region "Properties and Fields"
        public List<string> GenderList { get; set; } = new List<string> { "Nam", "Nữ" };

        private bool _IsReadOnly;
        public bool IsReadOnly
        {
            get => _IsReadOnly;
            set
            {
                if (_IsReadOnly != value)
                {
                    _IsReadOnly = value;                   
                    NotifyPropertyChanged("IsReadOnly");
                }
            }
        }
        private bool _CanEdit;
        public bool CanEdit
        {
            get => _CanEdit;
            set
            {
                if (_CanEdit != value)
                {
                    _CanEdit = value;
                    NotifyPropertyChanged("CanEdit");
                }
            }
        }
        ServiceFactory _ServiceFactory = new ServiceFactory();
        ObservableCollection<Department> _Departments;
        ObservableCollection<Position> _Positions;
        private Employee _Employee;
        public Employee Employee
        {
            get => _Employee;
            set
            {
                if (_Employee != value)
                {
                    _Employee = value;
                    if(_Employee != null)
                    {
                        EmployeeAvatar = _ServiceFactory.GetAvatar(_Employee.Id);
                        Employee.EmployeeDepartments = _ServiceFactory.LoadEmployeeDepartments(_Employee.Id).ToArray();
                        EmployeeDocuments = _ServiceFactory.LoadEmployeeDocuments(_Employee.Id, false);
                        for (int i = 0; i < _Employee.EmployeeDepartments.Length; i++)
                        {
                            Employee.EmployeeDepartments[i].Department = _Departments.Where(x => x.Id == Employee.EmployeeDepartments[i].DepartmentId).FirstOrDefault();
                            Employee.EmployeeDepartments[i].Position = _Positions.Where(x=>x.Id == Employee.EmployeeDepartments[i].PositionId).FirstOrDefault();
                        }
                    }                   
                    NotifyPropertyChanged("Employee");
                    NotifyPropertyChanged("Employee.IsSolider");
                    NotifyPropertyChanged("Employee.IsPartyMember");
                    NotifyPropertyChanged("Employee.EmployeeDepartments");
                }
            }
        }
        private byte[] _EmployeeAvatar;
        public byte[] EmployeeAvatar
        {
            get => _EmployeeAvatar;
            set
            {
                if (_EmployeeAvatar != value)
                {
                    _EmployeeAvatar = value;
                    NotifyPropertyChanged("EmployeeAvatar");
                }
            }
        }
        private ObservableCollection<EmployeeDocument> _EmployeeDocuments;
        public ObservableCollection<EmployeeDocument> EmployeeDocuments
        {
            get => _EmployeeDocuments;
            set
            {
                if (_EmployeeDocuments != value)
                {
                    _EmployeeDocuments = value;
                    NotifyPropertyChanged("EmployeeDocuments");
                }
            }
        }
        private EmployeeDocument _EmployeeDocumentSelected;
        public EmployeeDocument EmployeeDocumentSelected
        {
            get => _EmployeeDocumentSelected;
            set
            {
                if (_EmployeeDocumentSelected != value)
                {
                    _EmployeeDocumentSelected = value;
                    NotifyPropertyChanged("EmployeeDocumentSelected");
                }
            }
        }
        private ObservableCollection< EmployeeDocument> _MultiEmployeeDocumentSelected;
        public ObservableCollection<EmployeeDocument> MultiEmployeeDocumentSelected
        {
            get => _MultiEmployeeDocumentSelected;
            set
            {
                if (_MultiEmployeeDocumentSelected != value)
                {
                    _MultiEmployeeDocumentSelected = value;
                    NotifyPropertyChanged("MultiEmployeeDocumentSelected");
                }
            }
        }
        #endregion
        #region "Command"

        public ICommand SaveChangeEmployeeInfoCommand { get; set; }
        public ICommand UploadFileCommand { get; set; }
        public ICommand DeleteFileCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand EditEmployeeDocumentCommand { get; set; }
        public ICommand DownloadFileCommand { get; set; }
        #endregion
        internal EmployeePanelViewModel()
        {
            _ServiceFactory = new ServiceFactory();
            _Departments = _ServiceFactory.GetDepartments();
            _Positions = _ServiceFactory.LoadPositions();
            IsReadOnly = !SectionLogin.Ins.ListPermissions.Any(x => x.Code == "employeeEditEmployeeInfo");
            CanEdit = !_IsReadOnly;
            MultiEmployeeDocumentSelected = new ObservableCollection<EmployeeDocument>();
            
            SaveChangeEmployeeInfoCommand = new RelayCommand<Object>((p) => { if (!IsReadOnly) return true; else return false; }, (p) =>
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Bạn có muốn lưu thay đổi không", "Cảnh báo", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        if(_ServiceFactory.SaveChangeEmployee(_Employee,_EmployeeAvatar))
                        {
                            System.Windows.MessageBox.Show("Cập nhật thành công");
                        }                          
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                        System.Windows.MessageBox.Show(ex.StackTrace);
                    }
                    finally
                    {
                        
                    }
                }
            });

            UploadFileCommand = new RelayCommand<Object>((p) => { return true;  }, (p) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "All files (*.*)|*.*",
                    Multiselect = true
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string fileName in openFileDialog.FileNames)
                    {
                        byte[] content = System.IO.File.ReadAllBytes(fileName);

                        _ServiceFactory.UploadEmployeeDocument(content, new EmployeeDocument()
                        {
                            EmployeeId = _Employee.Id,
                            FileName = System.IO.Path.GetFileName(fileName),
                            IsDeleted = false,
                            IsEncrypted = false
                        });
                    }

                    // Tải lại danh sách tài liệu sau khi upload xong
                    EmployeeDocuments = _ServiceFactory.LoadEmployeeDocuments(_Employee.Id, false);
                }
            });

            DeleteFileCommand = new RelayCommand<Object>((p) => { if (_EmployeeDocumentSelected!=null) return true; else return false; }, (p) =>
            {
                _ServiceFactory.SetDeleteEmployeeDocument(_EmployeeDocumentSelected.Id);
                EmployeeDocuments = _ServiceFactory.LoadEmployeeDocuments(_Employee.Id, false);
            });
            EditEmployeeDocumentCommand = new RelayCommand<Object>((p) => { if (_EmployeeDocumentSelected != null) return true; else return false; }, (p) =>
            {
                var dialog = new EditEmployeeDocumentWindow(_EmployeeDocumentSelected);
                if (dialog.ShowDialog() == true)
                {
                    _ServiceFactory.ChangeEmployeeDocumentInfo(_EmployeeDocumentSelected.Id, dialog.DocumentName, dialog.Description);
                    EmployeeDocuments = _ServiceFactory.LoadEmployeeDocuments(_Employee.Id, false);
                }                
            });
            OpenFileCommand = new RelayCommand<Object>((p) => { if (_EmployeeDocumentSelected != null) return true; else return false; }, (p) =>
            {
                if (String.Compare(_EmployeeDocumentSelected.FileExtension, ".pdf", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    try
                    {
                        // Tải dữ liệu từ service
                        byte[] temp = _ServiceFactory.DownloadEmployeeDocument(_EmployeeDocumentSelected.Id);
                        if (temp != null && temp.Length > 0) // Kiểm tra dữ liệu hợp lệ
                        {
                            using (MemoryStream stream = new MemoryStream(temp))
                            {
                                // Hiển thị PDF Viewer
                                CommonPdfViewer commonPdfViewer = new CommonPdfViewer(stream);
                                commonPdfViewer.ShowDialog();
                            }
                        }
                        else
                        {
                            // Thông báo nếu không có dữ liệu
                            System.Windows.MessageBox.Show("Không thể tải tài liệu. Dữ liệu trống.", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Hiển thị lỗi nếu xảy ra
                        System.Windows.MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Hiển thị hộp thoại xác nhận
                    var result = System.Windows.MessageBox.Show(
                        "File này không phải PDF. Bạn có muốn tải về không?",
                        "Xác nhận",
                        System.Windows.MessageBoxButton.YesNo,
                        System.Windows.MessageBoxImage.Question);

                    if (result == System.Windows.MessageBoxResult.Yes)
                    {
                        // Mở FolderBrowserDialog để chọn thư mục lưu
                        using (System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog())
                        {
                            folderDialog.Description = "Chọn thư mục để lưu file";
                            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                string selectedPath = folderDialog.SelectedPath;
                                try
                                {
                                    // Tải dữ liệu từ service
                                    byte[] temp = _ServiceFactory.DownloadEmployeeDocument(_EmployeeDocumentSelected.Id);
                                    if (temp != null && temp.Length > 0)
                                    {
                                        // Lưu file vào thư mục đã chọn
                                        string filePath = Path.Combine(selectedPath, _EmployeeDocumentSelected.FileName);
                                        System.IO.File.WriteAllBytes(filePath, temp);

                                        // Thông báo lưu thành công
                                        System.Windows.MessageBox.Show($"File đã được lưu tại: {filePath}", "Thành công", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                                    }
                                    else
                                    {
                                        // Thông báo nếu dữ liệu trống
                                        System.Windows.MessageBox.Show("Không thể tải tài liệu. Dữ liệu trống.", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // Hiển thị lỗi nếu xảy ra
                                    System.Windows.MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                                }
                            }
                        }
                    }
                }    
               
            });
            DownloadFileCommand = new RelayCommand<Object>((p) => {   return MultiEmployeeDocumentSelected != null && MultiEmployeeDocumentSelected.Any();}, (p) =>
            {
                // Mở FolderBrowserDialog để chọn thư mục lưu
                using (System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    folderDialog.Description = "Chọn thư mục để lưu file";
                    if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string selectedPath = folderDialog.SelectedPath;
                        try
                        {
                            // Lặp qua các tài liệu đã chọn
                            foreach (var document in MultiEmployeeDocumentSelected)
                            {
                                // Tải dữ liệu từ service
                                byte[] temp = _ServiceFactory.DownloadEmployeeDocument(document.Id);
                                if (temp != null && temp.Length > 0)
                                {
                                    // Lưu file vào thư mục đã chọn
                                    string filePath = Path.Combine(selectedPath, document.FileName);
                                    System.IO.File.WriteAllBytes(filePath, temp);

                                    // Thông báo lưu thành công
                                    System.Windows.MessageBox.Show($"File {document.FileName} đã được lưu tại: {filePath}", "Thành công", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                                }
                                else
                                {
                                    // Thông báo nếu dữ liệu trống
                                    System.Windows.MessageBox.Show($"Không thể tải tài liệu {document.FileName}. Dữ liệu trống.", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Hiển thị lỗi nếu xảy ra
                            System.Windows.MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        }
                    }
                }
            });
        }
    }
}
