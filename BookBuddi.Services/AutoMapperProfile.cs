using AutoMapper;
using BookBuddi.Data.Models;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Book mappings
            CreateMap<Book, BookViewModel>().ReverseMap()
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedTime, opt => opt.Ignore());

            // Author mappings
            CreateMap<Author, AuthorViewModel>().ReverseMap()
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedTime, opt => opt.Ignore());

            // Member mappings
            CreateMap<Member, MemberViewModel>().ReverseMap()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Never map password hash from ViewModel
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedTime, opt => opt.Ignore());

            // Category mappings
            CreateMap<Category, CategoryViewModel>().ReverseMap()
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedTime, opt => opt.Ignore());

            // Genre mappings
            CreateMap<Genre, GenreViewModel>().ReverseMap()
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedTime, opt => opt.Ignore());

            // BorrowTransaction mappings
            CreateMap<BorrowTransaction, BorrowTransactionViewModel>().ReverseMap()
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedTime, opt => opt.Ignore());

            // Fine mappings
            CreateMap<Fine, FineViewModel>().ReverseMap()
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedTime, opt => opt.Ignore());

            // BookRequest mappings
            CreateMap<BookRequest, BookRequestViewModel>().ReverseMap()
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedTime, opt => opt.Ignore());

            // Notification mappings
            CreateMap<Notification, NotificationViewModel>().ReverseMap()
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedTime, opt => opt.Ignore());
        }
    }
}
