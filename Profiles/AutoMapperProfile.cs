using AutoMapper;
using InsuranceManagementSystem.DTOs.Auth;
using InsuranceManagementSystem.DTOs.Claim;
using InsuranceManagementSystem.DTOs.Customer;
using InsuranceManagementSystem.DTOs.Plan;
using InsuranceManagementSystem.DTOs.Policy;
using InsuranceManagementSystem.DTOs.PremiumPayment;
using InsuranceManagementSystem.DTOs.Product;
using InsuranceManagementSystem.DTOs.User;
using InsuranceManagementSystem.Enums;
using InsuranceManagementSystem.Models;

namespace InsuranceManagementSystem.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // =========================
            // USER
            // =========================

            CreateMap<User, UserResponseDto>()
                .ForMember(dest => dest.Role,
                    opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<RegisterRequestDto, User>();

            CreateMap<CreateAdminRequestDto, User>();

            CreateMap<CreateInternalStaffRequestDto, User>();


            // =========================
            // CUSTOMER
            // =========================

            CreateMap<CustomerRequestDto, Customer>()
                .ForMember(dest => dest.DateOfBirth,
                    opt => opt.MapFrom(src => DateOnly.FromDateTime(src.DateOfBirth)));

            CreateMap<Customer, CustomerResponseDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.MobileNumber,
                    opt => opt.MapFrom(src => src.User.MobileNumber))
                .ForMember(dest => dest.DateOfBirth,
                    opt => opt.MapFrom(src => src.DateOfBirth.ToDateTime(TimeOnly.MinValue)));


            // =========================
            // INSURANCE PRODUCT
            // =========================

            CreateMap<ProductRequestDto, InsuranceProduct>()
                .ForMember(dest => dest.ProductType,
                    opt => opt.MapFrom(src => Enum.Parse<ProductType>(src.ProductType, true)));

            CreateMap<InsuranceProduct, ProductResponseDto>()
                .ForMember(dest => dest.ProductType,
                    opt => opt.MapFrom(src => src.ProductType.ToString()));


            // =========================
            // POLICY PLAN
            // =========================

            CreateMap<PlanRequestDto, PolicyPlan>();

            CreateMap<PolicyPlan, PlanResponseDto>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.InsuranceProduct.ProductName))
                .ForMember(dest => dest.ProductType,
                    opt => opt.MapFrom(src => src.InsuranceProduct.ProductType.ToString()));


            // =========================
            // POLICY
            // =========================

            CreateMap<Policy, PolicyResponseDto>()
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.Customer.User.FullName))
                .ForMember(dest => dest.CustomerEmail,
                    opt => opt.MapFrom(src => src.Customer.User.Email))
                .ForMember(dest => dest.PlanName,
                    opt => opt.MapFrom(src => src.Plan.PlanName))
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.InsuranceProduct.ProductName))
                .ForMember(dest => dest.ProductType,
                    opt => opt.MapFrom(src => src.InsuranceProduct.ProductType))
                .ForMember(dest => dest.CoverageAmount,
                    opt => opt.MapFrom(src => src.Plan.CoverageAmount))
                .ForMember(dest => dest.PremiumAmount,
                    opt => opt.MapFrom(src => src.Plan.PremiumAmount))
                .ForMember(dest => dest.PremiumType,
                    opt => opt.MapFrom(src => src.Plan.PremiumType))
                .ForMember(dest => dest.StartDate,
                    opt => opt.MapFrom(src => src.StartDate.ToDateTime(TimeOnly.MinValue)))
                .ForMember(dest => dest.EndDate,
                    opt => opt.MapFrom(src => src.EndDate.ToDateTime(TimeOnly.MinValue)));


            // =========================
            // PREMIUM PAYMENT
            // =========================

            CreateMap<PremiumPaymentRequestDto, PremiumPayment>();

            CreateMap<PremiumPayment, PremiumPaymentResponseDto>()
                .ForMember(dest => dest.PolicyNumber,
                    opt => opt.MapFrom(src => src.Policy.PolicyNumber));


            // =========================
            // CLAIM DOCUMENT
            // =========================

            CreateMap<ClaimDocumentRequestDto, ClaimDocument>()
    .ForMember(dest => dest.DocumentReference,
        opt => opt.Ignore());

            CreateMap<ClaimDocument, ClaimDocumentResponseDto>()
                .ForMember(dest => dest.FilePath,
                    opt => opt.MapFrom(src => src.DocumentReference));


            // =========================
            // CLAIM STATUS HISTORY
            // =========================

            CreateMap<ClaimStatusHistory, ClaimStatusHistoryResponseDto>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.HistoryId))
                .ForMember(dest => dest.UpdatedByUserId,
                    opt => opt.MapFrom(src => src.UpdatedBy))
                .ForMember(dest => dest.UpdatedByUserName,
                    opt => opt.MapFrom(src => src.User.FullName));


            // =========================
            // CLAIM
            // =========================

            CreateMap<ClaimRequestDto, Claim>()
                .ForMember(dest => dest.IncidentDate,
                    opt => opt.MapFrom(src => DateOnly.FromDateTime(src.IncidentDate)))
                .ForMember(dest => dest.ClaimDocuments,
                    opt => opt.MapFrom(src => src.Documents));

            CreateMap<Claim, ClaimResponseDto>()
                .ForMember(dest => dest.PolicyNumber,
                    opt => opt.MapFrom(src => src.Policy.PolicyNumber))
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.Customer.User.FullName))
                .ForMember(dest => dest.IncidentDate,
                    opt => opt.MapFrom(src => src.IncidentDate.ToDateTime(TimeOnly.MinValue)))
                .ForMember(dest => dest.Documents,
                    opt => opt.MapFrom(src => src.ClaimDocuments))
                .ForMember(dest => dest.StatusHistory,
                    opt => opt.MapFrom(src => src.ClaimStatusHistories));
        }
    }
}