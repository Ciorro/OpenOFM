using OpenOFM.Core.Api.DTO;
using OpenOFM.Core.Models;

namespace OpenOFM.Core.Extensions
{
    internal static class ModelExtensions
    {
        public static RadioCategory ToModel(this RadioCategoryDTO categoryDTO)
        {
            return new RadioCategory
            {
                Id = categoryDTO.Id,
                Name = categoryDTO.Name,
                Slug = categoryDTO.Slug
            };
        }

        public static RadioStation ToModel(this RadioStationDTO radioStationDTO)
        {
            return new RadioStation
            {
                Id = radioStationDTO.Id,
                Name = radioStationDTO.Name,
                Slug = radioStationDTO.Slug,
                StreamUrl = radioStationDTO.StreamUrl,
                CoverUrl = radioStationDTO.CoverUrl,
                IsPremium = radioStationDTO.IsPremium
            };
        }
    }
}
