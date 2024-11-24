using AutoMapper;
using ERP.api.sales.dtos;
using ERP.models.sales;

public class SaleMappingProfile : Profile
{
    public SaleMappingProfile()
    {
        // Map SaleDto to Sale and vice versa
        CreateMap<SaleDto, Sale>()
            .ForMember(dest => dest.ItemsSold, opt => opt.MapFrom(src => src.ItemsSold));  // Map ItemsSold

        // Map SaleItemDto to SaleItem and vice versa
        CreateMap<SaleItemDto, SaleItem>()
             .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.ItemId));  // Correctly map ItemId
    }
}
