﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using XMLMerger.Commands;
using XMLMerger.Models;
using System.Configuration;
using System.Runtime.Remoting.Contexts;

namespace XMLMerger.ViewModels
{
    public class MergerViewModel : BaseViewModel
    {

        public ObservableCollection<DB> FromDatabases { get; set; }
        public ObservableCollection<Site> FromSites { get; set; }
        public ObservableCollection<Project> FromProjects { get; set; }
        public ObservableCollection<XMLFile> FromXMLFiles { get; set; }

        public ObservableCollection<DB> ToDatabases { get; set; }
        public ObservableCollection<Site> ToSites { get; set; }
        public ObservableCollection<Project> ToProjects { get; set; }
        public ObservableCollection<XmlTags> XMLStructure { get; set; }
        public ObservableCollection<string> Operations { get; set; }
        public ObservableCollection<Project> ToProjectsCollection { get; set; }
        public ObservableCollection<XmlTags> XmlStructureCollection { get; set; }
        public ObservableCollection<XMLFile> RestoreCollection { get; set; }

        private DB selectedFromDB;
        public DB SelectedFromDB
        {
            get { return selectedFromDB; }
            set
            {
                selectedFromDB = value;
                OnPropertyChanged(nameof(SelectedFromDB));
                LoadFromSites();
            }
        }

        private Site selectedFromSite;
        public Site SelectedFromSite
        {
            get { return selectedFromSite; }
            set
            {
                selectedFromSite = value;
                OnPropertyChanged(nameof(SelectedFromSite));
                LoadFromProjects();
            }
        }

        private Project selectedFromProject;
        public Project SelectedFromProject
        {
            get { return selectedFromProject; }
            set
            {
                selectedFromProject = value;
                OnPropertyChanged(nameof(SelectedFromProject));
                LoadXMLFiles();
            }
        }

        private XMLFile selectedXMLFile;
        public XMLFile SelectedXMLFile
        {
            get
            {
                return selectedXMLFile;
            }
            set
            {

                selectedXMLFile = value;
                OnPropertyChanged(nameof(SelectedXMLFile));
                ValidateXML();
                //LoadXmlStructure();
                //LoadXMLHierarchy();
            }
        }


        //private string selectedXMLStructure;
        //public string SelectedXMLStructure
        //{
        //    get { return selectedXMLStructure; }
        //    set
        //    {
        //        selectedXMLStructure = value;
        //        OnPropertyChanged(nameof(SelectedXMLStructure));
        //    }
        //}

        private string selectedOperation;
        public string SelectedOperation
        {
            get { return selectedOperation; }
            set
            {
                selectedOperation = value;
                OnPropertyChanged(nameof(SelectedOperation));
                SelectedFromDB = null;
                SelectedToDB = null;
                EnteredAttributeValue = null;
                ToProjectsCollection.Clear();
            }
        }


        private DB selectedToDB;
        public DB SelectedToDB
        {
            get { return selectedToDB; }
            set
            {
                selectedToDB = value;
                OnPropertyChanged(nameof(SelectedToDB));
                LoadToSites();
            }
        }

        private Site selectedToSite;
        public Site SelectedToSite
        {
            get { return selectedToSite; }
            set
            {
                selectedToSite = value;
                OnPropertyChanged(nameof(SelectedToSite));
                LoadToProjects();
            }
        }

        private XMLFile selectedRestoreFile;
        public XMLFile SelectedRestoreFile
        {
            get { return selectedRestoreFile; }
            set
            {
                selectedRestoreFile = value;
                OnPropertyChanged(nameof(SelectedRestoreFile));
                ValidateXML();
            }
        }

        private string enteredAttributeValue;
        public string EnteredAttributeValue
        {
            get { return enteredAttributeValue; }
            set
            {
                enteredAttributeValue = value;
                OnPropertyChanged(nameof(EnteredAttributeValue));
            }
        }

        public RelayCommand ApplyChangesCommand { get; set; }
        public RelayCommand OnCheckedToProjectCommand { get; }
        public RelayCommand OnUncheckedToProjectCommand { get; }
       
        public RelayCommand OnCheckedXmlStructureCommand { get; }
        public RelayCommand OnUncheckedXmlStructureCommand { get; }

        public MergerViewModel()
        {
            FromDatabases = new ObservableCollection<DB>();
            FromSites = new ObservableCollection<Site>();
            FromProjects = new ObservableCollection<Project>();
            FromXMLFiles = new ObservableCollection<XMLFile>();
            XMLStructure = new ObservableCollection<XmlTags>();

            ToDatabases = new ObservableCollection<DB>();
            ToSites = new ObservableCollection<Site>();
            ToProjects = new ObservableCollection<Project>();

            Operations = new ObservableCollection<string>();

            ToProjectsCollection = new ObservableCollection<Project>();
            XmlStructureCollection = new ObservableCollection<XmlTags>();

            ApplyChangesCommand = new RelayCommand(e => ApplyChanges(), e => IsApplyChanges());
            
            RestoreCollection = new ObservableCollection<XMLFile>();

            OnCheckedToProjectCommand = new RelayCommand(m => AddToProjects(), m => IsAddToProjects());
            OnUncheckedToProjectCommand = new RelayCommand(m => RemoveToProjects(), m => IsRemoveToProjects()); 

            OnCheckedXmlStructureCommand = new RelayCommand(m => AddXmlStructure(), m => IsAddXmlStructure());
            OnUncheckedXmlStructureCommand = new RelayCommand(m => RemoveXmlStructure(), m => IsRemoveXmlStructure());

            string mergerPath = "C:/XMLMerger";

            foreach (var directory in Directory.GetDirectories(mergerPath))
            {
                FromDatabases.Add(new DB { Name = Path.GetFileName(directory) });
                ToDatabases.Add(new DB { Name = Path.GetFileName(directory) });
            }
            Operations.Add("Replace");
            Operations.Add("Add");
            Operations.Add("Update");
            Operations.Add("Restore");
            Operations.Add("Remove"); 
            Operations.Add("Add with specific id");
            Operations.Add("Update with specific id");
            Operations.Add("Remove with specific id");

        }

        private void ApplyChanges()
        {
            if (SelectedOperation == "Replace")
            {
                ReplaceXMLFile();
            }
            else if (SelectedOperation == "Add")
            {
                AddXMLFile();
            }
            else if (SelectedOperation == "Update")
            {
                UpdateXMLFile();
            }
            else if (SelectedOperation == "Restore")
            {
                RestoreXMLFile();
            }
            else if (SelectedOperation == "Remove")
            {
                RemoveXMLFile();
            }
            else if (SelectedOperation == "Add with specific id")
            {
                AddWithId();
                EnteredAttributeValue = null;

            }
            else if (SelectedOperation == "Update with specific id")
            {
                UpdateWithId();
                EnteredAttributeValue = null;

            }
            else if (SelectedOperation == "Remove with specific id")
            {
                RemoveWithId();
                EnteredAttributeValue = null;
            }
            ShowLogMessageBox();
            SelectedFromDB = null;
            SelectedToDB = null;
            SelectedOperation = null;
            SelectedRestoreFile = null;
            ToProjectsCollection.Clear();
        }

        private void ShowLogMessageBox()
        {
            foreach (var currentToProject in ToProjectsCollection)
            {
                string logFolderPath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name, currentToProject.Name, "Log_Folder");
                //logFolderPath = $"\"{logFolderPath}\"";
                string logFilePath = Path.Combine(logFolderPath, "log.txt");

                if (File.Exists(logFilePath))
                {
                    string logText = $"Operation '{SelectedOperation}' executed successfully!!!";

                    MessageBoxResult result = MessageBox.Show(logText, "Log Information", MessageBoxButton.OKCancel, MessageBoxImage.Information);

                    if(result == MessageBoxResult.OK)
                    {
                        Process.Start("notepad.exe", logFilePath);
                    }
                }
                else
                {
                    MessageBox.Show("Log file not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool IsApplyChanges()
        {
            if (SelectedOperation == "Replace" && XMLStructure.Count != 0 && ToProjectsCollection.Count != 0)
            {
                return true;
            }

            else if (SelectedOperation == "Remove" && XmlStructureCollection.Count == 1 && ToProjectsCollection.Count == 1)
            {
                return true;
            }

            else if((SelectedOperation == "Add" || SelectedOperation == "Update") && ToProjectsCollection.Count != 0 
                && !ToProjectsCollection.Any(x => x.Name.Equals(SelectedFromProject)) && XmlStructureCollection.Count != 0)
            {
                return true;
            }

            else if (SelectedOperation == "Restore" && SelectedRestoreFile != null)
            {
                return true;
            }
            else if ((SelectedOperation == "Add with specific id" || SelectedOperation == "Update with specific id")
                && ToProjectsCollection.Count == 1 && !ToProjectsCollection.Any(x => x.Name.Equals(SelectedFromProject)) && XmlStructureCollection.Count == 1
                && !String.IsNullOrEmpty(EnteredAttributeValue))
            {
                return true;
            }
            else if (SelectedOperation == "Remove with specific id" && XmlStructureCollection.Count == 1 
                && ToProjectsCollection.Count == 1 && !String.IsNullOrEmpty(EnteredAttributeValue))
            {
                return true;
            }

            return false;
        }

        private void ReplaceXMLFile()
        {
            foreach(var currentToProject in ToProjectsCollection)
            {
                string fromProjectPath = Path.Combine("C:/XMLMerger", SelectedFromDB.Name, "SPProj", SelectedFromSite.Name, SelectedFromProject.Name);
                string toProjectPath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name, currentToProject.Name);

                string fromFilePath = Path.Combine(fromProjectPath, SelectedXMLFile.FileName);
                string toFilePath = Path.Combine(toProjectPath, SelectedXMLFile.FileName);

                if (File.Exists(fromFilePath))
                {
                    if (!File.Exists(toFilePath))
                    {
                        File.Copy(fromFilePath, toFilePath);

                        LogWriter(fromFilePath, toFilePath, null, null, null , currentToProject.Name);
                    }
                    else
                    {
                        MessageBox.Show($"File  '{SelectedXMLFile.FileName}' already present at {currentToProject.Name}. Can't overwrite the file.", "File exists", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        private void AddXMLFile()
        {
            foreach (var currentToProject in ToProjectsCollection)
            {
                if (currentToProject.Name != null && SelectedXMLFile != null)
                {
                    string fromProjectPath = Path.Combine("C:/XMLMerger", SelectedFromDB.Name, "SPProj", SelectedFromSite.Name, SelectedFromProject.Name);
                    string toProjectPath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name, currentToProject.Name);

                    string fromFilePath = Path.Combine(fromProjectPath, SelectedXMLFile.FileName);
                    string toFilePath = Path.Combine(toProjectPath, SelectedXMLFile.FileName);

                    if (File.Exists(fromFilePath))
                    {
                        if (File.Exists(toFilePath))
                        {
                            try
                            {
                                XDocument fromXml = XDocument.Load(fromFilePath);
                                XDocument toXml = XDocument.Load(toFilePath);

                                if (XNode.DeepEquals(fromXml, toXml))
                                {
                                    MessageBox.Show($"File '{SelectedXMLFile.FileName}' at both '{SelectedFromProject.Name}' and '{currentToProject.Name}' are same. Nothing to add at destination file.");
                                }

                                else
                                {
                                    foreach (var currentXmlTag in XmlStructureCollection)
                                    {
                                        List<string> addedIds = new List<string>();
                                        bool isAdded = false;

                                        string[] structure = currentXmlTag.Name.Split('>').Select(e => e.Trim()).ToArray();
                                        int structureLength = structure.Length;
                                        string selectedStructure = structure.Length > 1 ? structure[structureLength - 2] : structure[0];

                                        var fromElements = fromXml.Descendants(selectedStructure);
                                        var toElements = toXml.Descendants(selectedStructure);


                                        foreach (var fromElement in fromElements)
                                        {
                                            foreach (var toElement in toElements)
                                            {
                                                if (toElement.Attribute("id")?.Value == fromElement.Attribute("id")?.Value)
                                                {
                                                    var addedElements = from element in fromElement.Descendants(structure[structureLength - 1])
                                                                        where !toElement.Descendants(structure[structureLength - 1])
                                                                        .Any(x => x.Attribute("id")?.Value == element.Attribute("id")?.Value)
                                                                        select element;

                                                    foreach (var addedElement in addedElements)
                                                    {
                                                        string addedId = addedElement.Attribute("id")?.Value;
                                                        if (!string.IsNullOrEmpty(addedId))
                                                        {
                                                            addedIds.Add(addedId);
                                                        }
                                                        toElement.Add(addedElement);
                                                        isAdded = true;
                                                    }

                                                }

                                            }

                                        }

                                        if (isAdded)
                                        {
                                            string bkFileName = CreateAndReturnBackupFileName(toProjectPath, toFilePath);
                                            toXml.Save(toFilePath);

                                            LogWriter(fromFilePath, toFilePath, bkFileName, addedIds, currentXmlTag.Name, currentToProject.Name);
                                        }
                                        else
                                        {
                                            MessageBox.Show($"There is nothing to add on file '{SelectedXMLFile.FileName}' in '{currentToProject.Name}'");
                                        }

                                    }   

                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                        else
                        {

                            File.Copy(fromFilePath, toFilePath);

                            LogWriter(fromFilePath, toFilePath, null, null, null, currentToProject.Name);
                        }

                    }
                }
            }
        }

        private void AddWithId()
        {
            string fromProjectPath = Path.Combine("C:/XMLMerger", SelectedFromDB.Name, "SPProj", SelectedFromSite.Name, SelectedFromProject.Name);
            string toProjectPath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name, ToProjectsCollection.First().Name);

            string fromFilePath = Path.Combine(fromProjectPath, SelectedXMLFile.FileName);
            string toFilePath = Path.Combine(toProjectPath, SelectedXMLFile.FileName);

            if (File.Exists(fromFilePath))
            {
                if (File.Exists(toFilePath))
                {
                    try
                    {
                        XDocument fromXml = XDocument.Load(fromFilePath);
                        XDocument toXml = XDocument.Load(toFilePath);

                        if (XNode.DeepEquals(fromXml, toXml))
                        {
                            MessageBox.Show($"File '{SelectedXMLFile.FileName}' at both '{SelectedFromProject.Name}' and '{ToProjectsCollection.First().Name}' are same. Nothing to add at destination file.");
                        }

                        else
                        {
                            
                            List<string> addedIds = new List<string>();
                            bool isAdded = false;

                            string[] structure = XmlStructureCollection.First().Name.Split('>').Select(e => e.Trim()).ToArray();
                            int structureLength = structure.Length;
                            string selectedStructure = structure.Length > 1 ? structure[structureLength - 2] : structure[0];

                            string[] ids = EnteredAttributeValue.Split(',').Select(e => e.Trim()).ToArray();

                            var fromElements = fromXml.Descendants(selectedStructure);
                            var toElements = toXml.Descendants(selectedStructure);

                            foreach (var fromElement in fromElements)
                            {
                                foreach (var toElement in toElements)
                                {
                                    if (toElement.Attribute("id")?.Value == fromElement.Attribute("id")?.Value)
                                    {
                                        var addedElements = from element in fromElement.Descendants(structure[structureLength - 1])
                                                            where ids.Any(x => x == element.Attribute("id")?.Value)
                                                            select element;

                                        foreach (var addedElement in addedElements)
                                        {
                                            string addedId = addedElement.Attribute("id")?.Value;
                                            if (!string.IsNullOrEmpty(addedId))
                                            {
                                                addedIds.Add(addedId);
                                            }
                                            toElement.Add(addedElement);
                                            isAdded = true;
                                        }

                                    }

                                }

                            }

                            if (isAdded)
                            {
                                string bkFileName = CreateAndReturnBackupFileName(toProjectPath, toFilePath);
                                toXml.Save(toFilePath);

                                LogWriter(fromFilePath, toFilePath, bkFileName, addedIds, XmlStructureCollection.First().Name, ToProjectsCollection.First().Name);
                            }
                            else
                            {
                                MessageBox.Show($"There is nothing to add on file '{SelectedXMLFile.FileName}' in '{ToProjectsCollection.First().Name}'");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void UpdateXMLFile()
        {
            foreach (var currentToProject in ToProjectsCollection)
            {
                if (currentToProject.Name != null && SelectedXMLFile != null)
                {
                    string fromProjectPath = Path.Combine("C:/XMLMerger", SelectedFromDB.Name, "SPProj", SelectedFromSite.Name, SelectedFromProject.Name);
                    string toProjectPath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name, currentToProject.Name);

                    string fromFilePath = Path.Combine(fromProjectPath, SelectedXMLFile.FileName);
                    string toFilePath = Path.Combine(toProjectPath, SelectedXMLFile.FileName);

                    if (File.Exists(fromFilePath))
                    {
                        if (File.Exists(toFilePath))
                        {
                            try
                            {
                                XDocument fromXml = XDocument.Load(fromFilePath);
                                XDocument toXml = XDocument.Load(toFilePath);
                                if (XNode.DeepEquals(fromXml, toXml))
                                {
                                    MessageBox.Show($"File '{SelectedXMLFile.FileName}' at both '{SelectedFromProject.Name}' and '{currentToProject.Name}' are same. Nothing to add at destination file.");
                                }

                                else
                                {
                                    foreach (var currentXmlTag in XmlStructureCollection)
                                    {
                                        List<string> updatedIds = new List<string>();
                                        bool isUpdated = false;

                                        string[] structure = currentXmlTag.Name.Split('>').Select(e => e.Trim()).ToArray();
                                        int structureLength = structure.Length;
                                        string selectedStructure = structure.Length > 1 ? structure[structureLength - 2] : structure[0];

                                        var fromElements = fromXml.Descendants(selectedStructure);
                                        var toElements = toXml.Descendants(selectedStructure);

                                        foreach (var fromElement in fromElements)
                                        {
                                            foreach (var toElement in toElements)
                                            {
                                                if (toElement.Attribute("id")?.Value == fromElement.Attribute("id")?.Value)
                                                {
                                                    if (XNode.DeepEquals(fromElement, toElement))
                                                    {
                                                        break;
                                                    }
                                                    var curToElement = toElement.Descendants(structure[structureLength - 1]);
                                                    var updatedElements = from element in fromElement.Descendants(structure[structureLength - 1])
                                                                        where curToElement.Any(x => x.Attribute("id")?.Value == element.Attribute("id")?.Value)
                                                                        select element;

                                                    foreach (var updatedElement in updatedElements)
                                                    {
                                                        foreach (var element in curToElement)
                                                        {
                                                            if (updatedElement.Attribute("id")?.Value == element.Attribute("id")?.Value)
                                                            {
                                                                if (XNode.DeepEquals(updatedElement, element))
                                                                {
                                                                    break;
                                                                }
                                                                string updatedId = updatedElement.Attribute("id")?.Value;
                                                                if (!string.IsNullOrEmpty(updatedId))
                                                                {
                                                                    updatedIds.Add(updatedId);
                                                                }
                                                                element.ReplaceWith(updatedElement);
                                                                isUpdated = true;
                                                                break;
                                                            }
                                                        }
                                                        
                                                    }

                                                }

                                            }

                                        }

                                        if (isUpdated)
                                        {
                                            string bkFileName = CreateAndReturnBackupFileName(toProjectPath, toFilePath);
                                            toXml.Save(toFilePath);

                                            LogWriter(fromFilePath, toFilePath, bkFileName, updatedIds, currentXmlTag.Name, currentToProject.Name);
                                        }
                                        else
                                        {
                                            MessageBox.Show($"There is nothing to update on file '{SelectedXMLFile.FileName}' in '{currentToProject.Name}'");
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateWithId()
        {
            string fromProjectPath = Path.Combine("C:/XMLMerger", SelectedFromDB.Name, "SPProj", SelectedFromSite.Name, SelectedFromProject.Name);
            string toProjectPath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name, ToProjectsCollection.First().Name);

            string fromFilePath = Path.Combine(fromProjectPath, SelectedXMLFile.FileName);
            string toFilePath = Path.Combine(toProjectPath, SelectedXMLFile.FileName);

            if (File.Exists(fromFilePath))
            {
                if (File.Exists(toFilePath))
                {
                    try
                    {
                        XDocument fromXml = XDocument.Load(fromFilePath);
                        XDocument toXml = XDocument.Load(toFilePath);

                        if (XNode.DeepEquals(fromXml, toXml))
                        {
                            MessageBox.Show($"File '{SelectedXMLFile.FileName}' at both '{SelectedFromProject.Name}' and '{ToProjectsCollection.First().Name}' are same. Nothing to add at destination file.");
                        }

                        else
                        {

                            List<string> updatedIds = new List<string>();
                            bool isUpdated = false;

                            string[] structure = XmlStructureCollection.First().Name.Split('>').Select(e => e.Trim()).ToArray();
                            int structureLength = structure.Length;
                            string selectedStructure = structure.Length > 1 ? structure[structureLength - 2] : structure[0];

                            string[] ids = EnteredAttributeValue.Split(',').Select(e => e.Trim()).ToArray();

                            var fromElements = fromXml.Descendants(selectedStructure);
                            var toElements = toXml.Descendants(selectedStructure);

                            foreach (var fromElement in fromElements)
                            {
                                foreach (var toElement in toElements)
                                {
                                    if (toElement.Attribute("id")?.Value == fromElement.Attribute("id")?.Value)
                                    {
                                        if (XNode.DeepEquals(fromElement, toElement))
                                        {
                                            break;
                                        }
                                        var curToElement = from element in toElement.Descendants(structure[structureLength - 1])
                                                           where ids.Any(x => x == element.Attribute("id")?.Value)
                                                           select element;
                                        var updatedElements = from element in fromElement.Descendants(structure[structureLength - 1])
                                                              where curToElement.Any(x => x.Attribute("id")?.Value == element.Attribute("id")?.Value)
                                                              select element;

                                        foreach (var updatedElement in updatedElements)
                                        {
                                            foreach (var element in curToElement)
                                            {
                                                if (updatedElement.Attribute("id")?.Value == element.Attribute("id")?.Value)
                                                {
                                                    if (XNode.DeepEquals(updatedElement, element))
                                                    {
                                                        break;
                                                    }
                                                    string updatedId = updatedElement.Attribute("id")?.Value;
                                                    if (!string.IsNullOrEmpty(updatedId))
                                                    {
                                                        updatedIds.Add(updatedId);
                                                    }
                                                    element.ReplaceWith(updatedElement);
                                                    isUpdated = true;
                                                    break;
                                                }
                                            }

                                        }

                                    }

                                }

                            }

                            if (isUpdated)
                            {
                                string bkFileName = CreateAndReturnBackupFileName(toProjectPath, toFilePath);
                                toXml.Save(toFilePath);

                                LogWriter(fromFilePath, toFilePath, bkFileName, updatedIds, XmlStructureCollection.First().Name, ToProjectsCollection.First().Name);
                            }
                            else
                            {
                                MessageBox.Show($"There is nothing to update on file '{SelectedXMLFile.FileName}' in '{ToProjectsCollection.First().Name}'");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void RemoveXMLFile()
        {
            if (ToProjectsCollection.First().Name != null && SelectedRestoreFile != null)
            {
                string projectPath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name, ToProjectsCollection.First().Name);

                string filePath = Path.Combine(projectPath, SelectedRestoreFile.FileName);

                if (File.Exists(filePath))
                {
                    try
                    {
                        XDocument xml = XDocument.Load(filePath);

                        foreach (var currentXmlTag in XmlStructureCollection)
                        {
                            List<string> removedIds = new List<string>();
                            bool isRemoved = false;

                            string[] structure = currentXmlTag.Name.Split('>').Select(e => e.Trim()).ToArray();
                            int structureLength = structure.Length;
                            string selectedStructure = structure.Length > 1 ? structure[structureLength - 2] : structure[0];

                            var elements = xml.Descendants(selectedStructure);

                            foreach (var element in elements)
                            {
                                var removeElements = from ele in elements.Descendants(structure[structureLength - 1])
                                                    select ele;
                                if (removeElements != null)
                                {
                                    foreach (var removeElement in removeElements.ToList())
                                    {
                                        var temp = removeElement;
                                        string removedId = $"{removeElement.Attribute("id")?.Value}";
                                        if (!string.IsNullOrEmpty(removedId))
                                        {
                                            removedIds.Add(removedId);
                                        }
                                        temp.Remove();
                                        isRemoved = true;
                                    }
                                }
                            }


                            

                            if (isRemoved)
                            {
                                string bkFileName = CreateAndReturnBackupFileName(projectPath, filePath);
                                xml.Save(filePath);

                                RemoveLogWriter(filePath, bkFileName, currentXmlTag.Name, removedIds);
                            }
                            else
                            {
                                MessageBox.Show($"There is nothing to remove from file '{SelectedXMLFile.FileName}' in '{ToProjectsCollection.First().Name}'");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            
        }

        private void RemoveWithId()
        {
            if (ToProjectsCollection.First().Name != null && SelectedRestoreFile != null)
            {
                string projectPath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name, ToProjectsCollection.First().Name);

                string filePath = Path.Combine(projectPath, SelectedRestoreFile.FileName);

                if (File.Exists(filePath))
                {
                    try
                    {
                        XDocument xml = XDocument.Load(filePath);

                        foreach (var currentXmlTag in XmlStructureCollection)
                        {
                            List<string> removedIds = new List<string>();
                            bool isRemoved = false;

                            string[] structure = currentXmlTag.Name.Split('>').Select(e => e.Trim()).ToArray();
                            int structureLength = structure.Length;
                            string selectedStructure = structure.Length > 1 ? structure[structureLength - 2] : structure[0];

                            string[] ids = EnteredAttributeValue.Split(',').Select(e => e.Trim()).ToArray();

                            var elements = xml.Descendants(selectedStructure);

                            foreach (var element in elements)
                            {
                                var removeElements = from ele in elements.Descendants(structure[structureLength - 1])
                                                     where ids.Any(x => x == ele.Attribute("id").Value)
                                                     select ele;
                                if (removeElements != null)
                                {
                                    foreach (var removeElement in removeElements.ToList())
                                    {
                                        var temp = removeElement;
                                        string removedId = $"{removeElement.Attribute("id")?.Value}";
                                        if (!string.IsNullOrEmpty(removedId))
                                        {
                                            removedIds.Add(removedId);
                                        }
                                        temp.Remove();
                                        isRemoved = true;
                                    }
                                }
                            }




                            if (isRemoved)
                            {
                                string bkFileName = CreateAndReturnBackupFileName(projectPath, filePath);
                                xml.Save(filePath);

                                RemoveLogWriter(filePath, bkFileName, currentXmlTag.Name, removedIds);
                            }
                            else
                            {
                                MessageBox.Show($"There is nothing to remove from file '{SelectedXMLFile.FileName}' in '{ToProjectsCollection.First().Name}'");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void RestoreXMLFile()
        {
            foreach (var currentToProject in ToProjectsCollection)
            {
                string projectPath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name, currentToProject.Name);
                    
                if (Directory.Exists(Path.Combine(projectPath, "Log Backup")))
                {
                    string BackupFileIncludeValue = ConfigurationManager.AppSettings["BackupFileInclude"];
                    List<FileInfo> files = new List<FileInfo>();

                    foreach (var file in Directory.GetFiles(Path.Combine(projectPath, "Log Backup"), $"{Path.GetFileNameWithoutExtension(SelectedRestoreFile.FileName)}{BackupFileIncludeValue}*.xml"))
                    {
                        files.Add(new FileInfo(file));
                    }

                    if (files.Count > 0)
                    {
                        files = files.OrderByDescending(x => x.LastWriteTime).ToList();

                        MessageBoxResult result = MessageBox.Show("Updates may get lost from file. Press 'OK' to restore.", 
                            "Restore File", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                        if (result == MessageBoxResult.OK)
                        {
                            RestoreLogWriter(files[0].FullName, Path.Combine(projectPath, SelectedRestoreFile.FileName), SelectedRestoreFile.FileName, currentToProject.Name);
                            File.Delete(Path.Combine(projectPath, SelectedRestoreFile.FileName));

                            File.Move(files[0].FullName, Path.Combine(projectPath, SelectedRestoreFile.FileName));

                            //File.Delete(files[0].FullName);
                            files.Clear();

                        }

                    }
                    else
                    {
                        MessageBox.Show($"There are no backup file for '{SelectedRestoreFile}' in '{currentToProject.Name}'");
                    }


                }

            }
        }

        private void RestoreLogWriter(string sourceFilePath, string destinationFilePath, string fileName, string currentToProject)
        {
            string logFolderPath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name, currentToProject, "Log_Folder");
            string logFilePath;
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
                logFilePath = Path.Combine(logFolderPath, "log.txt");
                File.WriteAllText(logFilePath, "Log Recorded:\n\n");

            }
            else
            {
                logFilePath = Path.Combine(logFolderPath, "log.txt");
            }

            StringBuilder sbReportLogFile = new StringBuilder();

            sbReportLogFile.AppendLine("");
            sbReportLogFile.AppendLine($"Source File: {sourceFilePath}");
            sbReportLogFile.AppendLine($"Destination File: {destinationFilePath}");
            sbReportLogFile.AppendLine($"File: {fileName}");
            sbReportLogFile.AppendLine($"User: {Environment.UserName}");
            sbReportLogFile.AppendLine($"Date: {DateTime.Now}");
            sbReportLogFile.AppendLine("Operation: Restore");
            sbReportLogFile.AppendLine("Remark: Backup File is restored replacing original file.");
            File.AppendAllText(logFilePath, sbReportLogFile.ToString());

        }

        private void RemoveLogWriter(string filePath, string backupFile, string currentTag, List<string> removedIds = null)
        {
            string logFolderPath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name, ToProjectsCollection.First().Name, "Log_Folder");
            string logFilePath;
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
                logFilePath = Path.Combine(logFolderPath, "log.txt");
                File.WriteAllText(logFilePath, "Log Recorded:\n\n");

            }
            else
            {
                logFilePath = Path.Combine(logFolderPath, "log.txt");
            }

            StringBuilder sbReportLogFile = new StringBuilder();

            sbReportLogFile.AppendLine("");
            sbReportLogFile.AppendLine($"File: {filePath}");
            sbReportLogFile.AppendLine($"User: {Environment.UserName}");
            sbReportLogFile.AppendLine($"Date: {DateTime.Now}");
            sbReportLogFile.AppendLine($"Structure: {currentTag}");
            if (SelectedOperation == "Remove") sbReportLogFile.AppendLine("Operation: Remove");
            else sbReportLogFile.AppendLine("Operation: Remove with specific attribute.");
            if (backupFile == null)
            {
                sbReportLogFile.AppendLine($"Backup File: -");
            }
            else
            {
                sbReportLogFile.AppendLine($"Backup File: {backupFile}");
            }
            sbReportLogFile.AppendLine($"Remark: Removed Ids: {string.Join(", ", removedIds)}");

            File.AppendAllText(logFilePath, sbReportLogFile.ToString());

        }

        private void LogWriter(string fromFilePath, string toFilePath, string backupFile = null, List<string> addedIds = null, string currentTag = null, string currentToProject = null)
        {
            string logFolderPath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name, currentToProject, "Log_Folder");
            string logFilePath;
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
                logFilePath = Path.Combine(logFolderPath, "log.txt");
                File.WriteAllText(logFilePath, "Log Recorded:\n\n");

            }
            else
            {
                logFilePath = Path.Combine(logFolderPath, "log.txt");
            }
            currentTag = currentTag == null ? "...Blank..." : currentTag;
            StringBuilder sbReportLogFile = new StringBuilder();

            sbReportLogFile.AppendLine("");
            sbReportLogFile.AppendLine($"From File: {fromFilePath}");
            sbReportLogFile.AppendLine($"To File: {toFilePath}");
            sbReportLogFile.AppendLine($"User: {Environment.UserName}");
            sbReportLogFile.AppendLine($"Date: {DateTime.Now}");
            sbReportLogFile.AppendLine($"Structure: {currentTag}");

            switch (SelectedOperation)
            {
                case "Replace":
                    sbReportLogFile.AppendLine($"Operation: Replace");
                    sbReportLogFile.AppendLine($"Backup File: -");
                    sbReportLogFile.AppendLine($"Remark: New file is added to the project.");
                    break;
                case "Add":
                    sbReportLogFile.AppendLine($"Operation: Add");
                    if (backupFile == null)
                    {
                        sbReportLogFile.AppendLine($"Backup File: -");
                    }
                    else
                    {
                        sbReportLogFile.AppendLine($"Backup File: {backupFile}");
                    }
                    if (addedIds == null)
                    {
                        sbReportLogFile.AppendLine($"Remark: Full file added.");
                    }
                    else 
                    {
                        sbReportLogFile.AppendLine($"Remark: Added Ids: {string.Join(", ", addedIds)}");
                    }
                    break;
                case "Update":
                    sbReportLogFile.AppendLine($"Operation: Update");
                    if (backupFile == null)
                    {
                        sbReportLogFile.AppendLine($"Backup File: -");
                    }
                    else
                    {
                        sbReportLogFile.AppendLine($"Backup File: {backupFile}");
                    }
                    if (addedIds == null)
                    {
                        sbReportLogFile.AppendLine($"Remark: Full file added.");
                    }
                    else
                    {
                        sbReportLogFile.AppendLine($"Remark: Updated Ids: {string.Join(", ", addedIds)}");
                    }
                    break;
                case "Add with specific id":
                    sbReportLogFile.AppendLine($"Operation: Add with specific attribute");
                    if (backupFile == null)
                    {
                        sbReportLogFile.AppendLine($"Backup File: -");
                    }
                    else
                    {
                        sbReportLogFile.AppendLine($"Backup File: {backupFile}");
                    }
                    if (addedIds == null)
                    {
                        sbReportLogFile.AppendLine($"Remark: Full file added.");
                    }
                    else
                    {
                        sbReportLogFile.AppendLine($"Remark: Added Ids: {string.Join(", ", addedIds)}");
                    }
                    break;
                case "Update with specific id":
                    sbReportLogFile.AppendLine($"Operation: Update with specific attribute");
                    if (backupFile == null)
                    {
                        sbReportLogFile.AppendLine($"Backup File: -");
                    }
                    else
                    {
                        sbReportLogFile.AppendLine($"Backup File: {backupFile}");
                    }
                    if (addedIds == null)
                    {
                        sbReportLogFile.AppendLine($"Remark: Full file added.");
                    }
                    else
                    {
                        sbReportLogFile.AppendLine($"Remark: Updated Ids: {string.Join(", ", addedIds)}");
                    }
                    break;
            }


            //string logEntry = $"From File: {fromFilePath}\nTo File: {toFilePath}\nUser: {Environment.UserName}\nDate: {DateTime.Now}\n" +
            //$"Structure: {SelectedXMLStructure}\nOperation: {SelectedOperation}\nBackup File: Back Up\nRemark:\n\n";


            File.AppendAllText(logFilePath, sbReportLogFile.ToString());
        }

        private string CreateAndReturnBackupFileName(string projectPath, string filePath)
        {
            string BackupFileIncludeValue = ConfigurationManager.AppSettings["BackupFileInclude"];
            string backupPath = Path.Combine(projectPath, "Log Backup");
            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }
            string name = Path.GetFileNameWithoutExtension(filePath);
            string date = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            //int count = 0;

            string fileName = $"{name}{BackupFileIncludeValue}{date}.xml";
            if(!File.Exists(Path.Combine(backupPath, fileName)))
            {
                File.Copy(filePath, Path.Combine(backupPath, fileName));
            }
            
            DeleteBackupFile(backupPath, name, BackupFileIncludeValue);
            return fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="fileCount"></param>
        private void DeleteBackupFile(string path, string name, string FileInclude)
        {
            List<FileInfo> files = new List<FileInfo>();
            int DeleteBackupFileCountValue = Convert.ToInt32(ConfigurationManager.AppSettings["DeleteBackupFileCount"]);

            foreach (var file in Directory.GetFiles(path, $"{name}{FileInclude}*.xml"))
            {
                files.Add(new FileInfo(file));
            }

            if (files.Count > DeleteBackupFileCountValue)
            {
                files = files.OrderBy(x => x.CreationTime).ToList();
            }

            while (files.Count > DeleteBackupFileCountValue)
            {
                File.Delete(files[0].FullName);
                files.RemoveAt(0);
            }
        }


        
        private void LoadFromSites()
        {
            FromSites.Clear();
            if (SelectedFromDB != null)
            {
                string DBPath = Path.Combine("C:/XMLMerger", SelectedFromDB.Name, "SPProj");

                foreach (var directory in Directory.GetDirectories(DBPath))
                {
                    FromSites.Add(new Site { Name = Path.GetFileName(directory) });
                }
            }
        }

        private void LoadToSites()
        {
            ToSites.Clear();
            if (SelectedToDB != null)
            {
                string DBPath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj");

                foreach (var directory in Directory.GetDirectories(DBPath))
                {
                    ToSites.Add(new Site { Name = Path.GetFileName(directory) });
                }
            }
        }

        private void LoadFromProjects()
        {
            FromProjects.Clear();
            if (SelectedFromSite != null)
            {
                string sitePath = Path.Combine("C:/XMLMerger", SelectedFromDB.Name, "SPProj", SelectedFromSite.Name);

                foreach (var directory in Directory.GetDirectories(sitePath))
                {
                    FromProjects.Add(new Project { Name = Path.GetFileName(directory) });
                }
            }
        }

        private void LoadXMLFiles()
        {
            FromXMLFiles.Clear();
            if (SelectedFromProject != null)
            {
                string projectPath = Path.Combine("C:/XMLMerger", SelectedFromDB.Name, "SPProj", SelectedFromSite.Name, SelectedFromProject.Name);

                foreach (var file in Directory.GetFiles(projectPath, "*.xml"))
                {
                    FromXMLFiles.Add(new XMLFile { FileName = Path.GetFileName(file) });
                }
            }
        }

        private void ValidateXML()
        {
            bool isRemove = (SelectedOperation == "Remove" || selectedOperation == "Remove with specific id");
            if (SelectedXMLFile == null && !isRemove)
            {
                XMLStructure.Clear();
                XmlStructureCollection.Clear();
                return;
            }
            else if (isRemove && SelectedRestoreFile == null)
            {
                XMLStructure.Clear();
                XmlStructureCollection.Clear();
                return;
            }
            try
            {
                if (isRemove)
                {
                    XElement xmlFile = XElement.Load(Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name, ToProjectsCollection.First().Name, SelectedRestoreFile.FileName));
                    LoadXmlStructure();
                }
                else
                {
                    XElement xmlFile = XElement.Load(Path.Combine("C:/XMLMerger", SelectedFromDB.Name, "SPProj", SelectedFromSite.Name, SelectedFromProject.Name, SelectedXMLFile.FileName));
                    LoadXmlStructure();
                }
                LoadXmlStructure();
            }
            catch (Exception ex)
            {

                XMLStructure.Clear();
                MessageBox.Show(ex.Message);
            }


        }

        private void LoadXmlStructure()
        {
            XMLStructure.Clear();
            bool isRemove = (SelectedOperation == "Remove" || selectedOperation == "Remove with specific id");
            if (SelectedXMLFile != null  && !isRemove)
            {
                XMLStructure.Add(new XmlTags { Name = "...Blank..."});

                string filePath = Path.Combine("C:/XMLMerger", SelectedFromDB.Name, "SPProj", SelectedFromSite.Name, SelectedFromProject.Name, SelectedXMLFile.FileName);

                if (File.Exists(filePath))
                {
                    try
                    {
                        XDocument xmlDoc = XDocument.Load(filePath);

                        TraverseXmlStructure(xmlDoc.Root, "");
                        XmlStructureCollection.Clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else if (SelectedRestoreFile != null && isRemove)
            {
                string filePath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name, ToProjectsCollection.First().Name, SelectedRestoreFile.FileName);

                if (File.Exists(filePath))
                {
                    try
                    {
                        XDocument xmlDoc = XDocument.Load(filePath);

                        TraverseXmlStructure(xmlDoc.Root, "");
                        XmlStructureCollection.Clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void TraverseXmlStructure(XElement element, string currentPath)
        {
            string elementName = element.Name.LocalName;
            string tagPath = currentPath == "" ? elementName : $"{currentPath} > {elementName}";

            if (!XMLStructure.Any(x => x.Name.Equals(tagPath))) XMLStructure.Add(new XmlTags { Name = tagPath });

            foreach (var childElement in element.Elements())
            {
                TraverseXmlStructure(childElement, tagPath);
            }
        }

        private void LoadToProjects()
        {
            ToProjects.Clear();
            ToProjectsCollection.Clear();
            RestoreCollection.Clear();
            if (SelectedOperation == "Remove" && SelectedOperation == "Remove with specific id")
            {
                SelectedRestoreFile = null;
            }
            if (SelectedToSite != null)
            {
                string sitePath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name);

                foreach (var directory in Directory.GetDirectories(sitePath))
                {
                    ToProjects.Add(new Project { Name = Path.GetFileName(directory) });
                }
            }
        }

        public void AddToProjects()
        {
            foreach (var project in ToProjects)
            {
                if (project.IsChecked && !ToProjectsCollection.Contains(project))
                {
                    ToProjectsCollection.Add(project);
                    UpdateRestoreCollectionOnAddProject(project);
                }
                
            }
            if (ToProjectsCollection.Count > 1 && 
                (SelectedOperation == "Remove" || SelectedOperation == "Remove with specific id" || 
                SelectedOperation == "Add with specific id" || SelectedOperation == "Update with specific id"))
            {
                MessageBox.Show($"{SelectedOperation} operation can't be performed on more than one project.\nPlease select only one project.");
            }
        }


        private bool IsAddToProjects()
        {
            return true;
        }

        private void RemoveToProjects()
        {
            List<Project> tempChecked = ToProjectsCollection.ToList();
            foreach (var project in tempChecked)
            {
                if (!project.IsChecked)
                {
                    ToProjectsCollection.Remove(project);
                    UpdateRestoreCollectionOnRemoveProject();
                }
            }
            if (ToProjectsCollection.Count > 1 &&
                (SelectedOperation == "Remove" || SelectedOperation == "Remove with specific id" ||
                SelectedOperation == "Add with specific id" || SelectedOperation == "Update with specific id"))
            {
                MessageBox.Show($"{SelectedOperation} operation can't be performed on more than one project.\nPlease select only one project.");
            }
            tempChecked.Clear();
        }

        private bool IsRemoveToProjects()
        {
            return true;
        }

        public void AddXmlStructure()
        {
            foreach (var xmlTag in XMLStructure)
            {
                if (xmlTag.IsChecked && !XmlStructureCollection.Contains(xmlTag))
                {
                    XmlStructureCollection.Add(xmlTag);

                }
            }
            if (XmlStructureCollection.Count > 1 &&
                (SelectedOperation == "Remove" || SelectedOperation == "Remove with specific id" ||
                SelectedOperation == "Add with specific id" || SelectedOperation == "Update with specific id"))
            {
                MessageBox.Show($"{SelectedOperation} operation can't be performed on more than one structure.\nPlease select only one structure.");
            }

        }

        private bool IsAddXmlStructure()
        {
            return true;
        }

        private void RemoveXmlStructure()
        {
            List<XmlTags> tempChecked = XmlStructureCollection.ToList();
            foreach (var xmlTag in tempChecked)
            {
                if (!xmlTag.IsChecked)
                {
                    XmlStructureCollection.Remove(xmlTag);
                }
            }
            tempChecked.Clear();
            if (XmlStructureCollection.Count > 1 &&
                (SelectedOperation == "Remove" || SelectedOperation == "Remove with specific id" ||
                SelectedOperation == "Add with specific id" || SelectedOperation == "Update with specific id"))
            {
                MessageBox.Show($"{SelectedOperation} operation can't be performed on more than one structure.\nPlease select only one structure.");
            }
        }

        private bool IsRemoveXmlStructure()
        {
            return true;
        }

        private void UpdateRestoreCollectionOnAddProject(Project project)
        {
            string projectPath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name, project.Name);

            if (ToProjectsCollection.Count == 0)
            {
                RestoreCollection.Clear();
            }
            else if (ToProjectsCollection.Count == 1)
            {
                foreach (var file in Directory.GetFiles(projectPath, "*.xml"))
                {
                    if (!RestoreCollection.Any(x => x.FileName.Equals(Path.GetFileName(file))))
                    {
                        RestoreCollection.Add(new XMLFile { FileName = Path.GetFileName(file) });
                    }
                }
            }
            else
            {
                List<XMLFile> temp = new List<XMLFile>();
                foreach (var file in Directory.GetFiles(projectPath, "*.xml"))
                {
                    temp.Add(new XMLFile { FileName = Path.GetFileName(file) });
                }
                foreach (var file in RestoreCollection.ToList())
                {
                    if (!temp.Any(x => x.FileName.Equals(file.FileName)))
                    {
                        RestoreCollection.Remove(file);
                    }
                }
            }
        }
        private void UpdateRestoreCollectionOnRemoveProject()
        {
            if (ToProjectsCollection.Count == 0)
            {
                RestoreCollection.Clear();
            }
            else if (ToProjectsCollection.Count == 1)
            {
                string projectPath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name, ToProjectsCollection.ElementAt(0).Name);
                foreach (var file in Directory.GetFiles(projectPath, "*.xml"))
                {
                    if (!RestoreCollection.Any(x => x.FileName.Equals(Path.GetFileName(file))))
                    {
                        RestoreCollection.Add(new XMLFile { FileName = Path.GetFileName(file) });
                    }
                }
            }
            else
            {
                RestoreCollection.Clear();
                List<string> fileName = new List<string>();
                List<string> temp = new List<string>();
                string projectPath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name);
                foreach (var file in Directory.GetFiles(Path.Combine(projectPath, ToProjectsCollection.ElementAt(0).Name), "*.xml"))
                {
                    fileName.Add(Path.GetFileName(file));
                }
                foreach (var project in ToProjectsCollection.ToList())
                {

                    foreach (string file in fileName)
                    {
                        if (temp.Contains(file)) continue;
                        if (!Directory.GetFiles(Path.Combine(projectPath, project.Name), "*.xml").Any(x => Path.GetFileName(x).Equals(file)))
                        {
                            temp.Add(file);
                        }
                    }
                }
                foreach (string file in fileName)
                {
                    if (temp.Contains(file)) continue;
                    RestoreCollection.Add(new XMLFile { FileName= file });
                }

            }
        }


    }
}