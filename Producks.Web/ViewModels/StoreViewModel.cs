using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Producks.Web.ViewModels
{
    public class StoreViewModel
    {
        [Required]
        public IEnumerable<CategoryViewModel> Categories { get; set; }
        [Required]
        public IEnumerable<BrandsViewModel> Brands { get; set; }
    }
}
