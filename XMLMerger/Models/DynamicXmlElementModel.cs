using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLMerger.Models
{
    public class DynamicXmlElementModel
    {
        public string Name {  get; set; }
        public string Value { get; set; }
        public ObservableCollection<DynamicXmlElementModel> Children { get; set; } = new ObservableCollection<DynamicXmlElementModel>();
    }
}
