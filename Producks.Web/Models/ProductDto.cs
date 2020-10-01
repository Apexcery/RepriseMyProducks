using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Producks.Web.Models
{
    public class ProductDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        [DisplayName("Category Name")]
        public string CategoryName { get; set; }
        public int BrandId { get; set; }
        [DisplayName("Brand Name")]
        public string BrandName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int StockLevel { get; set; }
    }
}
