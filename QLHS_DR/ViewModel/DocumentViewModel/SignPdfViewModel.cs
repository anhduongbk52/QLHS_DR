using DevExpress.Office.DigitalSignatures;
using DevExpress.Pdf;
using DevExpress.Pdf.Native.BouncyCastle.Ocsp;
using EofficeCommonLibrary.Common.Util;
using QLHS_DR.ChatAppServiceReference;
using QLHS_DR.View.ProductView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace QLHS_DR.ViewModel.DocumentViewModel
{
    internal class SignPdfViewModel:BaseViewModel
    {
        #region "Properties and Field"


        private bool _CanOpenFile;
        public bool CanOpenFile
        {
            get => _CanOpenFile;
            set
            {
                if (_CanOpenFile != value)
                {
                    _CanOpenFile = value;
                    OnPropertyChanged("CanOpenFileElectrical");
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

        #endregion
        #region "Command"
        public ICommand LoadedWindowCommand { get; set; }

        public ICommand SignDocumentCommand { get; set; }

        #endregion
        public SignPdfViewModel()
        {
            SignDocumentCommand = new RelayCommand<Object>((p) => { if (_DocumentSource!=null) return true; else return false; }, (p) =>
            {
                try
                {
                    using (var signer = new PdfDocumentSigner(_DocumentSource))
                    {
                        IntPtr handle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
                        X509Certificate2 x509Certificate = CertUtil.SelectCert("Chọn chứng thư số", "Chọn một chứng thư số mà bạn sở hữu khóa sử dụng để ký số.", handle);
                        if(x509Certificate != null)
                        {
                            Pkcs7Signer pkcs7Signature = new Pkcs7Signer(x509Certificate, HashAlgorithmType.SHA256);

                            // Create a signature field on the first page:
                            var signatureFieldInfo = new PdfSignatureFieldInfo(1)
                            {
                                Name = "SignatureField",
                                SignatureBounds = new PdfRectangle(10, 10, 150, 150),
                                RotationAngle = PdfAcroFormFieldRotation.Rotate90
                            };

                            // Create a PKCS#7 signature:


                            // Apply a signature to a newly created signature field:
                            var cooperSignature = new PdfSignatureBuilder(pkcs7Signature, signatureFieldInfo);

                            // Specify an image and signer information:
                            string exePath = Assembly.GetExecutingAssembly().Location;
                            string jpgPath = Path.Combine(Path.GetDirectoryName(exePath), @"TestSignature\SignPicture.jpg");

                            cooperSignature.SetImageData(System.IO.File.ReadAllBytes(jpgPath));
                            cooperSignature.Location = "USA";
                            cooperSignature.Name = "Jane Cooper";
                            cooperSignature.Reason = "Acknowledgement";

                            // Apply a signature to an existing form field:
                            var santuzzaSignature = new PdfSignatureBuilder(pkcs7Signature, "SignatureField");

                            // Specify an image and signer information:
                            santuzzaSignature.SetImageData(System.IO.File.ReadAllBytes(jpgPath));
                            santuzzaSignature.Location = "Australia";
                            santuzzaSignature.Name = "Santuzza Valentina";
                            santuzzaSignature.Reason = "I Agree";

                            // Add signatures to an array:
                            PdfSignatureBuilder[] signatures = { cooperSignature, santuzzaSignature };

                            // Sign and save the document:
                            signer.SaveDocument("SignedDocument.pdf", signatures);
                        }  
                    }

                } catch(Exception ex)
                {
                    MessageBox.Show(ex.Message); 
                }
                
            });
        }
    }
}
