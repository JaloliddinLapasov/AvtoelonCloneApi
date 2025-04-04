using AvtoelonCloneApi.Dtos.AdDTOs;
using AvtoelonCloneApi.Models;

namespace AvtoelonCloneApi.Mappers
{
    public static class AdMappers
    {
        public static Ad ToAdFromAdDTO(this AdDto adDto)
        {
            return new Ad
            {
                Title = adDto.Title,
                Description = adDto.Description,
                Price = adDto.Price,
                Currency = adDto.Currency,
                Category = adDto.Category,
                Location = adDto.Location,
                ContactName = adDto.ContactName,
                ContactPhone = adDto.ContactPhone,
                ImagePath = adDto.ImagePath,
            };
        }
    }
}