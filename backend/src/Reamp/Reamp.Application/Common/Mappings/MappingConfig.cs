using Mapster;
using Reamp.Application.Accounts.Agencies.Dtos;
using Reamp.Application.Accounts.Clients.Dtos;
using Reamp.Application.Accounts.Staff.Dtos;
using Reamp.Application.Delivery.Dtos;
using Reamp.Application.Orders.Dtos;
using Reamp.Application.UserProfiles.Dtos;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Delivery.Entities;
using Reamp.Domain.Shoots.Entities;

namespace Reamp.Application.Common.Mappings
{
    /// <summary>
    /// Mapster mapping configuration
    /// </summary>
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {
            // UserProfile mappings
            TypeAdapterConfig<UserProfile, UserProfileDto>.NewConfig();

            // Agency mappings
            TypeAdapterConfig<Agency, AgencyDetailDto>.NewConfig()
                .Map(dest => dest.BranchCount, src => src.Branches.Count);

            TypeAdapterConfig<Agency, AgencyListDto>.NewConfig();

            TypeAdapterConfig<AgencyBranch, AgencyBranchDetailDto>.NewConfig();

            // Client mappings
            TypeAdapterConfig<Client, ClientDetailDto>.NewConfig();

            // Staff mappings
            TypeAdapterConfig<Staff, StaffDetailDto>.NewConfig();

            // Order mappings
            TypeAdapterConfig<ShootOrder, OrderListDto>.NewConfig()
                .Map(dest => dest.TaskCount, src => src.Tasks.Count);

            TypeAdapterConfig<ShootOrder, OrderDetailDto>.NewConfig()
                .Map(dest => dest.Tasks, src => src.Tasks);

            TypeAdapterConfig<ShootTask, TaskDetailDto>.NewConfig();

            // Delivery mappings
            TypeAdapterConfig<DeliveryPackage, DeliveryPackageDetailDto>.NewConfig()
                .Map(dest => dest.Items, src => src.Items)
                .Map(dest => dest.Accesses, src => src.Accesses);

            TypeAdapterConfig<DeliveryPackage, DeliveryPackageListDto>.NewConfig()
                .Map(dest => dest.ItemCount, src => src.Items.Count);

            TypeAdapterConfig<DeliveryItem, DeliveryItemDto>.NewConfig();

            TypeAdapterConfig<DeliveryAccess, DeliveryAccessDto>.NewConfig()
                .Map(dest => dest.HasPassword, src => !string.IsNullOrWhiteSpace(src.PasswordHash));
        }
    }
}

