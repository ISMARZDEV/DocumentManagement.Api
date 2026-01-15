using AutoMapper;
using Bhd.Application.DTOs.DocumentDTOs;
using Bhd.Domain.Constants;
using Bhd.Domain.Entities;

namespace Bhd.Application.Mappings
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<Document, DocumentDto>()
                .ForMember(dest => dest.UploadDate, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.FileUrl));

            CreateMap<DocumentSearchDto, DocumentSearchCriteria>()
                .ForMember(dest => dest.SortBy, opt => opt.MapFrom(src => src.SortBy))
                .ForMember(dest => dest.SortDirection, opt => opt.MapFrom(src => src.SortDirection))
                .ForMember(dest => dest.Page, opt => opt.MapFrom(src => src.Page))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize));
        }
    }
}
