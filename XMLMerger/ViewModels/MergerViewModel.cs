using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XMLMerger.Commands;
using XMLMerger.Models;

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
        //public ObservableCollection<XMLFile> ToXMLFiles { get; set; }

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
            get { return selectedXMLFile; }
            set
            {
                selectedXMLFile = value;
                OnPropertyChanged(nameof(SelectedXMLFile));
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

        private Project selectedToProject;
        public Project SelectedToProject
        {
            get { return selectedToProject; }
            set
            {
                selectedToProject = value;
                OnPropertyChanged(nameof(SelectedToProject));
            }
        }


        public MergerViewModel()
        {
            FromDatabases = new ObservableCollection<DB>();
            FromSites = new ObservableCollection<Site>();
            FromProjects = new ObservableCollection<Project>();
            FromXMLFiles = new ObservableCollection<XMLFile>();

            ToDatabases = new ObservableCollection<DB>();
            ToSites = new ObservableCollection<Site>();
            ToProjects = new ObservableCollection<Project>();
            //FromXMLFiles = new ObservableCollection<XMLFile>();

            string mergerPath = "C:/XMLMerger";

            foreach (var directory in Directory.GetDirectories(mergerPath))
            {
                FromDatabases.Add(new DB { Name =  Path.GetFileName(directory) });
            }

            foreach (var directory in Directory.GetDirectories(mergerPath))
            {
                ToDatabases.Add(new DB { Name = Path.GetFileName(directory) });
            }
        }

        private void LoadFromSites()
        {    
            FromSites.Clear();
            if (SelectedFromDB != null)
            {
                string dbPath = Path.Combine("C:/XMLMerger", SelectedFromDB.Name, "SPProj");

                foreach (var directory in Directory.GetDirectories(dbPath))
                {
                    FromSites.Add(new Site { Name = Path.GetFileName(directory) });
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

        private void LoadToSites()
        {
            ToSites.Clear();
            if (SelectedToDB != null)
            {
                string dbPath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj");

                foreach (var directory in Directory.GetDirectories(dbPath))
                {
                    ToSites.Add(new Site { Name = Path.GetFileName(directory) });
                }
            }
        }

        private void LoadToProjects()
        {
            ToProjects.Clear();
            if (SelectedToSite != null)
            {
                string sitePath = Path.Combine("C:/XMLMerger", SelectedToDB.Name, "SPProj", SelectedToSite.Name);

                foreach (var directory in Directory.GetDirectories(sitePath))
                {
                    ToProjects.Add(new Project { Name = Path.GetFileName(directory) });
                }
            }
        }


    }
}


//public string[] DB { get; set; }
//public string[] Site { get; set; }
//public string[] Projects { get; set; }
//public string[] XmlFileList { get; set; }

/*
 * 
 * private string _fromDB;
        public string FromDB
        {
            get { return _fromDB; }
            set 
            { 
                _fromDB = value;
                OnPropertyChanged(nameof(FromDB));
            }
        }

        private string _toDB;
        public string ToDB
        {
            get { return _toDB; }
            set
            {
                _toDB = value;
                OnPropertyChanged(nameof(ToDB));
            }
        }

        private string _fromSite;
        public string FromSite
        {
            get { return _fromSite; }
            set
            {
                _fromSite = value;
                OnPropertyChanged(nameof(FromSite));
            }
        }

        private string _toSite;
        public string ToSite
        {
            get { return _toSite; }
            set
            {
                _toSite = value;
                OnPropertyChanged(nameof(ToSite));
            }
        }

        private string _sourceProject;
        public string SourceProject
        {
            get { return _sourceProject; }
            set
            {
                _sourceProject = value;
                OnPropertyChanged(nameof(SourceProject));
            }
        }

        private string _targetProject;
        public string TargetProject
        {
            get { return _targetProject; }
            set
            {
                _targetProject = value;
                OnPropertyChanged(nameof(TargetProject));
            }
        }

        private string _xmlFile;
        public string XmlFile
        {
            get { return _xmlFile; }
            set
            {
                _xmlFile = value;
                OnPropertyChanged(nameof(XmlFile));
            }
        }
 * 
 */
