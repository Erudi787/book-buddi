using AutoMapper;
using BookBuddi.Data.Models;
using BookBuddi.Services.ServiceModels;

namespace BookBuddi.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Book mappings - Forward mapping ignores navigation properties (they don't exist on Book model)
            CreateMap<Book, BookViewModel>()
                .ForMember(dest => dest.CategoryName, opt => opt.Ignore())
                .ForMember(dest => dest.GenreName, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorNames, opt => opt.Ignore());

            // Reverse mapping for creating/updating books
            CreateMap<BookViewModel, Book>()
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

            // Rating mappings
            CreateMap<Rating, RatingViewModel>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.BookTitle))
                .ForMember(dest => dest.BookCoverUrl, opt => opt.MapFrom(src => src.Book.CoverImageUrl))
                .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member.FirstName + " " + src.Member.LastName))
                .ForMember(dest => dest.RatedDate, opt => opt.MapFrom(src => src.CreatedTime));

            CreateMap<CreateRatingModel, Rating>()
                .ForMember(dest => dest.RatingId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedTime, opt => opt.Ignore())
                .ForMember(dest => dest.Book, opt => opt.Ignore())
                .ForMember(dest => dest.Member, opt => opt.Ignore());
        }
    }
}
