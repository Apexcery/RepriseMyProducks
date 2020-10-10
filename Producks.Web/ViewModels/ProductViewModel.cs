using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Producks.Web.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        [DisplayName("Stock Status")]
        public bool InStock { get; set; }
        [DisplayName("Category")]
        public string CategoryName { get; set; }
        [DisplayName("Brand")]
        public string BrandName { get; set; }
        public bool Undercutters { get; set; }
    }
}
