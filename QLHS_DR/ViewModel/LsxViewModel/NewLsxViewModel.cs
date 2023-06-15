using DevExpress.Pdf;
using DevExpress.XtraRichEdit;
using Microsoft.Win32;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.LsxViewModel
{
    internal class NewLsxViewModel : BaseViewModel
    {
        #region "Properties and Fields"
        private string _TextInput;
        private ServiceFactory _ServiceFactory;
        private PublicFile _PublicFile;
        private bool _IsBusy;
        public bool IsBusy
        {
            get => _IsBusy;
            set
            {
                if (_IsBusy != value)
                {
                    _IsBusy = value;
                    OnPropertyChanged("IsBusy");
                }
            }
        }
        private ObservableCollection<User> _Users;
        public ObservableCollection<User> Users
        {
            get => _Users;
            set
            {
                if (_Users != value)
                {
                    _Users = value;
                    if (_Users != null)
                    {
                        SelectedUserCreator = _Users.Where(x => x.FullName.SequenceEqual("Nguyễn Hà Nam")).FirstOrDefault();
                    }
                    OnPropertyChanged("Users");
                }
            }
        }
        private ObservableCollection<Department> _Departments;
        public ObservableCollection<Department> Departments
        {
            get => _Departments;
            set
            {
                if (_Departments != value)
                {
                    _Departments = value;
                    OnPropertyChanged("Departments");
                }
            }
        }
        private ObservableCollection<FileAttachment> _FileAttachments;
        public ObservableCollection<FileAttachment> FileAttachments
        {
            get => _FileAttachments;
            set
            {
                if (_FileAttachments != value)
                {
                    _FileAttachments = value;
                    OnPropertyChanged("FileAttachments");
                }
            }
        }

        private FileAttachment _SelectedFile;
        public FileAttachment SelectedFile
        {
            get => _SelectedFile;
            set
            {
                if (_SelectedFile != value)
                {
                    _SelectedFile = value;
                    if (_SelectedFile.FileExtension.SequenceEqual(".pdf"))
                    {
                        DocumentSource = _SelectedFile.SourcePath;
                    }
                    OnPropertyChanged("SelectedFile");
                }
            }
        }
        private string _DocumentSource;
        public string DocumentSource
        {
            get => _DocumentSource;
            set
            {
                if (_DocumentSource != value)
                {
                    _DocumentSource = value;
                    OnPropertyChanged("DocumentSource");
                }
            }
        }
        private string _DOfficeNumber;
        public string DOfficeNumber
        {
            get => _DOfficeNumber;
            set
            {
                if (_DOfficeNumber != value)
                {
                    _DOfficeNumber = value;
                    OnPropertyChanged("DOfficeNumber");
                }
            }
        }
        private string _KHNumber;
        public string KHNumber
        {
            get => _KHNumber;
            set
            {
                if (_KHNumber != value)
                {
                    _KHNumber = value;
                    OnPropertyChanged("KHNumber");
                }
            }
        }
        private string _Name;
        public string Name
        {
            get => _Name;
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        private string _Refenrece;
        public string Reference
        {
            get => _Refenrece;
            set
            {
                if (_Refenrece != value)
                {
                    _Refenrece = value;
                    OnPropertyChanged("Reference");
                }
            }
        }
        private string _ContentRequest;
        public string ContentRequest
        {
            get => _ContentRequest;
            set
            {
                if (_ContentRequest != value)
                {
                    _ContentRequest = value;
                    OnPropertyChanged("ContentRequest");
                }
            }
        }
        private string _TimeRequest;
        public string TimeRequest
        {
            get => _TimeRequest;
            set
            {
                if (_TimeRequest != value)
                {
                    _TimeRequest = value;
                    OnPropertyChanged("TimeRequest");
                }
            }
        }
        private string _Description;
        public string Description
        {
            get => _Description;
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertyChanged("Description");
                }
            }
        }
        private Department _SelectedDepartmentCreator;
        public Department SelectedDepartmentCreator
        {
            get => _SelectedDepartmentCreator;
            set
            {
                if (_SelectedDepartmentCreator != value)
                {
                    _SelectedDepartmentCreator = value;
                    if (_SelectedDepartmentCreator != null && _ServiceFactory != null)
                    {
                        Users = _ServiceFactory.GetUserOfDepartment(_SelectedDepartmentCreator.Id);
                    }
                    OnPropertyChanged("SelectedDepartmentCreator");
                }
            }
        }
        private Department _SelectedReceiveDepartment;
        public Department SelectedReceiveDepartment
        {
            get => _SelectedReceiveDepartment;
            set
            {
                if (_SelectedReceiveDepartment != value)
                {
                    _SelectedReceiveDepartment = value;
                    OnPropertyChanged("SelectedReceiveDepartment");
                }
            }
        }
        private User _SelectedUserCreator;
        public User SelectedUserCreator
        {
            get => _SelectedUserCreator;
            set
            {
                if (_SelectedUserCreator != value)
                {
                    _SelectedUserCreator = value;
                    OnPropertyChanged("SelectedUserCreator");
                }
            }
        }
        private DateTime _StartDate;
        public DateTime StartDate
        {
            get => _StartDate;
            set
            {
                if (_StartDate != value)
                {
                    _StartDate = value;
                    OnPropertyChanged("StartDate");
                }
            }
        }
        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand OpenLsxFileCommand { get; set; }
        public ICommand OpenAttFileNewCommand { get; set; }
        public ICommand RemoveFileCommand { get; set; }
        public ICommand ClearFileCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        #endregion
        internal NewLsxViewModel()
        {
            _ServiceFactory = new ServiceFactory();
            FileAttachments = new ObservableCollection<FileAttachment>();
            LoadedWindowCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                Departments = _ServiceFactory.GetDepartments();
                SelectedDepartmentCreator = _Departments.Where(x => x.Code == "KH").FirstOrDefault();
                var temp = _ServiceFactory.GetMyDepartment(SectionLogin.Ins.CurrentUser.Id);
                SelectedReceiveDepartment = _Departments.Where(x => x.Id == temp.Id).FirstOrDefault();
                //OpenFileLsx();
            });
            SaveCommand = new RelayCommand<Window>((p) => { if (p != null && (_DOfficeNumber != null || _KHNumber != null)) return true; else return false; }, (p) =>
            {
                bool isSuccess = false;
                try
                {
                    IsBusy = true;
                    Lsx lsx = new Lsx()
                    {
                        DateStart = _StartDate,
                        ContentRequest = _ContentRequest,
                        DOfficeNumber = _DOfficeNumber,
                        KHNumber = _KHNumber,
                        TimeRequest = _TimeRequest,
                        Reference = _Refenrece,
                        FromDepartmentId = _SelectedDepartmentCreator.Id,
                        ReceiveDepartmentId = _SelectedReceiveDepartment.Id,
                        Description = _Description,
                        Name = _Name
                    };
                    if (_PublicFile != null)
                    {
                        byte[] fileBytes = System.IO.File.ReadAllBytes(_PublicFile.FilePath);
                        lsx.Id = _ServiceFactory.NewLsx(lsx, _PublicFile, fileBytes);
                    }
                    else
                    {
                        lsx.Id = _ServiceFactory.NewLsx(lsx, null, null);
                    }
                    if (lsx.Id != 0)
                    {
                        isSuccess = true;
                        foreach (FileAttachment fileAttachment in _FileAttachments)
                        {
                            PublicFile publicFile = new PublicFile()
                            {
                                FileExtension = fileAttachment.FileExtension,
                                Description = _Description,
                                FileName = fileAttachment.FileNameWithExt,
                            };
                            byte[] fileBytes = System.IO.File.ReadAllBytes(fileAttachment.SourcePath);
                            if (_ServiceFactory.NewFileOfLsx(lsx.Id, publicFile, fileBytes) == 0)
                            {
                                isSuccess = false;
                            }
                        }
                    }
                    else
                    {
                        isSuccess = false;
                    }
                    if (isSuccess)
                    {
                        MessageBox.Show("Lưu thành công.");
                        p.Close();
                    }
                    else
                    {
                        MessageBox.Show("Thao tác thất bại, vui lòng thử lại!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            });
            RemoveFileCommand = new RelayCommand<FileAttachment>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                if (_FileAttachments != null)
                {
                    FileAttachments.Remove(p);
                }
            });
            ClearFileCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                FileAttachments.Clear();
            });
            OpenLsxFileCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                OpenFileLsx();
            });
            OpenAttFileNewCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (var path in openFileDialog.FileNames)
                    {
                        if (!FileAttachments.Any(x => x.FileNameWithoutExt == Path.GetFileNameWithoutExtension(path)))
                        {
                            FileAttachments.Add(new FileAttachment()
                            {
                                No = FileAttachments.Count() + 1,
                                SourcePath = path,
                                FileExtension = Path.GetExtension(path),
                                FileNameWithoutExt = Path.GetFileNameWithoutExtension(path),
                                FileNameWithExt = Path.GetFileName(path)
                            }); ;
                        }
                    }
                }
            });
        }
        private void OpenFileLsx()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Zip files (*.zip)|*.zip|PDF files (*.pdf)|*.pdf|Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|Word files (*.doc, *.docx)|*.doc;*.docx|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                if (!FileAttachments.Any(x => x.FileNameWithoutExt == Path.GetFileNameWithoutExtension(openFileDialog.FileName) && x.FileExtension == Path.GetExtension(openFileDialog.FileName)))
                {
                    Name = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    FileAttachments.Add(new FileAttachment()
                    {
                        SourcePath = openFileDialog.FileName,
                        FileExtension = Path.GetExtension(openFileDialog.FileName),
                        FileNameWithoutExt = _Name,
                        FileNameWithExt = Path.GetFileName(openFileDialog.FileName),
                    });
                    if (DocScan.IsImageFile(openFileDialog.FileName))
                    {
                        try
                        {
                            IsBusy = true;
                            string executableFolderPath = AppDomain.CurrentDomain.BaseDirectory;
                            string outputFolderPath = Path.Combine(executableFolderPath, "draft");

                            // Đảm bảo thư mục nháp tồn tại

                            Directory.CreateDirectory(outputFolderPath);

                            string outputFilePath = Path.Combine(outputFolderPath, "result.pdf");

                            using (RichEditDocumentServer server = new RichEditDocumentServer())
                            {
                                DevExpress.XtraRichEdit.API.Native.DocumentImage docImage = server.Document.Images.Append(DevExpress.XtraRichEdit.API.Native.DocumentImageSource.FromFile(openFileDialog.FileName));
                                server.Document.Sections[0].Page.Width = docImage.Size.Width + server.Document.Sections[0].Margins.Right + server.Document.Sections[0].Margins.Left;
                                server.Document.Sections[0].Page.Height = docImage.Size.Height + server.Document.Sections[0].Margins.Top + server.Document.Sections[0].Margins.Bottom;
                                using (FileStream fs = new FileStream(outputFilePath, FileMode.Create))
                                {
                                    server.ExportToPdf(fs);
                                }
                            }
                            DocumentSource = outputFilePath;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            IsBusy = false;
                        }
                    }
                    else if (Path.GetExtension(openFileDialog.FileName) == ".pdf")
                    {
                        DocumentSource = openFileDialog.FileName;
                        try
                        {
                            using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
                            {
                                Regex trimmer = new Regex(@"\s+"); // Xóa khoảng trắng thừa trong chuỗi                                   

                                documentProcessor.LoadDocument(_DocumentSource);
                                PdfTextSearchParameters searchParametersDOfficeNumber = new PdfTextSearchParameters()
                                {
                                    CaseSensitive = false, //Không phân biệt chữ hoa và thường
                                    Direction = PdfTextSearchDirection.Forward,//Hướng tìm kiếm
                                    WholeWords = true // 
                                };
                                DOfficeNumber = "";
                                _TextInput = documentProcessor.Text;
                                string pattern = @"(?<=Số:\s*)\d+\/\w+";
                                var matches = Regex.Matches(_TextInput, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                if (matches.Count > 0)
                                {
                                    DOfficeNumber = trimmer.Replace(matches[matches.Count - 1].Value.Trim().ToUpper(), "");
                                }
                                string pattern1 = @"(?<=Số:\s*)\d+(.*?)(KH\.).*\n";
                                var matches1 = Regex.Matches(_TextInput, pattern1, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                if (matches1.Count > 0)
                                {
                                    KHNumber = trimmer.Replace(matches1[matches1.Count - 1].Value.Trim().ToUpper(), "");
                                }
                                //Get Reference
                                string patternReference = @"(?<=CĂN CỨ.+)(.*?)(?=II)";
                                var matchesReferences = Regex.Matches(_TextInput, patternReference, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                                if (matchesReferences.Count > 0)
                                {
                                    this.Reference = matchesReferences[0].Value.Trim().TrimStart(new char[] { ':', '.', '\n', '\r' });
                                }

                                string patternContentRequest = @"(?<=NỘI DUNG(:*))(.*?)(?=II)";
                                var matchesContentRequests = Regex.Matches(_TextInput, patternContentRequest, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                                if (matchesContentRequests.Count > 0)
                                {
                                    this.ContentRequest = matchesContentRequests[0].Value.Trim().TrimStart(new char[] { ':', '.', '\n', '\r' }); ;
                                }
                                string patternTimeRequest = @"(?<=(THỜI GIAN YÊU CẦU(:*)))(.*?)(?=\.\/\.)";
                                var matchesTimeRequests = Regex.Matches(_TextInput, patternTimeRequest, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                                if (matchesTimeRequests.Count > 0)
                                {
                                    this.TimeRequest = matchesTimeRequests[0].Value.Trim().TrimStart(new char[] { ':', '.', '\n', '\r' });
                                }
                                string patternStartDate = @"(?<=Ngày\s*giao\s*lệnh\s*)(.*?)(?=\n)";
                                var matchesStartDates = Regex.Matches(_TextInput, patternStartDate, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                                if (matchesStartDates.Count > 0)
                                {
                                    string temp = matchesStartDates[0].Value.Trim().TrimStart(new char[] { ':', '.', '\n', '\r' });
                                    StartDate = Convert.ToDateTime(trimmer.Replace(temp, " "));
                                }
                            }
                        }
                        catch { }
                    }
                    else if (DocScan.IsZipFile(openFileDialog.FileName))
                    {
                        try
                        {
                            string pattern = @"^\d+_KH.*(?=\.zip$)";
                            Match match = Regex.Match(Path.GetFileName(openFileDialog.FileName), pattern);

                            if (match.Success)
                            {
                                string dofficeNum = match.Value.Replace("_", "/");

                                string executableFolderPath = AppDomain.CurrentDomain.BaseDirectory;
                                string outputFolderPath = Path.Combine(executableFolderPath, "draft");

                                Directory.CreateDirectory(outputFolderPath);
                                outputFolderPath = Path.Combine(outputFolderPath, "ZipExtracted");
                                Directory.Delete(outputFolderPath, true);

                                Directory.CreateDirectory(outputFolderPath);
                                DocScan.ExtractZipFile(openFileDialog.FileName, outputFolderPath);

                                string[] filePaths = Directory.GetFiles(outputFolderPath);

                                if (filePaths.Count() > 0)
                                {
                                    foreach (var path in filePaths)
                                    {
                                        if (Path.GetExtension(path).ToLower() == ".pdf")
                                        {
                                            using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
                                            {
                                                Regex trimmer = new Regex(@"\s+"); // Xóa khoảng trắng thừa trong chuỗi                                   

                                                documentProcessor.LoadDocument(path);
                                                PdfTextSearchParameters searchParametersDOfficeNumber = new PdfTextSearchParameters()
                                                {
                                                    CaseSensitive = false, //Không phân biệt chữ hoa và thường
                                                    Direction = PdfTextSearchDirection.Forward,//Hướng tìm kiếm
                                                    WholeWords = true // 
                                                };
                                                _TextInput = documentProcessor.Text;
                                                string patternSerch = @"(?<=Số:\s*)\d+\/\w+";
                                                var matches = Regex.Matches(_TextInput, patternSerch, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                                if (matches.Count > 0)
                                                {
                                                    string num = trimmer.Replace(matches[matches.Count - 1].Value.Trim().ToUpper(), "");
                                                    if (num == dofficeNum)
                                                    {
                                                        DocumentSource = path;
                                                        DOfficeNumber = dofficeNum;
                                                        Name = Path.GetFileNameWithoutExtension(path);
                                                        string pattern1 = @"(?<=Số:\s*)\d+(.*?)(KH\.).*\n";
                                                        var matches1 = Regex.Matches(_TextInput, pattern1, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                                        if (matches1.Count > 0)
                                                        {
                                                            KHNumber = trimmer.Replace(matches1[matches1.Count - 1].Value.Trim().ToUpper(), "");
                                                        }
                                                        //Get Reference
                                                        string patternReference = @"(?<=CĂN CỨ.+)(.*?)(?=II)";
                                                        var matchesReferences = Regex.Matches(_TextInput, patternReference, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                                                        if (matchesReferences.Count > 0)
                                                        {
                                                            this.Reference = matchesReferences[0].Value.Trim().TrimStart(new char[] { ':', '.', '\n', '\r' });
                                                        }

                                                        string patternContentRequest = @"(?<=NỘI DUNG(:*))(.*?)(?=II)";
                                                        var matchesContentRequests = Regex.Matches(_TextInput, patternContentRequest, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                                                        if (matchesContentRequests.Count > 0)
                                                        {
                                                            this.ContentRequest = matchesContentRequests[0].Value.Trim().TrimStart(new char[] { ':', '.', '\n', '\r' }); ;
                                                        }
                                                        string patternTimeRequest = @"(?<=(THỜI GIAN YÊU CẦU(:*)))(.*?)(?=\.\/\.)";
                                                        var matchesTimeRequests = Regex.Matches(_TextInput, patternTimeRequest, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                                                        if (matchesTimeRequests.Count > 0)
                                                        {
                                                            this.TimeRequest = matchesTimeRequests[0].Value.Trim().TrimStart(new char[] { ':', '.', '\n', '\r' });
                                                        }
                                                        string patternStartDate = @"(?<=Ngày\s*giao\s*lệnh\s*)(.*?)(?=\n)";
                                                        var matchesStartDates = Regex.Matches(_TextInput, patternStartDate, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                                                        if (matchesStartDates.Count > 0)
                                                        {
                                                            string temp = matchesStartDates[0].Value.Trim().TrimStart(new char[] { ':', '.', '\n', '\r' });
                                                            StartDate = Convert.ToDateTime(trimmer.Replace(temp, " "));
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (!FileAttachments.Any(x => x.FileNameWithoutExt == Path.GetFileNameWithoutExtension(path)))
                                        {
                                            FileAttachments.Add(new FileAttachment()
                                            {
                                                No = FileAttachments.Count() + 1,
                                                SourcePath = path,
                                                FileExtension = Path.GetExtension(path),
                                                FileNameWithoutExt = Path.GetFileNameWithoutExtension(path),
                                                FileNameWithExt = Path.GetFileName(path)
                                            }); ;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    if (_DocumentSource != null)
                    {
                        _PublicFile = new PublicFile()
                        {
                            FilePath = _DocumentSource,
                            Description = _Description,
                            FileExtension = Path.GetExtension(openFileDialog.FileName)
                        };
                    }

                }
                else
                {
                    MessageBox.Show("File đã được thêm vào!");
                }
            }
        }
    }
    internal class FileAttachment
    {
        public int No { get; set; }
        public string SourcePath { get; set; }
        public string FileNameWithExt { get; set; }
        public string FileNameWithoutExt { get; set; }
        public string FileExtension { get; set; }
    }
}
