using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reamp.Application.Applications.Dtos;
using Reamp.Domain.Accounts.Entities;
using Reamp.Domain.Accounts.Enums;
using Reamp.Domain.Accounts.Repositories;
using Reamp.Domain.Common.Abstractions;
using Reamp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Reamp.Application.Applications.Services
{
    public sealed class ApplicationService : IApplicationService
    {
        private readonly IOrganizationApplicationRepository _applicationRepo;
        private readonly IUserProfileRepository _userProfileRepo;
        private readonly IAgencyRepository _agencyRepo;
        private readonly IStudioRepository _studioRepo;
        private readonly IAgentRepository _agentRepo;
        private readonly IStaffRepository _staffRepo;
        private readonly IUnitOfWork _uow;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ApplicationService> _logger;

        public ApplicationService(
            IOrganizationApplicationRepository applicationRepo,
            IUserProfileRepository userProfileRepo,
            IAgencyRepository agencyRepo,
            IStudioRepository studioRepo,
            IAgentRepository agentRepo,
            IStaffRepository staffRepo,
            IUnitOfWork uow,
            ApplicationDbContext dbContext,
            ILogger<ApplicationService> logger)
        {
            _applicationRepo = applicationRepo;
            _userProfileRepo = userProfileRepo;
            _agencyRepo = agencyRepo;
            _studioRepo = studioRepo;
            _agentRepo = agentRepo;
            _staffRepo = staffRepo;
            _uow = uow;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Guid> SubmitAgencyApplicationAsync(Guid applicantUserId, SubmitAgencyApplicationDto dto, CancellationToken ct = default)
        {
            var hasPending = await _applicationRepo.HasPendingApplicationAsync(applicantUserId, ApplicationType.Agency, ct);
            if (hasPending)
                throw new InvalidOperationException("You already have a pending agency application");

            var application = OrganizationApplication.CreateAgencyApplication(
                applicantUserId,
                dto.OrganizationName,
                dto.ContactEmail,
                dto.ContactPhone,
                dto.Description
            );

            await _applicationRepo.AddAsync(application, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Agency application submitted by user {UserId}", applicantUserId);
            return application.Id;
        }

        public async Task<Guid> SubmitStudioApplicationAsync(Guid applicantUserId, SubmitStudioApplicationDto dto, CancellationToken ct = default)
        {
            var hasPending = await _applicationRepo.HasPendingApplicationAsync(applicantUserId, ApplicationType.Studio, ct);
            if (hasPending)
                throw new InvalidOperationException("You already have a pending studio application");

            var application = OrganizationApplication.CreateStudioApplication(
                applicantUserId,
                dto.OrganizationName,
                dto.ContactEmail,
                dto.ContactPhone,
                dto.Description,
                dto.Address?.ToValueObject()
            );

            await _applicationRepo.AddAsync(application, ct);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Studio application submitted by user {UserId}", applicantUserId);
            return application.Id;
        }

        public async Task<PagedResult<ApplicationListDto>> GetApplicationsAsync(
            PageRequest pageRequest,
            ApplicationStatus? status = null,
            ApplicationType? type = null,
            CancellationToken ct = default)
        {
            var pagedApplications = await _applicationRepo.GetPagedAsync(pageRequest, status, type, ct);

            var dtos = new List<ApplicationListDto>();
            foreach (var app in pagedApplications.Items)
            {
                var profile = await _userProfileRepo.GetByApplicationUserIdAsync(app.ApplicantUserId, true, false, ct);
                dtos.Add(new ApplicationListDto
                {
                    Id = app.Id,
                    Type = app.Type,
                    Status = app.Status,
                    OrganizationName = app.OrganizationName,
                    ContactEmail = app.ContactEmail,
                    ApplicantUserId = app.ApplicantUserId,
                    ApplicantEmail = profile?.FirstName + " " + profile?.LastName ?? "Unknown",
                    ApplicantName = profile?.FirstName + " " + profile?.LastName ?? "Unknown",
                    CreatedAtUtc = app.CreatedAtUtc,
                    ReviewedAtUtc = app.ReviewedAtUtc
                });
            }

            return new PagedResult<ApplicationListDto>(dtos, pagedApplications.Total, pagedApplications.Page, pagedApplications.PageSize);
        }

        public async Task<List<ApplicationListDto>> GetMyApplicationsAsync(Guid userId, CancellationToken ct = default)
        {
            var applications = await _applicationRepo.GetByApplicantAsync(userId, ct);
            var profile = await _userProfileRepo.GetByApplicationUserIdAsync(userId, true, false, ct);

            return applications.Select(app => new ApplicationListDto
            {
                Id = app.Id,
                Type = app.Type,
                Status = app.Status,
                OrganizationName = app.OrganizationName,
                ContactEmail = app.ContactEmail,
                ApplicantUserId = app.ApplicantUserId,
                ApplicantEmail = profile?.FirstName + " " + profile?.LastName ?? "Unknown",
                ApplicantName = profile?.FirstName + " " + profile?.LastName ?? "Unknown",
                CreatedAtUtc = app.CreatedAtUtc,
                ReviewedAtUtc = app.ReviewedAtUtc
            }).ToList();
        }

        public async Task<ApplicationDetailDto> GetApplicationDetailAsync(Guid applicationId, CancellationToken ct = default)
        {
            var application = await _applicationRepo.GetByIdAsync(applicationId, false, ct);
            if (application == null)
                throw new InvalidOperationException("Application not found");

            var applicantProfile = await _userProfileRepo.GetByApplicationUserIdAsync(application.ApplicantUserId, true, false, ct);
            
            string? reviewerName = null;
            if (application.ReviewedBy.HasValue)
            {
                var reviewerProfile = await _userProfileRepo.GetByApplicationUserIdAsync(application.ReviewedBy.Value, true, false, ct);
                reviewerName = reviewerProfile != null ? $"{reviewerProfile.FirstName} {reviewerProfile.LastName}" : "Unknown";
            }

            return new ApplicationDetailDto
            {
                Id = application.Id,
                Type = application.Type,
                Status = application.Status,
                ApplicantUserId = application.ApplicantUserId,
                ApplicantEmail = applicantProfile?.FirstName + " " + applicantProfile?.LastName ?? "Unknown",
                ApplicantName = applicantProfile?.FirstName + " " + applicantProfile?.LastName ?? "Unknown",
                OrganizationName = application.OrganizationName,
                Description = application.Description,
                ContactEmail = application.ContactEmail,
                ContactPhone = application.ContactPhone,
                Address = application.Address != null ? new AddressDto
                {
                    Street = application.Address.Street,
                    City = application.Address.City,
                    State = application.Address.State,
                    PostalCode = application.Address.PostalCode,
                    Country = application.Address.Country
                } : null,
                CreatedOrganizationId = application.CreatedOrganizationId,
                ReviewedBy = application.ReviewedBy,
                ReviewerName = reviewerName,
                ReviewedAtUtc = application.ReviewedAtUtc,
                ReviewNotes = application.ReviewNotes,
                CreatedAtUtc = application.CreatedAtUtc
            };
        }

        public async Task ReviewApplicationAsync(Guid applicationId, Guid reviewedBy, ReviewApplicationDto dto, CancellationToken ct = default)
        {
            var application = await _applicationRepo.GetByIdAsync(applicationId, true, ct);
            if (application == null)
                throw new InvalidOperationException("Application not found");

            if (!application.CanBeReviewed())
                throw new InvalidOperationException("Application cannot be reviewed in its current state");

            if (dto.Approved)
            {
                Guid createdOrgId;
                
                if (application.Type == ApplicationType.Agency)
                {
                    createdOrgId = await CreateAgencyFromApplicationAsync(application, ct);
                }
                else
                {
                    createdOrgId = await CreateStudioFromApplicationAsync(application, ct);
                }

                application.Approve(reviewedBy, createdOrgId, dto.Notes);
                _logger.LogInformation("Application {ApplicationId} approved, organization {OrgId} created", applicationId, createdOrgId);
            }
            else
            {
                application.Reject(reviewedBy, dto.Notes);
                _logger.LogInformation("Application {ApplicationId} rejected", applicationId);
            }

            await _uow.SaveChangesAsync(ct);
        }

        public async Task CancelApplicationAsync(Guid applicationId, Guid userId, CancellationToken ct = default)
        {
            var application = await _applicationRepo.GetByIdAsync(applicationId, true, ct);
            if (application == null)
                throw new InvalidOperationException("Application not found");

            if (application.ApplicantUserId != userId)
                throw new UnauthorizedAccessException("You can only cancel your own applications");

            if (!application.CanBeCancelled())
                throw new InvalidOperationException("Application cannot be cancelled in its current state");

            application.Cancel();
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Application {ApplicationId} cancelled by user {UserId}", applicationId, userId);
        }

        private async Task<Guid> CreateAgencyFromApplicationAsync(OrganizationApplication application, CancellationToken ct)
        {
            var ownerProfile = await _userProfileRepo.GetByApplicationUserIdAsync(application.ApplicantUserId, false, true, ct);
            if (ownerProfile == null)
                throw new InvalidOperationException("Applicant user profile not found");

            var agency = Agency.Create(
                name: application.OrganizationName,
                createdBy: application.ApplicantUserId,
                contactEmail: application.ContactEmail,
                contactPhone: application.ContactPhone,
                description: application.Description
            );

            var existingAgent = await _agentRepo.GetByUserProfileIdAsync(ownerProfile.Id, ct);
            if (existingAgent != null && existingAgent.DeletedAtUtc == null)
                throw new InvalidOperationException("User already belongs to an agency");

            await _agencyRepo.AddAsync(agency, ct);

            var ownerAgent = new Agent(ownerProfile.Id, agency.Id, AgencyRole.Owner);
            await _agentRepo.AddAsync(ownerAgent, ct);

            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Agency {AgencyName} created from application with owner {OwnerId}", 
                application.OrganizationName, application.ApplicantUserId);

            return agency.Id;
        }

        private async Task<Guid> CreateStudioFromApplicationAsync(OrganizationApplication application, CancellationToken ct)
        {
            var ownerProfile = await _userProfileRepo.GetByApplicationUserIdAsync(application.ApplicantUserId, false, true, ct);
            if (ownerProfile == null)
                throw new InvalidOperationException("Applicant user profile not found");

            var studio = Studio.Create(
                name: application.OrganizationName,
                createdBy: application.ApplicantUserId,
                contactEmail: application.ContactEmail,
                contactPhone: application.ContactPhone,
                description: application.Description,
                address: application.Address
            );

            var existingStaff = await _staffRepo.GetByUserProfileIdAsync(ownerProfile.Id, asNoTracking: false, ct);
            if (existingStaff != null && existingStaff.DeletedAtUtc == null)
                throw new InvalidOperationException("User already belongs to a studio");

            await _studioRepo.AddAsync(studio, ct);

            var ownerStaff = new Staff(ownerProfile.Id, studio.Id, StudioRole.Owner);
            await _staffRepo.AddAsync(ownerStaff, ct);

            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation("Studio {StudioName} created from application with owner {OwnerId}", 
                application.OrganizationName, application.ApplicantUserId);

            return studio.Id;
        }
    }
}
