using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Producks.Web.Models
{
    public class StoreDto
    {
        [Required, DisplayName("Category")]
        public int CategoryId { get; set; }
        [Required, DisplayName("Brand")]
        public int BrandId { get; set; }
    }
}
