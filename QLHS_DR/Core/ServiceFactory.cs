﻿using DevExpress.Mvvm.Native;
using EofficeClient.Core;
using QLHS_DR.ChatAppServiceReference;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Windows;

namespace QLHS_DR.Core
{
    internal class ServiceFactory
    {
        MessageServiceClient _Client;
        internal ServiceFactory()
        {
            _Client = new MessageServiceClient();
        }
        internal void RemoveEmployeeToDepartment(int employeeId, int departmentId)
        {          
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.RemoveEmployeeToDepartment(employeeId, departmentId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                throw new FaultException(ex.Message);
            }          
        }
        internal bool UploadEmployeeDocument(byte[] content, EmployeeDocument employeeDocument)
        {
            bool result = false;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                result = _Client.UploadEmployeeDocument(content, employeeDocument);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show("Thao tác thất bại, vui lòng thử lại,");
                MessageBox.Show(ex.Message);
            }
            return result;
        }
        internal bool SaveChangeEmployeeDepartment(EmployeeDepartment employeeDepartment)
        {
            bool result = false;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                result = _Client.SaveChangeEmployeeDepartment(employeeDepartment);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                throw new FaultException(ex.Message);
            }
            return result;
        }
        internal byte[] DownloadEmployeeDocument(int employeeDocumentId)
        {
            byte[] ketqua = null;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                ketqua = _Client.DownloadEmployeeDocument(employeeDocumentId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return ketqua;
        }
        public ObservableCollection<EmployeeDocument> LoadEmployeeDocuments(int employeeId,bool isDeleted = false)
        {
            ObservableCollection<EmployeeDocument> kq = new ObservableCollection<EmployeeDocument>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                kq = _Client.LoadEmployeeDocuments(employeeId,isDeleted).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        public void ChangeEmployeeDocumentName(int employeeDocumentId, string newDocumentName)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.ChangeEmployeeDocumentName(employeeDocumentId, newDocumentName);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        public void ChangeEmployeeDocumentInfo(int employeeDocumentId, string documentName,string description)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.ChangeEmployeeDocumentInfo(employeeDocumentId, documentName, description);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        public void SetDeleteEmployeeDocument(int employeeDocumentId)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.SetDeleteEmployeeDocument(employeeDocumentId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }           
        }
        public int AddEmployeeToDepartment(EmployeeDepartment employeeDepartment)
        {
            int result=0;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                result = _Client.AddEmployeeToDepartment(employeeDepartment);
                _Client.Close();

            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return result;
        }
        public int NewEmployee(Employee employee, byte[] avatar)
        {
            int result = 0;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                result = _Client.NewEmployee(employee, avatar);
                _Client.Close();

            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return result;
        }
        public bool SaveChangeEmployee(Employee employee, byte[] avatar)
        {
            bool result = false;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                result = _Client.SaveChangeEmployee(employee, avatar);
                _Client.Close();
               
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        
        internal byte[] GetAvatar(int employeeId)
        {
            byte[] kq=null;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();

                kq = _Client.GetAvatar(employeeId);

                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal ObservableCollection<Employee> LoadEmployees()
        {
            ObservableCollection<Employee> kq = new ObservableCollection<Employee>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();

                kq = _Client.LoadEmployees().ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal async System.Threading.Tasks.Task<ObservableCollection<Employee>> LoadEmployeesAsync()
        {
            ObservableCollection<Employee> kq = new ObservableCollection<Employee>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();

                var employees = await System.Threading.Tasks.Task.Run(() => _Client.LoadEmployees());
                kq = employees.ToObservableCollection();

                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal ObservableCollection<Task> GetTaskOfProduct(int taskId)
        {
            ObservableCollection<Task> kq = new ObservableCollection<Task>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();

                kq = _Client.GetTaskOfProduct(taskId).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal ObservableCollection<int> GetAllProductIdOfTask(int taskId)
        {
            ObservableCollection<int> kq = new ObservableCollection<int>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();

                kq = _Client.GetAllProductIdOfTask(taskId).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal ObservableCollection<EmployeeDepartment> LoadEmployeeDepartments(int employeeId)
        {
            ObservableCollection<EmployeeDepartment> kq = new ObservableCollection<EmployeeDepartment>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();

                kq = _Client.LoadEmployeeDepartments(employeeId).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal ObservableCollection<EmployeeDepartment> LoadEmployeeDepartmentsByDepartmentId(int departmentId)
        {
            ObservableCollection<EmployeeDepartment> kq = new ObservableCollection<EmployeeDepartment>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();

                kq = _Client.LoadEmployeeDepartmentsByDepartmentId(departmentId).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal ObservableCollection<EmployeeDepartment> LoadEmployeeDepartmentsActiveByDepartmentId(int departmentId)
        {
            ObservableCollection<EmployeeDepartment> kq = new ObservableCollection<EmployeeDepartment>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();

                kq = _Client.LoadEmployeeDepartmentsActiveByDepartmentId(departmentId).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal ObservableCollection<EmployeeDepartment> LoadEmployeeDepartmentsUnActiveByDepartmentId(int departmentId)
        {
            ObservableCollection<EmployeeDepartment> kq = new ObservableCollection<EmployeeDepartment>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();

                kq = _Client.LoadEmployeeDepartmentsUnActiveByDepartmentId(departmentId).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal ObservableCollection<Department> GetDepartmentsOfEmployee(int employeeId)
        {
            ObservableCollection<Department> kq = new ObservableCollection<Department>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();

                kq = _Client.GetDepartmentsOfEmployee(employeeId).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
       
        internal ObservableCollection<LoginManager> LoadLoginManagers()
        {
            ObservableCollection<LoginManager> kq = new ObservableCollection<LoginManager>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();

                kq = _Client.LoadLoginManagers().ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        public int UpdateLoginManager(LoginManager loginManager)
        {
            int i = 0;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                i = _Client.UpdateLoginManager(loginManager);
                _Client.Close();

            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return i;
        }
        internal void SetTrustedLogin(int loginManagerId, bool isTrusted)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();

                _Client.SetTrustedLogin(loginManagerId, isTrusted);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal Product GetProductByProductCode(string productCode)
        {
            Product result = new Product();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                result = _Client.GetProductByProductCode(productCode);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return result;
        }
        internal ObservableCollection<Lsx> GetLsxesOfProduct(int productId)
        {
            ObservableCollection<Lsx> ketqua = new ObservableCollection<Lsx>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                ketqua = _Client.GetLsxesOfProduct(productId).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return ketqua;
        }
        internal byte[] DownloadPublicFile(int pubicFileId, bool convertToPdf)
        {
            byte[] ketqua = null;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                ketqua = _Client.DownloadPublicFile(pubicFileId, convertToPdf);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return ketqua;
        }
        internal void DeleteLsx(int lsxId)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.DeleteLsx(lsxId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal void RemoveFileInLsx(int publicFileId, int lsxId)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.RemoveFileInLsx(publicFileId, lsxId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal ObservableCollection<PublicFile> GetPublicFilesOfLsx(int lsxId)
        {
            ObservableCollection<PublicFile> publicFiles = new ObservableCollection<PublicFile>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                publicFiles = _Client.GetPublicFilesOfLsx(lsxId).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return publicFiles;
        }
        internal int NewMainJobTodo(MainJobTodo mainJobTodo)
        {
            int ketqua = 0;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                ketqua = _Client.NewMainJobTodo(mainJobTodo);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return ketqua;
        }
        internal void AddOrUpdateMainJobTodo(MainJobTodo mainJobTodo)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.AddOrUpdateMainJobTodo(mainJobTodo);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal ObservableCollection<MainJobTodo> GetMainJobTodoes(int lsxId)
        {
            ObservableCollection<MainJobTodo> mainJobTodoes = new ObservableCollection<MainJobTodo>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                mainJobTodoes = _Client.GetMainJobTodoes(lsxId).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return mainJobTodoes;
        }
        internal int AddProductToLsx(int lsxId, int productId)
        {
            int ketqua = 0;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                ketqua = _Client.AddProductToLsx(lsxId, productId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return ketqua;
        }
        internal void RemoveProductToLsx(int lsxId, int productId)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.RemoveProductToLsx(lsxId, productId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal void RemoveMainJobTodo(int mainJobTodoId)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.RemoveMainJobTodo(mainJobTodoId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }

        internal ObservableCollection<Product> GetProductsInLsx(int lsxId)
        {
            ObservableCollection<Product> result = new ObservableCollection<Product>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                result = _Client.GetProductsInLsx(lsxId).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return result;
        }


        internal int NewLsx(Lsx lsx, PublicFile publicFile, byte[] content)
        {
            int ketqua = 0;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                ketqua = _Client.NewLsx(lsx, publicFile, content);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return ketqua;
        }
        internal int NewFileOfLsx(int lsxId, PublicFile publicFile, byte[] content)
        {
            int ketqua = 0;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                ketqua = _Client.NewFileOfLsx(lsxId, publicFile, content);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return ketqua;
        }
        internal ObservableCollection<User> GetUserOfDepartment(int departmentId)
        {
            ObservableCollection<User> entitys = new ObservableCollection<User>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                entitys = _Client.GetUsersOfDepartment(departmentId).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return entitys;
        }
        internal Department GetMyDepartment(int userId)
        {
            Department entitys = new Department();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                entitys = _Client.GetDepartment(userId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return entitys;
        }
        internal TransformerDTO GetTransformerDTOById(int productId)
        {
            TransformerDTO result = null;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                result = _Client.GetTransformerDTOById(productId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return result;
        }
        internal ProductTypeNew GetProductTypeNew(int productTypeNewId)
        {
            ProductTypeNew result = null;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                result = _Client.GetProductTypeNew(productTypeNewId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return result;
        }
        internal void UploadProductManual(byte[] content, string description, string fileName, int docTitleId, int[] productIds)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.UploadProductManual(content, description, fileName, docTitleId, productIds);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show("Thao tác thất bại, vui lòng thử lại,");
                MessageBox.Show(ex.Message);
            }
        }


        internal void UpdateUserInformation(User user)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.UpdateUserInformation(user);
                _Client.Close();
                System.Windows.MessageBox.Show("Cập nhật thành công");
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show("Thao tác thất bại, vui lòng thử lại,");
                MessageBox.Show(ex.Message);
            }
        }
        internal void ResetAttemptCount(int userId)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.ResetAttemptCount(userId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show("Thao tác thất bại, vui lòng thử lại,");
                MessageBox.Show(ex.Message);
            }
        }
        internal void ResetPassword(int userId)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.ResetPassword(userId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show("Thao tác thất bại, vui lòng thử lại,");
                MessageBox.Show(ex.Message);
            }
        }
        internal void NewUser(User user, bool isDefaultPassword)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.NewUser(user, isDefaultPassword);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal ObservableCollection<Role> LoadRoles()
        {
            ObservableCollection<Role> roles = new ObservableCollection<Role>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                roles = _Client.LoadRoles().ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return roles;
        }
        internal ObservableCollection<User> LoadUsers()
        {
            ObservableCollection<User> users = new ObservableCollection<User>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                users = _Client.GetUsers().ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return users;
        }
        internal ObservableCollection<UserRole> LoadUserRolesByRoleId(int roleId)
        {
            ObservableCollection<UserRole> userRoles = new ObservableCollection<UserRole>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                userRoles = _Client.LoadUserRolesByRoleId(roleId).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return userRoles;
        }
        internal ObservableCollection<User> LoadUserInRole(int roleId)
        {
            ObservableCollection<User> users = new ObservableCollection<User>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                users = _Client.LoadUserInRole(roleId).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return users;
        }
        internal void NewUserRole(int roleId, int userId)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.NewUserRole(roleId, userId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal void RemoveUserRole(int roleId, int userId)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.RemoveUserRole(roleId, userId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal void RenameRole(int roleId, string newName)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.RenameRole(roleId, newName);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal void ChangeDescriptionRole(int roleId, string newDescription)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.RenameRole(roleId, newDescription);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal void NewRole(string roleName, string roleDescription)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.NewRole(roleName, roleDescription);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal void DeleteRole(int roleId)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.DeleteRole(roleId);
                _Client.Close();
                MessageBox.Show("Xóa thành công");
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal ObservableCollection<Department> GetDepartments()
        {

            ObservableCollection<Department> departments = new ObservableCollection<Department>();
            GroupDepartment[] groupDepartments;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                departments = _Client.GetDepartments().ToObservableCollection();
                groupDepartments = _Client.LoadGroupDepartments();
                _Client.Close();
                if (departments != null && groupDepartments != null)
                {
                    foreach (var department in departments)
                    {
                        department.GroupDepartment = groupDepartments.Where(x => x.Id == department.GroupDepartmentId).FirstOrDefault();
                    }
                }

            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return departments;
        }
        internal ObservableCollection<Position> LoadPositions()
        {
            ObservableCollection<Position> positions = new ObservableCollection<Position>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                positions = _Client.LoadPositions().ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return positions;
        }
        internal void AddUserToDepartment(int departmentId, int userId)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.AddUserToDepartment(departmentId, userId);
                _Client.Close();
                //MessageBox.Show("Thêm mới thành công");
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal void RemoveUserToDepartment(int departmentId, int userId)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.RemoveUserToDepartment(departmentId, userId);
                _Client.Close();
                MessageBox.Show("Xóa thành công");
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal ObservableCollection<UserDepartment> LoadUserDepartments()
        {
            Position[] positions;
            Department[] departments;
            ObservableCollection<UserDepartment> userDepartments = new ObservableCollection<UserDepartment>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                userDepartments = _Client.LoadUserDepartments().ToObservableCollection();
                departments = _Client.GetDepartments();
                positions = _Client.LoadPositions();
                if (userDepartments != null && positions != null)
                {
                    foreach (UserDepartment userDepartment in userDepartments)
                    {
                        userDepartment.Position = positions.Where(x => x.Id == userDepartment.PositionId).FirstOrDefault();
                        userDepartment.Department = departments.Where(x => x.Id == userDepartment.DepartmentId).FirstOrDefault();
                    }
                }
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return userDepartments;
        }
        internal ObservableCollection<UserDepartment> LoadUserDepartmentsByDepartmentId(int deparmentId)
        {
            Position[] positions;
            ObservableCollection<UserDepartment> userDepartments = new ObservableCollection<UserDepartment>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                userDepartments = _Client.LoadUserDepartmentsByDepartmentId(deparmentId).ToObservableCollection();
                positions = _Client.LoadPositions();
                if (userDepartments != null && positions != null)
                {
                    foreach (UserDepartment userDepartment in userDepartments)
                    {
                        userDepartment.Position = positions.Where(x => x.Id == userDepartment.PositionId).FirstOrDefault();
                    }
                }
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return userDepartments;
        }
        internal ObservableCollection<GroupDepartment> LoadGroupDepartments()
        {
            ObservableCollection<GroupDepartment> groupDepartments = new ObservableCollection<GroupDepartment>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                groupDepartments = _Client.LoadGroupDepartments().ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return groupDepartments;
        }
        internal void ChangePositionOfUserDepartment(int userDepartmentId, int positionId)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.ChangePositionOfUserDepartment(userDepartmentId, positionId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }

        internal void ChangePriorityProcessingOfUserDepartment(int userDepartmentId, int priorityProcessing)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.ChangePriorityProcessingOfUserDepartment(userDepartmentId, priorityProcessing);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal void LockDepartment(int departmentId, bool islock)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.LockDepartment(departmentId, islock);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal void NewDepartment(string departmentCode, string departmentName, int groupDepartmentId, int positionManagerId1, int positionManagerId2)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.NewDepartment(departmentCode, departmentName, groupDepartmentId, positionManagerId1, positionManagerId2);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal void DeleteDepartment(int departmentId)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.DeleteDepartment(departmentId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal ObservableCollection<Modul> LoadModuls()
        {
            ObservableCollection<Modul> moduls = new ObservableCollection<Modul>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                moduls = _Client.LoadModuls().ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return moduls;
        }
        internal void NewModul(string code, string description)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.NewModul(code, description);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal void DeleteModul(int modulId)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.DeleteModul(modulId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal void NewPermission(string code, string description, int modulId)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.NewPermission(code, description, modulId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal void DeletePermission(int permissionId)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.DeletePermission(permissionId);
                _Client.Close();
                MessageBox.Show("Xóa thành công");
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal ObservableCollection<Permission> LoadPermissions()
        {
            ObservableCollection<Permission> permissions = new ObservableCollection<Permission>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                permissions = _Client.LoadPermissions().ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return permissions;
        }
        internal ObservableCollection<Permission> GetPermissionsOfModul(int modulId)
        {
            ObservableCollection<Permission> permissions = new ObservableCollection<Permission>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                permissions = _Client.GetPermissionsOfModul(modulId).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return permissions;
        }
        internal ObservableCollection<RolePermissionD> LoadRolePermissionDs()
        {
            ObservableCollection<RolePermissionD> rolePermissionDs = new ObservableCollection<RolePermissionD>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                rolePermissionDs = _Client.LoadRolePermissionDs().ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return rolePermissionDs;
        }
        internal ObservableCollection<ChatAppServiceReference.Standard> LoadStandards()
        {
            ObservableCollection<ChatAppServiceReference.Standard> kq = new ObservableCollection<ChatAppServiceReference.Standard>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                kq = _Client.LoadStandards().ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal int NewProduct(Product product)
        {
            int productId = 0;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                productId = _Client.NewProduct(product);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return productId;
        }
        internal ObservableCollection<ProductTypeNew> LoadProducTypeNews()
        {
            ObservableCollection<ProductTypeNew> kq = new ObservableCollection<ProductTypeNew>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                kq = _Client.LoadProducTypeNews().ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal TransformerInfo GetTransformerInfo(int productId)
        {
            TransformerInfo kq = new TransformerInfo();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                kq = _Client.GetTransformerInfo(productId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal void UploadApprovalDocumentProduct(byte[] content, ApprovalDocumentProduct[] approvalDocumentProducts)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.UploadApprovalDocumentProduct(content, approvalDocumentProducts);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal ObservableCollection<ApprovalDocumentProduct> GetApprovalDocumentProducts(int productId, bool isExpired)
        {
            ObservableCollection<ApprovalDocumentProduct> kq = new ObservableCollection<ApprovalDocumentProduct>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                kq = _Client.GetApprovalDocumentProducts(productId, isExpired).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal int SetExpiredApprovalDocumentProduct(int approvalDocumentProductId)
        {
            int kq = 0;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                kq = _Client.SetExpiredApprovalDocumentProduct(approvalDocumentProductId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal void RecordLogin(LoginManager loginManager)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.RecordLogin(loginManager);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal int RestoreApprovalDocumentProduct(int approvalDocumentProductId)
        {
            int kq = 0;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                kq = _Client.RestoreApprovalDocumentProduct(approvalDocumentProductId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal byte[] DownloadApprovalDocumentProduct(int approvalDocumentProductId)
        {
            byte[] kq = null;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                kq = _Client.DownloadApprovalDocumentProduct(approvalDocumentProductId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal int GetLastApprovalNumber(int productId)
        {
            int kq = 0;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                kq = _Client.GetLastApprovalNumber(productId);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal int UpdateApprovalDocumentProduct(ApprovalDocumentProduct approvalDocumentProduct)
        {
            int kq = 0;
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                kq = _Client.UpdateApprovalDocumentProduct(approvalDocumentProduct);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return kq;
        }
        internal void AddApprovalDocToOtherProduct(int[] approvalDocumentDocIds, int[] productIds)
        {          
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.AddApprovalDocToOtherProduct(approvalDocumentDocIds, productIds);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal void SetSignatureDocumentStatus(int signatureDocumentId, SignatureDocumentStatus signatureDocumentStatus)
        {
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                _Client.SetSignatureDocumentStatus(signatureDocumentId, signatureDocumentStatus);
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
        }
        internal ObservableCollection<Log> GetLogs(DateTime from, DateTime dateTime_0, LogType type, int filterUserId)
        {
            ObservableCollection<Log> logs = new ObservableCollection<Log>();
            try
            {
                _Client = ServiceHelper.NewMessageServiceClient();
                _Client.Open();
                logs = _Client.GetLogs(from, dateTime_0, type, filterUserId).ToObservableCollection();
                _Client.Close();
            }
            catch (Exception ex)
            {
                _Client.Abort();
                MessageBox.Show(ex.Message);
            }
            return logs;
        }

    }
}

