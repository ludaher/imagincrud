using EnterpriseDT.Net.Ftp;
using ImaginCrud.Entities;
using ImaginCrud.Logic;
using ImaginCrud.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImaginCrud.Services.Main.Monitors
{
    public class FtpMonitor : MonitorBase
    {
        private static string FTP_SERVER = "FORMS_FTP_SERVER";
        private static string FTP_PATH = "FORMS_FTP_PATH";
        private static string FTP_USER = "FORMS_FTP_USER";
        private static string FTP_PASSWORD = "FORMS_FTP_PASSWORD";
        private static string PROCESSES_LOCAL_PATH = "PROCESSES_LOCAL_PATH";

        private string _ftpServer;
        private string _ftpPath;
        private string _ftpUser;
        private string _ftpPassword;
        private string _localPath;

        public FtpMonitor()
        {
            _ftpServer = ConfigurationManager.AppSettings[FTP_SERVER];
            _ftpPath = ConfigurationManager.AppSettings[FTP_PATH];
            _ftpUser = ConfigurationManager.AppSettings[FTP_USER];
            _ftpPassword = ConfigurationManager.AppSettings[FTP_PASSWORD];
            _localPath = ConfigurationManager.AppSettings[PROCESSES_LOCAL_PATH];
        }

        protected override void Run(object obj)
        {
            var state = (StateObjClass)obj;
            try
            {
                AppLogger.Logger.Info("Inicio de monitoreo");
                if (state.IsRunning) return;
                state.IsRunning = true;
                var logic = new FormLogic();
                var activeForms = logic.FindWithParameters(null, null, true);
                ///Conexión con el FTP
                using (FTPConnection ftpConnection = new FTPConnection())
                {
                    ftpConnection.ServerAddress = _ftpServer;
                    ftpConnection.UserName = _ftpUser;
                    ftpConnection.Password = _ftpPassword;
                    ftpConnection.Connect();
                    if (string.IsNullOrWhiteSpace(_ftpPath) == false)
                    {
                        if (!ftpConnection.DirectoryExists(_ftpPath))
                        {
                            ftpConnection.CreateDirectory(_ftpPath);
                        }
                        ftpConnection.ChangeWorkingDirectory(_ftpPath);
                    }
                    ///Recorre formularios activos
                    foreach (var form in activeForms)
                    {
                        var processFolder = Path.Combine(_localPath, form.FormId.ToString());
                        if (Directory.Exists(processFolder) == false)
                            Directory.CreateDirectory(processFolder);

                        if (!ftpConnection.DirectoryExists(form.FormId.ToString()))
                        {
                            ftpConnection.CreateDirectory(form.FormId.ToString());
                            continue;
                        }
                        ftpConnection.ChangeWorkingDirectory(form.FormId.ToString());
                        var items = ftpConnection.GetFileInfos().ToList();
                        if (items.Any())
                        {
                            form.ProductStatus = (int)ProductStatus.Received;
                            logic.Update(form);
                            _SaveFtpFolders(items, ftpConnection, form);
                        }

                        ftpConnection.ChangeWorkingDirectoryUp();
                    }
                    ftpConnection.Close();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Logger.Error(ex.ToString());
            }
            state.IsRunning = false;

        }

        private void _sendFileToRepository(byte[] file, string sourceFileName, string destinationPath)
        {
            var extension = Path.GetExtension(sourceFileName);
            switch (extension.ToLower())
            {
                case ".tiff":
                case ".tif":
                    PdfConverter.ConvertTiffToPdf(file, destinationPath);
                    break;
                default:
                    if (File.Exists(destinationPath))
                        File.Delete(destinationPath);
                    File.WriteAllBytes(destinationPath, file);
                    break;
            }

        }

        private void _SaveFtpFolders(List<FTPFile> yearFolders, FTPConnection ftpConnection, Form form)
        {
            ///Recorre archivos dentro del folder del formulario
            foreach (var yearFolder in yearFolders)
            {
                int year = 0;
                int month = 0;
                int day = 0;
                DateTime productionDate;
                if (yearFolder.Dir == false)
                    continue;
                try
                {
                    year = Convert.ToInt32(yearFolder.Name);
                    ftpConnection.ChangeWorkingDirectory(yearFolder.Name);
                    var monthFolders = ftpConnection.GetFileInfos().ToList();
                    foreach (var monthFolder in monthFolders)
                    {
                        if (monthFolder.Dir == false)
                            continue;
                        month = Convert.ToInt32(monthFolder.Name);
                        ftpConnection.ChangeWorkingDirectory(monthFolder.Name);
                        try
                        {
                            var dayFolders = ftpConnection.GetFileInfos().ToList();
                            foreach (var dayFolder in dayFolders)
                            {
                                if (dayFolder.Dir == false)
                                    continue;
                                day = Convert.ToInt32(dayFolder.Name);
                                ftpConnection.ChangeWorkingDirectory(dayFolder.Name);
                                try
                                {
                                    var items = ftpConnection.GetFileInfos().ToList();
                                    productionDate = new DateTime(year, month, day);
                                    _SaveFiles(items, ftpConnection, form, productionDate);
                                }
                                catch (Exception ex)
                                {
                                    AppLogger.Logger.Error($"Error en folder {ftpConnection.ServerDirectory}: {ex.ToString()}");
                                }
                                ftpConnection.ChangeWorkingDirectoryUp();
                            }

                        }
                        catch (Exception ex)
                        {
                            AppLogger.Logger.Error($"Error en folder {ftpConnection.ServerDirectory}: {ex.ToString()}");
                        }
                        ftpConnection.ChangeWorkingDirectoryUp();
                    }
                }
                catch (Exception ex)
                {
                    AppLogger.Logger.Error($"Error en folder {ftpConnection.ServerDirectory}: {ex.ToString()}");
                }
                ftpConnection.ChangeWorkingDirectoryUp();
            }
        }

        public void _SaveFiles(List<FTPFile> items, FTPConnection ftpConnection, Form form, DateTime productionDate)
        {

            foreach (var item in items)
            {
                if (item.Dir == true)
                    continue;
                var typingLogic = new TypingProcessLogic();
                var ftpFilePath = Path.Combine(ftpConnection.ServerDirectory, item.Name);
                var filePath = Path.Combine(_localPath, form.FormId.ToString(), string.Format("{0}.pdf", Path.GetFileNameWithoutExtension(item.Name)));
                try
                {
                    ///Pasa el archivo del ftp al sitio web
                    ///
                    var file = ftpConnection.DownloadByteArray(ftpFilePath);
                    _sendFileToRepository(file, item.Name, filePath);
                    ///Crea el registro del proceso
                    typingLogic.Insert(new TypingProcess()
                    {
                        FormId = form.FormId,
                        Observations = string.Format("Cargado el {0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")),
                        TypingProcessId = Path.GetFileNameWithoutExtension(item.Name),
                        TypingStatus = (int)ProcessStatus.Pending,
                        Priority = 5,
                        ProductionDate = productionDate
                    });
                    ///Borra el archivo
                    ftpConnection.DeleteFile(ftpFilePath);
                }
                catch (Exception ex)
                {
                    ///TODO: Registrar en el log
                    AppLogger.Logger.ErrorFormat("Archivo {0} con error: {1}", item.Name, ex.ToString());
                    File.Delete(filePath);
                }
            }
        }

    }
}
