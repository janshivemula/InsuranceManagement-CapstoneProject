using AutoMapper;
using InsuranceManagementSystem.DTOs.Common;
using InsuranceManagementSystem.DTOs.PremiumPayment;
using InsuranceManagementSystem.Enums;
using InsuranceManagementSystem.Exceptions;
using InsuranceManagementSystem.Models;
using InsuranceManagementSystem.Repositories.Interfaces;
using InsuranceManagementSystem.Services.Interfaces;

public class PremiumPaymentService : IPremiumPaymentService
{
    private readonly IPremiumPaymentRepository _premiumPaymentRepository;
    private readonly IPolicyRepository _policyRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PremiumPaymentService> _logger;

    public PremiumPaymentService(
        IPremiumPaymentRepository premiumPaymentRepository,
        IPolicyRepository policyRepository,
        IMapper mapper,
        ILogger<PremiumPaymentService> logger)
    {
        _premiumPaymentRepository = premiumPaymentRepository;
        _policyRepository = policyRepository;
        _mapper = mapper;
        _logger = logger;
    }

    // Get all payments
    public async Task<PagedResponse<PremiumPaymentResponseDto>> GetAllPaymentsAsync(PaginationRequestDto paginationDto)
    {
        _logger.LogInformation("Retrieving premium payments. Page: {Page}, Size: {Size}",
            paginationDto.PageNumber, paginationDto.PageSize);

        var pagedPayments = await _premiumPaymentRepository.GetAllAsync(paginationDto);

        return new PagedResponse<PremiumPaymentResponseDto>
        {
            Records = _mapper.Map<IEnumerable<PremiumPaymentResponseDto>>(pagedPayments.Records),
            CurrentPage = pagedPayments.CurrentPage,
            PageSize = pagedPayments.PageSize,
            TotalRecords = pagedPayments.TotalRecords,
            TotalPages = pagedPayments.TotalPages,
            IsLastPage = pagedPayments.IsLastPage,
            SortField = pagedPayments.SortField,
            SortDirection = pagedPayments.SortDirection
        };
    }

    // Get payments by Policy Id
    public async Task<PagedResponse<PremiumPaymentResponseDto>> GetPaymentsByPolicyIdAsync(int policyId, PaginationRequestDto paginationDto)
    {
        var policy = await _policyRepository.GetByIdAsync(policyId);

        if (policy == null)
            throw new NotFoundException("Policy not found.");

        var pagedPayments = await _premiumPaymentRepository
            .GetPaymentsByPolicyIdAsync(policyId, paginationDto);

        return new PagedResponse<PremiumPaymentResponseDto>
        {
            Records = _mapper.Map<IEnumerable<PremiumPaymentResponseDto>>(pagedPayments.Records),
            CurrentPage = pagedPayments.CurrentPage,
            PageSize = pagedPayments.PageSize,
            TotalRecords = pagedPayments.TotalRecords,
            TotalPages = pagedPayments.TotalPages,
            IsLastPage = pagedPayments.IsLastPage,
            SortField = pagedPayments.SortField,
            SortDirection = pagedPayments.SortDirection
        };
    }

    // Get payments by Customer Id
    public async Task<PagedResponse<PremiumPaymentResponseDto>> GetPaymentsByCustomerIdAsync(
        int customerId,
        int userId,
        string role,
        PaginationRequestDto paginationDto)
    {
        var policies = await _policyRepository.GetPoliciesByCustomerIdAsync(customerId);

        if (!policies.Any())
        {
            return new PagedResponse<PremiumPaymentResponseDto>
            {
                Records = Enumerable.Empty<PremiumPaymentResponseDto>(),
                CurrentPage = paginationDto.PageNumber,
                PageSize = paginationDto.PageSize,
                TotalRecords = 0,
                TotalPages = 0,
                IsLastPage = true,
                SortField = paginationDto.SortBy,
                SortDirection = paginationDto.SortDirection
            };
        }
        // Customers can view only their own payments
        if (role == "Customer")
        {
            if (policies.First().Customer.UserId != userId)
            {
                throw new UnauthorizedAccessException("You can view only your own payment history.");
            }
        }

        var pagedPayments = await _premiumPaymentRepository
            .GetPaymentsByCustomerIdAsync(customerId, paginationDto);

        return new PagedResponse<PremiumPaymentResponseDto>
        {
            Records = _mapper.Map<IEnumerable<PremiumPaymentResponseDto>>(pagedPayments.Records),
            CurrentPage = pagedPayments.CurrentPage,
            PageSize = pagedPayments.PageSize,
            TotalRecords = pagedPayments.TotalRecords,
            TotalPages = pagedPayments.TotalPages,
            IsLastPage = pagedPayments.IsLastPage,
            SortField = pagedPayments.SortField,
            SortDirection = pagedPayments.SortDirection
        };
    }

    // Get payment by Id
    public async Task<PremiumPaymentResponseDto?> GetPaymentByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving payment with ID {PaymentId}", id);

        var payment = await _premiumPaymentRepository.GetByIdAsync(id);

        if (payment == null)
            return null;

        return _mapper.Map<PremiumPaymentResponseDto>(payment);
    }

    // Make Premium Payment
    public async Task<PremiumPaymentResponseDto> MakePaymentAsync(PremiumPaymentRequestDto requestDto, int userId, string role)
    {
        var policy = await _policyRepository.GetByIdAsync(requestDto.PolicyId);

        if (policy == null)
            throw new NotFoundException("Policy not found.");

        // Customer can pay only for their own policy
        if (role == "Customer")
        {
            if (policy.Customer.UserId != userId)
            {
                _logger.LogWarning("Unauthorized payment attempt. UserId: {UserId}, PolicyId: {PolicyId}",
                    userId, policy.PolicyId);

                throw new UnauthorizedAccessException("You can make payment only for your own policy.");
            }
        }

        // Customer must be active
        if (!policy.Customer.IsActive)
            throw new BadRequestException("Customer account is inactive.");

        // Plan must be active
        if (!policy.Plan.IsActive)
            throw new BadRequestException("Policy plan is inactive.");

        // Product must be active
        if (!policy.InsuranceProduct.IsActive)
            throw new BadRequestException("Insurance product is inactive.");

        // Policy cannot be cancelled
        if (policy.PolicyStatus == PolicyStatus.Cancelled)
            throw new BadRequestException("Cancelled policies cannot accept payments.");

        // Policy cannot be expired
        if (policy.PolicyStatus == PolicyStatus.Expired)
            throw new BadRequestException("Expired policies cannot accept payments.");

        // One-time premium can be paid only once
        if (policy.Plan.PremiumType == PremiumType.OneTime && policy.PolicyStatus == PolicyStatus.Active)
        {
            throw new BadRequestException("One-time premium has already been paid for this policy.");
        }

        // Validate amount
        if (requestDto.Amount <= 0)
            throw new BadRequestException("Payment amount must be greater than zero.");

        // Validate required premium amount
        if (requestDto.Amount < policy.Plan.PremiumAmount)
        {
            throw new BadRequestException($"Minimum premium amount is {policy.Plan.PremiumAmount}.");
        }

        // Validate transaction reference
        if (string.IsNullOrWhiteSpace(requestDto.TransactionReference))
            throw new BadRequestException("Transaction reference is required.");

        string transactionReference = requestDto.TransactionReference.Trim();

        // Check duplicate transaction reference
        var existingPayment = await _premiumPaymentRepository.GetByTransactionReferenceAsync(transactionReference);

        if (existingPayment != null)
        {
            _logger.LogWarning("Duplicate transaction reference detected: {TransactionReference}", transactionReference);
            throw new ConflictException("Transaction reference already exists.");
        }

        var payment = new PremiumPayment
        {
            CustomerId = policy.CustomerId,
            PolicyId = policy.PolicyId,
            Amount = requestDto.Amount,
            PaymentDate = DateTime.UtcNow,
            PaymentMode = requestDto.PaymentMode,
            TransactionReference = transactionReference,
            PaymentStatus = PaymentStatus.Success,
            CreatedDate = DateTime.UtcNow
        };

        _logger.LogInformation("Recording premium payment. PolicyId: {PolicyId}, Amount: {Amount}",
            payment.PolicyId, payment.Amount);

        await _premiumPaymentRepository.AddAsync(payment);

        // Update policy
        policy.TotalPremiumPaid += requestDto.Amount;

        if (policy.PolicyStatus == PolicyStatus.PendingPayment)
        {
            policy.PolicyStatus = PolicyStatus.Active;
        }

        policy.UpdatedDate = DateTime.UtcNow;

        await _policyRepository.UpdateAsync(policy);
        await _premiumPaymentRepository.SaveChangesAsync();

        _logger.LogInformation("Premium payment recorded successfully. PaymentId: {PaymentId}", payment.PaymentId);

        return _mapper.Map<PremiumPaymentResponseDto>(payment);
    }
}