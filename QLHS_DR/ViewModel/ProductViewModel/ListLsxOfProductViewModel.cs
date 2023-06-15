using Microsoft.Win32;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.Core;
using QLHS_DR.View.LsxView;
using QLHS_DR.View.PdfView;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace QLHS_DR.ViewModel.ProductViewModel
{
    internal class ListLsxOfProductViewModel : BaseViewModel
    {
        #region "Properties and Fields"
        private ServiceFactory _ServiceFactory;
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
        private Lsx _SelectedLsx;
        public Lsx SelectedLsx
        {
            get => _SelectedLsx;
            set
            {
                if (_SelectedLsx != value)
                {
                    _SelectedLsx = value;
                    if (_SelectedLsx != null)
                    {
                        try
                        {
                            if (_SelectedLsx != null && _ServiceFactory != null)
                            {
                                PublicFiles = _ServiceFactory.GetPublicFilesOfLsx(_SelectedLsx.Id);
                            }
                        }
                        catch (Exception ex)
                        { MessageBox.Show(ex.Message); }
                    }
                    OnPropertyChanged("SelectedLsx");
                }
            }
        }
        private ObservableCollection<Lsx> _Lsxes;
        public ObservableCollection<Lsx> Lsxes
        {
            get => _Lsxes;
            set
            {
                if (_Lsxes != value)
                {
                    _Lsxes = value;
                    OnPropertyChanged("Lsxes");
                }
            }
        }
        private ObservableCollection<PublicFile> _PublicFiles;
        public ObservableCollection<PublicFile> PublicFiles
        {
            get => _PublicFiles;
            set
            {
                if (_PublicFiles != value)
                {
                    _PublicFiles = value;
                    OnPropertyChanged("PublicFiles");
                }
            }
        }
        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand NewLsxCommand { get; set; }
        public ICommand AddFileAttachmentCommand { get; set; }
        public ICommand DownloadFileAttachmentCommand { get; set; }
        public ICommand DownloadAllFilesAttachmentCommand { get; set; }
        public ICommand RemoveFileAttachmentCommand { get; set; }
        public ICommand ViewFileAttachmentCommand { get; set; }
        #endregion
        internal ListLsxOfProductViewModel(int productId)
        {
            _ServiceFactory = new ServiceFactory();
            PublicFiles = new ObservableCollection<PublicFile>();

            LoadedWindowCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                Lsxes = _ServiceFactory.GetLsxesOfProduct(productId);
            });

            NewLsxCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                NewLsxWindow newLsxWindow = new NewLsxWindow();
                newLsxWindow.ShowDialog();
                Lsxes = _ServiceFactory.GetLsxesOfProduct(productId);
            });
            AddFileAttachmentCommand = new RelayCommand<Object>((p) => { return true; }, (p) =>
            {
                bool status = true;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (var path in openFileDialog.FileNames)
                    {
                        PublicFile publicFile = new PublicFile()
                        {
                            FileName = Path.GetFileName(path),
                            FileExtension = Path.GetExtension(path)
                        };
                        byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                        if (_ServiceFactory.NewFileOfLsx(_SelectedLsx.Id, publicFile, fileBytes) == 0)
                        {
                            status = false;
                            break;
                        }
                    }
                    if (status)
                    {
                        MessageBox.Show("Tải lên thành công.");
                        PublicFiles = _ServiceFactory.GetPublicFilesOfLsx(_SelectedLsx.Id);
                    }
                    else { MessageBox.Show("Có lỗi xảy ra, vui lòng kiểm tra."); }
                }

            });
            ViewFileAttachmentCommand = new RelayCommand<PublicFile>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                ViewPublicFilePdfWindow viewPublicFilePdfWindow = new ViewPublicFilePdfWindow(p.Id);
                viewPublicFilePdfWindow.Show();
            });
            RemoveFileAttachmentCommand = new RelayCommand<PublicFile>((p) => { if (p != null && _SelectedLsx != null) return true; else return false; }, (p) =>
            {
                if (MessageBox.Show("Bạn có muốn xóa file: " + p.FileOfLsxes + " khỏi LSX số: " + _SelectedLsx.DOfficeNumber, "Cảnh báo!", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    _ServiceFactory.RemoveFileInLsx(p.Id, _SelectedLsx.Id);
                    PublicFiles = _ServiceFactory.GetPublicFilesOfLsx(_SelectedLsx.Id);
                }
            });
            DownloadFileAttachmentCommand = new RelayCommand<PublicFile>((p) => { if (p != null) return true; else return false; }, (p) =>
            {
                System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                folderBrowserDialog.Description = "Select a folder to save the file.";

                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string folderPath = folderBrowserDialog.SelectedPath;
                    string fileName = p.FileName; // Đặt tên file mặc định tại đây
                    string filePath = Path.Combine(folderPath, fileName);
                    try
                    {
                        byte[] content = _ServiceFactory.DownloadPublicFile(p.Id, false);
                        System.IO.File.WriteAllBytes(filePath, content);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show("An error occurred while saving the file: " + ex.Message);
                    }
                }
            });
            DownloadAllFilesAttachmentCommand = new RelayCommand<Object>((p) => { if (_PublicFiles != null) return true; else return false; }, (p) =>
            {
                System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                folderBrowserDialog.Description = "Select a folder to save the file.";

                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        foreach (PublicFile publicFile in _PublicFiles)
                        {
                            string folderPath = folderBrowserDialog.SelectedPath;

                            string fileName = publicFile.FileName;
                            string filePath = Path.Combine(folderPath, fileName);
                            byte[] content = _ServiceFactory.DownloadPublicFile(publicFile.Id, false);
                            System.IO.File.WriteAllBytes(filePath, content);
                        }
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show("An error occurred while saving the file: " + ex.Message);
                    }
                }
            });
        }
    }
}
