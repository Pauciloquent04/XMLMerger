using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
        public ObservableCollection<string> XMLStructure { get; set; }
        public List<string> Operations { get; set; }

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
                LoadXmlStructure();
            }
        }

        private string selectedXMLStructure;
        public string SelectedXMLStructure
        {
            get { return selectedXMLStructure; }
            set
            {
                selectedXMLStructure = value;
                OnPropertyChanged(nameof(SelectedXMLStructure));
            }
        }

        private string selectedOperation;
        public string SelectedOperation
        {
            get { return selectedOperation; }
            set
            {
                selectedOperation = value;
                OnPropertyChanged(nameof(SelectedOperation));
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
            XMLStructure = new ObservableCollection<string>();

            ToDatabases = new ObservableCollection<DB>();
            ToSites = new ObservableCollection<Site>();
            ToProjects = new ObservableCollection<Project>();

            Operations = new List<string>()
            {
                "Replace", "Add", "Update"
            };

            string mergerPath = "C:/XMLMerger";

            foreach (var directory in Directory.GetDirectories(mergerPath))
            {
                FromDatabases.Add(new DB { Name = Path.GetFileName(directory) });
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

        private void LoadXmlStructure()
        {
            XMLStructure.Clear();
            XMLStructure.Add("...Blank...");
            if (SelectedXMLFile != null)
            {
                switch (SelectedXMLFile.FileName.ToLower())
                {
                    case "students.xml":
                        XMLStructure.Add("Students");
                        XMLStructure.Add("Students > StudentDetail");
                        XMLStructure.Add("Students > StudentDetail > Personal");
                        XMLStructure.Add("Students > StudentDetail > Personal > Age");
                        XMLStructure.Add("Students > StudentDetail > Personal > Gender");
                        XMLStructure.Add("Students > StudentDetail > Personal > City");
                        XMLStructure.Add("Students > StudentDetail > Academic");
                        XMLStructure.Add("Students > StudentDetail > Academic > Tenth");
                        XMLStructure.Add("Students > StudentDetail > Academic > Twelfth");
                        XMLStructure.Add("Students > StudentDetail > Academic > UG");
                        XMLStructure.Add("Students > StudentDetail > Academic > PG");
                        XMLStructure.Add("Students > StudentDetail > Corporate");
                        XMLStructure.Add("Students > StudentDetail > Corporate > CompanyName");
                        XMLStructure.Add("Students > StudentDetail > Corporate > Address");
                        XMLStructure.Add("Students > StudentDetail > Corporate > Position");
                        XMLStructure.Add("Students > StudentDetail > Corporate > ID");
                        break;

                    case "catalog.xml":
                        XMLStructure.Add("catalog");
                        XMLStructure.Add("catalog > product");
                        XMLStructure.Add("catalog > product > catalog_item");
                        XMLStructure.Add("catalog > product > catalog_item > item_number");
                        XMLStructure.Add("catalog > product > catalog_item > price");
                        XMLStructure.Add("catalog > product > catalog_item > size");
                        XMLStructure.Add("catalog > product > catalog_item > size > color_swatch");
                        break;

                    default:
                        break;
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


//private ObservableCollection<T> GetDirectories<T>(string path, char type, ObservableCollection<T> fromDatabases)
//{
//    ObservableCollection <T> lstDirectories = new ObservableCollection<T> ();
//    switch (type)
//    {
//        case 'D':

//            foreach (var directory in Directory.GetDirectories(path))
//            {
//                lstDirectories.Add(new <T> { Name = Path.GetFileName(directory) });
//            }

//            return fromDatabases;
//            break;
//        case 'S':
//            FromSites = new ObservableCollection<Site>();
//            break;
//        case 'P':
//            FromProjects = new ObservableCollection<Project>();
//            break;
//    }
//}

