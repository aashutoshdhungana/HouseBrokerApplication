# HouseBrokerApplication

## Overview

HouseBrokerApplication is a modular .NET 9 solution for managing real estate listings, offers, deals, and commissions. It is designed with clean architecture principles, separating concerns into API, Application, Domain, and Infrastructure layers. The system supports JWT authentication, role-based access, and configurable commission slabs.

---

## Project Structure

- **API**  
  ASP.NET Core REST API. Handles HTTP requests, authentication (JWT), and exposes endpoints for listings, offers, deals, images, and user management.

- **Application**  
  Contains service interfaces and implementations, DTOs, validation logic, and business workflows. Orchestrates domain logic and external service calls.

- **Domain**  
  Core business entities (Listing, Offer, Deal, UserInfo, etc.), domain logic, and query specifications. Enforces business rules and aggregates.

- **Infrastructure**  
  Handles database access (EF Core, MySQL), external services (file storage), and identity management. Implements repository and unit of work patterns.

---

## Main Features

- **Listing Management**  
  - Brokers can create, update, and delete property listings.
  - Listings include address, price, property type, contact info, and images.

- **Image Upload**  
  - Brokers can upload and manage listing images.
  - Set primary image, remove images.

- **Offer Workflow**  
  - Home seekers can place or update offers on listings.
  - Brokers cannot place offers on their own listings.

- **Commission Calculation**  
  - Estimated commission is calculated for each offer.
  - Configurable commission slabs (by price range) or fallback rates.

- **Deal Management**  
  - Brokers can accept offers, marking listings as sold and creating deals.
  - Deal details include buyer info, commission, and price.

- **Authentication & Authorization**  
  - JWT-based authentication.
  - Role-based access (Broker, HomeSeeker).
  - Secure endpoints for sensitive operations.

---

## App Flow

1. **Broker registers and logs in.**
2. **Broker creates a listing.**
3. **Broker uploads listing photos.**
4. **Home seeker registers, logs in, and places an offer.**
5. **Estimated commission is calculated for each offer.**
6. **Broker reviews offers and accepts one to close a deal.**
7. **Deal is recorded, listing status is updated.**

---

## Validation and Mapping

This project uses FluentValidation for request/DTO validation and Mapster for object mapping between domain entities and DTOs.

- FluentValidation
  - Purpose: Define expressive, testable validation rules for DTOs.
  - Usage: Create validator classes deriving from AbstractValidator<TRequest>, register them with DI (AddValidatorsFromAssembly) and inject IValidator<T> where needed. Services typically call validator.Validate(request) and short-circuit on failures returning a validation result.
  - Example: IValidator<CreateUpdateListingRequest> is injected into ListingService and used to validate create/update requests before business logic runs.

- Mapster
  - Purpose: Map domain entities to DTOs (and vice versa) with minimal code and good performance.
  - Usage: Configure mappings centrally (MapsterConfig) and use the Adapt<TDestination>() extension to convert objects, e.g. listing.Adapt<ListingResponse>().
  - Example: After creating or updating a Listing entity, ListingService maps it to ListingResponse before returning to the API layer.

How they fit together
- Typical service flow:
  1. Validate incoming request with FluentValidation.
  2. Execute domain logic and persist changes.
  3. Map resulting domain entities to DTOs with Mapster and return.

Testing notes
- Validators can be unit tested by invoking Validate and asserting ValidationResult and ValidationFailure details.
- Mapster mappings can be validated in tests using Mapster configuration validation or by asserting mapped values in unit tests.

Examples

- FluentValidation example (CreateUpdateListingRequestValidator):

```csharp
using FluentValidation;
using HouseBrokerApplication.Application.DTOs.Requests;

public class CreateUpdateListingRequestValidator : AbstractValidator<CreateUpdateListingRequest>
{
    public CreateUpdateListingRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.ContactEmail).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.ContactEmail));
        RuleFor(x => x.ContactPhone).NotEmpty();
        RuleFor(x => x.Address).NotNull();
    }
}
```

- Mapster example (MapsterConfig):

```csharp
using Mapster;
using HouseBrokerApplication.Domain.Aggregates.Listing;
using HouseBrokerApplication.Application.DTOs.Responses;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<Listing, ListingResponse>.NewConfig()
            .Map(dest => dest.BrokerFullName, src => src.Broker != null ? src.Broker.FullName : string.Empty)
            .Map(dest => dest.PrimaryImageUrl, src => src.Images.FirstOrDefault(i => i.IsPrimary) != null ? src.Images.First(i => i.IsPrimary).FileInfo.Url : string.Empty);

        TypeAdapterConfig<Listing, DetailedListing>.NewConfig();
    }
}
```

---

## Setup Instructions

1. **Install .NET 9 SDK**
   - [Download .NET 9](https://dotnet.microsoft.com/download/dotnet/9.0)

2. **Configure AppSettings**
   - Update `appsettings.Development.json` (or `appsettings.json`) with:
     - Database connection string
     - JWT configuration (issuer, audience, secret)
     - Client ID and secret for authentication

3. **Run Database Migrations**
   - Use EF Core tools to apply migrations:
     ```sh
     dotnet ef database update --project HouseBrokerApplication.Infrastructure
     ```

4. **Register Users**
   - Use API endpoints to register brokers and home seekers.

5. **Authentication**
   - Login via API to obtain JWT token.
   - Use token for authenticated requests.

6. **Commission Slabs**
   - Add commission slabs to the database for configurable commission rates.
   - If no slab matches, fallback rates are used.

---

## Testing

- Unit tests are provided for controllers and services.
- Run tests with:
```
  dotnet test
```

---

## API Endpoints (Summary)

- **Listings**
- `POST /api/listing` - Create listing (Broker only)
- `PUT /api/listing/{id}` - Update listing (Broker only)
- `DELETE /api/listing/{id}` - Delete listing (Broker only)
- `GET /api/listing/{id}` - Get listing details (Public)
- `GET /api/listing` - List all listings (Public)
- `POST /api/listing/{listingId}/image` - Upload image (Broker only)
- `PUT /api/listing/{listingId}/image/{imageId}/primary` - Set primary image (Broker only)
- `DELETE /api/listing/{listingId}/image/{imageId}` - Remove image (Broker only)
- `POST /api/listing/{listingId}/offer` - Add/update offer (Authenticated)
- `DELETE /api/listing/{listingId}/offer/{offerId}` - Remove offer (Broker or offer creator)
- `POST /api/listing/{listingId}/deal/accept/{offerId}` - Accept offer (Broker only)
- `PUT /api/listing/{listingId}/status/offmarket` - Mark as off-market (Broker only)

- **Auth**
- `POST /api/auth/register` - Register user
- `POST /api/auth/login` - Login and get JWT

---

## Configuration Tips

- Ensure your database is accessible and the connection string is correct.
- Set strong JWT secrets in configuration.
- Use HTTPS in production.
- Commission slabs can be managed via database or admin endpoints.

---