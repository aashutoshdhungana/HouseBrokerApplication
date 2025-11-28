using HouseBrokerApplication.Application.DTOs.Responses;
using HouseBrokerApplication.Domain.Aggregates.Listing;
using Mapster;

namespace HouseBrokerApplication.Application.MapsterConfig
{
    public class MapsterConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Listing, ListingResponse>()
            .Map(dest => dest.BrokerFullName, src => src.Broker != null ? $"{src.Broker.FirstName} {src.Broker.LastName}" : "")
            .Map(dest => dest.PrimaryImageUrl,
                src => src.Images != null && src.Images.FirstOrDefault(x => x.IsPrimary) != null
                       ? src.Images.FirstOrDefault(x => x.IsPrimary).FileInfo != null
                         ? src.Images.FirstOrDefault(x => x.IsPrimary).FileInfo.Url
                         : null
                       : null);

            config.NewConfig<Deal, DealResponse>()
                        .Map(dest => dest.Id, src => src.Id.ToString())
                        .Map(dest => dest.OfferId, src => src.OfferId)
                        .Map(dest => dest.Commission, src => src.Commission)
                        .Map(dest => dest.DealDate, src => src.DealDate.ToString("yyyy-MM-dd"))
                        .Map(dest => dest.DealPrice, src => src.Offer != null ? src.Offer.OfferAmount : 0M)
                        .Map(dest => dest.BuyerFullName, src =>
                            src.Offer != null && src.Offer.Buyer != null
                                ? $"{src.Offer.Buyer.FirstName} {src.Offer.Buyer.LastName}"
                                : string.Empty)
                        .Map(dest => dest.BuyerEmail, src =>
                            src.Offer != null && src.Offer.Buyer != null
                                ? src.Offer.Buyer.ContactEmail
                                : string.Empty)
                        .Map(dest => dest.BuyerPhone, src =>
                            src.Offer != null && src.Offer.Buyer != null
                                ? src.Offer.Buyer.ContactPhone
                                : string.Empty);

            config.NewConfig<Offer, OfferResponse>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.BuyerId, src => src.BuyerId)
                .Map(dest => dest.OfferAmount, src => src.OfferAmount)
                .Map(dest => dest.BuyerName, src =>
                    src.Buyer != null
                        ? $"{src.Buyer.FirstName} {src.Buyer.LastName}"
                        : string.Empty);

            config.NewConfig<ListingImage, ImageResponse>()
                .Map(dest => dest.ImageName, src => src.FileInfo != null ? src.FileInfo.DisplayName : string.Empty)
                .Map(dest => dest.ImageUrl, src => src.FileInfo != null ? src.FileInfo.Url : string.Empty);
        }
    }
}
