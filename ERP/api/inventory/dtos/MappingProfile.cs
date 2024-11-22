using AutoMapper;
using ERP.models.inventory;

namespace ERP.api.inventory.dtos
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping from InventoryItem (Entity) to InventoryItemDto (DTO)
            CreateMap<InventoryItem, InventoryItemDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));  // Example of mapping enum to string

            // Mapping from InventoryItemDto (DTO) to InventoryItem (Entity)
            CreateMap<InventoryItemDto, InventoryItem>();
        }
    }
}
