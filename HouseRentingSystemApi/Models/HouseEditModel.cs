using HouseRentingSystemApi.Models.Enums;
using System.ComponentModel.DataAnnotations;
using static HouseRentingSystemApi.Data.DataConstants.DataConstants.House;

namespace HouseRentingSystemApi.Models
{
    public class HouseEditModel
    {
        [Required]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; }

        [Required]
        [MaxLength(AddressMaxLength)]
        public string Address { get; set; }

        [Required]
        public string Description { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public decimal PricePerMonth { get; set; }

        public CategoryViewEnum Category { get; set; }
    }
}
