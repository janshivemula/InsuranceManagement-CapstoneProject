using AutoMapper;
using InsuranceManagementSystem.DTOs.Common;
using InsuranceManagementSystem.DTOs.Policy;
using InsuranceManagementSystem.Enums;
using InsuranceManagementSystem.Exceptions;
using InsuranceManagementSystem.Models;
using InsuranceManagementSystem.Repositories.Interfaces;
using InsuranceManagementSystem.Services.Interfaces;

namespace InsuranceManagementSystem.Services.Implementations
{
    public class PolicyService : IPolicyService
    {
        private readonly IPolicyRepository _policyRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPolicyPlanRepository _planRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PolicyService> _logger;

        public PolicyService(
            IPolicyRepository policyRepository,
            ICustomerRepository customerRepository,
            IPolicyPlanRepository planRepository,
            IMapper mapper,
            ILogger<PolicyService> logger)
        {
            _policyRepository = policyRepository;
            _customerRepository = customerRepository;
            _planRepository = planRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // Get all policies
        public async Task<PagedResponse<PolicyResponseDto>> GetAllPoliciesAsync(PolicyQueryDto query)
        {
            var result = await _policyRepository.GetAllAsync(query);

            _logger.LogInformation("Retrieved policy list. Page={Page}, PageSize={PageSize}",
                query.PageNumber, query.PageSize);

            foreach (var policy in result.Items)
            {
                await UpdatePolicyStatusIfExpiredAsync(policy);
            }

            return new PagedResponse<PolicyResponseDto>
            {
                Records = _mapper.Map<IEnumerable<PolicyResponseDto>>(result.Items),
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = result.TotalRecords,
                TotalPages = (int)Math.Ceiling((double)result.TotalRecords / query.PageSize),
                IsLastPage = query.PageNumber >= (int)Math.Ceiling((double)result.TotalRecords / query.PageSize),
                SortField = query.SortBy,
                SortDirection = query.SortDirection
            };
        }

        // Get policies by customer
        public async Task<IEnumerable<PolicyResponseDto>> GetPoliciesByCustomerIdAsync(int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
                throw new NotFoundException("Customer not found.");

            var policies = await _policyRepository.GetPoliciesByCustomerIdAsync(customerId);

            foreach (var policy in policies)
            {
                await UpdatePolicyStatusIfExpiredAsync(policy);
            }

            return _mapper.Map<IEnumerable<PolicyResponseDto>>(policies);
        }

        // Get active policies of customer
        public async Task<IEnumerable<PolicyResponseDto>> GetActivePoliciesByCustomerIdAsync(int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
                throw new NotFoundException("Customer not found.");

            if (!customer.IsActive)
                throw new BadRequestException("Customer is inactive.");

            var policies = await _policyRepository.GetPoliciesByCustomerIdAsync(customerId);

            foreach (var policy in policies)
            {
                await UpdatePolicyStatusIfExpiredAsync(policy);
            }

            var activePolicies = policies.Where(p => p.PolicyStatus == PolicyStatus.Active);

            return _mapper.Map<IEnumerable<PolicyResponseDto>>(activePolicies);
        }

        public async Task<IEnumerable<PolicyResponseDto>> GetMyPoliciesAsync(int userId)
        {
            var customer = await _customerRepository.GetByUserIdAsync(userId);

            if (customer == null)
                throw new NotFoundException("Customer not found.");

            var policies = await _policyRepository.GetPoliciesByUserIdAsync(userId);

            foreach (var policy in policies)
            {
                await UpdatePolicyStatusIfExpiredAsync(policy);
            }

            return _mapper.Map<IEnumerable<PolicyResponseDto>>(policies);
        }

        // Get all active policies
        public async Task<IEnumerable<PolicyResponseDto>> GetActivePoliciesAsync()
        {
            var policies = await _policyRepository.GetActivePoliciesAsync();

            foreach (var policy in policies)
            {
                await UpdatePolicyStatusIfExpiredAsync(policy);
            }

            return _mapper.Map<IEnumerable<PolicyResponseDto>>(policies);
        }

        // Get policy by Id
        public async Task<PolicyResponseDto> GetPolicyByIdAsync(int policyId, int loggedInUserId, string role)
        {
            var policy = await _policyRepository.GetByIdAsync(policyId);

            if (policy == null)
                throw new NotFoundException("Policy not found.");

            _logger.LogInformation("Policy {PolicyId} retrieved.", policy.PolicyId);

            await UpdatePolicyStatusIfExpiredAsync(policy);

            if (role == "Customer" && policy.Customer.UserId != loggedInUserId)
                throw new UnauthorizedAccessException("You can only view your own policy.");

            return _mapper.Map<PolicyResponseDto>(policy);
        }

        // Get policy by Policy Number
        public async Task<PolicyResponseDto> GetPolicyByPolicyNumberAsync(string policyNumber, int loggedInUserId, string role)
        {
            if (string.IsNullOrWhiteSpace(policyNumber))
                throw new BadRequestException("Policy number is required.");

            var policy = await _policyRepository.GetByPolicyNumberAsync(policyNumber.Trim());

            if (policy == null)
                throw new NotFoundException("Policy not found.");

            await UpdatePolicyStatusIfExpiredAsync(policy);

            if (role == "Customer" && policy.Customer.UserId != loggedInUserId)
                throw new UnauthorizedAccessException("You can only view your own policy.");

            return _mapper.Map<PolicyResponseDto>(policy);
        }

        // Customer purchases policy
        public async Task<PolicyResponseDto> PurchasePolicyAsync(int customerUserId, PurchasePolicyRequestDto requestDto)
        {
            var customer = await _customerRepository.GetByUserIdAsync(customerUserId);

            if (customer == null)
                throw new NotFoundException("Customer not found.");

            if (!customer.IsActive)
                throw new BadRequestException("Inactive customer cannot purchase policies.");

            if (customer.DateOfBirth == default ||
                string.IsNullOrWhiteSpace(customer.Address) ||
                string.IsNullOrWhiteSpace(customer.City) ||
                string.IsNullOrWhiteSpace(customer.State) ||
                string.IsNullOrWhiteSpace(customer.PinCode) ||
                string.IsNullOrWhiteSpace(customer.NomineeName) ||
                string.IsNullOrWhiteSpace(customer.NomineeRelation))
            {
                throw new BadRequestException("Please complete your customer profile before purchasing a policy.");
            }

            var plan = await _planRepository.GetByIdAsync(requestDto.PlanId);

            if (plan == null)
                throw new NotFoundException("Policy plan not found.");

            if (!plan.IsActive)
                throw new BadRequestException("Policy plan is inactive.");

            if (!plan.InsuranceProduct.IsActive)
                throw new BadRequestException("Insurance product is inactive.");

            var existingPolicies = await _policyRepository.GetPoliciesByCustomerIdAsync(customer.CustomerId);

            if (existingPolicies.Any(p =>
                p.PlanId == requestDto.PlanId &&
                p.PolicyStatus != PolicyStatus.Cancelled &&
                p.PolicyStatus != PolicyStatus.Expired))
            {
                throw new ConflictException("Customer already owns this policy.");
            }

            if (requestDto.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
                throw new BadRequestException("Start date cannot be in the past.");

            var policy = new Policy
            {
                PolicyNumber = await GeneratePolicyNumberAsync(),
                CustomerId = customer.CustomerId,
                InsuranceProductId = plan.InsuranceProductId,
                PlanId = plan.PlanId,
                StartDate = requestDto.StartDate,
                EndDate = requestDto.StartDate.AddYears(plan.DurationInYears),
                PolicyStatus = PolicyStatus.PendingPayment,
                TotalPremiumPaid = 0,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            await _policyRepository.AddAsync(policy);
            await _policyRepository.SaveChangesAsync();

            _logger.LogInformation("Policy {PolicyNumber} purchased by Customer {CustomerId}.",
                policy.PolicyNumber, customer.CustomerId);

            var createdPolicy = await _policyRepository.GetByIdAsync(policy.PolicyId);

            return _mapper.Map<PolicyResponseDto>(createdPolicy);
        }

        // Internal Staff issues policy
        public async Task<PolicyResponseDto> IssuePolicyByInternalStaffAsync(IssuePolicyRequestDto requestDto)
        {
            var customer = await _customerRepository.GetByIdAsync(requestDto.CustomerId);

            if (customer == null)
                throw new NotFoundException("Customer not found.");

            if (!customer.IsActive)
                throw new BadRequestException("Inactive customer cannot receive policies.");

            if (customer.DateOfBirth == default ||
                string.IsNullOrWhiteSpace(customer.Address) ||
                string.IsNullOrWhiteSpace(customer.City) ||
                string.IsNullOrWhiteSpace(customer.State) ||
                string.IsNullOrWhiteSpace(customer.PinCode) ||
                string.IsNullOrWhiteSpace(customer.NomineeName) ||
                string.IsNullOrWhiteSpace(customer.NomineeRelation))
            {
                throw new BadRequestException("Customer profile is incomplete.");
            }

            var plan = await _planRepository.GetByIdAsync(requestDto.PlanId);

            if (plan == null)
                throw new NotFoundException("Policy plan not found.");

            if (!plan.IsActive)
                throw new BadRequestException("Policy plan is inactive.");

            if (!plan.InsuranceProduct.IsActive)
                throw new BadRequestException("Insurance product is inactive.");

            var existingPolicies = await _policyRepository.GetPoliciesByCustomerIdAsync(customer.CustomerId);

            if (existingPolicies.Any(p =>
                p.PlanId == requestDto.PlanId &&
                p.PolicyStatus != PolicyStatus.Cancelled &&
                p.PolicyStatus != PolicyStatus.Expired))
            {
                throw new ConflictException("Customer already owns this policy.");
            }

            if (requestDto.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
                throw new BadRequestException("Start date cannot be in the past.");

            var policy = new Policy
            {
                PolicyNumber = await GeneratePolicyNumberAsync(),
                CustomerId = customer.CustomerId,
                InsuranceProductId = plan.InsuranceProductId,
                PlanId = plan.PlanId,
                StartDate = requestDto.StartDate,
                EndDate = requestDto.StartDate.AddYears(plan.DurationInYears),
                PolicyStatus = PolicyStatus.PendingPayment,
                TotalPremiumPaid = 0,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            await _policyRepository.AddAsync(policy);
            await _policyRepository.SaveChangesAsync();

            _logger.LogInformation("Policy {PolicyNumber} issued to Customer {CustomerId}.",
                policy.PolicyNumber, customer.CustomerId);

            var createdPolicy = await _policyRepository.GetByIdAsync(policy.PolicyId);

            return _mapper.Map<PolicyResponseDto>(createdPolicy);
        }

        public async Task<PolicyResponseDto> CancelPolicyAsync(int policyId)
        {
            var policy = await _policyRepository.GetByIdAsync(policyId);

            if (policy == null)
                throw new NotFoundException("Policy not found.");

            if (policy.PolicyStatus == PolicyStatus.Cancelled)
                throw new BadRequestException("Policy is already cancelled.");

            if (policy.PolicyStatus == PolicyStatus.Expired)
                throw new BadRequestException("Expired policy cannot be cancelled.");

            policy.PolicyStatus = PolicyStatus.Cancelled;
            policy.UpdatedDate = DateTime.UtcNow;

            await _policyRepository.UpdateAsync(policy);
            await _policyRepository.SaveChangesAsync();

            _logger.LogInformation("Policy {PolicyNumber} cancelled.", policy.PolicyNumber);

            return _mapper.Map<PolicyResponseDto>(policy);
        }

        private async Task UpdatePolicyStatusIfExpiredAsync(Policy policy)
        {
            if (policy.PolicyStatus == PolicyStatus.Active &&
                policy.EndDate < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                policy.PolicyStatus = PolicyStatus.Expired;
                policy.UpdatedDate = DateTime.UtcNow;

                await _policyRepository.UpdateAsync(policy);
                await _policyRepository.SaveChangesAsync();

                _logger.LogInformation("Policy {PolicyNumber} automatically marked as Expired.", policy.PolicyNumber);
            }
        }

        private async Task<string> GeneratePolicyNumberAsync()
        {
            string policyNumber;
            bool exists;

            do
            {
                policyNumber = $"POL{DateTime.UtcNow:yyMMddHHmmss}";
                exists = await _policyRepository.GetByPolicyNumberAsync(policyNumber) != null;
            }
            while (exists);

            return policyNumber;
        }
    }
}