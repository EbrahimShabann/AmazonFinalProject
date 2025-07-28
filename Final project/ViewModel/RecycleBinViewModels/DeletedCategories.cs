using Final_project.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace Final_project.ViewModel.RecycleBinViewModels
{
    public class DeletedCategories
    {
        public string id { get; set; }
        public string name { get; set; }
        public virtual string ParentCategory { get; set; }
        public virtual string DeletedByUser { get; set; }
        public string img_Url { get; set; }
        public string deletedByImg { get; set; }
    }
}